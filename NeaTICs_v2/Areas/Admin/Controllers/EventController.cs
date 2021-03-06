﻿using System;
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
            //TODO: Revisar tipo de imagen a subir
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
                //TODO: Acá se debería preguntar si el archivo tiene una extensión de imagen válida
                //To know the file extension
                string Extension = System.IO.Path.GetExtension(@event.ImageToUpload.FileName);

                if (Extension != ".jpg" && Extension != ".jpeg" && Extension != ".png" && Extension != ".gif")
                {
                    ModelState.AddModelError("", "The image extension is wrong. Please, select an apropiate image");
                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                //To know the name of the file
                string ImageName = System.IO.Path.GetFileName(@event.ImageToUpload.FileName);
                //To know the physical path

                string physicalPath = Server.MapPath("~/Images/Eventos/" + ImageName);
                //To know the size of the file
                int FileSize = @event.ImageToUpload.ContentLength;

                //Si la imagen supera un tamaño máximo permitido se devuelve error
                if (FileSize >= /*1048576 1mb*/ Convert.ToInt32(ConfigurationManager.AppSettings["ImageSize"]) /*5mb*/)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                    ModelState.AddModelError("", "Maximum File Size 5 MB");
                    //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                //Si la imagen tiene una resolución muy baja se devuelve un error
                using (Image image = Image.FromStream(@event.ImageToUpload.InputStream, true, true))
                {
                    //TODO: Ver resolución de tamaño adecuada!
                    if (image.Width < Convert.ToInt32(ConfigurationManager.AppSettings["ImageWidth"]) && image.Height < Convert.ToInt32(ConfigurationManager.AppSettings["ImageHeigth"]))
                    {
                        ModelState.AddModelError("", "Resolution is Too Low!");
                        //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                //Guardo en la noticia la url de la imagen
                @event.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Eventos/" + ImageName; ;
                try
                {
                    if (ModelState.IsValid)
                    {
                        //Guardo la imagen en la folder
                        @event.ImageToUpload.SaveAs(physicalPath);
                        //Inserto nuevo evento en la BD
                        unitOfWork.EventsRepository.Insert(@event);
                        //Guardo
                        unitOfWork.Save();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
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
            TempEvent.UpdateAt = @event.UpdateAt;
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