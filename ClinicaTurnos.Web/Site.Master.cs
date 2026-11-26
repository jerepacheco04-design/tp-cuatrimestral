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
            if (usuario == null)
                return;

            bool esAdministrador = usuario.Perfil == PerfilUsuario.Administrador;
            bool esRecepcionista = usuario.Perfil == PerfilUsuario.Recepcionista;
            bool esMedico = usuario.Perfil == PerfilUsuario.Medico;
            linkInicio.Visible = true;
            linkPacientes.Visible = esAdministrador || esRecepcionista;
            linkEspecialidades.Visible = esAdministrador;
            linkHorarios.Visible = esAdministrador;
            linkMedicos.Visible = esAdministrador || esRecepcionista;
            linkTurnos.Visible = esAdministrador || esRecepcionista;
            lnkMiAgenda.Visible = esMedico && usuario.MedicoId.HasValue;
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