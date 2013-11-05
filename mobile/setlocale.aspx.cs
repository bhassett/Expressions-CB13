// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for setlocale.
	/// </summary>
	public partial class setlocale : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			 
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;

			String LocaleSetting = CommonLogic.QueryStringCanBeDangerousContent("LocaleSetting");
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = CommonLogic.QueryStringCanBeDangerousContent("Locale");
            }
            if (LocaleSetting.Length == 0)
            {
                LocaleSetting = Localization.WebConfigLocale;
            }
            if (LocaleSetting.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            LocaleSetting = Localization.CheckLocaleSettingForProperCase(LocaleSetting);
            ThisCustomer.SetLocale(LocaleSetting);

			Label1.Text = String.Format(AppLogic.GetString("setlocale.aspx.1",ThisCustomer.SkinID,ThisCustomer.LocaleSetting), AppLogic.GetLanguageCode(LocaleSetting));

			string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            if (ReturnURL.IndexOf("setlocale.aspx") != -1)
			{
				ReturnURL = String.Empty;
			}

			if (ReturnURL.Length == 0)
			{
				ReturnURL = "default.aspx";
			}
			Response.AddHeader("REFRESH","1; URL=" + Server.UrlDecode(ReturnURL));
		}

	}
}
