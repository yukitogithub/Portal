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
using System.Web.Mvc;
using NeaTICs_v2.Models;
using NeaTICs_v2.DAL;
//Para convertir a imagen y ver la resolución
using System.Drawing;
using System.Configuration;
using NeaTICs_v2.Helpers;

namespace NeaTICs_v2.Areas.Admin.Controllers
{
    public class NewAPIController : ApiController
    {
        //Unidad de trabajo con todos los repositorios
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET api/NewAPI
        //Listar todas las noticias
        public IEnumerable<New> GetNews()
        {
            var news = unitOfWork.NewsRepository.All();
            return news.AsEnumerable();
        }

        // GET api/NewAPI/5
        //Cargar la vista detallada de una noticia. Le paso el ID como parametro
        public New GetNew(int id)
        {
            //Método para traer una noticia de la BD por su ID
            New @new = unitOfWork.NewsRepository.GetByID(id);
            if (@new == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return @new;
        }

        // PUT api/NewAPI/5
        //Edición de una noticia. Recibe por parámetro todos los campos
        public HttpResponseMessage PutNew(int id, New @new)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != @new.ID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            New TempNew = unitOfWork.NewsRepository.GetByID(@new.ID);
            TempNew.Title = @new.Title;
            TempNew.Content = @new.Content;
            TempNew.CreatedAt = @new.CreatedAt;
            TempNew.UpdatedAt = @new.UpdatedAt;
            TempNew.Url = @new.Url;
            //Actualizo la noticia
            unitOfWork.NewsRepository.Update(TempNew);
            try
            {
                //Guardado
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/NewAPI
        //Creación de nueva noticia. Se crea una nueva noticia con los datos que se pasan del formulario
        public HttpResponseMessage PostNew(New @new)
        {
            //For saving an image into the database
            //var ImageData = new byte[@new.ImageToUpload.ContentLength];
            //@new.ImageToUpload.InputStream.Read(ImageData, 0, @new.ImageToUpload.ContentLength);
            //@new.Image = ImageData;

            if (@new.ImageToUpload == null)
            {
                ModelState.AddModelError("", "The image for the news is required. Please, select an apropiate image ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //The ImageHelper try to save the image if the image meet all the requirements and if it do then return the name
                        string name = ImageHelper.TryImage(@new.ImageToUpload, "News");
                        //Saving the url into the new
                        @new.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Noticias/" + name; ;
                        //Insert the new into the BD
                        unitOfWork.NewsRepository.Insert(@new);
                        //Saving
                        unitOfWork.Save();
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, @new);
                        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = @new.ID }));
                        return response;
                    }
                    else
                    {
                        ModelState.AddModelError("","You must complete all the required fields");
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

        // DELETE api/NewAPI/5
        //Borrar la noticia. Recibe el ID como parámetro
        public HttpResponseMessage DeleteNew(int id)
        {
            //Busco la noticia a borrar por ID en la BD
            New @new = unitOfWork.NewsRepository.GetByID(id);
            if (@new == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            //La borro
            unitOfWork.NewsRepository.Delete(@new);
            try
            {
                //Guardo
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, @new);
        }

        protected override void Dispose(bool disposing)
        {
            //Desconectar la base de datos
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}