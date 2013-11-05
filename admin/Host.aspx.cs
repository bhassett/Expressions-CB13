// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Xml;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.SqlQuery;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Admin.QueryBrokers;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

public partial class admin_Host : System.Web.UI.Page
{
    private bool _hostFilterPopUp = false;
    private bool _isFilterPostback = false;
    private string _controlId = string.Empty;
    private string _formName = string.Empty;
    private bool _hostOnForm = false;

    protected override void OnPreInit(EventArgs e)
    {
        var authenticationService = ServiceFactory.GetInstance<IAuthenticationService>();
        authenticationService.AuthenticateAdmin();

        base.OnPreInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        _controlId = Request.Form["cid"];
        _formName = Request.Form["form"];

        _hostFilterPopUp = bool.TryParse(Request.Form["filter"], out _hostFilterPopUp) && _hostFilterPopUp;
        _hostOnForm = bool.TryParse(Request.Form["hostOnForm"], out _hostOnForm) && _hostOnForm && !string.IsNullOrEmpty(_formName);
        _isFilterPostback = bool.TryParse(Request.Form["fpb"], out _isFilterPostback) && _isFilterPostback;

        if (!string.IsNullOrEmpty(_controlId))
        {
            LoadAndRenderControl();
        }
        else
        {
            RenderNotFoundControl();
        }
    }

    private void LoadAndRenderControl()
    {
        WidgetConfig config = WidgetFactory.Instance.GetWidget(_controlId);
        if (null != config)
        {
            Control control = null;
            if (_hostFilterPopUp)
            {
                if (null != config.FilterWidget)
                {
                    control = this.LoadControl(config.FilterWidget.Path);
                    IFilterControl filterControl = control as IFilterControl;
                    if (null != filterControl)
                    {
                        filterControl.ApplyConfig(config.FilterWidget.FilterConfig);
                    }
                }
                else
                {
                    RenderNotFoundControl();
                }
            }
            else
            {
                control = this.LoadControl(config.Path);
            }

            if (null != control)
            {
                // check if the control is a query broker consumer
                IQueryableControl qryControl = control as IQueryableControl;
                if (null != qryControl)
                {
                    IQueryBroker broker = config.GetQueryBroker();

                    // check if we are in filter post back
                    if (_isFilterPostback)
                    {
                        FilterInfo info = FilterInfo.ExtractFrom(Request.Form);
                        broker.ApplyFilter(info);
                    }

                    ITable tableQuery = broker.TableSelect;
                    qryControl.SetTableSelect(tableQuery);
                }

                RenderHostedControl(control);
            }
            else
            {
                RenderNotFoundControl();
            }
        }
        else
        {
            RenderNotFoundControl();
        }
    }

    private void RenderHostedControl(Control ctrl)
    {
        pnlContainer.Controls.Add(ctrl);

        StringBuilder output = new StringBuilder();

        using (StringWriter sw = new StringWriter(output))
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                if (!string.IsNullOrEmpty(_formName))
                {
                    form1.ID = _formName;
                }

                form1.RenderControl(hw);
            }
        }

        Response.Clear();

        if (!_hostOnForm)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(output.ToString().Replace("&nbsp", ""));
            XmlNode panelNode = doc.SelectSingleNode(string.Format("form/div[@id='{0}']", pnlContainer.ClientID));
            XmlDocument containedOnlyDoc = new XmlDocument();
            containedOnlyDoc.LoadXml(panelNode.OuterXml);

            containedOnlyDoc.Save(Response.OutputStream);
        }
        else
        {
            Response.Write(output.ToString());
        }

        Response.End();
    }

    private void RenderNotFoundControl()
    {
        Response.Clear();
        Response.StatusCode = 404;
        Response.Flush();
        Response.End();
    }
}
