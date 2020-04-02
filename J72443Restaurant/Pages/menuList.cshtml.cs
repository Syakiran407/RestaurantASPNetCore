using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace J72443Restaurant.Pages
{
    public class menuListModel : PageModel
    {

        private readonly AppDbContext _db;
        public IList<Food> food { get; private set; }
        public menuListModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            food = _db.Foods.FromSql("SELECT * FROM Foods").ToList();
        }
    }
}