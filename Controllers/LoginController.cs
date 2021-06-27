using Task.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Task.Models.DBModels;
using System.Linq;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private vueappContext _context;

        public LoginController(IConfiguration config, vueappContext context)
        {
            _config = config;
            this._context = context;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Login([FromBody] UserModel login)
        {
            Console.WriteLine("sdfsdfsff");
            IActionResult response = Unauthorized();
            var user = _context.User.First(e => e.Username == login.Username);

            Console.WriteLine(login.Username);
            Console.WriteLine(user.Password);
            if (user != null && user.IsVerified == 1)
            {
                if (VerifyPasswordHash(login.Password, user.Password, user.Salt))

                {
                    Console.WriteLine("verified");
                    var tokenString = GenerateJSONWebToken(user);
                    Console.WriteLine(tokenString);
                    response = Ok(new { 
                        user = user.Username,
                        token = tokenString
                         });
                }
                else
                {
                    Console.WriteLine("invalid password");
                }
            }
            return response;
        }
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                Console.WriteLine(computedHash);
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
        private string GenerateJSONWebToken(User login)    
        {    
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));    
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
            var claims = new[] {    
                new Claim(JwtRegisteredClaimNames.Sub, login.Userid.ToString()) 
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],    
              _config["Jwt:Issuer"],    
              claims,    
              expires: DateTime.Now.AddMinutes(120),    
              signingCredentials: credentials);    
    
            return new JwtSecurityTokenHandler().WriteToken(token);    
        }
        public partial class UserModel
        {

            public string Username { get; set; }
            public string Password { get; set; }

        }
    }
}
