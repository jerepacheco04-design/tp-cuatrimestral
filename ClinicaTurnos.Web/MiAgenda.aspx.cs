using System;
using System.Data.SqlClient;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class MiAgenda : PaginaBase
    {
        protected override bool EsPaginaMedico { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            if (Session["Mensaje"] is string mensaje)
            {
                lblMensaje.Text = Server.HtmlEncode(mensaje);
                lblMensaje.CssClass = "aviso exito";
                Session.Remove("Mensaje");
            }
            try
            {
                grilla.DataSource = new TurnoNegocio().ListarDelMedico(UsuarioActual.MedicoId.Value);
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudo cargar la agenda. Revisá la base de datos.";
            }
        }
    }
}
