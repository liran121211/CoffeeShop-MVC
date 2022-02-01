using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Dal;
using CoffeeShop.ViewModel;
using Newtonsoft.Json;
using System.Timers;

namespace CoffeeShop.Controllers
{
    [RequireHttps]
    public class HomeController : System.Web.Mvc.Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View(new Users());
        }

        [HttpPost]
        public ActionResult SubmitRegistration(Users form_data)
        {
            if (ModelState.IsValid)
            {
                UsersDal dal = new UsersDal();
                form_data.Role = "Customer";
                dal.dalUsers.Add(form_data);
                dal.SaveChanges();
                return RedirectToAction("Login", "Home");
            }
            else
                return View("SubmitRegistration", form_data);
        }

        public ActionResult Login()
        {
            // if user logged in return to account area
            if (isLoggenIn())
                return RedirectToAction("Index", "Account");
            return View();
        }


        [HttpPost]
        public ActionResult SubmitLogin(Users form_data)
        {
            if (form_data.Username == null || form_data.Password == null)
            {
                ViewBag.Message = "Did you forget to enter Username or Password?";
                return View("Login");
            }

            UsersDal dal_obj = new UsersDal();
            UsersViewModel vm = new UsersViewModel();
            List<Users> users_list = dal_obj.dalUsers.ToList();
            vm.vmUsers = users_list;

            foreach (Users user in vm.vmUsers)
            {
                if (user.Username.Trim().Equals(form_data.Username.Trim()) && user.Password.Trim().Equals(form_data.Password.Trim()))
                {
                    Session["Username"] = user.Username.ToString().Trim();

                    if (user.Role.Trim().CompareTo("Admin") == 0)
                        Session["isAdmin"] = true;
                    else
                        Session["isAdmin"] = false;

                    if (user.Role.Trim().CompareTo("Barista") == 0)
                        Session["isBarista"] = true;
                    else
                        Session["isBarista"] = false;

                    return RedirectToAction("Index", "Account");
                }
            }
            ViewBag.Message = "Invalid Username or/and Password!";
            return View("Login");
        }

        public ActionResult VipLogin()
        {
            // if user logged in return to account area
            if (isLoggenIn())
                return RedirectToAction("Index", "Account");
            return View("VipLogin");
        }

        [HttpPost]
        public ActionResult SubmitVipLogin(Users form_data)
        {
            if (form_data.VIPNumber.ToString().CompareTo("") == 0)
            {
                ViewBag.Message = "Did you forget to enter your VIP Card Number?";
                return View("Login");
            }

            UsersDal dal_obj = new UsersDal();
            UsersViewModel vm = new UsersViewModel();
            List<Users> users_list = dal_obj.dalUsers.ToList();
            vm.vmUsers = users_list;

            foreach (Users user in vm.vmUsers)
            {
                if (user.VIPNumber == form_data.VIPNumber)
                {
                    Session["Username"] = user.Username.ToString().Trim();

                    if (user.Role.Trim().CompareTo("Admin") == 0)
                        Session["isAdmin"] = true;
                    else
                        Session["isAdmin"] = false;

                    if (user.Role.Trim().CompareTo("Barista") == 0)
                        Session["isBarista"] = true;
                    else
                        Session["isBarista"] = false;

                    return RedirectToAction("Index", "Account");
                }
            }
            ViewBag.Message = "Invalid VIP Card Number!";
            return View("Login");
        }

        public ActionResult OrderStep1() // Chose Location
        {
            if (isLoggenIn())
            {
                if (TempData["OrderExpired"] != null)
                    ViewBag.Message = TempData["OrderExpired"];
                if (TempData["OrderError"] != null)
                    ViewBag.Message = TempData["OrderError"];

                MakeOrderViewModel order_form = new MakeOrderViewModel();
                if (TempData["OrderData"] != null && TempData["OrderData"] is MakeOrderViewModel)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                order_form.CurrentDate = DateTime.Now;
                order_form.OrderTimeInterval = 8;
                UpdateOrderProcess("OrderStep1");
                return View("OrderStep1", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep1(MakeOrderViewModel order_form)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep1"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }
                return OrderStep2(order_form);
            }
            return View("Login");
        }

        public ActionResult OrderStep2(MakeOrderViewModel order_form) // Chose Category
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep2") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                UpdateOrderProcess("OrderStep2");
                return View("OrderStep2", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep2(MakeOrderViewModel order_form)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep2"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }
                return OrderStep3(order_form);
            }
            return View("Login");
        }

        public ActionResult OrderStep3(MakeOrderViewModel order_form) // Chose Price Range
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep3") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                UpdateOrderProcess("OrderStep3");
                return View("OrderStep3", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep3(MakeOrderViewModel order_form, FormCollection form_collection)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep3"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }

                if (form_collection["UNLIMITED_BUDGET"] != null)
                {
                    order_form.MaxPricePerProduct = 999999.0;
                }
                return OrderStep4(order_form);
            }
            return View("Login");
        }

        public ActionResult OrderStep4(MakeOrderViewModel order_form) // Chose Menu
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep4") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                // Filter menus accordint to collected order_form parameters.
                MenuDal m_dal = new MenuDal();
                MenuProductsDal mp_dal = new MenuProductsDal();
                ProductsDal p_dal = new ProductsDal();
                List<Menu> all_menus = m_dal.dalMenu.ToList();
                order_form.AvailableMenus = new List<Menu>();

                // Iterate over all available menus in DB.
                foreach (Menu menu in all_menus)
                {
                    bool price_not_exceeded = false;
                    List<MenuProducts> temp_mpl = (from val in mp_dal.dalMenuProducts where val.Menu_Id == menu.Id select val).ToList();

                    // Filter all menus that do not qualify the (order_form) parameters.
                    if (menu.Location.CompareTo(order_form.Location) == 0)
                    {
                        if (menu.Category.CompareTo(order_form.Category) == 0)
                        {
                            // Iterate over all products prices of the given menu and check if price is exceeded (order_form) parameters. 
                            foreach (MenuProducts mp in temp_mpl)
                            {
                                Products temp_product = (from val in p_dal.dalProducts where val.Id == mp.Product_Id select val).First();
                                if (temp_product.Price > order_form.MaxPricePerProduct)
                                    price_not_exceeded = true;
                            }
                            if (!price_not_exceeded)
                                order_form.AvailableMenus.Add(menu);
                        }
                        else if (order_form.Category.CompareTo("ALL_CATEGORIES") == 0)
                        {
                            // Iterate over all products prices of the given menu and check if price is exceeded (order_form) parameters. 
                            foreach (MenuProducts mp in temp_mpl)
                            {
                                Products temp_product = (from val in p_dal.dalProducts where val.Id == mp.Product_Id select val).First();
                                if (temp_product.Price > order_form.MaxPricePerProduct)
                                    price_not_exceeded = true;
                            }
                            if (!price_not_exceeded)
                                order_form.AvailableMenus.Add(menu);
                        }

                    }
                    else if (order_form.Location.CompareTo("ALL_SEATS") == 0)
                    {
                        if (menu.Category.CompareTo(order_form.Category) == 0)
                        {
                            // Iterate over all products prices of the given menu and check if price is exceeded (order_form) parameters. 
                            foreach (MenuProducts mp in temp_mpl)
                            {
                                Products temp_product = (from val in p_dal.dalProducts where val.Id == mp.Product_Id select val).First();
                                if (temp_product.Price > order_form.MaxPricePerProduct)
                                    price_not_exceeded = true;
                            }
                            if (!price_not_exceeded)
                                order_form.AvailableMenus.Add(menu);
                        }
                        else if (order_form.Category.CompareTo("ALL_CATEGORIES") == 0)
                        {
                            // Iterate over all products prices of the given menu and check if price is exceeded (order_form) parameters. 
                            foreach (MenuProducts mp in temp_mpl)
                            {
                                Products temp_product = (from val in p_dal.dalProducts where val.Id == mp.Product_Id select val).First();
                                if (temp_product.Price > order_form.MaxPricePerProduct)
                                    price_not_exceeded = true;
                            }
                            if (!price_not_exceeded)
                                order_form.AvailableMenus.Add(menu);
                        }
                    }
                }

                UpdateOrderProcess("OrderStep4");
                return View("OrderStep4", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep4(MakeOrderViewModel order_form, FormCollection form_collection)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep4"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }

                MenuDal m_dal = new MenuDal();
                ProductsDal p_dal = new ProductsDal();
                MenuProductsDal mp_dal = new MenuProductsDal();
                int menu_id = -1;

                // Gather order_form parameters sent from view.
                if (form_collection["SelectedMenu"] != null)
                    menu_id = int.Parse(form_collection["SelectedMenu"]);
                if (TempData["AvailableMenus"] != null)
                    order_form.AvailableMenus = (List<Menu>)TempData["AvailableMenus"];
                if (TempData["MaxPricePerProduct"] != null)
                    order_form.MaxPricePerProduct = (double)TempData["MaxPricePerProduct"];

                order_form.SelectedMenu = (from val in m_dal.dalMenu where val.Id == menu_id select val).First();
                List<MenuProducts> menu_products = (from val in mp_dal.dalMenuProducts where val.Menu_Id == order_form.SelectedMenu.Id select val).ToList();
                foreach (MenuProducts mp in menu_products)
                {
                    order_form.AvailableProducts.Add((from val in p_dal.dalProducts where val.Id == mp.Product_Id select val).First());
                }
                order_form.LoggedInUser = RetrieveUser();
                return OrderStep5(order_form, null);
            }
            return View("Login");
        }

        public ActionResult OrderStep5(MakeOrderViewModel order_form, FormCollection form_collection) // Chose Products From Menu.
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep5") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                if (TempData["SortMenu"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep5") == 0)
                {
                    order_form = (MakeOrderViewModel)TempData["SortMenu"];
                    order_form.AvailableProducts = SortProducts(order_form.AvailableProducts, form_collection["Sort"].ToString());
                }

                UpdateOrderProcess("OrderStep5");
                return View("OrderStep5", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep5(MakeOrderViewModel order_form, FormCollection form_collection)
        {
            int CheckBoxToProduct(String encoded_value)
            {
                return int.Parse(encoded_value.Split('#')[1]);
            }

            int DropDownListToProduct(String encoded_value)
            {
                if (encoded_value.CompareTo("") == 0)
                    return 1;
                return int.Parse(encoded_value.Replace('X'.ToString(), String.Empty));
            }

            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep5"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }

                // Gather order_form parameters sent from view.
                if (TempData["SelectedMenu"] != null)
                    order_form.SelectedMenu = (Menu)TempData["SelectedMenu"];
                if (TempData["AvailableMenus"] != null)
                    order_form.AvailableMenus = (List<Menu>)TempData["AvailableMenus"];
                if (TempData["MaxPricePerProduct"] != null)
                    order_form.MaxPricePerProduct = (double)TempData["MaxPricePerProduct"];
                if (TempData["AvailableProducts"] != null)
                    order_form.AvailableProducts = (List<Products>)TempData["AvailableProducts"];
                if (TempData["LoggedInUser"] != null)
                    order_form.LoggedInUser = (Users)TempData["LoggedInUser"];

                ProductsDal p_dal = new ProductsDal();

                for (int i = 0; i < form_collection.Count; i++)
                {
                    int product_id = -1, product_quantity = 1;
                    Products temp_product = null;
                    if (form_collection.GetKey(i).Contains("CHECKBOX"))
                    {
                        product_id = CheckBoxToProduct(form_collection.GetKey(i));
                        product_quantity = DropDownListToProduct(form_collection["DDL#" + product_id.ToString()]);
                        temp_product = p_dal.dalProducts.Where(m => m.Id == product_id).First();

                        if (temp_product != null)
                            order_form.SelectedProducts.Add((temp_product, product_quantity));
                    }
                }
                order_form.TotalOrder = 0;
                order_form.TotalOrder = order_form.SelectedProducts.Sum(m => m.Item1.DiscountedPrice * m.Item2);

                return OrderStep6(order_form);
            }
            return View("Login");
        }

        public ActionResult OrderStep6(MakeOrderViewModel order_form) // Chose Seats
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep6") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                SeatsDal s_dal = new SeatsDal();
                TablesAvailabilityDal ta_dal = new TablesAvailabilityDal();
                TablesAvailabilityViewModel ta = new TablesAvailabilityViewModel();

                ta.vmTablesAvailability = ta_dal.dalTablesAvailability.ToList();
                order_form.AvailableSeats = s_dal.dalSeats.ToList();
                order_form.TablesAvailability = ta;

                UpdateOrderProcess("OrderStep6");
                return View("OrderStep6", order_form);
            }
            return View("Login");
        }



        public ActionResult SubmitOrderStep6(MakeOrderViewModel order_form)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep6"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }

                // Gather order_form parameters sent from view.
                if (TempData["SelectedMenu"] != null)
                    order_form.SelectedMenu = (Menu)TempData["SelectedMenu"];
                if (TempData["AvailableMenus"] != null)
                    order_form.AvailableMenus = (List<Menu>)TempData["AvailableMenus"];
                if (TempData["MaxPricePerProduct"] != null)
                    order_form.MaxPricePerProduct = (double)TempData["MaxPricePerProduct"];
                if (TempData["AvailableProducts"] != null)
                    order_form.AvailableProducts = (List<Products>)TempData["AvailableProducts"];
                if (TempData["LoggedInUser"] != null)
                    order_form.LoggedInUser = (Users)TempData["LoggedInUser"];
                if (TempData["SelectedProducts"] != null)
                    order_form.SelectedProducts = (List<(Products, int)>)TempData["SelectedProducts"];

                SeatsDal s_dal = new SeatsDal();
                List<(int, int)> selected_seats = new List<(int, int)>();
                for (int i = 0; i < order_form.AvailableSeats.Count; i++)
                {
                    if (order_form.AvailableSeats[i].Col1 == true)
                        selected_seats.Add((i, 1));
                    if (order_form.AvailableSeats[i].Col2 == true)
                        selected_seats.Add((i, 2));
                    if (order_form.AvailableSeats[i].Col3 == true)
                        selected_seats.Add((i, 3));
                    if (order_form.AvailableSeats[i].Col4 == true)
                        selected_seats.Add((i, 4));
                    if (order_form.AvailableSeats[i].Col5 == true)
                        selected_seats.Add((i, 5));

                    if (s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col1 == false)
                        s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col1 = order_form.AvailableSeats[i].Col1;
                    if (s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col2 == false)
                        s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col2 = order_form.AvailableSeats[i].Col2;
                    if (s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col3 == false)
                        s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col3 = order_form.AvailableSeats[i].Col3;
                    if (s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col4 == false)
                        s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col4 = order_form.AvailableSeats[i].Col4;
                    if (s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col5 == false)
                        s_dal.dalSeats.Where(m => m.Id == i + 13).First().Col5 = order_form.AvailableSeats[i].Col5;
                }
                s_dal.SaveChanges();
                order_form.SelectedSeats = selected_seats;

                order_form.OrderTimer = new Timer(order_form.OrderTimeInterval * 60000); // This will raise the event every one minute. 6 SEC
                order_form.OrderTimer.Enabled = true;
                order_form.OrderTimer.Elapsed += new ElapsedEventHandler(OrderTimer);

                void OrderTimer(object sender, ElapsedEventArgs e)
                {
                    order_form.OrderTimer.Stop();
                    ReverseSeats(order_form);
                }

                // VIP ADDONS SECTION
                int count_drinks = 0;
                int count_alcohol_drinks = 0;
                double special_discount = 0.0;

                if (order_form.LoggedInUser.VIP == true)
                {
                    List<(Products product, int quantity)> drinks = order_form.SelectedProducts.Where(m => m.Item1.Category.CompareTo("Drinks") == 0).ToList();
                    Products min_price_drink = null;
                    if (drinks.Count() >= 1)
                        min_price_drink = (from val in drinks where val.product.DiscountedPrice == drinks.Min(m => m.product.DiscountedPrice) select val).First().product;
                    foreach ((Products product, int quantity) p_q in order_form.SelectedProducts)// FREE 10TH CUP
                        if (p_q.product.Category.CompareTo("Drinks") == 0)
                            count_drinks += p_q.quantity;

                    if (count_drinks >= 10)
                        order_form.Promotions.Add((min_price_drink, 0, 1));


                    if (order_form.TotalOrder > 100)// 10% OFF ORDER ABOVE SPENDING 100 ILS OR MORE.
                    {
                        special_discount = order_form.TotalOrder * 0.10;
                        order_form.Promotions.Add((null, special_discount, 2));
                    }

                    List<(Products product, int quantity)> alcohol_drinks = order_form.SelectedProducts.Where(m => m.Item1.Category.CompareTo("Alcohol") == 0).ToList();
                    Products min_price_alcohol = null;
                    if (alcohol_drinks.Count() >= 1)
                        min_price_alcohol = (from val in alcohol_drinks where val.product.DiscountedPrice == alcohol_drinks.Min(m => m.product.DiscountedPrice) select val).First().product;
                    foreach ((Products product, int quantity) p_q in order_form.SelectedProducts)// FREE 3TH ALCOHOL DRINK IF 2 OR MORE BOUGTH.
                        if (p_q.product.Category.CompareTo("Alcohol") == 0)
                            count_alcohol_drinks += p_q.quantity;

                    if (count_alcohol_drinks >= 2)
                        order_form.Promotions.Add((min_price_alcohol, 0, 3));
                }


                foreach ((Products product, double discount, int prom_id) promotion in order_form.Promotions)// APPLY PROMOTIONS TO ORDER
                {
                    if (promotion.prom_id == 1)// APPLY 10TH FREE DRINK
                    {
                        Products prom_product = new Products(promotion.product);
                        prom_product.Price = 0;
                        prom_product.DiscountedPrice = 0;
                        order_form.SelectedProducts.Add((prom_product, 1));
                    }

                    if (promotion.prom_id == 2)// APPLY 10% OFF ENTIRE ORDER
                        order_form.TotalOrder = order_form.TotalOrder - promotion.discount;

                    if (promotion.prom_id == 3)// APPLY 10TH FREE DRINK
                    {
                        Products prom_product = new Products(promotion.product);
                        prom_product.Price = 0;
                        prom_product.DiscountedPrice = 0;
                        order_form.SelectedProducts.Add((prom_product, 1));
                    }
                }

                return OrderStep7(order_form);
            }
            return View("Login");
        }


        public ActionResult OrderStep7(MakeOrderViewModel order_form) // Confirm Order
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && Session["OrderSession"].ToString().CompareTo("OrderStep7") == 0)
                    order_form = (MakeOrderViewModel)TempData["OrderData"];

                UpdateOrderProcess("OrderStep7");
                return View("OrderStep7", order_form);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep7(MakeOrderViewModel order_form)
        {
            if (isLoggenIn())
            {
                if (!ValidateOrderProcess("OrderStep7"))
                {
                    TempData["OrderError"] = "An error occurred while processing the order, please try again...";
                    return RedirectToAction("OrderStep1", "Home");
                }

                // Gather order_form parameters sent from view.
                if (TempData["SelectedMenu"] != null)
                    order_form.SelectedMenu = (Menu)TempData["SelectedMenu"];
                if (TempData["AvailableMenus"] != null)
                    order_form.AvailableMenus = (List<Menu>)TempData["AvailableMenus"];
                if (TempData["MaxPricePerProduct"] != null)
                    order_form.MaxPricePerProduct = (double)TempData["MaxPricePerProduct"];
                if (TempData["AvailableProducts"] != null)
                    order_form.AvailableProducts = (List<Products>)TempData["AvailableProducts"];
                if (TempData["LoggedInUser"] != null)
                    order_form.LoggedInUser = (Users)TempData["LoggedInUser"];
                if (TempData["SelectedProducts"] != null)
                    order_form.SelectedProducts = (List<(Products, int)>)TempData["SelectedProducts"];
                if (TempData["SelectedSeats"] != null)
                    order_form.SelectedSeats = (List<(int, int)>)TempData["SelectedSeats"];
                if (TempData["TransactionIdentifier"] != null)
                    order_form.TransactionIdentifier = (String)TempData["TransactionIdentifier"];
                if (TempData["OrderTimer"] != null)
                    order_form.OrderTimer = (Timer)TempData["OrderTimer"];
                if (TempData["FinalOrderPrice"] != null)
                    order_form.TotalOrder = (double)TempData["FinalOrderPrice"];

                //Save all order data to db.
                OrdersDal o_dal = new OrdersDal();
                OrderProductsDal op_dal = new OrderProductsDal();
                Orders submitted_order = new Orders();
                String order_seats = "";

                foreach ((int, int) seat in order_form.SelectedSeats)
                    order_seats += seat.Item1.ToString() + "#" + seat.Item2.ToString() + "##";

                submitted_order.Barista = "Not Yet Assigned";
                submitted_order.Status = "Order_Placed";
                submitted_order.Date = order_form.CurrentDate;
                submitted_order.Location = order_form.Location;
                submitted_order.Category = order_form.Category;
                submitted_order.TotalOrder = order_form.TotalOrder;
                submitted_order.UserId = order_form.LoggedInUser.Id;
                submitted_order.Seats = order_seats;
                submitted_order.TransactionId = order_form.TransactionIdentifier;
                o_dal.dalOrders.Add(submitted_order);
                o_dal.SaveChanges();

                int order_id = o_dal.dalOrders.Attach(submitted_order).Id;
                foreach ((Products prod, int quantity) product in order_form.SelectedProducts)
                {
                    if (product.prod.Price == 0)
                    {
                        List<OrderProducts> _op_ignored = op_dal.dalOrderProducts.Where(m => m.OrderId == submitted_order.Id).ToList();
                        if (_op_ignored.Count > 0)
                        {
                            OrderProducts isExist = _op_ignored.Where(m => m.ProductId == product.prod.Id).First();
                            if (isExist != null)
                            {
                                isExist.Quantity++;
                                op_dal.SaveChanges();
                                continue;
                            }
                        }
                    }

                    OrderProducts op = new OrderProducts();
                    op.OrderId = order_id;
                    op.ProductId = product.prod.Id;
                    op.Quantity = product.quantity;
                    op_dal.dalOrderProducts.Add(op);
                    op_dal.SaveChanges();
                }

                order_form.OrderTimer.Stop();
                return OrderStep8(submitted_order);
            }
            return View("Login");
        }

        public ActionResult OrderStep8(Orders order)
        {
            if (isLoggenIn())
            {
                if (TempData["OrderData"] != null && TempData["OrderData"] is Orders)
                    order = (Orders)TempData["OrderData"];

                return View("OrderStep8", order);
            }
            return View("Login");
        }

        public ActionResult SubmitOrderStep8(FormCollection form_collection)
        {
            if (isLoggenIn())
            {
                Orders order = null;
                if (form_collection["SelectedPayment"] != null)
                {
                    if (TempData["OrderData"] != null && TempData["OrderData"] is Orders)
                    {
                        order = (Orders)TempData["OrderData"];
                        UpdateOrderProcess("OrderStep1");

                        if (int.Parse(form_collection["SelectedPayment"]) == 2)
                            return PayByCreditCard(order);

                        if (int.Parse(form_collection["SelectedPayment"]) == 1)
                        {
                            OrdersDal o_dal = new OrdersDal();
                            Orders cash_order = o_dal.dalOrders.Where(m => m.Id == order.Id).First();
                            cash_order.Status = "Manual_Payment";
                            o_dal.SaveChanges();
                            return OrderSuccess(cash_order);
                        }

                    }
                    else
                        return RedirectToAction("Orders", "Account");
                }
                else
                    return RedirectToAction("Orders", "Account");

                return View("OrderStep8", order);
            }
            return View("Login");
        }

        public ActionResult PayByCreditCard(Orders order)
        {
            if (isLoggenIn())
            {
                return View("PayByCreditCard", order);
            }
            return View("Login");
        }

        public ActionResult SubmitPayByCreditCard(FormCollection form_collection)
        {
            if (isLoggenIn())
            {
                OrdersDal o_dal = new OrdersDal();
                Orders order = null;

                if (TempData["OrderData"] != null && TempData["OrderData"] is Orders)
                    order = (Orders)TempData["OrderData"];
                else
                    return RedirectToAction("Orders", "Account");
                try
                {
                    order = o_dal.dalOrders.Where(m => m.Id == order.Id).First();
                }
                catch (InvalidOperationException e)
                {
                    ViewBag.Message = "There was an error processing your order, try again...";
                    return RedirectToAction("Orders", "Account");
                }

                String card_type = CreditCard.GetCardType(form_collection["CARD_NUMBER"].ToString()).ToString();
                if (CreditCard.IsCardNumberValid(form_collection["CARD_NUMBER"].ToString()))
                {
                    order.Status = "Payment_Verified";
                    o_dal.SaveChanges();
                    return OrderSuccess(order);
                }
                else
                {
                    ViewBag.Message = "Invalid Credit Card Details...";
                    return View("PayByCreditCard", order);
                }
            }
            return View("Login");
        }

        public ActionResult OrderSuccess(Orders order)
        {
            if (isLoggenIn())
            {
                TempData = null;
                UpdateOrderProcess("OrderSuccess");
                return View("OrderSuccess", order);
            }
            return View("Login");
        }

        [HttpGet]
        public ActionResult AuthorizePaypal(int id, string result)
        {
            if (isLoggenIn())
            {
                OrdersDal o_dal = new OrdersDal();
                Orders order = o_dal.dalOrders.Where(m => m.Id == id).First();
                if (result.CompareTo("success") == 0)
                {
                    order.Status = "Payment_Verified";
                    o_dal.SaveChanges();
                    Orders dummy_order = new Orders();
                    dummy_order.Id = id;
                    return OrderSuccess(dummy_order);
                }


                if (result.CompareTo("failed") == 0)
                {
                    order.Status = "Order_Placed";
                    o_dal.SaveChanges();
                    TempData["PayPalOrderID"] = id;
                    return RedirectToAction("CancelOrder", "Account", id);
                }
            }
            return View("Login");
        }

        public ActionResult OrderFailed(Orders order)
        {
            if (isLoggenIn())
            {
                TempData = null;
                UpdateOrderProcess("OrderFailed");
                return View("OrderFailed", order);
            }
            return View("Login");
        }

        public ActionResult OrderStepBack(MakeOrderViewModel order_form)
        {
            if (isLoggenIn())
            {
                if (Session["OrderSession"].ToString().CompareTo("OrderStep1") == 0)
                {
                    UpdateOrderProcess("OrderStep1");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    return RedirectToAction("Index", "Home");
                }

                if (Session["OrderSession"].ToString().CompareTo("OrderStep2") == 0)
                {
                    UpdateOrderProcess("OrderStep1");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    return RedirectToAction("OrderStep1", "Home");
                }

                if (Session["OrderSession"].ToString().CompareTo("OrderStep3") == 0)
                {
                    UpdateOrderProcess("OrderStep2");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    return RedirectToAction("OrderStep2", "Home");
                }

                if (Session["OrderSession"].ToString().CompareTo("OrderStep4") == 0)
                {
                    UpdateOrderProcess("OrderStep3");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    return RedirectToAction("OrderStep3", "Home");
                }

                if (Session["OrderSession"].ToString().CompareTo("OrderStep5") == 0)
                {
                    UpdateOrderProcess("OrderStep4");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    TempData["SortMenu"] = null;
                    return RedirectToAction("OrderStep4", "Home");
                }

                if (Session["OrderSession"].ToString().CompareTo("OrderStep6") == 0)
                {
                    UpdateOrderProcess("OrderStep5");
                    order_form = (MakeOrderViewModel)TempData["OrderData"];
                    TempData["SortMenu"] = null;
                    return RedirectToAction("OrderStep5", "Home");
                }
                if (Session["OrderSession"].ToString().CompareTo("OrderStep7") == 0)
                {
                    UpdateOrderProcess("OrderStep6");
                    order_form = ReverseSeats((MakeOrderViewModel)TempData["OrderData"]);
                    return RedirectToAction("OrderStep6", "Home");
                }
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Seats()
        {
            if (isLoggenIn())
            {
                SeatsDal dal = new SeatsDal();
                SeatsViewModel s_vm = new SeatsViewModel();

                s_vm.vmSeats = (from val in dal.dalSeats select val).ToList<Seats>();
                return View("Seats", s_vm);
            }
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult SubmitSeats(SeatsViewModel s_vm)
        {
            if (isLoggenIn())
            {
                if (ModelState.IsValid)
                {
                    SeatsDal dal = new SeatsDal();
                    for (int i = 0; i < dal.dalSeats.Count(); i++)
                    {
                        if (dal.dalSeats.Where(m => m.Id == i + 13).First().Col1 == false)
                            dal.dalSeats.Where(m => m.Id == i + 13).First().Col1 = s_vm.vmSeats[i].Col1;
                        if (dal.dalSeats.Where(m => m.Id == i + 13).First().Col2 == false)
                            dal.dalSeats.Where(m => m.Id == i + 13).First().Col2 = s_vm.vmSeats[i].Col2;
                        if (dal.dalSeats.Where(m => m.Id == i + 13).First().Col3 == false)
                            dal.dalSeats.Where(m => m.Id == i + 13).First().Col3 = s_vm.vmSeats[i].Col3;
                        if (dal.dalSeats.Where(m => m.Id == i + 13).First().Col4 == false)
                            dal.dalSeats.Where(m => m.Id == i + 13).First().Col4 = s_vm.vmSeats[i].Col4;
                        if (dal.dalSeats.Where(m => m.Id == i + 13).First().Col5 == false)
                            dal.dalSeats.Where(m => m.Id == i + 13).First().Col5 = s_vm.vmSeats[i].Col5;
                    }
                    dal.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("Seats", s_vm);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        private MakeOrderViewModel ReverseSeats(MakeOrderViewModel order_form)
        {
            SeatsDal s_dal = new SeatsDal();
            for (int i = 0; i < order_form.SelectedSeats.Count; i++)
            {
                int row_n = order_form.SelectedSeats[i].Item1;

                if (order_form.SelectedSeats[i].Item2 == 1)
                    s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col1 = false;
                if (order_form.SelectedSeats[i].Item2 == 2)
                    s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col2 = false;
                if (order_form.SelectedSeats[i].Item2 == 3)
                    s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col3 = false;
                if (order_form.SelectedSeats[i].Item2 == 4)
                    s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col4 = false;
                if (order_form.SelectedSeats[i].Item2 == 5)
                    s_dal.dalSeats.Where(m => m.Id == row_n + 13).First().Col5 = false;
            }
            s_dal.SaveChanges();
            order_form.AvailableSeats = s_dal.dalSeats.ToList();
            order_form.SelectedSeats = new List<(int, int)>();

            return order_form;
        }

        private List<Products> SortProducts(List<Products> p_list, String Mode)
        {
            if (Mode.CompareTo("PriceAsc") == 0)
            {
                return p_list.OrderBy(m => m.Price).ToList();
            }
            if (Mode.CompareTo("PriceDesc") == 0)
            {
                return p_list.OrderByDescending(m => m.Price).ToList();
            }
            if (Mode.CompareTo("Popularity") == 0)
            {
                return p_list.OrderBy(m => m.Rank).ToList();
            }
            if (Mode.CompareTo("Discounted") == 0)
            {
                return p_list.Where(m => m.DiscountedPrice < m.Price).ToList();
            }
            return p_list;
        }

        private Boolean isLoggenIn()
        {
            if (Session["Username"] != null)
                return true;
            return false;
        }

        private Users RetrieveUser()
        {
            if (Session["Username"] != null)
            {
                AccountController acc_ctlr = new AccountController();
                Users user = acc_ctlr.RetrieveUser(Session["Username"].ToString());
                return user;
            }
            return null;
        }

        private Boolean ValidateOrderProcess(String current_state)
        {
            // If user refreshed the page during the order, return to OrderStep1.
            if (Session["OrderSession"].ToString().CompareTo(current_state) == 0)
                return true;
            return false;
        }

        private void UpdateOrderProcess(String current_state)
        {
            Session["OrderSession"] = current_state;
        }

    }
}
