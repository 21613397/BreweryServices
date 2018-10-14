using ServicesForHire.Models;

using ServicesForHire.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ServicesForHire.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        private ApplicationDbContext db = new ApplicationDbContext();
     
     
        
        
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartVIEWModel_
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }
        public ActionResult Payment_Successfull()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartVIEWModel_
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }


        public ActionResult AddToCart(int id)
        {

            // Retrieve the product from the database
            var addedProduct = db.services
                .Single(album => album.serviceId == id);



            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);


            cart.AddToCart(addedProduct);
            Session["cout1"] = cart.GetCount();

            //ProductAnalysis c = new ProductAnalysis();

            //c.save(addedProduct.brandName, cart.GetCount());

            // Go back to the main store page for more shopping
            return RedirectToAction("Index", "Main");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpGet]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int itemCount = cart.RemoveFromCart(id);
            ViewData["CartCount"] = cart.GetCount();
            Session["cout1"] = cart.GetCount();
            return RedirectToAction("Index");

        }


        [HttpGet]
        public ActionResult Minus_1(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int itemCount = cart.Minus_1(id);
            ViewData["CartCount"] = cart.GetCount();
            Session["cout1"] = cart.GetCount();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Plus_1(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            int itemCount = cart.Plus_1(id);
            ViewData["CartCount"] = cart.GetCount();
            Session["cout1"] = cart.GetCount();

            return RedirectToAction("Index");
        }


        //
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            ViewData["CartCount"] = cart.GetCount();
            Session["cout1"] = cart.GetCount();
            return PartialView("CartSummary");

        }



       [Authorize]
        public ActionResult CheckOut()
        {
            var get = ShoppingCart.GetCart(this.HttpContext);
            
            
          
           

            var currentUserId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var name = currentUser.UserName;
            var currentUserId1 = User.Identity.GetUserId();
            var manager1 = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser1 = manager.FindById(User.Identity.GetUserId());
            //addr = currentUser.Address;

            var cell = currentUser.PhoneNumber;
            var sname = currentUser.Email;

            Order neworder = new Order();

            //List<Driver> dr = db.Drivers.ToList();

            //foreach (var d in db.Drivers.ToList())
            //{
            //    if (d.AssignedLocation.ToLower() == (currentUser.City).ToLower())
            //    {
            //        neworder.AssignedDriver = d.FirstName + "&nbsp;" + d.LastName;
            //        neworder.OrderProgress = 3;
            //    }
            //    else return View("NoDelivery");

            //}




            List<Item> cart = (List<Item>)Session["ShoppingCart"];
            if (cart != null)
            {
                foreach (Item cad in cart)
                {
                    neworder.TotalOrderCost += Convert.ToDecimal(cad.Amount());
                }
            }

            neworder.OrderId = neworder.OrderId + 1;
            neworder.OrderNumber = "#" +/* name.Substring(0, 3) + */Convert.ToString(neworder.OrderId) + neworder.OrderDate.Day.ToString() + neworder.OrderDate.Minute.ToString();
            //neworder.DeliveryAddress = addr;
 
            neworder.OrderQuantity = get.GetCount();
            neworder.TotalOrderCost = get.GetTotal();
            neworder.username = name + " " + sname;
            neworder.Cell = cell;

            neworder.DeliveryAddress = TempData["Address"] as string;
           
            db.Orders.Add(neworder);
            neworder.OrderId = neworder.OrderId + 1;
            db.SaveChanges();
           

            TempData["Order"] = neworder;


           // TempData["Amount"] =

            Service st = new Service();

           


            db.SaveChanges();



            get.CreateOrder(neworder);
            //return View();
            return RedirectToAction("Payment");
        }

       

       
        
        public ActionResult Payment()
        {
            Order neworder = TempData["Order"] as Order;

            if (TempData["Address"] != null)
            {
                string yho = TempData["Address"] as string;




                neworder.DeliveryAddress = yho;
            }

            //ViewBag.Address = db.DeliveryAddresses.ToList().Find(x => x. == order.customerId);
            //ViewBag.Items = db.OrderItems.ToList().FindAll(x => x.OrderId == neworder.OrderId);


            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()


            };
            Session["cout1"] = cart.GetCount();

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Payment(Order oo)
        {
            return View(oo);
        }

        public ActionResult Delivery()
        {
            return View();
        }

        // POST: DeliveryAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delivery([Bind(Include = "DeliveryAddressId,Address,streetname,city,country,code")] DeliveryAddress deliveryAddress)
        {
            if (ModelState.IsValid)
            {
                if (deliveryAddress.streetname != null || deliveryAddress.city != null || deliveryAddress.country != null || deliveryAddress.code != null)
                {
                    deliveryAddress.Address = deliveryAddress.streetname + "\n" + deliveryAddress.city + " " + deliveryAddress.country + " " + deliveryAddress.code;

                }

                db.DeliveryAddresses.Add(deliveryAddress);
                db.SaveChanges();


                TempData["Address"] = deliveryAddress.Address;

                TempData["cty"] = deliveryAddress.city;
                TempData["srt"] = deliveryAddress.streetname;
                TempData["code"] = deliveryAddress.code;
                TempData["surb"] = deliveryAddress.country;






                return RedirectToAction("CheckOut", "ShoppingCart");
            }

            return View(deliveryAddress);
        }





        public ActionResult PayFast()
        {




            var get = ShoppingCart.GetCart(this.HttpContext);

            

            Order neworder = new Order();

            //List<Driver> dr = db.Drivers.ToList();

            //foreach (var d in db.Drivers.ToList())
            //{
            //    if (d.AssignedLocation.ToLower() == (currentUser.City).ToLower())
            //    {
            //        neworder.AssignedDriver = d.FirstName + "&nbsp;" + d.LastName;
            //        neworder.OrderProgress = 3;
            //    }
            //    else return View("NoDelivery");

            //}




            List<Item> cart = (List<Item>)Session["ShoppingCart"];
            if (cart != null)
            {
                foreach (Item cad in cart)
                {
                    neworder.TotalOrderCost += Convert.ToDecimal(cad.Amount());
                }
            }

            neworder.OrderId = neworder.OrderId + 1;
            neworder.OrderNumber = "#" +Convert.ToString(neworder.OrderId) + neworder.OrderDate.Day.ToString() + neworder.OrderDate.Minute.ToString();
           

            neworder.OrderQuantity = get.GetCount();
            neworder.TotalOrderCost = get.GetTotal();
           

            var id = User.Identity.GetUserId();
            // Create the order in your DB and get the ID

            //string amount = db.Orders.Where(m => m.username == id).Select(m => m.TotalOrderCost).DefaultIfEmpty().Sum().ToString();

            //string amount = TempData["Amount"] as string;
            var listfee2 = db.services.ToList().Select(x => x.fee).Distinct();

            var p2 = listfee2.ToList();
            var total = +p2.Sum();

            string amount = total.ToString();

            string orderId = new Random().Next(1, 9999).ToString();
            string name = "cleaner force #" + orderId;
            string description = "Syakoroba!";

            string site = "";
            string merchant_id = "";
            string merchant_key = "";

            // Check if we are using the test or live system
            string paymentMode = System.Configuration.ConfigurationManager.AppSettings["PaymentMode"];

            if (paymentMode == "test")
            {
                site = "https://sandbox.payfast.co.za/eng/process?";
                merchant_id = "10000100";
                merchant_key = "46f0cd694581a";
            }
            else if (paymentMode == "live")
            {
                site = "https://www.payfast.co.za/eng/process?";
                merchant_id = System.Configuration.ConfigurationManager.AppSettings["PF_MerchantID"];
                merchant_key = System.Configuration.ConfigurationManager.AppSettings["PF_MerchantKey"];
            }
            else
            {
                throw new InvalidOperationException("Cannot process payment if PaymentMode (in web.config) value is unknown.");
            }
            // Build the query string for payment site

            StringBuilder str = new StringBuilder();
            str.Append("merchant_id=" + HttpUtility.UrlEncode(merchant_id));
            str.Append("&merchant_key=" + HttpUtility.UrlEncode(merchant_key));
            str.Append("&return_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_ReturnURL"]));
            str.Append("&cancel_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_CancelURL"]));
            //str.Append("&notify_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_NotifyURL"]));

            str.Append("&m_payment_id=" + HttpUtility.UrlEncode(orderId));
            str.Append("&amount=" + HttpUtility.UrlEncode(amount.ToString()));
            str.Append("&item_name=" + HttpUtility.UrlEncode(name));
            str.Append("&item_description=" + HttpUtility.UrlEncode(description));

            // Redirect to PayFast
            Response.Redirect(site + str.ToString());

            return View("Payment_Successfull");
        }



        public ActionResult PayFasts()
        {


            var userId = User.Identity.GetUserId();

            var email = User.Identity.GetUserName();
            ShoppingCart s = new ShoppingCart();

            var cart = ShoppingCart.GetCart(this.HttpContext);
            Order o = new Order();


            decimal Total = cart.GetTotal();
            string OrderId = new Random().Next(1, 99999).ToString();
            string name = "TestingLab, Order#" + OrderId;
            string description = "TestingLab";


            string site = "";
            string merchant_id = "";
            string merchant_key = "";

            // Check if we are using the mmor live system
            string paymentMode = System.Configuration.ConfigurationManager.AppSettings["PaymentMode"];

            if (paymentMode == "test")
            {
                site = "https://sandbox.payfast.co.za/eng/process?";
                merchant_id = "10000100";
                merchant_key = "46f0cd694581a";
            }
            else if (paymentMode == "live")
            {
                site = "https://www.payfast.co.za/eng/process?";
                merchant_id = System.Configuration.ConfigurationManager.AppSettings["PF_MerchantID"];
                merchant_key = System.Configuration.ConfigurationManager.AppSettings["PF_MerchantKey"];
            }
            else
            {
                throw new InvalidOperationException("Cannot process payment if PaymentMode (in web.config) value is unknown.");
            }
            // Build the query string for payment site

            StringBuilder str = new StringBuilder();
            str.Append("merchant_id=" + HttpUtility.UrlEncode(merchant_id));
            str.Append("&merchant_key=" + HttpUtility.UrlEncode(merchant_key));
            str.Append("&return_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_ReturnURL"]));
            str.Append("&cancel_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_CancelURL"]));
            //str.Append("&notify_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_NotifyURL"]));

            str.Append("&m_payment_id=" + HttpUtility.UrlEncode(OrderId));
            str.Append("&amount=" + HttpUtility.UrlEncode(Total.ToString()));
            str.Append("&item_name=" + HttpUtility.UrlEncode(name));
            str.Append("&item_description=" + HttpUtility.UrlEncode(description));

            // Redirect to PayFast
            Response.Redirect(site + str.ToString());

            return View();
        }
    }
}



        //  public ActionResult Create(Service cartClass, int id)
        //{
        //    var user = User.Identity.GetUserId();
        //    InventoryProduct p = db.Products.Find(id);
        //    int count = db.Products.Count(m => m.productId == id);
        //    if (count > 0)
        //    {
        //        var find = db.Products.Where(m => m.productId == id);
        //        foreach (var item in find)
        //        {
        //            InventoryProduct cart = db.Products.Find(item.productId);
        //            cart.quantityOnHand += 1;
        //            cart.unitPrice += p.unitPrice;
        //            db.Entry(cart).State = EntityState.Modified;
        //        }
        //        p.quantityOnHand -= 1;
        //        if (p.quantityOnHand<p.minimumStock)
        //        {
        //            p.status = "Almost Gone!";
        //        }
        //        else if (p.quantityOnHand ==0)
        //        {
        //            p.status = "Out of Stock!";
        //        }
        //       else
        //        {
        //            p.status = "In Stock";
        //        }


        //        db.Entry(p).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        cartClass.productName = p.productName;
        //        cartClass.unitPrice = p.unitPrice;
        //        cartClass.quantityOnHand = 1;
        //        cartClass.productId = id;
        //        //cartClass.UserId = user;

        //        if (cartClass.quantityOnHand < cartClass.minimumStock)
        //        {
        //            cartClass.status = "Almost Gone!";
        //        }
        //        if (cartClass.quantityOnHand == 0)
        //        {
        //            cartClass.status = "Out of Stock!";
        //        }
        //        else
        //        {
        //            cartClass.status = "In Stock";
        //        }

        //        db.Products.Add(cartClass);
        //        p.quantityOnHand -= 1;

                

        //        db.Entry(p).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

            

        //}
      

