using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NeaTICs_v2.Models
{
    public class Events
    {
        public int ID { get; set; }
        public Users Owner { get; set; }
        [Required(ErrorMessage="El título es requerido")]
        [Display(Name="Título")]
        public string Title { get; set; }
        [Required(ErrorMessage = "El contenido es requerido")]
        [Display(Name = "Contenido")]
        public string Content { get; set; }
        [Required(ErrorMessage="El lugar es requerido")]
        [Display(Name="Lugar del evento")]
        public string Place { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageToUpload { get; set; }

        [Display(Name="Imagen")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [Display(Name = "Fecha del evento")]
        public DateTime CreateAt { get; set; }

        [Display(Name = "Fecha de Actualización")]
        public DateTime UpdateAt { get; set; }
        
        [Required(ErrorMessage="El horario de inicio es requerido")]
        [Display(Name="Hora de inicio")]
        public DateTime StartAt { get; set; }

        [Display(Name = "Hora de finalización")]
        public DateTime EndAt { get; set; }

        [Display(Name = "Url Externa")]
        public string Url { get; set; }

        [Display(Name="Observaciones")]
        public string Observations { get; set; }
    }
}