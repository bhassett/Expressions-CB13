// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for yahooentity.
    /// </summary>
    public partial class yahooentity : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            String EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");

            if (EntityName.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            String EntityID = CommonLogic.QueryStringCanBeDangerousContent("EntityID");
            if (EntityName.Length != 0 && EntityID != String.Empty)
            {
                EntityHelper eHlp = new EntityHelper(EntityDefinitions.LookupSpecs(EntityName));
                Response.Write(XmlCommon.XmlEncode(AppLogic.GetStoreHTTPLocation(false) + SE.MakeEntityLink(EntityName, EntityID, String.Empty)) + Environment.NewLine);
                Response.Write(eHlp.GetEntityYahooObjectList(EntityID, Localization.WebConfigLocale, String.Empty, String.Empty));
            }
        }


    }
}
