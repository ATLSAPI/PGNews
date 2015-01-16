using System;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Body { get; set; }
        public byte[] Image { get; set; }
    }
    public class NewsDBContext : DbContext
    {
        public DbSet<News> News { get; set; }
    }
}