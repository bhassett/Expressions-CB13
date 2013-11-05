using System;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    public partial class _default : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CommonLogic.ServerVariables("HTTP_HOST").IndexOf(AppLogic.AppConfig("LiveServer"), StringComparison.InvariantCultureIgnoreCase) != -1 &&
                CommonLogic.ServerVariables("HTTP_HOST").IndexOf("WWW", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                if (AppLogic.AppConfigBool("RedirectLiveToWWW"))
                {
                    Response.Redirect("http://www." + AppLogic.AppConfig("LiveServer").ToLowerInvariant() + "/default.aspx?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            // this may be overwridden by the XmlPackage below!
            SectionTitle = string.Format(AppLogic.GetString("default.aspx.1", SkinID, ThisCustomer.LocaleSetting), AppLogic.AppConfig("StoreName"));

            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
        }

        override protected void OnInit(EventArgs e)
        {
            string HT = AppLogic.AppConfig("HomeTemplate");
            if (HT.Length != 0)
            {
                if (!HT.EndsWith(".ascx"))
                {
                    HT = HT + ".ascx";
                }
                SetTemplate(HT);
            }
            base.OnInit(e);
        }

    }
}