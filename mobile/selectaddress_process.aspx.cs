// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for selectaddress_process.
	/// </summary>
	public partial class selectaddress_process : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");
			
            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
			ThisCustomer.RequireCustomerRecord();
			bool Checkout = CommonLogic.QueryStringBool("checkout");
			String AddressID = CommonLogic.QueryStringCanBeDangerousContent("AddressID");
			String AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");

            if (AddressTypeString.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            AddressTypes AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);
			int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
			String ReturnURL = Server.UrlDecode(CommonLogic.QueryStringCanBeDangerousContent("ReturnURL"));

            if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

			if(AddressID != String.Empty && !ThisCustomer.OwnsThisAddress(AddressID))
			{
				throw new ArgumentException("That action is not allowed!");
			}

			if(!AllowShipToDifferentThanBillTo) 
			{
				//Shipping and Billing address nust be the same so save both
				AddressType = AddressTypes.Billing | AddressTypes.Shipping;
			}

			Address thisAddress = new Address();

			if (AddressID != String.Empty) //Users Selected an ID from the Address Grid
			{
				if (OriginalRecurringOrderNumber == 0)
				{
					if(AddressType == AddressTypes.Shipping)
					{
                        String sql = String.Format("update shoppingcart set ShippingAddressID={0} where ShippingAddressID={1} and CartType={2} and CustomerID={3}", AddressID.ToString(), ThisCustomer.PrimaryShippingAddressID.ToString(), ((int)CartTypeEnum.ShoppingCart).ToString(), ThisCustomer.CustomerCode.ToString());
						DB.ExecuteSQL(sql);
					}
                    thisAddress.LoadByCustomer(ThisCustomer, AddressType, AddressID);
				}
			}
			else  //Entered a new address to add
			{
                thisAddress.CustomerCode = ThisCustomer.CustomerCode;
				thisAddress.NickName = CommonLogic.FormCanBeDangerousContent("AddressNickName");
				thisAddress.FirstName = CommonLogic.FormCanBeDangerousContent("AddressFirstName");
				thisAddress.LastName = CommonLogic.FormCanBeDangerousContent("AddressLastName");
				thisAddress.Company = CommonLogic.FormCanBeDangerousContent("AddressCompany");
				thisAddress.Address1 = CommonLogic.FormCanBeDangerousContent("AddressAddress1");
				thisAddress.Suite = CommonLogic.FormCanBeDangerousContent("AddressSuite");
				thisAddress.City = CommonLogic.FormCanBeDangerousContent("AddressCity");
				thisAddress.State = CommonLogic.FormCanBeDangerousContent("AddressState");
                thisAddress.PostalCode = CommonLogic.FormCanBeDangerousContent("AddressZip");
				thisAddress.Country = CommonLogic.FormCanBeDangerousContent("AddressCountry");
				thisAddress.Phone = CommonLogic.FormCanBeDangerousContent("AddressPhone");
         
				AddressID = thisAddress.AddressID;

                ReturnURL = "selectaddress.aspx?Checkout=" + Checkout.ToString() + "&addressType=" + AddressType.ToString() + "&returnURL=" + Server.UrlEncode("account.aspx?" + Checkout.ToString());
            }
			if(OriginalRecurringOrderNumber != 0)
			{
				//put it in the ShoppingCart record
				string sql = String.Empty;
				if((AddressType & AddressTypes.Billing) != 0)
				{
					sql = String.Format("BillingAddressID={0}",AddressID);
				}
				if((AddressType & AddressTypes.Shipping) != 0)
				{
					if (sql.Length != 0) {sql += ",";}
					sql += String.Format("ShippingAddressID={0}",AddressID);
				}
				sql = String.Format("update ShoppingCart set " + sql + " where OriginalRecurringOrderNumber={0}",OriginalRecurringOrderNumber.ToString());
				DB.ExecuteSQL(sql);
			}

			if(OriginalRecurringOrderNumber == 0)
			{
                thisAddress.LoadByCustomer(ThisCustomer, AddressTypes.Billing, ThisCustomer.PrimaryBillingAddressID);
				if (thisAddress.AddressID == String.Empty)
				{
					Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType=Billing&ReturnURL={1}",Checkout.ToString(),Server.UrlEncode(ReturnURL)));
				}
                thisAddress.LoadByCustomer(ThisCustomer, AddressTypes.Shipping, ThisCustomer.PrimaryShippingAddressID);
				if (thisAddress.AddressID == String.Empty)
				{
					Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType=Shipping&ReturnURL={1}",Checkout.ToString(),Server.UrlEncode(ReturnURL)));
				}
				if (ReturnURL.Length != 0)
				{
					Response.Redirect(ReturnURL);
				}
				else
				{
					Response.Redirect("account.aspx?checkout=" + Checkout.ToString());
				}
			}
			else
			{
				Response.Redirect("account.aspx?checkout=" + Checkout.ToString());
			}
		}

	}
}
