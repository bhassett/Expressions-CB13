//------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    public partial class googlecheckout_postsale : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Loading Message
            lblMessage.Text = AppLogic.GetString("createaccount.aspx.91", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            string guidid = Request.QueryString[DomainConstants.GcQueryParamId];
            if (!string.IsNullOrEmpty(guidid))
            {
                var guid = new Guid(guidid);
                if (GcThreadProcessor.Exists(guid))
                {
                    if (GcThreadProcessor.IsCompleted(guid))
                    {
                        GcThreadProcessor.Remove(guid);
                        Response.Redirect(string.Format("orderconfirmation.aspx?ordernumber={0}", ThisCustomer.ThisCustomerSession["OrderNumber"]));
                    }
                    else
                    {
                        Response.AddHeader("Refresh", "1");
                        //ClientScript.RegisterClientScriptBlock(this.GetType(), "test", "WaitHandler('{0}')".FormatWith(ThisCustomer.ThisCustomerSession["OrderNumber"]));
                    }
                }
                else
                {
                    Response.Redirect("default.aspx");
                }
            }
        }
    }
}