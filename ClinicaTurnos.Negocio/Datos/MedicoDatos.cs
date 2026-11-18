using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                         e.Id AS EspecialidadId, e.Nombre AS Especialidad
                  FROM dbo.Medicos m
                  INNER JOIN dbo.TurnosTrabajo t ON t.Id = m.TurnoTrabajoId
                  LEFT JOIN dbo.MedicosEspecialidades me ON me.MedicoId = m.Id
                  LEFT JOIN dbo.Especialidades e ON e.Id = me.EspecialidadId
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

        public void Guardar(Medico medico)
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
                            using (SqlCommand borrar = new SqlCommand("DELETE FROM dbo.MedicosEspecialidades WHERE MedicoId = @Id", conexion, transaccion))
                            {
                                borrar.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                                borrar.ExecuteNonQuery();
                            }
                        }
                    }
                    using (SqlCommand comando = new SqlCommand(
                        "INSERT INTO dbo.MedicosEspecialidades (MedicoId, EspecialidadId) VALUES (@Id, @Especialidad)", conexion, transaccion))
                    {
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        SqlParameter especialidad = comando.Parameters.Add("@Especialidad", SqlDbType.Int);
                        foreach (Especialidad item in medico.Especialidades)
                        {
                            especialidad.Value = item.Id;
                            comando.ExecuteNonQuery();
                        }
                    }
                    transaccion.Commit();
                    medico.Id = id;
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
