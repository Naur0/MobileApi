using Microsoft.AspNetCore.Mvc;
using MobileApi.Data;
using MobileApi.Models;
using System.Linq;

namespace MobileApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/post
        [HttpGet]
        public IActionResult Get()
        {
            var posts = _context.Posts.ToList();
            return Ok(posts); // ✅ ALWAYS RETURNS []
        }

        // GET by id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var post = _context.Posts.Find(id);
            return post == null ? NotFound() : Ok(post);
        }

        // POST
        [HttpPost]
        public IActionResult Create(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
            return Ok(post);
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult Update(int id, Post updated)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            post.Title = updated.Title;
            post.Content = updated.Content;

            _context.SaveChanges();
            return Ok(post);
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return Ok();
        }
    }
}