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
using System.Collections;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CsvManager
{
    public class CsvService
    {
        public static readonly string _assembly = "Domain";
        public static readonly string _recordsNamespace = "Domain.CsvTypes.Records";
        public static readonly string _mapsNamespace = "Domain.CsvTypes.Maps";
        public static readonly string _entitiesNamespace = "Domain.Entities";
        public static readonly string _recordSuffix = "Record";
        public static readonly string _mapSuffix = "RecordMap";

        private readonly IServiceProvider serviceProvider;

        public CsvService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

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

        public static string GetDocsFolder(IWebHostEnvironment host)
        {
            return Path.Combine(host.WebRootPath, "app_data", "docs");
        }

        public async 
        Task
        SaveAll(List<RecordRequest> records, DataContext _dataContext)
        {
            if (records == null || !records.Any())
            {
                throw new ArgumentException("Invalid or empty data.");
            }

            var entitiesAssembly = Assembly.Load(_assembly);

            foreach (var record in records)
            {
                var entityType = entitiesAssembly.GetType($"{_entitiesNamespace}.{record.RecordName}");
                if (entityType == null)
                {
                    throw new Exception($"Entity '{record.RecordName}' not found (searched for {_entitiesNamespace}.{record.RecordName}).");
                }

                // Deserialize data to an array of objects (or list of entities)
                var dataArray = Newtonsoft.Json.JsonConvert.DeserializeObject(record.Data.ToString(), typeof(List<object>)) as List<object>;
                if (dataArray == null)
                {
                    throw new Exception($"Failed to deserialize data for '{record.RecordName}'.");
                }

                // For each object in the data array, deserialize to the entity type and add to context
                foreach (var dataItem in dataArray)
                {
                    var entity = Newtonsoft.Json.JsonConvert.DeserializeObject(dataItem.ToString(), entityType);
                    if (entity == null)
                    {
                        throw new Exception($"Failed to deserialize data item for '{record.RecordName}'.");
                    }

                    _dataContext.Add(entity);
                }
            }

            await _dataContext.SaveChangesAsync();
        }

        public List<object> ProcessCsvDynamic(string className, CsvImportOptions options, IWebHostEnvironment webHost, DataContext context)
        {
            // Resolve types
            Type tRecordType = Type.GetType($"{_entitiesNamespace}.{className}, {_assembly}");
            Type tMapType = Type.GetType($"{_mapsNamespace}.{className}RecordMap, {_assembly}");

            if (tRecordType == null || tMapType == null)
            {
                throw new InvalidOperationException($"Could not find types {className} or {className}RecordMap in assembly {_assembly}.");
            }

            // Get MethodInfo and create generic method
            MethodInfo method = typeof(CsvService)
                .GetMethod("ProcessCsv")
                .MakeGenericMethod(tRecordType, tMapType);

            // Invoke method
            var result = method.Invoke(this, new object[] { options, webHost, context });

            // return (List<object>)result;
            var specificList = result as IEnumerable;
            return specificList.Cast<object>().ToList();
        }

        public List<TRecord> ProcessCsv<TRecord, TMap>(CsvImportOptions options, IWebHostEnvironment webHost, DataContext context)
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
                var dateOnlyConversionOptions = new TypeConverterOptions { Formats = new[] { options.DateTimeFormat.Substring(0, 10) } };
                csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = conversionOptions.Formats;
                csv.Context.TypeConverterOptionsCache.GetOptions<DateOnly>().Formats = dateOnlyConversionOptions.Formats;
            }

            var records = csv.GetRecords<TRecord>().ToList();

            var fkMappings = GenerateMappings<TRecord>();
            //SetForeignKeys<TRecord>(records, csv, fkMappings, context); 

            return records;
        }

        public static (string FkProperty, string CsvHeader, string ParentProperty, Type ParentType)[] GenerateMappings<TChild>()
        {
            var childProperties = typeof(TChild).GetProperties();
            var namespaceName = typeof(TChild).Namespace;
            var mappings = new List<(string, string, string, Type)>();

            foreach (var property in childProperties)
            {
                if (property.Name.EndsWith("Id") && property.Name != "Id")
                {
                    var parentTypeName = property.Name.Substring(0, property.Name.Length - 2);
                    var parentType = Assembly.GetExecutingAssembly().GetTypes()
                        .FirstOrDefault(t => t.Name == parentTypeName && t.Namespace == namespaceName);

                    if (parentType != null)
                    {
                        mappings.Add((
                            property.Name,
                            parentTypeName + "Name",
                            "Name",
                            parentType
                        ));
                    }
                }
            }

            return mappings.ToArray();
        }

        //public static void SetForeignKeys<TChild>(
        //    List<TChild> childEntities,
        //    CsvReader csvReader,
        //    (string FkProperty, string CsvHeader, string ParentProperty, Type ParentType)[] mappings,
        //    DataContext dbContext
        //)
        //{
        //    var csvData = new List<Dictionary<string, string>>();
        //    while (csvReader.Read())
        //    {
        //        var row = mappings.ToDictionary(m => m.CsvHeader, m => csvReader.GetField(m.CsvHeader));
        //        csvData.Add(row);
        //    }

        //    if (csvData.Count - 1 != childEntities.Count)
        //        throw new ArgumentException("CSV data count must match the number of child entities.");

        //    foreach (var mapping in mappings)
        //    {
        //        var setMethod = typeof(DataContext).GetMethod(nameof(DataContext.Set), new Type[] { });
        //        var parentSet = setMethod?.MakeGenericMethod(mapping.ParentType).Invoke(dbContext, null) as IQueryable;
        //        if (parentSet == null)
        //            throw new InvalidOperationException($"Could not retrieve DbSet for {mapping.ParentType.Name}");

        //        for (int i = 0; i < childEntities.Count; i++)
        //        {
        //            var csvValue = csvData[i][mapping.CsvHeader];

        //            // Find the parent entity
        //            var parentEntity = parentSet.Cast<object>()
        //                .FirstOrDefault(e => EF.Property<object>(e, mapping.ParentProperty).ToString() == csvValue);

        //            if (parentEntity != null)
        //            {
        //                //// Set the FK on the child entity
        //                var fkProperty = typeof(TChild).GetProperty(mapping.FkProperty);
        //                if (fkProperty == null)
        //                    throw new InvalidOperationException($"FK property {mapping.FkProperty} not found on {typeof(TChild).Name}");

        //                //var parentKey = dbContext.Entry(parentEntity).Property("Id").CurrentValue; // Assuming primary key is 'Id'
        //                fkProperty.SetValue(childEntities[i], parentEntity);
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException($"Parent entity not found for value {csvValue} in {mapping.CsvHeader}");
        //            }
        //        }
        //    }
        //}
    }
}
