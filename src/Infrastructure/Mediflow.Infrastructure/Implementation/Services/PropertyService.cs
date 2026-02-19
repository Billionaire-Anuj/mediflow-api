using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mediflow.Domain.Entities;
using Mediflow.Application.Exceptions;
using Mediflow.Domain.Common.Property;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class PropertyService(IApplicationDbContext applicationDbContext) : IPropertyService
{
    public T? GetProperty<T>(string key)
    {
        var propertyModel = applicationDbContext.Properties.FirstOrDefault(x => x.Setting.Key == key);

        if (propertyModel == null) return default;

        var value = propertyModel.Setting.Value;

        switch (value)
        {
            case JToken token:
                return token.ToObject<T>();
            case string @string:
                return JsonConvert.DeserializeObject<T>(@string);
            case T typed:
                return typed;
            default:
            {
                var serialized = JsonConvert.SerializeObject(value);
                return JsonConvert.DeserializeObject<T>(serialized);
            }
        }
    }

    public T GetPropertyOrDefault<T>(string key, T defaultProperty)
    {
        var result = GetProperty<T>(key);
            
        if (result != null) return result;

        if (defaultProperty != null) SaveProperty(key, defaultProperty);
            
        return defaultProperty;
    }

    public List<KeyValueProperty> GetAllProperties()
    {
        var properties = applicationDbContext.Properties;

        return properties.Select(x => x.Setting).ToList();
    }

    public void SaveProperty(string key, object value)
    {
        var property = applicationDbContext.Properties.FirstOrDefault(x => x.Setting.Key == key);

        var keyValueProperty = new KeyValueProperty
        {
            Key = key,
            Value = value
        };

        if (property == null)
        {
            var propertyModel = new Property(keyValueProperty);

            applicationDbContext.Properties.Add(propertyModel);
        }
        else
        {
            property.Update(keyValueProperty);

            applicationDbContext.Properties.Update(property);
        }

        applicationDbContext.SaveChanges();
    }

    public void DeleteProperty(string key)
    {
        var propertyModel = applicationDbContext.Properties.FirstOrDefault(x => x.Setting.Key == key)
                            ?? throw new NotFoundException("The property could not be found.");

        applicationDbContext.Properties.Remove(propertyModel);

        applicationDbContext.SaveChanges();
    }
}