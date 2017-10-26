using System;
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
            var criterions = db.Criterion.ToList();
            ViewBag.Criterions = db.Criterion.ToList();
            ViewBag.CriterionNames = new SelectList(db.Criterion.ToList(),"IdCrit", "CName");
            ViewBag.MarksByCriterions = new Dictionary<int, List<Mark>>();
            foreach (var s in criterions)
            {
                bool isQuantitative = (s.CType == "Количественный");
                if (s.OptimType == "Max")
                {
                    ViewBag.MarksByCriterions[s.IdCrit] = new List<Mark>(db.Mark.Where(m => m.Criterion.IdCrit == s.IdCrit).OrderBy(x => isQuantitative ? x.NumMark : x.MRange).ToList());
                }
                else if (s.OptimType == "Min")
                {
                    ViewBag.MarksByCriterions[s.IdCrit] = new List<Mark>(db.Mark.Where(m => m.Criterion.IdCrit == s.IdCrit).OrderByDescending(x => isQuantitative ? x.NumMark : x.MRange).ToList());
                }
            }
            return View(db.Alternative.ToList());
        }

        public ActionResult GetByCriterion(int idCrit)
        {
            Dictionary<int, int> limitations = new Dictionary<int, int>();
            foreach(Criterion crit in db.Criterion)
            {
                limitations.Add(crit.IdCrit, Convert.ToInt32(Request.Params["Evaluated" + crit.IdCrit]));
            }

            List<Vector> allVectors = db.Vector.ToList();
            Dictionary<int, List<Vector>> vectorAlternativesDictionary = new Dictionary<int, List<Vector>>();
            foreach (Alternative alternative in db.Alternative)
            {
                List<Vector> vectorsByAlt = allVectors.Where(v => v.IdAlt == alternative.IdAlt).ToList();
                vectorAlternativesDictionary.Add(alternative.IdAlt, vectorsByAlt);
            }

            Criterion mainCriterion = db.Criterion.FirstOrDefault(x => x.IdCrit == idCrit);

            List<int> idsAltToDelete = new List<int>();
            foreach (int idAlt in vectorAlternativesDictionary.Keys)
            {
                foreach (Vector vector in vectorAlternativesDictionary[idAlt])
                {
                    if (vector.Mark.IdCrit == mainCriterion.IdCrit)
                    {
                        continue;
                    }
                    int idLimitMark = limitations[vector.Mark.IdCrit];
                    if (vector.Mark.Criterion.OptimType == "Min")
                    {
                        if (vector.Mark.Criterion.CType == "Количественный")
                        {
                            if (vector.Mark.NumMark > db.Mark.FirstOrDefault(m => m.IdMark == idLimitMark)?.NumMark)
                            {
                                idsAltToDelete.Add(idAlt);
                                break;
                            }
                        }
                        else if (vector.Mark.Criterion.CType == "Качественный")
                        {
                            if (vector.Mark.MRange > db.Mark.FirstOrDefault(m => m.IdMark == idLimitMark)?.MRange)
                            {
                                idsAltToDelete.Add(idAlt);
                                break;
                            }
                        }
                    }
                    else if (vector.Mark.Criterion.OptimType == "Max")
                    {
                        if (vector.Mark.Criterion.CType == "Количественный")
                        {
                            if (vector.Mark.NumMark < db.Mark.FirstOrDefault(m => m.IdMark == idLimitMark)?.NumMark)
                            {
                                idsAltToDelete.Add(idAlt);
                                break;
                            }
                        }
                        else if (vector.Mark.Criterion.CType == "Качественный")
                        {
                            if (vector.Mark.MRange < db.Mark.FirstOrDefault(m => m.IdMark == idLimitMark)?.MRange)
                            {
                                idsAltToDelete.Add(idAlt);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (int idAlt in idsAltToDelete)
            {
                vectorAlternativesDictionary.Remove(idAlt);
            }

            Dictionary<int, Mark> sortOrder = new Dictionary<int, Mark>();

            foreach (int idAlt in vectorAlternativesDictionary.Keys)
            {
                sortOrder.Add(idAlt, vectorAlternativesDictionary[idAlt].FirstOrDefault(x => x.Mark.IdCrit == mainCriterion.IdCrit).Mark);
            }

            List<int> resultAlt = new List<int>();
            Mark maxMark = sortOrder.FirstOrDefault().Value;
            foreach (var markValue in sortOrder.Values)
            {
                if (compareMarks(maxMark, markValue) == -1)
                {
                    maxMark = markValue;
                }
            }

            foreach (var pair in sortOrder)
            {
                if (compareMarks(maxMark, pair.Value) == 0)
                {
                    resultAlt.Add(pair.Key);
                }
            }

            List<Vector> vectorsForView = db.Vector.Where(v => resultAlt.Contains(v.IdAlt)).ToList();

            ViewBag.AlternativeNames = db.Alternative.Where(a => resultAlt.Contains(a.IdAlt)).Select(a => a.AName).ToList();
            return View(vectorsForView);
        }

        public ActionResult SelectingBest()
        {
            Dictionary<int, List<Vector>> vectors = new Dictionary<int, List<Vector>>();
            foreach (Alternative alt in db.Alternative)
            {
                List<Vector> vectorsForAlt = db.Vector.Where(v => v.IdAlt == alt.IdAlt).ToList();
                vectors.Add(alt.IdAlt, vectorsForAlt);
            }

            ViewBag.vectorsOfAlternatives = vectors;
            return View(db.Alternative.ToList());
        }

        private int compareMarks(Mark mark1, Mark mark2)
        {
            if (mark1.Criterion.OptimType == "Min")
            {
                if (mark1.Criterion.CType == "Количественный")
                {
                    if (mark1.NumMark < mark2.NumMark)
                    {
                        return 1;
                    }
                    if (mark1.NumMark > mark2.NumMark)
                    {
                        return -1;
                    }
                }
                else if (mark1.Criterion.CType == "Качественный")
                {
                    if (mark1.MRange < mark2.MRange)
                    {
                        return 1;
                    }
                    if (mark1.MRange > mark2.MRange)
                    {
                        return -1;
                    }
                }
            }
            if (mark1.Criterion.OptimType == "Max")
            {
                if (mark1.Criterion.CType == "Количественный")
                {
                    if (mark1.NumMark > mark2.NumMark)
                    {
                        return 1;
                    }
                    if (mark1.NumMark < mark2.NumMark)
                    {
                        return -1;
                    }
                }
                else if (mark1.Criterion.CType == "Качественный")
                {
                    if (mark1.MRange > mark2.MRange)
                    {
                        return 1;
                    }
                    if (mark1.MRange < mark2.MRange)
                    {
                        return -1;
                    }
                }
            }
            return 0;
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

