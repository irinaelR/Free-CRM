using CsvHelper.Configuration;
using CsvHelper;
using Domain.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;

namespace Infrastructure.CsvManager
{
    public class CsvService
    {
        public static string GetDocsFolder (IWebHostEnvironment host)
        {
            return Path.Combine(host.WebRootPath, "app_data", "docs");
        }

        public List<TRecord> ProcessCsv<TRecord, TMap>(CsvImportOptions options, IWebHostEnvironment webHost)
    where TMap : ClassMap<TRecord>, new()
        {
            string docsFolder = GetDocsFolder(webHost);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeaderRecord,
                TrimOptions = options.TrimFields ? TrimOptions.Trim : TrimOptions.None,
                Mode = CsvMode.RFC4180,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using var reader = new StreamReader(Path.Combine(docsFolder, options.FileName));
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<TMap>();

            // Configure date parsing
            if (!string.IsNullOrEmpty(options.DateTimeFormat))
            {
                var conversionOptions = new TypeConverterOptions { Formats = new[] { options.DateTimeFormat } };
                csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = conversionOptions.Formats;
            }

            var records = csv.GetRecords<TRecord>().ToList();
            return records;
        }

    }
}
