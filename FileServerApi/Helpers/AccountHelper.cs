using FileServer.Core.Entities;
using FileServer.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace FileServer.Api.Helpers
{
    public static class AccountHelper
    {

        public async static Task<bool> EmailExists(string email, UserManager<IdentityUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return true;
        }

        public async static Task InitializeNewUserQuota(IdentityUser user, FileContext _fileContext)
        {
            var fileUser = new AppUser() { Id  = user.Id };
            var UserQuota = new StorageQuota() { UserId = user.Id };
            await _fileContext.Users.AddAsync(fileUser);
            await _fileContext.StorageQuotas.AddAsync(UserQuota);
            await _fileContext.SaveChangesAsync();
        }

        public async static Task<IdentityUser?> GetUser(string email, UserManager<IdentityUser> userManager)
           => await userManager.FindByEmailAsync(email);

        public async static Task<bool> RoleExists(string RoleName, RoleManager<IdentityRole> roleManager)
            => await roleManager.RoleExistsAsync(RoleName);

        public async static Task<IdentityUser?> GetUserById(string id  , UserManager<IdentityUser> userManager)
            => await userManager.FindByIdAsync(id);
        
    }
}
