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
        public ActionResult Edit(int id)
        {
            Order order = db.Orders.Find(id);

            ViewBag.OrdersCompleted = order.OrderCompletes;
            ViewBag.Order = order;
            return View(order);

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
    }
}