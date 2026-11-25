using System;
using System.Web.UI;
using ClinicaTurnos.Dominio;

namespace ClinicaTurnos.Web
{
    public class PaginaBase : Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["ClaveFormulario"] == null)
                Session["ClaveFormulario"] = Guid.NewGuid().ToString();
            ViewStateUserKey = (string)Session["ClaveFormulario"];
            base.OnInit(e);

            bool esInicio = string.Equals(Request.AppRelativeCurrentExecutionFilePath, "~/Default.aspx", StringComparison.OrdinalIgnoreCase);
            if (!esInicio && !(Session["Usuario"] is Usuario))
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
