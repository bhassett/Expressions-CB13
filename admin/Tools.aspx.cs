using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using InterpriseSuiteEcommerceCommon.Tool;
using System.IO;

public partial class admin_Tools : System.Web.UI.Page
{
    public class FacebookPage
    {
        public string id { get; set; }   
    }

    #region Variables
    private const string FACEBOOK_GRAPH_API = "https://graph.facebook.com/";
    private const string FACEBOOK_GRAPH_API_POSTS = "https://graph.facebook.com/{0}/posts?limit={1}&access_token={2}";
    #endregion

    #region Methods
    private string GetFacebookPageID(string url)
    {
        string facebookPageID = string.Empty;

        try
        {
            /*
              sample of different facebook page formats 
              - www.facebook.com/my_page_name
              - www.facebook.com/pages/my_page_name
              - www.facebook.com/my_page_ID 
            */
            string identifier = url.Substring(url.LastIndexOf("/") + 1);  //get the page_name or the page_id
            string graphUrl = FACEBOOK_GRAPH_API + identifier;

            string json = string.Empty; 
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(graphUrl);
            httpWebRequest.Method = WebRequestMethods.Http.Get;
            httpWebRequest.Accept = "application/json";

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }

            FacebookPage fbPage = new JavaScriptSerializer().Deserialize<FacebookPage>(json);
            facebookPageID = fbPage.id;
        }
        catch (Exception)
        {
            facebookPageID = string.Empty;
        }

        return facebookPageID;
    }
    private string GenerateFacebookAPI(string facebookPageID, string postLimit, string accessToken)
    {
        return string.Format(FACEBOOK_GRAPH_API_POSTS, facebookPageID, postLimit, accessToken);
    }
    #endregion

    #region Events
    protected override void OnInit(EventArgs e)
    {
        this.Title = "InterpriseSuiteEcommerce - Tools";

        btnGenerateFacebookFeedboxAPI.Click += new EventHandler(btnGenerateFacebookFeedboxAPI_Click);

        base.OnInit(e);
    }

    protected void btnGenerateFacebookFeedboxAPI_Click(object sender, EventArgs e)
    {
        string sFacebookID = GetFacebookPageID(txtFacebookURL.Text);

        if (sFacebookID != string.Empty)
        {
            lblFacebookFeedboxAPI.Text = GenerateFacebookAPI(sFacebookID, txtPostLimit.Text, txtAccessToken.Text);
            lblFacebookFeedboxAPI.ForeColor = Color.Black;
        }
        else
        {
            lblFacebookFeedboxAPI.Text = "Invalid facebook page url";
            lblFacebookFeedboxAPI.ForeColor = Color.Red;
        }
        divFacebookFeedboxResult.Visible = true;
    }  

    protected void Page_Load(object sender, EventArgs e)
    {
    }
    #endregion
}