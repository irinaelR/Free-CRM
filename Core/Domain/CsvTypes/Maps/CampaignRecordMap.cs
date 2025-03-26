using CsvHelper.Configuration;
using Domain.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Maps
{
    public class CampaignRecordMap : ClassMap<Campaign>
    {
        public CampaignRecordMap()
        {
            Map(c => c.Title).Name("Title");
            Map(c => c.Description).Name("Description");
            Map(c => c.TargetRevenueAmount).Name("TargetRevenueAmount");
            Map(c => c.CampaignDateStart).Name("CampaignDateStart");
            Map(c => c.CampaignDateFinish).Name("CampaignDateFinish");
            Map(c => c.Status).Name("Status");

        }
    }
}
