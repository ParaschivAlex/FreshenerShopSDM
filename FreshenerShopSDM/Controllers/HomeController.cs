using FreshenerShopSDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace FreshenerShopSDM.Controllers
{
	public class HomeController : Controller
	{
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
		{
            /*var fresheners = from ord in db.OrderCompletes
                             orderby ord.FreshenerId.Count().Desc()
                             group ord.FreshenerId                                
                             select top 3 ord.FreshenerId;*/

            IEnumerable<int> fresheners = db.OrderCompletes.GroupBy(oc => oc.FreshenerId).OrderByDescending(oc => oc.Count()).Take(3).Select(oc => oc.Key);
            var top3Fresheners = db.Fresheners.Where(f => fresheners.Contains(f.FreshenerId)).ToList();
            System.Diagnostics.Debug.WriteLine(fresheners);
            ViewBag.MostPopular = top3Fresheners;
            return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "sadassdasdadas";

			return View();
		}
	}
}