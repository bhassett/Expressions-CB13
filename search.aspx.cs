// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for search.
	/// </summary>
	public partial class search : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("search.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
		}

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("jscripts/address_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/productcompare.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }
	}
}
