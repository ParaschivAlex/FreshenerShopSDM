using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace FreshenerShopSDM.Controllers
{
    public class FreshenersController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();

		private readonly int _perPage = 9;
		private List<Freshener> FreshenersSorted;

		public ActionResult Index()
		{
			var fresheners = db.Fresheners.Include("Category").OrderBy(f => f.FreshenerModifyDate);
			if (FreshenersSorted == null)
			{
				FreshenersSorted = fresheners.ToList();
			}

			if (TempData.ContainsKey("Fresheners"))
			{
				FreshenersSorted = TempData["Fresheners"] as List<Freshener>;
			}

			var search = "";

			if (Request.Params.Get("search") != null)
			{
				search = Request.Params.Get("search").Trim();
				List<int> freshenersIds = db.Fresheners.Where(fr => fr.FreshenerName.Contains(search) ||  fr.FreshenerDescription.Contains(search)).Select(f => f.FreshenerId).ToList();
				List<int> reviewIds = db.Reviews.Where(rev => rev.ReviewComment.Contains(search)).Select(rev => rev.FreshenerId).ToList();
				List<int> mergedIds = freshenersIds.Union(reviewIds).ToList();

				fresheners = db.Fresheners.Where(freshener => mergedIds.Contains(freshener.FreshenerId)).Include("Category").OrderBy(f => f.FreshenerModifyDate);
				FreshenersSorted = fresheners.ToList();
			}

			Console.WriteLine(FreshenersSorted);
			foreach (var fresh in FreshenersSorted)
			{
				Console.WriteLine(fresh);
				RatingChecker(fresh);
			}

			var numberOfFresheners = FreshenersSorted.Count();

			var currentPage = Convert.ToInt32(Request.Params.Get("page"));

			var offset = 0;

			if (!currentPage.Equals(0))
			{
				offset = (currentPage - 1) * this._perPage;
			}
			var paginatedFresheners = FreshenersSorted.Skip(offset).Take(this._perPage);

			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}

			ViewBag.total = numberOfFresheners;
			ViewBag.lastPage = Math.Ceiling((float)numberOfFresheners / (float)this._perPage);
			ViewBag.Fresheners = paginatedFresheners;

			ViewBag.SearchString = search;

			return View();
		}

		public ActionResult SortFresheners(int id)
		{
			switch (id)
			{
				case 1: // crescator dupa pret
					FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerPrice).ToList();
					break;
				case 2: //descrescator dupa pret
					FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerPrice).ToList();
					FreshenersSorted.Reverse();
					break;
				case 3: //crescator dupa rating
					FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerRating).ToList();
					break;
				case 4: //descrescator dupa rating
					FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerRating).ToList();
					FreshenersSorted.Reverse();
					break;
				default:
					break;
			}
			TempData["Filme"] = FreshenersSorted;
			return Redirect("/Fresheners/Index");
		}

		public ActionResult Show(int id)
		{
			Freshener freshener = db.Fresheners.Find(id);
			ViewBag.Freshener = freshener;
			ViewBag.Category = freshener.Category;
			ViewBag.Reviews = freshener.Reviews;
			ViewBag.utilizatorCurent = User.Identity.GetUserId();
			return View(freshener);
		}

		[Authorize(Roles = "Admin")]
		public ActionResult New()
		{
			/*Freshener freshener = new Freshener
			{
				CategoryList = GetCategories(),
				UserId = User.Identity.GetUserId()
			};*/

			Freshener freshener = new Freshener();
			freshener.CategoryList = GetCategories();
			freshener.UserId = User.Identity.GetUserId();

			return View(freshener);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public ActionResult New(Freshener freshener)
		{
			freshener.FreshenerModifyDate = DateTime.Now;
			freshener.UserId = User.Identity.GetUserId();
			try
			{
				if (ModelState.IsValid)
				{
					db.Fresheners.Add(freshener);
					db.SaveChanges();
					Console.WriteLine("DB.SAVEDCHANGES");
					TempData["message"] = "The freshener has been added!";
					return RedirectToAction("Index");
				}
				else
				{
					Console.WriteLine("Error on modelstate.isvalid for adding a new freshener.");
					freshener.CategoryList = GetCategories();
					return View(freshener);
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error on try catch for adding a new freshener.");
				freshener.CategoryList = GetCategories();
				return View(freshener);
			}
		}

		[Authorize(Roles = "Admin")]
		public ActionResult Edit(int id)
		{
			Freshener freshener = db.Fresheners.Find(id);
			freshener.CategoryList = GetCategories();
			ViewBag.Freshener = freshener;
			ViewBag.Category = freshener.Category;
			ViewBag.utilizatorCurent = User.Identity.GetUserId();
			return View(freshener);
			
		}
		[Authorize(Roles = "Admin")]
		[HttpPut]
		public ActionResult Edit(int id, Freshener requestFreshener)
		{

			try
			{
				if (ModelState.IsValid)
				{
					Freshener freshener = db.Fresheners.Find(id);

					if (TryUpdateModel(freshener))
					{
						freshener = requestFreshener;
						db.SaveChanges();
						TempData["message"] = "The freshener has been modified!";
					}
				}
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return View(requestFreshener);
			}

		}
		[Authorize(Roles = "Admin")]
		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Freshener freshener = db.Fresheners.Find(id);
			db.Fresheners.Remove(freshener);
			db.SaveChanges();
			TempData["message"] = "The freshener has been deleted!";
			return RedirectToAction("Index");
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetCategories()
		{
			var selectList = new List<SelectListItem>();
			var categories = from cat in db.Categories select cat;

			foreach (var category in categories)
			{
				selectList.Add(new SelectListItem
				{
					Value = category.CategoryId.ToString(),
					Text = category.CategoryName.ToString()
				});
			}
			return selectList;
		}

		[NonAction]
		public void RatingChecker(Freshener freshener)
		{
			float rating = 0;
			//int numberOfReviews = 0;
			var reviews = db.Reviews.Where(rv => rv.FreshenerId == freshener.FreshenerId);

			foreach (var rev in reviews)
			{
				rating += rev.ReviewGrade;
				//numberOfReviews++;
			}
			//rating /= numberOfReviews;
			rating /= reviews.Count();
			freshener.FreshenerRating = rating;
			db.SaveChanges();
		}

		/*[HttpPost]
		[Authorize(Roles = "Admin, User")]
		public ActionResult NewReview(Review rev)
		{
			rev.ReviewModifyDate = DateTime.Now;
			rev.UserId = User.Identity.GetUserId();
			try
			{
				if (ModelState.IsValid)
				{
					db.Reviews.Add(rev);
					db.SaveChanges();
					return Redirect("/Fresheners/Show/" + rev.FreshenerId);
				}
				else
				{
					Freshener fresh = db.Fresheners.Find(rev.FreshenerId);
					ViewBag.isAdmin = User.IsInRole("Admin");
					ViewBag.currentUser = User.Identity.GetUserId();
					TempData["message"] = "This field can not be modified!";
					return Redirect("/Fresheners/Show/" + rev.FreshenerId);

				}
			}

			catch (Exception e)
			{
				Freshener fresh = db.Fresheners.Find(rev.FreshenerId);
				ViewBag.isAdmin = User.IsInRole("Admin");
				ViewBag.currentUser = User.Identity.GetUserId();
				return View(fresh);
			}

		}
		*/
	}
}