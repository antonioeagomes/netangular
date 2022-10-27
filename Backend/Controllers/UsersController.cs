using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Backend.Data;
using Backend.DTO;
using Backend.Entities;
using Backend.Extensions;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    private readonly IUnityOfWork _unityOfWork;

    public UsersController(IUnityOfWork unityOfWork, IMapper mapper, IPhotoService photoService)
    {
        _photoService = photoService;
        _mapper = mapper;
        _unityOfWork = unityOfWork;
    }

    // [Authorize(Roles = "Member")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        var user = await GetLoggedUser();
        userParams.CurrentUsername = user.UserName;

        var users = await _unityOfWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        return Ok(users);
    }

    // [Authorize(Roles = "Admin")]
    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username) => Ok(await _unityOfWork.UserRepository.GetMemberAsync(username.ToLower()));

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdate)
    {
        var user = await GetLoggedUser();

        _mapper.Map(memberUpdate, user);

        _unityOfWork.UserRepository.Update(user);

        if (await _unityOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user");
    }




    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await GetLoggedUser();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos?.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _unityOfWork.Complete()) return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

        return BadRequest("Something went wrong");

    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetPhotoAsMain(int photoId)
    {
        var user = await GetLoggedUser();
        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo.IsMain) return Ok("This photo is already the main");

        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        if (await _unityOfWork.Complete()) return Ok();

        return BadRequest("Failed to set this photo as main");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await GetLoggedUser();
        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("Cannot delete the main");

        if (!string.IsNullOrEmpty(photo.PublicId))
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _unityOfWork.Complete()) return Ok();

        return BadRequest("Failed to delete");
    }

    private async Task<AppUser> GetLoggedUser() => await _unityOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());


}
