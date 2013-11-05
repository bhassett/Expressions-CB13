// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for news.
	/// </summary>
	public partial class news : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("news.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            XmlPackage1.SetContext = this;

            XSLTExtensionBase xslt = new XSLTExtensionBase(ThisCustomer, 1);
            xslt.ExtractBody("");
    	}
	}
}
