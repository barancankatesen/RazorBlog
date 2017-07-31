using BlogLayer.CF;
using BlogLayer.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogLayer.Models.DataModel;
using System.IO;
using System.Web.Hosting;

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
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(AdminHelper model)
        {
            if (ModelState.IsValid)
            {
                using (RazorBlogContext _db = new RazorBlogContext())
                {
                    var admin = _db.Admins.FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);
                    if (admin != null)
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
            data = data.Replace(",", "").Replace("\"", "").Replace(":", "").Replace(";", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace(")", "").Replace("(", "").Replace("&", "").Replace(" ", "").Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u").Replace("/", "").Replace("'\'", "");

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
        [HttpPost, ValidateInput(false)]
        public ActionResult MakaleEkle(FormCollection frm, HttpPostedFileBase file)
        {

            string ViewTitle = frm.Get("Title");
            string ViewDescription = frm.Get("Description");
            int ViewCategoryID = Convert.ToInt32(frm.Get("Category.CategoryID"));
            string ViewText = frm.Get("Text");
            string ViewKeywords = frm.Get("Keywords");
            int ViewYazarID = Convert.ToInt32(Session["adminid"]);




            RazorBlogContext _db = new RazorBlogContext();
            Makale ToAdd = new Makale();
            ToAdd.AuthorName = _db.Admins.FirstOrDefault(x => x.AdminID == ViewYazarID);
            ToAdd.Category = _db.Categories.FirstOrDefault(x => x.CategoryID == ViewCategoryID);
            ToAdd.Description = ViewDescription;
            if (file != null)
            {
                Random rnd = new Random();
                string sayi = rnd.Next(111111, 999999).ToString();
                string FileName = Path.GetFileName(file.FileName);
                string File = Path.Combine(Server.MapPath("~/MakaleResimler/"), sayi + FileName);
                file.SaveAs(File);
                string FilePath = "~/MakaleResimler/" + sayi + FileName;
                ToAdd.Image = FilePath;
            }

            ToAdd.Keywords = ViewKeywords;
            ToAdd.Text = ViewText;
            ToAdd.Title = ViewTitle;

            _db.Makales.Add(ToAdd);
            if (_db.SaveChanges() > 0)
            {
                ViewBag.Mesaj = "Makale Ekleme Başarılı";
            }
            else
            {
                ViewBag.Mesaj = "";
            }
            TumKategoriler();
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
            List<SelectListItem> ForDropDown = new List<SelectListItem>();
            List<Category> AllCategoryList = _db.Categories.ToList();
            foreach (var item in AllCategoryList)
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
            return View(ToEdit);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult MakaleGuncelle(FormCollection frm, HttpPostedFileBase file)
        {
            int ViewMakaleID = Convert.ToInt32(frm.Get("MakaleID"));
            string ViewTitle = frm.Get("Title");
            string ViewDescription = frm.Get("Description");
            int ViewCategoryID = Convert.ToInt32(frm.Get("Category.CategoryID"));
            string ViewText = frm.Get("Text");
            string ViewKeywords = frm.Get("Keywords");
            bool ViewResmiSil = frm.Get("ResmiSil").Contains("true");
            RazorBlogContext _db = new RazorBlogContext();
            Makale MakaleToEdit = _db.Makales.FirstOrDefault(x => x.MakaleID == ViewMakaleID);
            Category CategoryToAdd = _db.Categories.FirstOrDefault(x => x.CategoryID == ViewCategoryID);
            if (ViewResmiSil)
            {

                string FileFullPath = Request.MapPath(MakaleToEdit.Image);
                System.IO.File.Delete(FileFullPath);
                MakaleToEdit.Image = null;
            }
            else
            {
                if (file != null)
                {
                    string FileFullPath = Request.MapPath(MakaleToEdit.Image);
                    System.IO.File.Delete(FileFullPath);


                    Random rnd = new Random();
                    string sayi = rnd.Next(111111, 999999).ToString();
                    var FileName = Path.GetFileName(file.FileName);
                    var File = Path.Combine(Server.MapPath("~/MakaleResimler/"), sayi + FileName);
                    file.SaveAs(File);
                    string FilePath = "~/MakaleResimler/" + sayi + FileName;
                    MakaleToEdit.Image = FilePath;
                }
            }
            MakaleToEdit.Title = ViewTitle;
            MakaleToEdit.Description = ViewDescription;
            MakaleToEdit.Text = ViewText;
            MakaleToEdit.Keywords = ViewKeywords;
            MakaleToEdit.Category = CategoryToAdd;
            _db.SaveChanges();
            return RedirectToAction("MakaleListele");
        }
        public ActionResult YorumlariYonet()
        {
            RazorBlogContext _db = new RazorBlogContext();
            List<Yorum> YorumlarList = _db.Yorums.ToList();

            return View(YorumlarList);
        }
        public ActionResult YorumOnayla(int ViewYorumID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Yorum ToEdit = _db.Yorums.FirstOrDefault(x => x.YorumID == ViewYorumID);
            ToEdit.OnayDurumu = 1;
            _db.SaveChanges();
            return RedirectToAction("YorumlariYonet");
        }
        public ActionResult YorumOnayiKaldir(int ViewYorumID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Yorum ToEdit = _db.Yorums.FirstOrDefault(x => x.YorumID == ViewYorumID);
            ToEdit.OnayDurumu = 0;
            _db.SaveChanges();
            return RedirectToAction("YorumlariYonet");
        }
        public ActionResult YorumSil(int ViewYorumID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Yorum ToDelete = _db.Yorums.FirstOrDefault(x => x.YorumID == ViewYorumID);
            _db.Yorums.Remove(ToDelete);
            _db.SaveChanges();
            return RedirectToAction("YorumlariYonet");

        }
        public ActionResult Cikis()
        {
            Session.Remove("giris");
            Session.Remove("adminid");
            return RedirectToAction("index", "Home");
        }
        public ActionResult MailGuncelle()
        {
            RazorBlogContext _db = new RazorBlogContext();
            SistemMail ToEdit = _db.SistemMails.FirstOrDefault(x => x.SistemMailID == 1);
            return View(ToEdit);
        }
        [HttpPost]
        public ActionResult MailGuncelle(FormCollection frm)
        {
            RazorBlogContext _db = new RazorBlogContext();
            SistemMail ToEdit = _db.SistemMails.FirstOrDefault(x => x.SistemMailID == 1);
            string ViewMail = frm.Get("Mail");
            string ViewPassword = frm.Get("Password");
            ToEdit.Mail = ViewMail;
            ToEdit.Password = ViewPassword;
            if (_db.SaveChanges() > 0)
            {
                ViewBag.mesaj = "Mail Ayarları Güncellendi";

            }
            else
            {
                ViewBag.mesaj = "Mail Ayarları Güncelleme Esnasında Bir Hata Oluştu";
            }
            return RedirectToAction("index");
            
        }

    }
}