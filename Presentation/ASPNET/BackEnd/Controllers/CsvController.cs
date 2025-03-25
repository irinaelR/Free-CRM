using ASPNET.BackEnd.Common.Models;
using Domain.Common;
using Domain.CsvTypes.Maps;
using Domain.CsvTypes.Records;
using Domain.Entities;
using Infrastructure.CsvManager;
using Infrastructure.CsvManager.Data;
using Infrastructure.CsvManager.Errors;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class CsvController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly CsvService _csvService;
        private readonly DataImportService _dataImportService;
        private readonly DataContext _dataContext;

        public CsvController (IWebHostEnvironment webHost, CsvService csvService, DataImportService dataImportService, DataContext dataContext)
        {
            _webHost = webHost;
            _csvService = csvService;
            _dataImportService = dataImportService;
            _dataContext = dataContext;
        }
        
        [HttpPost("ReadFile")]
        public ActionResult<object[]> ReadCsvFiles(
            [FromBody] List<CsvImportOptions> options
        )
        {
            try
            {
                var incompleteCampaigns =
                    _csvService.ProcessCsv<IncompleteCampaignRecord, IncompleteCampaignRecordMap>(options[0], _webHost,
                        null);
                var campaignChildren =
                    _csvService.ProcessCsv<CampaignChildRecord, CampaignChildRecordMap>(options[1], _webHost,
                        incompleteCampaigns);
                var (campaigns, budgets, expenses) = _dataImportService.CreateAndSave(incompleteCampaigns, campaignChildren, _dataContext);
                
                return Ok(new object[] { campaigns, budgets, expenses });
            }
            catch (CsvProcessException cpe)
            {
                var error = new
                {
                    message = cpe.Message,
                    rows = cpe.BadDataList,
                };
                return BadRequest(error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [HttpPost("ReadSingle")]
        public ActionResult<string> ReadCsvFile(
            [FromBody] CsvImportOptions options
        )
        {
            try
            {
                // var records = _csvService.ProcessCsvDynamic(options.TableRecord, options, _webHost, _dataContext);
                var records = _csvService.ProcessCsv<IncompleteCampaignRecord, IncompleteCampaignRecordMap>(options, _webHost, null);
                //_dataContext.Tax.AddRange(records);
                //_dataContext.SaveChanges();
                return Ok(records);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("GetRecords")]
        public ActionResult<string> GetRecordsWithMaps()
        {
            try
            {
                var records = _csvService.GetRecordsWithMaps();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("Persist")]
        public async Task<ActionResult<string>> PersistAllData([FromBody] List<RecordRequest> records)
        {
            try
            {
                await _csvService.SaveAll(records, _dataContext);
                return Ok("ok");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
