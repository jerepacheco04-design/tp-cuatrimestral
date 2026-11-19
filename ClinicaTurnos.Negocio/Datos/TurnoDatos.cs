using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio.Datos
{
    internal class TurnoDatos
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaTurnos"].ConnectionString;

        public List<Turno> Listar()
        {
            List<Turno> turnos = new List<Turno>();
            string consulta = @"SELECT t.Id, t.FechaHora, t.Estado, t.MotivoConsulta, t.ObservacionesDiagnostico,
                                      p.Id AS PacienteId, p.Dni, p.Nombre AS PacienteNombre, p.Apellido AS PacienteApellido,
                                      m.Id AS MedicoId, m.Nombre AS MedicoNombre, m.Apellido AS MedicoApellido, m.Matricula,
                                      e.Id AS EspecialidadId, e.Nombre AS EspecialidadNombre
                               FROM dbo.Turnos t
                               INNER JOIN dbo.Pacientes p ON p.Id = t.PacienteId
                               INNER JOIN dbo.Medicos m ON m.Id = t.MedicoId
                               INNER JOIN dbo.Especialidades e ON e.Id = t.EspecialidadId
                               ORDER BY t.FechaHora, t.Id";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        turnos.Add(new Turno
                        {
                            Id = (int)lector["Id"],
                            FechaHora = (DateTime)lector["FechaHora"],
                            Estado = (EstadoTurno)Convert.ToInt32(lector["Estado"]),
                            MotivoConsulta = (string)lector["MotivoConsulta"],
                            ObservacionesDiagnostico = lector["ObservacionesDiagnostico"] == DBNull.Value
                                ? null
                                : (string)lector["ObservacionesDiagnostico"],
                            Paciente = new Paciente
                            {
                                Id = (int)lector["PacienteId"],
                                Dni = (string)lector["Dni"],
                                Nombre = (string)lector["PacienteNombre"],
                                Apellido = (string)lector["PacienteApellido"]
                            },
                            Medico = new Medico
                            {
                                Id = (int)lector["MedicoId"],
                                Nombre = (string)lector["MedicoNombre"],
                                Apellido = (string)lector["MedicoApellido"],
                                Matricula = (string)lector["Matricula"]
                            },
                            Especialidad = new Especialidad
                            {
                                Id = (int)lector["EspecialidadId"],
                                Nombre = (string)lector["EspecialidadNombre"]
                            }
                        });
                    }
                }
            }

            return turnos;
        }
    }
}
