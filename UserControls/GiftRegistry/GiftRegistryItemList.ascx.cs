using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;

public partial class UserControls_GiftRegistry_GiftRegistryItemList : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        BindControls();
    }

    #region Methods

    void InitResources()
    { 
    }

    void BindControls() 
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