using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;

public partial class UserControls_GiftRegistry_GiftRegistryViewForm : System.Web.UI.UserControl
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

    public void LoadGiftRegistry(GiftRegistry giftRegistry)
    {
        if (giftRegistry == null) return;

        StartDate = giftRegistry.StartDate;
        EndDate = giftRegistry.EndDate;
        Title = giftRegistry.Title;
        GuestMessage = giftRegistry.Message;
        PictureFileName = giftRegistry.PictureFileName;
        PrivatePrivacy = giftRegistry.IsPrivate;
        GuestPassword = giftRegistry.GuestPassword;
        CustomURL = giftRegistry.CustomURLPostfix;
        OwnerFullName = giftRegistry.OwnersFullName;
    }

    void BindControls() { }

    #endregion

    #region Properties

    public Customer ThisCustomer
    {
        get;
        set;
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
            DateTime? dt = null;
            if (ViewState["StartDate"] != null)
            {
                dt = (DateTime)ViewState["StartDate"];
            }
            return dt;
        }
        set
        {
            ViewState["StartDate"] = value;
        }
    }

    public DateTime? EndDate
    {
        get
        {
            DateTime? dt = null;
            if (ViewState["EndDate"] != null)
            {
                dt = (DateTime)ViewState["EndDate"];
            }
            return dt;
        }
        set
        {
            ViewState["EndDate"] = value;
        }
    }

    public string Title
    {
        get
        {
            string title = string.Empty;
            if (ViewState["Title"] != null)
            {
                title = ViewState["Title"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["Title"] = value;
        }
    }

    public string GuestMessage
    {
        get
        {
            string title = string.Empty;
            if (ViewState["GuestMessage"] != null)
            {
                title = ViewState["GuestMessage"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["GuestMessage"] = value;
        }
    }

    public string GuestPassword
    {
        get
        {
            string title = string.Empty;
            if (ViewState["GuestPassword"] != null)
            {
                title = ViewState["GuestPassword"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["GuestPassword"] = value;
        }
    }

    public string PictureFileName
    {
        get
        {
            string title = string.Empty;
            if (ViewState["PictureFileName"] != null)
            {
                title = ViewState["PictureFileName"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["PictureFileName"] = value;
        }
    }

    public bool PrivatePrivacy
    {
        get
        {
            bool privatePrivacy = false;
            if (ViewState["PrivatePrivacy"] != null)
            {
                privatePrivacy = (bool)ViewState["PrivatePrivacy"];
            }
            return privatePrivacy;
        }
        set
        {
            ViewState["PrivatePrivacy"] = value;
        }
    }

    public string CustomURL
    {
        get
        {
            string title = string.Empty;
            if (ViewState["CustomURL"] != null)
            {
                title = ViewState["CustomURL"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["CustomURL"] = value;
        }
    }

    public string OwnerFullName
    {
        get
        {
            string title = string.Empty;
            if (ViewState["OwnerFullName"] != null)
            {
                title = ViewState["OwnerFullName"].ToString();
            }
            return title;
        }
        set
        {
            ViewState["OwnerFullName"] = value;
        }
    }

    #endregion

}