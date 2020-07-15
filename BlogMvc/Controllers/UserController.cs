﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using BlogMvc.Models;
using BlogServices;
using BlogEntities;
using BlogContext;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlogMvc.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly Blog _blog;
        private readonly UserService _userService;
        public UserController(Blog blog, UserService userService)
        {
            _blog = blog;
            _userService = userService;
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var articles = await _blog.Articles.ToListAsync();
            var user = await _blog.Users.FindAsync(id);
            if (user == null) 
                return RedirectToAction("Index", "Home");

            return View ("~/Views/User/Main.cshtml", new UserViewModel { User = user, Articles = articles });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] User user)
        {
            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var userDb = await _blog.Users.FirstOrDefaultAsync(i => i.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase));
            if (userDb == null)
                return NotFound("User does not exist");

            var hashedPassword = _userService.HashPassword(user.Password);

            if (userDb.Password != hashedPassword) 
                return NotFound("Wrong password");

            await _userService.SignIn(user);

            return RedirectToAction("GetUser", new { id = int.Parse(HttpContext.User.Identity.Name) });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] User user)
        {    
            if (!ModelState.IsValid)
                return NotFound(ModelState);

            if (_blog.Users.Any(i => i.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                return NotFound("User with this email is existing in db");

            user.Password = _userService.HashPassword(user.Password);

            _blog.Users.Add(user);
            _blog.SaveChanges();
            
            await _userService.SignIn(user);
            
            return RedirectToAction("GetUser", new { id = int.Parse(HttpContext.User.Identity.Name) });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Update")]
        public IActionResult UpdateView()
        {
            return View("~/Views/User/Update.cshtml", new User { Id = int.Parse(User.Identity.Name) });
        }

        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, [FromBody] PasswordViewModel password)
        {
            return Ok();
            // return await Task.Run(() => View());
            // return _userController.UpdatePassword(id, password)
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        //    if (id != int.Parse(User.Identity.Name))
        //        return NotFound();
        //    Utils.DeleteCookie(HttpContext);
        //    return _userController.Delete(id);
        }

    }
}
