using KycApi.Service.Interface;
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
                    return NotFound("Customer data not found for the provided SSN.");
                }
                return Ok(aggregatedKyc);
            }
            catch (Exception ex)
            {
                //TODO:log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing the request");
            }
        }
    }
}