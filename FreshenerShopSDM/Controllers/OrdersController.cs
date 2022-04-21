using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var orders = from order in db.Orders
                           orderby order.OrderModifyDate
                           select order;
            ViewBag.Orders = orders;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Show(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.Order = order;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(order);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult New(Order ord)
        {
            ord.OrderModifyDate = DateTime.Now;
            ord.UserId = User.Identity.GetUserId();
            ord.OrderUsername = User.Identity.GetUserName();

            var currentUser = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(a => a.UserId == currentUser).First();

            var itemsInCart = from ic in db.ItemCarts where ic.CartId == cart.CartId select ic;

            if (itemsInCart != null)
            {
                ViewBag.ItemCarts = itemsInCart;
                ViewBag.currentUser = db.Users.Find(User.Identity.GetUserId());

                float totalSum = 0;

                foreach (var ic in cart.ItemCarts)
                {
                    totalSum = totalSum + ic.ItemCartQuantity * ic.Freshener.FreshenerPrice;
                }

                if (totalSum < 300)
                {
                    totalSum += 20;
                }

                ord.OrderTotal = totalSum;
            }
            try
            {
                db.Orders.Add(ord);
                db.SaveChanges();
                //TempData["message"] = "Your contact form has been sent.";
                ViewBag.Message = "Your contact form has been sent.";
                return View(ord);
            }
            catch (Exception)
            {
                //TempData["message"] = "The form has not been sent.";
                ViewBag.Message = "The form has not been sent.";
                return View(ord);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            TempData["message"] = "The contact form has been deleted!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}