using LaptopCart.Data;
using LaptopCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace LaptopCart.Controllers
{

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;

        }
        
        public IActionResult Index()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartitems = _context.CartItems.Where(c => c.UserId == userid);
            if(userid != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, cartitems.Count());

            }
            
                var products = _context.Products.ToList();
                return View(products);
            
            

            
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var products = _context.Products.FirstOrDefault(x => x.Id == id);
            return View(products);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(CartItem cartItem)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if user adding the same product to cart then we will update the quantity instead of adding new item to cart
            var cartproduct = _context.CartItems.FirstOrDefault(x => x.ProductId == cartItem.ProductId && x.UserId == userid);
            if (cartproduct != null)
            {
                cartproduct.Quantity += cartItem.Quantity;
                _context.CartItems.Update(cartproduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                cartItem.Id = 0;
                cartItem.UserId = userid;
                var cartitems = _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Product");

        }
    }
}
