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
		public ApplicationDbContext db = new ApplicationDbContext();

		private List<Freshener> freshenersList;

		public ActionResult Index(string sortOrder, string search, string currentFilter)
		{
			ViewBag.CurrentSort = sortOrder;

			//var fresheners = db.Fresheners.Include("Category").OrderBy(f => f.FreshenerModifyDate);

			if (search == null)
			{
				search = currentFilter;
			}

			ViewBag.CurrentFilter = search;

			var fresheners = from f in db.Fresheners
							 select f;

			var freshenersToUpdateRating = db.Fresheners.Include("Category").OrderBy(f => f.FreshenerModifyDate);
			freshenersList = freshenersToUpdateRating.ToList();
			
			if (!String.IsNullOrEmpty(search))
			{
				fresheners = fresheners.Where(fr => fr.FreshenerName.Contains(search) || fr.FreshenerDescription.Contains(search));
			}

			foreach (var fresh in freshenersList)
			{
				RatingChecker(fresh.FreshenerId);
			}
			
			switch (sortOrder)
			{
				case "1": //increasing by price
					fresheners = fresheners.OrderBy(f => f.FreshenerPrice);
					//System.Diagnostics.Debug.WriteLine("increasing price");
					break;
				case "2": //decreasing by price
					fresheners = fresheners.OrderByDescending(f => f.FreshenerPrice);
					//System.Diagnostics.Debug.WriteLine("decreasing price");
					break;
				case "3": //increasing by rating
					fresheners = fresheners.OrderBy(f => f.FreshenerRating);
					//Debug.WriteLine("rating up");
					break;
				case "4": //decreasing by rating
					fresheners = fresheners.OrderByDescending(f => f.FreshenerRating);
					//System.Diagnostics.Debug.WriteLine("decreasing rating");
					break;
				case "5": //newest
					fresheners = fresheners.OrderBy(f => f.FreshenerModifyDate);
					//System.Diagnostics.Debug.WriteLine("newest");
					break;
				case "6": //oldest
					fresheners = fresheners.OrderByDescending(f => f.FreshenerModifyDate);
					//System.Diagnostics.Debug.WriteLine("oldest");
					break;
				default:
					fresheners = fresheners.OrderBy(f => f.FreshenerModifyDate);
					break;
			}		

			var numberOfFresheners = fresheners.Count();

			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}

			ViewBag.total = numberOfFresheners;
			ViewBag.Fresheners = fresheners;
			//ViewBag.SearchString = search;
			//db.SaveChanges();
			return View();
		}

		public ActionResult Show(int id)
		{
			Freshener freshener = db.Fresheners.Find(id);
			ViewBag.Freshener = freshener;
			ViewBag.Category = freshener.Category;
			ViewBag.Reviews = freshener.Reviews;
			ViewBag.currentUser = User.Identity.GetUserId();
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
					//Console.WriteLine("DB.SAVEDCHANGES");
					TempData["message"] = "The freshener has been added! Add another freshener?";
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
			ViewBag.currentUser = User.Identity.GetUserId();
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
                        freshener.FreshenerModifyDate = DateTime.Now;
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

			var reviews = db.Reviews.Where(fr => fr.FreshenerId == id);
			foreach (var review in reviews)
			{
				db.Reviews.Remove(review);
			}

			/*List<int> ordersCompleted = db.OrderCompletes.Where(ordCom => ordCom.FreshenerId == freshener.FreshenerId).Select(ordC => ordC.OrderId).ToList();
			foreach (var orderCompleted in ordersCompleted)
			{

				Order orde = db.Orders.Find(orderCompleted);
					db.Orders.Remove(orde);
				}
			}*/

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

		[HttpPut]
		public ActionResult RatingChecker(int id)
		{
			int rating = 0;
			int numberOfReviews = 0;
			Freshener freshener = db.Fresheners.Find(id);

			var reviews = db.Reviews.Where(rv => rv.FreshenerId == freshener.FreshenerId);
			if (reviews != null)
			{
				if (TryUpdateModel(freshener))
				{
					foreach (var rev in reviews)
					{
						rating += rev.ReviewGrade;
						numberOfReviews++;
					}
					if (numberOfReviews != 0)
					{
						rating /= numberOfReviews;
						//rating /= reviews.Count();
						freshener.FreshenerRating = rating;
						//Debug.WriteLine(rating);
						db.SaveChanges();
						return View();
					}
					return View();
				}
			}
			else
			{
				return View();
			}
			return View();
		}
	}
}