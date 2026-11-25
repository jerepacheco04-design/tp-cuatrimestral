using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class ConsultaTurno : PaginaBase
    {
        protected override bool EsPaginaMedico { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            if (TurnoId == 0)
            {
                OcultarConsulta();
                return;
            }
            try
            {
                Turno turno = new TurnoNegocio().ObtenerDelMedico(TurnoId, UsuarioActual.MedicoId.Value);
                if (turno == null)
                {
                    OcultarConsulta();
                    return;
                }
                lblPaciente.Text = Server.HtmlEncode(turno.Paciente.Nombre + " " + turno.Paciente.Apellido);
                lblTurno.Text = Server.HtmlEncode("Turno " + turno.Id + " – " + turno.FechaHora.ToString("dd/MM/yyyy HH:mm") +
                    " – " + turno.Especialidad.Nombre + " – " + turno.EstadoVisible);
                lblMotivo.Text = Server.HtmlEncode("Motivo: " + turno.MotivoConsulta);
                lnkHistorial.NavigateUrl = "HistorialPaciente.aspx?turnoId=" + turno.Id;
                txtObservacion.Text = turno.ObservacionesDiagnostico ?? "";
                txtObservacion.ReadOnly = !turno.EstaPendiente;
                btnGuardar.Visible = turno.EstaPendiente;
                btnCerrar.Visible = btnAusente.Visible = turno.EstaPendiente && turno.FechaHora <= DateTime.Now;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar la consulta. Revisá la base de datos.";
                pnlConsulta.Visible = false;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (btnGuardar.Visible)
                GuardarConsulta(null);
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            if (btnCerrar.Visible)
                GuardarConsulta(EstadoTurno.Cerrado);
        }

        protected void btnAusente_Click(object sender, EventArgs e)
        {
            if (btnAusente.Visible)
                GuardarConsulta(EstadoTurno.NoAsistio);
        }

        private void GuardarConsulta(EstadoTurno? estado)
        {
            if (!pnlConsulta.Visible || TurnoId == 0)
                return;

            try
            {
                TurnoNegocio negocio = new TurnoNegocio();
                int medicoId = UsuarioActual.MedicoId.Value;
                if (estado == EstadoTurno.Cerrado)
                {
                    negocio.CerrarConsulta(TurnoId, medicoId, txtObservacion.Text);
                    Session["Mensaje"] = "Consulta cerrada.";
                }
                else if (estado == EstadoTurno.NoAsistio)
                {
                    negocio.MarcarAusencia(TurnoId, medicoId);
                    Session["Mensaje"] = "Turno marcado como No asistió.";
                }
                else
                {
                    negocio.GuardarObservacion(TurnoId, medicoId, txtObservacion.Text);
                    Session["Mensaje"] = "Observación guardada.";
                }
                Response.Redirect("MiAgenda.aspx");
            }
            catch (ArgumentException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (InvalidOperationException)
            {
                OcultarConsulta();
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo guardar la consulta. Intentá nuevamente.";
            }
        }

        private void OcultarConsulta()
        {
            lblMensaje.Text = "La consulta no está disponible.";
            pnlConsulta.Visible = false;
        }

        private int TurnoId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) && id > 0 ? id : 0;
            }
        }
    }
}
