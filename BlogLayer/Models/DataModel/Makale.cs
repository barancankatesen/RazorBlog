using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogLayer.Models.DataModel;
using System.ComponentModel.DataAnnotations;

namespace BlogLayer.Models.DataModel
{
    public class Makale
    {
        public int MakaleID { get; set; }
        [
        Required(ErrorMessage = "Lütfen Bir Başlık Giriniz")
        ]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        [
        Required(ErrorMessage = "Lütfen Bir Kategori Seçiniz")
        ]
        public virtual Category Category { get; set; }
        public virtual Admin AuthorName { get; set; }
        public string Keywords { get; set; }

        public virtual List<Yorum> Yorumlar { get; set; }



    }
}