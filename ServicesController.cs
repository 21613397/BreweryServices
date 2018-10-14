using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ServicesForHire.Models;

using System.IO;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;

namespace ServicesForHire.Controllers
{
    
    public class ServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InventoryProducts
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string productCategory, string search, string supplier)
        {
            var categs = from g in db.services
                         select g.Categories.categoryName;

            var categlist = new List<string>();
            categlist.AddRange(categs.Distinct());
            ViewData["productCategory"] = new SelectList(categlist);


           


            var products = db.services.Include(/*i => i.Catalogues).Include(*/i => i.Categories).Include(i => i.productDescription);

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(s => s.name.Contains(search));
            }
            if (!string.IsNullOrEmpty(productCategory))
            {
                products = products.Where(x => x.Categories.categoryName == productCategory);
            }
            //if (!string.IsNullOrEmpty(supplier))
            //{
            //    products = products.Where(x => x.Suppliers.supplierName == supplier);
            //}
            return View(db.services.ToList());
        }
       
       


     

     
       

     
        // GET: InventoryProducts/Details/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service inventoryProduct = db.services.Find(id);
            if (inventoryProduct == null)
            {
                return HttpNotFound();
            }
            return View(inventoryProduct);
        }

        // GET: InventoryProducts/Create
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName");
         
            return View();
        }

        // POST: InventoryProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "serviceId,indoor,outdoor,productDescription,Image,name,categoryId,SupplierId,duration,fee")] Service inventoryProduct, HttpPostedFileBase upload)
        { 

            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        inventoryProduct.Image = reader.ReadBytes(upload.ContentLength);
                        if (inventoryProduct.Image == null)
                        {

                            return View(inventoryProduct);
                        }
                    }
                }
                ;
                db.services.Add(inventoryProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", inventoryProduct.categoryId);

            return View(inventoryProduct);
        }

        // GET: InventoryProducts/Edit/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service inventoryProduct = db.services.Find(id);
            if (inventoryProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", inventoryProduct.categoryId);
            
            return View(inventoryProduct);
        }

        // POST: InventoryProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "serviceId,indoor,outdoor,productDescription,Image,name,categoryId,SupplierId,duration,fee")] Service inventoryProduct, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        inventoryProduct.Image = reader.ReadBytes(upload.ContentLength);
                        if (inventoryProduct.Image == null)
                        {

                            return View(inventoryProduct);
                        }
                    }
                }

                db.Entry(inventoryProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", inventoryProduct.categoryId);

            
            return View(inventoryProduct);
        }

        // GET: InventoryProducts/Delete/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service inventoryProduct = db.services.Find(id);
            if (inventoryProduct == null)
            {
                return HttpNotFound();
            }
            return View(inventoryProduct);
        }

        // POST: InventoryProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service inventoryProduct = db.services.Find(id);
            db.services.Remove(inventoryProduct);
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

        public ActionResult GetPic(string id)
        {
            int myId = Convert.ToInt32(id);
            var fileToRetrieve = db.services.FirstOrDefault(a => a.serviceId == myId);
            var ContentType = "image/jpg";
            return File(fileToRetrieve.Image, ContentType);
        }
       
        // POST: InventoryProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
     
    }
}
