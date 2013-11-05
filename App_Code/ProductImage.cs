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
/// Summary description for ProductImage
/// </summary>
[Serializable()]
public class EntityImage
{
    public State State;
    public string ContentType;
    public byte[] ImageRaw = new byte[] { };
    public string ImageFileName;
    public bool HasMicroImage;
    public bool HasMinicartImage;
    public bool HasMobileImage;
}

public enum State
{
    UnChanged = 0,
    Added = 1,
    Modified = 2,
    Deleted = 4
}

public enum ImageType
{
    Item = 0,
    Category = 1,
    Manufacturer = 2,
    Department = 4,
    Attribute = 8
}