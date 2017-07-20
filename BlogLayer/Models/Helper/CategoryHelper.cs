using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogLayer.Models.Helper
{
    public class CategoryHelper
    {

        public string CategoryName { get; set; }

        public string UrlName { get; set; }
        
        public static string UrlTemizle(string Temizlenecek)
        {
            Temizlenecek = Temizlenecek.Replace(",", "").Replace("\"", "").Replace(":", "").Replace(";", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace(")", "").Replace("(", "").Replace("&", "").Replace(" ", "").Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ş", "s").Replace("ü", "u").Replace("/","").Replace("'\'","").Replace("'","");
            return Temizlenecek;
        }
        public void kategoriolustur(string Name)
        {
            CategoryName = Name;

            Name = UrlTemizle(Name);

            UrlName = Name;

        }

    }
}