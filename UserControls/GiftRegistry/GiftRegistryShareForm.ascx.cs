using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;

public partial class UserControls_GiftRegistry_GiftRegistryShareForm : System.Web.UI.UserControl
{
    #region Initializer

    protected void Page_Load(object sender, EventArgs e)
    {
        SetValidators();
        chkSendCopy.Text = AppLogic.GetString("editgiftregistry.aspx.41", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
    }

    protected override void OnInit(EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    #endregion

    #region Methods

    public void LoadGiftRegistry(GiftRegistry giftRegistry)
    {
        if (giftRegistry == null) return;

        StartDate = giftRegistry.StartDate;
        EndDate = giftRegistry.EndDate;
        Title = giftRegistry.Title;
        CustomURL = giftRegistry.CustomURLPostfix;
        Subject = giftRegistry.Title;
    }

    public void ClearTextBox()
    {
        var txts = GetTextBoxes();
        foreach (var item in txts)
        {
            if (item.GetType() == typeof(TextBox) && item.ID != "txtSubject")
            {
                (item as TextBox).Text = string.Empty;
            }
        }
    }

    private void SetValidators()
    {
        var emailVals = GetEmailValidators();
        foreach (var validator in emailVals)
        {
            var valObj = (validator as RegularExpressionValidator);
            valObj.ValidationExpression = DomainConstants.EmailRegExValidator;
            valObj.ErrorMessage = "*"; //AppLogic.GetString("editgiftregistry.error.12", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }
    }

    public IEnumerable<WebControl> GetEmailValidators()
    {
        return this.Controls.OfType<WebControl>()
                            .Where(t => t is RegularExpressionValidator);
    }

    public IEnumerable<WebControl> GetTextBoxes()
    {
        return this.Controls.OfType<WebControl>()
                            .AsQueryable()
                            .Where(TextBoxExpresssion());
    }

    public IEnumerable<WebControl> GetEmailTextBoxes()
    {
        return this.Controls.OfType<WebControl>()
                            .AsQueryable()
                            .Where(TextBoxEmailExpression());
    }

    public IEnumerable<string> GetEmailAddresses()
    {
        var lst = new List<string>();
        var textBoxes = GetEmailTextBoxes();
        foreach (var item in textBoxes)
        {
            string value = (item as TextBox).Text;
            if (value.IsNullOrEmptyTrimmed()) continue;

            lst.Add(value);
        }

        return lst;
    }

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

    public bool IsSendMeCopy
    {
        get 
        {
            return chkSendCopy.Checked;
        }
    }

    public string HtmlMessage
    {
        get 
        {
            return ctrlWebEditorControl.Value;
        }
        set
        {
            ctrlWebEditorControl.Value = value;
        }
    }

    public string Subject 
    {
        get 
        {
            return txtSubject.Text;
        }
        set 
        {
            txtSubject.Text = value;
        }
    }

    #endregion

    private Expression<Func<WebControl, Boolean>> TextBoxExpresssion()
    {
        Expression <Func<WebControl, Boolean>> exp = (w => w is TextBox);
        return exp;
    }

    private Expression<Func<WebControl, Boolean>> TextBoxEmailExpression()
    {
        Expression<Func<WebControl, Boolean>> exp = (w => w is TextBox && (w as TextBox).CssClass.Contains("email"));
        return exp;
    }

}