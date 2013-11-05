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
    public partial class receipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SkinBase.RequireSecurePage();

            var thisCustomer = Customer.Current;
            bool blnShowReceipt = false;

            //get the values from the querystring
            string strSalesOrderCodeFromQueryString = CommonLogic.QueryStringCanBeDangerousContent("OrderNumber");
            string strCustGuidFromQueryString = CommonLogic.QueryStringCanBeDangerousContent("CustomerGUID");

            if (thisCustomer.IsNotRegistered)
            {
                //unregistered customers will have values stored in the cookie, get the values and compare to the querystring
                string strOrderNumberFromCookie = CommonLogic.CookieCanBeDangerousContent("OrderNumber", true);
                string strCustGuidFromCookie = CommonLogic.CookieCanBeDangerousContent("ContactGUID", true);

                //show the receipt only if both the order number and guid match
                blnShowReceipt = strCustGuidFromQueryString.Equals(strCustGuidFromCookie, StringComparison.InvariantCultureIgnoreCase) && strSalesOrderCodeFromQueryString.Equals(strOrderNumberFromCookie, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                //make sure that this customer owns this order to view
                if (thisCustomer.OwnsThisOrder(strSalesOrderCodeFromQueryString))
                {
                    blnShowReceipt = true;   
                }
            }

            //show the receipt if it's appropriate to do so
            if (blnShowReceipt && !string.IsNullOrEmpty(strSalesOrderCodeFromQueryString))
            {
                ViewerReport.Report = InterpriseHelper.CreateReport(strSalesOrderCodeFromQueryString);
            }
            else
            {
                Response.Redirect(SE.MakeDriverLink("ordernotfound"));
            }
        }
    }
}
