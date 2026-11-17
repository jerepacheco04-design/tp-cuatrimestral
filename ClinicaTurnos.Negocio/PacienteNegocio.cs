using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class PacienteNegocio
    {
        public List<Paciente> Listar()
        {
            return new PacienteDatos().Listar();
        }

        public Paciente Obtener(int id)
        {
            ValidarId(id);
            return new PacienteDatos().Obtener(id);
        }

        public void Agregar(Paciente paciente)
        {
            ValidarPaciente(paciente, 0);
            new PacienteDatos().Agregar(paciente);
        }

        public void Modificar(Paciente paciente)
        {
            if (paciente == null) throw new ArgumentException("El paciente es obligatorio.");
            ValidarId(paciente.Id);
            ValidarPaciente(paciente, paciente.Id);
            new PacienteDatos().Modificar(paciente);
        }

        public void Eliminar(int id)
        {
            ValidarId(id);
            new PacienteDatos().Eliminar(id);
        }

        private void ValidarId(int id)
        {
            if (id <= 0) throw new ArgumentException("El paciente seleccionado no es válido.");
        }

        private void ValidarPaciente(Paciente paciente, int idExcluir)
        {
            if (paciente == null) throw new ArgumentException("El paciente es obligatorio.");
            paciente.Dni = (paciente.Dni ?? "").Trim();
            paciente.Nombre = (paciente.Nombre ?? "").Trim();
            paciente.Apellido = (paciente.Apellido ?? "").Trim();
            paciente.Telefono = (paciente.Telefono ?? "").Trim();
            paciente.Email = (paciente.Email ?? "").Trim();
            paciente.Direccion = (paciente.Direccion ?? "").Trim();
            if (!Regex.IsMatch(paciente.Dni, @"^[0-9]{1,20}$"))
                throw new ArgumentException("El DNI es obligatorio y debe contener solo números, hasta 20 dígitos.");
            if (paciente.Nombre.Length == 0 || paciente.Nombre.Length > 80 || !Regex.IsMatch(paciente.Nombre, @"^[\p{L} ]+$"))
                throw new ArgumentException("Ingresá un nombre de hasta 80 caracteres, con letras y espacios.");
            if (paciente.Apellido.Length == 0 || paciente.Apellido.Length > 80 || !Regex.IsMatch(paciente.Apellido, @"^[\p{L} ]+$"))
                throw new ArgumentException("Ingresá un apellido de hasta 80 caracteres, con letras y espacios.");
            if (paciente.FechaNacimiento.Date > DateTime.Today || paciente.FechaNacimiento == DateTime.MinValue)
                throw new ArgumentException("Ingresá una fecha de nacimiento válida que no sea futura.");
            if (paciente.Telefono.Length > 30 || !Regex.IsMatch(paciente.Telefono, @"^[0-9]*$"))
                throw new ArgumentException("El teléfono debe contener solo números, hasta 30 dígitos.");
            if (paciente.Email.Length > 254 || !Regex.IsMatch(paciente.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Ingresá un correo electrónico válido.");
            if (paciente.Direccion.Length > 250)
                throw new ArgumentException("La dirección admite hasta 250 caracteres.");
            PacienteDatos datos = new PacienteDatos();
            if (datos.ExisteDni(paciente.Dni, idExcluir))
                throw new ArgumentException("Ya existe un paciente con ese DNI.");
        }

    }
}
