// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for disclaimer.
    /// </summary>
    public partial class disclaimer : System.Web.UI.Page
    {

        protected override void OnInit(EventArgs e)
        {
			//mobile button setup
            AgreeButton.Text = "I Agree";
            DoNotAgreeButton.Text = "I Do NOT Agree";

            //anonymous method
            DoNotAgreeButton.Click += (sender, evt) =>
            {
                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", string.Empty);
                Response.Redirect(AppLogic.AppConfig("SiteDisclaimerNotAgreedURL"));
            };

            base.OnInit(e);
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            if (IsPostBack)
            {
                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", CommonLogic.GetNewGUID());
                Panel1.Visible = false;
                Response.AddHeader("REFRESH", "1; URL=" + ReturnURL.Text);
            }
            else
            {
                DisclaimerContents.Text = new Topic("SiteDisclaimer", 1).Contents;

                AppLogic.SetSessionCookie("SiteDisclaimerAccepted", String.Empty);
                ReturnURL.Text = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
                AppLogic.CheckForScriptTag(ReturnURL.Text);
                if (ReturnURL.Text.Length == 0)
                {
                    ReturnURL.Text = AppLogic.AppConfig("SiteDisclaimerAgreedPage");
                    if (ReturnURL.Text.Length == 0)
                    {
                        if (CommonLogic.QueryStringBool("checkout"))
                        {
                            ReturnURL.Text = "shoppingcart.aspx?checkout=true";
                        }
                        else
                        {
                            ReturnURL.Text = "default.aspx";
                        }
                    }
                }
                // if disclaimer was already accepted, just send them on their way:
                if (CommonLogic.CookieCanBeDangerousContent("SiteDisclaimerAccepted", true).Length != 0)
                {
                    Response.Redirect(ReturnURL.Text);
                }
            }
        }
    }
}