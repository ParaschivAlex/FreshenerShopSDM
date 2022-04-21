using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderUsername { get; set; }
        public string OrderFirstName { get; set; }
        public string OrderLastName { get; set; }
        public string OrderCity { get; set; }
        public string OrderState { get; set; }
        public string OrderStreet { get; set; }
        public string OrderPostalCode { get; set; }
        public string OrderPhone { get; set; }
        public string OrderEmail { get; set; }
        public float OrderTotal { get; set; }
        public System.DateTime OrderModifyDate { get; set; }
        public virtual ICollection<ItemCart> ItemCarts { get; set; }
        public virtual ICollection<OrderComplete> OrderCompletes { get; set; }
        public int OrderCompleteId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}