using System.Web;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon;

public partial class UserControls_WebEditorControl : System.Web.UI.UserControl
{
    private bool _enableEncodeDecode = true;

    public string Value 
    { 
        get { return (EnableEncodeDecode) ? txtEditor.Text.ToHtmlEncode() : txtEditor.Text; }
        set { txtEditor.Text = (EnableEncodeDecode) ? value.ToHtmlDecode() : value; }
    }

    public bool EnableEncodeDecode 
    {
        get { return _enableEncodeDecode; }
        set { _enableEncodeDecode = value; }
    }

    public EditorDisplayMode DisplayMode
    {
        get 
        {
            if (ViewState["DisplayMode"] != null)
            {
                return (EditorDisplayMode)ViewState["DisplayMode"];
            }
            return EditorDisplayMode.Simple;
        }
        set 
        {
            ViewState["DisplayMode"] = value;
        }
    }

    public string GetDisplayMode()
    {
        return DisplayMode.ToString().ToLowerInvariant();
    }
}