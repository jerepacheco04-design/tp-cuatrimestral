using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class TurnosTrabajo : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            if (!IsPostBack)
            {
                if (Session["Mensaje"] != null)
                {
                    lblMensaje.Text = Server.HtmlEncode((string)Session["Mensaje"]);
                    lblMensaje.CssClass = "aviso exito";
                    Session.Remove("Mensaje");
                }

                CargarHorarios();
            }
        }

        protected void grilla_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar")
                return;

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id) || id <= 0)
            {
                lblMensaje.Text = "El horario seleccionado no es válido.";
                return;
            }

            try
            {
                new TurnoTrabajoNegocio().Eliminar(id);
                Session["Mensaje"] = "Horario eliminado correctamente.";
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
            catch (SqlException ex)
            {
                lblMensaje.Text = ex.Number == 547
                    ? "No se puede eliminar el horario porque tiene médicos asignados."
                    : "No se pudo eliminar el horario. Intentá nuevamente.";
            }
        }

        protected string ResumirFranjas(object valor)
        {
            List<FranjaHoraria> franjas = valor as List<FranjaHoraria>;
            if (franjas == null || franjas.Count == 0)
                return "";

            int[] orden = { 1, 2, 3, 4, 5, 6, 0 };
            List<string> resumen = new List<string>();
            foreach (int dia in orden)
            {
                foreach (FranjaHoraria franja in franjas)
                {
                    if ((int)franja.Dia == dia)
                    {
                        resumen.Add(NombreDia(franja.Dia) + " " + franja.HoraEntrada.ToString(@"hh\:mm") + " a " + franja.HoraSalida.ToString(@"hh\:mm"));
                    }
                }
            }

            return String.Join(", ", resumen.ToArray());
        }

        private string NombreDia(DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Monday: return "Lunes";
                case DayOfWeek.Tuesday: return "Martes";
                case DayOfWeek.Wednesday: return "Miércoles";
                case DayOfWeek.Thursday: return "Jueves";
                case DayOfWeek.Friday: return "Viernes";
                case DayOfWeek.Saturday: return "Sábado";
                default: return "Domingo";
            }
        }

        private void CargarHorarios()
        {
            try
            {
                grilla.DataSource = new TurnoTrabajoNegocio().Listar();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudieron cargar los horarios.";
            }
        }
    }
}
