using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class News
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        [AllowHtml]
        [Required]
        [StringLength(500000, MinimumLength = 10, ErrorMessage = "Valid news content is required.")]
        public string Body { get; set; }
        public byte[] Image { get; set; }
    }
    public class NewsDBContext : DbContext
    {
        public DbSet<News> News { get; set; }
    }
}