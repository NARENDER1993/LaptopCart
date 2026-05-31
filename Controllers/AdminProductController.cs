using LaptopCart.Data;
using LaptopCart.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaptopCart.Controllers
{
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

        public IActionResult Index()
        {
            return View();
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
