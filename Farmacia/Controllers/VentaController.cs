using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Venta.Models;
using System.Data;

namespace Ventas.Controllers
{
    public class VentaController : Controller
    {
        private readonly IConfiguration _conf;

        public VentaController(IConfiguration conf)
        {
            _conf = conf;
        }

       
 public IActionResult Index (){
        if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
      

       
        DataTable tbl= new DataTable();
        using (MySqlConnection cnx= new MySqlConnection(_conf.GetConnectionString("DevConnection"))){
            cnx.Open();
           
    string cookieValue = Request.Cookies["UsuarioCookie"];
            String query="select * from Ventas where idUser="+cookieValue;
            
    // Realizar operaciones con el valor de la cookie
     ViewData["nombreUser"]=ObtenerNombre(Convert.ToInt32(cookieValue));
     

           
            MySqlDataAdapter adp= new  MySqlDataAdapter(query,cnx);
            
            adp.Fill(tbl);
            cnx.Close();

        }
         return View(tbl);
       
        }else{
            return RedirectToAction("Index","Home");
        }
       
    }
        [HttpPost]
        public IActionResult Agregar(VentaViewModel venta)
        {
           
            if (ModelState.IsValid)
            {
                // Agregar la venta a la base de datos

                using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
                {
                    cnx.Open();
                    
                    string query = "INSERT INTO Ventas (idUser, total, cantidad_vendida) VALUES (@idUser, @total, @cantidad)";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@idUser", venta.idUser);
                    cmd.Parameters.AddWithValue("@total", venta.total);
                    cmd.Parameters.AddWithValue("@cantidad", venta.cantidad_vendida);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(venta);
        }

        public IActionResult Editar(int id)
        {
            

            var venta = GetVentaByIdFromDatabase(id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        [HttpPost]
        public IActionResult Editar(VentaViewModel venta)
        {
           

            if (ModelState.IsValid)
            {
                // Actualizar la venta en la base de datos

                using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
                {
                    cnx.Open();

                    string query = "UPDATE Ventas SET idUser = @idUser, total = @total,fecha = @fecha cantidad_vendida = @cantidad WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);
                    cmd.Parameters.AddWithValue("@idUser", venta.idUser);
                    cmd.Parameters.AddWithValue("@total", venta.total);
                    cmd.Parameters.AddWithValue("@cantidad", venta.cantidad_vendida);
                    cmd.Parameters.AddWithValue("@id", venta.Id);
                    cmd.Parameters.AddWithValue("@fecha", venta.fecha);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(venta);
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
           

            // Eliminar la venta de la base de datos

            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();

                string query = "DELETE FROM Ventas WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        private IQueryable<VentaViewModel> GetVentasFromDatabase()
        {
            IQueryable<VentaViewModel> ventas;

            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();

                string query = "SELECT * FROM Ventas";
                MySqlCommand cmd = new MySqlCommand(query, cnx);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    ventas = MapDataToVentas(reader);
                }
            }

            return ventas;
        }

        private VentaViewModel GetVentaByIdFromDatabase(int id)
        {
            VentaViewModel venta = null;

            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();

                string query = "SELECT * FROM Ventas WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        venta = MapDataToVenta(reader);
                    }
                }
            }

            return venta;
        }

        private IQueryable<VentaViewModel> MapDataToVentas(MySqlDataReader reader)
        {
            var ventas = new List<VentaViewModel>();

            while (reader.Read())
            {
                var venta = new VentaViewModel
                {
                    Id = Convert.ToInt32(reader["id"]),
                    idUser = Convert.ToInt32(reader["idUser"]),
                    total = Convert.ToDouble(reader["total"]),
                    cantidad_vendida = Convert.ToInt32(reader["cantidad_vendida"])
                };

                ventas.Add(venta);
            }

            return ventas.AsQueryable();
        }

        private VentaViewModel MapDataToVenta(MySqlDataReader reader)
        {
            var venta = new VentaViewModel
            {
                Id = Convert.ToInt32(reader["id"]),
                idUser = Convert.ToInt32(reader["idUser"]),
                total = Convert.ToDouble(reader["total"]),
                cantidad_vendida = Convert.ToInt32(reader["cantidad_vendida"]),
                fecha=Convert.ToDateTime(reader["fecha"])
            };

            return venta;
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
        private string ObtenerNombre(int id)
        {
            string rol="";
            // Aquí puedes realizar la lógica de validación de las credenciales
            // Conectarte a la base de datos y comparar el correo y la contraseña con los registros de usuarios

            // Ejemplo básico de validación (solo como referencia, debes implementar tu propia lógica):
            using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
            {
                cnx.Open();
                string query = "SELECT nombre FROM Usuarios WHERE id = @id";
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
