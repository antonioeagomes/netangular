using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Entities;

namespace Backend.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}
