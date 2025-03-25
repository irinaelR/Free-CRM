namespace Domain.CsvTypes.Records;

public class IncompleteCampaignRecord
{
    public string Code { get; set; }
    public string Title { get; set; }

    public string CheckLogic(List<IncompleteCampaignRecord> incompleteCampaignRecords)
    {
        if (incompleteCampaignRecords.Select(c => c.Code).Contains(Code))
        {
            return "Campaign number is a duplicate.";
        }

        return "";
    }
}