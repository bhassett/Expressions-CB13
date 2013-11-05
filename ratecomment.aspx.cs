// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Text;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;
using System.Data.SqlClient;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for ratecomment.
	/// </summary>
	public partial class ratecomment : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.CacheControl="private";
			Response.Expires=0;
			Response.AddHeader("pragma", "no-cache");

            Customer ThisCustomer = ((InterpriseSuiteEcommercePrincipal)Context.User).ThisCustomer;
            ThisCustomer.RequireCustomerRecord();

			String ProductID = CommonLogic.QueryStringCanBeDangerousContent("ProductID");
            String ItemCode = ProductID;
            String VotingCustomerID = CommonLogic.QueryStringCanBeDangerousContent("VotingCustomerID");
            String VotingContactID = CommonLogic.QueryStringCanBeDangerousContent("VotingContactID");
			String CustomerID = CommonLogic.QueryStringCanBeDangerousContent("CustomerID");
            String ContactID = CommonLogic.QueryStringCanBeDangerousContent("ContactID");            
			String MyVote = CommonLogic.QueryStringCanBeDangerousContent("MyVote").ToUpperInvariant();
			int HelpfulVal = CommonLogic.IIF(MyVote == "YES" , 1 , 0);
			bool IsProduct = CommonLogic.QueryStringBool("IsProduct");

			bool AlreadyVoted = false;

            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader rs = DB.GetRSFormat(con, "SELECT * FROM EcommerceRatingCommentHelpfulness with (NOLOCK) WHERE ItemCode=" + 
                                                                DB.SQuote(ItemCode) + " and RatingCustomerCode=" + DB.SQuote(CustomerID) +
                                                                " and VotingCustomerCode=" + DB.SQuote(VotingCustomerID) + " and RatingContactCode=" + DB.SQuote(ContactID) +
                                                                " and VotingContactCode=" + DB.SQuote(VotingContactID)))
                {
                    if (rs.Read())
                    {
                        AlreadyVoted = true;
                        // they have already voted on this comment, and are changing their minds perhaps, so adjust totals, and reapply vote:
                        if (DB.RSFieldBool(rs, "Helpful"))
                        {
                            DB.ExecuteSQL("UPDATE EcommerceRating SET FoundHelpful = FoundHelpful-1 WHERE ItemCode=" +
                                DB.SQuote(ItemCode) + " AND CustomerCode=" + DB.SQuote(CustomerID) +
                                " AND ContactCode=" + DB.SQuote(ContactID));
                        }
                        else
                        {
                            DB.ExecuteSQL("UPDATE EcommerceRating SET FoundNotHelpful = FoundNotHelpful-1 WHERE ItemCode=" +
                                DB.SQuote(ItemCode) + " AND CustomerCode=" + DB.SQuote(CustomerID) +
                                " AND ContactCode=" + DB.SQuote(ContactID));
                        }
                    }
                }
            }

			if(AlreadyVoted)
			{
                DB.ExecuteSQL("DELETE FROM EcommerceRatingCommentHelpfulness WHERE ItemCode=" + 
                    DB.SQuote(ItemCode) + " AND RatingCustomerCode=" + DB.SQuote(CustomerID) + " AND VotingCustomerCode=" + DB.SQuote(VotingCustomerID)
                    + " AND RatingContactCode=" + DB.SQuote(ContactID) + " AND VotingContactCode=" + DB.SQuote(VotingContactID));
			}

            DB.ExecuteSQL("INSERT INTO EcommerceRatingCommentHelpfulness(ItemCode,RatingCustomerCode,VotingCustomerCode,Helpful, WebsiteCode,RatingContactCode,VotingContactCode) VALUES(" +
                DB.SQuote(ItemCode) + "," + DB.SQuote(CustomerID) + "," + DB.SQuote(VotingCustomerID) + "," + HelpfulVal.ToString() + "," + DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode) + "," + DB.SQuote(ContactID) + "," + DB.SQuote(VotingContactID) + ")");
			if(MyVote == "YES")
			{
                DB.ExecuteSQL("UPDATE EcommerceRating SET FoundHelpful = FoundHelpful+1 WHERE ItemCode=" +
                    DB.SQuote(ItemCode) + " AND CustomerCode=" + DB.SQuote(CustomerID) + " AND ContactCode=" + DB.SQuote(ContactID));
			}
			else
			{
                DB.ExecuteSQL("UPDATE EcommerceRating SET FoundNotHelpful = FoundNotHelpful+1 WHERE ItemCode=" +
                    DB.SQuote(ItemCode) + " and CustomerCode=" + DB.SQuote(CustomerID) + " AND ContactCode=" + DB.SQuote(ContactID));
			}

			Response.Write("<html>\n");
			Response.Write("<head>\n");
			Response.Write("<title>Rate Comment</title>\n");
			Response.Write("</head>\n");
			Response.Write("<body>\n");
			Response.Write("<!-- INVOCATION: " + CommonLogic.PageInvocation() + " -->\n");
			Response.Write("</body>\n");
			Response.Write("</html>\n");

		}

	}
}






