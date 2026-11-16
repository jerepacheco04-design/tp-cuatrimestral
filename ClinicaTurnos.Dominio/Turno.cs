using System;

namespace ClinicaTurnos.Dominio
{
    public class Turno
    {
        public int Id { get; set; }
        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
        public Usuario Recepcionista { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoTurno Estado { get; set; }
        public string MotivoConsulta { get; set; }
        public string ObservacionesDiagnostico { get; set; }
    }
}
