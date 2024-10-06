using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using RSAAPI.Models;

namespace RSAAPI.Services
{
    public class AuthService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly string _clientId;
        private readonly string _userPoolId;

        public AuthService(IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config)
        {
            _cognitoClient = cognitoClient;
            _clientId = config["Cognito:ClientId"];
            _userPoolId = config["Cognito:UserPoolId"];
        }

        public async Task<AdminUpdateUserAttributesResponse> UpdateUserAttributeAsync(string sub, string? licenseKey, string? sandbox, string? apikey)
        {
            var list = new List<AttributeType>();

            if (licenseKey != null)
            {
                list.Add(new() { Name = "custom:licensekey", Value = licenseKey });
            }
            if (sandbox != null)
            {
                list.Add(new() { Name = "locale", Value = sandbox });
            }
            if (apikey != null)
            {
                list.Add(new() { Name = "profile", Value = apikey });
            }

            var request = new AdminUpdateUserAttributesRequest
            {
                UserPoolId = _userPoolId,
                Username = sub,
                UserAttributes = list
            };

            return await _cognitoClient.AdminUpdateUserAttributesAsync(request);
        }

        public async Task<AdminGetUserResponse> GetUserDetailsAsync(string sub)
        {
            var request = new AdminGetUserRequest
            {
                UserPoolId = _userPoolId, // Your Cognito User Pool ID
                Username = sub // Username or Sub (the unique user identifier)
            };

            var response = await _cognitoClient.AdminGetUserAsync(request);
            return response;
        }

        public async Task<SignUpResponse> SignUpAsync(string email, string password)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _clientId,
                Username = email,
                Password = password,
                UserAttributes = new List<AttributeType>
                {
                    new() { Name = "name", Value = email }
                }
            };
            return await _cognitoClient.SignUpAsync(signUpRequest);
        }

        public async Task<ConfirmSignUpResponse> ConfirmSignUpAsync(string email, string confirmationCode)
        {
            var confirmRequest = new ConfirmSignUpRequest
            {
                ClientId = _clientId,
                Username = email,
                ConfirmationCode = confirmationCode
            };
            return await _cognitoClient.ConfirmSignUpAsync(confirmRequest);
        }

        public async Task<CognitoAuthResponse> SignInAsync(string email, string password)
        {
            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "PASSWORD", password }
                }
             };

            var authResponse = await _cognitoClient.InitiateAuthAsync(authRequest);

            if (authResponse.AuthenticationResult != null)
            {
                return new CognitoAuthResponse
                {
                    IdToken = authResponse.AuthenticationResult.IdToken,
                    AccessToken = authResponse.AuthenticationResult.AccessToken,
                    RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                    ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
                };
            }

            throw new Exception("Authentication failed.");
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
        {
            var forgotPasswordRequest = new ForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = email
            };
            return await _cognitoClient.ForgotPasswordAsync(forgotPasswordRequest);
        }

        public async Task<ConfirmForgotPasswordResponse> ConfirmForgotPasswordAsync(string email, string confirmationCode, string newPassword)
        {
            var confirmForgotPasswordRequest = new ConfirmForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = email,
                ConfirmationCode = confirmationCode,
                Password = newPassword
            };
            return await _cognitoClient.ConfirmForgotPasswordAsync(confirmForgotPasswordRequest);
        }

        public async Task<CognitoAuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var authRequest = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
             };

            var authResponse = await _cognitoClient.InitiateAuthAsync(authRequest);

            if (authResponse.AuthenticationResult != null)
            {
                return new CognitoAuthResponse
                {
                    IdToken = authResponse.AuthenticationResult.IdToken,
                    AccessToken = authResponse.AuthenticationResult.AccessToken,
                    RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                    ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
                };
            }

            throw new Exception("Refresh token failed.");
        }

    }
}
