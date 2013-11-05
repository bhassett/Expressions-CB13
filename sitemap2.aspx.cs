// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for sitemap2.
	/// </summary>
	public partial class sitemap2 : SkinBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}
			SectionTitle = AppLogic.GetString("sitemap.aspx.1",SkinID,ThisCustomer.LocaleSetting, true);
			SiteMap1.LoadXml(new SiteMapComponentArt(base.EntityHelpers,SkinID,ThisCustomer).Contents);
		}
	}
}