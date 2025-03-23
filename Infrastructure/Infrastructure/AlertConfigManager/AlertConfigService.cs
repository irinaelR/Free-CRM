using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AlertConfigManager
{
    public class AlertConfigService : IAlertConfigService
    {
        private readonly DataContext _context;

        public AlertConfigService(DataContext context)
        {
            _context = context;
        }

        public async Task<AlertConfig> GetAlertConfigAsync(string id)
        {
            return await _context.AlertConfigs.FindAsync(id);
        }

        public async Task<AlertConfig> CreateAlertConfigAsync(AlertConfig alertConfig)
        {
            // Set creation and update timestamps
            alertConfig.DateAdded = DateTime.UtcNow;
            alertConfig.DateUpdated = DateTime.UtcNow;

            await _context.AlertConfigs.AddAsync(alertConfig);
            await _context.SaveChangesAsync();

            return alertConfig;
        }

        public async Task<AlertConfig> UpdateAlertConfigAsync(string id, double percentage)
        {
            var alertConfig = await _context.AlertConfigs.FindAsync(id);

            if (alertConfig == null)
            {
                throw new KeyNotFoundException($"AlertConfig with ID {id} not found.");
            }

            // Update only the percentage and the updated timestamp
            alertConfig.Percentage = percentage;
            alertConfig.DateUpdated = DateTime.UtcNow;

            _context.AlertConfigs.Update(alertConfig);
            await _context.SaveChangesAsync();

            return alertConfig;
        }

        public async Task<bool> DeleteAlertConfigAsync(string id)
        {
            var alertConfig = await _context.AlertConfigs.FindAsync(id);

            if (alertConfig == null)
            {
                return false;
            }

            _context.AlertConfigs.Remove(alertConfig);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
