using System.ComponentModel.DataAnnotations;

namespace Usuario.Models
{
    public class UsuarioViewModel {
        [Key]
        public int Id{get;set;}
        [Required]
        public String Nombre{get;set;}
        public String Apellidos{get;set;}
        public String Rol{get;set;}
        public String Correo{get;set;}
        public int Edad{get;set;}
        public String Contrasena{get;set;}
        
      
    }
}