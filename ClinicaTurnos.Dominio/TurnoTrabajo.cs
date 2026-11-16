using System.Collections.Generic;

namespace ClinicaTurnos.Dominio
{
    public class TurnoTrabajo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<FranjaHoraria> Franjas { get; set; }

        public TurnoTrabajo()
        {
            Franjas = new List<FranjaHoraria>();
        }
    }
}
