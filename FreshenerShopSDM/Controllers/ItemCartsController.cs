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

            var quantities = from q in db.ItemCarts where q.CartId == cart.CartId select q;

            if (quantities != null)
            {
                ViewBag.ItemCarts = quantities;
                ViewBag.currentUser = db.Users.Find(User.Identity.GetUserId());

                float totalSum = 0;

                foreach (var q in cart.ItemCarts)
                {
                    totalSum = totalSum + q.Quantity * q.Freshener.FreshenerPrice;
                }

                if (totalSum < 300)
                {
                    totalSum += 20;
                }

                ViewBag.Total = totalSum;
                return View();
            }
            else
            {
                throw new NullReferenceException("You can't see a cart that has no fresheners!");
            }

            
        }

        [HttpPost]
        public ActionResult New(ItemCart q)
        {
            //iau id-ul userului si id-ul cosului corespunzator acelui user
            var currentUser = User.Identity.GetUserId();
            Cart cart = db.Carts.Where(a => a.UserId == currentUser).First();
            //setez parametrul CartId la obiectul q primit dupa apasarea butonului "Adauga in cos", ceilalti 2 parametri fiind primiti din form
            q.CartId = cart.CartId;

            //verific daca exista deja produsul in cosul userului curent
            if (cart.ItemCarts.Where(a => a.FreshenerId == q.FreshenerId).Count() != 0)
            {
                //daca exista, iau produsul din cos printr-un obiect quantity_temp, incrementez cantitatea
                ItemCart quantity_temp = cart.ItemCarts.Where(a => a.FreshenerId == q.FreshenerId).First();
                quantity_temp.Quantity++;
                db.SaveChanges();
            }
            else
            {
                //daca nu exista acest produs in cos las cantitatea 1 si il adaug direct in baza de date
                db.ItemCarts.Add(q);
                db.SaveChanges();
            }

            return Redirect("/Fresheners/Index");
        }
    }
}