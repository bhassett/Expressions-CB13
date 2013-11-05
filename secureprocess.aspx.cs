// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Text;
using System.Data;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using InterpriseSuiteEcommerceGateways;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for secureprocess.
    /// 3-D Secure processing.
    /// </summary>
    public partial class secureprocess : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            Response.Cache.SetAllowResponseInBrowserHistory(false);

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

            String paReq = ThisCustomer.ThisCustomerSession["3Dsecure.paReq"];
            String PaRes = CommonLogic.FormCanBeDangerousContent("PaRes");
            String MerchantData = CommonLogic.FormCanBeDangerousContent("MD");
            String TransactionID = ThisCustomer.ThisCustomerSession["3Dsecure.XID"];
            string salesOrderCode = ThisCustomer.ThisCustomerSession.Session("3Dsecure.OrderNumber");
            String ErrorDesc = String.Empty;
            String ReturnURL = String.Empty;

            // The PaRes should have no whitespace in it, we need to strip it out.
            PaRes = PaRes.Replace(" ", "");
            PaRes = PaRes.Replace("\r", "");
            PaRes = PaRes.Replace("\n", "");

            if (PaRes.Length != 0)
            {
                ThisCustomer.ThisCustomerSession["3Dsecure.PaRes"] = PaRes;
            }

            if (ReturnURL.Length == 0 && MerchantData != ThisCustomer.ThisCustomerSession["3Dsecure.MD"])
            {
                ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, ThisCustomer.LocaleSetting, true));
            }

            if (ReturnURL.Length == 0 && ShoppingCart.CartIsEmpty(ThisCustomer.CustomerCode, CartTypeEnum.ShoppingCart))
            {
                ReturnURL = "ShoppingCart.aspx";
            }

            if (ReturnURL.Length == 0 && CommonLogic.IsStringNullOrEmpty(salesOrderCode))
            {
                ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, ThisCustomer.LocaleSetting, true));
            }

            if (ReturnURL.Length == 0)
            {
                if (paReq.Length == 0 || TransactionID.Length == 0)
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(AppLogic.GetString("secureprocess.aspx.1", 1, ThisCustomer.LocaleSetting, true));
                }
            }

            if (ReturnURL.Length == 0)
            {
                InterpriseShoppingCart cart = InterpriseShoppingCart.Get(ThisCustomer, CartTypeEnum.ShoppingCart, true);
                if(cart != null)
                {
                    InterpriseSuiteEcommerceCommon.Gateway gatewayToUse = null;

                    Address billingAddress = ThisCustomer.PrimaryBillingAddress;
                    Address shippingAddress = ThisCustomer.PrimaryShippingAddress;

                    string receiptCode = TransactionID; // This is what interprise sends as Vendor Transaction Code upon Capture Process
                    string status = cart.PlaceOrder(gatewayToUse, billingAddress, shippingAddress, ref salesOrderCode, ref receiptCode, true, true, true);

                    if (status == AppLogic.ro_OK)
                    {
                        ThisCustomer.ClearTransactions(true);

                        ReturnURL = string.Format("orderconfirmation.aspx?ordernumber={0}", Server.UrlEncode(salesOrderCode));
                    }
                    else
                    {
                        ErrorDesc = status;
                    }
                }
                else
                {
                    // ORDER CANNOT BE FOUND!!!
                    ReturnURL = "ShoppingCart.aspx";
                }
            }


            if (ReturnURL.Length == 0)
            {
                if (AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
                {
                    ReturnURL = "checkout1.aspx?error=1&errormsg=" + Server.UrlEncode(String.Format(AppLogic.GetString("secureprocess.aspx.2", 1, ThisCustomer.LocaleSetting, true), ErrorDesc));
                }
                else
                {
                    ReturnURL = "checkoutpayment.aspx?error=1&errormsg=" + Server.UrlEncode(String.Format(AppLogic.GetString("secureprocess.aspx.2", 1, ThisCustomer.LocaleSetting, true), ErrorDesc));
                }
            }

            ThisCustomer.ThisCustomerSession["3DSecure.CustomerID"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.OrderNumber"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.ACSUrl"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.paReq"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.XID"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3DSecure.MD"] = String.Empty;
            ThisCustomer.ThisCustomerSession["3Dsecure.PaRes"] = String.Empty;


            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            Response.Write("<html><head><title>3-D Secure Process</title></head><body>");
            Response.Write("<script type=\"text/javascript\">\n");
            Response.Write("top.location='" + ReturnURL + "';\n");
            Response.Write("</SCRIPT>\n");
            Response.Write("<div align=\"center\">" + String.Format(AppLogic.GetString("secureprocess.aspx.3", 1, ThisCustomer.LocaleSetting, true), ReturnURL) + "</div>");
            Response.Write("</body></html>");

        }
    }
}
