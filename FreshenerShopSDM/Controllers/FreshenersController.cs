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

		private readonly int _perPage = 3;
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

				fresheners = db.Fresheners.Where(freshener => freshenersIds.Contains(freshener.FreshenerId)).Include("Category").OrderBy(f => f.FreshenerModifyDate);
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
				case 1: //increasing by price
					FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerPrice).ToList();
                    //System.Diagnostics.Debug.WriteLine("increasing price");
                    break;
				case 2: //decreasing by price
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerPrice).ToList();
                    //System.Diagnostics.Debug.WriteLine("decreasing price");
                    FreshenersSorted.Reverse();
					break;
				case 3: //increasing by rating
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerRating).ToList();
                    //System.Diagnostics.Debug.WriteLine("increasing rating");
                    break;
				case 4: //decreasing by rating
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerRating).ToList();
                    //System.Diagnostics.Debug.WriteLine("decreasing rating");
                    FreshenersSorted.Reverse();
					break;
                case 5: //newest
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerModifyDate).ToList();
                    //System.Diagnostics.Debug.WriteLine("newest");
                    break;
                case 6: //oldest
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerModifyDate).ToList();
                   //System.Diagnostics.Debug.WriteLine("oldest");
                    FreshenersSorted.Reverse();
                    break;
                default:
                    FreshenersSorted = db.Fresheners.Include("Category").OrderBy(a => a.FreshenerModifyDate).ToList();
                    break;

            }
			TempData["Fresheners"] = FreshenersSorted;
			return Redirect("/Fresheners/Index");
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
					return RedirectToAction("New");
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
            if (reviews != null)
            {
                try
                {
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
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }
            else
            {
                return;
            }
		}
	}
}