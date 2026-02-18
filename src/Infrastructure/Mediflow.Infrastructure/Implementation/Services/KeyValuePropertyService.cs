using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mediflow.Domain.Common.Property;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class KeyValuePropertyService : IKeyValuePropertyService
{
    public T? GetProperty<T>(KeyValueProperty keyValuePair)
    {
        switch (keyValuePair.Value)
        {
            case JArray jArray:
                return jArray.ToObject<T>();
            case JObject jObject:
                return jObject.ToObject<T>();
            case string jsonString:
                try
                {
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (JsonException exception)
                {
                    throw new InvalidOperationException("Failed to deserialize the JSON string into the specified type.", exception);
                }
            default:
                return (T?)Convert.ChangeType(keyValuePair.Value, typeof(T));
        }
    }

    public KeyValueProperty SaveProperty(string key, object value)
    {
        return new KeyValueProperty()
        {
            Key = key,
            Value = value
        };
    }
}