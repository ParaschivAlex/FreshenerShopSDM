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
    public class ItemCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
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

                if (totalSum > 0 && totalSum < 300)
                {
                    totalSum += 20;
                }
                System.Diagnostics.Debug.WriteLine(totalSum);
                ViewBag.TotalSum = totalSum;

                return View();
            }
            else
            {
                throw new NullReferenceException("You can't see a cart that has no fresheners!");
            }           
        }

        [HttpPost]
        public ActionResult New(ItemCart ic)
        {
            var currentUser = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(a => a.UserId == currentUser).First();
            ic.CartId = cart.CartId;

            if (cart.ItemCarts.Where(a => a.FreshenerId == ic.FreshenerId).Count() != 0)
            {
                ItemCart itemInCart = cart.ItemCarts.Where(a => a.FreshenerId == ic.FreshenerId).First();
                itemInCart.ItemCartQuantity++;
            }
            else
            {
                db.ItemCarts.Add(ic);    
            }
            db.SaveChanges();

            return Redirect("/ItemCarts/Index");
        }

        [HttpPut]
        public ActionResult IncrementItem(int id)
        {
            ItemCart ic = db.ItemCarts.Find(id);
            ic.ItemCartQuantity++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPut]
        public ActionResult DecreaseItem(int id)
        {
            ItemCart ic = db.ItemCarts.Find(id);
            if (ic.ItemCartQuantity == 1)
            {
                db.ItemCarts.Remove(ic);
            }
            else
            {
                ic.ItemCartQuantity--;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            ItemCart ic = db.ItemCarts.Find(id);
            db.ItemCarts.Remove(ic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}