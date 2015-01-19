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
        public ActionResult Index(string sortBy, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortBy;
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.SortGenderParameter = string.IsNullOrEmpty(sortBy) ? "Gender desc" : "Gender";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var employee = from s in db.News
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                employee = employee.Where(s => s.Title.Contains(searchString));
                ViewBag.SearchMessage = employee.Count() + " news items match your search criteria:"+" '"+searchString+"'";
                ViewBag.Clear = "<input type='reset' value='Reset' />";
            }

            switch (sortBy)
            {
                case "Name desc":
                    employee = employee.OrderByDescending(x => x.ReleaseDate);
                    break;

                default:
                    employee = employee.OrderByDescending(x => x.ReleaseDate);
                    break;
            }
            if (employee.Count()<1)
            {
                ViewBag.Message = "No Results Found";
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
        [Authorize(Roles = "news-admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "news-admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Body,Image")] News news, HttpPostedFileBase theFile)
        {
            news.Image = ConvertToBytes(theFile);
            news.ReleaseDate = DateTime.Now;
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
        [Authorize(Roles = "news-admin")]
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
        [Authorize(Roles = "news-admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Body,Image")] News news, HttpPostedFileBase theFile)
        {
            if (theFile.ContentLength > 0)
            {

                news.Image = ConvertToBytes(theFile);
                news.ReleaseDate = DateTime.Now;
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
        [Authorize(Roles = "news-admin")]
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
        [Authorize(Roles = "news-admin")]
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
