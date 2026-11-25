using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio.Datos
{
    internal class UsuarioDatos
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaTurnos"].ConnectionString;

        public Usuario Autenticar(string nombreUsuario, string clave)
        {
            const string consulta = @"SELECT Id, NombreUsuario, Nombre, Apellido, Email, Perfil, Activo, ClaveHash, ClaveSalt
                                     FROM dbo.Usuarios
                                     WHERE NombreUsuario = @NombreUsuario";

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 50).Value = nombreUsuario;
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read() || !(bool)lector["Activo"])
                        return null;

                    byte[] hashGuardado = (byte[])lector["ClaveHash"];
                    byte[] sal = (byte[])lector["ClaveSalt"];
                    if (!Coinciden(hashGuardado, CalcularHash(clave, sal)))
                        return null;

                    return new Usuario
                    {
                        Id = (int)lector["Id"],
                        NombreUsuario = (string)lector["NombreUsuario"],
                        Nombre = (string)lector["Nombre"],
                        Apellido = (string)lector["Apellido"],
                        Email = (string)lector["Email"],
                        Perfil = (PerfilUsuario)Convert.ToInt32(lector["Perfil"]),
                        Activo = (bool)lector["Activo"]
                    };
                }
            }
        }

        private byte[] CalcularHash(string clave, byte[] sal)
        {
            byte[] claveBytes = Encoding.Unicode.GetBytes(clave);
            byte[] entrada = new byte[sal.Length + claveBytes.Length];
            Buffer.BlockCopy(sal, 0, entrada, 0, sal.Length);
            Buffer.BlockCopy(claveBytes, 0, entrada, sal.Length, claveBytes.Length);
            using (SHA256 sha = SHA256.Create())
                return sha.ComputeHash(entrada);
        }

        private bool Coinciden(byte[] guardado, byte[] recibido)
        {
            if (guardado.Length != recibido.Length)
                return false;

            int diferencia = 0;
            for (int i = 0; i < guardado.Length; i++)
                diferencia |= guardado[i] ^ recibido[i];

            return diferencia == 0;
        }
    }
}
