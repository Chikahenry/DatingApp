using Microsoft.AspNetCore.Mvc;
using DatingApp.Api.Data;
using DatingApp.Api.MOdels;
using DatingApp.Api.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System;
using AutoMapper;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public readonly IMapper _mapper; 
        public AuthController(IMapper mapper, IAuthRepository repository, IConfiguration config)
        {
            _mapper = mapper;
            _config = config;
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            // if (!ModelState.IsValid){
            //     return BadRequest("Ensure you fill the required field");
            // }
            userRegisterDto.Username = userRegisterDto.Username.ToLower();

            if (await _repository.UserExists(userRegisterDto.Username))
            {
                return BadRequest("User already exists");
            }

            var newUser = new User
            {
                Username = userRegisterDto.Username
            };

            var user = await _repository.Register(newUser, userRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {

            var checkForUser = await _repository.Login(userLoginDto.Username.ToLower(), userLoginDto.Password);

            if (checkForUser == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, checkForUser.Id.ToString()),
                new Claim(ClaimTypes.Name, checkForUser.Username)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserListDto>(checkForUser);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });

        }
    }
}

