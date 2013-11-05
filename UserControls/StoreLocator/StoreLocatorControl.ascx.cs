using System.Collections.Generic;
using System.Linq;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.StoreLocator;
using System;

public partial class UserControls_StoreLocator_StoreLocatorControl : System.Web.UI.UserControl, ILocatorSetting 
{
    public void LoadLocatorSetting(ILocatorSetting locatorSettings)
    {
        AuthenticationTokenKey = locatorSettings.AuthenticationTokenKey;
        Sensor = locatorSettings.Sensor;
        Version = locatorSettings.Version;
        Opton = locatorSettings.Opton;
    }

    public override void DataBind()
    {
        PopulateStoreType();
        base.DataBind();
    }

    private void PopulateStoreType()
    {
        var items = EnumExtensions.GetAllItems<StoreType>();
        dlStoreType.DataTextField = "Text";
        dlStoreType.DataValueField = "Value";
        dlStoreType.DataSource = items.Select(item =>
                                                new
                                                {
                                                    Text = item.ToString(),
                                                    Value = ((int)item).ToString()
                                                });
        dlStoreType.DataBind();
    }

    public string AuthenticationTokenKey
    {
        get
        {
            if(ViewState["AuthenticationID"] != null)
            {
                return ViewState["AuthenticationID"].ToString();
            }
            return string.Empty;
        }
        set
        {
            ViewState["AuthenticationID"] = value;
        }
    }

    public bool Sensor
    {
        get
        {
            if (ViewState["Sensor"] != null)
            {
                return bool.Parse(ViewState["Sensor"].ToString());
            }
            return false;
        }
        set
        {
            ViewState["Sensor"] = value;
        }
    }

    public string Version
    {
        get
        {
            if (ViewState["Version"] != null)
            {
                return ViewState["Version"].ToString();
            }
            return string.Empty;
        }
        set
        {
            ViewState["Version"] = value;
        }
    }

    public LocatorOption Opton
    {
        get
        {
            if (ViewState["Opton"] != null)
            {
                return ViewState["Opton"] as LocatorOption;
            }
            return null;
        }
        set
        {
            ViewState["Opton"] = value;
        }
    }

    public Customer ThisCustomer 
    {
        get
        {
            if (ViewState["ThisCustomer"] != null)
            {
                return ViewState["ThisCustomer"] as Customer;
            }
            return null;
        }
        set
        {
            ViewState["ThisCustomer"] = value;
        }
    }

}