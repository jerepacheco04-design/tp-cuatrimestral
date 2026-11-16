using System;
using System.Collections.Generic;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class EspecialidadNegocio
    {
        public List<Especialidad> Listar()
        {
            return new EspecialidadDatos().Listar();
        }

        public void Agregar(Especialidad especialidad)
        {
            especialidad.Nombre = (especialidad.Nombre ?? "").Trim();
            if (especialidad.Nombre.Length == 0 || especialidad.Nombre.Length > 80)
                throw new ArgumentException("Ingresá un nombre de especialidad de hasta 80 caracteres.");
            EspecialidadDatos datos = new EspecialidadDatos();
            if (datos.ExisteNombre(especialidad.Nombre))
                throw new ArgumentException("Ya existe una especialidad con ese nombre.");
            datos.Agregar(especialidad);
        }

    }
}
