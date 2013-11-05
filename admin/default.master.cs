// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

public partial class _default : System.Web.UI.MasterPage
{
    #region Member Variable

    public event EventHandler<RenderHeaderEventArgs> RenderHeaderIncludes;

    #endregion

    #region Events

    protected override void OnInit(EventArgs e)
    {
        var authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        authenticationService.AuthenticateAdmin();

        base.OnInit(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        RenderHeaderEventArgs ev = new RenderHeaderEventArgs();
        OnRenderHeaderIncludes(ev);

        base.Render(writer);
    }

    protected virtual void OnRenderHeaderIncludes(RenderHeaderEventArgs e)
    {
        if (null != RenderHeaderIncludes)
        {
            RenderHeaderIncludes(this, e);

            if (e.Includes.Count > 0)
            {
                foreach (string includeLine in e.Includes)
                {
                    litHeader.Text += includeLine;
                    litHeader.Text += "\n";
                }
            }
        }
    }

    protected void LinkButton_Click(object sender, EventArgs e)
    {
        if (sender == lnkResetCache)
        {
            DoResetCache();
        }
        else if (sender == lnkLogout)
        {
            DoSignOut();
        }
    }

    #endregion

    #region Methods

    private void DoSignOut()
    {
        var appConfigService = ServiceFactory.GetInstance<IAppConfigService>();

        //for CMS editor reset
        if (appConfigService.CacheMenus)
        {
            var customer = Security.GetCurrentlyLoggedInUser();
            if (customer != null && customer.ThisCustomerSession[DomainConstants.CMS_ENABLE_EDITMODE] != null)
            {
                bool? isEditing = customer.ThisCustomerSession[DomainConstants.CMS_ENABLE_EDITMODE].TryParseBool();

                if (isEditing.HasValue && isEditing.Value)
                {
                    var appCacheService = ServiceFactory.GetInstance<IApplicationCachingService>();
                    appCacheService.Reset();
                }
            }
        }

        var authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        authenticationService.SignOutAdmin();
    }

    private void DoResetCache()
    {
        AppLogic.m_RestartApp();
        ReloadSiteMap();

    }

    private void ReloadSiteMap()
    {
        if (System.Web.SiteMap.Enabled)
        {
            InterpriseSuiteEcommerce.ISESiteMapProviderFactory.Instance.Reset();
        }
    }

    #endregion
}
