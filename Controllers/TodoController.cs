using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Task.Models.DBModels;

namespace Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : Controller
    {
        private vueappContext _context;

        public TodoController(vueappContext context)
        {
            this._context = context;
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public IActionResult Get()
        {
            Todos todo = new Todos();
            var Userid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            todo.Id = int.Parse(Userid);
            Console.WriteLine(todo.Id);
            var res = _context.Todos.Find(todo.Id);
            return Ok(res);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public IActionResult FindBy([FromBody] TodoModel todo)
        {
            var user = _context.User.First(e => e.Username == todo.Username);
            var res = _context.Todos.Where(e => e.Userid == user.Userid);
            return Ok(res);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public IActionResult update([FromBody] Todos todo)
        {
            var res = _context.Todos.Update(todo);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public IActionResult FindById([FromBody] Todos todo)
        {
            var res = _context.Todos.Find(todo.Id);
            return Ok(res);
        }


        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Delete([FromBody] Todos todo)
        {
            var tododata = _context.Todos.First(e => e.Id == todo.Id);
            var res = _context.Todos.Remove(tododata);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Add([FromBody] TodoModel todo)
        {
            var res = _context.User.First(e => e.Username == todo.Username);
            Todos addTodo = new Todos();
            addTodo.Title = todo.Title;
            addTodo.Description = todo.Description;
            addTodo.Timestamp = todo.Timestamp;
            addTodo.Userid = res.Userid;
            _context.Add(addTodo);
            _context.SaveChanges();

            return Ok(addTodo);
        }



        public partial class TodoModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Username { get; set; }
            public DateTime Timestamp { get; set; }

        }

    }
}
