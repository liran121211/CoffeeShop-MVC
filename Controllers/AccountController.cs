using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.ViewModel;
using CoffeeShop.Dal;
using System.Web.Security;

namespace CoffeeShop.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account
        public ActionResult Index()
        {
            // Check if user logged in.
            if (Session["Username"] != null)
            {
                // Fetch user data from SQL by (username) token.
                Users logged_in_user = RetrieveUser(Session["Username"].ToString());
                if (logged_in_user != null)
                    return View("Index", logged_in_user);
                else
                    // In case of exception from SQL Query.
                    ViewBag.Message = "Something went wrong....";
            }
            return View("~/Views/Home/Login.cshtml");
        }

        public ActionResult ModifyDetails()
        {
            if (Session["Username"] != null)
                return View(RetrieveUser(Session["Username"].ToString()));
            return View("~/Views/Home/Login.cshtml");
        }


        public ActionResult SubmitUpdateDetails(Users form_data)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    UsersDal dal = new UsersDal();
                    String session_username = RetrieveUser(Session["Username"].ToString()).Username;
                    Users db_user = dal.dalUsers.Where(m => m.Username == session_username).First();
                    db_user.Username = form_data.Username;
                    db_user.Email = form_data.Email;
                    db_user.Password = form_data.Password;
                    db_user.Age = form_data.Age;
                    dal.SaveChanges();
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("UpdateDetails", form_data);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Orders()
        {
            Users logged_in_user = null;
            // get current logged in user object
            if (Session["Username"] != null)
            {
                logged_in_user = RetrieveUser(Session["Username"].ToString());

                // initiallize Dal objects
                OrdersDal o_dal = new OrdersDal();
                ProductsDal p_dal = new ProductsDal();
                OrderProductsDal op_dal = new OrderProductsDal();

                // create list and nested list for orders
                OrdersViewModel vm_o = new OrdersViewModel();
                ProductsViewModel vm_p = new ProductsViewModel();
                OrderProductsViewModel vm_op = new OrderProductsViewModel();

                
                vm_op.vmNestedOrderProducts = new List<List<OrderProducts>>();
                vm_p.vmProducts = new List<Products>();
                List<string> product_description = new List<string>();

                //get all orders of the logged in user
                vm_o.vmOrders = (from val in o_dal.dalOrders where val.UserId == logged_in_user.Id select val).ToList<Orders>();

                // loop through all user orders rows and add only order_product rows that match order id
                foreach (Orders order in vm_o.vmOrders)
                {
                    vm_op.vmOrderProducts = (from val in op_dal.dalOrderProducts where val.OrderId == order.Id select val).ToList<OrderProducts>();
                    if (vm_op.vmOrderProducts.Count() > 0)
                    {
                        vm_op.vmNestedOrderProducts.Add(vm_op.vmOrderProducts);
                        foreach(OrderProducts order_products in vm_op.vmOrderProducts)
                        {
                            vm_p.vmProducts.Add((from val in p_dal.dalProducts where val.Id == order_products.ProductId select val).First());
                        }
                    }
                }
                return View("Orders", Tuple.Create(vm_o.vmOrders, vm_op.vmNestedOrderProducts, vm_p.vmProducts));
            }
            return RedirectToAction("Login", "Home");
        }

        public Users RetrieveUser(String username)
        {
            try
            {
                UsersDal dal = new UsersDal();
                List<Users> session_user = (from val in dal.dalUsers where val.Username.Equals(username) select val).ToList<Users>();
                return session_user[0];
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

//CTRL+K+D
