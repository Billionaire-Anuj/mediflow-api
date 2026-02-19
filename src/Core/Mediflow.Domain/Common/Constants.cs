namespace Mediflow.Domain.Common;

public abstract class Constants
{
    public abstract class Roles
    {
        public abstract class TenantAdministrator
        {
            public const string Id = "019b68f0-0000-7000-a000-cc30eb9410dc";
            public const string Name = "Tenant Administrator";
            public const string Description = "An administrative role with full control over tenant-level configurations.";
            public const bool IsDisplayed = false;
            public const bool IsRegisterable = false;
        }

        public abstract class SuperAdmin
        {
            public const string Id = "019b68f0-0001-7000-a000-cc30eb9410dc";
            public const string Name = "Super Admin";
            public const string Description = "The highest authority role, responsible for overseeing operational governance across all modules.";
            public const bool IsDisplayed = true;
            public const bool IsRegisterable = false;
        }

        public abstract class Doctor
        {
            public const string Id = "019b68f0-0002-7000-a000-cc30eb9410dc";
            public const string Name = "Doctor";
            public const string Description = "Assigned with the patient-control and administration for diagnosis.";
            public const bool IsDisplayed = true;
            public const bool IsRegisterable = true;
        }

        public abstract class LabTechnician
        {
            public const string Id = "019b68f0-0003-7000-a000-cc30eb9410dc";
            public const string Name = "Lab Technician";
            public const string Description = "The role responsible for overseeing operational access for the laboratory.";
            public const bool IsDisplayed = true;
            public const bool IsRegisterable = true;
        }

        public abstract class Pharmacist
        {
            public const string Id = "019b68f0-0004-7000-a000-cc30eb9410dc";
            public const string Name = "Pharmacist";
            public const string Description = "The role responsible for overseeing operational access for the pharmacy.";
            public const bool IsDisplayed = true;
            public const bool IsRegisterable = true;
        }

        public abstract class Patient
        {
            public const string Id = "019b68f0-0005-7000-a000-cc30eb9410dc";
            public const string Name = "Patient";
            public const string Description = "The lowest level role able to book appointments and proceed with the features offered by Mediflow.";
            public const bool IsDisplayed = true;
            public const bool IsRegisterable = true;
        }
    }
    
    public abstract class DbProviderKeys
    {
        public const string SqlServer = "mssql";
        public const string Npgsql = "postgresql";
    }
    
    private abstract class FolderPath
    {
        public const string Images = "images";
        public const string EmailTemplates = "email-templates";
    }

    private abstract class FolderPrefix
    {
        public const string UserImagesPrefix = "user-images";
    }
    
    public abstract class FilePath
    {
        public const string EmailTemplatesFilePath = $"{FolderPath.EmailTemplates}/";
        public const string UserImagesFilePath = $"{FolderPath.Images}/{FolderPrefix.UserImagesPrefix}/";
    }
    
    public abstract class Cors
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    }
    
    public abstract class Cookie
    {
        public const string TokenPayload = "X-Mediflow-Token-Payload";
        public const string TokenSignature = "X-Mediflow-Token-Signature";
        public const string TokenExpiration = "X-Mediflow-Token-Expiration";
    }
}