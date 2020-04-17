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
using Microsoft.EntityFrameworkCore;

namespace J72443Restaurant.Pages.Admin
{
    public class EditModel : PageModel
    {
        private AppDbContext _db;
        [BindProperty]
        public Food food { get; set; }
        [BindProperty]
        public IFormFile Pic { get; set; }
        private readonly IHostingEnvironment _he;

        public EditModel(AppDbContext db, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            food = await _db.Foods.FindAsync(id);
            if (food == null)
            {
                return RedirectToPage("/menuList");
            }
            return Page();

        }

        public async Task <IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Pic != null)
            {
                var filename = Path.Combine(_he.WebRootPath, "uploaded_image", Path.GetFileName(Pic.FileName));
                food.Image = Path.Combine("uploaded_image", Path.GetFileName(Pic.FileName));
                Pic.CopyTo(new FileStream(filename, FileMode.Create));
            }

            _db.Entry(food).State = EntityState.Modified;
            if(food.Image == null)
            {
                _db.Entry(food).Property(f => f.Image).IsModified = false;
            }

            try
            {

                await _db.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"Menu {food.ID} not found", e);
            }

            return RedirectToPage("/menuList");
        }
    }
}