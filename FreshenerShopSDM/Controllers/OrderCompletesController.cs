using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    public class OrderCompletesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var ordersCompleted = from orderCompleted in db.OrderCompletes
                         orderby orderCompleted.OrderId
                         select orderCompleted;

            ViewBag.OrdersCompleted = ordersCompleted.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Show(int id)
        {
            OrderComplete orderCompleted = db.OrderCompletes.Find(id);
            Order order = db.Orders.Find(orderCompleted.OrderId);
            ViewBag.Order = order;
            ViewBag.OrdersCompleted = orderCompleted;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(orderCompleted);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            OrderComplete orderCompleted = db.OrderCompletes.Find(id);
            Order order = db.Orders.Find(orderCompleted.OrderId);
            ViewBag.Order = order;
            ViewBag.OrdersCompleted = orderCompleted;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(orderCompleted);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public ActionResult Edit(int id, OrderComplete requestOrderCompleted)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    OrderComplete orderCompleted = db.OrderCompletes.Find(id);

                    if (TryUpdateModel(orderCompleted))
                    {
                        Order order = db.Orders.Find(orderCompleted.OrderId);
                        ViewBag.Order = order;

                        orderCompleted = requestOrderCompleted;
                        db.SaveChanges();
                        TempData["message"] = "The order has been modified!";
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return View(requestOrderCompleted);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            OrderComplete orderCompleted = db.OrderCompletes.Find(id);
            db.OrderCompletes.Remove(orderCompleted);
            db.SaveChanges();
            TempData["message"] = "The order has been deleted!";
            return RedirectToAction("Index");
        }
    }
}