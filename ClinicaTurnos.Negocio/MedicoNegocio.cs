using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class MedicoNegocio
    {
        public List<Medico> Listar()
        {
            return new MedicoDatos().Listar();
        }

        public Medico Obtener(int id)
        {
            ValidarId(id);
            return new MedicoDatos().Listar(id).Find(medico => medico.Id == id);
        }

        public void Guardar(Medico medico)
        {
            if (medico == null)
                throw new ArgumentException("Ingresá los datos del médico.");
            medico.Matricula = (medico.Matricula ?? "").Trim();
            medico.Nombre = (medico.Nombre ?? "").Trim();
            medico.Apellido = (medico.Apellido ?? "").Trim();
            medico.Email = (medico.Email ?? "").Trim();

            if (medico.Id < 0)
                throw new ArgumentException("El médico seleccionado no es válido.");
            if (medico.Matricula.Length == 0 || medico.Matricula.Length > 30)
                throw new ArgumentException("Ingresá una matrícula de hasta 30 caracteres.");
            if (medico.Nombre.Length == 0 || medico.Nombre.Length > 80 || medico.Apellido.Length == 0 || medico.Apellido.Length > 80)
                throw new ArgumentException("Ingresá nombre y apellido de hasta 80 caracteres cada uno.");
            if (medico.Email.Length > 254 || !Regex.IsMatch(medico.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Ingresá un correo electrónico válido.");
            if (medico.TurnoTrabajo == null || medico.TurnoTrabajo.Id <= 0)
                throw new ArgumentException("Seleccioná un horario de trabajo.");
            if (medico.Especialidades == null || medico.Especialidades.Count == 0)
                throw new ArgumentException("Seleccioná al menos una especialidad.");

            try
            {
                new MedicoDatos().Guardar(medico);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new ArgumentException("Ya existe un médico con esa matrícula.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new ArgumentException("Alguna especialidad o el horario seleccionado ya no existe. Volvé a abrir el formulario.");
            }
        }

        public void Eliminar(int id)
        {
            ValidarId(id);
            new MedicoDatos().Eliminar(id);
        }

        private void ValidarId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El médico seleccionado no es válido.");
        }
    }
}
