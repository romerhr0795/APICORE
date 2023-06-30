using APICORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace APICORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productoController : ControllerBase
    {
        private readonly string cadenaSQL;
        public productoController(IConfiguration confi)
        {
            cadenaSQL = confi.GetConnectionString("CadenaSQL");
        }


        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Productos> lista = new List<Productos>();
            SqlConnection conexion = new SqlConnection(cadenaSQL);

            try
            {               
                var cmd = new SqlCommand("sp_lista_productos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Productos
                        {
                            IdProducto = Convert.ToInt32(rd["IdProducto"]),
                            CodigoBarra = rd["CodigoBarra"].ToString(),
                            Nombre = string.IsNullOrEmpty(rd["Nombre"].ToString()) ? "Sin Marca"  : rd["Nombre"].ToString(),
                            Marca = string.IsNullOrEmpty(rd["Marca"].ToString()) ? "SIN MARCA" : rd["Marca"].ToString(),
                            Categoria = string.IsNullOrEmpty( rd["Categoria"].ToString()) ? "sin marca" : rd["Marca"].ToString(),
                            Precio = Convert.ToDecimal(rd["Precio"]),
                        });
                    }

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = lista });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = lista });
            }
            finally
            {
                //Siempre se va ejecutar no importa si lo hace bien o genera error 
                conexion.Close();
            }
            


        }

        [HttpGet]
        [Route("Obtener/{idProducto:int}")]
        public IActionResult Obtener(int idProducto)
        {
            Productos ObjProducto = new Productos();
            SqlConnection conexion = new SqlConnection(cadenaSQL);


            try
            {
             
                {
                    conexion.Open();
                    var cmd = new SqlCommand("ObtenerProductoPorId", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Pasa el parametro a consuktar
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            ObjProducto.IdProducto = Convert.ToInt32(rd["IdProducto"]);
                            ObjProducto.CodigoBarra = rd["CodigoBarra"].ToString();
                            ObjProducto.Nombre = rd["Nombre"].ToString();
                            ObjProducto.Marca = rd["Marca"].ToString();
                            ObjProducto.Categoria = rd["Categoria"].ToString();
                            ObjProducto.Precio = Convert.ToDecimal(rd["Precio"]);
                        }
                    }

                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = ObjProducto });


            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
            finally
            {
               conexion.Close();    
            }


        }

        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] Productos objeto)
        {
            SqlConnection conexion = new SqlConnection(cadenaSQL);

            try

            {
                
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_producto", conexion);
                    cmd.Parameters.AddWithValue("codigoBarra", objeto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", objeto.Nombre);
                    cmd.Parameters.AddWithValue("marca", objeto.Marca);
                    cmd.Parameters.AddWithValue("categoria", objeto.Categoria);
                    cmd.Parameters.AddWithValue("precio", objeto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ok" });


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
            finally { conexion.Close(); }
        }



        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] Productos objecto)
        {
            SqlConnection conexion = new SqlConnection(cadenaSQL);
            try
            {
                
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_editar_producto", conexion);
                    cmd.Parameters.AddWithValue("idProducto", objecto.IdProducto == 0 ? DBNull.Value : objecto.IdProducto);
                    cmd.Parameters.AddWithValue("codigoBarra", objecto.CodigoBarra is null ? DBNull.Value : objecto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", objecto.Nombre is null ? DBNull.Value : objecto.Nombre);
                    cmd.Parameters.AddWithValue("marca", objecto.Marca is null ? DBNull.Value : objecto.Marca);
                    cmd.Parameters.AddWithValue("categoria", objecto.Categoria is null ? DBNull.Value : objecto.Categoria);
                    cmd.Parameters.AddWithValue("precio", objecto.Precio == 0 ? DBNull.Value : objecto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Editado" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "error" });
            }
            finally
            {
                conexion.Close();   
            }

        }

        [HttpDelete]
        [Route("Eliminar/{idProducto:int}")]
        public IActionResult Eliminar(int idProducto)
        {
            SqlConnection conexion = new SqlConnection(cadenaSQL);
            try
            {

               // using (var conexion = new SqlConnection(cadenaSQL))
                {
                    
                    var cmd = new SqlCommand("sp_eliminar_producto", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("idProducto", idProducto);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Eliminado" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "error" });
            }
            finally { conexion.Close(); }   

        }
    }
}
