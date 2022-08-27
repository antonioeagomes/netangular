using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO;

public class UserToRegisterDTO
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

public class UserToLoginDTO
{    
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserDTO
{
    public string Username { get; set; }

    public string Token { get; set; }

    public string PhotoUrl { get; set; }
}

