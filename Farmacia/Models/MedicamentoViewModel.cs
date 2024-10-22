using System.ComponentModel.DataAnnotations;

namespace Medicamentos.Models
{
    public class MedicamentoViewModel {
        [Key]
        public int Id{get;set;}
        [Required]
        public String Nombre{get;set;}
        public String Descripcion{get;set;}
        public String Tipo{get;set;}
        public String Clasificacion{get;set;}
        public int Cantidad{get;set;}
        public decimal Precio{get;set;}
        
        public DateTime fechaVen{get;set;}
    }
}