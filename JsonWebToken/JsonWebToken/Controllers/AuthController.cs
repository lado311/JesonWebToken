using JsonWebToken.Dto;
using JsonWebToken.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JsonWebToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        

        private readonly IConfiguration _Configuration;

        public AuthController(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        [HttpPost("Register")]
        public ActionResult<User> RegisterUser(UserDto userDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            user.UserName = userDto.UserName;
            user.PasswordHash = passwordHash;

            return new JsonResult(user);
        }

        [HttpPost("LogIn")]
        public ActionResult<User> LogInUser(UserDto userDto)
        {
            if(user.UserName !=  userDto.UserName)
            {
                return BadRequest("something wrong");
            }
            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
            {
                return BadRequest("something wrong");
            }

            string token = CreateToken(user);

            return Ok(token);
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(
                _Configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }














    }
}
