using LaptopCart.Data;
using LaptopCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LaptopCart.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private object fileExtension;

        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Product> productslist = _context.Products.ToList();
            return View(productslist);
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Find(model.Id);

                if (product == null)
                {
                    return NotFound();
                }

                // Update normal fields
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;

                // Check if new image selected
                if (model.ImageFile != null)
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(),
                                                 "wwwroot/images");

                    string fileName = Guid.NewGuid().ToString() +
                                      Path.GetExtension(model.ImageFile.FileName);

                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    product.ImagePath = "/images/" + fileName;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var result = await _context.Products.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfimed(int id)
        {

            var product= await _context.Products.FindAsync(id);
            if (product != null)
            {
                //if image file need to be deleted in folder
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImagePath.TrimStart('/'));
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/').Replace("/", "\\"));
                    if(System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

        return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            //this  is a server side validation to check if the product name is "Test" and if it is then we are adding a model error to the model state which will be displayed in the view
            if (product.Name?.Trim().ToUpper() == "TEST")
            {
                ModelState.AddModelError("Name", "Name cannot be test");
            }

            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                //get the wwwroot path       
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                //get the original filename without extension and replace spaces with underscores
                string OriginalFileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName).Replace(" ", "_");//Remove spaces
                string Extension = Path.GetExtension(product.ImageFile.FileName);
                //string unqueFilename=OriginalFileName+"_"+Guid.NewGuid().ToString()+Extension;//to make the filename unique we are appending a guid to the original filename
                //to make the filename unique we are appending a guid to the original filename using string interpolation
                string uniquefilename = $"{OriginalFileName}_{Guid.NewGuid()}{Extension}";
                //combine the wwwroot path with the uploads folder and the unique filename to get the full path where the file will be saved
                string uploadsFolder = Path.Combine(wwwRootPath, "images");
                // check if the uploads folder exists, if not create it
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                //combine the uploads folder path with the unique filename to get the full path where the file will be saved
                string filePath = Path.Combine(uploadsFolder, uniquefilename);
                //save the file to the specified path using a file stream
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                product.ImagePath = "/images/" + uniquefilename;
            }
            product.Createdat = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View();
        }

    }
}
