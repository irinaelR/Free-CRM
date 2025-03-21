using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Trigger).Name("Trigger");
            Map(m => m.Words).Name("Words");
        }
    }
}
