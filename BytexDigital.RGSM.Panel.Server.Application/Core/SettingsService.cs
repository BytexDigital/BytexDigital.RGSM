using System;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Persistence;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace BytexDigital.RGSM.Panel.Server.Application.Core
{
    public class SettingsService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SettingsService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<T> GetValueOrDefaultAsync<T>(string key, T defaultValue = default)
        {
            var setting = await _applicationDbContext.SharedSettings.FirstOrDefaultAsync(x => x.Key == key);

            if (setting == null)
            {
                return defaultValue;
            }

            if (typeof(T) == typeof(string)) return (T)Convert.ChangeType(setting.Value, typeof(T));

            return JsonConvert.DeserializeObject<T>(setting.Value);
        }

        public async Task SetValueAsync<T>(string key, T value)
        {
            string writeValue = null;

            if (typeof(T) == typeof(string))
            {
                writeValue = (string)Convert.ChangeType(value, typeof(string));
            }
            else
            {
                writeValue = JsonConvert.SerializeObject(value);
            }

            var setting = await _applicationDbContext.SharedSettings.FirstAsync(x => x.Key == key);

            setting.Value = writeValue;

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
