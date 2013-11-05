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
using System.Data.SqlClient;

public partial class admin_controls_ActiveCustomerControl : System.Web.UI.UserControl, IQueryableControl
{
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
