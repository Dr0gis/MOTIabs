using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MOTI;

namespace MOTI.Controllers
{
    public class MarksController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: Marks
        public ActionResult Index()
        {
            var mark = db.Mark.Include(m => m.Criterion);
            List<string> criterionNames = db.Criterion.Select(c => c.CName).ToList();
            ViewBag.CriterionNames = criterionNames;
            return View(mark.ToList());
        }

        // GET: Marks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Mark.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        // GET: Marks/Create
        public ActionResult Create()
        {
            ViewBag.IdCrit = new SelectList(db.Criterion, "IdCrit", "CName");
            ViewBag.Marks = db.Mark.ToList();
            ViewBag.ListCrit = db.Criterion.ToList();
            return View();
        }

        // POST: Marks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMark,IdCrit,MName,MRange,NumMark,NormMark")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                db.Mark.Add(mark);
                db.SaveChanges();
                Mark createdMark = db.Mark.Where(m => m.IdMark == mark.IdMark).FirstOrDefault();
                Criterion criterionMark = db.Criterion.Where(c => c.IdCrit == createdMark.IdCrit).FirstOrDefault();
                if (criterionMark.CType == "Количественный")
                {
                    createdMark.NumMark = Int32.Parse(createdMark.MName);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.IdCrit = new SelectList(db.Criterion, "IdCrit", "CName", mark.IdCrit);
            return View(mark);
        }

        // GET: Marks/Edit/5 
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Mark.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            //.Where( m => m.CName == cName)
            ViewBag.IdCrit = new SelectList(db.Criterion, "IdCrit", "CName", mark.IdCrit);
            return View(mark);
        }

        // POST: Marks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMark,IdCrit,MName,MRange,NumMark,NormMark")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mark).State = EntityState.Modified;
                db.SaveChanges();
                Mark createdMark = db.Mark.Where(m => m.IdMark == mark.IdMark).FirstOrDefault();
                Criterion criterionMark = db.Criterion.Where(c => c.IdCrit == createdMark.IdCrit).FirstOrDefault();
                if (criterionMark.CType == "Количественный")
                {
                    createdMark.NumMark = Int32.Parse(createdMark.MName);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.IdCrit = new SelectList(db.Criterion, "IdCrit", "CName", mark.IdCrit);
            return View(mark);
        }

        // GET: Marks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark mark = db.Mark.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mark mark = db.Mark.Find(id);
            db.Mark.Remove(mark);
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
