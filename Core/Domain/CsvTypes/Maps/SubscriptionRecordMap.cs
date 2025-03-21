using CsvHelper.Configuration;
using Domain.CsvTypes.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Maps
{
    public class SubscriptionRecordMap : ClassMap<SubscriptionRecord>
    {
        public SubscriptionRecordMap() 
        {
            Map(m => m.FromDate).Name("FromDate");
            Map(m => m.ToDate).Name("ToDate");
            Map(m => m.ClientId).Name("ClientId");
            Map(m => m.SubscriptionId).Name("SubscriptionId");
        }
    }
}
