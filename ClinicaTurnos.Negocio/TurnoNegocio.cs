using System;
using System.Collections.Generic;
using System.Linq;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class TurnoNegocio
    {
        private readonly TurnoDatos datos = new TurnoDatos();

        public List<Turno> Listar()
        {
            return datos.Listar();
        }

        public List<Turno> ListarPorMedico(int medicoId)
        {
            ValidarId(medicoId);
            return datos.ListarPorMedico(medicoId);
        }

        public void GuardarDiagnostico(int turnoId, int medicoId, string observaciones)
        {
            ValidarId(turnoId);
            ValidarId(medicoId);
            observaciones = (observaciones ?? "").Trim();
            if (observaciones.Length == 0 || observaciones.Length > 1000)
                throw new ArgumentException("Ingresá un diagnóstico de hasta 1000 caracteres.");

            datos.GuardarDiagnostico(turnoId, medicoId, observaciones);
        }

        public Turno Obtener(int id)
        {
            ValidarId(id);
            return datos.Listar(id).FirstOrDefault();
        }

        public List<Turno> ListarDelMedico(int medicoId)
        {
            return datos.Listar(medicoId: medicoId);
        }

        public Turno ObtenerDelMedico(int id, int medicoId)
        {
            ValidarId(id);
            return datos.Listar(id, medicoId, incluirDiagnostico: true).FirstOrDefault();
        }

        public List<Turno> ListarHistorial(int pacienteId, int? medicoId)
        {
            ValidarId(pacienteId);
            return datos.Listar(medicoId: medicoId, pacienteId: pacienteId, hasta: DateTime.Now, incluirDiagnostico: medicoId.HasValue)
                .OrderByDescending(turno => turno.FechaHora).ThenByDescending(turno => turno.Id).ToList();
        }

        public void GuardarObservacion(int id, int medicoId, string observacion)
        {
            GuardarConsulta(id, medicoId, ValidarObservacion(observacion), null);
        }

        public void CerrarConsulta(int id, int medicoId, string observacion)
        {
            GuardarConsulta(id, medicoId, ValidarObservacion(observacion), EstadoTurno.Cerrado);
        }

        public void MarcarAusencia(int id, int medicoId)
        {
            GuardarConsulta(id, medicoId, null, EstadoTurno.NoAsistio);
        }

        private void GuardarConsulta(int id, int medicoId, string observacion, EstadoTurno? estado)
        {
            ValidarId(id);
            if (!datos.GuardarConsulta(id, medicoId, observacion, estado))
                throw new InvalidOperationException("La consulta no está disponible.");
        }

        private string ValidarObservacion(string observacion)
        {
            observacion = (observacion ?? "").Trim();
            if (observacion.Length == 0 || observacion.Length > 1000)
                throw new ArgumentException("Ingresá una observación de hasta 1000 caracteres.");
            return observacion;
        }

        public List<Medico> ListarMedicos(int especialidadId)
        {
            return new MedicoNegocio().Listar().FindAll(medico =>
                medico.Especialidades.Exists(especialidad => especialidad.Id == especialidadId));
        }

        public List<TurnoOpcion> Sugerir(int pacienteId, int especialidadId)
        {
            DateTime hoy = DateTime.Today;
            return BuscarHorarios(pacienteId, especialidadId, 0, hoy, hoy.AddDays(14), 0)
                .Take(3).ToList();
        }

        public List<TurnoOpcion> ListarHorarios(int pacienteId, int especialidadId, int medicoId, DateTime fecha, int turnoId)
        {
            DateTime dia = fecha.Date;
            DateTime fin = dia == DateTime.MaxValue.Date ? DateTime.MaxValue : dia.AddDays(1);
            return BuscarHorarios(pacienteId, especialidadId, medicoId, dia, fin, turnoId);
        }

        private List<TurnoOpcion> BuscarHorarios(int pacienteId, int especialidadId, int medicoId, DateTime desde, DateTime hasta, int turnoId)
        {
            List<TurnoOpcion> opciones = new List<TurnoOpcion>();
            DateTime ahora = DateTime.Now;
            if (pacienteId <= 0 || especialidadId <= 0 || desde < ahora.Date)
                return opciones;

            List<Medico> medicos = ListarMedicos(especialidadId);
            if (medicoId > 0)
                medicos = medicos.FindAll(medico => medico.Id == medicoId);
            if (medicos.Count == 0)
                return opciones;

            var horarios = new TurnoTrabajoNegocio().Listar().ToDictionary(horario => horario.Id);
            List<Turno> ocupados = datos.ListarOcupados(pacienteId, medicoId, desde.Subtract(Turno.Duracion), hasta, turnoId);
            foreach (Medico medico in medicos)
            {
                TurnoTrabajo horario;
                if (!horarios.TryGetValue(medico.TurnoTrabajo.Id, out horario))
                    continue;

                for (DateTime dia = desde; dia < hasta;)
                {
                    foreach (FranjaHoraria franja in horario.Franjas.Where(f => f.Dia == dia.DayOfWeek))
                    {
                        TimeSpan inicio = TimeSpan.FromMinutes(Math.Ceiling(franja.HoraEntrada.TotalMinutes));
                        for (; inicio.Add(Turno.Duracion) <= franja.HoraSalida; inicio = inicio.Add(Turno.Duracion))
                        {
                            DateTime fecha = dia.Add(inicio);
                            if (fecha <= ahora || EstaOcupado(ocupados, pacienteId, medico.Id, fecha))
                                continue;
                            opciones.Add(new TurnoOpcion { Medico = medico, FechaHora = fecha });
                        }
                    }
                    if (dia == DateTime.MaxValue.Date)
                        break;
                    dia = dia.AddDays(1);
                }
            }
            return opciones.OrderBy(opcion => opcion.FechaHora).ThenBy(opcion => opcion.Medico.Id).ToList();
        }

        private bool EstaOcupado(List<Turno> ocupados, int pacienteId, int medicoId, DateTime fecha)
        {
            DateTime desde = fecha.Subtract(Turno.Duracion);
            DateTime hasta = fecha.Add(Turno.Duracion);
            return ocupados.Exists(turno => turno.FechaHora > desde && turno.FechaHora < hasta &&
                (turno.Paciente.Id == pacienteId || turno.Medico.Id == medicoId));
        }

        public void Agregar(Turno turno)
        {
            if (turno == null)
                throw new ArgumentException("Completá los datos del turno.");
            if (turno.Recepcionista == null || turno.Recepcionista.Id <= 0)
                throw new ArgumentException("Inicia sesion para registrar el turno.");
            if (turno.Paciente == null || turno.Paciente.Id <= 0)
                throw new ArgumentException("Seleccioná un paciente.");
            if (turno.Especialidad == null || turno.Especialidad.Id <= 0)
                throw new ArgumentException("Seleccioná una especialidad.");
            if (turno.Medico == null || turno.Medico.Id <= 0)
                throw new ArgumentException("Seleccioná un médico.");
            ValidarFecha(turno.FechaHora);

            turno.MotivoConsulta = (turno.MotivoConsulta ?? "").Trim();
            if (turno.MotivoConsulta.Length == 0 || turno.MotivoConsulta.Length > 500)
                throw new ArgumentException("Ingresá un motivo de consulta de hasta 500 caracteres.");

            datos.Agregar(turno);
        }

        public void Reprogramar(int id, DateTime fechaAnterior, DateTime fechaNueva)
        {
            ValidarFecha(fechaNueva);
            Turno turno = Obtener(id);
            if (turno == null || !turno.PuedeCambiarAgenda(DateTime.Now) || turno.FechaHora != fechaAnterior)
                throw new InvalidOperationException("El turno cambió o ya no se puede reprogramar. Volvé al listado.");
            if (fechaAnterior == fechaNueva)
                throw new ArgumentException("Elegí una fecha u hora distinta.");

            turno.FechaHora = fechaNueva;
            datos.Reprogramar(turno, fechaAnterior);
        }

        public void Cancelar(int id)
        {
            ValidarId(id);
            datos.Cancelar(id);
        }

        private void ValidarFecha(DateTime fechaHora)
        {
            if (fechaHora <= DateTime.Now)
                throw new ArgumentException("El turno debe ser en una fecha y hora futuras.");
            if (fechaHora.TimeOfDay.Add(Turno.Duracion) >= TimeSpan.FromDays(1))
                throw new ArgumentException("El turno debe empezar y terminar en el mismo día.");
        }

        private void ValidarId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Seleccioná un turno válido.");
        }
    }
}
