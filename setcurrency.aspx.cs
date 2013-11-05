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
	/// Summary description for setcurrency.
	/// </summary>
    public partial class setcurrency : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;

            String CurrencySetting = CommonLogic.QueryStringCanBeDangerousContent("CurrencySetting");

            if (CurrencySetting.Length == 0)
            {
                CurrencySetting = CommonLogic.QueryStringCanBeDangerousContent("Currency");
            }
            if (CurrencySetting.Length == 0)
            {
                CurrencySetting = Localization.GetPrimaryCurrency();
            }
            if (CurrencySetting.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            CurrencySetting = Localization.CheckCurrencySettingForProperCase(CurrencySetting);
            ThisCustomer.SetCurrency(CurrencySetting);

			Label1.Text = String.Format(AppLogic.GetString("setCurrency.aspx.1",ThisCustomer.SkinID,ThisCustomer.CurrencyCode),Currency.GetName(ThisCustomer.CurrencyCode));

			string ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            if (ReturnURL.IndexOf("setcurrency.aspx") != -1)
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
