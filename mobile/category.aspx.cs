using System;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    public partial class category : SkinBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set the Customer context, and set the SkinBase context, so meta tags to be set if they are not blank in the XmlPackage results
            Package1.SetContext = this;
        }

        override protected void OnInit(EventArgs e)
        {
            String HT = AppLogic.AppConfig("HomeTemplate");
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