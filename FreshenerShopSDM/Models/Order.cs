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

        [Required(ErrorMessage = "The first name is mandatory.")]
        [StringLength(64, ErrorMessage = "The first name is maximum 64 character long.")]
        public string OrderFirstName { get; set; }

        [Required(ErrorMessage = "The last name is mandatory.")]
        [StringLength(64, ErrorMessage = "The last name is maximum 64 character long.")]
        public string OrderLastName { get; set; }

        [Required(ErrorMessage = "The city is mandatory.")]
        [StringLength(128, ErrorMessage = "The city name is maximum 128 character long.")]
        public string OrderCity { get; set; }

        [Required(ErrorMessage = "The state is mandatory.")]
        [StringLength(128, ErrorMessage = "The state is maximum 128 character long.")]
        public string OrderState { get; set; }

        [Required(ErrorMessage = "The street is mandatory.")]
        [StringLength(256, ErrorMessage = "The street is maximum 256 character long.")]
        public string OrderStreet { get; set; }


        public string OrderPostalCode { get; set; }

        [Required(ErrorMessage = "The phone number is mandatory.")]
        [RegularExpression(@"^(\+4|)?[0-9]{6,}", ErrorMessage = "This phone number is not valid.")]
        public string OrderPhone { get; set; }

        [Required(ErrorMessage = "The email is mandatory.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "This email is not valid.")]
        public string OrderEmail { get; set; }

        public float OrderTotal { get; set; }
        public System.DateTime OrderModifyDate { get; set; }
        public bool OrderSent { get; set; }

        public virtual ICollection<ItemCart> ItemCarts { get; set; }
        public virtual ICollection<OrderComplete> OrderCompletes { get; set; }

        //public int OrderCompleteId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}