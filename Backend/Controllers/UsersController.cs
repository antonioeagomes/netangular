using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers() => Ok(await _userRepository.GetMembersAsync());

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username) => Ok(await _userRepository.GetMemberAsync(username.ToLower()));

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdate)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        _mapper.Map(memberUpdate, user);

        _userRepository.Update(user);

        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }

}
