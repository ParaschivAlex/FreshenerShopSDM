using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreshenerShopSDM.Models
{
	public class Review
	{
		[Key]
		public int ReviewId { get; set; }

		[StringLength(512, ErrorMessage = "The comment is maximum 512 character long.")]
		public string ReviewComment { get; set; }

		[Required(ErrorMessage = "The rating is mandatory.")]
		[Range(1, 5, ErrorMessage = "The rating should be between 1 and 5.")]
		public int ReviewGrade { get; set; }

		public DateTime ReviewModifyDate { get; set; }

		public int FreshenerId { get; set; }

		public string UserId { get; set; }


		public virtual ApplicationUser User { get; set; }
		public virtual Freshener Freshener { get; set; }

	}
}