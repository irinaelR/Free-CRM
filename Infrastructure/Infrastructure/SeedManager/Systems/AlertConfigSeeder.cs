using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SeedManager.Systems
{
    public class AlertConfigSeeder
    {
        public async Task SeedAlertConfigAsync(DataContext context)
        {
            // Check if there are any alert configs already
            if (!await context.AlertConfigs.AnyAsync())
            {
                // Add the default alert config
                await context.AlertConfigs.AddAsync(new AlertConfig
                {
                    Id = "AC001",
                    Percentage = 80,
                    DateAdded = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                });

                // Save changes to the database
                await context.SaveChangesAsync();
            }
        }
    }
}
