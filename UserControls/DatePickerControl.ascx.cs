using System;
using InterpriseSuiteEcommerceCommon.Extensions;

public partial class UserControls_DatePickerControl : System.Web.UI.UserControl
{

    public UserControls_DatePickerControl() 
    {
        ShowCalendarIcon = true;
    }

    public DateTime? DateValue 
    {
        get 
        {
            return txtDate.Text.TryParseDateTime();
        }
        set { txtDate.Text = (value.HasValue) ? value.Value.ToShortDateString() : string.Empty; }
    }

    public string CssClass 
    {
        get 
        {
            return txtDate.CssClass;
        }
        set 
        {
            txtDate.CssClass = value;
        }
    }

    public int Width 
    {
        set 
        {
            txtDate.Width = value;
        }
    }

    public int Height
    {
        set
        {
            txtDate.Height = value;
        }
    }

    public int SkinID { get; set; }

    public bool ShowCalendarIcon { get; set; }

}