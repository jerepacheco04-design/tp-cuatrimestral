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
            const string consulta = @"SELECT u.Id, u.NombreUsuario, COALESCE(m.Nombre, u.Nombre) AS Nombre,
                                            COALESCE(m.Apellido, u.Apellido) AS Apellido, COALESCE(m.Email, u.Email) AS Email,
                                            u.Perfil, u.Activo, u.ClaveHash, u.ClaveSalt, u.ClaveIteraciones, m.Id AS MedicoId
                                     FROM dbo.Usuarios u LEFT JOIN dbo.Medicos m ON m.UsuarioId = u.Id
                                     WHERE u.NombreUsuario = @NombreUsuario";

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
                    int iteraciones = lector["ClaveIteraciones"] == DBNull.Value ? 0 : (int)lector["ClaveIteraciones"];
                    PerfilUsuario perfil = (PerfilUsuario)Convert.ToInt32(lector["Perfil"]);
                    int? medicoId = lector["MedicoId"] == DBNull.Value ? (int?)null : (int)lector["MedicoId"];
                    if ((perfil == PerfilUsuario.Medico && !medicoId.HasValue) ||
                        !Coinciden(hashGuardado, CalcularHash(clave, sal, iteraciones)))
                        return null;

                    return new Usuario
                    {
                        Id = (int)lector["Id"],
                        NombreUsuario = (string)lector["NombreUsuario"],
                        Nombre = (string)lector["Nombre"],
                        Apellido = (string)lector["Apellido"],
                        Email = (string)lector["Email"],
                        Perfil = perfil,
                        MedicoId = medicoId,
                        Activo = (bool)lector["Activo"]
                    };
                }
            }
        }

        internal int CrearCuentaMedico(Medico medico, string clave, SqlConnection conexion, SqlTransaction transaccion)
        {
            const int iteraciones = 600000;
            byte[] sal = new byte[16];
            using (RandomNumberGenerator generador = RandomNumberGenerator.Create())
                generador.GetBytes(sal);
            const string consulta = @"INSERT INTO dbo.Usuarios (NombreUsuario, ClaveHash, ClaveSalt, ClaveIteraciones,
                                         Nombre, Apellido, Email, Perfil)
                                     OUTPUT INSERTED.Id
                                     VALUES (@Usuario, @Hash, @Salt, @Iteraciones, @Nombre, @Apellido, @Email, @Perfil)";
            using (SqlCommand comando = new SqlCommand(consulta, conexion, transaccion))
            {
                comando.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value = medico.NombreUsuario;
                comando.Parameters.Add("@Hash", SqlDbType.VarBinary, 32).Value = CalcularHash(clave, sal, iteraciones);
                comando.Parameters.Add("@Salt", SqlDbType.VarBinary, 16).Value = sal;
                comando.Parameters.Add("@Iteraciones", SqlDbType.Int).Value = iteraciones;
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 80).Value = medico.Nombre;
                comando.Parameters.Add("@Apellido", SqlDbType.NVarChar, 80).Value = medico.Apellido;
                comando.Parameters.Add("@Email", SqlDbType.NVarChar, 254).Value = medico.Email;
                comando.Parameters.Add("@Perfil", SqlDbType.TinyInt).Value = (byte)PerfilUsuario.Medico;
                try
                {
                    return (int)comando.ExecuteScalar();
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    throw new ArgumentException("Ya existe una cuenta con ese nombre de usuario.");
                }
            }
        }

        private byte[] CalcularHash(string clave, byte[] sal, int iteraciones)
        {
            if (iteraciones > 0)
            {
                using (Rfc2898DeriveBytes derivador = new Rfc2898DeriveBytes(clave, sal, iteraciones, HashAlgorithmName.SHA256))
                    return derivador.GetBytes(32);
            }

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
