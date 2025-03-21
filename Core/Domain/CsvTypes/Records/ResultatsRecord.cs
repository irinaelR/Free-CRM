using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Records
{
    public class ResultatsRecord
    {
        public int EtapeRang {  get; set; }
        public int NumDossard { get; set; }
        public string Nom {  get; set; }
        public string Genre { get; set; }
        public DateOnly DateNaissance {  get; set; }
        public string Equipe { get; set; }
        public DateTime Arrivee {  get; set; }
    }
}
