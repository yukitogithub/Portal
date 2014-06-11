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
                ModelState.AddModelError("", "The image for the new is required. Please, select an apropiate image");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                //For saving an image into a folder
                //To know the file extension
                string Extension = System.IO.Path.GetExtension(@new.ImageToUpload.FileName);

                if(Extension != ".jpg" && Extension != ".jpeg" && Extension != ".png" && Extension != ".gif")
                {
                    ModelState.AddModelError("", "The image extension is wrong. Please, select an apropiate image");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                
                //To know the name of the file
                string ImageName = System.IO.Path.GetFileName(@new.ImageToUpload.FileName);
                
                //To know the physical path
                string physicalPath = HttpContext.Current.Server.MapPath("~/Images/Noticias/" + ImageName);
                
                //To know the size of the file
                int FileSize = @new.ImageToUpload.ContentLength;

                //If the size is too big you get an error
                if (FileSize >= /*1048576 1mb*/ Convert.ToInt32(ConfigurationManager.AppSettings["ImageSize"]) /*5mb*/)
                {
                    //TODO: Poner el tamaño máximo del archivo permitido en el WebConfig
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                    ModelState.AddModelError("", "Maximum File Size 5 MB");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                //If the image is too small you get an error
                using (Image image = Image.FromStream(@new.ImageToUpload.InputStream, true, true))
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
                        @new.ImageToUpload.SaveAs(physicalPath);
                        //Saving the url into the new
                        @new.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Noticias/" + ImageName; ;
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