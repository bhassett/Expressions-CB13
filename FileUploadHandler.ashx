<%@ WebHandler Language="C#" Class="FileUploadHandler" %>

using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DataAccess;

public class FileUploadHandler : IHttpHandler {

    private Interprise.Facade.Inventory.ItemDetailFacade _inventoryFacade;
    private ImageUploadStatus _uploadStatus;

    private enum UploadType
    {
        Upload,
        Edit,
        Remove
    }
    
    private class UploadParam
    {
        public int Counter { get; set; }
        public string EntityCode { get; set; }
        public string EntityType { get; set; }
        public string Size { get; set; }
        public int SizeValue { get; set; }
        public string FileName { get; set; }
        public UploadType UploadType { get; set; }
        public bool IsApplyToAll { get; set; }
        public bool isImageExist { get; set; }
        public string ImageSrc { get; set; }
        public bool IsDeleteAll { get; set; }
    }
 
    public void ProcessRequest (HttpContext context) 
    {
        CurrentContext = context;

        StartImageProcessing();

        //removes the dataset
        CleanUp();
        
        context.Response.Write(GetJSONUploadStatus());
        context.Response.End();
    }

    public string GetJSONUploadStatus() 
    {
        UploadStatus.ResponseText = string.Empty;

        string json = InterpriseSuiteEcommerceCommon.Tool.JSONHelper.Serialize(UploadStatus);
        return json;
    }

    protected void StartImageProcessing()
    {
        var param = GetUploadParam();
        string filename = param.FileName;

        // check if upload new or modify
        bool isNew = (param.UploadType == UploadType.Upload);
        bool allowResize = (param.IsApplyToAll) || (param.Size == "medium" && isNew);
        
        switch (param.UploadType)
        { 
            case UploadType.Remove:
                
                //delete
                DeleteImageRecord(param.EntityType, param.EntityCode, filename, param.ImageSrc, param.isImageExist, param.Size, param.IsDeleteAll, allowResize);
                
                break;
            default:

                //check if there are uploaded files
                if (UploadedFiles.Count == 0) return;

                if (param.EntityType.ToLowerInvariant() == "product")
                {
                    var firstUploadedFile = FirstUploadedFile;
                    if (!IsValidateFileExtension(firstUploadedFile)) return;

                    //allow image resize if medium and IsApplyToAll is toggled
                    if (isNew)
                    {
                        filename = GetUploadedFileName(firstUploadedFile);
                    
                        //sync with data filename
                        filename = InventoryFacade.FixImageFileName(filename);
                    }
                    UploadImageFile(param.EntityType, param.EntityCode, filename, param.Size, isNew, allowResize, false);
                }
                else
                {
                    UploadEntityImageFile();
                }
                                
                break;
        }
        
    }

    private void UploadEntityImageFile()
    {
        var param = GetUploadParam();

        string filenameext = String.Empty;
        string fileNameWOutExt = String.Empty;
        string contentType = String.Empty;
        string fileName = String.Empty;
        
        var firstPostedFile = FirstUploadedFile;
        var imageToResize = System.Drawing.Image.FromStream(firstPostedFile.InputStream);

        //entity image has no new upload only edit
        if(param.UploadType == UploadType.Edit)
        {
            if (param.isImageExist)
            {
                fileName = param.FileName;
                filenameext = GetFileExtension(fileName);
                fileNameWOutExt = GetFileNameWithoutExtension(fileName);
            }
            else
            {
                fileNameWOutExt = param.EntityCode;
                filenameext = GetFileExtension(firstPostedFile.FileName);
            }
        }

        contentType = GetImageContentTypeByExtension(filenameext);

        if (param.Size.ToLowerInvariant() == "mobile")
        {
            var imageToResizeMobile = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
            AppLogic.MakeMobilePic(param.EntityType, fileNameWOutExt, imageToResizeMobile, filenameext.Replace(".", ""));
        } 
        else 
        {
            //large, medium, icon, minicart
            AppLogic.ResizeEntityOrObject(param.EntityType, imageToResize, fileNameWOutExt, param.Size, contentType, false);
        }
        
    }

    private void UploadImageFile(string entitytype, string entityCode, string filename, string size, bool isNew, bool isResize, bool isDeleteAll)
    {       
        if (size.IsNullOrEmptyTrimmed()) return;
        if (entitytype.IsNullOrEmptyTrimmed()) return;
        if (entityCode.IsNullOrEmptyTrimmed()) return;

        // check if image is no picture available

        var firstPostedFile = FirstUploadedFile;
        string filenameext = GetFileExtension(filename);
        string fileNameWOutExt = GetFileNameWithoutExtension(filename);
        
        var image = new EntityImage()
        {
            ImageFileName = filename,
            ContentType = GetImageContentTypeByExtension(filenameext),
            ImageRaw = new System.IO.BinaryReader(firstPostedFile.InputStream)
                                                   .ReadBytes(firstPostedFile.ContentLength)
        };

        filename = fileNameWOutExt;

        if (image.ContentType.IsNullOrEmptyTrimmed()) return;

        var imageToResize = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
        
        //save the old value
        bool useImageResize = AppLogic.AppConfigBool("UseImageResize");

        //manually update the config to activate the resizing inside ResizePhoto() method
        AppLogic.SetAppConfig(0, "UseImageResize", "true");

        if (size.ToLowerInvariant() == "mobile") //separate creation of mobile since it has different resizing
        {
            AppLogic.MakeMobilePic(entitytype, filename, imageToResize, filenameext.Replace(".", ""));
        }
        else if (size.ToLowerInvariant() == "minicart") //separate creation of minicart since it has different resizing
        {
            var imageToResizeMinicart = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
            AppLogic.MakeMiniCartPic(filename, imageToResizeMinicart, filenameext.Replace(".", ""));
        }
        else
        {
            //large, medium, icon
            AppLogic.ResizeEntityOrObject(entitytype, imageToResize, filename, size, image.ContentType, false);
        }

        //if resize is toggled resize the all the remaining images
        if (isResize && size == ImageSizeTypes.large.ToString())
        {
            AppLogic.ResizeEntityOrObject(entitytype, imageToResize, filename, ImageSizeTypes.medium.ToString(), image.ContentType, false);
            AppLogic.ResizeEntityOrObject(entitytype, imageToResize, filename, ImageSizeTypes.icon.ToString(), image.ContentType, false);
        }

        if (isResize && size == ImageSizeTypes.medium.ToString() || isResize)
        {
            var imageToResizeMicro = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
            var imageToResizeMinicart = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
            var imageToResizeMobile = System.Drawing.Image.FromStream(firstPostedFile.InputStream);
            
            AppLogic.MakeMicroPic(filename, imageToResizeMicro);
            AppLogic.MakeMiniCartPic(filename, imageToResizeMinicart, filenameext.Replace(".", ""));
            AppLogic.MakeMobilePic(entitytype, filename, imageToResizeMobile, filenameext.Replace(".", ""));
        }

        //manually update the config based from original value
        AppLogic.SetAppConfig(0, "UseImageResize", useImageResize.ToStringLower());

        // save the new
        if (entitytype.ToLowerInvariant() == "product")
        {
            SaveProductImageData(entityCode, image.ImageFileName, false, entitytype, true, string.Empty, size, isDeleteAll, isResize);
        }
        
    }

    private void LoadProductImageData(string itemcode)
    {
        if (itemcode.IsNullOrEmptyTrimmed()) { return; }

        var itemdataset = (Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway)InventoryFacade.CurrentDataset;
        string websiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;

        string[][] paramset;
        paramset = new string[][]{ new string[]{ itemdataset.InventoryItem.TableName
                                                                ,Interprise.Framework.Base.Shared.StoredProcedures.GETINVENTORYITEM
                                                                ,Interprise.Framework.Inventory.Shared.Const.AT_ITEMCODE, itemcode},
                                                   new string[]{itemdataset.InventoryOverrideImage.TableName,"READINVENTORYOVERRIDEIMAGE"
                                                                ,Interprise.Framework.Inventory.Shared.Const.AT_ITEMCODE, itemcode
                                                                ,Interprise.Framework.Inventory.Shared.Const.AT_WEBSITECODE, websiteCode},
                                                   new string[]{ itemdataset.InventoryImageWebOptionDescription.TableName,"READINVENTORYIMAGEWEBOPTIONDESCRIPTION"
                                                                ,Interprise.Framework.Inventory.Shared.Const.AT_ITEMCODE, itemcode
                                                                ,Interprise.Framework.Inventory.Shared.Const.AT_WEBSITECODE, websiteCode}
                                                };

        InventoryFacade.LoadDataSet(paramset, Interprise.Framework.Base.Shared.Enum.ClearType.Specific);
    }

    private void DeleteImageRecord(string entitytype, string entityCode, string filename, string srcfilename, bool isExist, string size, bool isDeleteAll, bool isResize)
    {
        if (entitytype.ToLowerInvariant() == "product")
        {
            SaveProductImageData(entityCode, filename, true, entitytype, isExist, srcfilename, size, isDeleteAll, isResize);
        }
        else 
        {
            if (!isExist) return;
            
            DeleteFile(srcfilename);
        }
    }

    protected void SaveProductImageData(string itemcode, string fileName, bool isdelete, string itemType, bool isExist, string srcfilename, string size, bool isDeleteAll, bool isResize)
    {
        try
        {
            string websiteCode = InterpriseHelper.ConfigInstance.WebSiteCode;

            LoadProductImageData(itemcode);

            var inventoryfacade = InventoryFacade;
            var itemDataset = (inventoryfacade.CurrentDataset as Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway);
            
            //check if exist
            var currentrow = itemDataset.InventoryOverrideImage.FindByItemCodeWebSiteCodeFilename(itemcode, websiteCode, fileName);
            
            //create
            if (currentrow == null && !isdelete)
            {
                inventoryfacade.AddWebImage(itemcode, websiteCode, fileName);
                inventoryfacade.SetAsDefaultWebImage(itemDataset.InventoryOverrideImage.Rows[0], true);

                currentrow = itemDataset.InventoryOverrideImage.FindByItemCodeWebSiteCodeFilename(itemcode, websiteCode, fileName);
                
                //per size adding
                currentrow.BeginEdit();

                if (!isResize && size.ToLowerInvariant() == "large")
                {
                    currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMicroColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = false;
                }
                else if (size.ToLowerInvariant() == "medium")
                {
                    currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasMicroColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = true;
                }
                else if (size.ToLowerInvariant() == "icon")
                {
                    currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMicroColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = false;
                }
                else if (size.ToLowerInvariant() == "minicart")
                {
                    currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMicroColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = false;
                }
                else if (size.ToLowerInvariant() == "mobile")
                {
                    currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = true;
                    currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = false;
                    currentrow[itemDataset.InventoryOverrideImage.HasMicroColumn.ColumnName] = false;
                }

                currentrow.EndEdit();

                //if (IsDefaultIconAlreadySetToOtherImages(itemDataset, itemcode, fileName))
                //{
                //    currentrow.BeginEdit();
                //    currentrow[itemDataset.InventoryOverrideImage.IsDefaultIconColumn.ColumnName] = false;
                //    currentrow.EndEdit();
                //}

                //if (IsDefaultMediumAlreadySetToOtherImages(itemDataset, itemcode, fileName))
                //{
                //    currentrow.BeginEdit();
                //    currentrow[itemDataset.InventoryOverrideImage.IsDefaultIconColumn.ColumnName] = false;
                //    currentrow.EndEdit();
                //}
                
                inventoryfacade.AddImageSEOProperties(InterpriseHelper.ConfigInstance.WebSiteCode, fileName);
            }
            else if (isdelete && currentrow != null) //delete
            {
                //bool hasMinicart = overrideRow.HasMinicart;
                //bool hasMobile = overrideRow.HasMobile;

                bool isOneOfTheMajorSize = (size.ToLowerInvariant() == "large") || (size.ToLowerInvariant() == "medium") || (size.ToLowerInvariant() == "icon");
                bool deleteNow = (boolToInt(currentrow.HasLarge) + boolToInt(currentrow.HasMedium) + boolToInt(currentrow.HasIcon) == 1) && isOneOfTheMajorSize;

                bool isDefaultIcon = currentrow.IsDefaultIcon;
                bool isDefaultMedium = currentrow.IsDefaultMedium;

                //isDeleteAll - option in the page during deletion
                if (deleteNow || isDeleteAll)
                {
                    inventoryfacade.DeleteImageSEOProperties(fileName);
                    currentrow.Delete();

                    if (isDefaultIcon)
                    {
                        TryLocateRecordForNewDefaultIcon(itemDataset, itemcode, fileName);
                    }

                    if (isDefaultMedium)
                    {
                        TryLocateRecordForNewDefaultMedium(itemDataset, itemcode, fileName);
                    }
                }
                else
                {
                    //update if deleting individual large, medium or icon
                    if (size.ToLowerInvariant() == "large")
                    {
                        currentrow.BeginEdit();
                        currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn] = false;
                        currentrow.EndEdit();
                    }
                    
                    if (size.ToLowerInvariant() == "medium") 
                    {
                        currentrow.BeginEdit();
                        currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn] = false;

                        //remove the default tag to this image
                        if (isDefaultMedium)
                        {
                            currentrow[itemDataset.InventoryOverrideImage.IsDefaultMediumColumn] = false;
                        }
                        
                        currentrow.EndEdit();
                        
                        TryLocateRecordForNewDefaultMedium(itemDataset, itemcode, fileName);
                    }
                    
                    if (size.ToLowerInvariant() == "icon") 
                    {
                        currentrow.BeginEdit();
                        currentrow[itemDataset.InventoryOverrideImage.HasIconColumn]  = false;

                        if (isDefaultIcon)
                        {
                            currentrow[itemDataset.InventoryOverrideImage.IsDefaultIconColumn] = false;
                        }
                        
                        currentrow.EndEdit();

                        TryLocateRecordForNewDefaultIcon(itemDataset, itemcode, fileName);
                    }
                    
                }

                //if (size.ToLowerInvariant() == "minicart") { overrideRow.HasMinicart = false; }
                //if (size.ToLowerInvariant() == "mobile") { overrideRow.HasMobile = false; }

                //isExist test the image if exist 
                //if no image icon is the src. then image does not exist
                if (isDeleteAll)
                {
                    bool exists = false;
                        
                    string url = AppLogic.LocateImageFilenameUrl("product", itemcode, "large", fileName, false, out exists);
                    if (exists) { DeleteFile(url); }

                    url = AppLogic.LocateImageFilenameUrl("product", itemcode, "medium", fileName, false, out exists);
                    if (exists) { DeleteFile(url); }

                    url = AppLogic.LocateImageFilenameUrl("product", itemcode, "icon", fileName, false, out exists);
                    if (exists) { DeleteFile(url); }
                        
                    url = AppLogic.LocateImageFilenameUrl("product", itemcode, "mobile", fileName, false, out exists);
                    if (exists) { DeleteFile(url); }

                    url = AppLogic.LocateImageFilenameUrl("product", itemcode, "minicart", fileName, false, out exists);
                    if (exists) { DeleteFile(url); }
                }
                else
                {
                    if (isExist)
                    {
                        //delete valid image
                        DeleteFile(srcfilename);
                    }
                }

            }
            else  //update
            {
                UpdateRowBySizes(currentrow, size, itemDataset, itemcode);
            }

            if (itemDataset.HasChanges())
            {
                string[][] overrideImageCommandSet = overrideImageCommandSet = new String[][] {
                            new string[] {itemDataset.InventoryOverrideImage.TableName, "CREATEINVENTORYOVERRIDEIMAGE", 
                                           "UPDATEINVENTORYOVERRIDEIMAGE", "DELETEINVENTORYOVERRIDEIMAGE"},
                            new string[] {itemDataset.InventoryImageWebOptionDescription.TableName, "CREATEINVENTORYIMAGEWEBOPTIONDESCRIPTION", 
                                            "UPDATEINVENTORYIMAGEWEBOPTIONDESCRIPTION", "DELETEINVENTORYIMAGEWEBOPTIONDESCRIPTION"}                                
                };
                inventoryfacade.UpdateDataSet(overrideImageCommandSet, Interprise.Framework.Base.Shared.Enum.TransactionType.None, String.Empty, false);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    private void UpdateRowBySizes(Interprise.Framework.Inventory.DatasetComponent.ItemDetailDataset.InventoryOverrideImageRow currentrow, string size,          Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {

        if (currentrow == null) return;
        
        bool isDefaultIcon = currentrow.IsDefaultIcon;
        bool isDefaultMedium = currentrow.IsDefaultMedium;

        int totalRecords = GetTotalImagesCount(itemDataset, itemCode);
        
        if (size.ToLowerInvariant() == "large") 
        {
            currentrow.BeginEdit();
            currentrow[itemDataset.InventoryOverrideImage.HasLargeColumn.ColumnName] = true;
            currentrow.EndEdit();
        }
        
        if (size.ToLowerInvariant() == "medium") 
        {
            bool hasMedium = currentrow.HasMedium;
            currentrow.BeginEdit();
            currentrow[itemDataset.InventoryOverrideImage.HasMediumColumn.ColumnName] = true;
            currentrow.EndEdit();

            //Locate for default medium if the total records is 1 and the current image does not exist
            if(!IsDefaultMediumAlreadySetToOtherImages(itemDataset, itemCode))
            {
                TryLocateRecordForNewDefaultMedium(itemDataset, itemCode);
            }
        }
        
        if (size.ToLowerInvariant() == "icon") 
        {
            bool hasIcon = currentrow.HasIcon;
            
            currentrow.BeginEdit();
            currentrow[itemDataset.InventoryOverrideImage.HasIconColumn.ColumnName] = true;
            currentrow.EndEdit();

            //Locate for default icon if the total records is 1 and the current image does not exist
            if (!IsDefaultIconAlreadySetToOtherImages(itemDataset, itemCode))
            {
                TryLocateRecordForNewDefaultIcon(itemDataset, itemCode);
            }
        }
        
        if (size.ToLowerInvariant() == "minicart")
        {
            currentrow.BeginEdit();
            currentrow[itemDataset.InventoryOverrideImage.HasMinicartColumn.ColumnName] = true;
            currentrow.EndEdit();
        }
        
        if (size.ToLowerInvariant() == "mobile")
        {
            currentrow.BeginEdit();
            currentrow[itemDataset.InventoryOverrideImage.HasMobileColumn.ColumnName] = true;
            currentrow.EndEdit();
        }
        
        currentrow.EndEdit();
    }

    private int GetTotalImagesCount(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {
        return itemDataset.InventoryOverrideImage
                        .Select(String.Format("{0} = '{1}' AND {2} = '{3}'"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode))
                        .Count();
    }

    private bool IsDefaultMediumAlreadySetToOtherImages(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {
        return IsDefaultMediumAlreadySetToOtherImages(itemDataset, itemCode, String.Empty);
    }

    private bool IsDefaultMediumAlreadySetToOtherImages(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode, string fileName)
    {
        if (!fileName.IsNullOrEmptyTrimmed())
        {
            return (itemDataset.InventoryOverrideImage.Select(String.Format("{0} = '{1}' AND {2} = '{3}' AND {4} <> '{5}' AND IsDefaultMedium = 1"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName), "ImageIndex")
                            .Count() > 0);
        }
        else
        {
            return (itemDataset.InventoryOverrideImage.Select(String.Format("{0} = '{1}' AND {2} = '{3}' AND IsDefaultMedium = 1"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName), "ImageIndex")
                            .Count() > 0);            
        }
    }

    private bool IsDefaultIconAlreadySetToOtherImages(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {
        return IsDefaultIconAlreadySetToOtherImages(itemDataset, itemCode, String.Empty);
    }
    
    private bool IsDefaultIconAlreadySetToOtherImages(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode, string fileName)
    {
        if (!fileName.IsNullOrEmptyTrimmed())
        {
            //locate for othere record to set as default since we already delete the default
            return (itemDataset.InventoryOverrideImage
                        .Select(String.Format("{0} = '{1}' AND {2} = '{3}' AND {4} <> '{5}' AND IsDefaultIcon = 1"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName), "ImageIndex")
                        .Count() > 0
                    );
        }
        else
        {
            //locate for othere record to set as default since we already delete the default
            return (itemDataset.InventoryOverrideImage
                        .Select(String.Format("{0} = '{1}' AND {2} = '{3}' AND IsDefaultIcon = 1"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName), "ImageIndex")
                        .Count() > 0
                    );
        }
        
    }

    private void TryLocateRecordForNewDefaultIcon(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {
        TryLocateRecordForNewDefaultIcon(itemDataset, itemCode, String.Empty);
    }
        
    private void TryLocateRecordForNewDefaultIcon(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode, string fileName)
    {
        string qry = String.Empty;

        if (!fileName.IsNullOrEmptyTrimmed())
        {
            //locate for othere record to set as default since we already delete the default
            qry = String.Format("{0} = '{1}' AND {2} = '{3}' AND {4} <> '{5}' AND (IsNull(HasIcon, 0) = 1) AND IsDefaultIcon = 0"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName);
        }
        else
        {
            //locate for othere record to set as default since we already delete the default
            qry = String.Format("{0} = '{1}' AND {2} = '{3}' AND (IsNull(HasIcon, 0) = 1) AND IsDefaultIcon = 0"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode);
        }

        var currentrows = itemDataset.InventoryOverrideImage.Select(qry, "ImageIndex");
        if (currentrows.Count() > 0)
        {
            var firstRowOfItem = (currentrows[0] as Interprise.Framework.Inventory.DatasetComponent.ItemDetailDataset.InventoryOverrideImageRow);
            firstRowOfItem.BeginEdit();
            firstRowOfItem[itemDataset.InventoryOverrideImage.IsDefaultIconColumn.ColumnName] = true;
            firstRowOfItem.EndEdit();
        }
    }

    private void TryLocateRecordForNewDefaultMedium(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode)
    {
        TryLocateRecordForNewDefaultMedium(itemDataset, itemCode, String.Empty);
    }

    private void TryLocateRecordForNewDefaultMedium(Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway itemDataset, string itemCode, string fileName)
    {
        string qry = String.Empty;

        if (!fileName.IsNullOrEmptyTrimmed())
        {
            //locate for othere record to set as default since we already delete the default
            qry = String.Format("{0} = '{1}' AND {2} = '{3}' AND {4} <> '{5}' AND (IsNull(HasMedium, 0) = 1) AND IsDefaultMedium = 0"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName);
        }
        else
        {
            //locate for othere record to set as default since we already delete the default
            qry = String.Format("{0} = '{1}' AND {2} = '{3}' AND (IsNull(HasMedium, 0) = 1) AND IsDefaultMedium = 0"
                            , itemDataset.InventoryOverrideImage.ItemCodeColumn.ColumnName, itemCode
                            , itemDataset.InventoryOverrideImage.WebSiteCodeColumn.ColumnName, InterpriseHelper.ConfigInstance.WebSiteCode
                            , itemDataset.InventoryOverrideImage.FilenameColumn.ColumnName, fileName);
        }

        var currentrows = itemDataset.InventoryOverrideImage.Select(qry, "ImageIndex");
        if (currentrows.Count() > 0)
        {
            var firstRowOfItem = (currentrows[0] as Interprise.Framework.Inventory.DatasetComponent.ItemDetailDataset.InventoryOverrideImageRow);
            firstRowOfItem.BeginEdit();
            firstRowOfItem[itemDataset.InventoryOverrideImage.IsDefaultMediumColumn.ColumnName] = true;
            firstRowOfItem.EndEdit();
        }
    }

    /// <summary>
    /// src is the image relative path. ie - images/product/medium/test.jpg
    /// </summary>
    /// <param name="src"></param>
    private void DeleteFile(string src)
    {
        string filePath = CommonLogic.SafeMapPath(src);
        try
        {
            //add double checking due to file might be deleted from IS side
            if (!System.IO.File.Exists(filePath)) return;
            
            System.IO.File.Delete(filePath);
        }
        catch
        {
            throw;
        }
    }
    
    private int boolToInt(bool stat)
    {
        return (stat) ? 1 : 0;
    }

    private void CleanUp()
    {
        _inventoryFacade = null;
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }

    private HttpFileCollection UploadedFiles
    {
        get
        {
            return CurrentContext.Request.Files;
        }
    }

    private HttpPostedFile FirstUploadedFile
    {
        get
        {
            if (GetUploadParam().UploadType == UploadType.Upload)
            {
                if (CurrentContext.Request.Browser.Browser == "IE")
                {
                    return CurrentContext.Request.Files.Get(1);
                }
                else
                {
                    return CurrentContext.Request.Files.Get(0);
                }
            }
            else
            {
                return CurrentContext.Request.Files.Get(0);
            }
                
        }
    }

    private HttpContext CurrentContext
    {
        get;
        set;
    }

    private UploadParam GetUploadParam()
    {
        UploadParam param = null;
        var request = CurrentContext.Request;

        if (request.QueryString.Count > 0)
        {
            param = new UploadParam()
            {
                EntityCode = request.QueryString["ec"],
                Counter = request.QueryString["pk"].TryParseInt().Value,
                EntityType = request.QueryString["et"],
                FileName = request.QueryString["fName"],
                Size = request.QueryString["s"],
                SizeValue = request.QueryString["sv"].TryParseInt().Value,
                UploadType = request.QueryString["ut"].TryParseEnum<UploadType>(),
                IsApplyToAll = request.QueryString["aa"].TryParseBool().Value,
                isImageExist = request.QueryString["exist"].TryParseBool().Value,
                ImageSrc = request.QueryString["src"],
                IsDeleteAll = request.QueryString["da"].TryParseBool().Value,
            };
        }

        return param;
    }

    private bool IsValidateFileExtension(HttpPostedFile uploadedFile)
    {
        //Validate filename
        string filenameext = GetFileExtension(uploadedFile.FileName);
        return DomainConstants.GetImageSupportedExtensions()
                              .Any(ext => "." + ext.ToUpper() == filenameext.ToUpper());
    }

    private string GetFileExtension(string fileName)
    {
        return fileName.Substring(fileName.LastIndexOf("."));
    }

    private string GetImageContentTypeByExtension(string fileNameExt)
    {
        string contenType = String.Empty;
        switch (fileNameExt)
        {
            case ".jpg":
                contenType = "image/jpeg";
                break;
            case ".gif":
                contenType = "image/gif";
                break;
            case ".png":
                contenType = "image/png";
                break;
        }
        return contenType;
    }

    private string GetUploadedFileName(HttpPostedFile uploadedFile)
    {
        if (System.IO.File.Exists(uploadedFile.FileName))
        {
            return new System.IO.FileInfo(uploadedFile.FileName).Name;
        }
        else
        {
            return uploadedFile.FileName;
        }
    }
    
    private string GetFileNameWithoutExtension(string fileName)
    {
        return fileName.Substring(0, fileName.IndexOf("."));
    }

    public ImageUploadStatus UploadStatus
    {
        get 
        {
            if (_uploadStatus == null)
            {
                _uploadStatus = new ImageUploadStatus();
            }
            return _uploadStatus;
        }
    }
    
    private string SkinID
    {
        get
        {
            return CommonLogic.CookieUSInt(InterpriseSuiteEcommerce.SkinBase.ro_SkinCookieName).ToString();
        }
    }

    public Interprise.Facade.Inventory.ItemDetailFacade InventoryFacade
    {
        get 
        {
            if (_inventoryFacade == null)
            {
                var itemDataset = new Interprise.Framework.Inventory.DatasetGateway.ItemDetailDatasetGateway();
                _inventoryFacade = new Interprise.Facade.Inventory.ItemDetailFacade(itemDataset);
            }
            return _inventoryFacade; 
        }
    }
   
}