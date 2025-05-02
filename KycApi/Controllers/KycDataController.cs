using KycApi.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KycApi.Controllers
{
    [Route("kyc-data")]
    [ApiController]
    public class KycDataController : ControllerBase
    {
        private readonly IAggregatedKycService _aggregatedKycService;
        public KycDataController(IAggregatedKycService aggregatedKycService)
        {
            _aggregatedKycService = aggregatedKycService;
        }

        // get aggregated kyc data of a customer
        [HttpGet("{ssn}")]
        public async Task<IActionResult> GetAggregatedKycData(string ssn)
        {
            try
            {
                var aggregatedKyc = await _aggregatedKycService.GetAggregatedKycData(ssn);
                if (aggregatedKyc == null)
                {
                    return NotFound("Customer not found");
                }
                return Ok(aggregatedKyc);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
