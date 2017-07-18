using BlogLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BlogLayer.CF
{
    public class RazorBlogContext:DbContext
    {
        public RazorBlogContext()
        {
            Database.Connection.ConnectionString = "Server=.;Database=MyBlog;User Id=sa;Password=123456789?";
        }

        public DbSet<Category> Categories { get; set; }

    }
}