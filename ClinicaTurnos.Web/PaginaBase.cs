using System;
using System.Web;
using System.Web.UI;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Web
{
    public class PaginaBase : Page
    {
        protected Usuario UsuarioActual
        {
            get { return Session["Usuario"] as Usuario; }
        }

        protected virtual bool RequiereSesion { get { return true; } }
        protected virtual bool EsPaginaMedico { get { return false; } }
        protected virtual bool PermiteMedico { get { return EsPaginaMedico; } }

        protected override void OnInit(EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Usuario usuario = UsuarioActual;
            if (RequiereSesion)
            {
                if (usuario == null)
                {
                    Response.Redirect("~/Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!PuedeAcceder(usuario, Request.AppRelativeCurrentExecutionFilePath) ||
                    (EsPaginaMedico && usuario.Perfil != PerfilUsuario.Medico) ||
                    (usuario.Perfil == PerfilUsuario.Medico && (!usuario.MedicoId.HasValue || !PermiteMedico)))
                {
                    Session["MensajeAcceso"] = "No tenés permiso para entrar a esa sección.";
                    Response.Redirect(usuario.Perfil == PerfilUsuario.Medico ? "~/MiAgenda.aspx" : "~/Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            if (Session["ClaveFormulario"] == null)
                Session["ClaveFormulario"] = Guid.NewGuid().ToString();
            ViewStateUserKey = (string)Session["ClaveFormulario"];
            base.OnInit(e);
        }

        private bool PuedeAcceder(Usuario usuario, string ruta)
        {
            if (usuario.Perfil == PerfilUsuario.Administrador)
                return true;

            if (usuario.Perfil == PerfilUsuario.Medico)
            {
                string[] paginasMedico =
                {
                    "~/MiAgenda.aspx",
                    "~/ConsultaTurno.aspx",
                    "~/HistorialPaciente.aspx"
                };
                foreach (string pagina in paginasMedico)
                    if (string.Equals(ruta, pagina, StringComparison.OrdinalIgnoreCase))
                        return true;
                return false;
            }

            if (usuario.Perfil != PerfilUsuario.Recepcionista)
                return false;

            string[] paginasRecepcion =
            {
                "~/Pacientes.aspx",
                "~/PacienteAlta.aspx",
                "~/Medicos.aspx",
                "~/MedicoAlta.aspx",
                "~/Turnos.aspx",
                "~/TurnoAlta.aspx",
                "~/HistorialPaciente.aspx"
            };

            foreach (string pagina in paginasRecepcion)
                if (string.Equals(ruta, pagina, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}