using System.ComponentModel.DataAnnotations;

namespace Venta.Models
{
    public class VentaViewModel {
        [Key]
        public int Id{get;set;}
        [Required]
        public int idUser{get;set;}
        public double total{get;set;}
        public int cantidad_vendida{get;set;}
         public DateTime fecha{get;set;}
    }
}