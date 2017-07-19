using BlogLayer.CF;
using BlogLayer.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogLayer.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            bool giris = Convert.ToBoolean(Session["giris"]);
            if (giris)
            {
                return View();
            }
            else
            {
               return RedirectToAction("login", "admin");
            }
            
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Login(AdminHelper model)
        {
            if (ModelState.IsValid)
            {
                using (RazorBlogContext _db = new RazorBlogContext())
                {
                    var admin = _db.Admins.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
                    if (admin!=null)
                    {
                        Session.Add("giris", true);
                        Session.Add("adminid", admin.AdminID);
                        return RedirectToAction("Index", "Admin");
                    }
                }
            }
            ViewBag.mesaj = "Kullanıcı Adı veya Şifre Yanlış";
            return View();
        }
    }
}