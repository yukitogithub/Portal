using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeaTICs_v2.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Method that try to save an image into a folder in the server. Ask some question about the image such as Size, Weight and others.
        /// If the image doesn't meet the requirements, then an exception is thrown
        /// </summary>
        /// <param name="image">The Image that is going to be saved</param>
        /// <param name="from">If you are trying to save an image for an even or a news. The word only accepted are "News" and "Events". Case sensitive</param>
        /// <returns>The name of the image. Not the complete url. Just the name</returns>
        public static string TryImage(HttpPostedFileBase image, string from)
        {
            //To know the file extension
            string Extension = System.IO.Path.GetExtension(image.FileName);
            string[] extArray = { ".jpg", ".jpeg", ".png", ".gif" };
            //if (Extension != ".jpg" && Extension != ".jpeg" && Extension != ".png" && Extension != ".gif")
            if (!extArray.Any(x=>x==Extension))
            {
                throw new Exception("The image extension is wrong. Please, select an apropiate image from the list [.jpg | .jpeg | .png | .gif]");
                //ModelState.AddModelError("", "The image extension is wrong. Please, select an apropiate image from the list [.jpg | .jpeg | .png | .gif]");
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            //To know the name of the file
            string ImageName = System.IO.Path.GetFileName(image.FileName);
            //To know the physical path
            string physicalPath = null;
            switch(from)
            {
                case "News":
                    physicalPath = HttpContext.Current.Server.MapPath("~/Images/Noticias/" + ImageName);
                    break;
                case "Events":
                    physicalPath = HttpContext.Current.Server.MapPath("~/Images/Eventos/" + ImageName);
                    break;
                default:
                    throw new Exception("Internal error. Please contact your administrator to solve this");
            }
            //To know the size of the file
            int FileSize = image.ContentLength;

            //Si la imagen supera un tamaño máximo permitido se devuelve error
            if (FileSize >= /*1048576 1mb*/ Convert.ToInt32(ConfigurationManager.AppSettings["ImageWeight"]) /*5mb*/)
            {
                throw new Exception("Maximum File Size 5 MB");
            }

            //Si la imagen tiene una resolución muy baja se devuelve un error
            string[] strArray = { "S", "SI", "Si", "si", "Y", "YES", "Yes", "yes" };
            if (strArray.Any(x => x == ConfigurationManager.AppSettings["ImageSizeControl"]))
            {
                using (Image imagetemp = Image.FromStream(image.InputStream, true, true))
                {
                    if (imagetemp.Width < Convert.ToInt32(ConfigurationManager.AppSettings["ImageWidth"]) && imagetemp.Height < Convert.ToInt32(ConfigurationManager.AppSettings["ImageHeigth"]))
                    {
                        throw new Exception("Resolution is Too Low!");
                    }
                }
            }
            try 
            { 
                image.SaveAs(physicalPath);
                return image.FileName;
            }
            catch 
            { 
                throw new Exception("Unable to save the image"); 
            }
            return image.FileName;
        }
    }
}