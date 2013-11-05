// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for showcategory.
	/// </summary>
	public partial class productcompare : SkinBase
	{
     
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
		}

        protected override bool EnableScriptGlobalization
        {
            get { return true; }
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/productcompare.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
 
        }

        override protected void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }

	}
}