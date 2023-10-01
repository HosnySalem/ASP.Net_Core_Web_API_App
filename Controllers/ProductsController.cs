using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sunrise.Contexts;
using Sunrise.Models;

namespace Sunrise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicatinDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicatinDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns></returns>
       
         [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult GetProducts(int catId, string? search, string sortType, string sortOrder, int pageSize = 20, int pageNumber = 1)
        {
            IQueryable<Product> prods = _context.Products.AsQueryable();

            if (catId != 0)
            {
                prods = prods.Where(p=>p.CategoryId == catId);
            }

            if (string.IsNullOrEmpty(search) == false)
            {
                prods = prods.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (sortType == "Name" && sortOrder == "asc")
                prods = prods.OrderBy(p => p.Name);
            else if (sortType == "Name" && sortOrder == "desc")
                prods = prods.OrderByDescending(p => p.Name);
            else if (sortType == "Description" && sortOrder == "asc")
                prods = prods.OrderBy(p => p.Description);
            else if (sortType == "Description" && sortOrder == "desc")
                prods = prods.OrderByDescending(p => p.Description);

            if (pageSize > 50) pageSize = 50;
            if (pageSize < 1) pageSize = 1;
            if (pageNumber < 1) pageNumber = 1;

            prods = prods.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            return Ok(prods.Include(p=>p.Category).ToList());
        }


        /// <summary>
        /// Gets the product's name, description, price and categoryId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product object</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            if (id == 0) return BadRequest();

            Product prod = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

            if (prod == null) return NotFound(); // 404

            return Ok(prod);
        }


        /// <summary>
        /// Adds a new Product
        /// </summary>
        /// <param name="prod"></param>
        /// <returns>the new Product and it's route values</returns>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public ActionResult PostProduct([FromForm]Product prod)
        {
            if (prod == null) return BadRequest();
            if(_context.Products.Any(p => p.Name == prod.Name))
            {
                ModelState.AddModelError("DublicatedName", "this name is registered to another Category. Enter a differnet name");
                return BadRequest(ModelState);
            }
            if(prod.Name == prod.Description)
            {
                ModelState.AddModelError("NameAndDescriptionMatch", "the name and description must be differnt from each other");
                return BadRequest(ModelState);
            }
            if (prod.ProdactionDate > DateTime.Now)
            {
                ModelState.AddModelError("FutureProductionDate", "Production Date can't be a future date.");
                return BadRequest(ModelState);
            }
            if(prod.Image == null)
            {
                prod.ImagePath = "\\images\\No_Image_Available.jpg";
            }
            else
            {
                string imgExtension = Path.GetExtension(prod.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + imgExtension;
                prod.ImagePath = "\\images\\products\\" + imgName;
                string imgFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;
                FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
               prod.Image.CopyTo(fileStream);
                fileStream.Dispose();
            }

            prod.CreatedAt = DateTime.Now;
            _context.Products.Add(prod);
            _context.SaveChanges();
            return CreatedAtAction("GetProduct", new {id = prod.Id},prod);

        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        public ActionResult PutProduct(Product prod)
        {
            if (prod == null) return BadRequest();
            if (_context.Products.Any(p => p.Name == prod.Name))
            {
                ModelState.AddModelError("DublicateName", "this name is registered to another Category. Enter a differnet name");
                return BadRequest(ModelState);
            }
            if (prod.Name == prod.Description)
            {
                ModelState.AddModelError("NameAndDescriptionMatch", "the name and description must be differnt from each other");
                return BadRequest(ModelState);
            }
            if (prod.ProdactionDate > DateTime.Now)
            {
                ModelState.AddModelError("FutureProductionDate", "Production Date can't be a future date.");
                return BadRequest(ModelState);
            }
            if(prod.Image != null)
            {
                if(prod.ImagePath != "\\images\\No_Image_Available.jpg")
                {
                    string oldImageFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;
                    if(System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                    string imgExtension = Path.GetExtension(prod.Image.FileName);
                    Guid imgGuid = Guid.NewGuid();
                    string imgName = imgGuid + imgExtension;
                    prod.ImagePath = "\\images\\products\\" + imgName;
                    string imgFullPath = _webHostEnvironment.WebRootPath + prod.ImagePath;
                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    prod.Image.CopyTo(fileStream);
                    fileStream.Dispose();
                }
            }
            prod.LastUpdaedAt = DateTime.Now;
            _context.Products.Update(prod);
            _context.SaveChanges();
            return NoContent();

        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            if (id == 0) return BadRequest();
            Product prod = _context.Products.Find(id);
            if (prod == null) return NotFound();
            _context.Products.Remove(prod);
            _context.SaveChanges();
            return NoContent();
        }
    }

}
