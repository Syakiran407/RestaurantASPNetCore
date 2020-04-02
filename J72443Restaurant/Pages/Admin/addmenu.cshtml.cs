using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace J72443Restaurant
{
    public class addmenuModel : PageModel
    {
        private AppDbContext _db;
        [BindProperty]
        public Food food { get; set; }
        [BindProperty]
        public IFormFile Pic { get; set; }
        private readonly IHostingEnvironment _he;

        public addmenuModel(AppDbContext db, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(Pic != null)
            {
                var filename = Path.Combine(_he.WebRootPath, "uploaded_image", Path.GetFileName(Pic.FileName));
                food.Image = Path.Combine("uploaded_image", Path.GetFileName(Pic.FileName));
                Pic.CopyTo(new FileStream (filename, FileMode.Create));
            }

            _db.Foods.Add(food);
            await _db.SaveChangesAsync();

            return RedirectToPage("/menuList");
        }

        public void OnGet()
        {

        }
    }
}