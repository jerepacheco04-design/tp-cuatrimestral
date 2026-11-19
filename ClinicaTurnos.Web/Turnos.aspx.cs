using System;
using System.Data.SqlClient;
using ClinicaTurnos.Dominio;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class Turnos : PaginaBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            if (!IsPostBack)
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

        private void CargarTurnos()
        {
            try
            {
                grilla.DataSource = new TurnoNegocio().Listar();
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.Text = "No se pudieron cargar los turnos. Ejecutá el script de base de datos y revisá la conexión.";
            }
        }
    }
}
