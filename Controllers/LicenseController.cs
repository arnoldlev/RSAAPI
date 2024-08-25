using Microsoft.AspNetCore.Mvc;
using RSAAPI.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace RSAAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpGet("/validateLicense")]
        public async Task<IActionResult> ValidateLicense([Required][FromQuery] string key)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _licenseService.ValidateLicense(key);

            return Ok(result);
        }
    }
}
