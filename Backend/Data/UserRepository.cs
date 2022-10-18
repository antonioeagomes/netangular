using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.DTO;
using Backend.Entities;
using Backend.Helpers;
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

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);

        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        switch (userParams.OrderBy)
        {
            case "created":
                query = query.OrderByDescending(m => m.CreateAt);
                break;
            default:
                query = query.OrderByDescending(m => m.LastActive);
                break;
        }

        return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(_mapper
                .ConfigurationProvider).AsNoTracking(),
                    userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser> GetUserByIdAsync(int id) => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    public async Task<AppUser> GetUserByUsernameAsync(string username) => await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.UserName == username);
    public async Task<IEnumerable<AppUser>> GetUsersAsync() => await _context.Users.ToListAsync();
    public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
    public void Update(AppUser user) => _context.Entry(user).State = EntityState.Modified;
}
