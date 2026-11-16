using System;
using System.Data.SqlClient;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Pacientes : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Mensaje"] != null)
                {
                    lblMensaje.Text = Server.HtmlEncode((string)Session["Mensaje"]);
                    lblMensaje.CssClass = "aviso exito";
                    Session.Remove("Mensaje");
                }
                try
                {
                    grilla.DataSource = new PacienteNegocio().Listar();
                    grilla.DataBind();
                }
                catch (SqlException)
                {
                    lblMensaje.CssClass = "aviso";
                    lblMensaje.Text = "No se pudieron cargar los datos. Revisá la conexión a la base de datos.";
                }
            }
        }
    }
}
