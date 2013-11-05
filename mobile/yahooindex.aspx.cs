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
	/// Summary description for yahooindex.
	/// </summary>
	public partial class yahooindex : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");
			
			Response.Write(new EntityHelper(EntityDefinitions.readonly_CategoryEntitySpecs).GetEntityYahooSiteMap("0", Localization.WebConfigLocale,true,true));
            Response.Write(new EntityHelper(EntityDefinitions.readonly_SectionEntitySpecs).GetEntityYahooSiteMap("0", Localization.WebConfigLocale, true, true));
            Response.Write(new EntityHelper(EntityDefinitions.readonly_ManufacturerEntitySpecs).GetEntityYahooSiteMap("0", Localization.WebConfigLocale, true, true));
		}

	}
}
