// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;

using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for reorder.
	/// </summary>
	public partial class reorder : SkinBase
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			SectionTitle = AppLogic.GetString("reorder.aspx.1",SkinID,ThisCustomer.LocaleSetting);

            RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
		}

		protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
		{
			String salesOrderCode = CommonLogic.QueryStringCanBeDangerousContent("so");

            if (!ThisCustomer.OwnsThisOrder(salesOrderCode))
            {
                Response.Redirect(SE.MakeDriverLink("ordernotfound"));
            }

            if (salesOrderCode == String.Empty)
			{
				writer.Write("<p>" + String.Format(AppLogic.GetString("reorder.aspx.2",SkinID,ThisCustomer.LocaleSetting),"account.aspx") + "</p>");
			}
			String StatusMsg = String.Empty;
            if (InterpriseHelper.ReOrderToCart(salesOrderCode, ThisCustomer, base.EntityHelpers, ref StatusMsg))
			{
                Response.Redirect(String.Format("shoppingcart.aspx{0}", StatusMsg));
			}
			else
			{ 
				Response.Write("<p>There were some errors in trying to create the order.</p>");
				Response.Write("<p>Error: " + StatusMsg + "</p>");
				Response.Write("<p>" + String.Format(AppLogic.GetString("reorder.aspx.2",SkinID,ThisCustomer.LocaleSetting),"shoppingcart.aspx",AppLogic.GetString("AppConfig.CartPrompt",SkinID,ThisCustomer.LocaleSetting)) + "</p>");
			}
		}
	}
}
