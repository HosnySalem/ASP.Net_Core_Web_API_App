using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunrise.Contexts;
using Sunrise.Models;

namespace Sunrise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicatinDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(ApplicatinDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult GetCategories() {
       // return Ok(_context.Categories);
       return Ok(_context.Categories.Include(c=>c.Products));
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public ActionResult GetCategories(int id)
        {
            if (id == 0) return BadRequest();
            Category cat = _context.Categories.Include(c=>c.Products).FirstOrDefault(c=>c.Id == id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public ActionResult PostCategory(Category cat)
        {
            if (cat == null)  return BadRequest();
          
            if(_context.Categories.Any(c => c.Name ==cat.Name))
            {
                ModelState.AddModelError("DublicatedName", "this name is registered to another Category. Enter a different name");
                return BadRequest(ModelState);
            }
            if (cat.Name == cat.Description)
            {
                ModelState.AddModelError("NameAndDescriptionMatch", "the name and description must be different from each other");
                return BadRequest(ModelState);
            }
            if (cat.Image == null)
            {
                cat.ImagePath = "\\images\\No_Image_Available.jpg";
            }
            else
            {
                string imgExtension = Path.GetExtension(cat.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + imgExtension;
                cat.ImagePath = "\\images\\categories\\" + imgName;
                string imgFullPath = _webHostEnvironment.WebRootPath + cat.ImagePath;
                FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                cat.Image.CopyTo(fileStream);
                fileStream.Dispose();
            }
            cat.CreatedAt = DateTime.Now;
            _context.Categories.Add(cat);
            _context.SaveChanges();
            return CreatedAtAction("GetCategories", new { id = cat.Id }, cat);
        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        public ActionResult PutCategory(Category cat)
        {
            if (cat == null) return BadRequest();

            if (_context.Categories.Any(c => c.Name == cat.Name))
            {
                ModelState.AddModelError("DublicatedName", "this name is registered to another Category. Enter a different name");
                return BadRequest(ModelState);
            }
            if (cat.Name == cat.Description)
            {
                ModelState.AddModelError("NameAndDescriptionMatch", "the name and description must be different from each other");
                return BadRequest(ModelState);
            }
            cat.LastUpdaedAt = DateTime.Now;
            _context.Categories.Update(cat);
            _context.SaveChanges();
            return NoContent();
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public ActionResult DeleteCategories(int id)
        {
            if (id == 0) return BadRequest();
            Category cat = _context.Categories.Find(id);
            if (cat == null) return NotFound();
            _context.Categories.Remove(cat);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
