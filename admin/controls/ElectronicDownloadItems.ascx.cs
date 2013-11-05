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
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;
using System.Data.Common;
using System.IO;
using System.Data.SqlClient;

public partial class admin_controls_ElectronicDownloadItems : System.Web.UI.UserControl, IQueryableControl
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (DataControlRowType.DataRow == e.Row.RowType)
        {
            // check if the download id is null
            DbDataRecord currentRow = e.Row.DataItem as DbDataRecord;
            if (null != currentRow)
            {
                int counter = (int)currentRow["Counter"];
                string itemCode = (string)currentRow["ItemCode"];

                
                // File Name Cell
                DataControlFieldCell fileNameCell = e.Row.Controls[0] as DataControlFieldCell;
                string fileNameText = string.Empty;
                if (currentRow["FileName"] == DBNull.Value ||
                    currentRow["FileName"] is string &&
                    string.IsNullOrEmpty((string)currentRow["FileName"]))
                {
                    fileNameText = "[UnAssigned]";
                }
                else
                {
                    fileNameText = Security.HtmlEncode((string)currentRow["FileName"]);
                }

                fileNameCell.Text = fileNameText;


                //Description
                DataControlFieldCell descriptionCell = e.Row.Controls[3] as DataControlFieldCell;
                string descriptionText = string.Empty;
                if (currentRow["WebDescription"] == DBNull.Value ||
                    currentRow["WebDescription"] is string &&
                    string.IsNullOrEmpty((string)currentRow["WebDescription"]))
                {
                    descriptionText = (string)currentRow["ItemDescription"];
                }
                else
                {
                    descriptionText = (string)currentRow["WebDescription"];
                }
                descriptionCell.Text = Security.HtmlEncode(descriptionText);
                
                // DownloadId Cell
                DataControlFieldCell downloadCell = e.Row.Controls[4] as DataControlFieldCell;

                string downloadText = string.Empty;
                if (currentRow["DownloadId"] == DBNull.Value || 
                    currentRow["DownloadId"] is string && 
                    string.IsNullOrEmpty((string)currentRow["DownloadId"]))
                {
                    downloadText = "Upload file";
                }
                else
                {
                    downloadText = (string)currentRow["DownloadId"];
                }

                downloadCell.Text = string.Format("<a href=\"DownloadableItem.aspx?id={0}\">{1}</a>", counter, Security.HtmlEncode(downloadText));

                
                // Status Cell
                DataControlFieldCell statusCell = e.Row.Controls[6] as DataControlFieldCell;
                DownloadableItem download = DownloadableItem.FindByItemCode(itemCode); 

                string statusText = string.Empty;

                if (null == download)
                {
                    statusText = "N/A";
                }
                else if (download.IsPhysicalFileExisting())
                {
                    if (currentRow["IsActive"] == DBNull.Value)
                    {
                        statusText = "N/A";
                    }
                    else if (currentRow["IsActive"] is bool)
                    {
                        statusText = CommonLogic.IIF((bool)currentRow["IsActive"], "A", "I");
                    }
                    else
                    {
                        statusText = "?";
                    }
                }
                else
                {
                    statusText = "?";
                }

                statusCell.Text = Security.HtmlEncode(statusText);
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region IQueryableControl Members

    public void SetTableSelect(InterpriseSuiteEcommerceCommon.InterpriseIntegration.SqlQuery.ITable table)
    {
        string sql = table.ToSqlSelectString();

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader reader = DB.GetRSFormat(con, sql))
            {
                grdList.DataSource = reader;
                grdList.DataBind();
            }
        }
    }

    #endregion
}
