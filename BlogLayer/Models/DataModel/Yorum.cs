using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogLayer.Models.DataModel
{
    public class Yorum
    {
        public int YorumID { get; set; }
        [
        Required(ErrorMessage = "Lütfen Bir Ad Giriniz")
        ]
        public string Name { get; set; }
        [
        Required(ErrorMessage = "Lütfen Bir Yorum Yapınız")
        ]
        
        public string Comment { get; set; }
        
        public int OnayDurumu { get; set; }

    }
}