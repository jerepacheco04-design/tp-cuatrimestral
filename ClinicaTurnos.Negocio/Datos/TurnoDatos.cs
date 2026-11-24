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

        public List<Turno> Listar(int? id = null)
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
                               WHERE @Id IS NULL OR t.Id = @Id
                               ORDER BY t.FechaHora, t.Id";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = (object)id ?? DBNull.Value;
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

        public void Agregar(Turno turno)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    ValidarDisponibilidad(turno, conexion, transaccion);
                    string consulta = @"INSERT INTO dbo.Turnos (PacienteId, MedicoId, EspecialidadId, FechaHora, Estado, MotivoConsulta)
                                        OUTPUT INSERTED.Id VALUES (@Paciente, @Medico, @Especialidad, @FechaHora, @Estado, @Motivo)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
                    {
                        comando.Parameters.Add("@Paciente", SqlDbType.Int).Value = turno.Paciente.Id;
                        comando.Parameters.Add("@Medico", SqlDbType.Int).Value = turno.Medico.Id;
                        comando.Parameters.Add("@Especialidad", SqlDbType.Int).Value = turno.Especialidad.Id;
                        comando.Parameters.Add("@FechaHora", SqlDbType.DateTime2).Value = turno.FechaHora;
                        comando.Parameters.Add("@Estado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Nuevo;
                        comando.Parameters.Add("@Motivo", SqlDbType.NVarChar, 500).Value = turno.MotivoConsulta;
                        int id = (int)comando.ExecuteScalar();
                        transaccion.Commit();
                        turno.Id = id;
                        turno.Estado = EstadoTurno.Nuevo;
                    }
                }
            }
        }

        public void Cancelar(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(
                @"UPDATE dbo.Turnos
                  SET Estado = @Cancelado
                  WHERE Id = @Id
                    AND Estado IN (@Nuevo, @Reprogramado)
                    AND FechaHora > @Ahora", conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                comando.Parameters.Add("@Cancelado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Cancelado;
                comando.Parameters.Add("@Nuevo", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Nuevo;
                comando.Parameters.Add("@Reprogramado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Reprogramado;
                comando.Parameters.Add("@Ahora", SqlDbType.DateTime2).Value = DateTime.Now;
                conexion.Open();
                if (comando.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("El turno no existe, ya pasó o ya no se puede cancelar.");
            }
        }

        public List<Turno> ListarOcupados(int pacienteId, int medicoId, DateTime desde, DateTime hasta, int turnoId)
        {
            List<Turno> ocupados = new List<Turno>();
            string consulta = @"SELECT PacienteId, MedicoId, FechaHora
                                FROM dbo.Turnos
                                WHERE Estado <> @Cancelado AND Id <> @Id
                                  AND FechaHora > @Desde AND FechaHora < @Hasta
                                  AND (@Medico = 0 OR MedicoId = @Medico OR PacienteId = @Paciente)";
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@Cancelado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Cancelado;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = turnoId;
                comando.Parameters.Add("@Paciente", SqlDbType.Int).Value = pacienteId;
                comando.Parameters.Add("@Medico", SqlDbType.Int).Value = medicoId;
                comando.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = desde;
                comando.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = hasta;
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ocupados.Add(new Turno
                        {
                            Paciente = new Paciente { Id = (int)lector["PacienteId"] },
                            Medico = new Medico { Id = (int)lector["MedicoId"] },
                            FechaHora = (DateTime)lector["FechaHora"]
                        });
                    }
                }
            }
            return ocupados;
        }

        public void Reprogramar(Turno turno, DateTime fechaAnterior)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                using (SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    ValidarDisponibilidad(turno, conexion, transaccion, turno.Id);
                    string consulta = @"UPDATE dbo.Turnos
                                        SET FechaHora = @FechaHora, Estado = @Reprogramado
                                        WHERE Id = @Id AND FechaHora = @FechaAnterior
                                          AND FechaHora > @Ahora AND @FechaHora > @Ahora
                                          AND Estado IN (@Nuevo, @Reprogramado)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
                    {
                        comando.Parameters.Add("@Id", SqlDbType.Int).Value = turno.Id;
                        comando.Parameters.Add("@FechaHora", SqlDbType.DateTime2).Value = turno.FechaHora;
                        comando.Parameters.Add("@FechaAnterior", SqlDbType.DateTime2).Value = fechaAnterior;
                        comando.Parameters.Add("@Ahora", SqlDbType.DateTime2).Value = DateTime.Now;
                        comando.Parameters.Add("@Nuevo", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Nuevo;
                        comando.Parameters.Add("@Reprogramado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Reprogramado;
                        if (comando.ExecuteNonQuery() == 0)
                            throw new InvalidOperationException("El turno cambió o ya no se puede reprogramar. Volvé al listado.");
                    }
                    transaccion.Commit();
                }
            }
        }

        private void ValidarDisponibilidad(Turno turno, SqlConnection conexion, SqlTransaction transaccion, int turnoId = 0)
        {
            string consultas = @"SELECT CASE
                                    WHEN NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Id = @Paciente) THEN 1
                                    WHEN NOT EXISTS (
                                        SELECT 1 FROM dbo.MedicosEspecialidades
                                        WHERE MedicoId = @Medico AND EspecialidadId = @Especialidad
                                    ) THEN 2
                                    WHEN NOT EXISTS (
                                        SELECT 1 FROM dbo.Medicos m
                                        INNER JOIN dbo.FranjasHorarias f ON f.TurnoTrabajoId = m.TurnoTrabajoId
                                        WHERE m.Id = @Medico AND f.Dia = @Dia
                                            AND f.HoraEntrada <= @Entrada AND f.HoraSalida >= @Salida
                                    ) THEN 3
                                    WHEN EXISTS (
                                        SELECT 1 FROM dbo.Turnos WITH (UPDLOCK)
                                        WHERE Estado <> @Cancelado AND Id <> @TurnoId AND FechaHora > @Desde AND FechaHora < @Hasta
                                            AND (MedicoId = @Medico OR PacienteId = @Paciente)
                                    ) THEN 4
                                    ELSE 0 END";
            using (SqlCommand comando = new SqlCommand(consultas, conexion, transaccion))
            {
                comando.Parameters.Add("@Paciente", SqlDbType.Int).Value = turno.Paciente.Id;
                comando.Parameters.Add("@Medico", SqlDbType.Int).Value = turno.Medico.Id;
                comando.Parameters.Add("@Especialidad", SqlDbType.Int).Value = turno.Especialidad.Id;
                comando.Parameters.Add("@TurnoId", SqlDbType.Int).Value = turnoId;
                comando.Parameters.Add("@Dia", SqlDbType.TinyInt).Value = (byte)turno.FechaHora.DayOfWeek;
                comando.Parameters.Add("@Entrada", SqlDbType.Time).Value = turno.FechaHora.TimeOfDay;
                comando.Parameters.Add("@Salida", SqlDbType.Time).Value = turno.FechaHora.TimeOfDay.Add(Turno.Duracion);
                comando.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = turno.FechaHora.Subtract(Turno.Duracion);
                comando.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value = turno.FechaHora.Add(Turno.Duracion);
                comando.Parameters.Add("@Cancelado", SqlDbType.TinyInt).Value = (byte)EstadoTurno.Cancelado;
                switch ((int)comando.ExecuteScalar())
                {
                    case 1: throw new ArgumentException("El paciente seleccionado ya no existe.");
                    case 2: throw new ArgumentException("El médico seleccionado no atiende esa especialidad.");
                    case 3: throw new ArgumentException("El turno de una hora debe quedar dentro del horario de trabajo del médico.");
                    case 4: throw new ArgumentException("El médico o el paciente ya tiene un turno que se superpone.");
                }
            }
        }
    }
}
