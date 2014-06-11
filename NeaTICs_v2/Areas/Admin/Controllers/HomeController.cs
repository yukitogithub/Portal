using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;
//using NeaTICs_v2.Filters;
using NeaTICs_v2.DAL;
using NeaTICs_v2.Models;
using System.Web.Security;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    //[InitializeSimpleMembership]
    //[Authorize(Roles = "Administrator")]
    
    [Authorize]
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Admin/Home/

        //[Authorize(Roles = "Administrator")]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            //if (!WebSecurity.IsAuthenticated)
            //{
                return View();
            //}
            //return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string data)
        {
            JObject o = JObject.Parse(data);
            string username = (string)o["Username"];
            string pass = (string)o["Password"];
            //if (!WebSecurity.Login((string)o["Usuario"], (string)o["Password"], persistCookie: false))
            //{
            //    Response.StatusCode = 500;
            //}
            //return View();

            try
            {
                if (ModelState.IsValid)
                {
                    Users user = unitOfWork.UsersRepository.Get(x => x.Username == username && x.Password == pass).FirstOrDefault();
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(user.Username, true);
                        Session["User"] = user;
                        //if (user.Profile.Name == "SuperAdministrador")
                        //{
                            //Roles.AddUsersToRole(new string[] { user.Username }, "Administrator");
                        return RedirectToAction("Index", "Home");
                        //}
                        //else
                        //    return View();
                    }
                    else
                        return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorTitle = "ERROR";
                ViewBag.ErrorMessage = e.Message;
            }

            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            return View();
            //return RedirectToAction("Index");
        }
    }
}
