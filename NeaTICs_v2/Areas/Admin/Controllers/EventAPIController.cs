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
using NeaTICs_v2.Helpers;

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
            //Ordenado por fecha de actualización y luego por fecha de creación
            return events.AsEnumerable().OrderByDescending(x => x.UpdateAt).ThenByDescending(x => x.CreateAt);
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
            TempEvent.UpdateAt = DateTime.Now;
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
                try
                {
                    if (ModelState.IsValid)
                    {
                        //The ImageHelper try to save the image if the image meet all the requirements and if it do then return the name
                        string name = ImageHelper.TryImage(events.ImageToUpload, "Events");
                        //Asing the url of the file
                        events.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Eventos/" + name;
                        //Seteo la fecha de actualización con la fecha actual del sistema
                        events.UpdateAt = DateTime.Now;
                        //Seteo nuevamente la hora de inicio y fin del evento pero con la fecha de creación
                        events.StartAt = events.CreateAt.Date + events.StartAt.TimeOfDay;
                        events.EndAt = events.CreateAt.Date + events.EndAt.TimeOfDay;
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
                        ModelState.AddModelError("", "You must complete all the required fields");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                catch (Exception e)
                {
                    //Log the error if is needed
                    ModelState.AddModelError("", e.Message);
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