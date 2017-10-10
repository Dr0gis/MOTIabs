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
    public class ResultsController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: Results
        //public ActionResult Index()
        //{
        //    var result = db.Result.Include(r => r.Alternative).Include(r => r.LPR);
        //    return View(result.ToList());
        //}
        public ActionResult Index(bool? calc)
        {
            var result = db.Result.Include(r => r.Alternative).Include(r => r.LPR);

            if (calc == null || calc == false)
            {
                return View(result.ToList());
            }

            Dictionary<Criterion, int?> CritMarksMax = new Dictionary<Criterion, int?>();
            foreach (var item in db.Criterion)
            {
                if (item.CType == "Количественный")
                {
                    CritMarksMax.Add(item, db.Mark.Where(m => m.IdCrit == item.IdCrit).Max(m => m.NumMark));
                }
                else if (item.CType == "Качественный")
                {
                    CritMarksMax.Add(item, db.Mark.Where(m => m.IdCrit == item.IdCrit).Max(m => m.MRange));
                }
            }

            Dictionary<Criterion, List <Mark>> CritListAdaptMark = new Dictionary<Criterion, List<Mark>>();
            foreach (var item in db.Criterion)
            {
                if (item.OptimType == "Max")
                {
                    CritListAdaptMark.Add(item, db.Mark.Where(m => m.IdCrit == item.IdCrit).ToList());
                }
                else if (item.OptimType == "Min")
                {
                    List<Mark> tempListMarks = new List<Mark>();
                    List<Mark> listMarks = db.Mark.Where(m => m.IdCrit == item.IdCrit).ToList();
                    foreach (var mark in listMarks)
                    {
                        if (item.CType == "Количественный")
                        {
                            Mark tempMark = new Mark();
                            tempMark.IdMark = mark.IdMark;
                            tempMark.MName = mark.MName;
                            tempMark.NumMark = mark.NumMark;
                            tempMark.IdCrit = mark.IdCrit;
                            tempMark.Criterion = mark.Criterion;
                            tempMark.Vector = mark.Vector;

                            tempMark.NumMark = CritMarksMax[item] - mark.NumMark;

                            tempListMarks.Add(tempMark);
                        }
                        else if (item.CType == "Качественный")
                        {
                            Mark tempMark = new Mark();
                            tempMark.IdMark = mark.IdMark;
                            tempMark.MName = mark.MName;
                            tempMark.MRange = mark.MRange;
                            tempMark.IdCrit = mark.IdCrit;
                            tempMark.Criterion = mark.Criterion;
                            tempMark.Vector = mark.Vector;

                            tempMark.MRange = CritMarksMax[item] - mark.MRange;

                            tempListMarks.Add(tempMark);
                        }   
                    }
                    CritListAdaptMark.Add(item, tempListMarks);
                }
            }


            Dictionary<Criterion, int?> SumCritMarks = new Dictionary<Criterion, int?>();

            foreach (var item in db.Criterion)
            {
                if (item.CType == "Количественный")
                {
                    SumCritMarks.Add(item, CritListAdaptMark[item].Sum(m => m.NumMark));
                }
                else if (item.CType == "Качественный")
                {
                    SumCritMarks.Add(item, CritListAdaptMark[item].Sum(m => m.MRange));
                }
            }

            Dictionary<Alternative, double?> resultAlt = new Dictionary<Alternative, double?>();

            foreach (var item in db.Alternative)
            {
                List<Mark> listMarks = item.Vector.Select(v => v.Mark).ToList();
                List<Mark> adaptListMark = new List<Mark>();
                foreach (var mark in listMarks)
                {
                    adaptListMark.Add(CritListAdaptMark[mark.Criterion].Find(x => x.IdMark == mark.IdMark));
                }
                List<double?> delSum = new List<double?>();
                foreach (var mark in adaptListMark)
                {
                    if (mark.Criterion.CType == "Количественный")
                    {
                        delSum.Add((double)mark.NumMark / (double)SumCritMarks[mark.Criterion]);
                    }
                    else if (mark.Criterion.CType == "Качественный")
                    {
                        delSum.Add((double)mark.MRange / (double)SumCritMarks[mark.Criterion]);
                    }    
                }
                double? sum = delSum.Sum();
                resultAlt.Add(item, sum);
            }

            var some = resultAlt.OrderByDescending(pair => pair.Value).ToList();
            for( int i = 0; i < some.Count; ++i)
            {
                Result res = new Result();
                res.IdAlt = some[i].Key.IdAlt;
                res.IdLPR = db.LPR.FirstOrDefault().IdLPR;
                res.Range = i + 1;
                db.Result.Add(res);
            }

            db.SaveChanges();

            result = db.Result.Include(r => r.Alternative).Include(r => r.LPR);

            return View(result.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Result.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName");
            ViewBag.IdLPR = new SelectList(db.LPR, "IdLPR", "LName");
            return View();
        }

        // POST: Results/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdRes,IdLPR,IdAlt,Range,AWeight")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Result.Add(result);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", result.IdAlt);
            ViewBag.IdLPR = new SelectList(db.LPR, "IdLPR", "LName", result.IdLPR);
            return View(result);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Result.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", result.IdAlt);
            ViewBag.IdLPR = new SelectList(db.LPR, "IdLPR", "LName", result.IdLPR);
            return View(result);
        }

        // POST: Results/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdRes,IdLPR,IdAlt,Range,AWeight")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", result.IdAlt);
            ViewBag.IdLPR = new SelectList(db.LPR, "IdLPR", "LName", result.IdLPR);
            return View(result);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Result.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Result result = db.Result.Find(id);
            db.Result.Remove(result);
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
