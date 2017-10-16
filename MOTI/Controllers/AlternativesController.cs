using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MOTI.Controllers
{
    public class AlternativesController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: Alternatives
        public ActionResult Index()
        {
            ViewBag.CriterionNames = new SelectList(db.Criterion.ToList(),"IdCrit", "CName");
            return View(db.Alternative.ToList());
        }

        public ActionResult GetByCriterion(int idCrit, string optimizationType)
        {
            Mark markWithValue;
            List<Vector> vectorsForView = new List<Vector>();

            Criterion criterion = db.Criterion.FirstOrDefault(x => x.IdCrit == idCrit);
            
            if (criterion != null)
            {
                int criterionId = criterion.IdCrit;
                int markId = 1;
                if (optimizationType == "Min")
                {
                    if (criterion.CType == "Количественный")
                    {
                        markWithValue = db.Mark.FirstOrDefault(x =>
                            x.IdCrit == criterionId && x.NumMark == db.Mark.Where(mark => mark.IdCrit == criterionId).Min(mark => mark.NumMark));
                        if (markWithValue != null)
                        {
                            markId = markWithValue.IdMark;
                        }
                        vectorsForView = db.Vector.Where(vect => vect.Mark.IdMark == markId).ToList();
                    }
                    else if (criterion.CType == "Качественный")
                    {
                        markWithValue = db.Mark.FirstOrDefault(x =>
                            x.IdCrit == criterionId && x.MRange == db.Mark.Where(mark => mark.IdCrit == criterionId).Min(mark => mark.MRange));
                        if (markWithValue != null)
                        {
                            markId = markWithValue.IdMark;
                        }
                        vectorsForView = db.Vector.Where(vect => vect.Mark.IdMark == markId).ToList();
                    }
                }
                else
                {
                    if (criterion.CType == "Количественный")
                    {
                        markWithValue = db.Mark.FirstOrDefault(x =>
                            x.IdCrit == criterionId && x.NumMark == db.Mark.Where(mark => mark.IdCrit == criterionId).Max(mark => mark.NumMark));
                        if (markWithValue != null)
                        {
                            markId = markWithValue.IdMark;
                        }
                        vectorsForView = db.Vector.Where(vect => vect.Mark.IdMark == markId).ToList();
                    }
                    else if (criterion.CType == "Качественный")
                    {
                        markWithValue = db.Mark.FirstOrDefault(x =>
                            x.IdCrit == criterionId && x.MRange == db.Mark.Where(mark => mark.IdCrit == criterionId).Max(mark => mark.MRange));
                        if (markWithValue != null)
                        {
                            markId = markWithValue.IdMark;
                        }
                        vectorsForView = db.Vector.Where(vect => vect.Mark.IdMark == markId).ToList();
                    }
                }
            }
            ViewBag.AlternativeNames = vectorsForView.Select(x=> x.Alternative.AName);
            return View(vectorsForView);
        }

        // GET: Alternatives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alternative alternative = db.Alternative.Find(id);
            if (alternative == null)
            {
                return HttpNotFound();
            }
            return View(alternative);
        }

        // GET: Alternatives/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Alternatives/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAlt,AName")] Alternative alternative)
        {
            if (ModelState.IsValid)
            {
                db.Alternative.Add(alternative);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(alternative);
        }

        // GET: Alternatives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alternative alternative = db.Alternative.Find(id);
            if (alternative == null)
            {
                return HttpNotFound();
            }
            return View(alternative);
        }

        // POST: Alternatives/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAlt,AName")] Alternative alternative)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alternative).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(alternative);
        }

        // GET: Alternatives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alternative alternative = db.Alternative.Find(id);
            if (alternative == null)
            {
                return HttpNotFound();
            }
            return View(alternative);
        }

        // POST: Alternatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Alternative alternative = db.Alternative.Find(id);
            db.Alternative.Remove(alternative);
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
