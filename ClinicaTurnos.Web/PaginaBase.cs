using System;
using System.Web.UI;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Web
{
    public class PaginaBase : Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["ClaveFormulario"] == null)
                Session["ClaveFormulario"] = Guid.NewGuid().ToString();
            ViewStateUserKey = (string)Session["ClaveFormulario"];
            base.OnInit(e);

            bool esInicio = string.Equals(Request.AppRelativeCurrentExecutionFilePath, "~/Default.aspx", StringComparison.OrdinalIgnoreCase);
            Usuario usuario = Session["Usuario"] as Usuario;
            if (!esInicio && usuario == null)
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!esInicio && !PuedeAcceder(usuario, Request.AppRelativeCurrentExecutionFilePath))
            {
                Session["MensajeAcceso"] = "No tenés permiso para entrar a esa sección.";
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        private bool PuedeAcceder(Usuario usuario, string ruta)
        {
            if (usuario.Perfil == PerfilUsuario.Administrador)
                return true;

            if (usuario.Perfil == PerfilUsuario.Medico)
                return usuario.MedicoId.HasValue &&
                    string.Equals(ruta, "~/Turnos.aspx", StringComparison.OrdinalIgnoreCase);

            if (usuario.Perfil != PerfilUsuario.Recepcionista)
                return false;

            string[] paginasRecepcion =
            {
                "~/Pacientes.aspx",
                "~/PacienteAlta.aspx",
                "~/Medicos.aspx",
                "~/MedicoAlta.aspx",
                "~/Turnos.aspx",
                "~/TurnoAlta.aspx"
            };

            foreach (string pagina in paginasRecepcion)
            {
                if (string.Equals(ruta, pagina, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
