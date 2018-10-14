using IdentitySample.Models;
using ServicesForHire.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServicesForHire.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private ApplicationDbContext db = new ApplicationDbContext();
        //readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: /Cart/
        public ActionResult Index()
        {
            return View("Cart");
        }
        private int isExisting(int id)
        {
            List<Item> cart = (List<Item>)Session["Cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].SubProduct.serviceId.Equals(id))
                    return i;
            return -1;
        }
        public ActionResult Buy(int id)
        {
            try
            {
                if (Session["Cart"] != null)
                {
                    List<Item> cart = (List<Item>)Session["Cart"];
                    int index = isExisting(id);
                    if (index != -1)
                    {
                        cart[index].Quantity++;
                    }
                    if (index == -1)
                    {
                        cart.Add(new Item
                        {
                            ItemId = (cart.Count + 1),
                            SubProduct = db.services.Find(id),
                            Quantity = 1
                        });
                    }
                }

                if (Session["Cart"] == null)
                {
                    List<Item> cart = new List<Item>();
                    cart.Add(new Item { ItemId = 1, SubProduct = db.services.Find(id), Quantity = 1 });
                    Session["Cart"] = cart;
                }
            }
            catch
            {
                return View("Cart");
            }
            return View("cart");
        }

        public ActionResult Delete(int id)
        {
            List<Item> cart = (List<Item>)Session["Cart"];
            int index = BisExisting(id);
            cart.RemoveAt(index);
            if (cart.Count == 0)
            {
                cart = null;
            }
            Session["Cart"] = cart;
            return View("Cart");
        }

        private int BisExisting(int id)
        {
            List<Item> cart = (List<Item>)Session["Cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].SubProduct.serviceId == id)
                    return i;
            return -1;
        }

        public ActionResult CartSummary()
        {
            return PartialView();
        }

        public ActionResult CheckOut()
        {

            return RedirectToAction("Login", "Account");

        }
        
    }
}