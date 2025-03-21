using Domain.Common;
using Domain.CsvTypes.Maps;
using Domain.CsvTypes.Records;
using Domain.Entities;
using Infrastructure.CsvManager;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class CsvController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly CsvService _csvService;
        private readonly DataContext _dataContext;

        public CsvController (IWebHostEnvironment webHost, CsvService csvService, DataContext dataContext)
        {
            _webHost = webHost;
            _csvService = csvService;
            _dataContext = dataContext;
        }

        [HttpPost("ReadFile")]
        public ActionResult<string> ReadCsvFile(
            [FromBody] CsvImportOptions options
        )
        {
            try
            {
                var records = _csvService.ProcessCsv<ResultatsRecord, ResultatRecordMap>(options, _webHost);
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
    }
}
