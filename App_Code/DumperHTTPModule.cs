// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/// <summary>
/// Summary description for DumperHTTPModule
/// </summary>
public class DumperHTTPModule : IHttpModule
{
    public DumperHTTPModule()
    {
        
    }

    #region IHttpModule Members

    public void Dispose()
    {
    }

    public void Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(context_BeginRequest);
    }

    void context_BeginRequest(object sender, EventArgs e)
    {
        HttpContext context = (sender as HttpApplication).Context;
        string time = DateTime.Now.TimeOfDay.ToString().Replace(":", ".");
        string dumpFileNameWithHeader = Path.Combine(context.Server.MapPath(@"dumps"), time) + ".txt";
        string dumpFileNameWithoutHeader = Path.Combine(context.Server.MapPath(@"dumps"), time) + ".xml";

        context.Request.SaveAs(dumpFileNameWithHeader, true);
        context.Request.SaveAs(dumpFileNameWithoutHeader, false);
    }

    #endregion
}
