using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSAAPI.Abstracts;
using RSAAPI.Services;
using RSAAPI.Messaging;
using System.ComponentModel.DataAnnotations;

namespace RSAAPI.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILicenseService _licenseService;
        private readonly EncryptionService _encryptionService;

        public UserController(IUserService userService, ILicenseService licenseService, EncryptionService encryptionService)
        {
            _userService = userService;
            _licenseService = licenseService;
            _encryptionService = encryptionService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            if (userEmail == null) return NotFound();

            var result = await _userService.GetOrCreateUser(userEmail);
            return Ok(new UserDto
                    {
                        Email = userEmail,
                        ApiToken = (result.ApiToken != null) ? await _encryptionService.DecryptData(result.ApiToken) : null,
                        SandboxToken = (result.SandBoxToken != null) ? await _encryptionService.DecryptData(result.SandBoxToken) : null,
                        LicenseKey = result.LicenseKey
                    }
            );
        }

        [HttpPost("license")]
        public async Task<IActionResult> SaveLicense([Required][FromQuery] string key)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            if (userEmail == null) return NotFound();

            var result = await _licenseService.ValidateLicense(key);
            if (result == null) return BadRequest(result);

            if (result.IsValid)
            {
                await _userService.SaveUserAsync(userEmail, new UserDto { LicenseKey = key });
            }
            return Ok(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Save([Required][FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            if (userEmail == null) return NotFound();

            var result = await _userService.SaveUserAsync(userEmail, userDto);
            return Ok(result);

        }


    }
}
