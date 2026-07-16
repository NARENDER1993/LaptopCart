using LaptopCart.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LaptopCart.Controllers
{
    public class CartController : Controller
    {
        //dependency injection of the ApplicationDbContext to access the database
        private readonly ApplicationDbContext _context;
        //constructor to initialize the ApplicationDbContext
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartitems = _context.CartItems.Where(c => c.UserId == userid).

                           Include(c => c.Product).ToList();


            return View(cartitems);
        }

        public async Task<IActionResult> Plus(int cartid)
        {
            var cartitemsfromDB = _context.CartItems.FirstOrDefault(c => c.Id == cartid);
            if (cartitemsfromDB == null)
                return NotFound();
            //it increases the quantity of the cart item by 1
            cartitemsfromDB.Quantity += 1;
            _context.CartItems.Update(cartitemsfromDB);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartid)
        {
            var cartfromDB = _context.CartItems.FirstOrDefault(c => c.Id == cartid);
            if (cartfromDB == null)
                return NotFound();
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (cartfromDB.Quantity <= 1)
            {
                _context.CartItems.Remove(cartfromDB);
                await _context.SaveChangesAsync();

                var caritemscount = _context.CartItems.Count(c => c.UserId == userid);
                HttpContext.Session.SetInt32(SD.SessionCart, caritemscount);


            }
            else
            {
                cartfromDB.Quantity -= 1;
                _context.CartItems.Update(cartfromDB);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartid)
        {
            var cartitemsfromdb=_context.CartItems.FirstOrDefault(c=>c.Id == cartid);
            if(cartitemsfromdb == null)
                return NotFound();
            _context.CartItems.Remove(cartitemsfromdb);
            await _context.SaveChangesAsync();
            //get the userid
            var userid=User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartitemscount = _context.CartItems.Count(c => c.UserId == userid);
            HttpContext.Session.SetInt32(SD.SessionCart, cartitemscount);


            return RedirectToAction(nameof(Index));
        }
    }
}
