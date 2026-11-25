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

        protected override void OnInit(EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Usuario usuario = UsuarioActual;
            if (RequiereSesion)
            {
                if (usuario == null)
                    Response.Redirect("~/Default.aspx");
                if (EsPaginaMedico)
                {
                    if (usuario.Perfil != PerfilUsuario.Medico || !usuario.MedicoId.HasValue)
                        Response.Redirect("~/Default.aspx");
                }
                else if (usuario.Perfil == PerfilUsuario.Medico)
                    Response.Redirect("~/MiAgenda.aspx");
            }

            if (Session["ClaveFormulario"] == null)
                Session["ClaveFormulario"] = Guid.NewGuid().ToString();
            ViewStateUserKey = (string)Session["ClaveFormulario"];
            base.OnInit(e);
        }
    }
}
