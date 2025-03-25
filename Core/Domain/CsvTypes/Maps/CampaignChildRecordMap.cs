using CsvHelper.Configuration;
using Domain.CsvTypes.Records;

namespace Domain.CsvTypes.Maps;

public class CampaignChildRecordMap : ClassMap<CampaignChildRecord>
{
    public CampaignChildRecordMap()
    {
        Map(cc => cc.CampaignNumber).Name("Campaign_number");
        Map(cc => cc.Title).Name("Title");
        Map(cc => cc.Type).Name("Type");
        Map(cc => cc.Date).Name("Date");
        Map(cc => cc.Amount).Name("Amount");
    }
}