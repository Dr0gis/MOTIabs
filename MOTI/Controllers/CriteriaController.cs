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
    public class CriteriaController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: Criteria
        public ActionResult Index()
        {
            return View(db.Criterion.ToList());
        }

        // GET: Criteria/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criterion criterion = db.Criterion.Find(id);
            if (criterion == null)
            {
                return HttpNotFound();
            }
            return View(criterion);
        }

        // GET: Criteria/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Criteria/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdCrit,CName,CRange,CWeight,OptimType,EdIzmer,ScaleType")] Criterion criterion)
        {
            if (ModelState.IsValid)
            {
                db.Criterion.Add(criterion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(criterion);
        }

        // GET: Criteria/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criterion criterion = db.Criterion.Find(id);
            if (criterion == null)
            {
                return HttpNotFound();
            }
            return View(criterion);
        }

        // POST: Criteria/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCrit,CName,CRange,CWeight,OptimType,EdIzmer,ScaleType")] Criterion criterion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(criterion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(criterion);
        }

        // GET: Criteria/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Criterion criterion = db.Criterion.Find(id);
            if (criterion == null)
            {
                return HttpNotFound();
            }
            return View(criterion);
        }

        // POST: Criteria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Criterion criterion = db.Criterion.Find(id);
            db.Criterion.Remove(criterion);
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
