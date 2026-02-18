namespace Mediflow.Application.Common.Authorization;

public static class MediflowActions
{
    public const string Menu = nameof(Menu);
    public const string View = nameof(View);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string ActivateDeactivate = nameof(ActivateDeactivate);
}

public static class MediflowResources
{
    // public const string Application = nameof(Application);
    // public const string AuditLog = nameof(AuditLog);
    // public const string BloodGroup = nameof(BloodGroup);
    // public const string Branch = nameof(Branch);
    // public const string Candidate = nameof(Candidate);
    // public const string Category = nameof(Category);
    // public const string Client = nameof(Client);
    // public const string ClientType = nameof(ClientType);
    // public const string Company = nameof(Company);
    // public const string Designation = nameof(Designation);
    // public const string EducationDegree = nameof(EducationDegree);
    // public const string Interview = nameof(Interview);
    // public const string InterviewMetricsScore = nameof(InterviewMetricsScore);
    // public const string Location = nameof(Location);
    // public const string Metric = nameof(Metric);
    // public const string Nationality = nameof(Nationality);
    // public const string Religion = nameof(Religion);
    // public const string Role = nameof(Role);
    // public const string Spokesperson = nameof(Spokesperson);
    // public const string User = nameof(User);
    // public const string UserLoginLog = nameof(UserLoginLog);
    // public const string Vacancy = nameof(Vacancy);
}

public abstract record MediflowPermission(string Resources, string Action)
{
    public string Name => NameFor(Resources, Action);

    public static string NameFor(string resources, string action) => $"Permissions.{resources}.{action}";
}