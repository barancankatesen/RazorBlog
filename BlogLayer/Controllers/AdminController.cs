using BlogLayer.CF;
using BlogLayer.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogLayer.Models.DataModel;

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

        private String UrlTemizle(string data)
        {
            data = data.Replace(",", "").Replace("\"", "").Replace(":", "").Replace(";", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace(")", "").Replace("(", "").Replace("&", "").Replace(" ", "").Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u").Replace("/","").Replace("'\'","");

            return data;
        }

        public ActionResult KategoriEkleOto()
        {

            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkleOto(FormCollection frm)
        {

          return OzhakikiKategoriEkle(frm);
        }
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(FormCollection frm)
        {
            return OzhakikiKategoriEkle(frm);

        }

        public ActionResult OzhakikiKategoriEkle(FormCollection frm)
        {
            string KategoriAdi = frm.Get("CategoryName");
            if (KategoriAdi == "" || KategoriAdi == null)
            {
                ViewBag.hatavar = true;
                ViewBag.hata = "Kategori Adı Boş Geçilemez";
            }
            else
            {
                ViewBag.hatavar = false;
                RazorBlogContext _db = new RazorBlogContext();
                List<Category> CakisanKategoriler = _db.Categories.Where(x => x.Name == KategoriAdi).ToList();
                if (CakisanKategoriler.Count > 0)
                {
                    ViewBag.hatavar = true;
                    ViewBag.hata = "Bu Kategori Zaten Mevcut";
                }
                else
                {
                    ViewBag.hatavar = false;
                    CategoryHelper chelp = new CategoryHelper();
                    chelp.kategoriolustur(KategoriAdi);
                    Category c = new Category();
                    c.Name = chelp.CategoryName;
                    c.UrlName = chelp.UrlName;
                    _db.Categories.Add(c);
                    if (_db.SaveChanges() == 0)
                    {
                        ViewBag.hatavar = true;
                        ViewBag.hata = "veritabanı bağlantısında bir sorun oluştu";
                    }
                    else
                    {
                        ViewBag.hatavar = false;
                    }
                }
            }
            return View();
        }
        public ActionResult KategoriListele()
        {
            RazorBlogContext _db = new RazorBlogContext();
            return View(_db.Categories.ToList());
        }

        public ActionResult KategoriSil(int KategoriID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Category CategoryToDel = _db.Categories.FirstOrDefault(x => x.CategoryID == KategoriID);
            return View(CategoryToDel);
        }
        [HttpPost]
        public ActionResult KategoriSil(FormCollection frm)
        {
            int gelenid = Convert.ToInt32(frm.Get("IDCarrier"));
            RazorBlogContext _db = new RazorBlogContext();
            Category toDel = _db.Categories.FirstOrDefault(x => x.CategoryID == gelenid);
            _db.Categories.Remove(toDel);
            _db.SaveChanges();
            return RedirectToAction("KategoriListele");
        }
       
    }
}