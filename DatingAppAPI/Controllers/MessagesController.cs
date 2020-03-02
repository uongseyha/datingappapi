using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repos;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            this._repos = datingRepository;
            this._mapper = mapper;
        }

        [HttpGet("{id}",Name ="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repos.GetMessage(id);

            if (messageFromRepo == null) return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repos.GetUser(userId, true);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await _repos.GetUser(messageForCreationDto.RecipientId,true);

            if (recipient == null) return BadRequest("Could not find the user");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repos.Add(message);

            if (await _repos.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            throw new Exception("Createing the message failed on save");
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId,[FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messagesFromRepo = await _repos.GetMessagesForUser(messageParams);

            var messages= _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessagesThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagesFromRepo = await _repos.GetMessageThread(userId, recipientId);

            var messagesThead = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messagesThead);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repos.GetMessage(id);

            if (messageFromRepo.SenderId == userId) messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientId == userId) messageFromRepo.RecipientDeleted = true;

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted) _repos.Delete(messageFromRepo);

            if (await _repos.SaveAll()) return NoContent();

            throw new Exception("Error on delete message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repos.GetMessage(id);

            if (messageFromRepo.RecipientId != userId) return Unauthorized();

            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;

            await _repos.SaveAll();

            return NoContent();
        }
    }
}