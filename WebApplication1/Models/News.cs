using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public byte[] Image { get; set; }
    }
    public class NewsDBContext : DbContext
    {
        public DbSet<News> News { get; set; }
    }
}