using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio.Datos
{
    internal class TurnoTrabajoDatos
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaTurnos"].ConnectionString;

        public List<TurnoTrabajo> Listar()
        {
            return Consultar(null);
        }

        public TurnoTrabajo Obtener(int id)
        {
            List<TurnoTrabajo> turnos = Consultar(id);
            return turnos.Count == 0 ? null : turnos[0];
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("SELECT COUNT(*) FROM dbo.TurnosTrabajo WHERE UPPER(Nombre) = UPPER(@Nombre) AND Id <> @Id", conexion))
            {
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = nombre;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = idExcluir;
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public void Agregar(TurnoTrabajo turno)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int id;
                        using (SqlCommand comando = new SqlCommand("INSERT INTO dbo.TurnosTrabajo (Nombre) OUTPUT INSERTED.Id VALUES (@Nombre)", conexion, transaccion))
                        {
                            comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = turno.Nombre;
                            id = (int)comando.ExecuteScalar();
                        }

                        GuardarFranjas(conexion, transaccion, id, turno.Franjas);
                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Modificar(TurnoTrabajo turno)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand comando = new SqlCommand("UPDATE dbo.TurnosTrabajo SET Nombre = @Nombre WHERE Id = @Id", conexion, transaccion))
                        {
                            comando.Parameters.Add("@Id", SqlDbType.Int).Value = turno.Id;
                            comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = turno.Nombre;
                            if (comando.ExecuteNonQuery() == 0)
                                throw new System.InvalidOperationException("El horario ya no existe. Volvé al listado.");
                        }

                        using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.FranjasHorarias WHERE TurnoTrabajoId = @Id", conexion, transaccion))
                        {
                            comando.Parameters.Add("@Id", SqlDbType.Int).Value = turno.Id;
                            comando.ExecuteNonQuery();
                        }

                        GuardarFranjas(conexion, transaccion, turno.Id, turno.Franjas);
                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.TurnosTrabajo WHERE Id = @Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conexion.Open();
                if (comando.ExecuteNonQuery() == 0)
                    throw new System.InvalidOperationException("El horario ya no existe. Volvé al listado.");
            }
        }

        private List<TurnoTrabajo> Consultar(int? id)
        {
            List<TurnoTrabajo> turnos = new List<TurnoTrabajo>();
            Dictionary<int, TurnoTrabajo> porId = new Dictionary<int, TurnoTrabajo>();
            string consulta = "SELECT t.Id, t.Nombre, f.Id AS FranjaId, f.Dia, f.HoraEntrada, f.HoraSalida FROM dbo.TurnosTrabajo t LEFT JOIN dbo.FranjasHorarias f ON f.TurnoTrabajoId = t.Id";
            if (id.HasValue)
                consulta += " WHERE t.Id = @Id";
            consulta += " ORDER BY t.Nombre, f.Dia";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                if (id.HasValue)
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = id.Value;
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        int turnoId = (int)lector["Id"];
                        TurnoTrabajo turno;
                        if (!porId.TryGetValue(turnoId, out turno))
                        {
                            turno = new TurnoTrabajo
                            {
                                Id = turnoId,
                                Nombre = (string)lector["Nombre"]
                            };
                            porId.Add(turnoId, turno);
                            turnos.Add(turno);
                        }

                        if (!lector.IsDBNull(lector.GetOrdinal("FranjaId")))
                        {
                            turno.Franjas.Add(new FranjaHoraria
                            {
                                Id = (int)lector["FranjaId"],
                                Dia = (DayOfWeek)(byte)lector["Dia"],
                                HoraEntrada = (TimeSpan)lector["HoraEntrada"],
                                HoraSalida = (TimeSpan)lector["HoraSalida"]
                            });
                        }
                    }
                }
            }

            return turnos;
        }

        private void GuardarFranjas(SqlConnection conexion, SqlTransaction transaccion, int turnoId, IList<FranjaHoraria> franjas)
        {
            foreach (FranjaHoraria franja in franjas)
            {
                using (SqlCommand comando = new SqlCommand("INSERT INTO dbo.FranjasHorarias (TurnoTrabajoId, Dia, HoraEntrada, HoraSalida) VALUES (@TurnoTrabajoId, @Dia, @HoraEntrada, @HoraSalida)", conexion, transaccion))
                {
                    comando.Parameters.Add("@TurnoTrabajoId", SqlDbType.Int).Value = turnoId;
                    comando.Parameters.Add("@Dia", SqlDbType.TinyInt).Value = (byte)franja.Dia;
                    comando.Parameters.Add("@HoraEntrada", SqlDbType.Time).Value = franja.HoraEntrada;
                    comando.Parameters.Add("@HoraSalida", SqlDbType.Time).Value = franja.HoraSalida;
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}
