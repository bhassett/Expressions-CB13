using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;

public partial class UserControls_ProfileControl : System.Web.UI.UserControl
{
    public bool _isAnonymousCheckout = false;

    public bool exCludeAccountName { get; set; }
    public bool UseFullnameTextbox { get; set; }
    public bool showEditPasswordArea { get; set; }
    public bool showSalutation { get; set; }
    public bool hideAccountNameArea { get; set; }
    public string selectedSalutation { get; set; }
    public bool isAnonymousCheckout {
        get
        {
            return _isAnonymousCheckout;
        }
        set
        {
            _isAnonymousCheckout = value;
        }
    }

    public string salutation
    {
        get
        {
            return drpLstSalutation.Text;
        }
        set
        {
            drpLstSalutation.Text = value;
        }
    }

    public string firstName
    {
        get
        {
            return txtFirstName.Text;
        }
        set
        {
            txtFirstName.Text = value;
        }
    }

    public string lastName
    {
        get
        {
            return txtLastName.Text;
        }
        set
        {
            txtLastName.Text = value;
        }
    }


    public string contactNumber
    {
        get
        {
            return txtContactNumber.Text;
        }
        set
        {
            txtContactNumber.Text = value;
        }
    }

    public string email
    {
        get
        {
            return txtEmail.Text;
        }
        set
        {
            txtEmail.Text = value;
        }
    }

    public string accountName
    {
        get
        {
            return txtAccountName.Text;
        }
        set
        {
            txtAccountName.Text = value;
        }
    }

    public string password
    {
        get
        {
            return txtPassword.Text;
        }
        set
        {
            txtPassword.Text = value;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Initialize();
    }

    private void Initialize()
    {
        if (!IsPostBack)
        {
            this.showSalutation = AppLogic.AppConfigBool("Address.ShowSalutation") && !this.isAnonymousCheckout;

            if (!UseFullnameTextbox && showSalutation)
            {
                drpLstSalutation.Items.Clear();
                AppLogic.AddItemsToDropDownList(ref drpLstSalutation, "Salutation", false);
            }

            drpLstSalutation.SelectedIndex = drpLstSalutation.Items.IndexOf(drpLstSalutation.Items.FindByText(selectedSalutation));
        }
    }

}