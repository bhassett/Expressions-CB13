using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using Interprise.Framework.ECommerce.DatasetGateway;

namespace InterpriseSuiteEcommerce
{
    public partial class cancelnotification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            ltMessage.Text = AppLogic.GetString("cancelnotification.aspx.cs.2", Customer.Current.SkinID, Customer.Current.LocaleSetting);
            string notificationType = CommonLogic.QueryStringCanBeDangerousContent("NotificationType");
            string itemCode = CommonLogic.QueryStringCanBeDangerousContent("itemCode");
            string contactCode = CommonLogic.QueryStringCanBeDangerousContent("contactCode");
            string emailAddress = CommonLogic.QueryStringCanBeDangerousContent("emailAddress");

            string[] customerQuery = contactCode.Split('|');
            string[] emailQuery = emailAddress.Split('|');

            //Verify if needed data are valid. If not redirect to error page.
            if (customerQuery.Length != 3 || emailQuery.Length != 3 || itemCode.IsNullOrEmptyTrimmed())
            {
                Response.Redirect(String.Format("pageerror.aspx?parameter={0}", AppLogic.GetString("pageerror.aspx.2", Customer.Current.SkinID, Customer.Current.LocaleSetting)));
            }

            string contactCodePwd = customerQuery[0].Replace(" ", "+");
            string contactCodeSalt = customerQuery[1].Replace(" ", "+");
            string contactCodeIv = customerQuery[2].Replace(" ", "+");

            string emailAddPwd = emailQuery[0].Replace(" ", "+");
            string emailAddSalt = emailQuery[1].Replace(" ", "+");
            string emailAddIv = emailQuery[2].Replace(" ", "+");

            string decryptedcontactCode = InterpriseHelper.Decryption(Convert.FromBase64String(contactCodePwd), Convert.FromBase64String(contactCodeSalt), Convert.FromBase64String(contactCodeIv));
            string decryptedEmailAddress = InterpriseHelper.Decryption(Convert.FromBase64String(emailAddPwd), Convert.FromBase64String(emailAddSalt), Convert.FromBase64String(emailAddIv));

            string[][] ruleloaddataset = new string[][] { new string[] {"ECOMMERCENOTIFICATION", "READECOMMERCENOTIFICATION", "@ContactCode", decryptedcontactCode,
                                                      "@WebsiteCode", InterpriseHelper.ConfigInstance.WebSiteCode, "@ItemCode", itemCode, "@EmailAddress", decryptedEmailAddress}};

            var ruleDatasetContainer = new EcommerceNotificationDatasetGateway();

            if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.LoadDataSet(
                InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, ruleloaddataset, ruleDatasetContainer))
            {
                EcommerceNotificationDatasetGateway.EcommerceNotificationRow ruleDatasetContainernewRow;

                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                {
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification.NewEcommerceNotificationRow();
                }
                else
                {
                    ruleDatasetContainernewRow = ruleDatasetContainer.EcommerceNotification[0];
                }

                bool onPriceDrop = AppLogic.CheckNotification(decryptedcontactCode, decryptedEmailAddress, itemCode, 1);
                bool onItemAvail = AppLogic.CheckNotification(decryptedcontactCode, decryptedEmailAddress, itemCode, 0);

                if (notificationType == "1")
                {
                    onPriceDrop = false;
                }
                else
                {
                    onItemAvail = false;
                }

                ruleDatasetContainernewRow.BeginEdit();
                ruleDatasetContainernewRow.NotifyOnPriceDrop = onPriceDrop;
                ruleDatasetContainernewRow.NotifyOnItemAvail = onItemAvail;
                ruleDatasetContainernewRow.EndEdit();

                if (!ruleDatasetContainernewRow.NotifyOnPriceDrop && !ruleDatasetContainernewRow.NotifyOnItemAvail)
                {
                    ruleDatasetContainernewRow.Delete();
                }

                if (ruleDatasetContainer.EcommerceNotification.Rows.Count == 0)
                {
                    ruleDatasetContainer.EcommerceNotification.AddEcommerceNotificationRow(ruleDatasetContainernewRow);
                }

                string[][] rulecommandset = new string[][] { new string[] { ruleDatasetContainer.EcommerceNotification.TableName, "CREATEECOMMERCENOTIFICATION",
                                                                        "UPDATEECOMMERCENOTIFICATION", "DELETEECOMMERCENOTIFICATION"} };

                if (Interprise.Facade.Base.SimpleFacade.Instance.CurrentBusinessRule.UpdateDataset(
                    InterpriseHelper.ConfigInstance.OnlineCompanyConnectionString, rulecommandset, ruleDatasetContainer))
                {
                    ltMessage.Text = AppLogic.GetString("cancelnotification.aspx.cs.2", Customer.Current.SkinID, Customer.Current.LocaleSetting);
                    Response.Write("<script type=text/javascript language=javascript>window.open('', '_parent', '');window.top.close();</script>");
                }
            }
        }
    }
}