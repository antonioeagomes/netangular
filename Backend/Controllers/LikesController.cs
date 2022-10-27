using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTO;
using Backend.Entities;
using Backend.Extensions;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUnityOfWork _unityOfWork;
    public LikesController(IUnityOfWork unityOfWork)
    {
        _unityOfWork = unityOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _unityOfWork.UserRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _unityOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _unityOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            LikedUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _unityOfWork.Complete()) return Ok();

        return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await _unityOfWork.LikesRepository.GetUserLikes(likesParams);

        Response.AddPaginationHeader(users.CurrentPage,
            users.PageSize, users.TotalCount, users.TotalPages);

        return Ok(users);
    }
}
