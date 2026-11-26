using System;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio.Datos;

namespace ClinicaTurnos.Negocio
{
    public class UsuarioNegocio
    {
        public Usuario IniciarSesion(string nombreUsuario, string clave)
        {
            nombreUsuario = (nombreUsuario ?? "").Trim();
            if (nombreUsuario.Length == 0 || string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Ingresá el usuario y la contraseña.");

            if (nombreUsuario.Length > 50 || clave.Length > 100)
                return null;
            return new UsuarioDatos().Autenticar(nombreUsuario, clave);
        }
    }
}
