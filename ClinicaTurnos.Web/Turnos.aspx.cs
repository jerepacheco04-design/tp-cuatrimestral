using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using System.Linq;
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

        protected void Filtrar_Click(object sender, EventArgs e)
        {
            if (sender == btnLimpiar)
            {
                txtFecha.Text = "";
                ddlEstado.SelectedIndex = 0;
            }

            lblMensaje.Text = "";
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
                DateTime fecha;
                bool filtrarFecha = DateTime.TryParseExact(txtFecha.Text, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
                if (txtFecha.Text.Length > 0 && !filtrarFecha)
                {
                    MostrarMensaje("Fecha no valida.");
                    return;
                }

                IEnumerable<Turno> turnos = new TurnoNegocio().Listar();
                if (filtrarFecha)
                    turnos = turnos.Where(turno => turno.FechaHora.Date == fecha);

                EstadoTurno estado;
                if (Enum.TryParse(ddlEstado.SelectedValue, out estado))
                    turnos = turnos.Where(turno => turno.Estado == estado);

                grilla.DataSource = turnos.ToList();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar los turnos. Intentá nuevamente más tarde.";
            }
        }
    }
}
