using Mediflow.Application.DTOs.Recommendations;
using Mediflow.Application.Exceptions;
using Mediflow.Domain.Entities;
using Mediflow.Infrastructure.Implementation.Services;
using Mediflow.Tests.Common;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace Mediflow.Tests.Services;

public class DoctorRecommendationServiceTests
{
    [Fact]
    public void GetRecommendations_AssessmentPrioritizesCriticalCareForRespiratoryDistressSignals()
    {
        using var context = TestApplicationDbContextFactory.Create();
        SeedSpecializations(context, "Critical Care Physician", "Cardiologist", "General Physician");

        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.WebRootPath).Returns(string.Empty);

        var service = new DoctorRecommendationService(environment.Object, context);

        var result = service.GetRecommendations(new DoctorRecommendationAssessmentDto
        {
            TemperatureCelsius = 38.7m,
            OxygenSaturationPercent = 91,
            Symptoms = ["shortness of breath", "cough"]
        });

        Assert.Equal("Critical Care Physician", result.RecommendedSpecialization);
        Assert.Contains(result.MatchedSignals, signal => signal.Contains("Oxygen saturation"));
        Assert.Contains("Critical Care Physician", result.AssessmentSummary);
    }

    [Fact]
    public void GetRecommendations_AssessmentSelectsPediatricsForChildPatients()
    {
        using var context = TestApplicationDbContextFactory.Create();
        SeedSpecializations(context, "Pediatrician", "General Physician");

        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.WebRootPath).Returns(string.Empty);

        var service = new DoctorRecommendationService(environment.Object, context);

        var result = service.GetRecommendations(new DoctorRecommendationAssessmentDto
        {
            AgeYears = 10
        });

        Assert.Equal("Pediatrician", result.RecommendedSpecialization);
    }

    [Fact]
    public void GetRecommendations_AssessmentRejectsEmptyInput()
    {
        using var context = TestApplicationDbContextFactory.Create();
        SeedSpecializations(context, "General Physician");

        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(x => x.WebRootPath).Returns(string.Empty);

        var service = new DoctorRecommendationService(environment.Object, context);

        Assert.Throws<BadRequestException>(() => service.GetRecommendations(new DoctorRecommendationAssessmentDto()));
    }

    private static void SeedSpecializations(Mediflow.Infrastructure.Persistence.ApplicationDbContext context, params string[] titles)
    {
        foreach (var title in titles)
        {
            var specialization = new Specialization(title, $"{title} description");
            specialization.AssignIdentifier(Guid.NewGuid());
            context.Specializations.Add(specialization);
        }

        context.SaveChanges();
    }
}
