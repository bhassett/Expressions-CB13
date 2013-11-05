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
	/// Summary description for googleindex.
	/// </summary>
	public partial class googleindex : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "text/xml";
			Response.ContentEncoding = new System.Text.UTF8Encoding();
			Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

			Response.Write("<sitemapindex xmlns=\"http://www.google.com/schemas/sitemap/0.84\">");
			Response.Write("<sitemap>");
			Response.Write("<loc>" + AppLogic.GetStoreHTTPLocation(false) + "googletopics.aspx</loc>");
			Response.Write("</sitemap>");
			
			Response.Write(new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs).GetEntityGoogleSiteMap("0",Localization.WebConfigLocale,true,true));
            Response.Write(new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs).GetEntityGoogleSiteMap("0", Localization.WebConfigLocale, true, true));
            Response.Write(new EntityHelper(EntityDefinitions.readonly_ManufacturerEntitySpecs).GetEntityGoogleSiteMap("0", Localization.WebConfigLocale, true, true));
		
			Response.Write("</sitemapindex>");

		}

	}
}
