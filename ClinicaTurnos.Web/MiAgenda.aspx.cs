using System;
using System.Data.SqlClient;
using System.Globalization;
using ClinicaTurnos.Negocio;

namespace ClinicaTurnos.Web
{
    public partial class MiAgenda : PaginaBase
    {
        protected override bool EsPaginaMedico { get { return true; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            if (Session["Mensaje"] is string mensaje)
            {
                lblMensaje.Text = Server.HtmlEncode(mensaje);
                lblMensaje.CssClass = "aviso exito";
                Session.Remove("Mensaje");
            }
            CargarAgenda(null);
        }

        protected void Filtrar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            lblMensaje.CssClass = "aviso";
            if (string.IsNullOrWhiteSpace(txtFecha.Text))
            {
                CargarAgenda(null);
                return;
            }

            DateTime fecha;
            if (!DateTime.TryParseExact(txtFecha.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out fecha))
            {
                lblMensaje.Text = "Elegí una fecha válida.";
                return;
            }

            CargarAgenda(fecha.Date);
        }

        protected void Limpiar_Click(object sender, EventArgs e)
        {
            txtFecha.Text = "";
            lblMensaje.Text = "";
            lblMensaje.CssClass = "aviso";
            CargarAgenda(null);
        }

        private void CargarAgenda(DateTime? fecha)
        {
            try
            {
                grilla.DataSource = new TurnoNegocio().ListarDelMedico(UsuarioActual.MedicoId.Value, fecha);
                grilla.DataBind();
            }
            catch (SqlException)
            {
                lblMensaje.CssClass = "aviso";
                lblMensaje.Text = "No se pudo cargar la agenda. Intentá nuevamente más tarde.";
            }
        }
    }
}
