
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Usuario.Models;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
namespace Usuarios.Controllers{
    public class UsuarioController:Controller{
    private readonly IConfiguration _conf;
    public UsuarioController (IConfiguration conf)
    {
        this._conf=conf;
    }
    public IActionResult Index (){
        if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       if(rol=="Admin"){

       
        DataTable tbl= new DataTable();
        using (MySqlConnection cnx= new MySqlConnection(_conf.GetConnectionString("DevConnection"))){
            cnx.Open();
            String query="select * from Usuarios";
            MySqlDataAdapter adp= new  MySqlDataAdapter(query,cnx);
            adp.Fill(tbl);
            cnx.Close();
        }
         return View(tbl);
       }else{
        return RedirectToAction("Index","Medicamento");
       }
        }else{
            return RedirectToAction("Index","Home");
        }
       
    }
    public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(UsuarioViewModel model)
        {
            if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       if(rol=="Admin"){
            if (ModelState.IsValid)
            {
                using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
                {
                    cnx.Open();
                    string query = "INSERT INTO Usuarios (nombre, apellidos, correo, contrasena, edad, rol) VALUES (@Nombre, @Apellidos, @Correo, @Contrasena, @Edad, @Rol)";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", model.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.Parameters.AddWithValue("@Contrasena", model.Contrasena);
                    cmd.Parameters.AddWithValue("@Edad", model.Edad);
                    cmd.Parameters.AddWithValue("@Rol", model.Rol);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
       }else{
        return RedirectToAction("Index","Medicamento");
       }
            }else{
            return RedirectToAction("Index","Home");
        }
        }

        public IActionResult Editar(int id)
        {
            if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       if(rol=="Admin"){
            UsuarioViewModel model = new UsuarioViewModel();
            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();
                string query = "SELECT * FROM Usuarios WHERE id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@Id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.Id = Convert.ToInt32(reader["id"]);
                    model.Nombre = reader["nombre"].ToString();
                    model.Apellidos = reader["apellidos"].ToString();
                    model.Correo = reader["correo"].ToString();
                    model.Contrasena = reader["contrasena"].ToString();
                    model.Edad = Convert.ToInt32(reader["edad"]);
                    model.Rol = reader["rol"].ToString();
                }
                reader.Close();
            }
            return View(model);
            }else{
        return RedirectToAction("Index","Medicamento");
       }
            }else{
            return RedirectToAction("Index","Home");
        }
        }

        [HttpPost]
        public IActionResult Editar(UsuarioViewModel model)
        {
            if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       if(rol=="Admin"){
            if (ModelState.IsValid)
            {
                using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
                {
                    cnx.Open();
                    string query = "UPDATE Usuarios SET nombre = @Nombre, apellidos = @Apellidos, correo = @Correo, contrasena = @Contrasena, edad = @Edad, rol = @Rol WHERE id = @Id";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Apellidos", model.Apellidos);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.Parameters.AddWithValue("@Contrasena", model.Contrasena);
                    cmd.Parameters.AddWithValue("@Edad", model.Edad);
                    cmd.Parameters.AddWithValue("@Rol", model.Rol);
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            return View(model);
             }else{
        return RedirectToAction("Index","Medicamento");
       }
            }else{
            return RedirectToAction("Index","Home");
        }
        }

        public IActionResult Eliminar(int id)
        {
            if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       if(rol=="Admin"){
            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();
                string query = "DELETE FROM Usuarios WHERE id = @Id";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
             }else{
        return RedirectToAction("Index","Medicamento");
       }
            }else{
            return RedirectToAction("Index","Home");
        }
        }
        [HttpPost]
        public IActionResult Login(string correo, string contrasena)
        {
            if (ModelState.IsValid)
            {
                // Verificar la autenticación del usuario
                if (ValidarCredenciales(correo, contrasena)!=0)
                {
                    // Crear una cookie para almacenar los datos del usuario
                    ViewData["cookie"]=correo+contrasena;

                    // Guardar los datos del usuario en la cookie
                    Response.Cookies.Append("UsuarioCookie", ValidarCredenciales(correo, contrasena).ToString());

                    return RedirectToAction("Index"); // Redireccionar a la página de inicio
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas. Por favor, inténtalo de nuevo.");
                }
            }

            return View();
        }

        public IActionResult Logout()
        {
            // Eliminar la cookie de usuario al cerrar sesión
            Response.Cookies.Delete("UsuarioCookie");

            return RedirectToAction("Index"); // Redireccionar a la página de inicio
        }

        private int ValidarCredenciales(string correo, string contrasena)
        {
            // Aquí puedes realizar la lógica de validación de las credenciales
            // Conectarte a la base de datos y comparar el correo y la contraseña con los registros de usuarios

            // Ejemplo básico de validación (solo como referencia, debes implementar tu propia lógica):
            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();
                string query = "SELECT id FROM Usuarios WHERE correo = @Correo AND contrasena = @Contrasena";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Contrasena", contrasena);
                int count = Convert.ToInt32(cmd.ExecuteScalar())==null ? 0:Convert.ToInt32(cmd.ExecuteScalar());
                return count ;
            }
        }
        private string ObtenerRol(int id)
        {
            string rol="";
            // Aquí puedes realizar la lógica de validación de las credenciales
            // Conectarte a la base de datos y comparar el correo y la contraseña con los registros de usuarios

            // Ejemplo básico de validación (solo como referencia, debes implementar tu propia lógica):
            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();
                string query = "SELECT rol FROM Usuarios WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@id", id);
               object result = cmd.ExecuteScalar();

    // Verificar si se obtuvo un resultado y realizar las acciones necesarias
    if (result != null)
    {
        rol = result.ToString();
        // Hacer algo con el rol obtenido
    }
                return rol ;
            }
        }
    }
}
   