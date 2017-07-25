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
        public virtual Category Category { get; set; }
        public virtual Admin AuthorName { get; set; }
        public string Keywords { get; set; }


    }
}