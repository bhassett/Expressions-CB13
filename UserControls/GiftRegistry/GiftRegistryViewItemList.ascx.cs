using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;

public partial class UserControls_GiftRegistry_GiftRegistryViewItemList : System.Web.UI.UserControl
{
    #region Initialization

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    #endregion

    #region Properties

    public Customer ThisCustomer { get; set; }

    public Guid? RegistryID
    {
        set
        {
            ViewState["RegistryID"] = value;
        }
        get
        {
            if (ViewState["RegistryID"] != null)
            {
                return (Guid)ViewState["RegistryID"];
            }
            return null;
        }
    }

    public string CustomUrl { get; set; }

    public IEnumerable<GiftRegistryItem> GiftRegistryItems
    {
        set
        {
            rptGiftRegistryItemList.DataSource = value;
            rptGiftRegistryItemList.DataBind();
        }
    }

    #endregion
}