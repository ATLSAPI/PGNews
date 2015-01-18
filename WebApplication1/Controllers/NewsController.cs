using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class NewsController : Controller
    {
        private NewsDBContext db = new NewsDBContext();

        // GET: News
        public ActionResult Index(string sortBy, int? page)
        {
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortBy) ? "Name desc"
            : "";

            ViewBag.SortGenderParameter = string.IsNullOrEmpty(sortBy) ? "Gender desc"
                : "Gender";
            var employee = db.News.AsQueryable();

            switch (sortBy)
            {
                case "Name desc":
                    employee = employee.OrderByDescending(x => x.ReleaseDate);
                    break;

                default:
                    employee = employee.OrderByDescending(x => x.Title);
                    break;
            }
            return (View(employee.ToPagedList(pageNumber: page ?? 1, pageSize: 4)));
        }
       
        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Body,Image")] News news, HttpPostedFileBase theFile)
        {
            news.Image = ConvertToBytes(theFile);
            news.ReleaseDate = DateTime.Today;
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }
        // Covert to bytes
        public byte[] ConvertToBytes(HttpPostedFileBase theFile)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(theFile.InputStream);
            imageBytes = reader.ReadBytes((int)theFile.ContentLength);
            return imageBytes;
        }
        //
        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        //
        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.News where temp.ID == Id select temp.Image;
            byte[] cover = q.First();
            return cover;
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Body,Image")] News news, HttpPostedFileBase theFile)
        {
            if (theFile.ContentLength > 0)
            {

                news.Image = ConvertToBytes(theFile);
                news.ReleaseDate = DateTime.Today;
                if (ModelState.IsValid)
                {
                    db.Entry(news).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
                
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
