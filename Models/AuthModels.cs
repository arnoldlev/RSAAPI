namespace RSAAPI.Models
{

    public class SignUpRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ConfirmSignUpRequestModel
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class SignInRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordRequestModel
    {
        public string Email { get; set; }
    }

    public class ConfirmForgotPasswordRequestModel
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
        public string NewPassword { get; set; }
    }

    public class CognitoAuthResponse
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
    public class RefreshTokenRequestModel
    {
        public string RefreshToken { get; set; }
    }

    public class UserUpdateModel
    {
        public string LicenseKey { get; set; }
        public string SandboxKey { get; set; }
        public string ApiKey { get; set; }
    }
}
