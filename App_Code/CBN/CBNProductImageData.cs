using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Summary description for CBNProductImageData
/// </summary>
[Serializable()]
public class CBNEntityImageData
{
    /// <summary>
    /// Gets or sets the id for the image, use the Counter column for this
    /// </summary>
    public string ItemCode;

    public string ItemName;
    public long InventoryItemId;
    public int ImageIndex;
    public bool IsDefaultIcon;
    public bool IsDefaultMedium;
    public bool HasIcon;
    public bool HasMicro;
    public bool HasMedium;
    public bool HasLarge;

    /// <summary>
    /// Gets or sets the Icon image for the item
    /// </summary>
    public EntityImage Icon;

    /// <summary>
    /// Gets or sets the Medium image for this item
    /// </summary>
    public EntityImage Medium;

    /// <summary>
    /// Gets or sets the Large image for this item
    /// </summary>
    public EntityImage Large;

    /// <summary>
    /// Gets the Icon images for this item
    /// </summary>    
    public List<CBNEntityImage> IconImages = new List<CBNEntityImage>();

    /// <summary>
    /// Gets the Micro images for this item
    /// </summary>    
    public List<CBNEntityImage> MicroImages = new List<CBNEntityImage>();

    /// <summary>
    /// Gets the Medium images for this item
    /// </summary>    
    public List<CBNEntityImage> MediumImages = new List<CBNEntityImage>();

    /// <summary>
    /// Gets the large images for this item
    /// </summary>
    public List<CBNEntityImage> LargeImages = new List<CBNEntityImage>();
}