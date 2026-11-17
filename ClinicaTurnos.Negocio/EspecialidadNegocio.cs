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

        public Especialidad Obtener(int id)
        {
            ValidarId(id);
            return new EspecialidadDatos().Obtener(id);
        }

        public void Agregar(Especialidad especialidad)
        {
            if (especialidad == null)
            {
                throw new ArgumentException("Ingresá los datos de la especialidad.");
            }

            especialidad.Nombre = (especialidad.Nombre ?? "").Trim();
            ValidarNombre(especialidad.Nombre);

            EspecialidadDatos datos = new EspecialidadDatos();
            if (datos.ExisteNombre(especialidad.Nombre, 0))
            {
                throw new ArgumentException("Ya existe una especialidad con ese nombre.");
            }

            datos.Agregar(especialidad);
        }

        public void Modificar(Especialidad especialidad)
        {
            if (especialidad == null)
            {
                throw new ArgumentException("Ingresá los datos de la especialidad.");
            }

            ValidarId(especialidad.Id);
            especialidad.Nombre = (especialidad.Nombre ?? "").Trim();
            ValidarNombre(especialidad.Nombre);

            EspecialidadDatos datos = new EspecialidadDatos();
            if (datos.ExisteNombre(especialidad.Nombre, especialidad.Id))
            {
                throw new ArgumentException("Ya existe una especialidad con ese nombre.");
            }

            datos.Modificar(especialidad);
        }

        public void Eliminar(int id)
        {
            ValidarId(id);
            new EspecialidadDatos().Eliminar(id);
        }

        private void ValidarId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("La especialidad seleccionada no es válida.");
            }
        }

        private void ValidarNombre(string nombre)
        {
            if (nombre.Length == 0 || nombre.Length > 80)
            {
                throw new ArgumentException("Ingresá un nombre de especialidad de hasta 80 caracteres.");
            }
        }
    }
}
