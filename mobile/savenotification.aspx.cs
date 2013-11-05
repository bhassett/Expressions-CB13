using System;
using Interprise.Framework.ECommerce.DatasetGateway;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce.mobile
{
    public partial class savenotification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            ltMessage.Text = AppLogic.GetString("savenotification.aspx.cs.1", Customer.Current.SkinID, Customer.Current.LocaleSetting);
            Int32 NotificationType = Int32.Parse(CommonLogic.QueryStringCanBeDangerousContent("NotificationType"));
            String itemCode = CommonLogic.QueryStringCanBeDangerousContent("itemCode");
            String ProductURL = CommonLogic.QueryStringCanBeDangerousContent("ProductURL") + InterpriseHelper.MakeItemLink(itemCode);


            string[][] ruleloaddataset;
            ruleloaddataset = new string[][] { new string[] {"ECOMMERCENOTIFICATION", "READECOMMERCENOTIFICATION", "@ContactCode", Customer.Current.ContactCode,
                                                      "@WebsiteCode", InterpriseHelper.ConfigInstance.WebSiteCode, "@ItemCode", itemCode, "@EmailAddress", Customer.Current.EMail}};

            EcommerceNotificationDatasetGateway ruleDatasetContainer = new EcommerceNotificationDatasetGateway();

            if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.LoadDataSet(
                InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, ruleloaddataset, ruleDatasetContainer))
            {
                EcommerceNotificationDatasetGateway.EcommerceNotificationRow ruleDatasetContainernewRow;

                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification.NewEcommerceNotificationRow();
                else
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification[0];

                Boolean OnPriceDrop = AppLogic.CheckNotification(Customer.Current.ContactCode, Customer.Current.EMail, itemCode, 1);
                Boolean OnItemAvail = AppLogic.CheckNotification(Customer.Current.ContactCode, Customer.Current.EMail, itemCode, 0);

                if (NotificationType == 1)
                {
                    OnPriceDrop = true;
                }
                else
                {
                    OnItemAvail = true;
                }

                ruleDatasetContainernewRow.BeginEdit();
                ruleDatasetContainernewRow.WebSiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;
                ruleDatasetContainernewRow.ItemCode = itemCode;
                ruleDatasetContainernewRow.ContactCode = Customer.Current.ContactCode;
                ruleDatasetContainernewRow.EmailAddress = Customer.Current.EMail;
                ruleDatasetContainernewRow.NotifyOnPriceDrop = OnPriceDrop;
                ruleDatasetContainernewRow.NotifyOnItemAvail = OnItemAvail;
                ruleDatasetContainernewRow.ProductURL = ProductURL;

                byte[] salt = InterpriseHelper.GenerateSalt();
                byte[] iv = InterpriseHelper.GenerateVector();
                string contactCodeCypher = InterpriseHelper.Encryption(Customer.Current.ContactCode, salt, iv);
                string emailAddressCypher = InterpriseHelper.Encryption(Customer.Current.EMail, salt, iv);

                ruleDatasetContainernewRow.EncryptedContactCode = contactCodeCypher + "|" + Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(iv);
                ruleDatasetContainernewRow.EncryptedEmailAddress = emailAddressCypher + "|" + Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(iv);


                ruleDatasetContainernewRow.EndEdit();


                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                    ruleDatasetContainer.EcommerceNotification.AddEcommerceNotificationRow(ruleDatasetContainernewRow);

                string[][] rulecommandset;
                rulecommandset = new string[][] { new string[] { ruleDatasetContainer.EcommerceNotification.TableName, "CREATEECOMMERCENOTIFICATION",
                                                                        "UPDATEECOMMERCENOTIFICATION", "DELETEECOMMERCENOTIFICATION"} };

                if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.UpdateDataset(
                    InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, rulecommandset, ruleDatasetContainer))
                {
                    ltMessage.Text = AppLogic.GetString("savenotification.aspx.cs.2", Customer.Current.SkinID, Customer.Current.LocaleSetting);
                    Response.Write("<script type=text/javascript language=javascript>window.top.close();</script>");
                }
            }
        }
    }
}
 

      