using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;

public partial class UserControls_ShippingAddressControl : System.Web.UI.UserControl
{
    protected override void OnLoad(EventArgs e)
    {

        base.OnLoad(e);
        this._init();

    }

    private void _init()
    {

        if (!IsPostBack)
        {

            shipping_country_input_select.Items.Clear();
            AppLogic.AddItemsToDropDownList(ref shipping_country_input_select, "Country", true);

            string addressTypesLabel = AppLogic.GetString("address.cs.21", Customer.Current.SkinID, Customer.Current.LocaleSetting);
           
            shipping_address_type.Items.Add(addressTypesLabel);

            shipping_address_type.Items.Add(ResidenceTypes.Residential.ToString());
            shipping_address_type.Items.Add(ResidenceTypes.Commercial.ToString());

        }

    }
}