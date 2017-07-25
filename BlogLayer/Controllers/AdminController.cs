using BlogLayer.CF;
using BlogLayer.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogLayer.Models.DataModel;
using System.IO;

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
        public ActionResult KategoriSil(Category c)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Category ToDel = _db.Categories.FirstOrDefault(x => x.CategoryID == c.CategoryID);
            _db.Categories.Remove(ToDel);
            _db.SaveChanges();
            return RedirectToAction("KategoriListele");
        }

       

        public ActionResult KategoriGuncelle(int KategoriID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Category ToEdit = _db.Categories.FirstOrDefault(x => x.CategoryID == KategoriID);

            return View(ToEdit);
        }
        [HttpPost]
        public ActionResult KategoriGuncelle(Category c)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Category ToEdit = _db.Categories.FirstOrDefault(x => x.CategoryID == c.CategoryID);
            ToEdit.Name = c.Name;
            ToEdit.UrlName = c.UrlName;
            _db.SaveChanges();
            return RedirectToAction("KategoriListele");
        }

        public List<Category> TumKategoriler()
        {
            RazorBlogContext _db = new RazorBlogContext();
            List<Category> KategorilerList = _db.Categories.ToList();
            ViewBag.kategoriler = KategorilerList;
            return KategorilerList;
        }

        public List<Admin> TumAdminler()
        {
            RazorBlogContext _db = new RazorBlogContext();
            List<Admin> AdminList = _db.Admins.ToList();
            ViewBag.admins = AdminList;
            return AdminList;
        }

        public ActionResult MakaleEkle()
        {
            TumKategoriler();
            return View();
        }
        [HttpPost,ValidateInput(false)]
        public ActionResult MakaleEkle(FormCollection frm,HttpPostedFileBase file)
        {
            
            string ViewTitle = frm.Get("Title");
            string ViewDescription = frm.Get("Description");
            int ViewCategoryID = Convert.ToInt32(frm.Get("Category"));
            string ViewText = frm.Get("Text");
            string ViewKeywords = frm.Get("Keywords");
            int ViewYazarID = Convert.ToInt32(Session["adminid"]);

            Random rnd = new Random();
            string sayi = rnd.Next(111111, 999999).ToString();
            var FileName = Path.GetFileName(file.FileName);
            var File = Path.Combine(Server.MapPath("~/MakaleResimler/"), sayi + FileName);
            file.SaveAs(File);
            string FilePath = "~/MakaleResimler/" + sayi + FileName;

            Makale ToAdd = new Makale();
            ToAdd.AuthorName = TumAdminler().FirstOrDefault(x => x.AdminID == ViewYazarID);
            ToAdd.Category = TumKategoriler().FirstOrDefault(x => x.CategoryID == ViewCategoryID);
            ToAdd.Description = ViewDescription;
            ToAdd.Image = FilePath;
            ToAdd.Keywords = ViewKeywords;
            ToAdd.Text = ViewText;
            ToAdd.Title = ViewTitle;
            RazorBlogContext _db = new RazorBlogContext();
            _db.Makales.Add(ToAdd);
            if (_db.SaveChanges()>0)
            {
                ViewBag.Mesaj = "Makale Ekleme Başarılı";
            }
            else
            {
                ViewBag.Mesaj = "";
            }
            return View();
        }
        public ActionResult MakaleListele()
        {
            RazorBlogContext _db = new RazorBlogContext();
            return View(_db.Makales.ToList());
        }

        public ActionResult MakaleSil(int GelenMakaleID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Makale ToDel = _db.Makales.FirstOrDefault(x => x.MakaleID == GelenMakaleID);
            return View(ToDel);
        }
        [HttpPost]
        public ActionResult MakaleSil(Makale m)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Makale ToDel = _db.Makales.FirstOrDefault(x => x.MakaleID == m.MakaleID);
            _db.Makales.Remove(ToDel);
            _db.SaveChanges();
            return RedirectToAction("MakaleListele");
        }

        public ActionResult MakaleGuncelle(int GelenMakaleID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Makale ToEdit = _db.Makales.FirstOrDefault(x => x.MakaleID == GelenMakaleID);
            List<Category> AllCategoryList = _db.Categories.ToList();
            List<SelectListItem> ForDropDown = new List<SelectListItem>();
            foreach (Category item in AllCategoryList)
            {
                SelectListItem sl = new SelectListItem();
                if (ToEdit.Category.CategoryID == item.CategoryID)
                {
                    sl.Selected = true;
                }
                sl.Text = item.Name;
                sl.Value = item.CategoryID.ToString();
                ForDropDown.Add(sl);
            }
            ViewBag.SelectedCategoryListForDropDown = ForDropDown;
            ViewBag.asddsa = new SelectList((_db.Categories), "CategoryID", "Name", 3);
            return View(ToEdit);
        }

       
    }
}