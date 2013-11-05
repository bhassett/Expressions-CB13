using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CBNProductImage
/// </summary>
[Serializable()]
public class CBNEntityImage
{
    public byte[] ImageRaw = new byte[] { };
    public string ContentType;
    public string ImageFileName;
    public bool HasIconImage;
    public bool HasMicroImage;
    public bool HasMediumImage;
    public bool HasLargeImage;
    public bool IsDefaultIcon;
    public bool IsDefaultMedium;
    public int ImageIndex;
}