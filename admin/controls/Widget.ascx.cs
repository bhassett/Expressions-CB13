using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;

public partial class admin_controls_Widget : System.Web.UI.UserControl
{

    #region Public Properties

    public WidgetType Type
    {
        get
        {
            if (ViewState["WidgetType"] != null)
            {
                return (WidgetType)ViewState["WidgetType"];
            }
            return WidgetType.Blank;
        }
        set
        {
            ViewState["WidgetType"] = value;
        }
    }

    public string Title 
    {
        get 
        {
            if (ViewState["Title"] != null)
            {
                return ViewState["Title"].ToString();
            }
            return String.Empty;
        }
        set
        {
            ViewState["Title"] = value;
        }    
    }

    public bool IsMaximized
    {
        get
        {
            bool retVal = false;
            if (ViewState["IsMaximized"] != null)
            {
                retVal = (bool)ViewState["IsMaximized"];
            }
            return retVal;
        }
        set
        {
            ViewState["IsMaximized"] = value;
        }
    }

    public string MaxHeight 
    {
        get
        {
            if (ViewState["MaxHeight"] != null)
            {
                return ViewState["MaxHeight"].ToString();
            }
            return "100%";
        }
        set
        {
            ViewState["MaxHeight"] = value;
        }
    }

    #endregion

    #region Initialize

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion

}