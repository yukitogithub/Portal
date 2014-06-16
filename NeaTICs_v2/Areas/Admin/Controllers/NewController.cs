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
    public class NewController : Controller
    {
        //Unidad de trabajo con todos los repositorios
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Admin/New/
        //Listar todas las noticias
        public ActionResult Index()
        {
            //Método para traer todas las noticias de la BD
            var news = unitOfWork.NewsRepository.All();
            return View(news.ToList());
        }

        //
        // GET: /Admin/New/Details/5
        //Cargar la vista detallada de una noticia. Le paso el ID como parametro
        public ActionResult Details(int id = 0)
        {
            //Método para traer una noticia de la BD por su ID
            New @new = unitOfWork.NewsRepository.GetByID(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
        }

        //
        // GET: /Admin/New/Create
        //Cargado del formulario para crear una nueva noticia
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/New/Create
        //POST del formulario de creación de nueva noticia. Se crea una nueva noticia con los datos que se pasan del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(New @new)
        {
            //var contentType = @new.ImageToUpload.ContentType;
            //Acá convierto el archivo que suben en un arreglo de bytes para ser guardado en la BD
            //var ImageData = new byte[@new.ImageToUpload.ContentLength];
            //@new.ImageToUpload.InputStream.Read(ImageData, 0, @new.ImageToUpload.ContentLength);
            //@new.Image = ImageData;
            if (@new.ImageToUpload == null)
            {
                ModelState.AddModelError("", "The image for the news is required. Please, select an apropiate image");
            }
            else
            {   
                try
                {
                    if (ModelState.IsValid)
                    {
                        //The ImageHelper try to save the image if the image meet all the requirements and if it do then return the name
                        string name = ImageHelper.TryImage(@new.ImageToUpload, "News");
                        //Guardo en la noticia la url de la imagen
                        @new.ImageUrl = ConfigurationManager.AppSettings["ImageUrl"] + "Noticias/" + name;
                        //Inserto la nueva noticia en la BD
                        unitOfWork.NewsRepository.Insert(@new);
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
            return View(@new);
        }

        //
        // GET: /Admin/New/Edit/5
        //Cargado de la página detallada de noticias pero con los campos para editar. Se le pasa el ID de la noticia como parametro
        public ActionResult Edit(int id = 0)
        {
            New @new = unitOfWork.NewsRepository.GetByID(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
        }

        //
        // POST: /Admin/New/Edit/5
        //POST del formulario de edición de la noticia. Recibe por parámetro todos los campos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(New @new)
        {
            //var ImageData = new byte[@new.ImageToUpload.ContentLength];
            //@new.ImageToUpload.InputStream.Read(ImageData, 0, @new.ImageToUpload.ContentLength);
            //@new.Imagen = ImageData;
            //Recupero la noticia a ser editada desde la BD por ID
            New TempNew = unitOfWork.NewsRepository.GetByID(@new.ID);
            //Paso todos los atributos a editar a la noticia que recuperé de la BD
            //Hago esto porque sino vuelve a guardar la imagen pero como nulo
            TempNew.Title = @new.Title;
            TempNew.Content = @new.Content;
            TempNew.CreatedAt = @new.CreatedAt;
            TempNew.UpdatedAt = @new.UpdatedAt;
            TempNew.Url = @new.Url;
            try
            {
                if (ModelState.IsValid)
                {
                    //Actualizo la noticia
                    unitOfWork.NewsRepository.Update(TempNew);
                    //Guardado
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(TempNew);
        }

        //
        // GET: /Admin/New/Delete/5
        //Cargado de la vista detallada de una noticia para ser borrada. Se le pasa por parámetro el ID
        public ActionResult Delete(int id = 0)
        {
            New @new = unitOfWork.NewsRepository.GetByID(id);
            if (@new == null)
            {
                return HttpNotFound();
            }
            return View(@new);
        }

        //
        // POST: /Admin/New/Delete/5
        //POST para borrar la noticia. Recibe el ID como parámetro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Busco la noticia a borrar por ID en la BD
                New @new = unitOfWork.NewsRepository.GetByID(id);
                //La borro
                unitOfWork.NewsRepository.Delete(@new);
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