using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Entities;

namespace Backend.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);

    Task<MemberDTO> GetMemberAsync(string username);
    Task<IEnumerable<MemberDTO>> GetMembersAsync();
}
