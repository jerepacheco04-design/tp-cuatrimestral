using System;
using System.Collections.Generic;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class TurnoTrabajoNegocio
    {
        public List<TurnoTrabajo> Listar()
        {
            return new TurnoTrabajoDatos().Listar();
        }

        public TurnoTrabajo Obtener(int id)
        {
            ValidarId(id);
            return new TurnoTrabajoDatos().Obtener(id);
        }

        public void Agregar(TurnoTrabajo turno)
        {
            ValidarTurno(turno, 0);
            new TurnoTrabajoDatos().Agregar(turno);
        }

        public void Modificar(TurnoTrabajo turno)
        {
            if (turno == null)
                throw new ArgumentException("Ingresá los datos del horario.");
            ValidarId(turno.Id);
            ValidarTurno(turno, turno.Id);
            new TurnoTrabajoDatos().Modificar(turno);
        }

        public void Eliminar(int id)
        {
            ValidarId(id);
            new TurnoTrabajoDatos().Eliminar(id);
        }

        private void ValidarTurno(TurnoTrabajo turno, int idExcluir)
        {
            if (turno == null)
                throw new ArgumentException("Ingresá los datos del horario.");

            turno.Nombre = (turno.Nombre ?? "").Trim();
            if (turno.Nombre.Length == 0 || turno.Nombre.Length > 80)
                throw new ArgumentException("Ingresá un nombre de hasta 80 caracteres.");

            if (turno.Franjas == null || turno.Franjas.Count == 0)
                throw new ArgumentException("Seleccioná al menos un día de trabajo.");

            HashSet<DayOfWeek> dias = new HashSet<DayOfWeek>();
            foreach (FranjaHoraria franja in turno.Franjas)
            {
                if (franja == null || !Enum.IsDefined(typeof(DayOfWeek), franja.Dia))
                    throw new ArgumentException("Seleccioná días válidos.");
                if (franja.HoraEntrada < TimeSpan.Zero || franja.HoraSalida > TimeSpan.FromDays(1) || franja.HoraEntrada >= franja.HoraSalida)
                    throw new ArgumentException("La hora de entrada debe ser anterior a la hora de salida.");
                if (!dias.Add(franja.Dia))
                    throw new ArgumentException("No repitas días dentro del mismo horario.");
            }

            TurnoTrabajoDatos datos = new TurnoTrabajoDatos();
            if (datos.ExisteNombre(turno.Nombre, idExcluir))
                throw new ArgumentException("Ya existe un horario con ese nombre.");
        }

        private void ValidarId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El horario seleccionado no es válido.");
        }
    }
}
