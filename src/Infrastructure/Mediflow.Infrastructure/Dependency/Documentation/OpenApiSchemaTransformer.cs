using System.Numerics;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;

namespace Mediflow.Infrastructure.Dependency.Documentation;

/// <summary>
/// Single and unified OpenAPI Schema Transformer.
/// </summary>
internal sealed class OpenApiSchemaTransformer : IOpenApiSchemaTransformer
{
    private static readonly NullabilityInfoContext NullabilityContext = new();

    /// <summary>
    /// The following transformer handles the Normalization and Sanitization of OpenAPI Schemas Generated from CLR Types.
    /// Leaf Type: Primitive Schemas or Data Types such as String, Decimal, Enum and So On that Has NO Child Schemas.
    /// Normalization: Process of Converting a CLR Type to a More Specific and Accurate Representation and Consistent Structure.
    /// Sanitization: Process of Removing Inconsistent and Invalid Schema Properties that May Exist in the Generated Schema from the CLR Type.
    /// Ensures that the ENUMS are Represented as Strings, Numerics have the Correct Type and Format, Dictionaries are Represented as Objects with Additional Properties, Arrays are Properly Structured, and Required Properties are Set According to CLR Nullability.
    /// </summary>
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var clrType = GetClrType(context);

        var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;

        #region Sanitization
        if (schema.Type is not null && !schema.Type.Value.HasFlag(JsonSchemaType.Array) && schema.Items is not null)
            schema.Items = null;
        #endregion

        #region Type Normalization (Enumerations, Numerics, Dictionaries and Arrays)
        if (underlyingType.IsEnum)
        {
            NormalizeEnum(schema, clrType);
            return;
        }

        if (TryMapNumeric(underlyingType, out var jsonType, out string? format, out bool forceString))
        {
            NormalizeNumeric(schema, clrType, jsonType, format, forceString);
            return;
        }

        if (TryGetDictionaryValueType(underlyingType, out var valueType))
        {
            await NormalizeDictionaryAsync(schema, clrType, valueType, context, cancellationToken);
            return;
        }

        if (TryGetEnumerableElementType(underlyingType, out var elementType))
        {
            await NormalizeArrayAsync(schema, clrType, elementType, context, cancellationToken);
            return;
        }
        #endregion

        #region Object Required and Optional Normalization
        if (schema.Properties is null || schema.Properties.Count == 0)
            return;

        if (context.JsonTypeInfo.Kind != System.Text.Json.Serialization.Metadata.JsonTypeInfoKind.Object)
            return;

        ApplyRequiredFromClrNullability(schema, context);
        #endregion
    }

    #region Helpers: CLR Type Discovery
    /// <summary>
    /// Resolves the CLR Type (via JSON Property Type for Properties, otherwise via the JSON Type Info for Root Model Schemas).
    /// </summary>
    private static Type GetClrType(OpenApiSchemaTransformerContext context)
    {
        return context.JsonPropertyInfo is not null ? context.JsonPropertyInfo.PropertyType : context.JsonTypeInfo.Type;
    }
    #endregion

    #region Enums
    /// <summary>
    /// Forces an Enumeration CLR Type to STRING Format.
    /// </summary>
    private static void NormalizeEnum(OpenApiSchema schema, Type clrType)
    {
        bool isNullable = IsClrNullable(clrType, memberInfo: null);

        schema.Type = isNullable ? JsonSchemaType.String | JsonSchemaType.Null : JsonSchemaType.String;
        schema.Format = null;

        schema.Enum = Enum.GetNames(Nullable.GetUnderlyingType(clrType) ?? clrType)
            .Select(static name => (JsonNode)JsonValue.Create(name))
            .ToList();

        ClearCompositions(schema);
        schema.Items = null;
        schema.AdditionalProperties = null;
        schema.AdditionalPropertiesAllowed = true;
    }
    #endregion

    #region Numerics
    /// <summary>
    /// Converts Numeric CLR Types to their Corresponding OpenAPI Schema Types and Formats.
    /// </summary>
    private static void NormalizeNumeric(OpenApiSchema schema, Type clrType, JsonSchemaType jsonType, string? format, bool forceString)
    {
        bool isNullable = IsClrNullable(clrType, memberInfo: null);

        schema.Type = forceString
            ? isNullable ? JsonSchemaType.String | JsonSchemaType.Null : JsonSchemaType.String
            : isNullable ? jsonType | JsonSchemaType.Null : jsonType;

        schema.Format = forceString ? null : format;

        ClearCompositions(schema);

        schema.Items = null;
        schema.AdditionalProperties = null;
        schema.AdditionalPropertiesAllowed = true;
    }

    /// <summary>
    /// Maps CLR Numeric Types to OpenAPI Numeric Type or Format.
    /// </summary>
    private static bool TryMapNumeric(Type type, out JsonSchemaType jsonType, out string? format, out bool asString)
    {
        asString = false;
        format = null;

        // Integers
        if (type == typeof(byte) || type == typeof(sbyte) ||
            type == typeof(short) || type == typeof(ushort) ||
            type == typeof(int) || type == typeof(uint) ||
            type == typeof(long) || type == typeof(ulong) ||
            type == typeof(nint) || type == typeof(nuint))
        {
            jsonType = JsonSchemaType.Integer;

            if (type == typeof(int) || type == typeof(uint)) format = "int32";
            else if (type == typeof(long) || type == typeof(ulong) || type == typeof(nint) || type == typeof(nuint)) format = "int64";

            return true;
        }

        // Floating Points
        if (type == typeof(float))
        {
            jsonType = JsonSchemaType.Number;
            format = "float";
            return true;
        }

        // Double
        if (type == typeof(double))
        {
            jsonType = JsonSchemaType.Number;
            format = "double";
            return true;
        }

        // Decimal
        if (type == typeof(decimal))
        {
            jsonType = JsonSchemaType.Number;
            format = "decimal";
            return true;
        }

        // Big Integer
        if (type == typeof(BigInteger))
        {
            jsonType = JsonSchemaType.Integer;
            asString = true;
            return true;
        }

        // Complex Format
        if (type == typeof(Complex))
        {
            jsonType = JsonSchemaType.String;
            asString = true;
            return true;
        }

        jsonType = default;
        return false;
    }
    #endregion

    #region Dictionaries
    /// <summary>
    /// Detects Dictionaries with String keys and Returns the VALUE Type.
    /// </summary>
    private static bool TryGetDictionaryValueType(Type type, out Type valueType)
    {
        valueType = null!;

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(Dictionary<,>) || definition == typeof(IDictionary<,>))
            {
                var arguments = type.GetGenericArguments();

                if (arguments[0] == typeof(string))
                {
                    valueType = arguments[1];
                    return true;
                }
            }
        }

        var dictionaryInterface = type.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>) && x.GetGenericArguments()[0] == typeof(string));

        if (dictionaryInterface is null) return false;

        valueType = dictionaryInterface.GetGenericArguments()[1];

        return true;
    }

    /// <summary>
    /// Normalizes a Dictionary Schema by Setting the Type to Object and Defining the Additional Properties as the Value Type Schema.
    /// </summary>
    private static async Task NormalizeDictionaryAsync(OpenApiSchema schema, Type clrType, Type valueType, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var memberInfo = context.JsonPropertyInfo?.AttributeProvider as MemberInfo;
        bool isNullable = IsClrNullable(clrType, memberInfo);

        schema.Format = null;
        schema.Items = null;
        schema.Properties = null;

        schema.OneOf = null;
        schema.AnyOf = null;
        schema.AllOf = null;

        schema.AdditionalPropertiesAllowed = true;

        var valueSchema = valueType == typeof(string)
            ? new OpenApiSchema { Type = JsonSchemaType.String }
            : new OpenApiSchema();

        if (valueType != typeof(string))
        {
            var openApiSchema = await context.GetOrCreateSchemaAsync(valueType, cancellationToken: cancellationToken);

            valueSchema = openApiSchema;
        }

        if (!isNullable)
        {
            schema.Type = JsonSchemaType.Object;
            schema.AdditionalProperties = valueSchema;
            return;
        }

        schema.OneOf =
        [
            new OpenApiSchema
            {
                Type = JsonSchemaType.Null
            },
            new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                AdditionalPropertiesAllowed = true,
                AdditionalProperties = valueSchema
            }

        ];

        schema.Type = null;
        schema.AdditionalProperties = null;
    }
    #endregion

    #region Arrays / Lists
    /// <summary>
    /// Detects Arrays and Common Enumerable Types and Returns the ELEMENT Type (List, Collection, Enumerable).
    /// </summary>
    private static bool TryGetEnumerableElementType(Type type, out Type elementType)
    {
        elementType = null!;

        if (type == typeof(string)) return false;

        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(List<>) || definition == typeof(IList<>) || definition == typeof(ICollection<>) || definition == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterface is null) return false;

        elementType = enumerableInterface.GetGenericArguments()[0];

        return true;
    }

    /// <summary>
    /// Normalizes an Array Schema by Setting the Type to Array and Defining the Items Schema as the Element Type Schema.
    /// </summary>
    private static async Task NormalizeArrayAsync(OpenApiSchema schema, Type clrType, Type elementType, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var memberInfo = context.JsonPropertyInfo?.AttributeProvider as MemberInfo;
        bool isNullable = IsClrNullable(clrType, memberInfo);

        var itemSchema = await context.GetOrCreateSchemaAsync(elementType, cancellationToken: cancellationToken);

        schema.AdditionalProperties = null;
        schema.AdditionalPropertiesAllowed = true;

        if (!isNullable)
        {
            schema.Type = JsonSchemaType.Array;
            schema.Items = itemSchema;
            schema.OneOf = null;
            schema.AnyOf = null;
            return;
        }

        schema.OneOf =
        [
            new OpenApiSchema
            {
                Type = JsonSchemaType.Null
            },
            new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = itemSchema
            }

        ];

        schema.Type = null;
        schema.Items = null;

        schema.AnyOf = null;
        schema.AllOf = null;
    }
    #endregion

    #region Required and Optional Attributes
    /// <summary>
    /// Application of Non-Nullable CLR Properties and Models as REQUIRED in the OpenAPI Schema.
    /// </summary>
    private static void ApplyRequiredFromClrNullability(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        var requiredProperties = new HashSet<string>();

        foreach (var jsonProp in context.JsonTypeInfo.Properties)
        {
            string jsonName = jsonProp.Name;

            if (schema.Properties != null && !schema.Properties.TryGetValue(jsonName, out _))
                continue;

            if (jsonProp.AttributeProvider is not MemberInfo memberInfo)
                continue;

            bool isNullable = IsMemberNullable(memberInfo);

            if (!isNullable)
                requiredProperties.Add(jsonName);
        }

        schema.Required = requiredProperties.Count > 0 ? requiredProperties : null;
    }

    /// <summary>
    /// Examines the Required and Nullability of Properties and Model Classes.
    /// </summary>
    private static bool IsClrNullable(Type clrType, MemberInfo? memberInfo)
    {
        if (clrType.IsValueType)
            return Nullable.GetUnderlyingType(clrType) is not null;

        return memberInfo is not null && IsMemberNullable(memberInfo);
    }

    /// <summary>
    /// Evaluates the Required and Nullability of Properties and Model Classes.
    /// </summary>
    private static bool IsMemberNullable(MemberInfo memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo propertyInfo => NullabilityContext.Create(propertyInfo).ReadState == NullabilityState.Nullable,
            FieldInfo fieldInfo => NullabilityContext.Create(fieldInfo).ReadState == NullabilityState.Nullable,
            _ => false
        };
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Removes OneOf or AnyOf or AllOf in the Schema to Ensure as A Single Consistent Shape.
    /// </summary>
    private static void ClearCompositions(OpenApiSchema schema)
    {
        schema.AnyOf = null;
        schema.OneOf = null;
        schema.AllOf = null;
    }
    #endregion
}
