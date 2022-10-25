using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

public class AppUser : IdentityUser<int>
{
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Introduction { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public ICollection<UserLike> LikedByUsers { get; set; }
    public ICollection<UserLike> LikedUsers { get; set; }
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }
    public ICollection<AppUserRole> UserRole { get; set; }
}
