using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NeaTICs_v2.Models;
using NeaTICs_v2.DAL;
using System.Drawing;
using System.Configuration;
using NeaTICs_v2.Helpers;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Authorize]
    public class EventController : Controller
    {
        //private Context db = new Context();
        private UnitOfWork unitOfWork = new UnitOfWork();

        //
        // GET: /Admin/Event/

        //Listar todas los eventos
        public ActionResult Index()
        {
            //Método para traer todos los eventos de la BD
            var events = unitOfWork.EventsRepository.All();
            return View(events.ToList());
        }

        //
        // GET: /Admin/Event/Details/5

        //Cargar la vista detallada de un evento. Le paso el ID como parametro
        public ActionResult Details(int id = 0)
        {
            //Método para traer un evento de la BD por su ID
            Events @event = unitOfWork.EventsRepository.GetByID(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //
        // GET: /Admin/Event/Create

        //Cargado del formulario para crear un nuevo evento
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Event/Create
        //POST del formulario de creación de nuevo evento. Se crea un nuevo evento con los datos que se pasan del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Events @event)
        {
            //var contentType = @event.ImageToUpload.ContentType;
            //Acá convierto el archivo que suben en un arreglo de bytes para ser guardado en la BD
            //var ImageData = new byte[@event.ImageToUpload.ContentLength];
            //@event.ImageToUpload.InputStream.Read(ImageData, 0, @event.ImageToUpload.ContentLength);
            //@event.Image = ImageData;
            if (@event.ImageToUpload == null)
            {
                ModelState.AddModelError("", "The image for the event is required. Please, select an apropiate image");
            }
            else
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //The ImageHelper try to save the image if the image meet all the requirements and if it do then return the name
                        string name = ImageHelper.TryImage(@event.ImageToUpload, "Events");
                        //Guardo en la noticia la url de la imagen
                        @event.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Eventos/" + name;
                        //Seteo la fecha de actualización con la fecha actual del sistema
                        @event.UpdateAt = DateTime.Now;
                        //Seteo nuevamente la hora de inicio y fin del evento pero con la fecha de creación
                        @event.StartAt = @event.CreateAt.Date + @event.StartAt.TimeOfDay;
                        @event.EndAt = @event.CreateAt.Date + @event.EndAt.TimeOfDay;
                        //Inserto nuevo evento en la BD
                        unitOfWork.EventsRepository.Insert(@event);
                        //Guardo
                        unitOfWork.Save();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new Exception("You must complete all the required fields");
                    }
                }
                catch (Exception e)
                {
                    //Log the error if is needed
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View(@event);
        }

        //
        // GET: /Admin/Event/Edit/5
        //Cargado de la página detallada de eventos pero con los campos para editar. Se le pasa el ID del evento como parametro.
        public ActionResult Edit(int id = 0)
        {
            Events @event = unitOfWork.EventsRepository.GetByID(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //
        // POST: /Admin/Event/Edit/5
        //POST del formulario de edición del evento. Recibe por parámetro todos los campos.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Events @event)
        {
            //var ImageData = new byte[@event.ImageToUpload.ContentLength];
            //@event.ImageToUpload.InputStream.Read(ImageData, 0, @event.ImageToUpload.ContentLength);
            //@event.Imagen = ImageData;
            //Recupero el evento a ser editado desde la BD por ID
            Events TempEvent = unitOfWork.EventsRepository.GetByID(@event.ID);
            //Paso todos los atributos a editar al evento que recuperé de la BD
            //Hago esto porque sino vuelve a guardar la imagen pero como nulo
            TempEvent.Title = @event.Title;
            TempEvent.Content = @event.Content;
            TempEvent.Place = @event.Place;
            TempEvent.CreateAt = @event.CreateAt;
            TempEvent.UpdateAt = DateTime.Now;
            TempEvent.StartAt = @event.StartAt;
            TempEvent.EndAt = @event.EndAt;
            TempEvent.Url = @event.Url;
            try
            {
                if (ModelState.IsValid)
                {
                    //Actualizo el evento
                    unitOfWork.EventsRepository.Update(TempEvent);
                    //Guardado
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(TempEvent);
        }

        //
        // GET: /Admin/Event/Delete/5
        //Cargado de la vista detallada de un evento para ser borrado. Se le pasa por parámetro el ID
        public ActionResult Delete(int id = 0)
        {
            Events @event = unitOfWork.EventsRepository.GetByID(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        //
        // POST: /Admin/Event/Delete/5
        //POST para borrar el evento. Recibe el ID como parámetro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Busco el evento a borrar por ID en la BD
                Events @event = unitOfWork.EventsRepository.GetByID(id);
                //Lo borro
                unitOfWork.EventsRepository.Delete(@event);
                //Guardo
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(id);
        }

        protected override void Dispose(bool disposing)
        {
            //Desconectar la base de datos
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}