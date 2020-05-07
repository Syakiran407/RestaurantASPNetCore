using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace J72443Restaurant.Data
{
    public class Basket
    {

        [StringLength(50)]
        public string Email { get; set; }
        [Key]
        public int BasketID { get; set; }
    } 
}
