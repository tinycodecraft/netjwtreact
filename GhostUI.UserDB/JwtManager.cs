using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using static GhostUI.Abstraction.Interfaces;

namespace GhostUI.UserDB
{
    public class JwtManager : IJwtManager
    {
        private readonly UserManager<IdentityUser> mgr;
        public JwtManager(UserManager<IdentityUser> itmgr)
        {
            mgr = itmgr;
        }



        public async Task<string> Register(string username, string password)
        {
            var result = await mgr.CreateAsync(new IdentityUser { UserName = username, Email = $"{username}@mail.com" }, password);
            
            if (result.Succeeded)
            {
                return "";
            }
            return  string.Join("\n", result.Errors.Select(e=> e.Description));
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await mgr.FindByNameAsync(username);
            
            if (user == null)
            {
                return $"{username} could not be found";

            }

            var valid = await mgr.CheckPasswordAsync(user, password);


            if (!valid)
            {
                var token = await mgr.GeneratePasswordResetTokenAsync(user);
                var succeeded = await mgr.ResetPasswordAsync(user, token, "abc123");
                return $"{username} does not match the password in DB, it's now reset to abc123";
            }
            
            return string.Empty;
        }
    }
}
