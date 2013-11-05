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
using System.Text;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.SqlQuery;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Admin.QueryBrokers;
using System.Globalization;
using System.Data.Common;
using System.Collections.Generic;
using System.Data.SqlClient;

public partial class admin_controls_OpenSalesOrderControl : System.Web.UI.UserControl, IQueryableControl
{
    #region IQueryableControl Members

    public void SetTableSelect(ITable table)
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

    protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (DataControlRowType.DataRow == e.Row.RowType)
        {
            // check if the download id is null
            DbDataRecord currentRow = e.Row.DataItem as DbDataRecord;
            if (null != currentRow)
            {
                DateTime date = (DateTime)currentRow["Date"];

                DataControlFieldCell dateCell = e.Row.Controls[3] as DataControlFieldCell;
                if (null != dateCell)
                {
                    dateCell.Text = date.ToString("yyyy-MM-dd");
                }

                decimal total = (decimal)currentRow["Total"];
                string currencyCode = (string)currentRow["Currency"];
                DataControlFieldCell totalCell = e.Row.Controls[4] as DataControlFieldCell;
                if (null != totalCell)
                {
                    NumberFormatInfo currencyFormat = Currency.GetCurrencyFormat(currencyCode);
                    totalCell.Text = total.ToString("C", currencyFormat);
                }
            }
        }
    }

    #endregion
}
