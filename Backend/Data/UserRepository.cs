using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.DTO;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;
public class UserRepository : IUserRepository
{
    private readonly ILogger<AppUser> _logger;
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, ILogger<AppUser> logger, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDTO> GetMemberAsync(string username) => await _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync() => await _context.Users
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task<AppUser> GetUserByIdAsync(int id) => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    public async Task<AppUser> GetUserByUsernameAsync(string username) => await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    public async Task<IEnumerable<AppUser>> GetUsersAsync() => await _context.Users.ToListAsync();
    public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
    public void Update(AppUser user) => _context.Entry(user).State = EntityState.Modified;
}