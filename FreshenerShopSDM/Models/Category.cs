using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
	public class Category
	{
		[Key]
		public int CategoryId { get; set; }

		[StringLength(128, ErrorMessage = "The name is maximum 128 character long.")]
		public string CategoryName { get; set; }

		[StringLength(256, ErrorMessage = "The slug is maximum 256 character long.")]
		public string CategorySlug { get; set; }

		[Required(ErrorMessage = "Category image is mandatory.")]
		public string CategoryImage { get; set; }

		public string CategoryDescription { get; set; }

		public virtual ICollection<Freshener> Fresheners { get; set; }
	}
}