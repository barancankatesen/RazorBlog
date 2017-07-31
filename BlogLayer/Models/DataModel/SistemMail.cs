using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlogLayer.Models.DataModel
{
    public class SistemMail
    {
        public int SistemMailID { get; set; }
        [
        Required(ErrorMessage = "Mail Giriniz"),
        DisplayName("Mail Adı")
        ]
        public string Mail { get; set; }
        [
            DataType(DataType.Password)
            Required(ErrorMessage = "Şifre Giriniz"),
        DisplayName("Şifre")
            ]
        public string Password { get; set; }

    }
}