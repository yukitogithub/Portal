using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace NeaTICs_v2.Models
{
    public class New
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage="El título es requerido")]
        [Display(Name="Título")]
        public string Title { get; set; }
        [Required(ErrorMessage="El contenido es requerido")]
        [Display(Name="Contenido")]
        public string Content { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageToUpload { get; set; }

        [Display(Name="Imagen")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage="La fecha es requerida")]
        [Display(Name="Fecha de Creación")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Fecha de Actualización")]
        public DateTime UpdatedAt { get; set; }
        public Users Owner { get; set; }
        public List<Tag> Tags { get; set; }
        
        //De donde vienen las noticias
        [Display(Name="Url Externa")]
        public string Url { get; set; }
    }
}