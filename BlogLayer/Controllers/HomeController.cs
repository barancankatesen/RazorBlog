﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogLayer.Models.DataModel;
using BlogLayer.CF;
using BlogLayer.Models.Helper;

namespace BlogLayer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string StrSayfa)
        {
            int Sayfa = 0;
            if (StrSayfa==""||StrSayfa==null)
            {
                Sayfa = 1;
            }
            else
            {
                Sayfa = Convert.ToInt32(StrSayfa);
            }
            int Kactan = 0;
            int Kacar = 2;
            RazorBlogContext _db = new RazorBlogContext();
            //List<Makale> MakalelerList = _db.Makales.ToList();
            int MakaleSayisi = _db.Makales.Count();
            double SayfaSayisi = Math.Ceiling((float)((1.0 * MakaleSayisi) / Kacar));
            ViewBag.sayfasayisi = SayfaSayisi;
            ViewBag.sayfa = Sayfa;
            Kactan = ((Sayfa-1) * Kacar);
            List<Makale> SayfaMakale = new List<Makale>(_db.Makales.OrderByDescending(x=>x.MakaleID).Skip(Kactan).Take(Kacar));
            ViewBag.sayfamakale =SayfaMakale;
            return View();
        }
        public ActionResult MakaleGoster(int ViewMakaleID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            Makale MakaleToShow = _db.Makales.FirstOrDefault(x => x.MakaleID == ViewMakaleID);
            return View(MakaleToShow);
        }
        [HttpPost]
        public ActionResult MakaleGoster(FormCollection frm ,int ViewMakaleID)
        {
            RazorBlogContext _db = new RazorBlogContext();
            string ViewCommenterName = frm.Get("isim");
            string ViewComment = frm.Get("yorum");
            Yorum YorumToAdd = new Yorum();
            YorumToAdd.Name = ViewCommenterName;
            YorumToAdd.Comment = ViewComment;
            Makale MakaleToEdit = _db.Makales.FirstOrDefault(x => x.MakaleID == ViewMakaleID);
            MakaleToEdit.Yorumlar.Add(YorumToAdd);
            if (_db.SaveChanges()>0)
            {
                ViewBag.Mesaj = "Yorum Başarı İle Gönderildi Yöneticiler Tarafından Onaylanınca Yayınlanacak";
            }
            else
            {
                ViewBag.Mesaj = "Yorum Gönderme Sırasında Bir Hata Oluştu";
            }
            return View(MakaleToEdit);
        }
        public ActionResult KategoriGoster(int ViewKategoriID, string StrSayfa)
        {
            int Sayfa = 0;
            if (StrSayfa == "" || StrSayfa == null)
            {
                Sayfa = 1;
            }
            else
            {
                Sayfa = Convert.ToInt32(StrSayfa);
            }
            int Kactan = 0;
            int Kacar = 2;
            RazorBlogContext _db = new RazorBlogContext();
            int MakaleSayisi = _db.Makales.Where(x=>x.Category.CategoryID==ViewKategoriID).Count();
            double SayfaSayisi = Math.Ceiling((float)((1.0 * MakaleSayisi) / Kacar));
            ViewBag.sayfasayisi = SayfaSayisi;
            ViewBag.sayfa = Sayfa;
            Kactan = ((Sayfa - 1) * Kacar);
            List<Makale> SayfaMakale = new List<Makale>(_db.Makales.Where(x=>x.Category.CategoryID==ViewKategoriID).OrderByDescending(x => x.MakaleID).Skip(Kactan).Take(Kacar));
            ViewBag.sayfamakale = SayfaMakale;
            ViewBag.AktifKategoriID = ViewKategoriID;
            return View();
        }
        public ActionResult Hakkimizda()
        {

            return View();
        }
        public ActionResult Iletisim()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Iletisim(FormCollection frm)
        {
            string isim = frm.Get("isim");
            string mail = frm.Get("email");
            string mesaj = frm.Get("mesaj");
            string body = String.Format("<table><tr><td>İsim</td><td>E-Mail</td><td>Mesaj</td></tr><tr><td>{0}</td><td>{1}</td><td>{2}</td></tr></table>", isim, mail, mesaj);
            Mail ToSend = new Mail();
            if (ToSend.MailGonder(mail, "Blog Sitesinden Gelen MesajBaran", body))
            {
                ViewBag.Mesaj = "Mesajınız başarılı bir şekilde gönderildi";
              

            }
            else
            {
                ViewBag.Mesaj = "Mesaj gönderimi sırasında bir hata oluştu";
            }
                

            return View();
        }
    }
}