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
    /// Summary description for setvatsetting.
    /// </summary>
    public partial class setvatsetting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = Customer.Current;
        
            if (!ThisCustomer.HasCustomerRecord)
            {
                ThisCustomer.RequireCustomerRecord();
            }

            string vatSetting = Customer.ValidateVATSetting(CommonLogic.QueryStringCanBeDangerousContent("VATSetting"));
            VatDefaultSetting xVat = (VatDefaultSetting)Enum.Parse(typeof(VatDefaultSetting), vatSetting);

            ThisCustomer.VATSettingRaw = xVat;
            string message = string.Empty;

            switch (xVat)
            {
                case VatDefaultSetting.Inclusive:
                    message = AppLogic.GetString("setvatsetting.aspx.2", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    break;
                default:
                    message = AppLogic.GetString("setvatsetting.aspx.3", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                    break;
            }

            Label1.Text = string.Format(AppLogic.GetString("setvatsetting.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting), message);

            string returnUrl = CommonLogic.QueryStringCanBeDangerousContent("RETURNURL");
            AppLogic.CheckForScriptTag(returnUrl);
            if (returnUrl.Contains("setvatsetting.aspx"))
            {
                returnUrl = string.Empty;
            }

            if (CommonLogic.IsStringNullOrEmpty(returnUrl))
            {
                returnUrl = "default.aspx";
            }

            Response.AddHeader("REFRESH", "1; URL=" + Security.UrlDecode(returnUrl));
        }
    }
}

