using Application.Common.Services.DatabaseCleaningManager;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Infrastructure.SeedManager;
using Infrastructure.SeedManager.Demos;
using Infrastructure.SeedManager.Systems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DatabaseCleaner
{
    public class DatabaseCleanerService : IDatabaseCleanerService
    {
        private readonly DataContext _context;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseCleanerService(DataContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        void RepopulateSystemData(IHost host)
        {
            if (host != null)
            {
                host.SeedSystemData();

            }
            else
            {
                if (!_context.Roles.Any()) //if empty, thats mean never been seeded before
                {
                    var roleSeeder = _serviceProvider.GetRequiredService<RoleSeeder>();
                    roleSeeder.GenerateDataAsync().Wait();

                    var userAdminSeeder = _serviceProvider.GetRequiredService<UserAdminSeeder>();
                    userAdminSeeder.GenerateDataAsync().Wait();

                    var companySeeder = _serviceProvider.GetRequiredService<CompanySeeder>();
                    companySeeder.GenerateDataAsync().Wait();

                }
            }
        }

        void RepopulateDemoData(IHost host)
        {
            if (host != null)
            {
                host.SeedDemoData();
            }
            else if (!_context.Tax.Any())
            {
                var taxSeeder = _serviceProvider.GetRequiredService<TaxSeeder>();
                taxSeeder.GenerateDataAsync().Wait();

                var userSeeder = _serviceProvider.GetRequiredService<UserSeeder>();
                userSeeder.GenerateDataAsync().Wait();

                var customerCategorySeeder = _serviceProvider.GetRequiredService<CustomerCategorySeeder>();
                customerCategorySeeder.GenerateDataAsync().Wait();

                var customerGroupSeeder = _serviceProvider.GetRequiredService<CustomerGroupSeeder>();
                customerGroupSeeder.GenerateDataAsync().Wait();

                var customerSeeder = _serviceProvider.GetRequiredService<CustomerSeeder>();
                customerSeeder.GenerateDataAsync().Wait();

                var customerContactSeeder = _serviceProvider.GetRequiredService<CustomerContactSeeder>();
                customerContactSeeder.GenerateDataAsync().Wait();

                var vendorCategorySeeder = _serviceProvider.GetRequiredService<VendorCategorySeeder>();
                vendorCategorySeeder.GenerateDataAsync().Wait();

                var vendorGroupSeeder = _serviceProvider.GetRequiredService<VendorGroupSeeder>();
                vendorGroupSeeder.GenerateDataAsync().Wait();

                var vendorSeeder = _serviceProvider.GetRequiredService<VendorSeeder>();
                vendorSeeder.GenerateDataAsync().Wait();

                var vendorContactSeeder = _serviceProvider.GetRequiredService<VendorContactSeeder>();
                vendorContactSeeder.GenerateDataAsync().Wait();

                var unitMeasureSeeder = _serviceProvider.GetRequiredService<UnitMeasureSeeder>();
                unitMeasureSeeder.GenerateDataAsync().Wait();

                var productGroupSeeder = _serviceProvider.GetRequiredService<ProductGroupSeeder>();
                productGroupSeeder.GenerateDataAsync().Wait();

                var productSeeder = _serviceProvider.GetRequiredService<ProductSeeder>();
                productSeeder.GenerateDataAsync().Wait();

                var salesOrderSeeder = _serviceProvider.GetRequiredService<SalesOrderSeeder>();
                salesOrderSeeder.GenerateDataAsync().Wait();

                var purchaseOrderSeeder = _serviceProvider.GetRequiredService<PurchaseOrderSeeder>();
                purchaseOrderSeeder.GenerateDataAsync().Wait();



                var salesTeamSeeder = _serviceProvider.GetRequiredService<SalesTeamSeeder>();
                salesTeamSeeder.GenerateDataAsync().Wait();

                var salesRepresentativeSeeder = _serviceProvider.GetRequiredService<SalesRepresentativeSeeder>();
                salesRepresentativeSeeder.GenerateDataAsync().Wait();

                var campaignSeeder = _serviceProvider.GetRequiredService<CampaignSeeder>();
                campaignSeeder.GenerateDataAsync().Wait();

                var budgetSeeder = _serviceProvider.GetRequiredService<BudgetSeeder>();
                budgetSeeder.GenerateDataAsync().Wait();

                var expenseSeeder = _serviceProvider.GetRequiredService<ExpenseSeeder>();
                expenseSeeder.GenerateDataAsync().Wait();

                var leadSeeder = _serviceProvider.GetRequiredService<LeadSeeder>();
                leadSeeder.GenerateDataAsync().Wait();

                var leadContactSeeder = _serviceProvider.GetRequiredService<LeadContactSeeder>();
                leadContactSeeder.GenerateDataAsync().Wait();

                var leadActivitySeeder = _serviceProvider.GetRequiredService<LeadActivitySeeder>();
                leadActivitySeeder.GenerateDataAsync().Wait();
            }
        }

        async Task IDatabaseCleanerService.RecreateDatabase(bool includeDemoData)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            using (var scope = _serviceProvider.CreateScope())
            {
                var host = scope.ServiceProvider.GetService<IHost>();
                RepopulateSystemData(host);
                if (includeDemoData)
                {
                    RepopulateDemoData(host);
                }
            }
        }

        void IDatabaseCleanerService.RepopulateWithDemo()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var host = scope.ServiceProvider.GetService<IHost>();
                RepopulateDemoData(host);
            }
        }
    }
}
