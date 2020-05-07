using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace J72443Restaurant.Data
{
    public class CheckoutCustomer
    {
        [Key]
        [StringLength(50)]
        public string Email { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public int BasketID { get; set; }
    }
}
