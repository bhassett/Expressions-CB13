// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web.Security;
using System.Linq;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce.mobile
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
            ThisCustomer.ThisCustomerSession.Clear();
            
            Session.Clear();
            Session.Abandon();


            Response.Cookies.Clear();
            Response.Expires = 0;
            Response.Cache.SetNoStore();

            //Do special handling of key when IE and site has multiple domain
            if (Request.Browser.Browser == "IE" &&
                Request.Cookies.Keys.OfType<string>().Where(k => k.ToUpper() == FormsAuthentication.FormsCookieName).Count() > 1)
            {
                Request.Cookies.Clear();

                Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

                var autCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (autCookie != null)
                {
                    Context.User = new InterpriseSuiteEcommercePrincipal(Customer.MakeAnonymous());
                    Customer.Current.RequireCustomerRecord();
                    var customer = Customer.Current;

                    string cookieUserName = customer.ContactGUID.ToString();
                    string encryptedData = FormsAuthentication.Encrypt(
                                                new FormsAuthenticationTicket(1, cookieUserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                                                    false, string.Empty, FormsAuthentication.FormsCookiePath));

                    autCookie.Value = encryptedData;
                    Request.Cookies.Set(autCookie);
                    Response.Cookies.Set(autCookie);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
            }

            this.Title = AppLogic.AppConfig("StoreName") + " - Signout";
            Literal1.Text = AppLogic.GetString(Literal1.Text.Replace("(!", "").Replace("!)", ""), ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            Response.AddHeader("REFRESH", "1; URL=default.aspx");
		}
	}
}
