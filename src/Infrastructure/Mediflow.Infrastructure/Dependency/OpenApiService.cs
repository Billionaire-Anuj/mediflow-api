using Microsoft.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mediflow.Infrastructure.Dependency.Documentation;

namespace Mediflow.Infrastructure.Dependency;

public static class OpenApiService
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        // services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        // {
        //     var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
        //     var loggerFactory = provider.GetService<ILoggerFactory>();
        //
        //     return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        // });

        services.AddOpenApi(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;

            options.AddSchemaTransformer<OpenApiSchemaTransformer>();

            options.AddDocumentTransformer((document, _, _) =>
            {
                NormalizeOperationResponses(document);

                return Task.CompletedTask;
            });
        });

        services.AddSwaggerConfiguration();

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
    {
        if (app is not WebApplication application) return app;

        application.MapOpenApi();

        application.AddCustomScalarInterface();

        application.AddCustomSwaggerInterface();

        return app;
    }

    private static void NormalizeOperationResponses(OpenApiDocument document)
    {
        foreach (var pathItem in document.Paths.Values)
        {
            if (pathItem.Operations is null) continue;

            foreach (var operation in pathItem.Operations.Values)
            {
                if (operation.Responses is null) continue;

                foreach (var response in operation.Responses.Values.Where(response => string.Equals(response.Description, "OK", StringComparison.OrdinalIgnoreCase)))
                {
                    response.Description = string.Empty;
                }

                operation.Responses.Remove("400");
                operation.Responses.Remove("401");
                operation.Responses.Remove("403");
                operation.Responses.Remove("404");
                operation.Responses.Remove("409");
                operation.Responses.Remove("422");
                operation.Responses.Remove("500");
            }
        }
    }
}
