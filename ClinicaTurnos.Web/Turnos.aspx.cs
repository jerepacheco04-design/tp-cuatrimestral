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
            Usuario usuario = Session["Usuario"] as Usuario;
            bool esMedico = usuario != null && usuario.Perfil == PerfilUsuario.Medico;
            lblTitulo.Text = esMedico ? "Mis turnos" : "Turnos de pacientes";
            linkAgregar.Visible = !esMedico;
            grilla.Columns[8].Visible = !esMedico;

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

        protected string NombreEstado(object valor)
        {
            switch ((EstadoTurno)valor)
            {
                case EstadoTurno.Nuevo: return "Nuevo";
                case EstadoTurno.Reprogramado: return "Reprogramado";
                case EstadoTurno.Cancelado: return "Cancelado";
                case EstadoTurno.NoAsistio: return "No asistió";
                case EstadoTurno.Cerrado: return "Cerrado";
                default: return "";
            }
        }

        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GuardarDiagnostico")
            {
                GuardarDiagnostico(e);
                return;
            }

            if (e.CommandName != "CancelarTurno")
                return;

            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario == null || usuario.Perfil == PerfilUsuario.Medico)
            {
                MostrarMensaje("No tenés permiso para cancelar turnos.");
                CargarTurnos();
                return;
            }

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
                Response.Redirect("Turnos.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
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
            Usuario usuario = Session["Usuario"] as Usuario;
            bool esMedico = usuario != null && usuario.Perfil == PerfilUsuario.Medico;
            Label etiquetaDiagnostico = (Label)e.Row.FindControl("lblDiagnostico");
            TextBox campoDiagnostico = (TextBox)e.Row.FindControl("txtDiagnostico");
            LinkButton botonGuardarDiagnostico = (LinkButton)e.Row.FindControl("btnGuardarDiagnostico");
            LinkButton botonCancelar = (LinkButton)e.Row.FindControl("btnCancelar");
            HyperLink linkReprogramar = (HyperLink)e.Row.FindControl("linkReprogramar");

            etiquetaDiagnostico.Visible = !esMedico;
            campoDiagnostico.Visible = esMedico;
            botonGuardarDiagnostico.Visible = esMedico && turno.Estado != EstadoTurno.Cancelado;
            if (botonCancelar != null && linkReprogramar != null)
                botonCancelar.Visible = linkReprogramar.Visible = !esMedico && turno.PuedeCambiarAgenda(DateTime.Now);
        }

        private void GuardarDiagnostico(GridViewCommandEventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario == null || usuario.Perfil != PerfilUsuario.Medico || !usuario.MedicoId.HasValue)
            {
                MostrarMensaje("No tenés permiso para modificar este turno.");
                CargarTurnos();
                return;
            }

            int turnoId;
            GridViewRow fila = ((LinkButton)e.CommandSource).NamingContainer as GridViewRow;
            TextBox campoDiagnostico = fila == null ? null : (TextBox)fila.FindControl("txtDiagnostico");
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out turnoId) || campoDiagnostico == null)
            {
                MostrarMensaje("No se pudo guardar el diagnóstico.");
                CargarTurnos();
                return;
            }

            try
            {
                new TurnoNegocio().GuardarDiagnostico(turnoId, usuario.MedicoId.Value, campoDiagnostico.Text);
                Session["Mensaje"] = "Diagnóstico guardado correctamente.";
                Response.Redirect("Turnos.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
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
                MostrarMensaje("No se pudo guardar el diagnóstico. Intentá nuevamente.");
                CargarTurnos();
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.CssClass = "aviso";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        private void CargarTurnos()
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            bool esMedico = usuario != null && usuario.Perfil == PerfilUsuario.Medico;
            try
            {
                TurnoNegocio negocio = new TurnoNegocio();
                grilla.EmptyDataText = esMedico ? "Todavía no tenés turnos asignados." : "Todavía no hay turnos cargados.";
                grilla.DataSource = esMedico
                    ? negocio.ListarPorMedico(usuario.MedicoId.Value)
                    : negocio.Listar();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar los turnos.";
            }
        }
    }
}
