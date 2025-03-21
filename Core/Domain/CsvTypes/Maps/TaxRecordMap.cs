using CsvHelper.Configuration;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Maps
{
    public class TaxRecordMap : ClassMap<Tax>
    {
        public TaxRecordMap() 
        {
            Map(t => t.Name).Name("Name");
            Map(t => t.Description).Name("Description");
            Map(t => t.Percentage).Name("Percentage");
        }
    }
}
