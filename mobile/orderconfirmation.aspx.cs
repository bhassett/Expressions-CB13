//------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for orderconfirmation.
    /// </summary>
    public partial class orderconfirmation : SkinBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            //Init();

            if (!ThisCustomer.IsNotRegistered && 
                !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") &&
                !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }

            // this may be overridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("orderconfirmation.aspx.1", SkinID, ThisCustomer.LocaleSetting);

            // clear anything that should not be stored except for immediate usage:
            var BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer, AddressTypes.Billing, ThisCustomer.PrimaryBillingAddressID);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            string CustomerID = ThisCustomer.CustomerCode; 
            string OrderNumber = CommonLogic.QueryStringCanBeDangerousContent("OrderNumber", true);
            bool isvalid = false;
            decimal ordertotal = decimal.Zero;

            //don't allow the customer any further if they dont own this order.
            foreach (string salesOrderToCheck in OrderNumber.Split(','))
            {
                if (ThisCustomer.IsUnregisteredAnonymous ||
                !ThisCustomer.OwnsThisOrder(salesOrderToCheck))
                {
                    Response.Redirect(SE.MakeDriverLink("ordernotfound"));
                }
            }

            //Assign anonymous id as customer id for report generation.
            if (!ThisCustomer.IsRegistered && OrderNumber != "")
            {
                ThisCustomer.EMail = ThisCustomer.GetAnonEmail(); 
                ThisCustomer.CustomerCode = ThisCustomer.AnonymousCustomerCode;
                CustomerID = ThisCustomer.CustomerCode;
            }

            // WRITE OUT ANY HEADER CHECKOUT SEQUENCE GRAPHIC:
            if (CustomerID != string.Empty && OrderNumber != string.Empty)
            {
                string multiOrderNumber = DB.SQuote(OrderNumber);
                multiOrderNumber = "(" + multiOrderNumber.Replace(",", "','") + ")";

                bool hasFailedTransaction = false;
                hasFailedTransaction = DB.GetSqlN(string.Format("SELECT COUNT(*) AS N FROM  CustomerSalesOrder with (NOLOCK) WHERE SalesOrderCode IN {0} AND IsVoided = 1", multiOrderNumber)) > 0;

                using (var con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (var rs = DB.GetRSFormat(con, "select SUM(TotalRate)AS TotalRate from CustomerSalesOrder with (NOLOCK) where BillToCode=" + DB.SQuote(CustomerID) + " and SalesOrderCode IN" + multiOrderNumber))
                    {
                        if (rs.Read())
                        {
                            ordertotal = DB.RSFieldDecimal(rs, "TotalRate");
                            isvalid = true;
                        }
                    }
                }

                if (isvalid)
                {
                    string PM = AppLogic.CleanPaymentMethod(ThisCustomer.PaymentMethod);
                    bool AlreadyConfirmed = false;
                    string StoreName = AppLogic.AppConfig("StoreName");
                    bool UseLiveTransactions = AppLogic.AppConfigBool("UseLiveTransactions");

                    var cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

                    bool multipleAttachment = false;
                    if (multiOrderNumber.IndexOf(',') != -1)
                    {
                        multipleAttachment = true; 
                    }
                    
                    //Send mail.
                    foreach (string salesOrderToEmail in OrderNumber.Split(','))
                    {
                        if (ThisCustomer.PaymentTermCode.ToUpper() != "REQUEST QUOTE" && ThisCustomer.PaymentTermCode.ToUpper() != "PURCHASE ORDER")
                        {
                            AppLogic.SendOrderEMail(ThisCustomer, cart, salesOrderToEmail, false, PM, true, multipleAttachment);
                        }
                        else
                        {
                            AppLogic.SendOrderEMail(ThisCustomer, cart, salesOrderToEmail, false, PM, multipleAttachment);
                        }
                    }

                    string XmlPackageName = AppLogic.AppConfig("XmlPackage.OrderConfirmationPage");
                    if (XmlPackageName.Length == 0)
                    {
                        XmlPackageName = "orderconfirmation.xml.config";
                    }

                    if (XmlPackageName.Length != 0)
                    {
                        string[] salesOrderCodes = OrderNumber.Split(',');
                        for(int ctr=0; ctr<salesOrderCodes.Length; ctr++)
                        {
                            string salesOrderCode = salesOrderCodes[ctr];

                            var runtimeParams = new List<XmlPackageParam>();
                            if (ctr == 0)
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeHeader", true.ToString().ToLowerInvariant()));
                            }
                            else
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeHeader", false.ToString().ToLowerInvariant()));
                            }

                            string salesOrderStage = string.Empty;
                            using (var con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (var rs = DB.GetRSFormat(con, "SELECT Stage from CustomerSalesOrderWorkFlowView where salesOrderCode=" + DB.SQuote(salesOrderCode)))
                                {
                                    if (rs.Read())
                                    {
                                        salesOrderStage = DB.RSField(rs, "Stage");
                                    }
                                }
                            }

                            runtimeParams.Add(new XmlPackageParam("OrderNumber", salesOrderCode));
                            runtimeParams.Add(new XmlPackageParam("SalesOrderStage", salesOrderStage));

                            if (ThisCustomer.PaymentTermCode.ToUpper() == "REQUEST QUOTE")
                            {
                                runtimeParams.Add(new XmlPackageParam("PaymentMethod", "REQUESTQUOTE"));
                            }
                            else if (ThisCustomer.PaymentTermCode.ToUpper() == "PURCHASE ORDER")
                            {
                                runtimeParams.Add(new XmlPackageParam("PaymentMethod", "PURCHASEORDER"));
                            }
                            else
                            {
                                runtimeParams.Add(new XmlPackageParam("PaymentMethod", ThisCustomer.PaymentMethod));
                            }
                            runtimeParams.Add(new XmlPackageParam("Email", ThisCustomer.EMail));

                            if (ctr + 1 == salesOrderCodes.Length)
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeFooter", true.ToString().ToLowerInvariant()));
                                runtimeParams.Add(new XmlPackageParam("WriteFailedTransaction", hasFailedTransaction.ToString().ToLowerInvariant()));
                            }
                            else
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeFooter", false.ToString().ToLowerInvariant()));
                            }

                            writer.Write(AppLogic.RunXmlPackage(XmlPackageName, base.GetParser, ThisCustomer, SkinID, String.Empty, runtimeParams, true, true));
                        }
                    }

                    if (!AlreadyConfirmed && AppLogic.AppConfigBool("GoogleAnalytics.ConversionTracking"))
                    {
         
                       string test = AppLogic.GAEcommerceTracking(ThisCustomer);
                       ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), DB.GetNewGUID(), test, false);
                     
                    }
                }
                else
                {
                    writer.Write("<div align=\"center\">");
                    writer.Write("<br/><br/><br/><br/><br/>");
                    writer.Write(AppLogic.GetString("orderconfirmation.aspx.15", SkinID, ThisCustomer.LocaleSetting));
                    writer.Write("<br/><br/><br/><br/><br/>");
                    writer.Write("</div>");
                }
            }
            else
            {
                writer.Write("<p><b>Error: Invalid Customer ID or Invalid Order Number</b></p>");
            }

            if (!ThisCustomer.IsRegistered || AppLogic.AppConfigBool("ForceSignoutOnOrderCompletion"))
            {
                //Setting cookie values for anonymous receipts. We should look into a more secure way to do this, but for now
                //it's better than what we had.
                AppLogic.SetSessionCookie("ContactGUID", ThisCustomer.ContactGUID.ToString());
                AppLogic.SetSessionCookie("OrderNumber", CommonLogic.QueryStringCanBeDangerousContent("OrderNumber", true));

                if (AppLogic.AppConfigBool("SiteDisclaimerRequired"))
                {
                    AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);
                }
                //V3_9 Kill the Authentication ticket.
                Session.Clear();
                Session.Abandon();
                ThisCustomer.ThisCustomerSession.Clear();
                FormsAuthentication.SignOut();
            }
        }

    }
}
