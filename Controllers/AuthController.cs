using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc;
using RSAAPI.Models;
using RSAAPI.Services;

namespace RSAAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _cognitoService;

        public AuthController(AuthService cognitoService)
        {
            _cognitoService = cognitoService;
        }

        [HttpGet("user/{subid}")]
        public async Task<IActionResult> GetUser([FromRoute] string subid)
        {
            try
            {
                var userResponse = await _cognitoService.GetUserDetailsAsync(subid);
 
                var filteredAttributes = userResponse.UserAttributes
                    .Where(attr => new[] { "custom:licensekey", "profile", "locale" }.Contains(attr.Name))
                    .ToDictionary(attr => attr.Name, attr => attr.Value);

                return Ok(new UserUpdateModel
                {
                    LicenseKey = filteredAttributes["custom:licensekey"],
                    ApiKey = filteredAttributes["profile"],
                    SandboxKey = filteredAttributes["locale"]
                }); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("user/{subid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string subid, [FromBody] UserUpdateModel model)
        {
            try
            {
                var authResponse = await _cognitoService.UpdateUserAttributeAsync(subid, model.LicenseKey, model.SandboxKey, model.ApiKey);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestModel model)
        {
            try
            {
                var authResponse = await _cognitoService.SignInAsync(model.Email, model.Password);
                return Ok(authResponse); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel model)
        {
            try
            {
                var response = await _cognitoService.SignUpAsync(model.Email, model.Password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-signup")]
        public async Task<IActionResult> ConfirmSignUp([FromBody] ConfirmSignUpRequestModel model)
        {
            try
            {
                var response = await _cognitoService.ConfirmSignUpAsync(model.Email, model.ConfirmationCode);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            try
            {
                var response = await _cognitoService.ForgotPasswordAsync(model.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm-forgot-password")]
        public async Task<IActionResult> ConfirmForgotPassword([FromBody] ConfirmForgotPasswordRequestModel model)
        {
            try
            {
                var response = await _cognitoService.ConfirmForgotPasswordAsync(model.Email, model.ConfirmationCode, model.NewPassword);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel model)
        {
            try
            {
                var authResponse = await _cognitoService.RefreshTokenAsync(model.RefreshToken);
                return Ok(authResponse); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
