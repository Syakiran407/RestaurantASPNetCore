using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace J72443Restaurant.Pages
{
    
    public class menuListModel : PageModel
    {

        private readonly AppDbContext _db;
        public IList<Food> food { get; private set; }
        [BindProperty]
        public string Search { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        public menuListModel(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void OnGet()
        {
            food = _db.Foods.FromSql("SELECT * FROM Foods").ToList();
        }

        public IActionResult OnPostSearch()
        {
            food = _db.Foods.FromSql("SELECT * FROM Foods WHERE FoodName LIKE " + "'" + Search + "%' OR FoodName LIKE" + "'%" + Search + "' OR Description LIKE '%" + Search + "%'")
                 .ToList();
            return Page();
        }

        [Authorize]
        public async Task<IActionResult> OnPostDeleteAsync(int itemID)
        {
            var item = await _db.Foods.FindAsync(itemID);
            if (item != null)
            {
                _db.Foods.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        [Authorize]
        public async Task<IActionResult> OnPostBuyAsync(int itemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            var item = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", itemID, customer.BasketID)
            .ToList()
            .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    StockID = itemID,
                    Quantity = 1
                };
                _db.BasketItems.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found!", e);
                }
            }
            return RedirectToPage();
        }
    }

}

