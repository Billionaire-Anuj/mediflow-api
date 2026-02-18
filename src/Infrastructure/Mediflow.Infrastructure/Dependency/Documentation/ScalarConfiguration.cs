using Scalar.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace Mediflow.Infrastructure.Dependency.Documentation;

internal static class ScalarConfiguration
{
    internal static void AddCustomScalarInterface(this WebApplication app)
    {
        app.MapScalarApiReference(options =>
        {
            options.OpenApiRoutePattern = "/openapi/{documentName}.json";
            options.Title = "Mediflow API";
            options.DarkMode = true;
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.RestSharp);
            options.Layout = ScalarLayout.Modern;
            options.ShowSidebar = true;
            options.Authentication = new ScalarAuthenticationOptions()
            {
                PreferredSecuritySchemes = new List<string>()
            };
            options.TagSorter = TagSorter.Alpha;
            options.DocumentDownloadType = DocumentDownloadType.Direct;
            options.HideDarkModeToggle = false;
            options.OperationSorter = OperationSorter.Alpha;
            options.DotNetFlag = true;
            options.HideModels = true;
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(
                ScalarTarget.CSharp,
                ScalarClient.RestSharp);
        });
    }
}