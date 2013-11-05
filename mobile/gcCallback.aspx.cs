// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Text;
using System.Web;
using System.Xml;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceGateways;
using InterpriseSuiteEcommerceGateways.gc;

namespace InterpriseSuiteEcommerce
{
    public partial class gcCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("loadcheck") == "1")
            {
                Response.Write("<loadcheck>" + System.DateTime.Now.ToString() + "</loadcheck>");
            }
            else
            {
                Response.CacheControl = "private";
                Response.Expires = 0;
                Response.AddHeader("pragma", "no-cache");

                // this callback requires basic authentication
                if (!GoogleCheckout.VerifyMessageAuthentication(Request.Headers["Authorization"]) && 
                    !AppLogic.AppConfigBool("GoogleCheckout.UseSandbox"))
                {
                    Response.StatusCode = 401;
                    Response.StatusDescription = "Access Denied";
                }
                else if (Request.ContentLength > 0)
                {
                    // place notification into string
                    string xmlData = Encoding.UTF8.GetString(Request.BinaryRead(Request.ContentLength));                    

                    //  Select the appropriate function to handle the notification
                    //  by evaluating the root tag of the document
                    XmlDocument googleResponse = new XmlDocument();
                    googleResponse.LoadXml(xmlData);

                    switch (googleResponse.DocumentElement.Name)
                    {
                        case "merchant-calculation-callback":
                            Response.Write(GoogleCheckout.CreateMerchantCalculationResults(xmlData));
                            break;
                        case "new-order-notification":
                            sendNotificationAcknowledgment();
							StartThreading(xmlData);
                            break;
                        case "order-state-change-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessOrderStateChangeNotification(xmlData);
                            break;
                        case "risk-information-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessRiskInformationNotification(xmlData);
                            break;
                        case "charge-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessChargeAmountNotification(xmlData);
                            break;
                        case "chargeback-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessChargebackAmountNotification(xmlData);
                            break;
                        case "authorization-amount-notification":
                            sendNotificationAcknowledgment();
                            GoogleCheckout.ProcessAuthorizationAmountNotification(xmlData);
                            break;
                        case "request-received":
                            GoogleCheckout.ProcessRequestReceived(xmlData);
                            break;
                        case "error":
                            GoogleCheckout.ProcessErrorNotification(xmlData);
                            break;
                        case "diagnosis":
                            GoogleCheckout.ProcessDiagnosisNotification(xmlData);
                            break;
                        default:
                            GoogleCheckout.ProcessUnknownNotification(xmlData);
                            break;
                    }
                }
            }
        }

        private void StartThreading(string xmlData)
        {
            var gNewOrderNotification = GoogleCheckout.DecodeRequest(xmlData, typeof(NewOrderNotification)) as NewOrderNotification;
            var CustomerInfo = gNewOrderNotification.shoppingcart.merchantprivatedata.Any[0];
            var customerGuid = new Guid(CustomerInfo.Attributes["ContactGuid"].Value);

            var paramWrapper = new ParamWrapper()
            {
                CurrentContext = HttpContext.Current,
                ID = customerGuid,
                XmlData = xmlData,
                XmlCustomerInfo = CustomerInfo
            };

            GcThreadProcessor.ThreadStart(ProcessNewOrderNotification, paramWrapper);
        }

        public void sendNotificationAcknowledgment()
        {
            Response.Write(GoogleCheckout.SendNotificationAcknowledgment());
            Response.Flush();
        }

        private static void ProcessNewOrderNotification(object paramWrapper) 
        {
            var param = paramWrapper as ParamWrapper;
            string ordernumber = GoogleCheckout.ProcessNewOrderNotification(param.XmlData, (paramWrapper as ParamWrapper).CurrentContext, param.XmlCustomerInfo);
            GoogleCheckout.AddMerchantOrderNumber(ordernumber);
            GcThreadProcessor.Finalize(param.ID);
        }

    }
}
