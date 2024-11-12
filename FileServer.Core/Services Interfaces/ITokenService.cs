using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Services_Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(IdentityUser user);
    }
}
