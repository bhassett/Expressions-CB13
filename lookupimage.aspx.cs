// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using InterpriseSuiteEcommerceCommon;

public partial class lookupimage : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        Response.CacheControl = "private";
        Response.Expires = 0;
        Response.AddHeader("pragma", "no-cache");

        string entity = CommonLogic.QueryStringCanBeDangerousContent("entity");
        string size = CommonLogic.QueryStringCanBeDangerousContent("size");
        int index = CommonLogic.QueryStringUSInt("index");
        string filename = CommonLogic.QueryStringCanBeDangerousContent("filename");
        string itemcode = CommonLogic.QueryStringCanBeDangerousContent("itemcode");
        int id = CommonLogic.QueryStringUSInt("id");

        bool useWaterMark = AppLogic.AppConfigBool("Watermark.Enabled");
        bool imageExists = false;

        string url = String.Empty;

        if (entity.Equals("product", StringComparison.InvariantCultureIgnoreCase))
        {
            url = AppLogic.LocateImageFilenameUrl(entity, itemcode, size, filename, useWaterMark, out  imageExists);
        }
        else
        {
            url = AppLogic.LocateImageUrl(entity, id, size, index, useWaterMark, out imageExists);
        }

        if (useWaterMark)
        {
            if (imageExists)
            {
                Response.Redirect(url);
                return;
            }
        }

        Response.StatusCode = CommonLogic.IIF(imageExists, 200, 404);

        string path = CommonLogic.SafeMapPath(url);
        using (var img = Bitmap.FromFile(path))
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            switch (extension)
            {
                case ".jpg":
                    Response.ContentType = "image/jpeg";
                    var encoderParameters = new EncoderParameters();
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    img.Save(Response.OutputStream, ImageCodecInfo.GetImageEncoders()[1], encoderParameters);
                    break;

                case ".gif":
                    Response.ContentType = "image/gif";
                    img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;

                case ".png":
                    Response.ContentType = "image/png";
                    img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
            }
        }

    }
}
