using System;
using System.Web.UI;

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
        }
    }
}
