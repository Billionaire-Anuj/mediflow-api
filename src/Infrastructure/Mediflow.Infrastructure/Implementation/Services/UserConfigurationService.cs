using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Domain.Common.Property;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class UserConfigurationService(IApplicationDbContext applicationDbContext) : IUserConfigurationService
{
    public T? GetProperty<T>(Guid userId, string key)
    {
        var user = applicationDbContext.Users
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException("The user could not be found.");

        var userConfiguration =
            applicationDbContext.UserConfigurations.FirstOrDefault(x =>
                x.UserId == user.Id && x.Configurations.Key == key);

        if (userConfiguration == null) return default;

        var value = userConfiguration.Configurations.Value;

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

    public T GetPropertyOrDefault<T>(Guid userId, string key, T defaultProperty)
    {
        var result = GetProperty<T>(userId, key);
            
        if (result != null) return result;

        if (defaultProperty != null) SaveProperty(userId, key, defaultProperty);
            
        return defaultProperty;
    }

    public List<KeyValueProperty> GetAllProperties(Guid userId)
    {
        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The user could not be found.");

        var properties = applicationDbContext.UserConfigurations.Where(x => x.UserId == user.Id).AsQueryable();

        return properties.Select(x => x.Configurations).ToList();
    }

    public void SaveProperty(Guid userId, string key, object value)
    {
        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The user could not be found.");

        var userConfiguration = applicationDbContext.UserConfigurations.FirstOrDefault(x => 
            x.UserId == user.Id && x.Configurations.Key == key);

        var property = new KeyValueProperty
        {
            Key = key,
            Value = value
        };

        if (userConfiguration == null)
        {
            var userConfigurationModel = new UserConfiguration(user.Id, property);

            applicationDbContext.UserConfigurations.Add(userConfigurationModel);
        }
        else
        {
            userConfiguration.UpdateConfigurations(property);
            
            applicationDbContext.UserConfigurations.Update(userConfiguration);
        }

        applicationDbContext.SaveChanges();
    }

    public void DeleteProperty(Guid userId, string key)
    {
        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The user could not be found.");

        var userConfiguration = applicationDbContext.UserConfigurations.FirstOrDefault(x => 
                                    x.UserId == user.Id && x.Configurations.Key == key)
                                ?? throw new NotFoundException("The following user's configuration could not be found.");

        applicationDbContext.UserConfigurations.RemoveRange(userConfiguration);

        applicationDbContext.SaveChanges();
    }
}
