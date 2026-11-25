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
                txtObservacion.Text = turno.ObservacionesDiagnostico ?? "";
                bool editable = turno.Estado != EstadoTurno.Cancelado && turno.Estado != EstadoTurno.NoAsistio;
                txtObservacion.ReadOnly = !editable;
                btnGuardar.Visible = editable;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar la consulta. Revisá la base de datos.";
                pnlConsulta.Visible = false;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!pnlConsulta.Visible || !btnGuardar.Visible || TurnoId == 0)
                return;
            try
            {
                new TurnoNegocio().GuardarObservacion(TurnoId, UsuarioActual.MedicoId.Value, txtObservacion.Text);
                Session["Mensaje"] = "Observación guardada.";
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
                lblMensaje.Text = "No se pudo guardar la observación. Intentá nuevamente.";
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
