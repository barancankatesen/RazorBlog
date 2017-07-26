using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogLayer.Models.DataModel;
using BlogLayer.CF;

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
    }
}