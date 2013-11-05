// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.Globalization;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for googletopics.
    /// </summary>
    public partial class googletopics : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = new System.Text.UTF8Encoding();
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            int SkinID = 1; // not sure what to do about this...google can't invoke different skins easily
            String StoreLoc = AppLogic.GetStoreHTTPLocation(false);

            Response.Write("<urlset xmlns=\"" + AppLogic.AppConfig("GoogleSiteMap.Xmlns") + "\">");

            if (AppLogic.AppConfigBool("SiteMap.ShowTopics"))
            {
                // DB Topics:
                StringBuilder sql = new StringBuilder(2500);

                sql.Append("select wtl.[Name], wtl.Title, wtl.TopicID ");
                sql.Append("from EcommerceTopicLanguage wtl with (NOLOCK) ");
                sql.Append("inner join EcommerceTopic wt with (NOLOCK) on wt.TopicID=wtl.TopicID and wtl.WebSiteCode = wt.WebSiteCode ");
                sql.AppendFormat("where wtl.WebsiteCode={0} and wtl.LanguageCode={1} and ", DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode), DB.SQuote(Customer.Current.LanguageCode));
                sql.AppendFormat("wt.ShowInSiteMap=1 and (wt.SkinID IS NULL or wt.SkinID=0 or wt.SkinID={0})", SkinID.ToString());


                DataSet ds = DB.GetDS(sql.ToString(), AppLogic.CachingOn, System.DateTime.Now.AddMinutes(AppLogic.CacheDurationMinutes()));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Response.Write("<url>");
                    Response.Write("<loc>" + XmlCommon.XmlEncode(StoreLoc + SE.MakeDriverLink(DB.RowFieldByLocale(row, "Name", Localization.WebConfigLocale))) + "</loc> ");
                    Response.Write("<changefreq>" + AppLogic.AppConfig("GoogleSiteMap.TopicChangeFreq") + "</changefreq> ");
                    Response.Write("<priority>" + AppLogic.AppConfig("GoogleSiteMap.TopicPriority") + "</priority> ");
                    Response.Write("</url>");
                }
                ds.Dispose();

                // File Topics:
                // create an array to hold the list of files
                ArrayList fArray = new ArrayList();

                // get information about our initial directory
                String SFP = CommonLogic.SafeMapPath("skins/skin_" + SkinID.ToString() + "/template.htm").Replace("template.htm", "");

                DirectoryInfo dirInfo = new DirectoryInfo(SFP);

                // retrieve array of files & subdirectories
                FileSystemInfo[] myDir = dirInfo.GetFileSystemInfos();

                for (int i = 0; i < myDir.Length; i++)
                {
                    // check the file attributes

                    // if a subdirectory, add it to the sArray    
                    // otherwise, add it to the fArray
                    if (((Convert.ToUInt32(myDir[i].Attributes) & Convert.ToUInt32(FileAttributes.Directory)) > 0))
                    {
                        
                    }
                    else
                    {
                        bool skipit = false;
                        if (!myDir[i].FullName.EndsWith("HTM", StringComparison.InvariantCultureIgnoreCase) || 
                            (myDir[i].FullName.IndexOf("TEMPLATE", StringComparison.InvariantCultureIgnoreCase) != -1) || 
                            (myDir[i].FullName.IndexOf("AFFILIATE_", StringComparison.InvariantCultureIgnoreCase) != -1) || 
                            (myDir[i].FullName.IndexOf(AppLogic.ro_PMMicropay, StringComparison.InvariantCultureIgnoreCase) != -1))
                        {
                            skipit = true;
                        }
                        if (!skipit)
                        {
                            fArray.Add(Path.GetFileName(myDir[i].FullName));
                        }
                    }
                }

                if (fArray.Count != 0)
                {
                    // sort the files alphabetically
                    fArray.Sort(0, fArray.Count, null);
                    for (int i = 0; i < fArray.Count; i++)
                    {
                        Response.Write("<url>");
                        Response.Write("<loc>" + StoreLoc + SE.MakeDriverLink(fArray[i].ToString().Replace(".htm", "")) + "</loc> ");
                        Response.Write("<changefreq>" + AppLogic.AppConfig("GoogleSiteMap.TopicChangeFreq") + "</changefreq> ");
                        Response.Write("<priority>" + AppLogic.AppConfig("GoogleSiteMap.TopicPriority") + "</priority> ");
                        Response.Write("</url>");
                    }
                }
            }

            Response.Write("</urlset>");
        }

    }
}






