using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAppAPI.Data;
using DatingAppAPI.DTO;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository authRepository,IConfiguration config)
        {
            this._authRepository = authRepository;
            this._config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterRequest userRegister)
        {
            userRegister.userName = userRegister.userName.ToLower();

            if (await _authRepository.UserExist(userRegister.userName)) 
                return BadRequest("User already exist");

            var createUser = new User
            {
                UserName = userRegister.userName
            };

            var createRegister = await _authRepository.Register(createUser, userRegister.password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest userLogin)
        {
            var login = await _authRepository.Login(userLogin.userName, userLogin.password);

            if (login == null) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,login.Id.ToString()),
                new Claim(ClaimTypes.Name, login.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token=tokenHandler.WriteToken(token)});
        }
    }
}