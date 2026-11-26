using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class TurnoAlta : PaginaBase
    {
        private readonly TurnoNegocio negocio = new TurnoNegocio();
        private const string FormatoFecha = "yyyy-MM-dd";
        private const string FormatoHora = "HH:mm";
        private const string FormatoSugerencia = "yyyy-MM-dd HH:mm";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            txtFecha.Attributes["min"] = DateTime.Today.ToString(FormatoFecha, CultureInfo.InvariantCulture);
            if (TurnoId < 0)
            {
                lblMensaje.Text = "El turno seleccionado no es válido.";
                pnlFormulario.Visible = false;
                return;
            }
            if (TurnoId > 0)
            {
                Title = "Reprogramar turno";
                litTitulo.Text = "Reprogramar turno";
                btnGuardar.Text = "Guardar cambios";
                ddlPaciente.Enabled = false;
                ddlEspecialidad.Enabled = false;
                ddlMedico.Enabled = false;
                txtMotivo.ReadOnly = true;
            }
            if (IsPostBack)
                return;

            ddlPaciente.Items.Add(new ListItem("Seleccioná un paciente", ""));
            ddlEspecialidad.Items.Add(new ListItem("Seleccioná una especialidad", ""));
            ddlMedico.Items.Add(new ListItem("Seleccioná una especialidad primero", ""));
            ReiniciarHoras();
            try
            {
                if (TurnoId > 0)
                {
                    CargarTurno();
                    return;
                }
                foreach (Paciente paciente in new PacienteNegocio().Listar())
                    ddlPaciente.Items.Add(new ListItem(paciente.Apellido + ", " + paciente.Nombre + " (" + paciente.Dni + ")", paciente.Id.ToString()));
                foreach (Especialidad especialidad in new EspecialidadNegocio().Listar())
                    ddlEspecialidad.Items.Add(new ListItem(especialidad.Nombre, especialidad.Id.ToString()));

                if (ddlPaciente.Items.Count == 1)
                    lblMensaje.Text = "No hay pacientes cargados.";
                else if (ddlEspecialidad.Items.Count == 1)
                    lblMensaje.Text = "No hay especialidades cargadas.";
                btnGuardar.Enabled = ddlPaciente.Items.Count > 1 && ddlEspecialidad.Items.Count > 1;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo cargar el formulario del turno.";
                btnGuardar.Enabled = false;
            }
        }

        private void CargarTurno()
        {
            Turno turno = negocio.Obtener(TurnoId);
            if (turno == null || !turno.PuedeCambiarAgenda(DateTime.Now))
            {
                lblMensaje.Text = "El turno no existe o ya no se puede reprogramar.";
                pnlFormulario.Visible = false;
                return;
            }
            ViewState["FechaAnterior"] = turno.FechaHora;
            lblTurnoActual.Text = "Turno " + turno.Id + " – Fecha actual: " + turno.FechaHora.ToString("dd/MM/yyyy HH:mm");
            ddlPaciente.Items.Clear();
            ddlPaciente.Items.Add(new ListItem(turno.Paciente.Apellido + ", " + turno.Paciente.Nombre, turno.Paciente.Id.ToString()));
            ddlEspecialidad.Items.Clear();
            ddlEspecialidad.Items.Add(new ListItem(turno.Especialidad.Nombre, turno.Especialidad.Id.ToString()));
            ddlMedico.Items.Clear();
            ddlMedico.Items.Add(new ListItem(turno.Medico.Apellido + ", " + turno.Medico.Nombre, turno.Medico.Id.ToString()));
            txtMotivo.Text = turno.MotivoConsulta;
            txtFecha.Text = turno.FechaHora.ToString(FormatoFecha, CultureInfo.InvariantCulture);
            CargarHoras();
        }

        protected void ddlPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSugerencias();
            CargarHoras();
        }

        protected void ddlEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlMedico.Items.Clear();
            ddlMedico.Items.Add(new ListItem("Seleccioná un médico", ""));
            ReiniciarHoras();
            pnlSugerencias.Visible = false;
            int especialidad = LeerId(ddlEspecialidad);
            if (especialidad == 0)
                return;

            try
            {
                foreach (Medico medico in negocio.ListarMedicos(especialidad))
                    ddlMedico.Items.Add(new ListItem(medico.Apellido + ", " + medico.Nombre, medico.Id.ToString()));
                if (ddlMedico.Items.Count == 1)
                    lblMensaje.Text = "Todavía no hay médicos para esta especialidad.";
                else
                    CargarSugerencias();
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron cargar los médicos.";
            }
        }

        private void CargarSugerencias()
        {
            ddlSugerencia.Items.Clear();
            pnlSugerencias.Visible = false;
            int paciente = LeerId(ddlPaciente);
            int especialidad = LeerId(ddlEspecialidad);
            if (TurnoId != 0 || paciente == 0 || especialidad == 0)
                return;

            try
            {
                foreach (TurnoOpcion opcion in negocio.Sugerir(paciente, especialidad))
                {
                    string texto = opcion.FechaHora.ToString("dd/MM/yyyy HH:mm") + " – " + opcion.Medico.Apellido + ", " + opcion.Medico.Nombre;
                    string valor = opcion.Medico.Id + "|" + opcion.FechaHora.ToString(FormatoSugerencia, CultureInfo.InvariantCulture);
                    ddlSugerencia.Items.Add(new ListItem(texto, valor));
                }
                pnlSugerencias.Visible = ddlSugerencia.Items.Count > 0;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron buscar horarios disponibles.";
            }
        }

        protected void btnUsarSugerencia_Click(object sender, EventArgs e)
        {
            string[] valor = ddlSugerencia.SelectedValue.Split('|');
            DateTime fecha;
            if (TurnoId != 0 || valor.Length != 2 || ddlMedico.Items.FindByValue(valor[0]) == null ||
                !DateTime.TryParseExact(valor[1], FormatoSugerencia, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                return;

            ddlMedico.SelectedValue = valor[0];
            txtFecha.Text = fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture);
            string hora = fecha.ToString(FormatoHora, CultureInfo.InvariantCulture);
            CargarHoras(hora);
            if (ddlHora.SelectedValue != hora)
            {
                lblMensaje.Text = "Ese horario ya no está disponible.";
                CargarSugerencias();
            }
        }

        protected void ActualizarHorarios(object sender, EventArgs e)
        {
            CargarHoras();
        }

        private void ReiniciarHoras()
        {
            ddlHora.Items.Clear();
            ddlHora.Items.Add(new ListItem("Seleccioná una hora", ""));
            ddlHora.Enabled = false;
        }

        private void CargarHoras(string seleccionar = "")
        {
            ReiniciarHoras();
            int paciente = LeerId(ddlPaciente);
            int especialidad = LeerId(ddlEspecialidad);
            int medico = LeerId(ddlMedico);
            DateTime fecha;
            if (paciente == 0 || especialidad == 0 || medico == 0 ||
                !DateTime.TryParseExact(txtFecha.Text, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                return;

            try
            {
                foreach (TurnoOpcion opcion in negocio.ListarHorarios(paciente, especialidad, medico, fecha, TurnoId))
                {
                    string hora = opcion.FechaHora.ToString(FormatoHora, CultureInfo.InvariantCulture);
                    ddlHora.Items.Add(new ListItem(hora, hora));
                }
                ddlHora.Enabled = ddlHora.Items.Count > 1;
                if (!ddlHora.Enabled)
                    lblMensaje.Text = "No hay horarios disponibles para ese día.";
                else if (ddlHora.Items.FindByValue(seleccionar) != null)
                    ddlHora.SelectedValue = seleccionar;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron cargar los horarios disponibles.";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (TurnoId < 0 || !btnGuardar.Enabled || !pnlFormulario.Visible)
                return;

            DateTime fechaHora;
            if (!DateTime.TryParseExact(txtFecha.Text + " " + ddlHora.SelectedValue, FormatoSugerencia,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaHora))
            {
                lblMensaje.Text = "Elegí una fecha y un horario disponible.";
                return;
            }
            try
            {
                if (TurnoId > 0)
                {
                    if (!(ViewState["FechaAnterior"] is DateTime fechaAnterior))
                        return;
                    negocio.Reprogramar(TurnoId, fechaAnterior, fechaHora);
                    Session["Mensaje"] = "Turno reprogramado correctamente.";
                }
                else
                {
                    Turno turno = new Turno
                    {
                        Paciente = new Paciente { Id = LeerId(ddlPaciente) },
                        Especialidad = new Especialidad { Id = LeerId(ddlEspecialidad) },
                        Medico = new Medico { Id = LeerId(ddlMedico) },
                        Recepcionista = UsuarioActual,
                        FechaHora = fechaHora,
                        MotivoConsulta = txtMotivo.Text
                    };
                    negocio.Agregar(turno);
                    Session["Mensaje"] = "Turno agregado correctamente. Número " + turno.Id + ".";
                }
                Response.Redirect("Turnos.aspx");
            }
            catch (ArgumentException ex)
            {
                CargarHoras(ddlHora.SelectedValue);
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                lblMensaje.Text = Server.HtmlEncode(ex.Message);
                btnGuardar.Enabled = false;
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudo guardar el turno. Intentá nuevamente.";
            }
        }

        private int TurnoId
        {
            get
            {
                string valor = Request.QueryString["id"];
                if (valor == null)
                    return 0;
                int id;
                return int.TryParse(valor, out id) && id > 0 ? id : -1;
            }
        }

        private int LeerId(DropDownList lista)
        {
            int id;
            return int.TryParse(lista.SelectedValue, out id) && id > 0 ? id : 0;
        }
    }
}
