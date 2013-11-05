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

namespace InterpriseSuiteEcommerce
{

    // this provides a topic page that is NOT rendered in side the skin. The topic page itself should also contain the full <html>, not just the body.
    public partial class driver2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set the Customer context, and set the SkinBase context, so meta tags will be set if they are not blank in the Topic results
            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            Topic1.ThisCustomer = ThisCustomer;
            if (Topic1.TopicName.Length == 0)
            {
                String PN = CommonLogic.QueryStringCanBeDangerousContent("TopicName");
                if (PN.Length == 0)
                {
                    PN = CommonLogic.QueryStringCanBeDangerousContent("Topic");
                }
                if (PN.Length == 0)
                {
                    PN = CommonLogic.QueryStringCanBeDangerousContent("Topic");
                }
                if (PN.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    throw new ArgumentException("SECURITY EXCEPTION");
                }
                Topic1.TopicName = PN;
            }

            if (CommonLogic.FormCanBeDangerousContent("Password").Length != 0)
            {
                ThisCustomer.ThisCustomerSession["Topic" + Topic1.TopicName] = CommonLogic.FormCanBeDangerousContent("Password");
            }
        }
    }
}