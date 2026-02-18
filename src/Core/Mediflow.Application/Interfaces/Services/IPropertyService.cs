using Mediflow.Domain.Common.Property;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IPropertyService : ITransientService
{
    T? GetProperty<T>(string key);

    T GetPropertyOrDefault<T>(string key, T defaultProperty);

    List<KeyValueProperty> GetAllProperties();

    void SaveProperty(string key, object value);

    void DeleteProperty(string key);
}