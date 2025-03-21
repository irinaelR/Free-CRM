using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CsvManager
{
    public class CsvImportOptions
    {
        public string FileName { get; set; }
        public char Delimiter { get; set; } = ','; // Default delimiter is comma
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd"; // Default date format
        public bool HasHeaderRecord { get; set; } = true;
        public bool TrimFields { get; set; } = true;
        public string TableRecord { get; set; }
    }
}
