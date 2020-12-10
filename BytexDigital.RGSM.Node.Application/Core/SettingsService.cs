//using System;
//using System.Threading.Tasks;

//using BytexDigital.RGSM.Node.Persistence;

//using Microsoft.EntityFrameworkCore;

//using Newtonsoft.Json;

//namespace BytexDigital.RGSM.Node.Application.Options
//{
//    public class SettingsService
//    {
//        public const string KEY_SETTING_NODEID = "node_id";

//        private readonly NodeDbContext _nodeDbContext;

//        public SettingsService(NodeDbContext nodeDbContext)
//        {
//            _nodeDbContext = nodeDbContext;
//        }

//        public async Task EnsureIdExistsAsync()
//        {
//            await EnsureSettingExistsAsync(KEY_SETTING_NODEID, Guid.NewGuid().ToString());
//        }

//        public async Task<T> GetValueAsync<T>(string key)
//        {
//            var setting = await _nodeDbContext.NodeSettings.FirstAsync(x => x.Key == key);

//            if (typeof(T) == typeof(string)) return (T)Convert.ChangeType(setting.Value, typeof(T));

//            return JsonConvert.DeserializeObject<T>(setting.Value);
//        }

//        public async Task SetValueAsync<T>(string key, T value)
//        {
//            string writeValue = null;

//            if (typeof(T) == typeof(string))
//            {
//                writeValue = (string)Convert.ChangeType(value, typeof(string));
//            }
//            else
//            {
//                writeValue = JsonConvert.SerializeObject(value);
//            }

//            var setting = await _nodeDbContext.NodeSettings.FirstAsync(x => x.Key == key);

//            setting.Value = writeValue;

//            await _nodeDbContext.SaveChangesAsync();
//        }

//        private async Task EnsureSettingExistsAsync(string key, string value)
//        {
//            var setting = await _nodeDbContext.NodeSettings.FirstOrDefaultAsync(x => x.Key == key);

//            if (setting == null)
//            {
//                setting = _nodeDbContext.CreateEntity(x => x.NodeSettings);

//                setting.Key = key;
//                setting.Value = value;

//                await _nodeDbContext.SaveChangesAsync();
//            }
//        }
//    }
//}
