using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Authorization;
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
        public menuListModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            food = _db.Foods.FromSql("SELECT * FROM Foods").ToList();
        }

        public IActionResult OnPost(String Name)
        {
            food = _db.Foods.FromSql("SELECT * FROM Foods WHERE FoodName LIKE " + "'" + Search + "%' OR FoodName LIKE" + "'%" + Search + "' OR Description LIKE '%" + Search + "%'")
                .ToList();
            return Page();
        }
       
        public async Task <IActionResult> OnPostDeleteAsync(int id)
        {
            var food = await _db.Foods.FindAsync(id);
            if(food != null)
            {
                _db.Foods.Remove(food);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}