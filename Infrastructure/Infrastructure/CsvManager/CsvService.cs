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
using System.Reflection;

namespace Infrastructure.CsvManager
{
    public class CsvService
    {
        public static readonly string _assembly = "Domain";
        public static readonly string _recordsNamespace = "Domain.CsvTypes.Records";
        public static readonly string _mapsNamespace = "Domain.CsvTypes.Maps";
        public static readonly string _recordSuffix = "Record";
        public static readonly string _mapSuffix = "RecordMap";

        public List<string> GetClassNamesWithoutSuffix(string assemblyName, string namespaceName, string suffix)
        {
            List<string> result = new List<string>();

            try
            {
                // Load the assembly by name
                Assembly assembly = Assembly.Load(assemblyName);

                // Get all types in the assembly
                Type[] types = assembly.GetTypes();

                // Filter types to only include those in the specified namespace and ending with the suffix
                var filteredTypes = types
                    .Where(t => t.Namespace == namespaceName && t.Name.EndsWith(suffix))
                    .Select(t => t.Name.Substring(0, t.Name.Length - suffix.Length))
                    .ToList();

                result.AddRange(filteredTypes);
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or rethrow as needed)
                Console.WriteLine($"Error loading types: {ex.Message}");
            }

            return result;
        }

        public List<string> GetRecordsNames()
        {
            return GetClassNamesWithoutSuffix(_assembly, _recordsNamespace, _recordSuffix);
        }

        public List<string> GetMapsNames()
        {
            return GetClassNamesWithoutSuffix(_assembly, _mapsNamespace, _mapSuffix);
        }

        public IEnumerable<dynamic> GetRecordsWithMaps()
        {
            // List<string> recordNames = GetRecordsNames();
            List<string> mapNames = GetMapsNames();

            var objectList = mapNames.Select(s => new { Name = s }).ToList();
            return objectList;
        }

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
                var dateOnlyConversionOptions = new TypeConverterOptions { Formats = new[] { options.DateTimeFormat.Substring(0,10) } };
                csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = conversionOptions.Formats;
                csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = dateOnlyConversionOptions.Formats;
            }

            var records = csv.GetRecords<TRecord>().ToList();
            return records;
        }

    }
}
