namespace Domain.CsvTypes.Records;

public class CampaignChildRecord
{
    public string CampaignNumber { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    
    public static string TYPE_BUDGET = "Budget";
    public static string TYPE_EXPENSE= "Expense";

    public string CheckLogic(List<IncompleteCampaignRecord> campaignRecords)
    {
        string message = "";
        if (!campaignRecords.Select(c => c.Code).Contains(CampaignNumber))
        {
            message += "Campaign does not exist. ";
        }

        if (Amount <= 0)
        {
            message += "Amount must be greater than 0. ";
        }

        if (!Type.Equals(TYPE_BUDGET, StringComparison.OrdinalIgnoreCase) && !Type.Equals(TYPE_EXPENSE, StringComparison.OrdinalIgnoreCase))
        {
            message += "Type must be either 'Budget' or 'Expense'. ";
        }
        
        return message;
    }
}