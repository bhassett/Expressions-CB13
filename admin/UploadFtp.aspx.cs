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
using System.IO;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.InterpriseIntegration.Web;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

public partial class admin_UploadFtp : System.Web.UI.Page
{
    #region Variable Declaration

    private List<DownloadItem> _downloadItemsNotYetMapped = new List<DownloadItem>();

    private class DownloadItem
    {
        private string _itemCode = string.Empty;
        private string _itemName = string.Empty;
        private string _description = string.Empty;

        public DownloadItem(string itemCode, string itemName)
        {
            _itemCode = itemCode;
            _itemName = itemName;
        }

        public string ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    #endregion

    #region Methods

    #region OnInit
    /// <summary>
    /// OnInit
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        Title = "Map ftp uploaded files";
        btnMap.Visible = false;
        grdMapped.Visible = false;

        base.OnInit(e);
    }
    #endregion

    #region DoScan
    /// <summary>
    /// 
    /// </summary>
    private void DoScan()
    {
        DirectoryInfo downloadsFolder = new DirectoryInfo(Server.MapPath("~/download"));

        //Dictionary<string, string> files = new Dictionary<string, string>();
        Dictionary<string, DownloadableItem> downloadsNotMapped = new Dictionary<string, DownloadableItem>();

        //Dictionary<string, string> extMimeMaps = MimeHelper.GetExtensionMimeTypeMaps();

        // first maintain a list of all our files...
        foreach(FileInfo file in downloadsFolder.GetFiles())
        {
            DownloadableItem download = new DownloadableItem();
            download.FileName = file.Name;

            string ext = Path.GetExtension(file.Name);
            ext = ext.StartsWith(".") ? ext.Substring(1) : ext;
            download.Extension = ext;
            download.ContentType = CachedMimeLookUp.Instance.GetMimeType(ext);
            download.ContentLength = file.Length;

            downloadsNotMapped.Add(download.FileName, download);
        }


        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader reader = DB.GetRSFormat(con, "SELECT DownloadId, Extension FROM EcommerceDownload with (NOLOCK)"))
            {
                while (reader.Read())
                {
                    string mappedFileName =
                    string.Format("{0}.{1}", DB.RSField(reader, "DownloadId"), DB.RSField(reader, "Extension"));

                    // compare case in-sensitive
                    mappedFileName = mappedFileName.ToLowerInvariant();

                    if (downloadsNotMapped.ContainsKey(mappedFileName))
                    {
                        downloadsNotMapped.Remove(mappedFileName);
                    }
                }
            }
        }

        // now check
        if (downloadsNotMapped.Count > 0)
        {
            PopulateDownloadItemsNotYetMapped();

            grdUnMapped.DataSource = downloadsNotMapped.Values;
            grdUnMapped.DataBind();

            if (_downloadItemsNotYetMapped.Count > 1)
            {
                btnMap.Visible = true;
            }
        }
    }

    #region PopulateDownloadItemsNotYetMapped
    /// <summary>
    /// 
    /// </summary>
    private void PopulateDownloadItemsNotYetMapped()
    {
        // first index dummy data
        DownloadItem dummySelect = new DownloadItem(string.Empty, "Select");
        dummySelect.Description = "Select";
        _downloadItemsNotYetMapped.Add(dummySelect);

        string userCode = HttpContext.Current.User.Identity.Name;
        string languageCode = string.Empty;

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader reader = DB.GetRSFormat(con, "SELECT LanguageCode FROM SystemUserAccount with (NOLOCK) WHERE UserCode = {0}", DB.SQuote(userCode)))
            {
                if (reader.Read())
                {
                    languageCode = DB.RSField(reader, "LanguageCode");
                }
            }
        }

        using (SqlConnection con = DB.NewSqlConnection())
        {
            con.Open();
            using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.ItemCode, i.ItemName, id.ItemDescription FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryItemDescription id with (NOLOCK) ON id.ItemCode = i.ItemCode AND id.LanguageCode = {0} LEFT OUTER JOIN EcommerceDownload w with (NOLOCK) ON i.ItemCode = w.ItemCode WHERE i.ItemType = 'Electronic Download' AND w.DownloadId IS NULL", DB.SQuote(languageCode)))
            {
                while (reader.Read())
                {
                    DownloadItem item = new DownloadItem(DB.RSField(reader, "ItemCode"), DB.RSField(reader, "ItemName"));
                    item.Description = DB.RSField(reader, "ItemDescription");

                    _downloadItemsNotYetMapped.Add(item);
                }
            }
        }
    }
    #endregion

    #endregion

    #region DoMap
    /// <summary>
    /// 
    /// </summary>
    private void DoMap()
    {
        try
        {
            Dictionary<string, string> itemFileMap = new Dictionary<string, string>();

            // first let's retrieve all the files that have been set which item code to map to
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.EndsWith("cboItemCode"))
                {
                    // 
                    string itemCode = Request.Form[key];
                    if (!string.IsNullOrEmpty(itemCode))
                    {
                        if (itemFileMap.ContainsKey(itemCode))
                        {
                            throw new DownloadMappingException("Duplicate mapping exists! please remove the duplicate");
                        }
                        else
                        {
                            // retrieve the file name for this mapping file
                            // since were assuming the id in this format _ctl0:pnlMain:grdList:_ctl18:cboItemCode
                            // we will find our map(which is just a supporting field) through the item code combo box
                            string mapKey = key.Replace("cboItemCode", "map");

                            string mapFile = Request.Form[mapKey];
                            if (string.IsNullOrEmpty(mapFile))
                            {
                                throw new InvalidOperationException("Map file not found!!!");
                            }
                            else
                            {
                                itemFileMap.Add(itemCode, mapFile);
                            }
                        }
                    }
                }
            }

            List<DownloadableItem> mappedDownloads = new List<DownloadableItem>();

            foreach (string itemCode in itemFileMap.Keys)
            {
                string fileName = itemFileMap[itemCode];
                DownloadableItem item = DownloadableItem.MapNew(itemCode, fileName);
                mappedDownloads.Add(item);
            }

            if (mappedDownloads.Count > 0)
            {
                grdMapped.Visible = true;
                grdMapped.DataSource = mappedDownloads;
                grdMapped.DataBind();

                lblError.Text = string.Empty;
            }
            else
            {
                grdMapped.Visible = true;
                lblError.Text = "Please map one(1) or more files";
            }
        }
        catch (DownloadMappingException dmap)
        {
            lblError.Text = dmap.Message;
        }
    }
    #endregion

    #endregion

    #region Event Handlers

    #region btnScan_Click
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnScan_Click(object sender, EventArgs e)
    {
        DoScan();

        grdMapped.Visible = false;
    }
    #endregion

    #region Grid_RowDataBound
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GridUnmapped_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (DataControlRowType.DataRow == e.Row.RowType)
        {
            // check if the download id is null
            DownloadableItem currentRow = e.Row.DataItem as DownloadableItem;
            if (null != currentRow)
            {

                /************************************************/
                /* File Name Cell
                /************************************************/
                DataControlFieldCell fileNameCell = e.Row.Controls[0] as DataControlFieldCell;
                string fileNameText = string.Empty;
                if (string.IsNullOrEmpty(currentRow.FileName))
                {
                    // this should never happen
                    throw new InvalidOperationException("!!!");
                }
                else
                {
                    fileNameText = Security.HtmlEncode(currentRow.FileName);
                }

                fileNameCell.Text = fileNameText;

                /************************************************/
                /* Item Code Cell
                /************************************************/
                DropDownList cboItemCode = e.Row.FindControl("cboItemCode") as DropDownList;
                if (null != cboItemCode)
                {
                    if (_downloadItemsNotYetMapped.Count > 1)
                    {
                        cboItemCode.DataSource = _downloadItemsNotYetMapped;
                        cboItemCode.DataValueField = "ItemCode";
                        cboItemCode.DataTextField = "Description";
                        cboItemCode.DataBind();
                    }
                    else
                    {
                        cboItemCode.Visible = false;
                    }
                }

                HiddenField map = e.Row.FindControl("map") as HiddenField;
                if (null != map)
                {
                    map.Value = currentRow.FileName;
                }
            }
        }
    }
    #endregion

    protected void GridMapped_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (DataControlRowType.DataRow == e.Row.RowType)
        {
            // check if the download id is null
            DownloadableItem currentRow = e.Row.DataItem as DownloadableItem;
            if (null != currentRow)
            {
                /************************************************/
                /* File Name Cell
                /************************************************/
                DataControlFieldCell fileNameCell = e.Row.Controls[0] as DataControlFieldCell;
                string fileNameText = string.Empty;
                if (string.IsNullOrEmpty(currentRow.FileName))
                {
                    // this should never happen
                    throw new InvalidOperationException("!!!");
                }
                else
                {
                    fileNameText = Security.HtmlEncode(currentRow.FileName);
                }

                fileNameCell.Text = fileNameText;


                /************************************************/
                /* Download Id Cell
                /************************************************/
                DataControlFieldCell cell = e.Row.Controls[2] as DataControlFieldCell;
                int counter = 0;


                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader reader = DB.GetRSFormat(con, "SELECT Counter FROM InventoryItem with (NOLOCK) WHERE ItemCode = {0}", DB.SQuote(currentRow.ItemCode)))
                    {
                        if (reader.Read())
                        {
                            counter = DB.RSFieldInt(reader, "Counter");
                        }
                    }
                }
                
                cell.Text = string.Format("<a href=\"upload.aspx?id={0}\">{1}</a>", counter, currentRow.DownloadId);
            }
        }
    }

    #endregion

    #region btnMap_Click
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMap_Click(object sender, EventArgs e)
    {
        DoMap();

        // rescan to refresh list
        DoScan();
    }
    #endregion

    private class DownloadMappingException : ApplicationException
    {
        public DownloadMappingException(string message) : base(message) { }
    }

    
}




