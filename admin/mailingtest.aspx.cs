// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using InterpriseSuiteEcommerceCommon;
using System.Xml;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Authentication;
using System.Data.SqlClient;
using InterpriseSuiteEcommerceCommon.Tool;

public partial class mailingTest : Page 
{
    private ISSIUserAccount thisUserAccount = ISSIUserAccount.Current;
    private string _testSalesOrderCode = string.Empty;
    private Guid _customerGuid = Guid.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        ltError.Text = String.Empty;

        this.Title = "EMail Test and Configuration";


        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader reader = DB.GetRSFormat(con, "SELECT TOP 1 so.SalesOrderCode, c.ContactGuid FROM CustomerSalesOrder so with (NOLOCK) INNER JOIN CRMContact c with (NOLOCK) ON c.ContactCode = so.ContactCode WHERE so.Type = 'Sales Order'"))
            {
                if (reader.Read())
                {
                    _testSalesOrderCode = DB.RSField(reader, "SalesOrderCode");
                    _customerGuid = DB.RSFieldGUID2(reader, "ContactGuid");
                }
            }
        }

        // NOTE :
        //  Logins to the mini-admin site is by user accounts on Interprise
        //  under the Administrator Role. Ergo, We don't have Super Admin Users
        if (!IsPostBack)
        {
            LoadContent();
        }
    }

    protected void LoadContent()
    {
        ddXmlPackageReceipt.Items.Clear();
        ddXmlPackageOrderNotifications.Items.Clear();

        Hashtable ht = GetEmailConfigs();

        txtMailMe_ServerSimple.Text = ht["MailMe_Server"].ToString();
        txtMailServerUserSimple.Text = ht["MailMe_User"].ToString();
        txtMailServerPwdSimple.Text = ht["MailMe_Pwd"].ToString();
        txtReceiptFromSimple.Text = ht["ReceiptEMailFrom"].ToString();
        txtOrderNotificationToSimple.Text = ht["GotOrderEMailTo"].ToString();

        txtMailMe_ServerAdvanced.Text = ht["MailMe_Server"].ToString();
        txtMailServerUserAdvanced.Text = ht["MailMe_User"].ToString();
        txtMailServerPwdAdvanced.Text = ht["MailMe_Pwd"].ToString();
        txtReceiptFromAdvanced.Text = ht["ReceiptEMailFrom"].ToString();
        txtOrderNotificationToAdvanced.Text = ht["GotOrderEMailTo"].ToString();
        txtReceiptFromNameAdvanced.Text = ht["ReceiptEMailFromName"].ToString();
        txtOrderNotificationToNameAdvanced.Text = ht["GotOrderEMailFromName"].ToString();
        txtMailServerPortAdvanced.Text = ht["MailMe_Port"].ToString();
        txtOrderNotificationFromAdvanced.Text = ht["GotOrderEMailFrom"].ToString();
        txtOrderNotificationFromNameAdvanced.Text = ht["GotOrderEMailFromName"].ToString();

        if (AppLogic.AppConfigBool("SendOrderEMailToCustomer")) 
        {
            rblSendReceiptsSimple.SelectedIndex = 0;
            rblSendReceiptsAdvanced.SelectedIndex = 0;
        }
        else
        {
            rblSendReceiptsSimple.SelectedIndex = 1;
            rblSendReceiptsAdvanced.SelectedIndex = 1;
        }

        if (AppLogic.AppConfigBool("MailMe_UseSSL"))
        {
            rblMailServerSSLAdvanced.SelectedIndex = 0;
        }
        else
        {
            rblMailServerSSLAdvanced.SelectedIndex = 1;
        }

        var xmlPackages = AppLogic.ReadXmlPackages("notification", 1, true, CurrentContext.IsRequestingFromMobileMode(Customer.Current));
        foreach (string s in xmlPackages)
        {
            ddXmlPackageReceipt.Items.Add(new ListItem(s, s));
            ddXmlPackageOrderNotifications.Items.Add(new ListItem(s, s));
        }

        foreach (ListItem li in ddXmlPackageReceipt.Items)
        {
            if (li.Value.Equals(ht["XmlPackage.OrderReceipt"].ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                ddXmlPackageReceipt.SelectedValue = ht["XmlPackage.OrderReceipt"].ToString().ToLowerInvariant();
            }
            if (li.Value.Equals(ht["XmlPackage.NewOrderAdminNotification"].ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                ddXmlPackageReceipt.SelectedValue = ht["XmlPackage.NewOrderAdminNotification"].ToString().ToLowerInvariant();
            }
        }

    }

    private Hashtable GetEmailConfigs()
    {
        Hashtable ht = new Hashtable();

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader rss = DB.GetRSFormat(con, "Select * from EcommerceAppConfig with (NOLOCK) where WebsiteCode = " + DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode) + " AND  (GroupName='EMAIL' or GroupName='XMLPACKAGE' or Name='TurnOffStoreAdminEMailNotifications' or Name='SendOrderEMailToCustomer' or Name='SendShippedEMailToCustomer')"))
            {
                while (rss.Read())
                {
                    String key = DB.RSField(rss, "Name");

                    ht.Add(key, DB.RSField(rss, "ConfigValue"));

                }
            }
        }

        return ht;
    }

    private void UpdateEmailConfigsSimple()
    {
        StringBuilder sql = new StringBuilder(2500);

        sql.Append("UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailMe_ServerSimple.Text));
        sql.Append(" WHERE Name='MailMe_Server'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailServerUserSimple.Text));
        sql.Append(" WHERE Name='MailMe_User'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailServerPwdSimple.Text));
        sql.Append(" WHERE Name='MailMe_Pwd'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationToSimple.Text));
        sql.Append(" WHERE Name='GotOrderEMailTo'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationToSimple.Text));
        sql.Append(" WHERE Name='MailMe_ToAddress'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromSimple.Text));
        sql.Append(" WHERE Name='ReceiptEMailFrom'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromSimple.Text));
        sql.Append(" WHERE Name='MailMe_FromAddress'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue='" + CommonLogic.IIF(rblSendReceiptsSimple.SelectedValue == "1", "true", "false") + "'");
        sql.Append(" WHERE Name='SendOrderEMailToCustomer'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        DB.ExecuteSQL(sql.ToString());
    }

    private void UpdateEmailConfigsAdvanced()
    {
        StringBuilder sql = new StringBuilder(5000);

        sql.Append("UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailMe_ServerAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_Server'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailServerUserAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_User'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailServerPwdAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_Pwd'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtMailServerPortAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_Port'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue='" + CommonLogic.IIF(rblMailServerSSLAdvanced.SelectedValue == "1", "true", "false") + "'");
        sql.Append(" WHERE Name='MailMe_UseSSL'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationToAdvanced.Text));
        sql.Append(" WHERE Name='GotOrderEMailTo'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationToAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_ToAddress'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationToNameAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_ToName'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationFromAdvanced.Text));
        sql.Append(" WHERE Name='GotOrderEMailFrom'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtOrderNotificationFromNameAdvanced.Text));
        sql.Append(" WHERE Name='GotOrderEMailFromName'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=");
        if (ddXmlPackageOrderNotifications.SelectedValue != "0")
        {
            sql.Append(DB.SQuote(ddXmlPackageOrderNotifications.SelectedValue.ToLowerInvariant()));
        }
        else
        {
            sql.Append("'notification.adminneworder.xml.config'");
        }
        sql.Append(" WHERE Name='XmlPackage.NewOrderAdminNotification'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromAdvanced.Text));
        sql.Append(" WHERE Name='ReceiptEMailFrom'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_FromAddress'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromNameAdvanced.Text));
        sql.Append(" WHERE Name='MailMe_FromName'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=" + DB.SQuote(txtReceiptFromNameAdvanced.Text));
        sql.Append(" WHERE Name='ReceiptEMailFromName'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue=");
        if (ddXmlPackageReceipt.SelectedValue != "0")
        {
            sql.Append(DB.SQuote(ddXmlPackageReceipt.SelectedValue.ToLowerInvariant()));
        }
        else
        {
            sql.Append("'notification.receipt.xml.config'");
        }
        sql.Append(" WHERE Name='XmlPackage.OrderReceipt'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));
        sql.Append(" UPDATE dbo.EcommerceAppConfig SET ConfigValue='" + CommonLogic.IIF(rblSendReceiptsAdvanced.SelectedValue == "1", "true", "false") + "'");
        sql.Append(" WHERE Name='SendOrderEMailToCustomer'" + string.Format(" AND WebsiteCode = {0}", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)));

        DB.ExecuteSQL(sql.ToString());
    }

    private String SendTestReceiptEmail()
    {
        if (!AppLogic.AppConfigBool("SendOrderEMailToCustomer"))
        {
            return AppLogic.GetString("mailingtest.aspx.8", 1, thisUserAccount.LocaleSetting);
        }
        try
        {
            String SubjectReceipt = String.Format(AppLogic.GetString("common.cs.2", 1, thisUserAccount.LocaleSetting), AppLogic.AppConfig("StoreName"));
            Customer thisCustomer = Customer.Find(_customerGuid);
            AppLogic.SendMail(SubjectReceipt, AppLogic.Receipt(thisCustomer), true, AppLogic.AppConfig("ReceiptEMailFrom"), AppLogic.AppConfig("ReceiptEMailFromName"), AppLogic.AppConfig("GotOrderEMailTo"), string.Empty, String.Empty, String.Empty, AppLogic.AppConfig("MailMe_Server"), _testSalesOrderCode, false, false, true);
        }
        catch (Exception e)
        {
            int MailMe_PwdLen = AppLogic.AppConfig("MailMe_Pwd").ToString().Length;
            int MailMe_UserLen = AppLogic.AppConfig("MailMe_User").ToString().Length;
            String errMsg = String.Empty;

            if (e.Message.ToString().IndexOf("AUTHENTICATION", StringComparison.InvariantCultureIgnoreCase) != -1 || 
                e.Message.ToString().IndexOf("OBJECT REFERENCE", StringComparison.InvariantCultureIgnoreCase) != -1 || 
                e.Message.ToString().IndexOf("NO SUCH USER HERE", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                if (MailMe_UserLen == 0 && MailMe_PwdLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.3", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.7", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.6", 1, thisUserAccount.LocaleSetting);
                }
                else if (MailMe_UserLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.3", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.7", 1, thisUserAccount.LocaleSetting);
                }
                else if (MailMe_PwdLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.3", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.6", 1, thisUserAccount.LocaleSetting);
                }
                else
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.3", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.9", 1, thisUserAccount.LocaleSetting);
                }

                if (errMsg.Length != 0)
                {
                    return errMsg;
                }
            }
            return AppLogic.GetString("mailingtest.aspx.3", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + e.Message.ToString();
        }
        return AppLogic.GetString("mailingtest.aspx.1", 1, thisUserAccount.LocaleSetting);
    }

    private String SendTestNewOrderNotification()
    {
        try
        {
            String newOrderSubject = String.Format(AppLogic.GetString("common.cs.5", 1, thisUserAccount.LocaleSetting), AppLogic.AppConfig("StoreName"));

            String PackageName = AppLogic.AppConfig("XmlPackage.NewOrderAdminNotification");

            using (XmlPackage2 p = new XmlPackage2(PackageName, null, 1, String.Empty, XmlPackageParam.FromString("ordernumber=" + _testSalesOrderCode)))
            {
                String newOrderNotification = p.TransformString();

                String SendToList = AppLogic.AppConfig("GotOrderEMailTo").ToString().Replace(",", ";");
                if (SendToList.IndexOf(';') != -1)
                {
                    foreach (String s in SendToList.Split(';'))
                    {
                        AppLogic.SendMail(newOrderSubject, newOrderNotification + AppLogic.AppConfig("MailFooter"), true, AppLogic.AppConfig("GotOrderEMailFrom"), AppLogic.AppConfig("GotOrderEMailFromName"), s.Trim(), s.Trim(), String.Empty, AppLogic.MailServer(), true);
                    }
                }
                else
                {
                    AppLogic.SendMail(newOrderSubject, newOrderNotification + AppLogic.AppConfig("MailFooter"), true, AppLogic.AppConfig("GotOrderEMailFrom"), AppLogic.AppConfig("GotOrderEMailFromName"), SendToList, SendToList, String.Empty, AppLogic.MailServer(), true);
                }
            }
        }
        catch (Exception e)
        {
            int MailMe_PwdLen = AppLogic.AppConfig("MailMe_Pwd").ToString().Length;
            int MailMe_UserLen = AppLogic.AppConfig("MailMe_User").ToString().Length;
            String errMsg = String.Empty;

            if (e.Message.ToString().IndexOf("AUTHENTICATION", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                e.Message.ToString().IndexOf("OBJECT REFERENCE", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                e.Message.ToString().IndexOf("NO SUCH USER HERE", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                if (MailMe_UserLen == 0 && MailMe_PwdLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.4", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.7", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.6", 1, thisUserAccount.LocaleSetting);
                }
                else if (MailMe_UserLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.4", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.7", 1, thisUserAccount.LocaleSetting);
                }
                else if (MailMe_PwdLen == 0)
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.4", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.6", 1, thisUserAccount.LocaleSetting);
                }
                else
                {
                    errMsg = AppLogic.GetString("mailingtest.aspx.4", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + AppLogic.GetString("mailingtest.aspx.9", 1, thisUserAccount.LocaleSetting);
                }

                if (errMsg.Length != 0)
                {
                    return errMsg;
                }
            }
            return AppLogic.GetString("mailingtest.aspx.4", 1, thisUserAccount.LocaleSetting) + "<br/>&nbsp;�" + e.Message.ToString();
        }
        return AppLogic.GetString("mailingtest.aspx.2", 1, thisUserAccount.LocaleSetting);
    }

    private bool IsSendAdminOrderNofiicationEnabled()
    {
        return ((MultiPage1.SelectedIndex == 0 && rblSendOrderNotificationsSimple.SelectedIndex == 0) ||
                (MultiPage1.SelectedIndex == 1 && rblSendOrderNotificationsAdvanced.SelectedIndex == 0));
    }

    private String TestAll()
    {
        String ErrMsg = SendTestReceiptEmail();

        if (IsSendAdminOrderNofiicationEnabled())
        {
            ErrMsg = ErrMsg + "<br/><br/>" + SendTestNewOrderNotification();
        }
        else
        {
            ErrMsg = ErrMsg + "<br/><br/>" + AppLogic.GetString("mailingtest.aspx.5", 1, thisUserAccount.LocaleSetting);
        }
        return ErrMsg;
    }

    protected void btnSendAllSimple_Click(object sender, EventArgs e)
    {
        UpdateEmailConfigsSimple();
        AppLogic.ReloadAppConfigs();
        ltError.Text = TestAll();
        LoadContent();
    }

    protected void btnSendTestReceiptAdvanced_Click(object sender, EventArgs e)
    {
        UpdateEmailConfigsAdvanced();
        AppLogic.ReloadAppConfigs();
        ltError.Text = SendTestReceiptEmail();
    }

    protected void btnSendNewOrderNotificationAdvanced_Click(object sender, EventArgs e)
    {
        UpdateEmailConfigsAdvanced();
        AppLogic.ReloadAppConfigs();
        if (IsSendAdminOrderNofiicationEnabled())
        {
            ltError.Text = SendTestNewOrderNotification();
        }
        else
        {
            ltError.Text = AppLogic.GetString("mailingtest.aspx.5", 1, thisUserAccount.LocaleSetting);
        }
    }

    protected void btnSendAllAdvanced_Click(object sender, EventArgs e)
    {
        UpdateEmailConfigsAdvanced();
        AppLogic.ReloadAppConfigs();
        ltError.Text = TestAll();
    }
}


