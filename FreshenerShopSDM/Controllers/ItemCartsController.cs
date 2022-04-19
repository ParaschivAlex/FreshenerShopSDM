using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    public class ItemCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var currentUser = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(a => a.UserId == currentUser).First();

            var items_in_carts = from ic in db.ItemCarts where ic.CartId == cart.CartId select ic;
            ViewBag.Quantities = items_in_carts;
            ViewBag.currentUser = db.Users.Find(User.Identity.GetUserId());

            float totalSum = 0;

            foreach (var ic in cart.ItemCarts)
            {
                totalSum = totalSum + ic.Quantity * ic.Freshener.FreshenerPrice;
            }

            ViewBag.Total = totalSum;

            return View();
        }

        [HttpPost]
        public ActionResult New(ItemCart ic)
        {
            var currentUser = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(a => a.UserId == currentUser).First();

            ic.CartId = cart.CartId;

            if (cart.ItemCarts.Where(a => a.FreshenerId == ic.FreshenerId).Count() != 0)
            {
                ItemCart quantity_temp = cart.ItemCarts.Where(a => a.FreshenerId == ic.FreshenerId).First();
                quantity_temp.Quantity++;
                db.SaveChanges();
            }
            else
            {
                db.ItemCarts.Add(ic);
                db.SaveChanges();
            }

            return Redirect("/Fresheners/Index");
        }

        [HttpPut]
        public ActionResult EditPlus(int id)
        {
            ItemCart ic = db.ItemCarts.Find(id);
            ic.Quantity++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPut]
        public ActionResult EditMinus(int id)
        {
            ItemCart ic = db.ItemCarts.Find(id);
            if (ic.Quantity == 1)
            {
                db.ItemCarts.Remove(ic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ic.Quantity--;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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