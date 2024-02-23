using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDoList.Models;
using Microsoft.AspNet.Identity;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ToDoes
        public ActionResult Index()
        {
            return View();
        }
        private IEnumerable<ToDo> getMyTodos()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.Id == currentUserId);
            IEnumerable<ToDo> myTodos = db.ToDos.ToList().Where(u => u.User == currentUser);

            int completedTodo = 0;
            foreach(ToDo todo in myTodos)
            {
                if (todo.IsDone)
                    completedTodo++;
            }

            ViewBag.percent =Math.Round((100f *(float) completedTodo) /(float) myTodos.Count());
            return myTodos;
        }

        public ActionResult BuildTodoTable()
        {
            return PartialView("_TodoTable", getMyTodos());
        }
        // GET: ToDoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id); 
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // GET: ToDoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.Id == currentUserId);
                toDo.User = currentUser;
                db.ToDos.Add(toDo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[OutputCache(Location = OutputCacheLocation.None , NoStore = true)]
        public ActionResult AjaxCreate([Bind(Include = "ID,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.Id == currentUserId);
                toDo.User = currentUser;
                toDo.IsDone = false;
                db.ToDos.Add(toDo);
                db.SaveChanges();
            }

            return PartialView("_TodoTable", getMyTodos());
        }

        [HttpPost]
        public ActionResult AjaxEdit(int? id , bool value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else
            {
                toDo.IsDone = value;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_TodoTable", getMyTodos());
            }
        }
        
        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.Id == currentUserId);
            ToDo toDo = db.ToDos.Find(id);
            if (toDo.User == currentUser)
            {
                db.ToDos.Remove(toDo);
                db.SaveChanges();

            }
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
