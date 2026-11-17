using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
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
        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar") return;
            lblMensaje.CssClass = "aviso";
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0)
            {
                lblMensaje.Text = "El paciente seleccionado no es válido.";
                return;
            }
            try
            {
                new PacienteNegocio().Eliminar(id);
                Session["Mensaje"] = "Registro eliminado correctamente.";
                Response.Redirect("Pacientes.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo eliminar el paciente. Revisá la base de datos e intentá nuevamente.";
            }
        }
    }
}
