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
    public class LPRsController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: LPRs
        public ActionResult Index()
        {
            return View(db.LPR.ToList());
        }

        // GET: LPRs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LPR lPR = db.LPR.Find(id);
            if (lPR == null)
            {
                return HttpNotFound();
            }
            return View(lPR);
        }

        // GET: LPRs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LPRs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLPR,LName,LRange")] LPR lPR)
        {
            if (ModelState.IsValid)
            {
                db.LPR.Add(lPR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lPR);
        }

        // GET: LPRs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LPR lPR = db.LPR.Find(id);
            if (lPR == null)
            {
                return HttpNotFound();
            }
            return View(lPR);
        }

        // POST: LPRs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLPR,LName,LRange")] LPR lPR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lPR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lPR);
        }

        // GET: LPRs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LPR lPR = db.LPR.Find(id);
            if (lPR == null)
            {
                return HttpNotFound();
            }
            return View(lPR);
        }

        // POST: LPRs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LPR lPR = db.LPR.Find(id);
            db.LPR.Remove(lPR);
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
