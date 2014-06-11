using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using NeaTICs_v2.Models;
using NeaTICs_v2.DAL;
using System.Drawing;
using System.Configuration;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    public class EventAPIController : ApiController
    {
        //Unidad de trabajo con todos los repositorios
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/EventAPI
        //Listar todas los eventos
        public IEnumerable<Events> GetEvents()
        {
            //Método para traer todos los eventos de la BD
            var events = unitOfWork.EventsRepository.All();
            return events.AsEnumerable();
        }

        // GET api/EventAPI/5
        //Cargar la vista detallada de un evento. Le paso el ID como parametro
        public Events GetEvents(int id)
        {
            //Método para traer un evento de la BD por su ID
            Events events = unitOfWork.EventsRepository.GetByID(id);
            return events;
        }

        // PUT api/EventAPI/5
        //Edición de un evento. Recibe por parámetro todos los campos
        public HttpResponseMessage PutEvents(int id, Events events)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != events.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //Recupero el evento a ser editado desde la BD por ID
            Events TempEvent = unitOfWork.EventsRepository.GetByID(events.ID);
            //Paso todos los atributos a editar al evento que recuperé de la BD
            //Hago esto porque sino vuelve a guardar la imagen pero como nulo
            TempEvent.Title = events.Title;
            TempEvent.Content = events.Content;
            TempEvent.Place = events.Place;
            TempEvent.CreateAt = events.CreateAt;
            TempEvent.UpdateAt = events.UpdateAt;
            TempEvent.StartAt = events.StartAt;
            TempEvent.EndAt = events.EndAt;
            TempEvent.Url = events.Url;
            TempEvent.Observations = events.Observations;
            
            //Actualizo el evento
            unitOfWork.EventsRepository.Update(TempEvent);

            try
            {
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/EventAPI
        //Creación de nuevo evento. Se crea un nuevo evento con los datos que se pasan del formulario
        public HttpResponseMessage PostEvents(Events events)
        {
            //For saving an image into the database
            //var ImageData = new byte[events.ImageToUpload.ContentLength];
            //events.ImageToUpload.InputStream.Read(ImageData, 0, events.ImageToUpload.ContentLength);
            //events.Image = ImageData;

            if (@events.ImageToUpload == null)
            {
                ModelState.AddModelError("", "The image for the new is required. Please, select an apropiate image");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                //For saving an image into a folder
                //To know the file extension
                string Extension = System.IO.Path.GetExtension(@events.ImageToUpload.FileName);

                if (Extension != ".jpg" && Extension != ".jpeg" && Extension != ".png" && Extension != ".gif")
                {
                    ModelState.AddModelError("", "The image extension is wrong. Please, select an apropiate image");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                //To know the name of the file
                string ImageName = System.IO.Path.GetFileName(@events.ImageToUpload.FileName);

                //To know the physical path
                string physicalPath = HttpContext.Current.Server.MapPath("~/Images/Eventos/" + ImageName);

                //To know the size of the file
                int FileSize = @events.ImageToUpload.ContentLength;

                //If the size is too big you get an error
                if (FileSize >= /*1048576 1mb*/ Convert.ToInt32(ConfigurationManager.AppSettings["ImageSize"]) /*5mb*/)
                {
                    //TODO: Poner el tamaño máximo del archivo permitido en el WebConfig
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                    ModelState.AddModelError("", "Maximum File Size 5 MB");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                //If the image is too small you get an error
                using (Image image = Image.FromStream(@events.ImageToUpload.InputStream, true, true))
                {
                    //TODO: Ver resolución de tamaño adecuada!
                    //TODO: Poner los valores en el webconfig!
                    if (image.Width < Convert.ToInt32(ConfigurationManager.AppSettings["ImageWidth"]) || image.Height < Convert.ToInt32(ConfigurationManager.AppSettings["ImageHeigth"]))
                    {
                        ModelState.AddModelError("", "Resolution is Too Low!");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                try
                {
                    if (ModelState.IsValid)
                    {
                        //Saving the image into the folder
                        @events.ImageToUpload.SaveAs(physicalPath);
                        //Asing the url of the file
                        @events.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Eventos/" + ImageName;
                        //Inserto nuevo evento en la BD
                        unitOfWork.EventsRepository.Insert(events);
                        //Guardo
                        unitOfWork.Save();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, events);
                        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = events.ID }));
                        return response;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
        }

        // DELETE api/EventAPI/5
        //Borrar el evento. Recibe el ID como parámetro
        public HttpResponseMessage DeleteEvents(int id)
        {
            //Busco el evento a borrar por ID en la BD
            Events events = unitOfWork.EventsRepository.GetByID(id);
            if (events == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            //Lo borro
            unitOfWork.EventsRepository.Delete(events);

            try
            {
                //Guardo
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, events);
        }

        protected override void Dispose(bool disposing)
        {
            //Desconectar la base de datos
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}