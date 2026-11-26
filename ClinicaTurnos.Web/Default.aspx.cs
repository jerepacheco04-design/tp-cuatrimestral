using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Inicio : PaginaBase
    {
        protected override bool RequiereSesion { get { return false; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = UsuarioActual;
            pnlLogin.Visible = usuario == null;
            pnlInicio.Visible = usuario != null;
            if (usuario != null)
            {
                lblBienvenida.Text = "Bienvenido, " + Server.HtmlEncode(usuario.Nombre + " " + usuario.Apellido + ".");
                lblDescripcionInicio.Text = usuario.Perfil == PerfilUsuario.Medico
                    ? "Desde Mi agenda podés consultar tus turnos y registrar observaciones."
                    : "Administración de la clínica.";
                lnkMiAgenda.Visible = usuario.Perfil == PerfilUsuario.Medico;
                pnlInicioGestion.Visible = !lnkMiAgenda.Visible;
                if (Session["MensajeAcceso"] is string mensaje)
                {
                    lblMensajeInicio.Text = Server.HtmlEncode(mensaje);
                    Session.Remove("MensajeAcceso");
                }
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            try
            {
                Usuario usuario = new UsuarioNegocio().IniciarSesion(txtUsuario.Text, txtClave.Text);
                if (usuario == null)
                {
                    lblMensaje.Text = "El usuario o la contraseña no son correctos.";
                    return;
                }

                Session.Clear();
                Session["Usuario"] = usuario;
                Session["ClaveFormulario"] = Guid.NewGuid().ToString();
                Response.Redirect(usuario.Perfil == PerfilUsuario.Medico ? "~/MiAgenda.aspx" : "~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo iniciar sesión. Intentá nuevamente más tarde.";
            }
        }
    }
}