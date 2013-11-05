//------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Data;
using System.Globalization;
using System.Web.Security;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Text;

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
            if (!ThisCustomer.IsNotRegistered &&
                !AppLogic.AppConfigBool("PasswordIsOptionalDuringCheckout") &&
                !AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"))
            {
                RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }

            // this may be overridden by the XmlPackage below!
            SectionTitle = AppLogic.GetString("orderconfirmation.aspx.1", SkinID, ThisCustomer.LocaleSetting, true);

            // clear anything that should not be stored except for immediate usage:
            Address BillingAddress = new Address();
            BillingAddress.LoadByCustomer(ThisCustomer, AddressTypes.Billing, ThisCustomer.PrimaryBillingAddressID);


        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            String CustomerID = ThisCustomer.CustomerCode; 
            String OrderNumber = CommonLogic.QueryStringCanBeDangerousContent("OrderNumber", true);
            bool isvalid = false;
            
            decimal orderTotal = Decimal.Zero;
            decimal freightRate = Decimal.Zero;
            decimal freightTaxRate = Decimal.Zero;
            decimal taxRate = Decimal.Zero;

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
            
            // ----------------------------------------------------------------------------------------
            // WRITE OUT ANY HEADER CHECKOUT SEQUENCE GRAPHIC:
            // ----------------------------------------------------------------------------------------
            writer.Write("<div align=\"center\">");
            writer.Write("<img src=\"" + AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_6.gif") + "\" width=\"550\" height=\"54\" border=\"0\" >\n");
            writer.Write("</div>");

            if (CustomerID != String.Empty && OrderNumber != String.Empty)
            {
                String multiOrderNumber = DB.SQuote(OrderNumber);
                multiOrderNumber = "(" + multiOrderNumber.Replace(",", "','") + ")";

                bool hasFailedTransaction = false;
                hasFailedTransaction = DB.GetSqlN(string.Format("SELECT COUNT(*) AS N FROM  CustomerSalesOrder with (NOLOCK) WHERE SalesOrderCode IN {0} AND IsVoided = 1", multiOrderNumber)) > 0;

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader rs = DB.GetRSFormat(con, "select SUM(TotalRate) AS TotalRate, SUM(FreightRate) AS FreightRate, SUM(TaxRate) AS TaxRate FROM CustomerSalesOrder with (NOLOCK) where BillToCode=" + DB.SQuote(CustomerID) + " and SalesOrderCode IN" + multiOrderNumber))
                    {
                        if (rs.Read())
                        {
                            orderTotal = rs.ToRSFieldDecimal("TotalRate");
                            freightRate = rs.ToRSFieldDecimal("FreightRate");
                            taxRate = rs.ToRSFieldDecimal("TaxRate");

                            isvalid = true;

                        }
                    }
                }

                if (isvalid)
                {
                    String PM = AppLogic.CleanPaymentMethod(ThisCustomer.PaymentMethod);

                    bool AlreadyConfirmed = false;
                                       
                    String StoreName = AppLogic.AppConfig("StoreName");
                    bool UseLiveTransactions = AppLogic.AppConfigBool("UseLiveTransactions");

                    InterpriseShoppingCart cart = new InterpriseShoppingCart(base.EntityHelpers, SkinID, ThisCustomer, CartTypeEnum.ShoppingCart, String.Empty, false, true);

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
                            //This will only send email to admin.
                            AppLogic.SendOrderEMail(ThisCustomer, cart, salesOrderToEmail, false, PM, multipleAttachment);
                        }
                    }

                    String XmlPackageName = AppLogic.AppConfig("XmlPackage.OrderConfirmationPage");
                    if (XmlPackageName.Length == 0)
                    {
                        XmlPackageName = "page.orderconfirmation.xml.config";
                    }

                    if (XmlPackageName.Length != 0)
                    {
                        string[] salesOrderCodes = OrderNumber.Split(',');
                        for(int ctr=0; ctr<salesOrderCodes.Length; ctr++)
                        {
                            string salesOrderCode = salesOrderCodes[ctr];

                            List<XmlPackageParam> runtimeParams = new List<XmlPackageParam>();
                            if (ctr == 0)
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeHeader", true.ToString().ToLowerInvariant()));
                            }
                            else
                            {
                                runtimeParams.Add(new XmlPackageParam("IncludeHeader", false.ToString().ToLowerInvariant()));
                            }

                            string salesOrderStage = string.Empty;

                            using (SqlConnection con = DB.NewSqlConnection())
                            {
                                con.Open();
                                using (IDataReader rs = DB.GetRSFormat(con, "SELECT Stage from CustomerSalesOrderWorkFlowView where salesOrderCode=" + DB.SQuote(salesOrderCode)))
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

                    #region Conversion

                    if (!AlreadyConfirmed)
                    {

                        #region Google Analytics

                        if (AppLogic.AppConfigBool("GoogleAnalytics.ConversionTracking"))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), DB.GetNewGUID(), AppLogic.GAEcommerceTracking(ThisCustomer), false);
                        }

                        #endregion

                        #region Buy Safe
                        string buySafeSealHash = AppLogic.AppConfig("BuySafe.SealHash");
                        bool registerBuySafeScript = AppLogic.AppConfigBool("BuySafe.Enabled") && !buySafeSealHash.IsNullOrEmptyTrimmed() && !ThisCustomer.EMail.IsNullOrEmptyTrimmed();

                        if (registerBuySafeScript)
                        {
                            var buySAFEGuaranteed = new StringBuilder();

                            buySAFEGuaranteed.Append("<span id='BuySafeGuaranteeSpan'></span>");
                            buySAFEGuaranteed.Append("<script src='//seal.buysafe.com/private/rollover/rollover.js'></script>");
                            buySAFEGuaranteed.Append("<script type='text/javascript'>");
                            buySAFEGuaranteed.Append(" if(window.buySAFE && buySAFE.Loaded){ ");
                            buySAFEGuaranteed.AppendFormat(" buySAFE.Hash = '{0}'; ", buySafeSealHash);
                            buySAFEGuaranteed.AppendFormat(" buySAFE.Guarantee.order = '{0}'; ", OrderNumber);
                            buySAFEGuaranteed.AppendFormat(" buySAFE.Guarantee.subtotal = {0}; ", orderTotal);
                            buySAFEGuaranteed.AppendFormat(" buySAFE.Guarantee.email = '{0}'; ", ThisCustomer.EMail);
                            buySAFEGuaranteed.Append(" WriteBuySafeGuarantee('JavaScript'); ");
                            buySAFEGuaranteed.Append(" }");
                            buySAFEGuaranteed.Append("</script>");

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), DB.GetNewGUID(), buySAFEGuaranteed.ToString(), false);
                        }
                        #endregion

                        #region Bing Ads 
                            
                        string bingAdsTrackingScript = AppLogic.GetBingAdsTrackingScript(AppLogic.BING_ADS_TYPE_CONVERSION, freightRate, taxRate, orderTotal);

                        if (!bingAdsTrackingScript.IsNullOrEmptyTrimmed())
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), DB.GetNewGUID(), bingAdsTrackingScript, false);
                        }

                        #endregion

                    }

                    #endregion

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
