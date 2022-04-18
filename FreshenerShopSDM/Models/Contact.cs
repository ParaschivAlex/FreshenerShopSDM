using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(64, ErrorMessage = "The name is maximum 64 character long.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Your email is required.")]
        [StringLength(64, ErrorMessage = "The email is maximum 64 character long.")]
        public string ContactEmail { get; set; }

        [StringLength(16, ErrorMessage = "The phone number is maximum 16 character long.")]
        public string ContactPhoneNumber { get; set; }

        [Required(ErrorMessage = "The subject is mandatory.")]
        [StringLength(32, ErrorMessage = "The subject is maximum 32 character long.")]
        public string ContactSubject { get; set; }

        [Required(ErrorMessage = "The message is mandatory.")]
        [StringLength(512, ErrorMessage = "The name is maximum 512 character long.")]
        public string ContactMessage { get; set; }

        public DateTime ContactModifyDate { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}