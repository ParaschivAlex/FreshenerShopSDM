using FreshenerShopSDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
	public class CategoriesController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
		{
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}

			var categories = from category in db.Categories
							 orderby category.CategoryName
							 select category;
			ViewBag.Categories = categories;
			return View();
		}

        public ActionResult Show(int id)
		{
            try
            {
                Category category = db.Categories.Find(id);
                var fresheners = from freshener in db.Fresheners
                                 where freshener.CategoryId == category.CategoryId
                                 select freshener;
                //Console.WriteLine(fresheners);
                if (fresheners != null)
                {
                    ViewBag.Categories = category;
                    ViewBag.Fresheners = fresheners;
                    return View(category);
                }
                else
                {
                    throw new NullReferenceException("You can't check a category that has no fresheners!");
                }
            }
            catch (Exception e)
            {               
                TempData["message"] = e;
                return Redirect("/Categories/Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public ActionResult New(Category cat)
		{
			try
			{
				db.Categories.Add(cat);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				return View();
			}
		}

		[Authorize(Roles = "Admin")]
		public ActionResult Edit(int id)
		{
			Category category = db.Categories.Find(id);
			return View(category);
		}

		[Authorize(Roles = "Admin")]
		[HttpPut]
		public ActionResult Edit(int id, Category requestCategory)
		{
			try
			{
				Category category = db.Categories.Find(id);
				if (TryUpdateModel(category))
				{
					category.CategoryName = requestCategory.CategoryName;
					db.SaveChanges();
					TempData["message"] = "The category has been modified!";
					return RedirectToAction("Index");
				}

				return View(requestCategory);
			}
			catch (Exception)
			{
				return View(requestCategory);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Category category = db.Categories.Find(id);
			List<int> freshenersToDelete = db.Fresheners.Where(fr => fr.CategoryId == id).Select(f => f.FreshenerId).ToList();

			foreach (var freshenerDelete in freshenersToDelete)
            {
				Freshener fresh = db.Fresheners.Find(freshenerDelete);

				List<int> reviews = db.Reviews.Where(rev => rev.FreshenerId == fresh.FreshenerId).Select(r => r.ReviewId).ToList();
				foreach (var review in reviews)
				{
					Review rev = db.Reviews.Find(review);
					db.Reviews.Remove(rev);
				}

				/*List<int> ordersCompleted = db.OrderCompletes.Where(ordCom => ordCom.FreshenerId == freshener.FreshenerId).Select(ordC => ordC.OrderId).ToList();
				foreach (var orderCompleted in ordersCompleted)
				{

					Order orde = db.Orders.Find(orderCompleted);
						db.Orders.Remove(orde);
					}
				}*/

				db.Fresheners.Remove(fresh);
            }
			db.Categories.Remove(category);
			TempData["message"] = "The category has been deleted!";
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}