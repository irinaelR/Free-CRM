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

    private double? GetAllConfirmedBudgets(string campaignId)
    {
        return _context.Budget
            .Where(x => x.CampaignId == campaignId && x.Status == BudgetStatus.Confirmed)
            .Sum(x => x.Amount);
    }

    private double? GetAllConfirmedExpenses(string campaignId)
    {
        return _context.Expense
            .Where(x => x.CampaignId == campaignId && x.Status == ExpenseStatus.Confirmed)
            .Sum(x => x.Amount);
    }

    private AlertConfig? GetAlertConfig()
    {
        return _context.AlertConfigs.Find("AC001");
    }

    private bool CheckBudgetExpense(string? campaignId, double? newAmount)
    {
        double? budget = GetAllConfirmedBudgets(campaignId);
        
        double? expenses = GetAllConfirmedExpenses(campaignId);
        expenses += newAmount;
        
        AlertConfig? config = GetAlertConfig();
        double percentage = config.Percentage;
        
        return budget != null && expenses != null && expenses < (budget * percentage / 100);
    }

    public bool CheckBudgetExpenseRequest(CreateExpenseRequest request)
    {
        return CheckBudgetExpense(request.CampaignId, request.Amount);
    }
}