using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeaTICs_v2.Models;
using NeaTICs_v2;
using NeaTICs_v2.DAL;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Authorize]
    public class ContactController : Controller
    {
        //IContactMessageRepository _repository;
        private UnitOfWork unitOfWork = new UnitOfWork();

        //public ContactController()
        //{
        //    _repository = new ContactMessageRepository();
        //}

        //public ContactController(IContactMessageRepository repository)
        //{
        //    _repository = repository;
        //}

        //
        // GET: /Admin/Contact/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            //ContactMessage message = _repository.SearchById(id);
            ContactMessage message = unitOfWork.ContactMessageRepository.GetByID(id);
            message.IsRead = true;
            //_repository.Edit(message);
            unitOfWork.ContactMessageRepository.Update(message);
            return View(message);
        }

        protected override void Dispose(bool disposing)
        {
            //Desconectar la base de datos
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
