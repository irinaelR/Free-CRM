using CsvHelper.Configuration;
using Domain.CsvTypes.Records;

namespace Domain.CsvTypes.Maps;

public class IncompleteCampaignRecordMap : ClassMap<IncompleteCampaignRecord>
{
    public IncompleteCampaignRecordMap()
    {
        Map(m => m.Code).Name("campaign_code");
        Map(m => m.Title).Name("campaign_title");
    }
}