using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon;

public partial class UserControls_GiftRegistry_GiftRegistryList : System.Web.UI.UserControl
{

    #region Initialization

    protected void Page_Init(object sender, EventArgs e)
    {
        BindControls();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        lblNoRecord.Text = AppLogic.GetString("giftregistry.aspx.5", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
    }

    #endregion

    #region Methods

    void BindControls() { }

    #endregion

    #region Properties

    public IEnumerable<GiftRegistry> GiftRegistries
    {
        set
        {
            lblNoRecord.Visible = value.Count() == 0;

            rptGiftRegistries.DataSource = value;
            rptGiftRegistries.DataBind();
        }
    }

    public Customer ThisCustomer { get; set; }

    #endregion

}