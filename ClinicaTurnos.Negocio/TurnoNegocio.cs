using System;
using System.Collections.Generic;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class TurnoNegocio
    {
        public List<Turno> Listar()
        {
            return new TurnoDatos().Listar();
        }

        public List<Medico> ListarMedicos(int especialidadId)
        {
            return new MedicoNegocio().Listar().FindAll(medico =>
                medico.Especialidades.Exists(especialidad => especialidad.Id == especialidadId));
        }

        public void Agregar(Turno turno)
        {
            if (turno == null)
                throw new ArgumentException("Completá los datos del turno.");
            if (turno.Paciente == null || turno.Paciente.Id <= 0)
                throw new ArgumentException("Seleccioná un paciente.");
            if (turno.Especialidad == null || turno.Especialidad.Id <= 0)
                throw new ArgumentException("Seleccioná una especialidad.");
            if (turno.Medico == null || turno.Medico.Id <= 0)
                throw new ArgumentException("Seleccioná un médico.");
            if (turno.FechaHora <= DateTime.Now)
                throw new ArgumentException("El turno debe ser en una fecha y hora futuras.");
            if (turno.FechaHora.TimeOfDay >= TimeSpan.FromHours(23))
                throw new ArgumentException("El turno debe empezar y terminar en el mismo día.");

            turno.MotivoConsulta = (turno.MotivoConsulta ?? "").Trim();
            if (turno.MotivoConsulta.Length == 0 || turno.MotivoConsulta.Length > 500)
                throw new ArgumentException("Ingresá un motivo de consulta de hasta 500 caracteres.");

            new TurnoDatos().Agregar(turno);
        }
    }
}
