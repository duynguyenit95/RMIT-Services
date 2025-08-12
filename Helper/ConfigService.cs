using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Context;
using TaskScheduler.Entity;

namespace TaskScheduler.Helper
{
    public class ConfigService
    {
        public readonly RPAGateContext _rpaContext;
        public ConfigService(RPAGateContext rpaContext)
        {
            _rpaContext = rpaContext;
        }

        public async Task<SystemConfig?> GetSystemConfig(string configGroup, string configKey)
        {
            return await _rpaContext.SystemConfigs.Where(x => x.ConfigGroup == configGroup && x.ConfigKey == configKey).FirstOrDefaultAsync();
        }
        public async Task<SystemConfig> UpdateSystemConfig(string configGroup, string configKey, string configValue)
        {
            var entity = await _rpaContext.SystemConfigs
                .Where(x => x.ConfigGroup == configGroup && x.ConfigKey == configKey)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new SystemConfig
                {
                    ConfigGroup = configGroup,
                    ConfigKey = configKey,
                    ConfigValue = configValue,
                    LastUpdatedBy = "rpa",
                    LastUpdatedTime = DateTimeOffset.Now
                };
                await _rpaContext.SystemConfigs.AddAsync(entity);
            }
            else
            {
                entity.ConfigValue = configValue;
                entity.LastUpdatedBy = "rpa";
                entity.LastUpdatedTime = DateTimeOffset.Now;
            }
            await _rpaContext.SaveChangesAsync();
            return entity;
        }
    }
}
