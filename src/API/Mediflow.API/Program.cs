using Mediflow.Domain.Common;
using Hangfire;
using Mediflow.API.Middleware;
using Microsoft.Net.Http.Headers;
using Mediflow.Infrastructure.Dependency;
using Mediflow.Infrastructure.Implementation.Jobs;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

services.AddControllerConfiguration();

builder.AddConfigurations();

services.AddDependencyServices();

services.AddInfrastructureService(configuration);

services.AddDataSeedService();

services.AddEndpointsApiExplorer();

services.AddOpenApiDocumentation();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<TokenCookieMiddleware>();

app.UseMiddleware<RequestNormalizationMiddleware>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseOpenApiDocumentation();
}

app.UseCors(Constants.Cors.MyAllowSpecificOrigins);

app.UseRouting();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        var origin = context.Context.Request.Headers.Origin.ToString();

        if (string.IsNullOrWhiteSpace(origin))
            return;

        context.Context.Response.Headers[HeaderNames.AccessControlAllowOrigin] = origin;
        context.Context.Response.Headers[HeaderNames.Vary] = "Origin";

        context.Context.Response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

RecurringJob.AddOrUpdate<AppointmentReminderNotificationJob>(
    "patient-appointment-reminder-notifications",
    job => job.SendPatientAppointmentRemindersAsync(),
    Cron.Minutely);

app.Run();
