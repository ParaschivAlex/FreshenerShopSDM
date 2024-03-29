﻿using FreshenerShopSDM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreshenerShopSDM.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();

		public ActionResult Index()
		{
			var users = from user in db.Users
						orderby user.UserName
						select user;
			ViewBag.UsersList = users;
			return View();
		}

		public ActionResult Show(string id)
		{
			ApplicationUser user = db.Users.Find(id);
			ViewBag.currentUser = User.Identity.GetUserId();
			string currentRole = user.Roles.FirstOrDefault().RoleId;
			var userRoleName = (from role in db.Roles
								where role.Id == currentRole
								select role.Name).First();
			ViewBag.roleName = userRoleName;
			return View(user);
		}

		public ActionResult Edit(string id)
		{
			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;
			return View(user);
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetAllRoles()
		{
			var selectList = new List<SelectListItem>();
			var roles = from role in db.Roles select role;
			foreach (var role in roles)
			{
				selectList.Add(new SelectListItem
				{
					Value = role.Id.ToString(),
					Text = role.Name.ToString()
				});
			}
			return selectList;
		}

		[HttpPut]
		public ActionResult Edit(string id, ApplicationUser newData)
		{
			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;
			try
			{
				ApplicationDbContext context = new ApplicationDbContext();
				var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
				var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
				if (TryUpdateModel(user))
				{
					user.UserName = newData.UserName;
					user.Email = newData.Email;
					user.PhoneNumber = newData.PhoneNumber;
					var roles = from role in db.Roles select role;
					foreach (var role in roles)
					{
						UserManager.RemoveFromRole(id, role.Name);
					}
					var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
					UserManager.AddToRole(id, selectedRole.Name);
					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Response.Write(e.Message);
				newData.Id = id;
				return View(newData);
			}
		}

		[HttpDelete]
		public ActionResult Delete(string id)
		{
			ApplicationDbContext context = new ApplicationDbContext();
			var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
			var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

			var reviews = db.Reviews.Where(rev => rev.UserId == id);
			foreach (var review in reviews)
			{
				db.Reviews.Remove(review);
			}

			var carts = db.Carts.Where(crt => crt.UserId == id);
			foreach (var cart in carts)
			{
				db.Carts.Remove(cart);
			}

			var orders = db.Orders.Where(ord => ord.UserId == id);
			foreach (var order in orders)
			{
				db.Orders.Remove(order);
			}

			var contactforms = db.Contacts.Where(cnt => cnt.UserId == id);
			foreach (var contact in contactforms)
			{
				db.Contacts.Remove(contact);
			}

			db.SaveChanges();
			UserManager.Delete(user);
			return RedirectToAction("Index");
		}
	}
}