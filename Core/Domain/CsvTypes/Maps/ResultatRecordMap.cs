using CsvHelper.Configuration;
using Domain.CsvTypes.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CsvTypes.Maps
{
    public class ResultatRecordMap : ClassMap<ResultatsRecord>
    {
        public ResultatRecordMap() 
        {
            Map(r => r.EtapeRang).Name("etape_rang");
            Map(r => r.NumDossard).Name("numero dossard");
            Map(r => r.Nom).Name("nom");
            Map(r => r.Genre).Name("genre");
            Map(r => r.DateNaissance).Name("date naissance");
            Map(r => r.Equipe).Name("equipe");
            Map(r => r.Arrivee).Name("arrivée");
        }
    }
}
