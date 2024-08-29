using Microsoft.EntityFrameworkCore;
using RSAAPI.Abstracts;
using RSAAPI.Database;
using RSAAPI.Messaging;
using RSAAPI.Models;

namespace RSAAPI.Services
{
    public class UserService : IUserService
    {
        private readonly RSAContext _context;

        public UserService(RSAContext context)
        {
            _context = context;
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

        public async Task SaveUserAsync(string email, UserDto user)
        {
            var result = await GetOrCreateUser(email);

            result.ModifiedDate = DateTime.Now;
            result.LicenseKey = user.LicenseKey ?? result.LicenseKey;
            result.ApiToken = user.ApiToken ?? result.ApiToken;
            result.SandBoxToken = user.SandBoxToken ?? result.SandBoxToken;

            await _context.SaveChangesAsync();
        }
    }
}
