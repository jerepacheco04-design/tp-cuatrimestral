using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio.Datos
{
    internal class EspecialidadDatos
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaTurnos"].ConnectionString;

        public List<Especialidad> Listar()
        {
            List<Especialidad> especialidades = new List<Especialidad>();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("SELECT Id, Nombre FROM dbo.Especialidades ORDER BY Nombre", conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Especialidad especialidad = new Especialidad
                        {
                            Id = (int)lector["Id"],
                            Nombre = (string)lector["Nombre"]
                        };
                        especialidades.Add(especialidad);
                    }
                }
            }

            return especialidades;
        }

        public Especialidad Obtener(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("SELECT Id, Nombre FROM dbo.Especialidades WHERE Id = @Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Especialidad
                        {
                            Id = (int)lector["Id"],
                            Nombre = (string)lector["Nombre"]
                        };
                    }
                }
            }

            return null;
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("SELECT COUNT(*) FROM dbo.Especialidades WHERE UPPER(Nombre) = UPPER(@Nombre) AND Id <> @Id", conexion))
            {
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = nombre;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = idExcluir;
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public void Agregar(Especialidad especialidad)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("INSERT INTO dbo.Especialidades (Nombre) VALUES (@Nombre)", conexion))
            {
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = especialidad.Nombre;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Modificar(Especialidad especialidad)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("UPDATE dbo.Especialidades SET Nombre = @Nombre WHERE Id = @Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = especialidad.Id;
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = especialidad.Nombre;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.Especialidades WHERE Id = @Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
