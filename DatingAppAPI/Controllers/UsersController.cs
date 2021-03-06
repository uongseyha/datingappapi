﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Helpers;
using DatingAppAPI.Data;
using DatingAppAPI.DTO;
using DatingAppAPI.Helper;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repos;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            this._repos = datingRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repos.GetUser(currentUserId, true);
            userParams.UserId = currentUserId;

            if (String.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";

            var users = await _repos.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repos.GetUser(id,true);
            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromDto = await _repos.GetUser(id,true);
            _mapper.Map(userForUpdateDto, userFromDto);

            if (await _repos.SaveAll()) return NoContent();

            throw new Exception($"Updating user {id} fail on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repos.GetLike(id, recipientId);

            if (like != null) return BadRequest("You already like the user");

            if (await _repos.GetUser(recipientId, true) == null) return NotFound();

            like = new Models.Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repos.Add<Like>(like);

            if (await _repos.SaveAll()) 
                return Ok();

            return BadRequest("Failed to like the user");
        }
    }
}