using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Especialidades : PaginaBase
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

                CargarEspecialidades();
            }
        }

        protected void Filtrar_Click(object sender, EventArgs e)
        {
            if (sender == btnLimpiar)
                txtBuscar.Text = "";

            lblMensaje.Text = "";
            CargarEspecialidades();
        }

        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar")
            {
                return;
            }

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0)
            {
                lblMensaje.Text = "No se pudo identificar la especialidad seleccionada.";
                return;
            }

            try
            {
                new EspecialidadNegocio().Eliminar(id);
                Session["Mensaje"] = "Especialidad eliminada correctamente.";
                Response.Redirect("Especialidades.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException ex)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = ex.Number == 547
                    ? "No se puede eliminar la especialidad porque tiene médicos asignados."
                    : "No se pudo eliminar la especialidad. Intentá nuevamente.";
            }
        }

        private void CargarEspecialidades()
        {
            try
            {
                string busqueda = txtBuscar.Text.Trim();
                grilla.DataSource = new EspecialidadNegocio().Listar()
                    .Where(especialidad => especialidad.Nombre.IndexOf(busqueda, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar las especialidades.";
            }
        }
    }
}
