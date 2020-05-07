using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace J72443Restaurant.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IList<Food> food { get; private set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _he;
        public IFormFile Pic { get; set; }

        public IndexModel(AppDbContext db, UserManager<ApplicationUser> userManager, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
            _userManager = userManager;
        }
 
        public async Task<IActionResult> OnGetAsync(int id)
        {
            food = _db.Foods.FromSql("SELECT TOP 6 * FROM Foods ORDER BY Foods.ID DESC").ToList();
  
            return Page();
        }
    }
}
