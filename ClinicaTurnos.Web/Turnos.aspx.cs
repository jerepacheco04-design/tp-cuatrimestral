using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Turnos : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            if (IsPostBack)
                return;

            if (Session["Mensaje"] is string mensaje)
            {
                lblMensaje.Text = Server.HtmlEncode(mensaje);
                lblMensaje.CssClass = "aviso exito";
                Session.Remove("Mensaje");
            }
            CargarTurnos();
        }

        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "CancelarTurno")
                return;

            int turnoId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out turnoId))
            {
                MostrarMensaje("No se pudo identificar el turno.");
                CargarTurnos();
                return;
            }

            try
            {
                new TurnoNegocio().Cancelar(turnoId);
                Session["Mensaje"] = "Turno cancelado correctamente.";
                Response.Redirect("Turnos.aspx");
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message);
                CargarTurnos();
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje(ex.Message);
                CargarTurnos();
            }
            catch (SqlException)
            {
                MostrarMensaje("No se pudo cancelar el turno. Intentá nuevamente.");
                CargarTurnos();
            }
        }

        protected void grilla_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Turno turno = (Turno)e.Row.DataItem;
            LinkButton botonCancelar = (LinkButton)e.Row.FindControl("btnCancelar");
            HyperLink linkReprogramar = (HyperLink)e.Row.FindControl("linkReprogramar");
            botonCancelar.Visible = linkReprogramar.Visible = turno.PuedeCambiarAgenda(DateTime.Now);
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.CssClass = "aviso";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        private void CargarTurnos()
        {
            try
            {
                grilla.DataSource = new TurnoNegocio().Listar();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar los turnos. Ejecutá el script de base de datos y revisá la conexión.";
            }
        }
    }
}
