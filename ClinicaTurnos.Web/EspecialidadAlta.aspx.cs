using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class EspecialidadAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            Especialidad dato = new Especialidad
            {
                Nombre = txtNombre.Text
            };
            try
            {
                new EspecialidadNegocio().Agregar(dato);
                Session["Mensaje"] = "Registro agregado correctamente.";
                Response.Redirect("Especialidades.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron guardar los cambios. Revisá la base de datos e intentá nuevamente.";
            }
        }
    }
}
