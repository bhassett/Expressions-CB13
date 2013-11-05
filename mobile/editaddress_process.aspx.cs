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
	/// Summary description for editaddress_process.
	/// </summary>
	public partial class editaddress_process : System.Web.UI.Page
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
			String DeleteAddressID = CommonLogic.FormCanBeDangerousContent("DeleteAddressID");
			bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

			if(DeleteAddressID == String.Empty) 
			{
				DeleteAddressID = CommonLogic.QueryStringCanBeDangerousContent("DeleteAddressID");
			}
			if(DeleteAddressID != String.Empty) 
			{
				Address adr = new Address();
                adr.LoadByCustomer(ThisCustomer, AddressType, AddressID); 
				Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}",Checkout.ToString(),AddressType));
			}

            Address thisAddress = new Address(); 
			thisAddress.AddressID = AddressID;
            thisAddress.LoadByCustomer(ThisCustomer, AddressType, AddressID);
			thisAddress.AddressType = AddressType;
			thisAddress.PaymentMethod = CommonLogic.FormCanBeDangerousContent("PaymentMethod");
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
			if((thisAddress.AddressType & AddressTypes.Billing) != 0)
			{
				if(AppLogic.CleanPaymentMethod(thisAddress.PaymentMethod) == AppLogic.ro_PMCreditCard)
				{
					thisAddress.CardName = CommonLogic.FormCanBeDangerousContent("CardName");    
					thisAddress.CardType = CommonLogic.FormCanBeDangerousContent("CardType");    

					string tmpS = CommonLogic.FormCanBeDangerousContent("CardNumber");
					if(!tmpS.StartsWith("*")) 
					{
						thisAddress.CardNumber = tmpS;
					}
					thisAddress.CardExpirationMonth = CommonLogic.FormCanBeDangerousContent("CardExpirationMonth");    
					thisAddress.CardExpirationYear = CommonLogic.FormCanBeDangerousContent("CardExpirationYear");    
				}
			}

			Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}",Checkout.ToString(),AddressType));
		}

	}
}
