using System;

namespace ClinicaTurnos.Dominio
{
    public class FranjaHoraria
    {
        public int Id { get; set; }
        public DayOfWeek Dia { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
    }
}
