using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeaTICs_v2.Models;
using NeaTICs_v2.DAL;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    public class ContactMessageController : ApiController
    {
        //private IContactMessageRepository _repository;
        private UnitOfWork unitOfWork = new UnitOfWork();

        //public ContactMessageController()
        //{
        //    _repository = new ContactMessageRepository();
        //}

        //public ContactMessageController(IContactMessageRepository repositoryQuestion)
        //{
        //    _repository = repositoryQuestion;
        //}

        //GET api/ContactMessage
        //Listar todos los mensajes
        public IEnumerable<ContactMessage> Get()
        {
            IList<ContactMessage> ContactMessages = unitOfWork.ContactMessageRepository.All().ToList();
            //Ordenado por fecha (El último mensaje mandado aparece al principio de la lista)
            return ContactMessages.AsEnumerable().OrderByDescending(x=>x.Date);
        }

        //GET api/ContactMessage/5
        //Devolver detalles de un mensaje por ID
        public ContactMessage Get(int id)
        {
            ContactMessage message = unitOfWork.ContactMessageRepository.GetByID(id);
            return message;
        }

        protected override void Dispose(bool disposing)
        {
            //Desconectar la base de datos
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
