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
	/// Summary description for _default.
	/// </summary>
	public partial class _default : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(CommonLogic.ServerVariables("HTTP_HOST").IndexOf(AppLogic.AppConfig("LiveServer"), StringComparison.InvariantCultureIgnoreCase) != -1 && 
                CommonLogic.ServerVariables("HTTP_HOST").IndexOf("WWW", StringComparison.InvariantCultureIgnoreCase) == -1)
			{
				if(AppLogic.AppConfigBool("RedirectLiveToWWW"))
				{
					Response.Redirect("http://www." + AppLogic.AppConfig("LiveServer").ToLowerInvariant() + "/default.aspx?" + CommonLogic.ServerVariables("QUERY_STRING"));
				}
			}
		
			if(AppLogic.AppConfigBool("GoNonSecureAgain"))
			{
				SkinBase.GoNonSecureAgain();
			}

            // this may be overwridden by the XmlPackage below!
            SectionTitle = String.Format(AppLogic.GetString("default.aspx.1", SkinID, ThisCustomer.LocaleSetting), AppLogic.AppConfig("StoreName"));
            
            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;

		}

		override protected void OnInit(EventArgs e)
		{
			String HT = AppLogic.AppConfig("HomeTemplate");
			if(HT.Length != 0 )
			{
				if(!HT.EndsWith(".ascx"))
				{
					HT = HT + ".ascx";
				}
				SetTemplate(HT);
			}
			base.OnInit(e);
		}
	}
}
