﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Models
{
	public class Freshener
	{
		[Key]
		public int FreshenerId { get; set; }

		[Required(ErrorMessage = "The name is mandatory.")]
		[StringLength(128, ErrorMessage = "The name is maximum 128 character long.")]
		public string FreshenerName { get; set; }

		[StringLength(256, ErrorMessage = "The slug is maximum 256 character long.")]
		public string FreshenerSlug { get; set; }

		[Required(ErrorMessage = "The description is mandatory.")]
		[DataType(DataType.MultilineText)]
		public string FreshenerDescription { get; set; }

		[Range(0.1, int.MaxValue, ErrorMessage = "The price can't be below 0.")]
		public float FreshenerPrice { get; set; }

		public int FreshenerRating { get; set; }

		[Required(ErrorMessage = "Freshener image is mandatory.")]
		public string FreshenerImage { get; set; }

		[Required(ErrorMessage = "False OR True to tell if the freshener is on stock.")]
		public bool FreshenerAvailability { get; set; }

		public string FreshenerCode { get; set; }

		public int FreshenerStock { get; set; }

		public DateTime FreshenerModifyDate { get; set; }

		[Required(ErrorMessage = "The category is mandatory.")]
		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }
		public IEnumerable<SelectListItem> CategoryList { get; set; }

		public virtual ICollection<Review> Reviews { get; set; }

		public string UserId { get; set; }
		public virtual ApplicationUser User { get; set; }

		public virtual ICollection<ItemCart> ItemCarts { get; set; }
        public virtual ICollection<OrderComplete> OrderCompletes { get; set; }
    }
}