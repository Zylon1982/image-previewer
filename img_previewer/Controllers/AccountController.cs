using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using img_previewer.Models;

namespace img_previewer.Controllers
{
    public class AccountController : Controller
    {
        //Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserAccount user)
        {
            using (TaskDbContext db = new TaskDbContext())
            {
                var usr = db.userAccount.Where(e => e.Email == user.Email && e.Password == user.Password).FirstOrDefault();
                if(usr != null)
                {
                    Session["UserId"] = usr.UserId.ToString();
                    Session["Email"] = usr.Email.ToString();
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "UserName or Password is wrong.");
                }
            }
            return View();
        }

        public ActionResult LoggedIn()
        {
            if(Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            if(ModelState.IsValid)
            {
                using (TaskDbContext db = new TaskDbContext())
                {
                    db.userAccount.Add(account);
                    db.SaveChanges();
                }
                ModelState.Clear();
                ViewBag.Message = account.UserName + " succesfully registered.";
            }
            return View();
        }
    }
}