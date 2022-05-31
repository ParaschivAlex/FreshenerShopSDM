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

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var orders = from order in db.Orders
                                  orderby order.OrderId
                                  select order;

            ViewBag.Orders = orders.ToList();
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult UserOrders()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var currentUser = User.Identity.GetUserId();

            var orders = from order in db.Orders
                         where order.UserId == currentUser
                         orderby order.OrderId
                         select order;

            ViewBag.Orders = orders.ToList();
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult New()
        {

            Order order = new Order();
            
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
                    AddOrderIdToItemCart(order.OrderId);

                    db.SaveChanges();

                    TempData["message"] = "The order has been added!";

                    ViewBag.OrderId = order.OrderId;

                    var currentUserEmail = db.Users.FirstOrDefault(u => u.Id == currentUser);
                    order.OrderUsername = currentUserEmail.Email;

                    CompleteOrderDeleteItemsFromItemCart();

                    return RedirectToAction("CompleteOrder", new { id = order.OrderId });
                }
                else
                {
                    Console.WriteLine("Error on modelstate.isvalid for adding a new order.");
                    TempData["message"] = "Order not added!";
                    return View(order);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error on try catch for adding a new order.");
                TempData["message"] = "Order not added!";
                return View(order);
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
                return true;
            }
            return false;
        }

        [Authorize(Roles = "User, Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var currentUser = User.Identity.GetUserId();
            Order order = db.Orders.Find(id);
            if (User.IsInRole("Admin") || currentUser == order.UserId)
            {

                var items = db.OrderCompletes.Where(o => o.OrderId == order.OrderId);

                foreach (var item in items)
                {
                    db.OrderCompletes.Remove(item);
                }
                db.Orders.Remove(order);

                db.SaveChanges();
                TempData["message"] = "The order has been deleted!";
                
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                return Redirect("/Home/Index");
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            return Redirect("/Home/Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Show(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.OrdersCompleted = order.OrderCompletes;
            ViewBag.Order = order;
            return View(order);
        }

        [Authorize(Roles = "Admin, User")]
        public ActionResult UserShowOrder(int id)
        {
            var currentUser = User.Identity.GetUserId();
            Order order = db.Orders.Find(id);
            if (User.IsInRole("Admin") || currentUser == order.UserId)
            {
                ViewBag.OrdersCompleted = order.OrderCompletes;
                ViewBag.Order = order;
                return View(order);
            }
            return Redirect("/Home/Index");
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