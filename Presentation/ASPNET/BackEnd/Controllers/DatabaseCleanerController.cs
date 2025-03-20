using Application.Common.Services.DatabaseCleaningManager;
using Application.Features.CustomerManager.Commands;
using ASPNET.BackEnd.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class DatabaseCleanerController : Controller
    {
        private readonly IDatabaseCleanerService _databaseService;

        public DatabaseCleanerController(IDatabaseCleanerService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("WipeDatabase")]
        public async Task<ActionResult<string>> WipeDatabase(
            [FromQuery] bool includeDemoData = false  
        )
        {
            try
            {
                await _databaseService.RecreateDatabase(includeDemoData);

                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("RepopulateDatabase")]
        public ActionResult<string> RepopulateDatabase()
        {
            try
            {
                _databaseService.RepopulateWithDemo();

                return Ok("ok");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
