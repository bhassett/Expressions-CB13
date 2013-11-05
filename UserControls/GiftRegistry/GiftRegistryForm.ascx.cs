using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using System.Web.UI.HtmlControls;

public partial class UserControls_GiftRegistry_GiftRegistryForm : System.Web.UI.UserControl
{
    #region Initialization

    protected void Page_Init(object sender, EventArgs e)
    {
        BindControls();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
    }

    #endregion

    #region Methods

    void BindControls() { }

    public void LoadGiftRegistry(GiftRegistry giftRegistry)
    {
        if (giftRegistry == null) return;

        ctrlDatePickerStartDate.DateValue = giftRegistry.StartDate;
        ctrlDatePickerEndDate.DateValue = giftRegistry.EndDate;

        Title = giftRegistry.Title;
        GuestMessage = giftRegistry.Message;
        PrivatePrivacy = giftRegistry.IsPrivate;
        GuestPassword = giftRegistry.GuestPassword;
        CustomURL = giftRegistry.CustomURLPostfix;
        PictureFileName = giftRegistry.PictureFileName;

        txtGuestPassword.Enabled = (giftRegistry.IsPrivate);
    }

    #endregion

    #region Properties

    public bool IsEditMode 
    {
        get 
        {
            bool retval = false;
            if (ViewState["IsEditMode"] != null)
            { 
                retval = (bool)ViewState["IsEditMode"];
            }
            return retval;
        }
        set 
        {
            ViewState["IsEditMode"] = value;
        }
    }

    public Customer ThisCustomer
    {
        get 
        {
            Customer customer= null;
            if (ViewState["ThisCustomer"] != null)
            {
                customer = ViewState["ThisCustomer"] as Customer;
            }
            return customer;
        }
        set 
        {
            ctrlDatePickerStartDate.SkinID = value.SkinID;
            ctrlDatePickerEndDate.SkinID = value.SkinID;
            ViewState["ThisCustomer"] = value;
        }
    }

    public Guid? RegistryID
    {
        get
        {
            var guid = Guid.NewGuid();
            if (ViewState["RegistryID"] != null)
            {
                guid = (Guid)ViewState["RegistryID"];
            }
            return guid;
        }
        set
        {
            ViewState["RegistryID"] = value;
        }
    }

    public DateTime? StartDate 
    { 
        get 
        {
            return ctrlDatePickerStartDate.DateValue;
        }
        set 
        {
            ctrlDatePickerStartDate.DateValue = value;
        }
    }

    public DateTime? EndDate
    {
        get
        {
            return ctrlDatePickerEndDate.DateValue;
        }
        set
        {
            ctrlDatePickerEndDate.DateValue = value;
        }
    }

    public string Title 
    {
        get 
        {
            return txtTitle.Text;
        }
        set 
        {
            txtTitle.Text = value;
        } 
    }

    public string GuestMessage 
    {
        get 
        {
            return WebEditorControl.Value;
        }
        set 
        {
            WebEditorControl.Value = value;
        }
    }

    public string GuestPassword
    {
        get
        {
            return txtGuestPassword.Text;
        }
        set
        {
            txtGuestPassword.Text = value;
        }
    }

    public string PictureFileName 
    {
        get 
        {
            if (!txtFileUpload.FileName.IsNullOrEmptyTrimmed()) return txtFileUpload.FileName;

            if (ViewState["PictureFileName"] != null)
            {
                return ViewState["PictureFileName"].ToString();
            }
            return string.Empty;
        }
        set 
        {
            ViewState["PictureFileName"] = value;
        }
    }

    public System.IO.Stream PhotoStream
    {
        get
        {
            return txtFileUpload.FileContent;
        }
    }

    public bool PrivatePrivacy
    {
        get 
        {
            return rdPrivate.Checked;
        }
        set 
        {
            if (value)
            {
                rdPrivate.Checked = true;
            }
            else
            {
                rdPublic.Checked = true;
            }

        }
    }

    public string CustomURL 
    {
        get 
        {
            return txtCustomURL.Text;
        }
        set 
        {
            txtCustomURL.Text = value;
        }
    }

    public string PrimaryURLText 
    {
        get 
        {
            return lblprimaryurl.Text;
        }
        set 
        {
            lblprimaryurl.Text = value;
        }
    }

    public HtmlAnchor LinkPreview
    {
        get 
        {
            return lnkPreView;
        }
    }

    #endregion



}