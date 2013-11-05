// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Net.Mail;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for notification.
	/// </summary>
	public partial class notification : System.Web.UI.Page
	{
        String OrderNumber;
        String CustomerID;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

			OrderNumber = CommonLogic.QueryStringCanBeDangerousContent("OrderNumber");
			CustomerID = CommonLogic.QueryStringCanBeDangerousContent("CustomerID");
            Title = String.Format(AppLogic.GetString("notification.aspx.1", 1, Customer.Current.LocaleSetting), AppLogic.AppConfig("StoreName"), OrderNumber.ToString());
            if (!IsPostBack)
            {
                InitializePageContent();
            }
		}


        private void InitializePageContent()
        {
            notification_aspx_1.Text = String.Format(AppLogic.GetString("notification.aspx.1", 1, Customer.Current.LocaleSetting), AppLogic.AppConfig("StoreName"), OrderNumber.ToString());
            notification_aspx_2.Text = String.Format(AppLogic.GetString("notification.aspx.2", 1, Customer.Current.LocaleSetting), Localization.ToNativeDateTimeString(DateTime.Now));
            notification_aspx_4.Text = AppLogic.GetString("notification.aspx.4", 1, Customer.Current.LocaleSetting);
            String strReceiptURL = AppLogic.GetStoreHTTPLocation(true) + "receipt.aspx?ordernumber=" + OrderNumber.ToString() + "&customerid=" + CustomerID.ToString();
            String strXMLURL = AppLogic.GetStoreHTTPLocation(true) + AppLogic.AppConfig("AdminDir") + "/orderXML.aspx?ordernumber=" + OrderNumber.ToString() + "&customerid=" + CustomerID.ToString();
            ReceiptURL.NavigateUrl = strReceiptURL;
            ReceiptURL.Text = AppLogic.GetString("notification.aspx.3", 1, Customer.Current.LocaleSetting);
            notification_aspx_5.Text = AppLogic.GetString("notification.aspx.5", 1, Customer.Current.LocaleSetting);
            XmlURL.NavigateUrl = strXMLURL;
            XmlURL.Text = AppLogic.GetString("notification.aspx.3", 1, Customer.Current.LocaleSetting);
        }
	}
}
