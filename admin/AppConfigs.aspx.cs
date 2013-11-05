using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using System.Data;
using Interprise.Framework.ECommerce.DatasetComponent;
using Interprise.Framework.ECommerce.DatasetGateway;
using Interprise.Framework.Base.Exceptions;
using System.Text;

public partial class admin_AppConfigs : System.Web.UI.Page
{
    #region Fields
    private const string DEFAULT_SEARCH_FIELD = "Name";
    private const string DEFAULT_SORT_EXPRESSION = "Name";
    private const SortDirection DEFAULT_SORT_DIRECTION = SortDirection.Ascending;
    private const string SORT_DIRECTION_VIEWSTATE = "SortDirection";
    private const string SORT_EXPRESSION_VIEWSTATE = "SortExpression";
    private List<string> m_violations = new List<string>();
    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            //This is the first time the page has loaded 
            //so load the data from the database and bind it.
            LoadData();
            BindData();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        int intCount = 0;
        //Loop through each violation.
        foreach (string strMessage in m_violations)
        {
            intCount++;
            //Register an alert script with the violation message.
            string strScript = string.Format("alert({0});", CommonLogic.JSStringEncode(strMessage));
            ScriptManager.RegisterClientScriptBlock(hfAlert, hfAlert.GetType(), "Message" + intCount.ToString(), strScript, true);
        }

        //If we had items in the list then show the alert hidden field
        //which will render the script blocks during an AJAX post back.
        hfAlert.Visible = (intCount > 0);
    }

    protected void btnInsert_OnClick(object sender, EventArgs e)
    {
        //Hide the commands and show the add new form.
        pnlCommands.Visible = false;
        pnlAddNew.Visible = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        //Get the cached gateway.
        ApplicationConfigurationDatasetGateway appConfigGateway = SessionStateSink.AdminAppConfigGateway;
        //Get a new WebStoreAppConfigRow.
        ApplicationConfigurationDataset.EcommerceAppConfigRow newRow = InterpriseHelper.AddNewStoreAppConfigRow(appConfigGateway);
        
        //Set the values from the add form.
        newRow.BeginEdit();
        newRow.Name = txtName.Text;
        newRow.GroupName = txtGroupName.Text;
        newRow.ConfigValue = txtConfigValue.Text;
        newRow.Description = txtDescription.Text;
        newRow.EndEdit();

        //Attempt to save the new record.
        if (InterpriseHelper.SaveStoreAppConfigs(appConfigGateway))
        {
            //The new record was saved.
            
            //Show the commands and hide the add new form.
            pnlCommands.Visible = true;
            pnlAddNew.Visible = false;

            //Re-bind the data.
            BindData();
        }
        else
        {
            //Record was not saved we need to show the violations.
            HandleViolations(newRow);

            //Reject any changes that were made.
            appConfigGateway.RejectChanges();
        }

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //Reject any changes that were made.
        SessionStateSink.AdminAppConfigGateway.RejectChanges();

        //Show the commands and hide the add new form.
        pnlCommands.Visible = true;
        pnlAddNew.Visible = false; 
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //Get the cached gateway.
        ApplicationConfigurationDatasetGateway appConfigGateway = SessionStateSink.AdminAppConfigGateway;

        //Filter out the records.
        ApplyFilter(appConfigGateway);

        //Reset the sort back to the defaults.
        SortWebStoreAppConfig(appConfigGateway, DEFAULT_SORT_EXPRESSION, DEFAULT_SORT_DIRECTION);

        //Make sure we are not in edit mode.
        gvAppConfig.EditIndex = -1;
        //Reset the page index back to the first page.
        gvAppConfig.PageIndex = 0;
        //Re-bind the data.
        BindData();
    }

    protected void gvAppConfig_Sorting(object sender, GridViewSortEventArgs e)
    {
        //See if the requested sort expression matches what we have cached.
        if (this.SortExpressionViewState == e.SortExpression)
        {
            //See what the current direction is and flip it.
            if (this.SortDirectionViewState == SortDirection.Ascending)
            {
                this.SortDirectionViewState = SortDirection.Descending;
            }
            else
            {
                this.SortDirectionViewState = SortDirection.Ascending;
            }
        }
        else
        {
            //We have a new sort expression so reset the direction to ascending.
            this.SortDirectionViewState = SortDirection.Ascending;
        }

        //Update the sort expression.
        this.SortExpressionViewState = e.SortExpression;

        //Get the cached gateway.
        ApplicationConfigurationDatasetGateway appConfigGateway = SessionStateSink.AdminAppConfigGateway;

        //Sort the records.
        SortWebStoreAppConfig(appConfigGateway, this.SortExpressionViewState, this.SortDirectionViewState);

        //Make sure we are not in edit mode.
        gvAppConfig.EditIndex = -1;
        //Reset back to the first page.
        gvAppConfig.PageIndex = 0;
        //Re-bind the data.
        BindData();
    }

    protected void gvAppConfig_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //Update the page index on the grid.
        gvAppConfig.PageIndex = e.NewPageIndex;
        //Make sure we are not in edit mode.
        gvAppConfig.EditIndex = -1;
        //Re-bind the data.
        BindData();
    }

    protected void gvAppConfig_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //Put the requested row into edit mode.
        gvAppConfig.EditIndex = e.NewEditIndex;
        //Re-bind the data.
        BindData();
    }

    protected void gvAppConfig_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //Get the dataset.
        ApplicationConfigurationDatasetGateway appConfigGateway = SessionStateSink.AdminAppConfigGateway;

        //Get the updating row from the grid.
        GridViewRow gridRow = gvAppConfig.Rows[e.RowIndex];
        //Get the matching row from the table.
        
        //ApplicationConfigurationDataset.EcommerceStoreAppConfigRow rowToUpdate = GetRowToUpdate(
        //    appConfigGateway.EcommerceStoreAppConfig, (Guid)gvAppConfig.DataKeys[e.RowIndex]["AppConfigGUID"]);

        ApplicationConfigurationDataset.EcommerceAppConfigRow rowToUpdate = GetRowToUpdate(
            appConfigGateway.EcommerceAppConfig, (Guid)gvAppConfig.DataKeys[e.RowIndex]["AppConfigGUID"]);

        //Update the record with the data from the grid.
        rowToUpdate.BeginEdit();
        rowToUpdate.Name = ((TextBox)gridRow.Cells[1].FindControl("txtName")).Text;
        rowToUpdate.GroupName = ((TextBox)gridRow.Cells[2].FindControl("txtGroupName")).Text;
        rowToUpdate.ConfigValue = ((TextBox)gridRow.Cells[3].FindControl("txtConfigValue")).Text;
        rowToUpdate.Description = ((TextBox)gridRow.Cells[4].FindControl("txtDescription")).Text;
        rowToUpdate.EndEdit();

        try
        {
            //Save the changes
            if (InterpriseHelper.SaveStoreAppConfigs(appConfigGateway))
            {
                //Take the item out of edit mode.
                gvAppConfig.EditIndex = -1;
                //Re-bind the data.
                BindData();
            }
            else
            {
                //Record was not saved we need to show the violations.
                HandleViolations(rowToUpdate);

                //Reject any changes that were made.
                appConfigGateway.RejectChanges();
            }
        }
        catch (DataConcurrencyException)
        {
            //We had a concurrency error.

            //Re-load the data from the database (This will also apply the current sort).
            LoadData();
            //Apply the current filter.
            ApplyFilter(SessionStateSink.AdminAppConfigGateway);

            //Add the conncurency violation.
            m_violations.Add("The record you were working with was modified by another user."
                + " Your changes have been lost and the record has been refreshed with the new data.");

            //Take the item out of edit mode.
            gvAppConfig.EditIndex = -1;
            //Re-bind the data.
            BindData();
        }
    }

    protected void gvAppConfig_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Reject any changes that were made.
        SessionStateSink.AdminAppConfigGateway.RejectChanges();

        //Take the current row out of edit mode.
        gvAppConfig.EditIndex = -1;
        //Re-bind the data.
        BindData();
    }

    #endregion

    #region Properties
    /// <summary>
    /// Gets the stored sort direction.
    /// </summary>
    private SortDirection SortDirectionViewState
    {
        get
        {
            //Defalut the return value.
            SortDirection returnValue = DEFAULT_SORT_DIRECTION;

            //See if we have a value in view state.
            if (ViewState[SORT_DIRECTION_VIEWSTATE] != null)
            {
                //Get the value from view state.
                returnValue = (SortDirection)ViewState[SORT_DIRECTION_VIEWSTATE];
            }

            return returnValue;
        }
        set
        {
            //Store the value in view state.
            ViewState[SORT_DIRECTION_VIEWSTATE] = value;
        }
    }

    /// <summary>
    /// Gets the stored sort expression.
    /// </summary>
    private string SortExpressionViewState
    {
        get
        {
            //Defalut the return value.
            string returnValue = DEFAULT_SORT_EXPRESSION;

            //See if we have a value in view state.
            if (ViewState[SORT_EXPRESSION_VIEWSTATE] != null)
            {
                //Get the value from view state.
                returnValue = (string)ViewState[SORT_EXPRESSION_VIEWSTATE];
            }

            return returnValue;
        }
        set
        {
            //Store the value in view state.
            ViewState[SORT_EXPRESSION_VIEWSTATE] = value;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Loads the data from the database and stores it in session.
    /// </summary>
    private void LoadData()
    {
        //Load the data from the database.
        ApplicationConfigurationDatasetGateway appConfigGateway = InterpriseHelper.SelectAllStoreAppConfigs();
        //Sort the data.
        SortWebStoreAppConfig(appConfigGateway, this.SortExpressionViewState, this.SortDirectionViewState);
        //Cache the data in session.
        SessionStateSink.AdminAppConfigGateway  = appConfigGateway;
    }

    /// <summary>
    /// Binds the data to the grid.
    /// </summary>
    private void BindData()
    {
        //Get the data out of session.
        ApplicationConfigurationDatasetGateway appConfigGateway = SessionStateSink.AdminAppConfigGateway;
        if (appConfigGateway != null)
        {
            //Get the EcommerceStoreAppConfig table and set it as the data source.
            ApplicationConfigurationDataset.EcommerceAppConfigDataTable table = appConfigGateway.EcommerceAppConfig;
            gvAppConfig.DataSource = table;
        }
        else
        {
            gvAppConfig.DataSource = null;
        }

        //Bind the data to the grid.
        gvAppConfig.DataBind();
    }

    /// <summary>
    /// Handles any violations we get.
    /// </summary>
    /// <param name="rowInViolation">The row in violation.</param>
    private void HandleViolations(ApplicationConfigurationDataset.EcommerceAppConfigRow rowInViolation)
    {
        //Loop through each column in error and add the violations to the page violation list.
        foreach (DataColumn columnInError in rowInViolation.GetColumnsInError())
        {
            m_violations.Add(rowInViolation.GetColumnError(columnInError));
        }
    }

    /// <summary>
    /// Sorts the web store app config table.
    /// </summary>
    /// <param name="appConfigGateway">The <see cref="ApplicationConfigurationDatasetGateway"/> containing the 
    /// web store app config table.</param>
    /// <param name="strColumnName">The name of the column to sort on.</param>
    /// <param name="directionToSort">The direction to sort.</param>
    private void SortWebStoreAppConfig(ApplicationConfigurationDatasetGateway appConfigGateway, string strColumnName,
        SortDirection directionToSort)
    {
        //Sort the records.
        appConfigGateway.EcommerceAppConfig.DefaultView.Sort = GetSortString(strColumnName, directionToSort);
    }

    /// <summary>
    /// Gets the sort string.
    /// </summary>
    /// <param name="strColumnName">The name of the column to sort on.</param>
    /// <param name="directionToSort">The direction to sort.</param>
    /// <returns>The sort string containing the field and direction.</returns>
    private string GetSortString(string strColumnName, SortDirection directionToSort)
    {
        //Build the sort string and return it.
        switch (directionToSort)
        {
            case SortDirection.Descending:
                {
                    return strColumnName + " DESC";
                }
            case SortDirection.Ascending:
            default:
                {
                    return strColumnName + " ASC";
                }
        }
    }
    /// <summary>
    /// Gets the row to update from the specified <paramref name="table"/> that matches the <paramref name="appConfigGUID"/>.
    /// </summary>
    /// <param name="table">The table to get the row from</param>
    /// <param name="appConfigGUID">The app config GUID to find.</param>
    /// <returns>The matching row; null if the row was not found.</returns>
    private ApplicationConfigurationDataset.EcommerceAppConfigRow GetRowToUpdate(
        ApplicationConfigurationDataset.EcommerceAppConfigDataTable table, Guid appConfigGUID)
    {
        //Get the matching row from the table.
        DataRow[] rows = table.Select("AppConfigGUID = '" + appConfigGUID.ToString() + "'");

        if (rows.Length > 0)
        {
            return (ApplicationConfigurationDataset.EcommerceAppConfigRow)rows[0];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Apply the search filter.
    /// </summary>
    /// <param name="appConfigGateway">The gateway containing the data to filter.</param>
    private void ApplyFilter(ApplicationConfigurationDatasetGateway appConfigGateway)
    {
        //See if the search has been blanked out.
        if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
        {
            //Clear the row filter.
            appConfigGateway.EcommerceAppConfig.DefaultView.RowFilter = "";
        }
        else
        {
            //Apply the filter.
            appConfigGateway.EcommerceAppConfig.DefaultView.RowFilter = string.Format("{0} LIKE '%{1}%'",
                DEFAULT_SEARCH_FIELD, EscapeFilterChars(txtSearch.Text));
        }
    }

    /// <summary>
    /// Escapes out all of the special filter charaters.
    /// </summary>
    /// <param name="strTextToEscape">The string to escape.</param>
    /// <returns>The original string with the special charaters esacped.</returns>
    private string EscapeFilterChars(string strTextToEscape)
    {
        //Create a string builder with it's inital size the same as the string passed in.
        StringBuilder returnValue = new StringBuilder(strTextToEscape.Length);

        //Loop through each charater and escape out special charaters.

        //NOTE: We can't just use .Replace() on the string becuase if we replace "[" with "[[]"
        //the second replace statment which replaces "]" with "[]]" will esacape out the close bracket from the first
        //replace statement and you end up getting this "[[[]]". 
        //The wild card charaters chould be done with .Replace() but since we are already looping through all the
        //charaters we might as well check them so we don't do it two more times.
        foreach (char currentChar in strTextToEscape)
        {
            switch(currentChar)
            {
                case '[':
                    {
                        returnValue.Append("[[]");
                        break;
                    }
                case ']':
                    {
                        returnValue.Append("[]]");
                        break;
                    }
                case '%':
                    {
                        returnValue.Append("[%]");
                        break;
                    }
                case '*':
                    {
                        returnValue.Append("[*]");
                        break;
                    }
                case '\'':
                    {
                        returnValue.Append("\'\'");
                        break;
                    }
                default:
                    {
                        returnValue.Append(currentChar);
                        break;
                    }
            }
        }

        return returnValue.ToString();
    }
    #endregion
}



