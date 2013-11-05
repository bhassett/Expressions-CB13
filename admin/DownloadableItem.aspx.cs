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
using System.Web.Configuration;
using InterpriseSuiteEcommerceCommon;
using System.IO;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;
using System.Data.SqlClient;

public partial class admin_DownloadableItem : System.Web.UI.Page
{
    #region Methods

    #region DisplayDownloadInfo
    /// <summary>
    /// Displays the information of the downloadable item
    /// </summary>
    /// <param name="download"></param>
    private void DisplayDownloadInfo(DownloadableItem download)
    {
        if (null != download)
        {
            lblDownloadIdCaption.Text = string.Format("Download Id : {0}", download.DownloadId);
            if (CommonLogic.FileExists(string.Format("images/icon/{0}.png", download.Extension))) {
                imgFileType.ImageUrl = string.Format("images/icon/{0}.png", download.Extension);
            }
            else {
                imgFileType.ImageUrl = "images/icon/defaultfile.png";
            }
            
            lblFileNameCaption.Text = string.Format("File Name: {0}", download.FileName);
            lblContentType.Text = string.Format("Content Type: {0}", download.ContentType);
            lblDownloadCaption.Text = string.Format("This file has been downloaded {0} times", download.GetNumberOfDownloads());
            lblDownloadSize.Text = string.Format("File size: {0} kb", download.ContentLength);
            if (download.IsPhysicalFileExisting())
                lblActiveCaption.Text = string.Format("This file is {0}", download.IsActive ? "active" : "inactive");
            else lblActiveCaption.Text = "This file is missing!!!"; 

            pnlInfo.Visible = true;
        }
        else
        {
            pnlInfo.Visible = false;
        }
    }
    #endregion

    #region GetMaxRequestLength
    /// <summary>
    /// Gets the maximum number of kilobytes the server can accept in the input type=file
    /// </summary>
    /// <returns></returns>
    private int GetMaxRequestLength()
    {
        HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
        if (null != section)
        {
            return section.MaxRequestLength;
        }

        return 0;
    }
    #endregion

    #endregion

    #region Event Handlers

    #region OnInit
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        lblAllowableSize.Text = string.Format("You can upload a maximum size of {0} KB", GetMaxRequestLength());

        // reverse lookup the item code by the id, if specified
        int counter = CommonLogic.QueryStringUSInt("id");

        string itemCode = string.Empty;
        string itemName = string.Empty;

        if (counter > 0)
        {
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, "SELECT ItemCode, ItemName FROM InventoryItem with (NOLOCK) WHERE Counter = {0}", counter))
                {
                    if (reader.Read())
                    {
                        itemCode = DB.RSField(reader, "ItemCode");
                        itemName = DB.RSField(reader, "ItemName");
                    }
                }
            }
        }

        lblItemCode.Text = itemCode;
        lblItemName.Text = itemName;
        Title = string.Format("Upload file for {0}", itemName);

        // info
        if (counter > 0)
        {
            DownloadableItem download = DownloadableItem.FindByItemCode(itemCode);
            DisplayDownloadInfo(download);
        }
        else
        {
            pnlInfo.Visible = false;
        }

        base.OnInit(e);
    }
    #endregion

    #region btnUpload_Click
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (this.fileUpload.HasFile)
        {
            DownloadableItem download = DownloadableItem.AddNew(lblItemCode.Text, fileUpload.FileName, fileUpload.PostedFile.ContentType, fileUpload.FileBytes);
            DisplayDownloadInfo(download);
        }
    }
    #endregion

    #endregion
}
