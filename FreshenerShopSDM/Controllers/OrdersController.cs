using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin, User")]
        public ActionResult New()
        {

            Order order = new Order();
            order.OrderUsername = User.Identity.GetUserName();
            order.UserId = User.Identity.GetUserId();

            if (CheckEmptyCart() == false)
            {
                TempData["message"] = "Your cart is empty!";
                return Redirect("/ItemCarts/Index/");
            }

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

                float TS = (float)Math.Round(totalSum * 100f) / 100f;

                order.OrderTotal = TS;

            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Orders.Add(order);
                    //db.SaveChanges();
                    AddOrderIdToItemCart(order.OrderId);

                    db.SaveChanges();

                    //Console.WriteLine("DB.SAVEDCHANGES");
                    TempData["message"] = "The order has been added!";

                    ViewBag.OrderId = order.OrderId;

                    CompleteOrderDeleteItemsFromItemCart();

                    return RedirectToAction("CompleteOrder", new { id = order.OrderId });
                }
                else
                {
                    Console.WriteLine("Error on modelstate.isvalid for adding a new order.");
                    TempData["message"] = "Something went wrong!";
                    return Redirect("/Home/Index/");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error on try catch for adding a new order.");
                TempData["message"] = "Something went wrong!";
                return Redirect("/Home/Index/");
            }
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
            //System.Diagnostics.Debug.WriteLine(id);

            foreach (var completedItem in completedItems)
            {
                db.OrderCompletes.Add(completedItem);
            }
        }

        public void CompleteOrderDeleteItemsFromItemCart()
        {
            var currentUser = User.Identity.GetUserId();
            var cart = db.Carts.Where(c => c.UserId == currentUser).FirstOrDefault();
            var items = db.ItemCarts.Where(i => i.CartId == cart.CartId);

            foreach (var item in items)
            {
                db.ItemCarts.Remove(item);
            }

            db.SaveChanges();
        }

        public bool CheckEmptyCart()
        {
            var currentUser = User.Identity.GetUserId();
            var cart = db.Carts.Where(c => c.UserId == currentUser).FirstOrDefault();
            var items = db.ItemCarts.Where(i => i.CartId == cart.CartId);

            foreach (var item in items)
            {
                //System.Diagnostics.Debug.WriteLine(item);
                //if (item.ItemCartId != null)
                //{
                return true;
                // }
            }
            return false;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.Order = order;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(order);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Order requestOrder)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Order order = db.Orders.Find(id);

                    if (TryUpdateModel(order))
                    {
                        order = requestOrder;
                        order.OrderModifyDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["message"] = "The order has been modified!";
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return View(requestOrder);
            }

        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult CompleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.Order = order;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(order);

        }
    }
}