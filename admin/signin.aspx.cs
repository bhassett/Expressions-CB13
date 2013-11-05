// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

public partial class admin_signin : System.Web.UI.Page
{
    #region Events

    protected override void OnInit(EventArgs e)
    {
        this.loginCtrl.LoggingIn += (sender, ex) =>
        {
            ex.Cancel = true;
            DoAuthentication();
        };

        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) { return; }

        AutoLogin();
    }

    #endregion

    #region Private Methods

    private void DisplayLoginFailed()
    {
        lblError.Text = "Invalid Login.";
    }

    private void DisplayAccountLocked()
    {
        lblError.Text = "Your account has been locked due to too many failed login attempts.  Please contact the site administrator.";
    }

    private void AutoLogin()
    {
        if (ServiceFactory.GetInstance<IAuthenticationService>()
                          .IsAdminCurrentlyLoggedIn())
        {
            ServiceFactory.GetInstance<INavigationService>()
                          .NavigateToWebAdminDefaultPage();
        }
    }

    private void DoAuthentication()
    {
        var status = ServiceFactory.GetInstance<IAuthenticationService>()
                                   .TryLoginAdmin(loginCtrl.UserName, loginCtrl.Password);

        if (!status.IsSuccess)
        {
            if (!status.IsLocked)
            {
                DisplayLoginFailed();
                return;
            }
            else
            {
                DisplayAccountLocked();
            }
        }
        else
        {
            ServiceFactory.GetInstance<INavigationService>()
                          .NavigateToWebAdminDefaultPage(true);
        }
    }

    #endregion
}