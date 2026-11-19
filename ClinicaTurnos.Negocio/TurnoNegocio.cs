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
    }
}
