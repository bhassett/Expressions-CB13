using System;
using System.Collections.Generic;
using System.Web;

using Interprise.Framework.ECommerce.DatasetGateway;

/// <summary>
/// Used to store and retrive data from the Session State.
/// </summary>
public static class SessionStateSink
{
    #region Constants
    private const string ADMIN_APP_CONFIG_GATEWAY = "AdminAppConfigGateway";
    #endregion

    #region AdminAppConfigGateway
    /// <summary>
    /// Gets or Sets the gatway object used for storing data in the admin AppConfig.aspx page.
    /// </summary>
    public static ApplicationConfigurationDatasetGateway AdminAppConfigGateway
    {
        get
        {
            return (ApplicationConfigurationDatasetGateway)HttpContext.Current.Session[ADMIN_APP_CONFIG_GATEWAY];
        }
        set
        {
            HttpContext.Current.Session.Add(ADMIN_APP_CONFIG_GATEWAY, value);
        }
    }
    /// <summary>
    /// Removes the value stored in session.
    /// </summary>
    public static void RemoveAdminAppConfigGateway()
    {
        HttpContext.Current.Session.Remove(ADMIN_APP_CONFIG_GATEWAY);
    }
    #endregion
}
