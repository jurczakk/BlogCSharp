﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using BlogServices;
using BlogEntities;
using BlogContext;
using BlogMvc.Models;

namespace BlogMvc.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly Blog _blog;
        private readonly UserService _userService;

        public UserController(Blog blog, UserService userService)
        {
            _blog = blog;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {   
            if (!User.Identity.Name.Equals(id.ToString())) 
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });

            var user = await _blog.Users.FindAsync(id);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            var articles = _blog.Articles.ToList().Select(article => new ArticleViewModel 
            {
                Article = article,
                Comments = _blog.Comments.Where(x => x.ArticleId == article.Id).ToList()
            });

            return View("~/Views/User/Main.cshtml", new UserViewModel { User = user, ArticleViewModel = articles });
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) 
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
            return View();
        }

        [AllowAnonymous]
        [HttpPost("Login")] 
        public async Task<IActionResult> Login([FromForm] User user)
        {
            if (!ModelState.IsValid)
                return View();
                
            var userDb = await _blog.Users.FirstOrDefaultAsync(i => 
                i.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase));
            
            if (userDb == null)
            {
                ModelState.AddModelError("error", "User does not exist");
                return View();
            }

            if (!_userService.Verify(user.Password, userDb.Password)) 
            {
                ModelState.AddModelError("error", "Wrong Password");
                return View();
            }

            await _userService.SignIn(userDb);

            return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
        }

        [AllowAnonymous]
        [HttpGet("Register")] 
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) 
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
            return View();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] User user)
        {
            if (!ModelState.IsValid)
                return View();

            user.Email = user.Email.Trim();

            if (_blog.Users.Any(i => i.Email.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase)))
            {
                ModelState.AddModelError("error", "User with this email is existing in db");
                return View();
            }

            user.Password = _userService.Hash(user.Password);
            
            _blog.Users.Add(user);
            _blog.SaveChanges();
            
            await _userService.SignIn(user);
            
            return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Update")]
        public IActionResult Update()
        {
            return View();
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PasswordViewModel password)
        {
            if (!User.Identity.Name.Equals(id.ToString()))
                return NotFound();

            if (!ModelState.IsValid)
                return View();
                
            var userDb = await _blog.Users.FindAsync(id);
            if (userDb == null)
            {
                ModelState.AddModelError("error", "Unexpected error, user with this id does not exist");
                return View(); 
            }
                
            if (!_userService.Verify(password.Old, userDb.Password))
            {
                ModelState.AddModelError("error", "Old password is not equal");
                return View();
            }
                
            userDb.Password = _userService.Hash(password.New);

            _blog.Users.Update(userDb);
            await _blog.SaveChangesAsync();

            return NoContent();
        }
    }
}