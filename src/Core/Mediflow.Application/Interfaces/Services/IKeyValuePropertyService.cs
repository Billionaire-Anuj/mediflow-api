using Mediflow.Domain.Common.Property;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IKeyValuePropertyService : ITransientService
{
    T? GetProperty<T>(KeyValueProperty keyValuePair);
    
    KeyValueProperty SaveProperty(string key, object value);
}