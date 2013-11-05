// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for pollvote.
	/// </summary>
	public partial class pollvote : System.Web.UI.Page
	{
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            string queryStr = string.Empty;
			ThisCustomer.RequireCustomerRecord();

			String PollID = HttpUtility.UrlDecode(CommonLogic.FormCanBeDangerousContent("PollID"));
            String CustomerID = ThisCustomer.CustomerCode;
			String PollAnswerID = HttpUtility.UrlDecode(CommonLogic.FormCanBeDangerousContent("Poll_" + PollID.ToString()));

            if (PollID != String.Empty && CustomerID != String.Empty && PollAnswerID != String.Empty)
			{
				// record the vote:

                bool pollFound = false;

				try
				{
                    Poll votedPoll = Poll.GetPoll(PollID, ThisCustomer);
                    if (null != votedPoll)
                    {
                        votedPoll.RecordVote(ThisCustomer, int.Parse(PollAnswerID));
                        pollFound = true;
                    }
				}
				catch {}

                if (pollFound)
                {
                    Response.Redirect(string.Format("polls.aspx?pollid={0}", PollID));
                }
			}

            Response.Redirect("polls.aspx");
		}
	}
}
