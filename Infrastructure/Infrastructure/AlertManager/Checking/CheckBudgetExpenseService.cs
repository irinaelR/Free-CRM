using Application.Features.ExpenseManager.Commands;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataAccessManager.EFCore.Contexts;

namespace Infrastructure.AlertManager.Checking;

public class CheckBudgetExpenseService
{
    private DataContext _context;
    private NumberSequenceService _numberSequenceService;

    public CheckBudgetExpenseService(DataContext context, NumberSequenceService numberSequenceService)
    {
        _context = context;
        _numberSequenceService = numberSequenceService;
    }

    private double? GetAllConfirmedBudgets(string campaignId, DateTime maxDate)
    {
        return _context.Budget
            .Where(x => x.CampaignId == campaignId && x.BudgetDate <= maxDate && x.Status == BudgetStatus.Confirmed)
            .Sum(x => x.Amount);
    }

    private double? GetAllConfirmedExpenses(string campaignId, DateTime maxDate)
    {
        return _context.Expense
            .Where(x => x.CampaignId == campaignId && x.ExpenseDate <= maxDate && x.Status == ExpenseStatus.Confirmed)
            .Sum(x => x.Amount);
    }

    private AlertConfig? GetAlertConfig()
    {
        return _context.AlertConfigs.Find("AC001");
    }

    private bool CheckBudgetExpense(string? campaignId, double? newAmount, DateTime maxDate)
    {
        double? budget = GetAllConfirmedBudgets(campaignId, maxDate);
        
        double? expenses = GetAllConfirmedExpenses(campaignId, maxDate);
        expenses += newAmount;
        
        AlertConfig? config = GetAlertConfig();
        double percentage = config.Percentage;
        
        return budget != null && expenses != null && expenses < (budget * percentage / 100);
    }

    public bool CheckBudgetExpenseRequest(CreateExpenseRequest request)
    {
        return CheckBudgetExpense(request.CampaignId, request.Amount, request.ExpenseDate ?? DateTime.UtcNow);
    }
}