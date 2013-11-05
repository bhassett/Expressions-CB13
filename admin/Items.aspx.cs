using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Authentication;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure; 

public partial class admin_Items : System.Web.UI.Page
{

    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        // check if there is a view state. if there is redirect to signin.aspx
        string strViewState = Request.Form["__VIEWSTATE"];
        if (string.IsNullOrEmpty(strViewState))
        {
            Master.RenderHeaderIncludes += MasterPage_RenderHeaderIncludes;
            base.OnInit(e);

            ScriptManagerProxy1.Scripts.Add(new ScriptReference("js/common_ajax.js"));
            ScriptManagerProxy1.Scripts.Add(new ScriptReference("js/filter_ajax.js"));

            ServiceReference service = new ServiceReference("~/actionservice.asmx");
            service.InlineScript = false;
            ScriptManagerProxy1.Services.Add(service);
        }
        else
        {
            //Call the signout method when event target is lnkLogout
            string strEventTarget = Request.Form["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(strEventTarget) && strEventTarget == "ctl00$lnkLogout")
                ServiceFactory.GetInstance<IAuthenticationService>()
                              .SignOutAdmin();
        }

        base.OnInit(e);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region Events

    private void MasterPage_RenderHeaderIncludes(object sender, RenderHeaderEventArgs e)
    {
    }

    #endregion

}