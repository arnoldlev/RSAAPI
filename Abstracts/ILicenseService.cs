using RSAAPI.Models;

namespace RSAAPI.Abstracts
{
    public interface ILicenseService
    {
        Task<LicenseResult?> ValidateLicense(string key);
    }
}
