using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace J72443Restaurant.Data
{
    [Authorize]
    public class Food
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(50)]
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        [StringLength(100)]
        public string Image { get; set; }
    }
}
