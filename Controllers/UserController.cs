using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Task.Models.DBModels;

namespace Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private vueappContext _context;

        public UserController(vueappContext context)
        {
            this._context = context;
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public ActionResult<IEnumerable<string>> Get()
        {
            var res = _context.User.ToList();
            return Ok(res);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateUser([FromBody] LoginModel login)
        {

            int count = 0;
            var res = _context.User.ToList();
            foreach (var item in res)
            {
                if (item.Username == login.Username)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                Random generator = new Random();
                String rand_num = generator.Next(0, 1000000).ToString("D6");

                byte[] passwordHash, passwordSalt;

                CreatePasswordHash(login.Password, out passwordHash, out passwordSalt);
                
                User user =  new User();

                user.Username = login.Username;
                user.Salt = passwordSalt;
                user.Password = passwordHash;

                user.VerificationCode = rand_num;
                user.IsVerified = 0;
                Console.WriteLine(user);
                _context.Add(user);
                _context.SaveChanges();

                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Admin",
                "raobilalkhalid613@gmail.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User",
                user.Username);
                message.To.Add(to);

                message.Subject = "Verification Code";
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $"<h1>{rand_num}</h1>";
                bodyBuilder.TextBody = rand_num;
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("raobilalkhalid613@gmail.com", "umairkhalid");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();


                return Ok();
            }
            else
            {
                throw new Exception("Already exist username");
            }
        }




        [HttpPost]
        [Route("[action]")]

        public IActionResult VerifyUser([FromBody] User user)
        {
            var res = _context.User.First(e => e.Username == user.Username);
            if (res.VerificationCode == user.VerificationCode)
            {
                res.IsVerified = 1;
                _context.SaveChanges();
                return Ok(res);
            }
            else
            {
                throw new Exception("Invalid Verification Code");
            }

        }

        public partial class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }

        }
    }
}
