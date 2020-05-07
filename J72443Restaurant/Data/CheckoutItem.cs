using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace J72443Restaurant.Data
{
    public class CheckoutItem
    {
        [Key, Required]
        public int ID { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required, StringLength(50)]
        public string FoodName { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

}
