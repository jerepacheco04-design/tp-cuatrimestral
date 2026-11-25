using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio.Datos
{
    internal class MedicoDatos
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaTurnos"].ConnectionString;

        public List<Medico> Listar(int? id = null)
        {
            List<Medico> medicos = new List<Medico>();
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(
                @"SELECT m.Id, m.Nombre, m.Apellido, m.Email, m.Matricula, t.Id AS TurnoId, t.Nombre AS Horario,
                         e.Id AS EspecialidadId, e.Nombre AS Especialidad, u.NombreUsuario
                  FROM dbo.Medicos m
                  INNER JOIN dbo.TurnosTrabajo t ON t.Id = m.TurnoTrabajoId
                  LEFT JOIN dbo.MedicosEspecialidades me ON me.MedicoId = m.Id
                  LEFT JOIN dbo.Especialidades e ON e.Id = me.EspecialidadId
                  LEFT JOIN dbo.Usuarios u ON u.Id = m.UsuarioId
                  WHERE @Id IS NULL OR m.Id = @Id
                  ORDER BY m.Apellido, m.Nombre, m.Id, e.Nombre, e.Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = (object)id ?? DBNull.Value;
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    Medico medico = null;
                    while (lector.Read())
                    {
                        int medicoId = (int)lector["Id"];
                        if (medico == null || medico.Id != medicoId)
                        {
                            medico = new Medico
                            {
                                Id = medicoId,
                                Nombre = (string)lector["Nombre"],
                                Apellido = (string)lector["Apellido"],
                                Email = (string)lector["Email"],
                                Matricula = (string)lector["Matricula"],
                                NombreUsuario = lector["NombreUsuario"] as string,
                                TurnoTrabajo = new TurnoTrabajo
                                {
                                    Id = (int)lector["TurnoId"],
                                    Nombre = (string)lector["Horario"]
                                }
                            };
                            medicos.Add(medico);
                        }
                        if (lector["EspecialidadId"] != DBNull.Value)
                        {
                            medico.Especialidades.Add(new Especialidad
                            {
                                Id = (int)lector["EspecialidadId"],
                                Nombre = (string)lector["Especialidad"]
                            });
                        }
                    }
                }
            }
            return medicos;
        }

        public void Guardar(Medico medico, string clave)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction())
                {
                    int id = medico.Id;
                    string consulta = id == 0
                        ? @"INSERT INTO dbo.Medicos (Nombre, Apellido, Email, Matricula, TurnoTrabajoId)
                            OUTPUT INSERTED.Id VALUES (@Nombre, @Apellido, @Email, @Matricula, @Horario)"
                        : @"UPDATE dbo.Medicos SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email,
                            Matricula = @Matricula, TurnoTrabajoId = @Horario WHERE Id = @Id";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
                    {
                        comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = medico.Nombre;
                        comando.Parameters.Add("@Apellido", SqlDbType.NVarChar, 80).Value = medico.Apellido;
                        comando.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = medico.Email;
                        comando.Parameters.Add("@Matricula", SqlDbType.NVarChar, 30).Value = medico.Matricula;
                        comando.Parameters.Add("@Horario", SqlDbType.Int).Value = medico.TurnoTrabajo.Id;
                        if (id == 0)
                            id = (int)comando.ExecuteScalar();
                        else
                        {
                            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                            if (comando.ExecuteNonQuery() == 0)
                                throw new InvalidOperationException("El médico ya no existe. Volvé al listado.");
                        }
                    }
                    ActualizarEspecialidades(medico, id, conexion, transaccion);
                    if (medico.NombreUsuario.Length > 0)
                        CrearAcceso(medico, id, clave, conexion, transaccion);
                    transaccion.Commit();
                    medico.Id = id;
                }
            }
        }

        private void CrearAcceso(Medico medico, int id, string clave, SqlConnection conexion, SqlTransaction transaccion)
        {
            using (SqlCommand existe = new SqlCommand("SELECT UsuarioId FROM dbo.Medicos WHERE Id = @Id", conexion, transaccion))
            {
                existe.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                if (existe.ExecuteScalar() != DBNull.Value)
                    throw new InvalidOperationException("Este médico ya tiene acceso. Volvé a abrir su ficha.");
            }

            int usuario = new UsuarioDatos().CrearCuentaMedico(medico, clave, conexion, transaccion);
            using (SqlCommand vincular = new SqlCommand("UPDATE dbo.Medicos SET UsuarioId = @Usuario WHERE Id = @Id", conexion, transaccion))
            {
                vincular.Parameters.Add("@Usuario", SqlDbType.Int).Value = usuario;
                vincular.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                vincular.ExecuteNonQuery();
            }
        }

        private void ActualizarEspecialidades(Medico medico, int id, SqlConnection conexion, SqlTransaction transaccion)
        {
            HashSet<int> actuales = new HashSet<int>();
            using (SqlCommand consultar = new SqlCommand("SELECT EspecialidadId FROM dbo.MedicosEspecialidades WHERE MedicoId = @Id", conexion, transaccion))
            {
                consultar.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                using (SqlDataReader lector = consultar.ExecuteReader())
                {
                    while (lector.Read())
                        actuales.Add(lector.GetInt32(0));
                }
            }
            HashSet<int> elegidas = new HashSet<int>(medico.Especialidades.Select(item => item.Id));
            using (SqlCommand borrar = new SqlCommand(
                "DELETE FROM dbo.MedicosEspecialidades WHERE MedicoId = @Id AND EspecialidadId = @Especialidad", conexion, transaccion))
            {
                borrar.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                SqlParameter especialidad = borrar.Parameters.Add("@Especialidad", SqlDbType.Int);
                foreach (int especialidadId in actuales.Except(elegidas))
                {
                    especialidad.Value = especialidadId;
                    try
                    {
                        borrar.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Number == 547)
                    {
                        throw new InvalidOperationException("No se puede quitar una especialidad que tiene turnos asociados.");
                    }
                }
            }
            using (SqlCommand agregar = new SqlCommand(
                "INSERT INTO dbo.MedicosEspecialidades (MedicoId, EspecialidadId) VALUES (@Id, @Especialidad)", conexion, transaccion))
            {
                agregar.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                SqlParameter especialidad = agregar.Parameters.Add("@Especialidad", SqlDbType.Int);
                foreach (int especialidadId in elegidas.Except(actuales))
                {
                    especialidad.Value = especialidadId;
                    agregar.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand("DELETE FROM dbo.Medicos WHERE Id = @Id", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                conexion.Open();
                if (comando.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("El médico ya no existe. Volvé al listado.");
            }
        }
    }
}
