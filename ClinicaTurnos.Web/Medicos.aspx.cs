using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Medicos : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            lblMensaje.CssClass = "aviso";
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
                grilla.DataSource = new MedicoNegocio().Listar();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar los médicos.";
            }
        }

        protected string MostrarEspecialidades(object valor)
        {
            List<Especialidad> especialidades = (List<Especialidad>)valor;
            return string.Join(", ", especialidades.Select(especialidad => especialidad.Nombre));
        }

        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar")
                return;

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0)
            {
                lblMensaje.Text = "El médico seleccionado no es válido.";
                return;
            }
            try
            {
                new MedicoNegocio().Eliminar(id);
                Session["Mensaje"] = "Médico eliminado correctamente.";
                Response.Redirect("Medicos.aspx");
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo eliminar el médico. Intentá nuevamente.";
            }
        }
    }
}
