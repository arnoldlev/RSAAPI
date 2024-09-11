using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using RSAAPI.Abstracts;
using RSAAPI.Database;
using RSAAPI.Messaging;
using RSAAPI.Migrations;
using RSAAPI.Models;

namespace RSAAPI.Services
{
    public class UserService : IUserService
    {
        private readonly RSAContext _context;
        private readonly EncryptionService _encryptionService;

        public UserService(RSAContext context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<DbUser> GetOrCreateUser(string email)
        {
            DbUser? dbuser = await _context.Users.Where(e => e.Email == email).FirstOrDefaultAsync();
            if (dbuser == null) {
                dbuser = new DbUser() { Email = email };
                await _context.AddAsync(dbuser);
                await _context.SaveChangesAsync();
            } 
            return dbuser;
        }
        //new UserDto { Email = email, SandBoxToken = dbuser.SandBoxToken, ApiToken = dbuser.ApiToken, LicenseKey = dbuser.LicenseKey };

        public async Task<UserDto> SaveUserAsync(string email, UserDto user)
        {
            var result = await GetOrCreateUser(email);

            result.ModifiedDate = DateTime.Now;
            result.LicenseKey = user.LicenseKey ?? result.LicenseKey;
            result.ApiToken = (user.ApiToken != null && user.ApiToken.Length > 1) ? 
                                await _encryptionService.EncryptData(user.ApiToken) : result.ApiToken;
            result.SandBoxToken = (user.SandboxToken != null && user.SandboxToken.Length > 1) ?
                                await _encryptionService.EncryptData(user.SandboxToken) : result.SandBoxToken;
            
            await _context.SaveChangesAsync();
            return new UserDto { Email = email };
        }
    }
}
