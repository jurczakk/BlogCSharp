﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogContext;
using BlogEntities;
using BlogServices;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;

namespace BlogMvc.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly IBlogDbContext _blogDbContext; 
        public CommentController(IBlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        [HttpGet("Add/{id}")]
        public IActionResult Add(int id)
        {
            var comment = new Comment { ArticleId = id };
            if (!_blogDbContext.Articles.Any(x => x.Id == comment.ArticleId))
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
            return View(comment);
        }

        [HttpPost("Add/{id}")]
        public IActionResult Add(int id, [FromForm] Comment comment)
        {
            comment.ArticleId = id;

            if (!ModelState.IsValid)
                return View("~/Views/Comment/Add.cshtml", comment);

            if (!_blogDbContext.Articles.Any(x => x.Id == id))
                return NotFound();

            comment.UserId = int.Parse(User.Identity.Name);
            comment.Author = User.FindFirst(ClaimTypes.Email).Value;
         
            _blogDbContext.Comments.Add(comment);
            _blogDbContext.SaveChanges();

            return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
        }

        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var comment = await _blogDbContext.Comments.FindAsync(id);
            if (comment == null || !comment.UserId.ToString().Equals(User.Identity.Name))
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });
            return View(comment);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Comment updatedComment)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Comment/Update.cshtml", updatedComment);

            var comment = await _blogDbContext.Comments.FindAsync(id);
            if (comment == null || !comment.UserId.ToString().Equals(User.Identity.Name))
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });

            comment.Content = updatedComment.Content;

            _blogDbContext.Comments.Update(comment);
            _blogDbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _blogDbContext.Comments.FindAsync(id);
            if (comment == null || !comment.UserId.ToString().Equals(User.Identity.Name))
                return RedirectToAction("GetById", "User", new { id = User.Identity.Name });

            _blogDbContext.Comments.Remove(comment);
            _blogDbContext.SaveChanges();

            return NoContent();
        }
    }
}
