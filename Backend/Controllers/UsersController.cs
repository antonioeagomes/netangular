using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class UsersController : BaseApiController
{
    private readonly ILogger<AppUser> _logger;
    private readonly DataContext _context;

    public UsersController(DataContext context, ILogger<AppUser> logger)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() => await _context.Users.ToListAsync();       

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUsers(int id) => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    
}
