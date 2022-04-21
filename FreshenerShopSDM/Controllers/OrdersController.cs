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
        public ActionResult New()
        {

            Order order = new Order();
            order.OrderUsername = User.Identity.GetUserName();
            order.UserId = User.Identity.GetUserId();        

            return View(order);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult New(Order order)
        {
            order.OrderModifyDate = DateTime.Now;
            order.UserId = User.Identity.GetUserId();

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

                order.OrderTotal = totalSum;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                    AddOrderIdToItemCart(order.OrderId);

                    db.SaveChanges();
                    //Console.WriteLine("DB.SAVEDCHANGES");
                    TempData["message"] = "The order has been added!";
                    return View(order);
                }
                else
                {
                    Console.WriteLine("Error on modelstate.isvalid for adding a new order.");
                    return View(order);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error on try catch for adding a new order.");
                return View(order);
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

        public void AddOrderIdToItemCart(int id)
        {
            var currentUser = User.Identity.GetUserId();
            var cart = db.Carts.Where(c => c.UserId == currentUser).FirstOrDefault();
            var items = db.ItemCarts.Where(i => i.CartId == cart.CartId);

            foreach (var item in items)
            {
                item.OrderId = id;
            }

            var completedItems = items.ToList().Select(i => new OrderComplete
            {
                FreshenerId = i.FreshenerId,               
                OrderId = id,
                FreshenerQuantity = i.ItemCartQuantity
            });
            System.Diagnostics.Debug.WriteLine(id);

            foreach (var completedItem in completedItems)
            {
                db.OrderCompletes.Add(completedItem);
            }
        }
    }
}