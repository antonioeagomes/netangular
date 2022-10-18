using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Name)?.Value;

    public static int GetUserId(this ClaimsPrincipal user) => int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

}