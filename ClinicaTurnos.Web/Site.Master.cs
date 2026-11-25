using System;
using System.Web.UI;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Web
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            navPrincipal.Visible = usuario != null;
            lnkMiAgenda.Visible = usuario != null && usuario.Perfil == PerfilUsuario.Medico;
            pnlGestion.Visible = usuario != null && usuario.Perfil != PerfilUsuario.Medico;
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
