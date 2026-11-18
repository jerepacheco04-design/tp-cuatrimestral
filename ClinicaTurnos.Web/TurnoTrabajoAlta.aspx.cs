using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class TurnoTrabajoAlta : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            int id = IdEdicion;
            if (id < 0)
            {
                lblMensaje.Text = "El horario seleccionado no es válido.";
                btnGuardar.Enabled = false;
                return;
            }

            if (id == 0)
            {
                litTitulo.Text = "Agregar horario";
                btnGuardar.Text = "Guardar";
                return;
            }

            litTitulo.Text = "Modificar horario";
            btnGuardar.Text = "Guardar cambios";
            if (!IsPostBack)
            {
                try
                {
                    TurnoTrabajo turno = new TurnoTrabajoNegocio().Obtener(id);
                    if (turno == null)
                    {
                        lblMensaje.Text = "No se encontró el horario seleccionado.";
                        btnGuardar.Enabled = false;
                        return;
                    }

                    txtNombre.Text = turno.Nombre;
                    foreach (FranjaHoraria franja in turno.Franjas)
                        CargarFranja(franja);
                }
                catch (SqlException)
                {
                    lblMensaje.Text = "No se pudo cargar el horario. Ejecutá el script de base de datos y revisá la conexión.";
                    btnGuardar.Enabled = false;
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int id = IdEdicion;
            if (id < 0 || !btnGuardar.Enabled)
                return;

            Page.Validate();
            if (!Page.IsValid)
                return;

            try
            {
                TurnoTrabajo turno = CrearTurno(id);
                TurnoTrabajoNegocio negocio = new TurnoTrabajoNegocio();
                if (id == 0)
                {
                    negocio.Agregar(turno);
                    Session["Mensaje"] = "Horario agregado correctamente.";
                }
                else
                {
                    negocio.Modificar(turno);
                    Session["Mensaje"] = "Horario modificado correctamente.";
                }

                Response.Redirect("TurnosTrabajo.aspx");
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
                lblMensaje.Text = "No se pudieron guardar los cambios. Ejecutá el script de base de datos y revisá la conexión.";
            }
        }

        private TurnoTrabajo CrearTurno(int id)
        {
            TurnoTrabajo turno = new TurnoTrabajo
            {
                Id = id,
                Nombre = txtNombre.Text
            };

            AgregarFranja(turno, chkLunes, txtLunesEntrada, txtLunesSalida, DayOfWeek.Monday);
            AgregarFranja(turno, chkMartes, txtMartesEntrada, txtMartesSalida, DayOfWeek.Tuesday);
            AgregarFranja(turno, chkMiercoles, txtMiercolesEntrada, txtMiercolesSalida, DayOfWeek.Wednesday);
            AgregarFranja(turno, chkJueves, txtJuevesEntrada, txtJuevesSalida, DayOfWeek.Thursday);
            AgregarFranja(turno, chkViernes, txtViernesEntrada, txtViernesSalida, DayOfWeek.Friday);
            AgregarFranja(turno, chkSabado, txtSabadoEntrada, txtSabadoSalida, DayOfWeek.Saturday);
            AgregarFranja(turno, chkDomingo, txtDomingoEntrada, txtDomingoSalida, DayOfWeek.Sunday);

            return turno;
        }

        private void AgregarFranja(TurnoTrabajo turno, CheckBox checkDia, TextBox entradaTexto, TextBox salidaTexto, DayOfWeek dia)
        {
            if (!checkDia.Checked)
                return;

            TimeSpan entrada;
            TimeSpan salida;
            if (!TimeSpan.TryParse(entradaTexto.Text, CultureInfo.InvariantCulture, out entrada) ||
                !TimeSpan.TryParse(salidaTexto.Text, CultureInfo.InvariantCulture, out salida))
                throw new ArgumentException("Completá una hora de entrada y salida válidas para cada día seleccionado.");

            turno.Franjas.Add(new FranjaHoraria
            {
                Dia = dia,
                HoraEntrada = entrada,
                HoraSalida = salida
            });
        }

        private void CargarFranja(FranjaHoraria franja)
        {
            switch (franja.Dia)
            {
                case DayOfWeek.Monday: CargarFranja(chkLunes, txtLunesEntrada, txtLunesSalida, franja); break;
                case DayOfWeek.Tuesday: CargarFranja(chkMartes, txtMartesEntrada, txtMartesSalida, franja); break;
                case DayOfWeek.Wednesday: CargarFranja(chkMiercoles, txtMiercolesEntrada, txtMiercolesSalida, franja); break;
                case DayOfWeek.Thursday: CargarFranja(chkJueves, txtJuevesEntrada, txtJuevesSalida, franja); break;
                case DayOfWeek.Friday: CargarFranja(chkViernes, txtViernesEntrada, txtViernesSalida, franja); break;
                case DayOfWeek.Saturday: CargarFranja(chkSabado, txtSabadoEntrada, txtSabadoSalida, franja); break;
                case DayOfWeek.Sunday: CargarFranja(chkDomingo, txtDomingoEntrada, txtDomingoSalida, franja); break;
            }
        }

        private void CargarFranja(CheckBox checkDia, TextBox entradaTexto, TextBox salidaTexto, FranjaHoraria franja)
        {
            checkDia.Checked = true;
            entradaTexto.Text = franja.HoraEntrada.ToString(@"hh\:mm");
            salidaTexto.Text = franja.HoraSalida.ToString(@"hh\:mm");
        }

        private int IdEdicion
        {
            get
            {
                string valor = Request.QueryString["id"];
                if (String.IsNullOrWhiteSpace(valor))
                    return 0;

                int id;
                return int.TryParse(valor, out id) && id > 0 ? id : -1;
            }
        }
    }
}
