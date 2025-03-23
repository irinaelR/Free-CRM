using Domain.Entities;
using Infrastructure.AlertManager;
using Infrastructure.AlertManager.Checking;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertConfigController : ControllerBase
    {
        private readonly IAlertConfigService _alertConfigService;
        private readonly CheckBudgetExpenseService _checkService;

        public AlertConfigController(IAlertConfigService alertConfigService, CheckBudgetExpenseService checkService)
        {
            _alertConfigService = alertConfigService;
            _checkService = checkService;
        }

        [HttpPost("UpdateAlertConfig")]
        public async Task<IActionResult> UpdateAlertConfig([FromBody] AlertConfigRequest request)
        {

            try
            {
                var updatedConfig = await _alertConfigService.UpdateAlertConfigAsync(request.Id, request.Percentage);
                return Ok(updatedConfig);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred while updating the alert configuration. {ex.Message}");
            }
        }

        [HttpGet("GetAlertConfig")]
        public async Task<IActionResult> GetAlertConfig()
        {
            try
            {
                var alertConfig = await _alertConfigService.GetAlertConfigAsync("AC001");
                return Ok(alertConfig);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred while updating the alert configuration. {ex.Message}");
            }
        }

        // [HttpGet("Test")]
        // public List<Budget> GetAllConfirmed([FromQuery] string campaignId)
        // {
        //     return _checkService.GetAllConfirmed(campaignId);
        // }
    }
}
