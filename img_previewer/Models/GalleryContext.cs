using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using img_previewer.Models;

namespace img_previewer.Models
{
    public class GalleryContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }
    }
}