using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Records
{
    public class SubscriptionRecord
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ClientId { get; set; }
        public string SubscriptionId { get; set; }
    }
}
