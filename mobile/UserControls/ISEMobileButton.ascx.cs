using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mobile_UserControls_ISEMobileButton : System.Web.UI.UserControl, IButtonControl
{
    public event CommandEventHandler Command;
    public event EventHandler Click;

    protected override void OnInit(EventArgs e)
    {
        lnkMobileButton.Click += new EventHandler(lnkMobileButton_Click);
        base.OnInit(e);
    }

    protected void Page_Load(EventArgs e)
    {
        Initialized();
    }

    protected void Initialized()
    {
        lnkMobileButton.Visible = true;
        lnkMobileButton.Command += Command;
        lnkMobileButton.CommandArgument = CommandArgument;
        lnkMobileButton.CommandName = CommandName;
        lnkMobileButton.PostBackUrl = PostBackUrl;
        lnkMobileButton.ValidationGroup = ValidationGroup;
        lnkMobileButton.OnClientClick = OnClientClick;
        lnkMobileButton.CausesValidation = CausesValidation;
    }

    protected override void CreateChildControls()
    {
        LabelText.Controls.Add(litText);
        lnkMobileButton.Controls.Add(LabelText);
        base.CreateChildControls();
    }

    public string Text { 
        get {
            return litText.Text;
        }
        set {
            litText.Text = value;
        }
    }

    public string PostBackUrl 
    { 
        get {
            return lnkMobileButton.PostBackUrl;
        }
        set {
            lnkMobileButton.PostBackUrl = value;
        }
    }

    public string ValidationGroup 
    {
        get { return lnkMobileButton.ValidationGroup; }
        set { lnkMobileButton.ValidationGroup = value; }
    }

    public string OnClientClick { 
        get 
        {
            return lnkMobileButton.OnClientClick;
        }
        set
        {
            lnkMobileButton.OnClientClick = value;
        }
    }

    public bool CausesValidation { 
        get {
            return lnkMobileButton.CausesValidation;
        }
        set {
            lnkMobileButton.CausesValidation = value;
        }
    }

    public string CommandName {
        get { return lnkMobileButton.CommandName; }
        set { lnkMobileButton.CommandName = value; }
    }

    public string CommandArgument {
        get { return lnkMobileButton.CommandArgument; }
        set { lnkMobileButton.CommandArgument = value; }
    }

    void lnkMobileButton_Click(object sender, EventArgs e)
    {
        if (Click != null)
        {
            Click(sender, e);
        }
    }

    public bool Enabled {
        get 
        {
            return lnkMobileButton.Enabled;
        }
        set 
        {
            lnkMobileButton.Enabled = value;
        }
    }

    public bool Visible {
        get {
            return lnkMobileButton.Visible;
        }
        set {
            lnkMobileButton.Visible = value;
        }
    }

    public LinkButton TheButton
    {
        get {
            return lnkMobileButton;
        }
    }

}