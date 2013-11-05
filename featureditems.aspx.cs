// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------
//FeatureItem WebPage is for Interprise Integration Only !!!
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using InterpriseSuiteEcommerceCommon;
using System.Web.UI;

namespace InterpriseSuiteEcommerce
{
    public partial class featureditems : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("featureditems.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            XmlPackage1.SetContext = this;
        }

        protected override bool EnableScriptGlobalization
        {
            get { return true; }
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));            
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }
    }
}