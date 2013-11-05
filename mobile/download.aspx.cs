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
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;

public partial class download : System.Web.UI.Page
{
    private Customer ThisCustomer = Customer.Current;

    private void RedirectNonRegisteredUser()
    {
        string requestedPage = Security.UrlEncode(Request.Url.PathAndQuery);
        string signinAndGoBackLink = string.Format("signin.aspx?returnurl={0}", requestedPage);

        string html =
        string.Format("<html><body><div align=\"center\">You need to <a href=\"{0}\">login</a> to download this file. Click <a href=\"{0}\">here</a> to signin.</div></body></html>", signinAndGoBackLink);

        Response.Clear();
        Response.Write(html);
        Response.Flush();
        Response.End();
    }

    protected override void OnInit(EventArgs e)
    {
        if (string.IsNullOrEmpty(CommonLogic.QueryStringCanBeDangerousContent("d")))
        {
            HttpResponseHelper.RespondWithFileNotFound(Response);
            return;
        }
		
		//removed the binding of event to the html and code it here for mobile
        txtCaptcha.TextChanged += txtCaptcha_TextChanged;
        btnDownload.Click += btnDownload_Click;
        btnDownload.Text = "Download";

        ThisCustomer.RequireCustomerRecord();
        //Make sure the current customer is logged in.
        if (ThisCustomer.IsNotRegistered)
        {
            RedirectNonRegisteredUser();
        }
        else
        {
            // get the querystring for the download id
            string strDownloadId = CommonLogic.QueryStringCanBeDangerousContent("d");
            // get the querystring for the order id
            string orderId = CommonLogic.QueryStringCanBeDangerousContent("sid");

            if (InterpriseHelper.IsCorrectCustomer(ThisCustomer, orderId))
            {
                //The customer is either logged in and its their download, or the customer is anonymuos and the download
                //is for an anonymous customer.
                divSignInPrompt.Visible = false;
                lblCaption.Text = "Please enter the text on the image below";
                divDownload.Visible = true;

                InterpriseHelper.ClearCustomerDownloadableLinkFromSession(ThisCustomer);

                if (!IsPostBack)
                {
                    GenerateAndShowCaptchaImage();
                }
            }
            else{
                txtCaptcha.Visible = false;
                btnDownload.Visible = false;                
                lblError.Text = "You are not allowed to download this file since this belongs to a different customer!";
            }
        }

        base.OnInit(e);
    }

    private void GenerateAndShowCaptchaImage()
    {
        imgCaptcha.ImageUrl = "Captcha.ashx?id=1";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");
    }


    protected void btnDownload_Click(object sender, EventArgs e)
    {
        if (Session["SecurityCode"].Equals(txtCaptcha.Text))
        {
            string downloadId = CommonLogic.QueryStringCanBeDangerousContent("d");
            string orderId = CommonLogic.QueryStringCanBeDangerousContent("sid");
            Response.Redirect("download.axd?d=" + downloadId + "&sid=" + orderId);
        }
        else
        {
            txtCaptcha.Text = string.Empty;
            GenerateAndShowCaptchaImage();
            lblError.Text = "You have entered a wrong security code, please try again";
        }
    }
    protected void txtCaptcha_TextChanged(object sender, EventArgs e)
    {
        btnDownload_Click(this, e);
    }
}
