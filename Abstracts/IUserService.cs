using RSAAPI.Messaging;
using RSAAPI.Models;

namespace RSAAPI.Abstracts
{
    public interface IUserService
    {
        Task<DbUser> GetOrCreateUser(string email);

        Task<UserDto> SaveUserAsync(string email, UserDto user);
    }
}
