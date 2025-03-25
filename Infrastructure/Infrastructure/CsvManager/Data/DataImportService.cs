using Application.Common.Repositories;
using Application.Features.NumberSequenceManager;
using Domain.CsvTypes.Records;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataAccessManager.EFCore.Contexts;

namespace Infrastructure.CsvManager.Data;

public class DataImportService
{
    private NumberSequenceService _numberSequenceService;

    public DataImportService(NumberSequenceService numberSequenceService)
    {
        _numberSequenceService = numberSequenceService;
    }

    public List<Campaign> CreateCampaigns(List<IncompleteCampaignRecord> incompleteCampaignRecords,
        List<CampaignChildRecord> childRecords, DataContext context)
    {
        List<Campaign> campaigns = new List<Campaign>();
        
        var random = new Random();
        
        var salesTeamIds = context.SalesTeam.Select(st => st.Id).ToList();

        foreach (var record in incompleteCampaignRecords)
        {
            DateTime earliestDate = GetEarliestDate(record.Code, childRecords);
            DateTime latestDate = GetLatestDate(record.Code, childRecords);
            var campaign = new Campaign
            {
                Number = record.Code,
                Title = record.Title,
                Description = $"Description for campaign {record.Code}",
                TargetRevenueAmount = 10000 * Math.Ceiling((random.NextDouble() * 89) + 1),
                CampaignDateStart = earliestDate.AddMonths(-1),
                CampaignDateFinish = latestDate.AddMonths(1),
                Status = CampaignStatus.Confirmed,
                SalesTeamId = GetRandomValue(salesTeamIds, random)
            };
            campaigns.Add(campaign);
        }
        
        return campaigns;
    }

    public List<Budget> CreateBudgets(List<CampaignChildRecord> childRecords, List<Campaign> campaigns)
    {
        var filteredRecords = childRecords.Where(c => c.Type.Equals("Budget", StringComparison.OrdinalIgnoreCase));
        
        List<Budget> budgets = new List<Budget>();

        foreach (var record in filteredRecords)
        {
            var budget = new Budget
            {
                Number = _numberSequenceService.GenerateNumber(nameof(Budget), "", "BUD"),
                Title = record.Title,
                Description = $"Description for {record.Title}",
                BudgetDate = record.Date,
                Status = BudgetStatus.Confirmed,
                Amount = record.Amount,
                CampaignId = campaigns.First(c => c.Number == record.CampaignNumber).Id
            };
            
            budgets.Add(budget);
        }
        
        return budgets;
    }

    public List<Expense> CreateExpenses(List<CampaignChildRecord> childRecords, List<Campaign> campaigns)
    {
        var filteredRecords = childRecords.Where(c => c.Type.Equals("Expense", StringComparison.OrdinalIgnoreCase));
        
        List<Expense> expenses = new List<Expense>();

        foreach (var record in filteredRecords)
        {
            var expense = new Expense
            {
                Number = _numberSequenceService.GenerateNumber(nameof(Expense), "", "EXP"),
                Title = record.Title,
                Description = $"Description for {record.Title}",
                ExpenseDate = record.Date,
                Status = ExpenseStatus.Confirmed,
                Amount = record.Amount,
                CampaignId = campaigns.First(c => c.Number == record.CampaignNumber).Id
            };
            expenses.Add(expense);
        }

        return expenses;
    }

    public (List<Campaign> campaigns, List<Budget> budgets, List<Expense> expenses) CreateAndSave(List<IncompleteCampaignRecord> incompleteCampaignRecords,
        List<CampaignChildRecord> childRecords, DataContext context)
    {
        List<Campaign> campaigns = CreateCampaigns(incompleteCampaignRecords, childRecords, context);
        context.Campaign.AddRange(campaigns);
        context.SaveChanges();

        List<Budget> budgets = CreateBudgets(childRecords, campaigns);
        context.Budget.AddRange(budgets);
        context.SaveChanges();
        
        List<Expense> expenses = CreateExpenses(childRecords, campaigns);
        context.Expense.AddRange(expenses);
        context.SaveChanges();
        
        return (campaigns, budgets, expenses);
    }
    
    private static string GetRandomValue(List<string> list, Random random)
    {
        return list[random.Next(list.Count)];
    }

    private DateTime GetEarliestDate(string campaignNumber, List<CampaignChildRecord> childRecords)
    {
        var filteredChildren = childRecords.Where(x => x.CampaignNumber == campaignNumber);
        return filteredChildren.OrderBy(x => x.Date).First().Date;
    }

    private DateTime GetLatestDate(string campaignNumber, List<CampaignChildRecord> childRecords)
    {
        var filteredChildren = childRecords.Where(x => x.CampaignNumber == campaignNumber);
        return filteredChildren.OrderByDescending(x => x.Date).First().Date;
    }
}