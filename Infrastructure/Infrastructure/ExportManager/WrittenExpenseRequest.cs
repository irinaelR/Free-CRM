using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.ExportManager;

public class WrittenExpenseRequest
{
    public string Number { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ExpenseDate { get; set; }
    public int Status { get; set; }
    public double Amount { get; set; }
    public string CampaignId { get; set; }

    public Expense MakeExpense()
    {
        return new Expense()
        {
            Number = this.Number,
            Title = this.Title,
            Description = this.Description,
            ExpenseDate = this.ExpenseDate,
            Status = (ExpenseStatus)this.Status,
            Amount = this.Amount,
            CampaignId = this.CampaignId
        };
    }
}