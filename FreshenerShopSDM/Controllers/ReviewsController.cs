using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    public class ReviewsController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();

		[Authorize(Roles = "Admin")]
		public ActionResult Index()
		{
			var reviews = from review in db.Reviews.Include("Freshener")
						  orderby review.ReviewId
						  select review;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Reviews = reviews;

			return View();
		}

		[Authorize(Roles = "Admin")]
		public ActionResult Show(int id)
		{
			Review review = db.Reviews.Find(id);
			ViewBag.Review = review;
			return View();
		}

		[Authorize(Roles = "Admin, User")]
		public ActionResult Edit(int id)
		{
			Review rev = db.Reviews.Find(id);
			if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
			{
				ViewBag.Review = rev;
				return View(rev);
			}
			else
			{
				TempData["message"] = "You do not have the rights to modify this review!";
				return RedirectToAction("Index", "Fresheners");
			}

		}

		[HttpPut]
		[Authorize(Roles = "Admin, User")]
		public ActionResult Edit(int id, Review requestReview)
		{
			try
			{
				Review rev = db.Reviews.Find(id);

				if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
				{
					if (TryUpdateModel(rev))
					{
						rev.ReviewComment = requestReview.ReviewComment;
						rev.ReviewGrade = requestReview.ReviewGrade;
						rev.ReviewModifyDate = DateTime.Now;
						db.SaveChanges();
					}
					return Redirect("/Fresheners/Show/" + rev.FreshenerId);
				}
				else
				{
					TempData["message"] = "You do not have the rights to modify this review!";
					return RedirectToAction("Index", "Fresheners");
				}

			}
			catch (Exception)
			{
				return View(requestReview);
			}

		}

		[Authorize(Roles = "Admin")]
		public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Admin,User")]
		[HttpPost]
		public ActionResult New(Review rev)
		{
			rev.ReviewModifyDate = DateTime.Now;
			rev.UserId = User.Identity.GetUserId();
			if (ModelState.IsValid)
			{
				db.Reviews.Add(rev);
				db.SaveChanges();
				TempData["message"] = "The review has been added successfully!";
			}
			return Redirect("/Fresheners/Show/" + rev.FreshenerId);
		}

		[Authorize(Roles = "Admin, User")]
		[HttpDelete]
		public ActionResult Delete(int id)
		{

			Review rev = db.Reviews.Find(id);
			if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
			{
				db.Reviews.Remove(rev);
				TempData["message"] = "The review has been deleted!";
				db.SaveChanges();
				return Redirect("/Fresheners/Show/" + rev.FreshenerId);
			}
			else
			{
				TempData["message"] = "You do not have the rights to delete this review!";
				return Redirect("/Fresheners/Show/" + rev.FreshenerId);
			}
		}
	}
}