using AutoMapper;
using Backend.Data;
using Backend.DTO;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;
public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
    ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(UserToRegisterDTO userToRegister)
    {

        if (await UserExists(userToRegister.Username)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(userToRegister);

        user.UserName = userToRegister.Username.ToLower();

        var result = await _userManager.CreateAsync(user, userToRegister.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);


        return Ok(
            new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            });

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(UserToLoginDTO userToLogin)
    {

        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(u => u.UserName == userToLogin.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, userToLogin.Password, false);

        if (!result.Succeeded) return Unauthorized();        

        return Ok(new UserDTO
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
            KnownAs = user.KnownAs
        });

    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
}
