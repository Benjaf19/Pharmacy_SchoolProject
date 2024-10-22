
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Medicamentos.Models;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
namespace Medicamentos.Controllers{
    public class MedicamentoController:Controller{
    private readonly IConfiguration _conf;
    public MedicamentoController (IConfiguration conf)
    {
        this._conf=conf;
    }
    public IActionResult Index (){
        if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       ViewData["Rol"]=rol;
        DataTable tbl= new DataTable();
        using (MySqlConnection cnx= new MySqlConnection(_conf.GetConnectionString("DevConnection"))){
            cnx.Open();
            String query="select * from medicamento";
            MySqlDataAdapter adp= new  MySqlDataAdapter(query,cnx);
            adp.Fill(tbl);
            cnx.Close();
        }
        return View(tbl);
        }else{return RedirectToAction("Index","Home");}
    }
    public IActionResult Agregar()
{
    return View();
}

[HttpPost]
public IActionResult Agregar(MedicamentoViewModel model)
{
    if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       ViewData["Rol"]=rol;
    if (ModelState.IsValid)
    {
        using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
        {
            cnx.Open();
            string query = "INSERT INTO Medicamento (nombre, descripcion, tipo, clasificacion, precio, cantidad, fecha_vencimiento) " +
                           "VALUES (@Nombre, @Descripcion, @Tipo, @Clasificacion, @Precio, @Cantidad, @FechaVencimiento)";
            MySqlCommand cmd = new MySqlCommand(query, cnx);
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", model.Descripcion);
            cmd.Parameters.AddWithValue("@Tipo", model.Tipo);
            cmd.Parameters.AddWithValue("@Clasificacion", model.Clasificacion);
            cmd.Parameters.AddWithValue("@Precio", model.Precio);
            cmd.Parameters.AddWithValue("@Cantidad", model.Cantidad);
            cmd.Parameters.AddWithValue("@FechaVencimiento", model.fechaVen);
            cmd.ExecuteNonQuery();
            cnx.Close();
        }

        return RedirectToAction("Index");
    }

    return View(model);
    }else{return View();}
}
public IActionResult Editar(int id)
{
    if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       ViewData["Rol"]=rol;
    using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
    {
        cnx.Open();
        string query = "SELECT * FROM Medicamento WHERE id = @Id";
        MySqlCommand cmd = new MySqlCommand(query, cnx);
        cmd.Parameters.AddWithValue("@Id", id);

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                MedicamentoViewModel model = new MedicamentoViewModel
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nombre = Convert.ToString(reader["nombre"]),
                    Descripcion = Convert.ToString(reader["descripcion"]),
                    Tipo = Convert.ToString(reader["tipo"]),
                    Clasificacion = Convert.ToString(reader["clasificacion"]),
                    Precio = Convert.ToDecimal(reader["precio"]),
                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                    fechaVen = Convert.ToDateTime(reader["fecha_vencimiento"])
                };

                return View(model);
            }
        }
    }

    return RedirectToAction("Index");
    }else{return View();}
}

[HttpPost]
public IActionResult Editar(MedicamentoViewModel model)
{
    if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       ViewData["Rol"]=rol;
    if (ModelState.IsValid)
    {
        using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
        {
            cnx.Open();
            string query = "UPDATE Medicamento SET nombre = @Nombre, descripcion = @Descripcion, tipo = @Tipo, clasificacion = @Clasificacion, " +
                           "precio = @Precio, cantidad = @Cantidad, fecha_vencimiento = @FechaVencimiento WHERE id = @Id";
            MySqlCommand cmd = new MySqlCommand(query, cnx);
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", model.Descripcion);
            cmd.Parameters.AddWithValue("@Tipo", model.Tipo);
            cmd.Parameters.AddWithValue("@Clasificacion", model.Clasificacion);
            cmd.Parameters.AddWithValue("@Precio", model.Precio);
            cmd.Parameters.AddWithValue("@Cantidad", model.Cantidad);
            cmd.Parameters.AddWithValue("@FechaVencimiento", model.fechaVen);
            cmd.Parameters.AddWithValue("@Id", model.Id);

            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }

    return View(model);
    }else{return View();}
}

public IActionResult Eliminar(int id)
{
    if(Request.Cookies["UsuarioCookie"]!=null){
       string rol= ObtenerRol(Convert.ToInt32(Request.Cookies["UsuarioCookie"]));
       ViewData["Rol"]=rol;
    using (MySqlConnection cnx = new MySqlConnection(_conf.GetConnectionString("DevConnection")))
    {
        cnx.Open();
        string query = "DELETE FROM Medicamento WHERE id = @Id";
        MySqlCommand cmd = new MySqlCommand(query, cnx);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    return RedirectToAction("Index");
    }else{return View();}
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