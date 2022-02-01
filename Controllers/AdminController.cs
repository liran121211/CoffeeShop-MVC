using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Controllers;
using CoffeeShop.Dal;
using CoffeeShop.ViewModel;

namespace CoffeeShop.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                Users user = this.RetrieveUser();
                return View("Index", user);
            }
            return RedirectToAction("Index", "Home");
        }



        public ActionResult ManageProducts()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                Users user = this.RetrieveUser();
                ProductsDal dal = new ProductsDal();
                ProductsViewModel p_vm = new ProductsViewModel();
                p_vm.vmProducts = dal.dalProducts.ToList();

                return View("ManageProducts", p_vm);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ModifyProduct(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                ProductsDal dal = new ProductsDal();
                Products modified_product = dal.dalProducts.Where(m => m.Id == id).First();
                return View("ModifyProduct", modified_product);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitProductModification(Products form_data)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    ProductsDal dal = new ProductsDal();
                    Products db_product = dal.dalProducts.Where(m => m.Id == form_data.Id).First();
                    db_product.Id = form_data.Id;
                    db_product.AgeLimited = form_data.AgeLimited;
                    db_product.Availability = form_data.Availability;
                    db_product.Image = form_data.Image;
                    db_product.Name = form_data.Name;
                    db_product.Price = form_data.Price;
                    db_product.DiscountedPrice = form_data.DiscountedPrice;
                    db_product.Rank = form_data.Rank;
                    db_product.Category = form_data.Category;
                    dal.SaveChanges();
                    return RedirectToAction("ManageProducts", "Admin");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ModifyProduct", form_data);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddProduct()
        {
            if (this.isLoggenIn() && this.isAdmin())
                return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitNewProduct(Products form_data)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    ProductsDal dal = new ProductsDal();
                    dal.dalProducts.Add(form_data);
                    dal.SaveChanges();
                    return RedirectToAction("ManageProducts", "Admin");
                }
                else
                    return View("AddProduct", form_data);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DeleteProduct(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                ProductsDal dal = new ProductsDal();
                Products db_product = dal.dalProducts.Where(m => m.Id == id).First();
                dal.dalProducts.Remove(db_product);
                dal.SaveChanges();
                return RedirectToAction("ManageProducts", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ManageOrders()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                // initiallize Dal objects
                OrdersDal o_dal = new OrdersDal();
                ProductsDal p_dal = new ProductsDal();
                OrderProductsDal op_dal = new OrderProductsDal();

                // create list and nested list for orders
                OrdersViewModel vm_o = new OrdersViewModel();
                ProductsViewModel vm_p = new ProductsViewModel();
                OrderProductsViewModel vm_op = new OrderProductsViewModel();

                vm_o.vmOrders = (from val in o_dal.dalOrders select val).ToList<Orders>();
                vm_p.vmProducts = (from val in p_dal.dalProducts select val).ToList<Products>();
                vm_op.vmNestedOrderProducts = new List<List<OrderProducts>>();
                foreach (Orders order in vm_o.vmOrders)
                {
                    vm_op.vmOrderProducts = (from val in op_dal.dalOrderProducts where val.OrderId == order.Id select val).ToList<OrderProducts>();
                    if (vm_op.vmOrderProducts.Count() > 0)
                        vm_op.vmNestedOrderProducts.Add(vm_op.vmOrderProducts);
                }
                return View("ManageOrders", Tuple.Create(vm_o, vm_op, vm_p));
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ModifyOrder(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                OrdersDal o_dal = new OrdersDal();
                UsersDal u_dal = new UsersDal();
                Orders modified_order = o_dal.dalOrders.Where(m => m.Id == id).First();
                List<Users> barista_users = u_dal.dalUsers.Where(m => m.Role.Trim().Equals("Barista")).ToList();
                List<SelectListItem> users_drop_list = new List<SelectListItem>();
                foreach (Users user in barista_users)
                {
                    users_drop_list.Add(new SelectListItem { Text = (user.FirstName).ToString() + " " + (user.LastName).ToString() });
                }
                AdminOrdersSectionViewModel vmAOS = new AdminOrdersSectionViewModel();
                vmAOS.O = modified_order;
                vmAOS.Sli = users_drop_list;
                return View("ModifyOrder", vmAOS);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitOrderModification(AdminOrdersSectionViewModel form_data)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    OrdersDal dal = new OrdersDal();
                    Orders db_order = dal.dalOrders.Where(m => m.Id == form_data.O.Id).First();
                    db_order.Status = form_data.O.Status;
                    db_order.Barista = form_data.O.Barista;
                    dal.SaveChanges();
                    return RedirectToAction("ManageOrders", "Admin");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ModifyProduct", form_data);
                }
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult ManageMenus()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                MenuDal m_dal = new MenuDal();
                MenuViewModel vm_m = new MenuViewModel();
                vm_m.vmMenus = (from val in m_dal.dalMenu select val).ToList<Menu>();
                return View("ManageMenus", vm_m);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ModifyMenu(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                MenuDal m_dal = new MenuDal();
                ProductsDal p_dal = new ProductsDal();
                MenuProductsDal mp_dal = new MenuProductsDal();
                AdminMenusSectionViewModel vm_ams = new AdminMenusSectionViewModel();

                Menu modified_menu = m_dal.dalMenu.Where(m => m.Id == id).First();
                List<Products> all_products = p_dal.dalProducts.ToList();
                List<MenuProducts> all_menus = mp_dal.dalMenuProducts.ToList();
                List<MenuProducts> current_menu = (from val in all_menus where val.Menu_Id == modified_menu.Id select val).ToList<MenuProducts>();

                List<Products> current_menu_products = new List<Products>();
                foreach (Products product in all_products)
                {
                    foreach (MenuProducts mp in current_menu)
                        if (product.Id == mp.Product_Id)
                            current_menu_products.Add(product);
                }

                vm_ams.All_products = all_products;
                vm_ams.Current_products = current_menu_products;
                vm_ams.Menu = modified_menu;

                return View("ModifyMenu", vm_ams);
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult DeleteMenu(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                MenuProductsDal mp_dal = new MenuProductsDal();
                List<MenuProducts> mp_rows = mp_dal.dalMenuProducts.Where(m => m.Menu_Id == id).ToList();
                foreach (MenuProducts mp in mp_rows)
                {
                    mp_dal.dalMenuProducts.Remove(mp);
                }
                mp_dal.SaveChanges();

                MenuDal m_dal = new MenuDal();
                Menu m_row = m_dal.dalMenu.Where(m => m.Id == id).First();
                m_dal.dalMenu.Remove(m_row);
                m_dal.SaveChanges();

                return RedirectToAction("ManageMenus", "Admin");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitMenuModification(AdminMenusSectionViewModel form_data, FormCollection form_collection)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    MenuDal m_dal = new MenuDal();
                    Menu modified_menu = m_dal.dalMenu.Where(m => m.Id == form_data.Menu.Id).First();
                    modified_menu.Category = form_data.Menu.Category;
                    modified_menu.Name = form_data.Menu.Name;
                    modified_menu.Time = form_data.Menu.Time;
                    modified_menu.Location = form_data.Menu.Location;

                    // if menu chosen to listed as main menu (Currently Listed) then make all other menus as not listed.
                    if (form_data.Menu.Listed == true)
                    {
                        foreach (Menu menu in m_dal.dalMenu.ToList())
                            menu.Listed = false;
                        modified_menu.Listed = true;
                    }
                    m_dal.SaveChanges();

                    ProductsDal p_dal = new ProductsDal();
                    List<Products> all_products = p_dal.dalProducts.ToList<Products>();

                    MenuProductsDal mp_dal = new MenuProductsDal();
                    List<MenuProducts> current_menu_products = mp_dal.dalMenuProducts.Where(m => m.Menu_Id == form_data.Menu.Id).ToList<MenuProducts>();


                    //remove all rows related to the chosen menu
                    for (int i = 0; i < current_menu_products.Count; i++)
                        mp_dal.dalMenuProducts.Remove(current_menu_products[i]);
                    mp_dal.SaveChanges();


                    //Extract products from the ModifyMenu Form
                    List<Products> submitted_products = new List<Products>();
                    for (int i = 0; i < form_collection.Count; i++)
                    {
                        Products temp_product = null;
                        try
                        {
                            if (form_collection[i.ToString()] != null)
                                if (form_collection[i.ToString()].Contains("true"))
                                    temp_product = p_dal.dalProducts.Where(m => m.Id == i).First();
                        }
                        catch (InvalidOperationException) { temp_product = null; }

                        if (temp_product != null)
                            submitted_products.Add(temp_product);
                    }

                    //Add new chosen products back to the database
                    for (int k = 0; k < submitted_products.Count; k++)
                    {
                        MenuProducts row = new MenuProducts();
                        row.Menu_Id = modified_menu.Id;
                        row.Product_Id = submitted_products[k].Id;
                        mp_dal.dalMenuProducts.Add(row);
                    }
                    mp_dal.SaveChanges();

                    return RedirectToAction("ManageMenus", "Admin");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ModifyMenu", form_data);
                }
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult AddMenu()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                ProductsDal p_dal = new ProductsDal();
                AdminMenusSectionViewModel vm_ams = new AdminMenusSectionViewModel();
                List<Products> all_products = p_dal.dalProducts.ToList();
                List<Products> current_products = new List<Products>();
                Menu menu = new Menu();
                vm_ams.All_products = all_products;
                vm_ams.Current_products = current_products;
                vm_ams.Menu = menu;

                return View("AddMenu", vm_ams);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitNewMenu(AdminMenusSectionViewModel form_data, FormCollection form_collection)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    MenuDal m_dal = new MenuDal();
                    Menu new_menu = new Menu();
                    new_menu.Category = form_data.Menu.Category;
                    new_menu.Name = form_data.Menu.Name;
                    new_menu.Time = form_data.Menu.Time;
                    new_menu.Location = form_data.Menu.Location;

                    // if menu chosen to listed as main menu (Currently Listed) then make all other menus as not listed.
                    if (form_data.Menu.Listed == true)
                    {
                        foreach (Menu menu in m_dal.dalMenu.ToList())
                            menu.Listed = false;
                        new_menu.Listed = true;
                    }
                    m_dal.dalMenu.Add(new_menu);
                    m_dal.SaveChanges();

                    ProductsDal p_dal = new ProductsDal();
                    List<Products> all_products = p_dal.dalProducts.ToList<Products>();

                    MenuProductsDal mp_dal = new MenuProductsDal();
                    List<MenuProducts> new_menu_products = new List<MenuProducts>();

                    //Extract products from the ModifyMenu Form
                    List<Products> submitted_products = new List<Products>();
                    for (int i = 0; i < form_collection.Count; i++)
                    {
                        Products temp_product = null;
                        //start i=5, skipping to the product checkboxes.
                        try
                        {
                            if (form_collection[i.ToString()] != null)
                                if (form_collection[i.ToString()].Contains("true"))
                                    temp_product = p_dal.dalProducts.Where(m => m.Id == i).First();
                        }
                        catch (InvalidOperationException) { temp_product = null; }

                        if (temp_product != null)
                            submitted_products.Add(temp_product);
                    }

                    //Add new chosen products back to the database
                    for (int k = 0; k < submitted_products.Count; k++)
                    {
                        MenuProducts row = new MenuProducts();
                        row.Menu_Id = new_menu.Id;
                        row.Product_Id = submitted_products[k].Id;
                        mp_dal.dalMenuProducts.Add(row);
                    }
                    mp_dal.SaveChanges();

                    return RedirectToAction("ManageMenus", "Admin");
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ModifyMenu", form_data);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ManageUsers()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                UsersDal u_dal = new UsersDal();
                UsersViewModel u_vm = new UsersViewModel();

                u_vm.vmUsers = (from val in u_dal.dalUsers select val).ToList<Users>();
                return View("ManageUsers", u_vm);
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult ModifyUser(int id)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                UsersDal u_dal = new UsersDal();
                Users user = u_dal.dalUsers.Where(m => m.Id == id).First();
                View("ModifyUser", user);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitModifyUser(Users form_data)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    UsersDal dal = new UsersDal();
                    Users db_user = dal.dalUsers.Where(m => m.Id == form_data.Id).First();
                    db_user.Username = form_data.Username;
                    db_user.Email = form_data.Email;
                    db_user.Password = form_data.Password;
                    db_user.Age = form_data.Age;
                    db_user.VIP = form_data.VIP;
                    db_user.VIPNumber = form_data.VIPNumber;
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

        public ActionResult ManageSeats()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                SeatsDal s_dal = new SeatsDal();
                SeatsViewModel s_vm = new SeatsViewModel();

                s_vm.vmSeats = (from val in s_dal.dalSeats select val).ToList<Seats>();
                return View("ManageSeats", s_vm);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SubmitSeats(SeatsViewModel s_vm)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    SeatsDal dal = new SeatsDal();
                    for (int i = 0; i < dal.dalSeats.Count(); i++)
                    {
                        dal.dalSeats.Where(m => m.Id == i + 13).First().Col1 = s_vm.vmSeats[i].Col1;
                        dal.dalSeats.Where(m => m.Id == i + 13).First().Col2 = s_vm.vmSeats[i].Col2;
                        dal.dalSeats.Where(m => m.Id == i + 13).First().Col3 = s_vm.vmSeats[i].Col3;
                        dal.dalSeats.Where(m => m.Id == i + 13).First().Col4 = s_vm.vmSeats[i].Col4;
                        dal.dalSeats.Where(m => m.Id == i + 13).First().Col5 = s_vm.vmSeats[i].Col5;
                    }
                    dal.SaveChanges();
                    TempData["Message"] = "Tables Modification Saved Successfully!";
                    return RedirectToAction("ManageSeats", "Admin", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ManageSeats", s_vm);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ManageTablesAvailability()
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                TablesAvailabilityDal ta_dal = new TablesAvailabilityDal();
                TablesAvailabilityViewModel ta_vm = new TablesAvailabilityViewModel();
                ta_vm.vmTablesAvailability = ta_dal.dalTablesAvailability.ToList();
                return View("ManageTablesAvailability", ta_vm);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SubmitTablesAvailabilityModification(TablesAvailabilityViewModel ta_vm)
        {
            if (this.isLoggenIn() && this.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    TablesAvailabilityDal ta_dal = new TablesAvailabilityDal();
                    for (int i = 0; i < ta_dal.dalTablesAvailability.Count(); i++)
                    {
                        int table_n = ta_vm.vmTablesAvailability[i].Id;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Sunday = ta_vm.vmTablesAvailability[i].Sunday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Monday = ta_vm.vmTablesAvailability[i].Monday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Tuesday = ta_vm.vmTablesAvailability[i].Tuesday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Wednesday = ta_vm.vmTablesAvailability[i].Wednesday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Thursday = ta_vm.vmTablesAvailability[i].Thursday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Friday = ta_vm.vmTablesAvailability[i].Friday;
                        ta_dal.dalTablesAvailability.Where(m => m.Id == table_n).First().Saturday = ta_vm.vmTablesAvailability[i].Saturday;
                    }
                    ta_dal.SaveChanges();
                    TempData["Message"] = "Tables Modification Saved Successfully!";
                    return RedirectToAction("ManageTablesAvailability", "Admin", ViewBag.Message);
                }
                else
                {
                    ViewBag.Message = "The information given is invalid!";
                    return View("ManageTablesAvailability", ta_vm);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private Boolean isAdmin()
        {
            if (Session["Username"] != null)
            {
                AccountController acc_ctlr = new AccountController();
                Users user = acc_ctlr.RetrieveUser(Session["Username"].ToString());
                if (user.Role.Trim().Equals("Admin"))
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

        private Users RetrieveUser()
        {
            AccountController acc_ctlr = new AccountController();
            Users user = acc_ctlr.RetrieveUser(Session["Username"].ToString());
            return user;
        }
    }
}
