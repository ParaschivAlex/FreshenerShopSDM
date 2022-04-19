using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var contacts = from contact in db.Contacts
                             orderby contact.ContactModifyDate
                             select contact;
            ViewBag.Contacts = contacts;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Show(int id)
        {
            Contact contact = db.Contacts.Find(id);
            ViewBag.Contact = contact;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(contact);
        }

        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult New(Contact con)
        {
            con.ContactModifyDate = DateTime.Now;
            con.UserId = User.Identity.GetUserId();
            try
            {
                db.Contacts.Add(con);
                db.SaveChanges();
                //TempData["message"] = "Your contact form has been sent.";
                ViewBag.Message = "Your contact form has been sent.";
                return View(con);
            }
            catch (Exception)
            {
                //TempData["message"] = "The form has not been sent.";
                ViewBag.Message = "The form has not been sent.";
                return View(con);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
            TempData["message"] = "The contact form has been deleted!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}