using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;

namespace Infrastructure.ExportManager;
using Microsoft.AspNetCore.Hosting;

public class EntityWriter
{
    public Campaign CampaignToDuplicate { get; set; }
    public List<Budget> ChildBudgets { get; set; }
    public List<Expense> ChildExpenses { get; set; }

    public EntityWriter(Campaign c, List<Budget> b, List<Expense> e)
    {
        CampaignToDuplicate = c;
        ChildBudgets = b;
        ChildExpenses = e;
    }
    
    public EntityWriter() {}

    public static (Campaign campaign, List<Budget> budgets, List<Expense> expense) Find(string campaignId, DataContext context)
    {
        var c = context.Campaign.Where(c => c.Id.Equals(campaignId)).First();
        List<Budget> b = context.Budget.Where(b => b.CampaignId.Equals(campaignId)).ToList();
        List<Expense> e = context.Expense.Where(e => e.CampaignId.Equals(campaignId)).ToList();
        return (c, b, e);

    }
    
    public static string GetDocsFolder(IWebHostEnvironment host)
    {
        return Path.Combine(host.WebRootPath, "app_data", "docs");
    }

    public string GetFileContent()
    {
        List<string> campaignLines = WriteCampaign();
        List<string> budgetLines = WriteBudgets();
        List<string> expenseLines = WriteExpenses();

        // Combine all lines with a newline separator
        return string.Join("\n", campaignLines.Concat(budgetLines).Concat(expenseLines));
    }


    public List<string> WriteCampaign()
    {
        List<string> campaignLines = new List<string>();

        campaignLines.Add("---CAMPAIGN---");
        campaignLines.Add(string.Join(";", new string[]
        {
            CampaignToDuplicate.Number + " copy",  
            CampaignToDuplicate.Title, 
            CampaignToDuplicate.Description, 
            CampaignToDuplicate.TargetRevenueAmount.ToString(), 
            CampaignToDuplicate.CampaignDateStart.ToString(), 
            CampaignToDuplicate.CampaignDateFinish.ToString(), 
            (int)(CampaignToDuplicate.Status ?? 0) + "", 
            CampaignToDuplicate.SalesTeamId, 
            CampaignToDuplicate.IsDeleted.ToString()
        }));

        return campaignLines;
    }

    public List<string> WriteBudgets()
    {
        List<string> budgetLines = new List<string>();

        budgetLines.Add("---BUDGET---");
        foreach (Budget b in ChildBudgets)
        {
            budgetLines.Add(string.Join(";", new string[]
            {
                b.Number,
                b.Title,
                b.Description,
                b.BudgetDate.ToString(),
                (int)(b.Status ?? 0) + "",
                b.Amount.ToString(),
                b.IsDeleted.ToString()
            }));
        }

        return budgetLines;
    }
    
    public List<string> WriteExpenses()
    {
        List<string> budgetLines = new List<string>();

        budgetLines.Add("---EXPENSE---");
        foreach (Budget b in ChildBudgets)
        {
            budgetLines.Add(string.Join(";", new string[]
            {
                b.Number,
                b.Title,
                b.Description,
                b.BudgetDate.ToString(),
                (int)(b.Status ?? 0) + "",
                b.Amount.ToString(),
                b.IsDeleted.ToString()
            }));
        }

        return budgetLines;
    }
}