using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Mediflow.Application.Common.Response;
using Microsoft.Extensions.DependencyInjection;
using Mediflow.Application.Common.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Mediflow.Infrastructure.Dependency;

public static class ControllerConfiguration
{
    public static IServiceCollection AddControllerConfiguration(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterConfiguration()));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err => new
                    {
                        err.ErrorMessage
                    }))
                    .ToList();

                var exception = errors.Select(x => x.ErrorMessage);

                var response = new ResponseDto<object>((int)HttpStatusCode.BadRequest, string.Join(" ", exception), null);

                return new BadRequestObjectResult(response);
            };
        });

        // TODO: Addition of Fluent Validation
        // var assemblies = Assembly
        //     .GetExecutingAssembly()
        //     .GetReferencedAssemblies()
        //     .Where(x => x.FullName.StartsWith("GVAC."))
        //     .Select(Assembly.Load)
        //     .Append(Assembly.GetExecutingAssembly());
        //
        // services.AddValidatorsFromAssemblies(assemblies);
        //
        // services.AddFluentValidationAutoValidation();
        //
        // services.AddFluentValidationAutoValidation(configuration =>
        // {
        //     configuration.DisableDataAnnotationsValidation = true;
        // });

        return services;
    }
}