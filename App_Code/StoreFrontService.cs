// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Services;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;
using Microsoft.Web.Services3;
using System.Linq.Expressions;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;

/// <summary>
/// Summary description for ImageGalleryService
/// </summary>
[WebService(Namespace = "http://www.interprisedatacenter.com/services/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[Policy("InterpriseSuiteEcommerceServicePolicy")]
public class StoreFrontService : System.Web.Services.WebService
{
    #region Constructor
    public StoreFrontService() { }
    #endregion

    #region SaveImage
    /// <summary>
    /// SaveImage
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="path"></param>
    /// <param name="format"></param>
    /// <param name="overwriteExisting"></param>
    private void SaveImage(byte[] buffer, string path, ImageFormat format, bool overwriteExisting)
    {
        MemoryStream strm = new MemoryStream(buffer);
        Image img = Image.FromStream(strm);
        SaveImage(img, path, format, overwriteExisting);
        strm.Flush();
        strm.Dispose();
    }
    #endregion

    #region SaveImage
    /// <summary>
    /// SaveImage
    /// </summary>
    /// <param name="img"></param>
    /// <param name="path"></param>
    /// <param name="format"></param>
    /// <param name="overwriteExisting"></param>
    private void SaveImage(Image img, string path, ImageFormat format, bool overwriteExisting)
    {
        try
        {
            if (File.Exists(path))
            {
                if (overwriteExisting) { File.Delete(path); }
                else { return; }
            }

            img.Save(path, format);
            img.Dispose();
        }
        catch { }
    }
    #endregion

    #region GenerateNextSlideImageFileNames
    /// <summary>
    /// GenerateNextSlideImageFileNames
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="img1"></param>
    /// <param name="img2"></param>
    private void GenerateNextSlideImageFileNames(string dirPath, ref string img1, ref string img2)
    {
        string[] imgFiles = System.IO.Directory.GetFiles(dirPath);
        int nextSlideNumber = 1;
        foreach (string imgFile in imgFiles)
        {
            if (imgFile.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) &&
                imgFile.EndsWith("_lg.jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                nextSlideNumber++;
            }
        }

        img1 = string.Format("{0}\\slide{1}.jpg", dirPath, nextSlideNumber.ToString().PadLeft(2, '0'));
        img2 = string.Format("{0}\\slide{1}_lg.jpg", dirPath, nextSlideNumber.ToString().PadLeft(2, '0'));
    }
    #endregion

    #region CreateThumbNail
    /// <summary>
    /// CreateThumbNail
    /// </summary>
    /// <param name="mainImage"></param>
    /// <returns></returns>
    private Image CreateThumbNail(Image mainImage)
    {
        Single sizer = 0;
        int boxWidth = 125;
        int boxHeight = 125;
        int newWidth = 0;
        int newHeight = 0;

        if (mainImage.Height > mainImage.Width)
        {
            sizer = (Single)boxWidth / (Single)mainImage.Height;
        }
        else
        {
            sizer = (Single)boxHeight / (Single)mainImage.Width;
        }

        newWidth = Convert.ToInt32(mainImage.Width * sizer);
        newHeight = Convert.ToInt32(mainImage.Height * sizer);

        return mainImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
    }
    #endregion

    #region GetAvailablePackages
    /// <summary>
    /// Retrieves the available xml packages...
    /// </summary>
    /// <param name="packagePrefix"></param>
    /// <returns></returns>
    [WebMethod()]
    public string[] GetAvailablePackages(string packagePrefix)
    {
        var availablePackages = AppLogic.ReadXmlPackages(packagePrefix, AppLogic.DefaultSkinID());
        if (availablePackages != null && availablePackages.Length > 0)
        {
            return availablePackages;
        }

        return new string[] { };
    }

    [WebMethod()]
    public string[] GetMobileAvailablePackages(string packagePrefix)
    {
        var availablePackages = AppLogic.ReadXmlPackages(packagePrefix, 1, false, true);
        if (availablePackages != null && availablePackages.Length > 0)
        {
            return availablePackages;
        }

        return new string[] { };
    }

    #endregion

    #region GetProductDirectory
    private string GetProductDirectory()
    {
        return ServiceFactory.GetInstance<IIOService>().GetProductImageDirectory();
    }
    #endregion

    #region GetManufacturerDirectory
    private string GetManufacturerDirectory()
    {
        return ServiceFactory.GetInstance<IIOService>().GetManufacturerImageDirectory();
    }
    #endregion

    #region GetCategoryDirectory
    private string GetCategoryDirectory()
    {
        return ServiceFactory.GetInstance<IIOService>().GetCategoryImageDirectory();
    }
    #endregion

    #region GetDepartmentDirectory
    private string GetDepartmentDirectory()
    {
        return ServiceFactory.GetInstance<IIOService>().GetDepartmentImageDirectory();
    }
    #endregion

    #region GetAttributeDirectory
    private string GetAttributeDirectory()
    {
        return ServiceFactory.GetInstance<IIOService>().GetAttributeImageDirectory();
    }
    #endregion

    #region GetEntityDirectory

    private string GetEntityDirectory(ImageType entityType)
    {
        string entityDirectory = string.Empty;

        switch (entityType)
        {
            case ImageType.Item:
                entityDirectory = GetProductDirectory();
                break;
            case ImageType.Category:
                entityDirectory = GetCategoryDirectory();
                break;
            case ImageType.Department:
                entityDirectory = GetDepartmentDirectory();
                break;
            case ImageType.Manufacturer:
                entityDirectory = GetManufacturerDirectory();
                break;
            case ImageType.Attribute:
                entityDirectory = GetAttributeDirectory();
                break;
        }

        return entityDirectory;
    }

    private string GetMobileEntityDirectory(ImageType entityType)
    {
        string entityDirectory = string.Empty;

        string basePath = CommonLogic.SafeMapPath("mobile/images/spacer.gif");
        string replaceValue = "mobile\\images\\spacer.gif";

        switch (entityType)
        {
            case ImageType.Item:
                entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\product");
                break;
            case ImageType.Category:
                entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\category");
                break;
            case ImageType.Department:
                entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\department");
                break;
            case ImageType.Manufacturer:
                entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\manufacturer");
                break;
            case ImageType.Attribute:
                entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\attribute");
                break;
        }

        if (!Directory.Exists(entityDirectory)) { return string.Empty; }
        return entityDirectory;
    }

    private string GetMobileEntityDirectory(string entityFolder)
    {
        string entityDirectory = string.Empty;

        string basePath = CommonLogic.SafeMapPath("mobile/images/spacer.gif");
        string replaceValue = "mobile\\images\\spacer.gif";

        if (entityFolder.ToLower().EndsWith("product") || entityFolder.ToLower().EndsWith("product\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\product"); }
        else if (entityFolder.ToLower().EndsWith("category") || entityFolder.ToLower().EndsWith("category\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\category"); }
        else if (entityFolder.ToLower().EndsWith("department") || entityFolder.ToLower().EndsWith("department\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\department"); }
        else if (entityFolder.ToLower().EndsWith("manufacturer") || entityFolder.ToLower().EndsWith("manufacturer\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\manufacturer"); }
        else if (entityFolder.ToLower().EndsWith("attribute") || entityFolder.ToLower().EndsWith("attribute\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\attribute"); }
        else if (entityFolder.ToLower().EndsWith("section") || entityFolder.ToLower().EndsWith("section\\")) { entityDirectory = basePath.Replace(replaceValue, "mobile\\images\\department"); }

        if (!Directory.Exists(entityDirectory)) { return string.Empty; }
        return entityDirectory;

    }

    #endregion

    #region SaveEntityImage
    [WebMethod()]
    public void SaveEntityImage(EntityImageData data, ImageType entityType)
    {
        string entityDirectory = GetEntityDirectory(entityType);
        if (!CommonLogic.IsStringNullOrEmpty(entityDirectory))
        {
            SaveEntityImageData(data, entityType, entityDirectory);
        }
    }
    #endregion

    #region SaveEntityImageData
    public void SaveEntityImageData(EntityImageData data, ImageType entityType, string entityDirectory)
    {
        if (data.Icon != null)
        {
            UpdateProductImage(string.Format("{0}\\icon\\{1}", entityDirectory, data.Id.ToString()), data.Id.ToString(), "icon", entityType, data.Icon, false);
        }

        if (data.Medium != null)
        {
            UpdateProductImage(string.Format("{0}\\medium\\{1}", entityDirectory, data.Id.ToString()), data.Id.ToString(), "medium", entityType, data.Medium, false);
        }

        if (data.Large != null)
        {
            UpdateProductImage(string.Format("{0}\\large\\{1}", entityDirectory, data.Id.ToString()), data.Id.ToString(), "large", entityType, data.Large, false);
        }

        if (data.MiniCart != null)
        {
            UpdateProductImage(string.Format("{0}\\minicart\\{1}", entityDirectory, data.Id.ToString()), data.MiniCart);
        }

        if (data.Mobile != null)
        {
            string mobileDirectory = GetMobileEntityDirectory(entityType);
            UpdateProductImage(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.Id.ToString()), data.Mobile);
        }

        for (int ctr = 0; ctr < 10; ctr++)
        {
            int number = ctr + 1;

            //Only save the "multi-image" if the main medium image is set or is not being deleted.
            if (data.Medium != null && data.Medium.State != State.Deleted)
            {
                if (data.MediumImages.Count > 0)
                {
                    if (data.MediumImages[ctr] != null)
                    {
                        UpdateProductImage(string.Format("{0}\\medium\\{1}_{2}", entityDirectory, data.Id.ToString(), number), string.Format("{0}_{1}", data.Id.ToString(), number), "medium", entityType, data.MediumImages[ctr], true);
                    }
                }
            }
            else
            {
                //Delete the medium images.
                DeleteProductImages(string.Format("{0}\\medium\\{1}_{2}", entityDirectory, data.Id.ToString(), number));

                //Delete the micro images as well.
                DeleteProductImages(string.Format("{0}\\micro\\{1}_{2}", entityDirectory, data.Id.ToString(), number));
            }


            //Only save the "multi-image" if the main large image is set or is not being deleted.
            if (data.Large != null && data.Large.State != State.Deleted)
            {
                if (data.LargeImages.Count > 0)
                {
                    if (data.LargeImages[ctr] != null)
                    {
                        UpdateProductImage(string.Format("{0}\\large\\{1}_{2}", entityDirectory, data.Id.ToString(), number), string.Format("{0}_{1}", data.Id.ToString(), number), "large", entityType, data.LargeImages[ctr], true);
                    }
                }
            }
            else
            {
                //Delete the large images.
                DeleteProductImages(string.Format("{0}\\large\\{1}_{2}", entityDirectory, data.Id.ToString(), number));
            }
        }

        if (null != data.Swatches && data.Swatches.Count > 0)
        {
            foreach (EntityImageSwatch productSwatch in data.Swatches)
            {
                UpdateProductImage(string.Format("{0}\\swatch\\{1}_{2}", entityDirectory, data.Id.ToString(), productSwatch.Id.ToString()), string.Format("{0}_{1}", data.Id.ToString(), productSwatch.Id.ToString()), "swatch", entityType, productSwatch.Image, true);
            }
        }
    }

    public void SaveEntityImageData(EntityImageData data, string entityDirectory)
    {
        if (data.Icon != null)
        {
            UpdateProductImage(string.Format("{0}\\icon\\{1}", entityDirectory, data.Id.ToString()), data.Icon);
        }

        if (data.Medium != null)
        {
            UpdateProductImage(string.Format("{0}\\medium\\{1}", entityDirectory, data.Id.ToString()), data.Medium);
        }

        if (data.Large != null)
        {
            UpdateProductImage(string.Format("{0}\\large\\{1}", entityDirectory, data.Id.ToString()), data.Large);
        }

        if (data.MiniCart != null)
        {
            UpdateProductImage(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MiniCart.ImageFileName.Split('.')[0]), data.MiniCart);
        }

        if (data.Mobile != null)
        {
            string mobileDirectory = GetMobileEntityDirectory(entityDirectory);
            UpdateProductImage(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.Mobile.ImageFileName.Split('.')[0]), data.Mobile);
        }

        for (int ctr = 0; ctr < 10; ctr++)
        {
            int number = ctr + 1;

            //Only save the "multi-image" if the main medium image is set or is not being deleted.
            if (data.Medium != null && data.Medium.State != State.Deleted)
            {
                if (data.MediumImages.Count > 0)
                {
                    if (data.MediumImages[ctr] != null)
                    {
                        UpdateProductImage(string.Format("{0}\\medium\\{1}_{2}", entityDirectory, data.Id, number), data.MediumImages[ctr]);
                    }
                }
            }
            else
            {
                //Delete the medium images.
                DeleteProductImages(string.Format("{0}\\medium\\{1}_{2}", entityDirectory, data.Id.ToString(), number));
            }


            //Only save the "multi-image" if the main large image is set or is not being deleted.
            if (data.Large != null && data.Large.State != State.Deleted)
            {
                if (data.LargeImages.Count > 0)
                {
                    if (data.LargeImages[ctr] != null)
                    {
                        UpdateProductImage(string.Format("{0}\\large\\{1}_{2}", entityDirectory, data.Id, number), data.LargeImages[ctr]);
                    }
                }
            }
            else
            {
                //Delete the large images.
                DeleteProductImages(string.Format("{0}\\large\\{1}_{2}", entityDirectory, data.Id.ToString(), number));
            }
        }

        if (null != data.Swatches && data.Swatches.Count > 0)
        {
            foreach (EntityImageSwatch productSwatch in data.Swatches)
            {
                UpdateProductImage(string.Format("{0}\\swatch\\{1}_{2}", entityDirectory, data.Id, productSwatch.Id), productSwatch.Image);
            }
        }
    }
    #endregion

    #region SaveEntityImageFileName
    [WebMethod()]
    public void SaveEntityImageFileName(EntityImageData data, ImageType entityType)
    {
        string entityDirectory = GetEntityDirectory(entityType);
        if (!entityDirectory.IsNullOrEmptyTrimmed())
        {
            SaveEntityImageFileNameData(data, entityType, entityDirectory);
        }

        //string mobileDirectory = GetMobileEntityDirectory(entityType);
        //if (!entityDirectory.IsNullOrEmptyTrimmed())
        //{
        //    SaveEntityImageFileNameData(data, entityType, mobileDirectory);
        //}
    }
    #endregion

    #region SaveEntityImageFileNameWithoutResizing
    [WebMethod()]
    public void SaveEntityImageFileNameWithoutResizing(EntityImageData data, ImageType entityType)
    {
        string entityDirectory = GetEntityDirectory(entityType);
        if (!entityDirectory.IsNullOrEmptyTrimmed())
        {
            SaveEntityImageFileNameData(data, entityType, entityDirectory, true);
        }
    }
    #endregion

    #region SaveEntityImageFileNameData
    public void SaveEntityImageFileNameData(EntityImageData data, ImageType entityType, string entityDirectory, bool disableImageResizing = false)
    {
        string mobileDirectory = GetMobileEntityDirectory(entityType);

        // icon images
        for (int ctr = 0; ctr < data.IconImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.IconImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\icon\\{1}", entityDirectory, data.IconImages[ctr].ImageFileName.Split('.')[0].ToString()), string.Format("{0}", data.IconImages[ctr].ImageFileName.Split('.')[0].ToString()), "icon", entityType, data.IconImages[ctr], true, disableImageResizing);
            }
        }

        // medium images
        for (int ctr = 0; ctr < data.MediumImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MediumImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\medium\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString()), string.Format("{0}", data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString()), "medium", entityType, data.MediumImages[ctr], true, disableImageResizing);
            }
            else
            {
                //Delete the medium images.
                DeleteProductImages(string.Format("{0}\\medium\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString(), number));

                //Delete the micro images as well.
                DeleteProductImages(string.Format("{0}\\micro\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString(), number));

                //Delete the micro images as well.
                DeleteProductImages(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString(), number));

                //Delete the mobile images as well.
                DeleteProductImages(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }

        // large images
        for (int ctr = 0; ctr < data.LargeImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.LargeImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\large\\{1}", entityDirectory, data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString()), string.Format("{0}", data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString()), "large", entityType, data.LargeImages[ctr], true, disableImageResizing);
            }
            else
            {
                //Delete the large images.
                DeleteProductImages(string.Format("{0}\\large\\{1}", entityDirectory, data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString(), number));

                DeleteProductImages(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }

        // use above code when swatch filename is ready
        if (null != data.Swatches && data.Swatches.Count > 0)
        {
            foreach (EntityImageSwatch productSwatch in data.Swatches)
            {
                UpdateProductImage(string.Format("{0}\\swatch\\{1}_{2}", entityDirectory, data.Id.ToString(), productSwatch.Id.ToString()), string.Format("{0}_{1}", data.Id.ToString(), productSwatch.Id.ToString()), "swatch", entityType, productSwatch.Image, true, disableImageResizing);
            }
        }

        // minicart images
        for (int ctr = 0; ctr < data.MinicartImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MinicartImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MinicartImages[ctr].ImageFileName.Split('.')[0].ToString()), data.MinicartImages[ctr]);
            }
            else
            {
                //Delete the minicart images.
                DeleteProductImages(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MinicartImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }


        // mobile images
        for (int ctr = 0; ctr < data.MobileImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MobileImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.MobileImages[ctr].ImageFileName.Split('.')[0].ToString()), data.MobileImages[ctr]);
            }
            else
            {
                //Delete the mobile images.
                DeleteProductImages(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.MobileImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }

        AssignDefaultImageToItem(data.Id);

    }

    public void SaveEntityImageFileNameData(EntityImageData data, string entityDirectory)
    {
        // icon images
        for (int ctr = 0; ctr < data.IconImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.IconImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\icon\\{1}", entityDirectory, data.IconImages[ctr].ImageFileName.Split('.')[0].ToString()), data.IconImages[ctr]);
            }
        }

        // medium images
        for (int ctr = 0; ctr < data.MediumImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MediumImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\medium\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString()), data.MediumImages[ctr]);
            }
            else
            {
                //Delete the medium images.
                DeleteProductImages(string.Format("{0}\\medium\\{1}", entityDirectory, data.MediumImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }

        // large images
        for (int ctr = 0; ctr < data.LargeImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.LargeImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\large\\{1}", entityDirectory, data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString()), data.LargeImages[ctr]);
            }
            else
            {
                //Delete the large images.
                DeleteProductImages(string.Format("{0}\\large\\{1}", entityDirectory, data.LargeImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }

        }

        // use above code when swatch filename is applicable
        if (null != data.Swatches && data.Swatches.Count > 0)
        {
            foreach (EntityImageSwatch productSwatch in data.Swatches)
            {
                UpdateProductImage(string.Format("{0}\\swatch\\{1}_{2}", entityDirectory, data.Id, productSwatch.Id), productSwatch.Image);
            }
        }

        // minicart images
        for (int ctr = 0; ctr < data.MinicartImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MinicartImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MinicartImages[ctr].ImageFileName.Split('.')[0].ToString()), data.MinicartImages[ctr]);
            }
            else
            {
                //Delete the minicart images.
                DeleteProductImages(string.Format("{0}\\minicart\\{1}", entityDirectory, data.MinicartImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }


        // mobile images
        string mobileDirectory = GetMobileEntityDirectory(entityDirectory);
        for (int ctr = 0; ctr < data.MobileImages.Count; ctr++)
        {
            int number = ctr + 1;
            if (data.MobileImages[ctr] != null)
            {
                UpdateProductImage(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.MobileImages[ctr].ImageFileName.Split('.')[0].ToString()), data.MobileImages[ctr]);
            }
            else
            {
                //Delete the mobile images.
                DeleteProductImages(string.Format("{0}\\mobile\\{1}", mobileDirectory, data.MobileImages[ctr].ImageFileName.Split('.')[0].ToString(), number));
            }
        }

    }
    #endregion

    #region UpdateProductImage

    private void UpdateProductImage(string filePathWithoutExtension, string fileNameWithoutPathAndExtension, string imageSizeType, ImageType entityType, EntityImage image, bool isMultiImage, bool disableImageResizing = false)
    {

        if (image.State == State.UnChanged) { return; }

        try
        {
            string[] extensions = new string[] { "jpg", "gif", "png" };
            string correctExtension = string.Empty;
            string img_ContentType = string.Empty;
            ImageFormat format = null;
            string imgEntityType = string.Empty;

            string mobileDirectory = GetMobileEntityDirectory(entityType);

            switch (image.ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":
                    correctExtension = "jpg";
                    img_ContentType = "image/jpeg";
                    format = ImageFormat.Jpeg;
                    break;

                case "image/gif":
                    correctExtension = "gif";
                    img_ContentType = "image/gif";
                    format = ImageFormat.Gif;
                    break;

                case "image/x-png":
                case "image/png":
                    correctExtension = "png";
                    img_ContentType = "image/png";
                    format = ImageFormat.Png;
                    break;
            }

            // If the format isn't supported, don't write it...
            if (!CommonLogic.IsStringNullOrEmpty(correctExtension) || format != null)
            {
                //First delete the existing image.
                DeleteProductImages(filePathWithoutExtension);

                if (imageSizeType == "medium")
                {
                    if (AppLogic.AppConfigBool("UseImageResize") || image.State == State.Deleted)
                    {
                        //If this is a medium sized image we need to also delete the mini cart image.
                        if (image.HasMinicartImage || image.State == State.Deleted)
                        {
                            DeleteProductImages(filePathWithoutExtension.Replace("\\medium\\", "\\minicart\\"));
                        }

                        //If this is a medium sized image we need to also delete the micro image.
                        if ((image.HasMicroImage || AppLogic.AppConfigBool("MultiMakesMicros")) || image.State == State.Deleted)
                        {
                            DeleteProductImages(filePathWithoutExtension.Replace("\\medium\\", "\\micro\\"));
                        }
                    }
                }

                //If the state was set to delete then exit the function.
                if (image.State == State.Deleted) { return; }

                //Save the file with the current requested extension.
                string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, correctExtension);

                if (imageSizeType == "medium")
                {
                    imgEntityType = CommonLogic.IIF(entityType.ToString() == ImageType.Item.ToString(), "Product", entityType.ToString());
                    if (imgEntityType == "Product" && (!image.HasMinicartImage))
                    {
                        using (MemoryStream strmmini = new MemoryStream(image.ImageRaw))
                        {
                            using (Image imgmini = Image.FromStream(strmmini))
                            {
                                AppLogic.MakeMiniCartPic(fileNameWithoutPathAndExtension, imgmini);
                            }
                        }
                    }
                }

                using (MemoryStream strm = new MemoryStream(image.ImageRaw))
                {
                    using (Image img = Image.FromStream(strm))
                    {
                        imgEntityType = CommonLogic.IIF(entityType.ToString() == ImageType.Item.ToString(), "Product", entityType.ToString());
                        AppLogic.ResizeEntityOrObject(imgEntityType, img, fileNameWithoutPathAndExtension, imageSizeType, img_ContentType, disableImageResizing);

                        if (AppLogic.AppConfigBool("UseImageResize"))
                        {
                            if ((imageSizeType == "medium") && AppLogic.AppConfigBool("MultiMakesMicros"))
                            {
                                AppLogic.MakeMicroPic(fileNameWithoutPathAndExtension, img);
                            }

                            if (imageSizeType == "large")
                            {
                                AppLogic.CreateOthersFromLarge(imgEntityType, img, fileNameWithoutPathAndExtension, img_ContentType, isMultiImage, disableImageResizing);
                            }
                        }

                        strm.Flush();
                    }
                }


                if (image.HasMobileImage) { return; }

                //for mobile image
                string mobileFile = string.Format("{0}\\mobile\\{1}", mobileDirectory, fileNameWithoutPathAndExtension);
                DeleteProductImages(mobileFile);

                using (var streamImage = new MemoryStream(image.ImageRaw))
                {
                    using (Image imgMobile = System.Drawing.Image.FromStream(streamImage))
                    {
                        AppLogic.MakeMobilePic(imgEntityType, fileNameWithoutPathAndExtension, imgMobile, correctExtension);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void UpdateProductImage(string filePathWithoutExtension, EntityImage image)
    {
        if (image.State == State.UnChanged) { return; }

        try
        {
            string[] extensions = new string[] { "jpg", "gif", "png" };
            string correctExtension = string.Empty;
            ImageFormat format = null;

            switch (image.ContentType)
            {
                case "image/jpeg":
                case "image/jpg":
                    correctExtension = "jpg";
                    format = ImageFormat.Jpeg;
                    break;

                case "image/gif":
                    correctExtension = "gif";
                    format = ImageFormat.Gif;
                    break;

                case "image/x-png":
                    correctExtension = "png";
                    format = ImageFormat.Png;
                    break;
            }

            // If the format isn't supported, don't write it...
            if (!CommonLogic.IsStringNullOrEmpty(correctExtension) || format != null)
            {
                //First delete the existing image.
                DeleteProductImages(filePathWithoutExtension);

                //If the state was set to delete then exit the function.
                if (image.State == State.Deleted) { return; }


                string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, correctExtension);

                using (MemoryStream strm = new MemoryStream(image.ImageRaw))
                {
                    using (Image img = Image.FromStream(strm))
                    {
                        using (FileStream outStream = new FileStream(filePathWithExtension, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            img.Save(outStream, format);
                            strm.Flush();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region DeleteProductImages

    /// <summary>
    /// Deletes the file for every supported extension. 
    /// </summary>
    /// <param name="filePathWithoutExtension">The path to the file without the extension.</param>
    private void DeleteProductImages(string filePathWithoutExtension)
    {
        string filePathWithExtension;
        //Loop through each extension and delete the matching file if it exists.
        foreach (string ext in _supportedExtensions)
        {
            filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, ext);
            if (CommonLogic.FileExists(filePathWithExtension))
            {
                System.IO.File.Delete(filePathWithExtension);
            }
        }
    }


    #endregion

    #region _supportedExtensions
    private static string[] _supportedExtensions = { "jpg", "jpeg", "gif", "png" };
    #endregion

    #region GetImageFormatByExtension
    private ImageFormat GetImageFormatByExtension(string extension)
    {
        ImageFormat format = null;
        switch (extension.ToLowerInvariant())
        {
            case "jpg":
            case "jpeg":
                format = ImageFormat.Jpeg;
                break;
            case "gif":
                format = ImageFormat.Gif;
                break;
            case "png":
                format = ImageFormat.Png;
                break;
        }

        return format;
    }
    #endregion

    #region GetEntityImage
    [WebMethod()]
    public EntityImageData GetEntityImage(int entityCounter, ImageType imageType)
    {
        string entityFolder = GetEntityDirectory(imageType);
        if (!CommonLogic.IsStringNullOrEmpty(entityFolder))
        {
            bool existing = false;
            bool entityIsItemAndTypeIsMatrixGroup = false;

            switch (imageType)
            {
                case ImageType.Item:
                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.ItemType FROM InventoryItem e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                        {
                            existing = reader.Read();
                            if (existing)
                            {
                                entityIsItemAndTypeIsMatrixGroup = (DB.RSField(reader, "ItemType") == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_MATRIX_GROUP);
                            }
                        }
                    }
                    break;
                case ImageType.Category:

                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.* FROM SystemCategory e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                        {
                            existing = reader.Read();
                        }
                    }
                    break;
                case ImageType.Manufacturer:

                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.* FROM SystemManufacturer e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                        {
                            existing = reader.Read();
                        }
                    }
                    break;
                case ImageType.Department:

                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.* FROM InventorySellingDepartment e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                        {
                            existing = reader.Read();
                        }
                    }
                    break;

                case ImageType.Attribute:

                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.* FROM SystemItemAttributeSourceFilterValue e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                        {
                            existing = reader.Read();
                        }
                    }
                    break;
            }

            if (existing)
            {
                return GetEntityImageData(entityCounter, entityFolder, entityIsItemAndTypeIsMatrixGroup);
            }
        }

        return null;
    }
    #endregion

    #region GetEntityImageData
    public EntityImageData GetEntityImageData(int entityCounter, string entityFolder, bool entityIsItemAndTypeIsMatrixGroup)
    {
        try
        {
            string productDirectory = GetProductDirectory();
            string mobileproductDirectory = GetMobileEntityDirectory(entityFolder);
            EntityImageData data = new EntityImageData();

            data.Id = entityCounter;

            data.Icon = FindImage(string.Format("{0}\\icon\\{1}", entityFolder, entityCounter.ToString()), true, string.Empty);
            data.Medium = FindImage(string.Format("{0}\\medium\\{1}", entityFolder, entityCounter.ToString()), true, string.Format("{0}\\micro\\{1}", entityFolder, entityCounter.ToString()));
            data.Large = FindImage(string.Format("{0}\\large\\{1}", entityFolder, entityCounter.ToString()), true, string.Empty);
            data.Mobile = FindImage(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, entityCounter.ToString()), true, string.Empty);

            for (int ctr = 1; ctr <= 10; ctr++)
            {
                data.MediumImages.Add(
                    FindImage(
                        string.Format("{0}\\medium\\{1}_{2}", entityFolder, entityCounter, ctr),
                        true,
                        string.Format("{0}\\micro\\{1}_{2}", entityFolder, entityCounter, ctr)
                    )
                );

                data.LargeImages.Add(
                    FindImage(
                        string.Format("{0}\\large\\{1}_{2}", entityFolder, entityCounter, ctr),
                        true,
                        string.Empty
                    )
                );
            }

            if (entityIsItemAndTypeIsMatrixGroup)
            {
                List<int> counterOfItemsThatIsConfiguredForSwatch = new List<int>();

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.ItemType, i.Counter as MatrixCounter, i.ItemCode, id.Counter as Counter, imi.MatrixItemCode, imi.IsSwatchItem FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryMatrixItem imi with (NOLOCK) ON imi.ItemCode = i.ItemCode INNER JOIN InventoryItem id with (NOLOCK) ON imi.MatrixItemCode = id.ItemCode WHERE i.Counter = {0} AND imi.IsSwatchItem = 1", entityCounter))
                    {
                        while (reader.Read())
                        {
                            counterOfItemsThatIsConfiguredForSwatch.Add(DB.RSFieldInt(reader, "Counter"));
                        }
                    }
                }

                // now let's see if we have a swatch image for the item
                foreach (int swatchItemCounter in counterOfItemsThatIsConfiguredForSwatch)
                {
                    EntityImageSwatch swatch = new EntityImageSwatch();
                    swatch.Image = FindImage(string.Format("{0}\\swatch\\{1}_{2}", entityFolder, entityCounter, swatchItemCounter), true, string.Empty);
                    swatch.Id = swatchItemCounter;
                    data.Swatches.Add(swatch);
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region SyncImageFile
    [WebMethod()]
    public string[] SyncImageFile(ImageType imageType)
    {
        var ItemCounterList = new List<int>();
        string entityFolder = GetEntityDirectory(imageType); //Item
        
        string FilePath = GetProductDirectory();
        var DirectoryFiles = System.IO.Directory.GetFiles(FilePath, "*", System.IO.SearchOption.AllDirectories);

        FilePath = FilePath + "\\";

        //DirectoryFiles = DirectoryFiles.Select(d => d.Replace(FilePath, string.Empty)).ToArray();

        for (int i = 0; i <= DirectoryFiles.Length - 1; i++)
        {
            DirectoryFiles[i] = DirectoryFiles[i].Replace(FilePath, string.Empty);
        }

        return DirectoryFiles; //returns complete lists of files in Product directory
    }
    #endregion

    #region SyncImageFileData
    public EntityImageData SyncImageFileData(List<int> ItemCounterList, string entityFolder, bool entityIsItemAndTypeIsMatrixGroup)
    {
        try
        {
            string productDirectory = GetProductDirectory();
            EntityImageData data = new EntityImageData();

            for (int ctr = 0; ctr <= ItemCounterList.Count - 1; ctr++)
            {
                //for default icon image
                EntityImage IconImage = new EntityImage();
                IconImage = FindImageData(
                            string.Format("{0}\\icon\\{1}", entityFolder, ItemCounterList[ctr]),
                            false,
                            string.Empty,
                            ItemCounterList[ctr].ToString()
                        );
                if (IconImage != null)
                    data.IconImages.Add(IconImage);

                //for medium default image
                EntityImage MediumImage = new EntityImage();
                MediumImage = FindImageData(
                            string.Format("{0}\\medium\\{1}", entityFolder, ItemCounterList[ctr]),
                            false,
                            string.Format("{0}\\micro\\{1}", entityFolder, ItemCounterList[ctr]),
                            ItemCounterList[ctr].ToString()
                        );
                if (MediumImage != null)
                    data.MediumImages.Add(MediumImage);

                //for medium multi image
                for (int ctrm = 1; ctrm <= 10; ctrm++)
                {
                    EntityImage MultiMediumImage = new EntityImage();
                    MultiMediumImage = FindImageData(
                            string.Format("{0}\\medium\\{1}_{2}", entityFolder, ItemCounterList[ctr], ctrm),
                            false,
                            string.Format("{0}\\micro\\{1}_{2}", entityFolder, ItemCounterList[ctr], ctrm),
                            string.Format("{0}_{1}", ItemCounterList[ctr], ctrm)
                        );
                    if (MultiMediumImage != null)
                        data.MediumImages.Add(MultiMediumImage);
                }

                //for large default image
                EntityImage LargeImage = new EntityImage();
                LargeImage = FindImageData(
                            string.Format("{0}\\large\\{1}", entityFolder, ItemCounterList[ctr]),
                            false,
                            string.Empty,
                            ItemCounterList[ctr].ToString()
                        );
                if (LargeImage != null)
                    data.LargeImages.Add(LargeImage);

                //for large multi image
                for (int ctrl = 1; ctrl <= 10; ctrl++)
                {
                    EntityImage MultiLargeImage = new EntityImage();
                    MultiLargeImage = FindImageData(
                            string.Format("{0}\\large\\{1}_{2}", entityFolder, ItemCounterList[ctr], ctrl),
                            false,
                            string.Empty,
                            string.Format("{0}_{1}", ItemCounterList[ctr], ctrl)
                        );
                    if (MultiLargeImage != null)
                        data.LargeImages.Add(MultiLargeImage);
                }

                if (entityIsItemAndTypeIsMatrixGroup)
                {
                    List<int> counterOfItemsThatIsConfiguredForSwatch = new List<int>();

                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.ItemType, i.Counter as MatrixCounter, i.ItemCode, id.Counter as Counter, imi.MatrixItemCode, imi.IsSwatchItem FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryMatrixItem imi with (NOLOCK) ON imi.ItemCode = i.ItemCode INNER JOIN InventoryItem id with (NOLOCK) ON imi.MatrixItemCode = id.ItemCode WHERE i.Counter = {0} AND imi.IsSwatchItem = 1", ItemCounterList[ctr]))
                        {
                            while (reader.Read())
                            {
                                counterOfItemsThatIsConfiguredForSwatch.Add(DB.RSFieldInt(reader, "Counter"));
                            }
                        }
                    }

                    // now let's see if we have a swatch image for the item
                    foreach (int swatchItemCounter in counterOfItemsThatIsConfiguredForSwatch)
                    {
                        EntityImageSwatch swatch = new EntityImageSwatch();
                        swatch.Image = FindImage(string.Format("{0}\\swatch\\{1}_{2}", entityFolder, ItemCounterList[ctr], swatchItemCounter), true, string.Empty);
                        swatch.Id = swatchItemCounter;
                        data.Swatches.Add(swatch);
                    }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region GetEntityImageFileName
    [WebMethod()]
    public EntityImageData GetEntityImageFileName(int entityCounter, ImageType imageType)
    {
        string entityFolder = GetEntityDirectory(imageType);
        if (!CommonLogic.IsStringNullOrEmpty(entityFolder))
        {
            bool existing = false;
            bool entityIsItemAndTypeIsMatrixGroup = false;
            string ItemCode = string.Empty;
            List<string> IconImageFileName = new List<string>();
            List<string> MediumImageFileName = new List<string>();
            List<string> LargeImageFileName = new List<string>();
            List<string> MobileImageFileName = new List<string>();

            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.ItemCode, e.ItemType FROM InventoryItem e with (NOLOCK) WHERE e.Counter = {0}", entityCounter))
                {
                    existing = reader.Read();
                    if (existing)
                    {
                        entityIsItemAndTypeIsMatrixGroup = (DB.RSField(reader, "ItemType") == Interprise.Framework.Base.Shared.Const.ITEM_TYPE_MATRIX_GROUP);
                        ItemCode = (DB.RSField(reader, "ItemCode"));
                    }
                }

                //using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.Filename, e.HasIcon, e.HasMedium, e.HasLarge, e.HasMicro ,e.HasMobile FROM InventoryOverrideImage e with (NOLOCK) WHERE e.ItemCode = {0} AND WebSiteCode = {1}", DB.SQuote(ItemCode), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
                using (IDataReader reader = DB.GetRSFormat(con, "SELECT e.Filename FROM InventoryOverrideImage e with (NOLOCK) WHERE e.ItemCode = {0} AND WebSiteCode = {1}", DB.SQuote(ItemCode), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
                {
                    while (reader.Read())
                    {
                        //if ((DB.RSFieldBool(reader, "HasIcon")))
                        IconImageFileName.Add(DB.RSField(reader, "Filename").ToString());
                        //if ((DB.RSFieldBool(reader, "HasMedium")))
                        MediumImageFileName.Add(DB.RSField(reader, "Filename").ToString());
                        //if ((DB.RSFieldBool(reader, "HasLarge")))
                        LargeImageFileName.Add(DB.RSField(reader, "Filename").ToString());
                        //if ((DB.RSFieldBool(reader, "HasMobile")) || (DB.RSFieldBool(reader, "HasMedium")))
                        MobileImageFileName.Add(DB.RSField(reader, "Filename").ToString());
                    }
                }
            }

            if (existing)
            {
                return GetEntityImageFileNameData(IconImageFileName, MediumImageFileName, LargeImageFileName, MobileImageFileName, entityFolder, entityIsItemAndTypeIsMatrixGroup, entityCounter);
            }
        }

        return null;
    }
    #endregion

    #region GetEntityImageFileNameData
    public EntityImageData GetEntityImageFileNameData(List<string> IconFilename, List<string> MediumFilename, List<string> LargeFilename, string entityFolder, bool entityIsItemAndTypeIsMatrixGroup, int entityCounter)
    {
        try
        {
            string productDirectory = GetProductDirectory();
            string mobileproductDirectory = GetMobileEntityDirectory(entityFolder);
            EntityImageData data = new EntityImageData();
            //get icon images here
            for (int ctri = 0; ctri <= IconFilename.Count - 1; ctri++)
            {
                data.IconImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\icon\\{1}", entityFolder, IconFilename[ctri].Split('.')[0]),
                        true,
                        IconFilename[ctri],
                        string.Empty
                    )
                );
                data.IconImages[ctri].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, IconFilename[ctri]));
                data.IconImages[ctri].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, IconFilename[ctri]));
            }
            //get medium images here
            for (int ctrm = 0; ctrm <= MediumFilename.Count - 1; ctrm++)
            {

                data.MediumImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\medium\\{1}", entityFolder, MediumFilename[ctrm].Split('.')[0]),
                        true,
                        MediumFilename[ctrm],
                        string.Format("{0}\\micro\\{1}", entityFolder, MediumFilename[ctrm].Split('.')[0])
                    )
                );

                data.MediumImages[ctrm].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, MediumFilename[ctrm]));
                data.MediumImages[ctrm].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, MediumFilename[ctrm]));
            }
            //get large images here
            for (int ctrl = 0; ctrl <= LargeFilename.Count - 1; ctrl++)
            {
                data.LargeImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\large\\{1}", entityFolder, LargeFilename[ctrl].Split('.')[0]),
                        true,
                        LargeFilename[ctrl],
                        string.Empty
                    )
                );
                data.LargeImages[ctrl].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, LargeFilename[ctrl]));
                data.LargeImages[ctrl].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, LargeFilename[ctrl]));
            }

            if (entityIsItemAndTypeIsMatrixGroup)
            {
                List<int> counterOfItemsThatIsConfiguredForSwatch = new List<int>();

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.ItemType, i.Counter as MatrixCounter, i.ItemCode, id.Counter as Counter, imi.MatrixItemCode, imi.IsSwatchItem FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryMatrixItem imi with (NOLOCK) ON imi.ItemCode = i.ItemCode INNER JOIN InventoryItem id with (NOLOCK) ON imi.MatrixItemCode = id.ItemCode WHERE i.Counter = {0} AND imi.IsSwatchItem = 1", entityCounter))
                    {
                        while (reader.Read())
                        {
                            counterOfItemsThatIsConfiguredForSwatch.Add(DB.RSFieldInt(reader, "Counter"));
                        }
                    }
                }

                // now let's see if we have a swatch image for the item
                foreach (int swatchItemCounter in counterOfItemsThatIsConfiguredForSwatch)
                {
                    EntityImageSwatch swatch = new EntityImageSwatch();
                    swatch.Image = FindImage(string.Format("{0}\\swatch\\{1}_{2}", entityFolder, entityCounter, swatchItemCounter), true, string.Empty);
                    swatch.Id = swatchItemCounter;
                    data.Swatches.Add(swatch);
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region GetEntityImageFileNameData
    public EntityImageData GetEntityImageFileNameData(List<string> IconFilename, List<string> MediumFilename, List<string> LargeFilename, List<string> MobileFilename, string entityFolder, bool entityIsItemAndTypeIsMatrixGroup, int entityCounter)
    {
        try
        {
            string productDirectory = GetProductDirectory();
            string mobileproductDirectory = GetMobileEntityDirectory(entityFolder);
            EntityImageData data = new EntityImageData();

            // get mobile images here
            for (int ctrmob = 0; ctrmob <= MobileFilename.Count - 1; ctrmob++)
            {
                if (CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, MobileFilename[ctrmob])))
                {
                    data.MobileImages.Add(
                            FindImageFileName(
                                string.Format("{0}\\mobile\\{1}", mobileproductDirectory, MobileFilename[ctrmob].Split('.')[0]),
                                true,
                                MobileFilename[ctrmob],
                                string.Empty
                            )
                        );

                    data.MobileImages[data.MobileImages.Count-1].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, MobileFilename[ctrmob]));
                    data.MobileImages[data.MobileImages.Count - 1].HasMobileImage = true;
                }
            }

            //get icon images here
            for (int ctri = 0; ctri <= IconFilename.Count - 1; ctri++)
            {
                data.IconImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\icon\\{1}", entityFolder, IconFilename[ctri].Split('.')[0]),
                        true,
                        IconFilename[ctri],
                        string.Empty
                    )
                );
                data.IconImages[ctri].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, IconFilename[ctri]));
                data.IconImages[ctri].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, IconFilename[ctri]));

            }
            //get medium images here
            for (int ctrm = 0; ctrm <= MediumFilename.Count - 1; ctrm++)
            {

                data.MediumImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\medium\\{1}", entityFolder, MediumFilename[ctrm].Split('.')[0]),
                        true,
                        MediumFilename[ctrm],
                        string.Format("{0}\\micro\\{1}", entityFolder, MediumFilename[ctrm].Split('.')[0])
                    )
                );

                data.MediumImages[ctrm].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, MediumFilename[ctrm]));
                data.MediumImages[ctrm].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, MediumFilename[ctrm]));

                //minicart image data
                if (data.MediumImages[ctrm].HasMinicartImage)
                {
                    data.MinicartImages.Add(FindImageFileName(
                            string.Format("{0}\\minicart\\{1}", entityFolder, MediumFilename[ctrm].Split('.')[0]),
                            true,
                            MediumFilename[ctrm],
                            string.Empty));
                    data.MinicartImages[data.MinicartImages.Count-1].HasMinicartImage = data.MediumImages[ctrm].HasMinicartImage;
                    data.MinicartImages[data.MinicartImages.Count - 1].HasMobileImage = data.MediumImages[ctrm].HasMobileImage;

                }

            }

            //get large images here
            for (int ctrl = 0; ctrl <= LargeFilename.Count - 1; ctrl++)
            {
                data.LargeImages.Add(
                    FindImageFileName(
                        string.Format("{0}\\large\\{1}", entityFolder, LargeFilename[ctrl].Split('.')[0]),
                        true,
                        LargeFilename[ctrl],
                        string.Empty
                    )
                );
                data.LargeImages[ctrl].HasMinicartImage = CommonLogic.FileExists(string.Format("{0}\\minicart\\{1}", entityFolder, LargeFilename[ctrl]));
                data.LargeImages[ctrl].HasMobileImage = CommonLogic.FileExists(string.Format("{0}\\mobile\\{1}", mobileproductDirectory, LargeFilename[ctrl]));

            }

            if (entityIsItemAndTypeIsMatrixGroup)
            {
                List<int> counterOfItemsThatIsConfiguredForSwatch = new List<int>();

                using (SqlConnection con = DB.NewSqlConnection())
                {
                    con.Open();
                    using (IDataReader reader = DB.GetRSFormat(con, "SELECT i.ItemType, i.Counter as MatrixCounter, i.ItemCode, id.Counter as Counter, imi.MatrixItemCode, imi.IsSwatchItem FROM InventoryItem i with (NOLOCK) INNER JOIN InventoryMatrixItem imi with (NOLOCK) ON imi.ItemCode = i.ItemCode INNER JOIN InventoryItem id with (NOLOCK) ON imi.MatrixItemCode = id.ItemCode WHERE i.Counter = {0} AND imi.IsSwatchItem = 1", entityCounter))
                    {
                        while (reader.Read())
                        {
                            counterOfItemsThatIsConfiguredForSwatch.Add(DB.RSFieldInt(reader, "Counter"));
                        }
                    }
                }

                // now let's see if we have a swatch image for the item
                foreach (int swatchItemCounter in counterOfItemsThatIsConfiguredForSwatch)
                {
                    EntityImageSwatch swatch = new EntityImageSwatch();
                    swatch.Image = FindImage(string.Format("{0}\\swatch\\{1}_{2}", entityFolder, entityCounter, swatchItemCounter), true, string.Empty);
                    swatch.Id = swatchItemCounter;
                    data.Swatches.Add(swatch);
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region FindImage
    private EntityImage FindImage(string filePathWithoutExtension, bool createBlankIfNotExisting, string microfilePathWithoutExtension)
    {
        string[] extensions = new string[] { "jpg", "gif", "png" };
        foreach (string ext in extensions)
        {
            string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, ext);
            string microPathWithExtension = string.Format("{0}.{1}", microfilePathWithoutExtension, ext);
            if (CommonLogic.FileExists(filePathWithExtension))
            {
                EntityImage imageData = new EntityImage();

                ImageFormat format = null;
                switch (ext)
                {
                    case "jpg":
                        format = ImageFormat.Jpeg;
                        imageData.ContentType = "image/jpeg";
                        break;
                    case "gif":
                        format = ImageFormat.Gif;
                        imageData.ContentType = "image/gif";
                        break;
                    case "png":
                        format = ImageFormat.Png;
                        imageData.ContentType = "image/x-png";
                        break;
                }

                using (MemoryStream strm = new MemoryStream())
                {
                    using (Image img = Image.FromFile(filePathWithExtension))
                    {
                        img.Save(strm, format);
                        strm.Flush();

                        imageData.ImageRaw = strm.ToArray();
                    }
                }
                //initialize micro images here
                if (microfilePathWithoutExtension.Length > 0)
                {
                    imageData.HasMicroImage = CommonLogic.FileExists(microPathWithExtension);
                }

                return imageData;
            }
        }

        // reached this point, image not found
        if (createBlankIfNotExisting)
        {
            return new EntityImage();
        }

        return null;
    }
    #endregion

    #region FindImageData
    private EntityImage FindImageData(string filePathWithoutExtension, bool createBlankIfNotExisting, string microfilePathWithoutExtension, string filenameWithoutExtension)
    {
        string[] extensions = new string[] { "jpg", "gif", "png" };
        foreach (string ext in extensions)
        {
            string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, ext);
            string microPathWithExtension = string.Format("{0}.{1}", microfilePathWithoutExtension, ext);
            if (CommonLogic.FileExists(filePathWithExtension))
            {
                EntityImage imageData = new EntityImage();

                ImageFormat format = null;
                switch (ext)
                {
                    case "jpg":
                        format = ImageFormat.Jpeg;
                        imageData.ContentType = "image/jpeg";
                        break;
                    case "gif":
                        format = ImageFormat.Gif;
                        imageData.ContentType = "image/gif";
                        break;
                    case "png":
                        format = ImageFormat.Png;
                        imageData.ContentType = "image/x-png";
                        break;
                }
                
                //no need to return actual image
                imageData.ImageRaw = null;

                //initialize micro images here
                if (microfilePathWithoutExtension.Length > 0)
                {
                    imageData.HasMicroImage = CommonLogic.FileExists(microPathWithExtension);
                }
                //get image filename
                imageData.ImageFileName = string.Format("{0}.{1}", filenameWithoutExtension, ext);
                return imageData;
            }
        }

        // reached this point, image not found
        if (createBlankIfNotExisting)
        {
            return new EntityImage();
        }
        return null;
    }
    #endregion

    #region FindImageFileName
    private EntityImage FindImageFileName(string filePathWithoutExtension, bool createBlankIfNotExisting, string fileName, string microfilePathWithoutExtension)
    {

        bool hasMicro = false;
        string microMIME = String.Empty;
        var splitConfig = AppLogic.SplitOtherConfig("MicroStyle");
        if (splitConfig.ContainsKey("mime")) microMIME = splitConfig["mime"].ToString();
        if (microMIME.Length == 0) microMIME = "jpg";
        if (microfilePathWithoutExtension.Length > 0 && !hasMicro) hasMicro = CommonLogic.FileExists(string.Format("{0}.{1}", microfilePathWithoutExtension, microMIME));

        string[] extensions = new string[] { "jpg", "gif", "png" };
        foreach (string ext in extensions)
        {
            string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, ext);
            string microPathWithExtension = string.Format("{0}.{1}", microfilePathWithoutExtension, ext);

            if (microfilePathWithoutExtension.Length > 0 && !hasMicro)
            { hasMicro = CommonLogic.FileExists(microPathWithExtension); }

            if (CommonLogic.FileExists(filePathWithExtension))
            {
                EntityImage imageData = new EntityImage();

                ImageFormat format = null;
                switch (ext)
                {
                    case "jpg":
                        format = ImageFormat.Jpeg;
                        imageData.ContentType = "image/jpeg";
                        break;
                    case "gif":
                        format = ImageFormat.Gif;
                        imageData.ContentType = "image/gif";
                        break;
                    case "png":
                        format = ImageFormat.Png;
                        imageData.ContentType = "image/x-png";
                        break;
                }

                using (MemoryStream strm = new MemoryStream())
                {
                    using (Image img = Image.FromFile(filePathWithExtension))
                    {
                        img.Save(strm, format);
                        strm.Flush();

                        imageData.ImageRaw = strm.ToArray();
                    }
                }

                //added for image filename and micro    
                imageData.ImageFileName = fileName;
                imageData.HasMicroImage = hasMicro;

                return imageData;
            }
        }

        // reached this point, image not found
        if (createBlankIfNotExisting)
        {
            return new EntityImage();
        }

        return null;
    }
    #endregion

    #region ResetCache
    [Flags]
    [Serializable]
    public enum ResetCacheOption
    {
        AppConfig = 0,
        StringResource = 1,
        ImagePath = 2,
        CategoryEntity = 4,
        ManufacturerEntity = 8,
        DepartmentEntity = 16,
        URLRedirect = 32,
        AttributeEntity = 64,
        All = AppConfig | StringResource | ImagePath | CategoryEntity | ManufacturerEntity | DepartmentEntity | URLRedirect | AttributeEntity
    }

    [WebMethod()]
    public void ResetCache(ResetCacheOption option)
    {
        if (option == ResetCacheOption.All)
        {
            AppLogic.m_RestartApp();
            ReloadSiteMap();

            //added because the Clear Web Cache was not clearing the current context cache
            System.Collections.IDictionaryEnumerator CacheEnum = HttpContext.Current.Cache.GetEnumerator();

            while (CacheEnum.MoveNext())
            {
                HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString());
            }
        }
        else
        {
            if ((option & ResetCacheOption.AppConfig) == ResetCacheOption.AppConfig)
            {
                AppLogic.ReloadAppConfigs();
            }

            if ((option & ResetCacheOption.StringResource) == ResetCacheOption.StringResource)
            {
                AppLogic.ReloadStringResources();
                ReloadSiteMap();
            }

            if ((option & ResetCacheOption.ImagePath) == ResetCacheOption.ImagePath)
            {
                AppLogic.ReloadImageFileNameCache();
            }

            if ((option & ResetCacheOption.CategoryEntity) == ResetCacheOption.CategoryEntity)
            {
                AppLogic.ReloadEntityHelper("Category");
                ReloadSiteMap();
            }

            if ((option & ResetCacheOption.ManufacturerEntity) == ResetCacheOption.ManufacturerEntity)
            {
                AppLogic.ReloadEntityHelper("Manufacturer");
                ReloadSiteMap();
            }

            if ((option & ResetCacheOption.DepartmentEntity) == ResetCacheOption.DepartmentEntity)
            {
                AppLogic.ReloadEntityHelper("Department");                
                ReloadSiteMap();
            }
            if ((option & ResetCacheOption.URLRedirect) == ResetCacheOption.URLRedirect)
            {
                RedirectLogic.InitializeRedirectURL();
            }
            if ((option & ResetCacheOption.AttributeEntity) == ResetCacheOption.AttributeEntity)
            {
                AppLogic.ReloadEntityHelper("Attribute");
                ReloadSiteMap();
            }
        }
    }

    private void ReloadSiteMap()
    {
        if (System.Web.SiteMap.Enabled)
        {
            InterpriseSuiteEcommerce.ISESiteMapProviderFactory.Instance.Reset();
            MenuManager.ResetCache();
        }
    }

    #endregion

    #region AppConfig
    [WebMethod()]
    public void SetAppConfig(int AppConfigID, String Name, String ConfigValue)
    {
        // just does in memory update:
        AppLogic.SetAppConfig(AppConfigID, Name, ConfigValue);
    }
    #endregion

    #region StringResource
    [WebMethod()]
    public void SetStringResource(int StringResourceID, String Name, String UseText, String LocaleSetting)
    {
        // just does in memory update:
        AppLogic.SetStringResource(StringResourceID, Name, UseText, LocaleSetting);
        ReloadSiteMap();
    }
    #endregion

    #region URLRedirect
    [WebMethod()]
    public void ResetURLRedirect()
    {
        RedirectLogic.InitializeRedirectURL();
    }
    #endregion

    #region GetUserResetPasswordTemplate
    [WebMethod()]
    public string GetUserResetPasswordTemplate(string contactCode, string emailAddress)
    {
        return InterpriseHelper.GetPasswordEmailTemplate(emailAddress, contactCode);
    }

    #endregion

    #region AssignDefaultImageToItem
    [WebMethod()]
    public void AssignDefaultImageToItem(int productId)
    {
        InterpriseSuiteEcommerceCommon.DTO.ProductImage.AssignDefaultImageToItem(productId);
    }
    #endregion

    //CBN Code Starts here

    #region GetCBNEntityImageFileNameData
    [WebMethod()]
    public List<CBNEntityImageData> GetCBNEntityImageFileNameData(List<CBNEntityImageData> listImageItem, ImageType imageType)
    {
        int ImageCount = listImageItem.Count;
        string entityFolder = GetEntityDirectory(imageType);
        var listCBNEntityImageData = new List<CBNEntityImageData>();

        for (int i = 0; i < ImageCount; i++)
        {
            CBNEntityImageData ImageData = new CBNEntityImageData();
            ImageData.ItemCode = Convert.ToString(listImageItem[i].ItemCode);
            ImageData.InventoryItemId = Convert.ToInt64(listImageItem[i].InventoryItemId);
            ImageData.ItemName = Convert.ToString(listImageItem[i].ItemName);

            // Large
            int LargeCount = listImageItem[i].LargeImages.Count;
            for (int l = 0; l < LargeCount; l++)
            {
                CBNEntityImage largeimage = new CBNEntityImage();

                largeimage.ImageIndex = Convert.ToInt32(listImageItem[i].LargeImages[l].ImageIndex);
                largeimage.ImageRaw = CBNFindImageFileName(string.Format("{0}\\large\\{1}", entityFolder, listImageItem[i].LargeImages[l].ImageFileName.Split('.')[0])).ImageRaw;
                largeimage.ImageFileName = Convert.ToString(listImageItem[i].LargeImages[l].ImageFileName);
                ImageData.LargeImages.Add(largeimage);
            }

            listCBNEntityImageData.Add(ImageData);
        }

        return listCBNEntityImageData;
    }
    #endregion

    #region CBNFindImageFileName
    private CBNEntityImage CBNFindImageFileName(string fullpath)
    {
        string[] extensions = new string[] { "jpg", "gif", "png" };
        foreach (string ext in extensions)
        {
            string filePathWithExtension = string.Format("{0}.{1}", fullpath, ext);
            if (CommonLogic.FileExists(filePathWithExtension))
            {
                CBNEntityImage imageData = new CBNEntityImage();
                ImageFormat format = null;
                switch (ext)
                {
                    case "jpg":
                        format = ImageFormat.Jpeg;
                        imageData.ContentType = "image/jpeg";
                        break;
                    case "gif":
                        format = ImageFormat.Gif;
                        imageData.ContentType = "image/gif";
                        break;
                    case "png":
                        format = ImageFormat.Png;
                        imageData.ContentType = "image/x-png";
                        break;
                }

                using (MemoryStream strm = new MemoryStream())
                {
                    using (Image img = Image.FromFile(filePathWithExtension))
                    {
                        img.Save(strm, format);
                        strm.Flush();

                        imageData.ImageRaw = strm.ToArray();
                    }
                }

                return imageData;
            }
        }

        return null;
    }
    #endregion

    #region UpdateCBNProductImage
    private void UpdateCBNProductImage(string filePathWithoutExtension, string fileNameWithoutPathAndExtension, string imageSizeType, ImageType entityType, CBNEntityImage image, bool isMultiImage)
    {
        try
        {

            string[] extensions = new string[] { "jpg", "gif", "png" };
            string correctExtension = string.Empty;
            string img_ContentType = string.Empty;
            ImageFormat format = null;

            switch (image.ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/pjpeg":
                    correctExtension = "jpg";
                    img_ContentType = "image/jpeg";
                    format = ImageFormat.Jpeg;
                    break;

                case "image/gif":
                    correctExtension = "gif";
                    img_ContentType = "image/gif";
                    format = ImageFormat.Gif;
                    break;

                case "image/x-png":
                case "image/png":
                    correctExtension = "png";
                    img_ContentType = "image/png";
                    format = ImageFormat.Png;
                    break;
            }

            // If the format isn't supported, don't write it...
            if (!CommonLogic.IsStringNullOrEmpty(correctExtension) || format != null)
            {
                //First delete the existing image.
                DeleteProductImages(filePathWithoutExtension);

                //Save the file with the current requested extension.
                string filePathWithExtension = string.Format("{0}.{1}", filePathWithoutExtension, correctExtension);

                using (MemoryStream strm = new MemoryStream(image.ImageRaw))
                {
                    using (Image img = Image.FromStream(strm))
                    {
                        string imgEntityType = CommonLogic.IIF(entityType.ToString() == ImageType.Item.ToString(), "Product", entityType.ToString());
                        imgEntityType = CommonLogic.IIF(entityType.ToString() == ImageType.Department.ToString(), "Department", imgEntityType.ToString());

                        AppLogic.ResizeEntityOrObject(imgEntityType, img, fileNameWithoutPathAndExtension, imageSizeType, img_ContentType, false);

                        if (AppLogic.AppConfigBool("UseImageResize"))
                        {
                            //NOTE: only large image is transmitted from CBN so we will force other sizes to be created including micro images
                            if ((imageSizeType == "medium") && AppLogic.AppConfigBool("MultiMakesMicros"))
                            {
                                AppLogic.MakeMicroPic(fileNameWithoutPathAndExtension, img);
                            }
                            //if (imageSizeType == "large")
                            //{
                            //    AppLogic.CreateOthersFromLarge(imgEntityType, img, fileNameWithoutPathAndExtension, img_ContentType, isMultiImage, false);
                            //}
                        }

                        strm.Flush();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region SaveCBNEntityImageFileName
    [WebMethod()]
    public void SaveCBNEntityImageFileName(List<CBNEntityImageData> listImageItem, ImageType imageType)
    {

        string entityDirectory = GetEntityDirectory(imageType);
        int imagecount = listImageItem.Count;
        if (!CommonLogic.IsStringNullOrEmpty(entityDirectory))
        {
            //item count
            for (int img = 0; img < imagecount; img++)
            {
                // large images
                for (int ctr = 0; ctr < listImageItem[img].LargeImages.Count; ctr++)
                {
                    int number = ctr + 1;
                    if (listImageItem[img].LargeImages[ctr] != null)
                    {
                        //large images
                        UpdateCBNProductImage(
                            string.Format("{0}\\large\\{1}", entityDirectory,
                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            string.Format("{0}", listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            "large", imageType, listImageItem[img].LargeImages[ctr], true);
                        //medium images
                        UpdateCBNProductImage(
                            string.Format("{0}\\medium\\{1}", entityDirectory,
                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            string.Format("{0}", listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            "medium", imageType, listImageItem[img].LargeImages[ctr], true);
                        //icon images
                        UpdateCBNProductImage(
                            string.Format("{0}\\icon\\{1}", entityDirectory,
                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            string.Format("{0}", listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].ToString()),
                            "icon", imageType, listImageItem[img].LargeImages[ctr], true);
                    }
                    else
                    {
                        //Delete the large images.
                        DeleteProductImages(string.Format("{0}\\large\\{1}", entityDirectory,
                                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].
                                                              ToString(), number));
                        //Delete the medium images.
                        DeleteProductImages(string.Format("{0}\\medium\\{1}", entityDirectory,
                                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].
                                                              ToString(), number));
                        //Delete the icon images.
                        DeleteProductImages(string.Format("{0}\\icon\\{1}", entityDirectory,
                                                          listImageItem[img].LargeImages[ctr].ImageFileName.Split('.')[0].
                                                              ToString(), number));
                    }
                }
            }

        }


    }
    #endregion

    #region Compress
    public static byte[] Compress(byte[] bytData)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            Stream s = new GZipStream(ms, CompressionMode.Compress);
            s.Write(bytData, 0, bytData.Length);
            s.Close();
            byte[] compressedData = (byte[])ms.ToArray();
            return compressedData;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Decompress

    public CBNEntityImage Decompress(CBNEntityImage image)
    {
        //decompression code

        using (var stream = new GZipStream(new MemoryStream(image.ImageRaw), CompressionMode.Decompress))
        {
            const int size = 4096; var buffer = new byte[size];
            using (var memory = new MemoryStream())
            {
                int count = 0;
                do
                {
                    count = stream.Read(buffer, 0, size); if (count > 0) { memory.Write(buffer, 0, count); }
                } while (count > 0);

                image.ImageRaw = memory.ToArray();

            }
        }
        return image;
    }

    #endregion

    //CBN Code Ends here

}




