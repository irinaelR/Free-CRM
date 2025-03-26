using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.ExportManager;

public class WrittenCampaignRequest
{
    public string Number { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double TargetRevenueAmount { get; set; }
    public DateTime CampaignDateStart { get; set; }
    public DateTime CampaignDateFinish { get; set; }
    public int Status { get; set; }
    public string SalesTeamId { get; set; }

    public Campaign MakeCampaign()
    {
        return new Campaign()
        {
            Number = this.Number,
            Title = this.Title,
            Description = this.Description,
            TargetRevenueAmount = this.TargetRevenueAmount,
            CampaignDateStart = this.CampaignDateStart,
            CampaignDateFinish = this.CampaignDateFinish,
            Status = (CampaignStatus) this.Status,
            SalesTeamId = this.SalesTeamId,
            
        };
    }
}