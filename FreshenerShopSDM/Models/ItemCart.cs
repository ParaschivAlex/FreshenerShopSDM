using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
    public class ItemCart
    {
        [Key]
        public int ItemCartId { get; set; }
        public int FreshenerId { get; set; }
        public int CartId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public virtual Freshener Freshener { get; set; }
        public virtual Cart Cart { get; set; }
        public virtual Order Order { get; set; }
    }
}