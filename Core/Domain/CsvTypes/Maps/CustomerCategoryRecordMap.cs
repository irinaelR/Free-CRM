using CsvHelper.Configuration;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Maps
{
    public class CustomerCategoryRecordMap : ClassMap<CustomerCategory>
    {
        public CustomerCategoryRecordMap() 
        {
            Map(c => c.Name).Name("Name");
            Map(c => c.Description).Name("Description");
        }
    }
}
