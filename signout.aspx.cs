// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for signout.
	/// </summary>
	public partial class signout : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = (Context.User as InterpriseSuiteEcommercePrincipal).ThisCustomer;

            if (AppLogic.AppConfigBool("SiteDisclaimerRequired"))
            {
                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);
            }

			AppLogic.SetSessionCookie("AffiliateID","");
            AppLogic.SetCookie("LocaleSetting", ThisCustomer.LocaleSetting, new TimeSpan(1000, 0, 0, 0, 0));

            InterpriseHelper.CreateContactSiteLog(ThisCustomer, "Logout");
            Session.Clear();
            Session.Abandon();

            Response.Cookies.Clear();
            Response.Expires = 0;
            Response.Cache.SetNoStore();

            //save the last record of fullmode
            bool? isRequestedFullMode = ThisCustomer.ThisCustomerSession[DomainConstants.MOBILE_FULLMODE_QUERYTSTRING].TryParseBool();
            bool value = (isRequestedFullMode.HasValue) ? isRequestedFullMode.Value : false;

            //build the query string
            string addedQueryString = (isRequestedFullMode.HasValue && isRequestedFullMode.Value)? "?" + DomainConstants.MOBILE_FULLMODE_QUERYTSTRING + "=true" : String.Empty;

            //create anonymous and pass the value again so it will not go to the mobile design
            ThisCustomer.ThisCustomerSession.Clear();

            FormsAuthentication.SignOut();
            Security.SignOutCrossDomainCookie();

            this.Title = AppLogic.AppConfig("StoreName") + " - Signout";
            Literal1.Text = AppLogic.GetString(Literal1.Text.Replace("(!", "").Replace("!)", ""), ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            Response.Redirect(String.Format("default.aspx{0}", addedQueryString));

            //Response.AddHeader("REFRESH", String.Format("1; URL=default.aspx{0}", addedQueryString));

		}
	}
}
