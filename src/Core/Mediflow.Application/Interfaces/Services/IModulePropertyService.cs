using Mediflow.Domain.Common.Property;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IModulePropertyService : ITransientService
{
    T? GetProperty<T>(Guid moduleId, string key);

    T GetPropertyOrDefault<T>(Guid moduleId, string key, T defaultProperty);

    List<KeyValueProperty> GetAllProperties(Guid moduleId);

    void SaveProperty(Guid moduleId, string key, object value);

    void DeleteProperty(Guid moduleId, string key);
}