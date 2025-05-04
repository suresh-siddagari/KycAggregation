using KycApi.Model;
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

        /// <summary>
        /// Get aggregated KYC data by SSN
        /// </summary>
        /// <param name="ssn">customer social security number</param>
        /// <returns>Aggregated KYC data</returns>
        [HttpGet("{ssn}")]
        [ProducesResponseType(typeof(AggregatedKyc), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAggregatedKycData(string ssn)
        {
            try
            {
                var aggregatedKyc = await _aggregatedKycService.GetAggregatedKycData(ssn);
                if (aggregatedKyc == null)
                {
                    return NotFound(new { error = "Customer data not found for the provided SSN." });
                }
                return Ok(aggregatedKyc);
            }
            catch (Exception ex)
            {
                //TODO:log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred while processing the request." });
            }
        }
    }
}