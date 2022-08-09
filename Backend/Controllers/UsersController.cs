using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Backend.Data;
using Backend.DTO;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers() => Ok(await _userRepository.GetMembersAsync());

    [Authorize]
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username) => Ok(await _userRepository.GetMemberAsync(username.ToLower()));
    
}
