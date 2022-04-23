using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
    public class OrderComplete
    {
        [Key]
        public int OrderCompleteId {get; set;}
        public int FreshenerId { get; set; }
        public int OrderId { get; set; }
        public int FreshenerQuantity { get; set; }

        public virtual Freshener Freshener { get; set; }
        public virtual Order Order { get; set; }
    }
}