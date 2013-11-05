// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

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
/// Summary description for ProductImageData
/// </summary>
[Serializable()]
public class EntityImageData
{
    /// <summary>
    /// Gets or sets the id for the image, use the Counter column for this
    /// </summary>
    public int Id;

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
    /// Gets or sets the minicart image for this item
    /// </summary>
    public EntityImage MiniCart;

    /// <summary>
    /// Gets or sets the mobile image for this item
    /// </summary>
    public EntityImage Mobile;

    /// <summary>
    /// Gets the MiniCart images for this item
    /// </summary>    
    public List<EntityImage> IconImages = new List<EntityImage>();

    /// <summary>
    /// Gets the Medium images for this item
    /// </summary>    
    public List<EntityImage> MediumImages = new List<EntityImage>();

    /// <summary>
    /// Gets the large images for this item
    /// </summary>
    public List<EntityImage> LargeImages = new List<EntityImage>();

    /// <summary>
    /// Gets the swatch images for this item
    /// </summary>
    public List<EntityImageSwatch> Swatches = new List<EntityImageSwatch>();

    /// <summary>
    /// Gets the mobile images for this item
    /// </summary>
    public List<EntityImage> MobileImages = new List<EntityImage>();

    /// <summary>
    /// Gets the minicart images for this item
    /// </summary>
    public List<EntityImage> MinicartImages = new List<EntityImage>();
}