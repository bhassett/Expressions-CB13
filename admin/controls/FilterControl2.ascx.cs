// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Admin.QueryBrokers;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.SqlQuery;
using System.IO;
using System.Text;

public partial class admin_controls_FilterControl2 : System.Web.UI.UserControl, IQueryableControl, IFilterControl 
{
    protected override void OnInit(EventArgs e)
    {
        txtTopCount.Text = AppLogic.AppConfig("AdminInitialListCount");
        txtDateFrom.Text = DateTime.Today.AddDays(-30).ToString("MM-dd-yyyy");
        txtDateTo.Text = DateTime.Today.AddDays(30).ToString("MM-dd-yyyy");

        base.OnInit(e);
    }

    public override void RenderControl(HtmlTextWriter writer)
    {   
        base.RenderControl(writer);        
    }

    private void RenderClientScripts(HtmlTextWriter writer)
    {
    }

    public void SetTableSelect(ITable table)
    {
        cboOrderBy.Items.Clear();

        foreach (IColumn column in table.Columns)
        {
            cboOrderBy.Items.Add(new ListItem(column.ColumnName, column.ColumnName));
        }
    }

    public void ApplyConfig(FilterConfig config)
    {
        if (config.ShowDate)
        {
            pnlFilterByDate.Visible = true;
            lblDateFilterColumn.Text = config.DateColumn;
        }
        else
        {
            pnlFilterByDate.Visible = false;
        }
    }
}
