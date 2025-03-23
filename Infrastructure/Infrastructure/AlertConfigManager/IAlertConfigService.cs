using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AlertConfigManager
{
    public interface IAlertConfigService
    {
        Task<AlertConfig> GetAlertConfigAsync(string id);
        Task<AlertConfig> CreateAlertConfigAsync(AlertConfig alertConfig);
        Task<AlertConfig> UpdateAlertConfigAsync(string id, double percentage);
        Task<bool> DeleteAlertConfigAsync(string id);
    }
}
