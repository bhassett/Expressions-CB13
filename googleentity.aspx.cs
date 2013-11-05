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
using System.Web;
using System.Web.SessionState;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for googleentity.
    /// </summary>
    public partial class googleentity : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = new System.Text.UTF8Encoding();
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            String EntityName = CommonLogic.QueryStringCanBeDangerousContent("EntityName");

            if (EntityName.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            String EntityID = CommonLogic.QueryStringCanBeDangerousContent("EntityID");

            EntityHelper eHlp = new EntityHelper(EntityDefinitions.LookupSpecs(EntityName));

            Response.Write("<urlset xmlns=\"" + AppLogic.AppConfig("GoogleSiteMap.Xmlns") + "\">");

            Response.Write("<url>");
            Response.Write("<loc>" + XmlCommon.XmlEncode(AppLogic.GetStoreHTTPLocation(false) + SE.MakeEntityLink(EntityName, EntityID, String.Empty)) + "</loc> ");
            Response.Write("<changefreq>" + AppLogic.AppConfig("GoogleSiteMap.EntityChangeFreq") + "</changefreq> ");
            Response.Write("<priority>" + AppLogic.AppConfig("GoogleSiteMap.EntityPriority") + "</priority> ");
            Response.Write("</url>");

            Response.Write(eHlp.GetEntityGoogleObjectList(EntityID, Localization.WebConfigLocale, String.Empty, String.Empty));

            Response.Write("</urlset>");

        }

    }
}
