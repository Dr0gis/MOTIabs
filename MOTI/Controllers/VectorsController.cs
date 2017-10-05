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
    public class VectorsController : Controller
    {
        private Database1Entities db = new Database1Entities();
        
        // GET: Vectors
        public ActionResult Index()
        {
            var vector = db.Vector.Include(v => v.Alternative).Include(v => v.Mark);
            List<string> alternatives = db.Alternative.Select(x => x.AName).ToList();
            ViewBag.Alternatives = alternatives;
            return View(vector.ToList());
        }

        // GET: Vectors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vector vector = db.Vector.Find(id);
            if (vector == null)
            {
                return HttpNotFound();
            }
            return View(vector);
        }

        // GET: Vectors/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName"); 

            List<String> critsName = db.Criterion.Select(x => x.CName).ToList();
            ViewBag.CritName = critsName;

            List<string> edIzmer = db.Criterion.Select(x => x.EdIzmer).ToList();
            ViewBag.EdIzmer = edIzmer;

            List<int> idCrits = db.Criterion.Select(x => x.IdCrit).ToList();           
            List<SelectList> MNamesLists = new List<SelectList>();
            foreach(int i in idCrits)
            {
                MNamesLists.Add(new SelectList(db.Mark.Where(mark => mark.Criterion.IdCrit == i), "IdMark", "MName"));
            }
            ViewBag.MNames = MNamesLists;

            return View();
        }

        // POST: Vectors/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vector vector)
        {

            int idAlt = Int32.Parse(Request.Params["IdAlt"]);
            List<int> mIds = new List<int>();
            List<int> idCrits = db.Criterion.Select(x => x.IdCrit).ToList();
            //List<Mark> marksWithSuchIdAlt = db.Vector.Where(v => v.IdAlt == idAlt).Select( v => v.Mark ).ToList();

            for (int i = 0; i < idCrits.Count; ++i)
            {
                string tempStr = Request.Params["IdMark" + i];
                int tempInt = Int32.Parse(tempStr);
                mIds.Add(tempInt);
            }

            foreach (int idMark in mIds)
            {
                Vector vectorTemp = new Vector();
                vectorTemp.IdAlt = idAlt;
                vectorTemp.IdMark = idMark;
                Mark some = db.Mark.Where(y => y.IdMark == idMark).First();
                var crits = db.Vector.Where(x => x.Mark.IdCrit == some.IdCrit && x.IdAlt == idAlt);
                List<Vector> listCrits = crits.ToList();
                if (listCrits.Count == 0)
                {
                    db.Vector.Add(vectorTemp);
                }
                else
                {
                    crits.FirstOrDefault().IdMark = idMark;
                    //db.Entry(crits.First())
                }
                db.SaveChanges();
            }

            #region comments
            //List<string> mNames = 

            //if (ModelState.IsValid)
            //{
            //    db.Vector.Add(vector);
            //    db.SaveChanges();
            //    
            //}

            //ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", vector.IdAlt);
            //ViewBag.IdMark = new SelectList(db.Mark, "IdMark", "MName", vector.IdMark);
            #endregion

            return RedirectToAction("Index");
        }

        // GET: Vectors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vector vector = db.Vector.Find(id);
            if (vector == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", vector.IdAlt);
            ViewBag.CName = db.Mark.Where(mark => mark.IdMark == vector.IdMark).Select(m => m.Criterion.CName).ToList()[0];
            ViewBag.IdMark = new SelectList(db.Mark.Where(mark => mark.IdCrit == vector.Mark.IdCrit), "IdMark", "MName", vector.IdMark);
            return View(vector);
        }

        // POST: Vectors/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdVect,IdAlt,IdMark")] Vector vector)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vector).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAlt = new SelectList(db.Alternative, "IdAlt", "AName", vector.IdAlt);
            ViewBag.IdMark = new SelectList(db.Mark, "IdMark", "MName", vector.IdMark);
            return View(vector);
        }

        // GET: Vectors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vector vector = db.Vector.Find(id);
            if (vector == null)
            {
                return HttpNotFound();
            }
            return View(vector);
        }

        // POST: Vectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vector vector = db.Vector.Find(id);
            db.Vector.Remove(vector);
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
