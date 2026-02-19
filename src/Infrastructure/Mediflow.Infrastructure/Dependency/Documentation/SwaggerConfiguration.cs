using Microsoft.OpenApi;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure.Dependency.Documentation;

internal static class SwaggerConfiguration
{
    internal static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Mediflow API",
                Version = "v1",
                Contact = new OpenApiContact()
                {
                    Name = "Mediflow",
                    Email = "mediflow@affinatic.com",
                    Url = new Uri("https://mediflow.com")
                },
                Description = "Mediflow - Medical and Healthcare Poral",
                License = new OpenApiLicense
                {
                    Name = "Mediflow",
                    Url = new Uri("https://mediflow.com")
                }
            });

            c.EnableAnnotations();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
        });
    }
    
    internal static void AddCustomSwaggerInterface(this IApplicationBuilder app)
    {
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });
    
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Mediflow API v1");
            options.DefaultModelsExpandDepth(-1);
            options.DocExpansion(DocExpansion.None);
            options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
        });
    }
}