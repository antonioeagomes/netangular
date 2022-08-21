using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO;

public class MemberDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public int Age { get; set; }
    public string KnownAs { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PhotoUrl { get; set; }
    public ICollection<PhotoDto> Photos { get; set; }

}

public class MemberUpdateDTO
{
    public int Id { get; set; }
    public string Introduction { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

}