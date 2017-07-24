using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogLayer.Models.DataModel;


namespace BlogLayer.Models.DataModel
{
    public class Makale
    {
        public int MakaleID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public Admin AuthorName { get; set; }


    }
}