// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using InterpriseSuiteEcommerceCommon;
using System.Web.UI;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for sitemap.
	/// </summary>
	public partial class sitemap : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}
			SectionTitle = AppLogic.GetString("sitemap.aspx.1",SkinID,ThisCustomer.LocaleSetting);
            string XmlPackageName = AppLogic.AppConfig("XmlPackage.SiteMapPage");
            if (XmlPackageName.Length != 0)
            {
                PackagePanel.Visible = true;
                EntityPanel.Visible = false;
                XmlPackage1.PackageName = XmlPackageName;
                XmlPackage1.SetContext = this;
            }
            else
            {
                PackagePanel.Visible = false;
                EntityPanel.Visible = true;
                Literal1.Text = new SiteMap1(base.EntityHelpers, SkinID, ThisCustomer).Contents;
            }
		}

	}
}
