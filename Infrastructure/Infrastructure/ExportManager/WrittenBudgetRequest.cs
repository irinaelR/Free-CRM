using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.ExportManager;

public class WrittenBudgetRequest
{
    public string Number { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime BudgetDate { get; set; }
    public int Status { get; set; }
    public double Amount { get; set; }
    public string CampaignId { get; set; }

    public Budget MakeBudget()
    {
        return new Budget()
        {
            Number = this.Number,
            Title = this.Title,
            Description = this.Description,
            BudgetDate = this.BudgetDate,
            Status = (BudgetStatus)this.Status,
            Amount = this.Amount,
            CampaignId = this.CampaignId
        };
    }
}