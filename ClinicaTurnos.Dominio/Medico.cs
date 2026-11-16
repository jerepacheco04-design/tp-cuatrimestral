using System.Collections.Generic;

namespace ClinicaTurnos.Dominio
{
    public class Medico : Usuario
    {
        public string Matricula { get; set; }
        public List<Especialidad> Especialidades { get; set; }
        public TurnoTrabajo TurnoTrabajo { get; set; }

        public Medico()
        {
            Perfil = PerfilUsuario.Medico;
            Especialidades = new List<Especialidad>();
        }
    }
}
