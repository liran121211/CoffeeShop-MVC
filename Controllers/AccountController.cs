using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.ViewModel;
using CoffeeShop.Dal;
using System.Web.Security;
using Newtonsoft.Json;

namespace CoffeeShop.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account
        public ActionResult Index()
        {
            // Check if user logged in.
            if (isLoggenIn())
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
            if (isLoggenIn())
                return View(RetrieveUser(Session["Username"].ToString()));
            return View("~/Views/Home/Login.cshtml");
        }

        public ActionResult SubmitUpdateDetails(Users form_data)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    UsersDal dal = new UsersDal();
                    String session_username = RetrieveUser(Session["Username"].ToString()).Username;
                    Users db_user = dal.dalUsers.Where(m => m.Username == session_username).First();
                    db_user.FirstName = form_data.FirstName;
                    db_user.LastName = form_data.LastName;
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
            if (isLoggenIn())
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
                        foreach (OrderProducts order_products in vm_op.vmOrderProducts)
                        {
                            vm_p.vmProducts.Add((from val in p_dal.dalProducts where val.Id == order_products.ProductId select val).First());
                        }
                    }
                }
                return View("Orders", Tuple.Create(vm_o.vmOrders, vm_op.vmNestedOrderProducts, vm_p.vmProducts));
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult BaristaManageOrders()
        {
            Users logged_in_user = null;
            // get current logged in user object
            if (isLoggenIn() && isBarista())
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
                vm_o.vmOrders = (from val in o_dal.dalOrders where val.Barista.CompareTo("Not Yet Assigned") == 0 select val).ToList<Orders>();
                foreach (Orders order in o_dal.dalOrders.Where(m => m.Barista.CompareTo(logged_in_user.FirstName + " " + logged_in_user.LastName) == 0).ToList())
                {
                    vm_o.vmOrders.Add(order);
                }

                // loop through all user orders rows and add only order_product rows that match order id
                foreach (Orders order in vm_o.vmOrders)
                {
                    vm_op.vmOrderProducts = (from val in op_dal.dalOrderProducts where val.OrderId == order.Id select val).ToList<OrderProducts>();
                    if (vm_op.vmOrderProducts.Count() > 0)
                    {
                        vm_op.vmNestedOrderProducts.Add(vm_op.vmOrderProducts);
                        foreach (OrderProducts order_products in vm_op.vmOrderProducts)
                        {
                            vm_p.vmProducts.Add((from val in p_dal.dalProducts where val.Id == order_products.ProductId select val).First());
                        }
                    }
                }
                return View("BaristaManageOrders", Tuple.Create(vm_o.vmOrders, vm_op.vmNestedOrderProducts, vm_p.vmProducts));
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult AssignOrder(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    order.Barista = RetrieveUser(Session["Username"].ToString()).FirstName + " " + RetrieveUser(Session["Username"].ToString()).LastName;
                    o_dal.SaveChanges();
                    TempData["Message"] = "Order assigned successfully!";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "There was error while assigning an order...";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult VerifyCustomerPayment(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    order.Status = "Payment_Verified";
                    o_dal.SaveChanges();
                    TempData["Message"] = "Order Updated successfully!";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "There was error while updating an order...";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult PrepareOrder(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    order.Status = "Processing";
                    o_dal.SaveChanges();
                    TempData["Message"] = "Order Updated successfully!";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "There was error while updating an order...";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult MakePayment(int id)
        {
            if (isLoggenIn())
            {
                OrdersDal o_dal = new OrdersDal();
                Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                TempData["OrderData"] = order;
                return RedirectToAction("OrderStep8", "Home", null);
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult FinishOrder(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    SeatsDal s_dal = new SeatsDal();
                    OrdersDal o_dal = new OrdersDal();
                    Orders finished_order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    List<(int, int)> released_seats = ParseStringSeats(finished_order.Seats);

                    for (int i = 0; i < released_seats.Count; i++)
                    {
                        int row_n = released_seats[i].Item1;
                        if (released_seats[i].Item2 == 1)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col1 = false;
                        if (released_seats[i].Item2 == 2)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col2 = false;
                        if (released_seats[i].Item2 == 3)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col3 = false;
                        if (released_seats[i].Item2 == 4)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col4 = false;
                        if (released_seats[i].Item2 == 5)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col5 = false;
                    }

                    finished_order.Status = "Finished";
                    s_dal.SaveChanges();
                    o_dal.SaveChanges();
                    TempData["Message"] = "Thanks For Being Our Customer :)";
                    return RedirectToAction("Orders", "Account", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "There was error while picking up your order...";
                    return RedirectToAction("Orders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult CancelOrder(int id=-999)
        {
            if (isLoggenIn())
            {
                if (TempData["PayPalOrderID"] != null)
                    id = (int)TempData["PayPalOrderID"];

                if (ModelState.IsValid)
                {
                    SeatsDal s_dal = new SeatsDal();
                    OrdersDal o_dal = new OrdersDal();
                    Orders canceled_order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    List<(int, int)> canceled_seats = ParseStringSeats(canceled_order.Seats);

                    for (int i = 0; i < canceled_seats.Count; i++)
                    {
                        int row_n = canceled_seats[i].Item1;
                        if (canceled_seats[i].Item2 == 1)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col1 = false;
                        if (canceled_seats[i].Item2 == 2)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col2 = false;
                        if (canceled_seats[i].Item2 == 3)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col3 = false;
                        if (canceled_seats[i].Item2 == 4)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col4 = false;
                        if (canceled_seats[i].Item2 == 5)
                            s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col5 = false;
                    }
                    canceled_order.Status = "Canceled";
                    s_dal.SaveChanges();
                    o_dal.SaveChanges();
                    return RedirectToAction("OrderFailed", "Home");
                }
                else
                {
                    ViewBag.Message = "There was error while cancelling your order...";
                    return RedirectToAction("Orders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SetOrderReady(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    order.Status = "Ready";
                    o_dal.SaveChanges();
                    TempData["Message"] = "Order Updated successfully!";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "There was error while updating an order...";
                    return RedirectToAction("BaristaManageOrders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult ModifyOrder(int id)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders modified_order = o_dal.dalOrders.Where(m => m.Id == id).First();
                    return View("ModifyOrder", modified_order);
                }
                else
                {
                    ViewBag.Message = "There was error while modifying your order...";
                    return RedirectToAction("Orders", "Account", ViewBag.Message);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult SubmitModifyOrder(Orders form_data)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal o_dal = new OrdersDal();
                    Orders db_order = o_dal.dalOrders.Where(m => m.Id == form_data.Id).First();
                    db_order.Barista = form_data.Barista;
                    db_order.Category = form_data.Category;
                    db_order.Date = form_data.Date;
                    db_order.Id = form_data.Id;
                    db_order.Location = form_data.Location;
                    db_order.Seats = form_data.Seats;
                    db_order.Status = form_data.Status;
                    db_order.TotalOrder = form_data.TotalOrder;
                    db_order.TransactionId = form_data.TransactionId;
                    db_order.UserId = form_data.UserId;
                    o_dal.SaveChanges();
                    return RedirectToAction("Orders", "Account");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ModifyOrder", form_data);
                }
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

        private List<(int, int)> ParseStringSeats(String seats)
        {
            string[] seats_blocks = seats.Split(new[] { "##" }, StringSplitOptions.None);
            List<(int Table, int Chair)> splited_seats = new List<(int Table, int Chair)>();
            for (int i = 0; i < seats_blocks.Length; i++)
            {
                if (seats_blocks[i].CompareTo("") != 0)
                {
                    splited_seats.Add((int.Parse(seats_blocks[i].Split('#')[0]), int.Parse(seats_blocks[i].Split('#')[1])));
                }
            }
            return splited_seats;
        }

        private Boolean isBarista()
        {
            if (isLoggenIn())
            {
                AccountController acc_ctlr = new AccountController();
                Users user = acc_ctlr.RetrieveUser(Session["Username"].ToString());
                if (user.Role.Trim().Equals("Barista"))
                    return true;
            }
            return false;
        }
        private Boolean isLoggenIn()
        {
            if (Session["Username"] != null)
                return true;
            return false;
        }
    }
}

//CTRL+K+D
