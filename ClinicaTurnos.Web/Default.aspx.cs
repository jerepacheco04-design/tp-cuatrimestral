using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Inicio : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            pnlLogin.Visible = usuario == null;
            pnlInicio.Visible = usuario != null;
            if (usuario != null)
            {
                lblBienvenida.Text = "Bienvenido, " + Server.HtmlEncode(usuario.Nombre + " " + usuario.Apellido + ".");
                lblDescripcionInicio.Text = usuario.Perfil == PerfilUsuario.Medico
                    ? "Desde Mis turnos podés consultar tu agenda y completar el diagnóstico."
                    : "Administración de la clínica.";
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
                Response.Redirect("~/Default.aspx", false);
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
