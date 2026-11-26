using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Negocio
{
    public class CorreoNegocio
    {
        public bool EnviarConfirmacion(Turno turno)
        {
            string usuario = ConfigurationManager.AppSettings["CorreoUsuario"];
            string clave = ConfigurationManager.AppSettings["CorreoClave"];
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
                return false;

            try
            {
                Paciente paciente = new PacienteNegocio().Obtener(turno.Paciente.Id);
                Medico medico = new MedicoNegocio().Obtener(turno.Medico.Id);
                if (paciente == null || string.IsNullOrWhiteSpace(paciente.Email) || medico == null)
                    return false;

                using (MailMessage mensaje = new MailMessage())
                using (SmtpClient correo = new SmtpClient())
                {
                    mensaje.To.Add(new MailAddress(paciente.Email));
                    mensaje.Subject = "Confirmacion de turno";
                    mensaje.BodyEncoding = Encoding.UTF8;
                    mensaje.Body = "Numero de turno: " + turno.Id + "\r\n"
                        + "Fecha: " + turno.FechaHora.ToString("dd/MM/yyyy") + "\r\n"
                        + "Hora: " + turno.FechaHora.ToString("HH:mm") + "\r\n"
                        + "Medico: " + medico.Nombre + " " + medico.Apellido;

                    correo.Credentials = new NetworkCredential(usuario, clave);
                    correo.Timeout = 5000;
                    correo.Send(mensaje);
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (SmtpException)
            {
                return false;
            }
        }
    }
}
