// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Web;
using InterpriseSuiteEcommerceCommon;
using System.IO;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for Version.
    /// </summary>
    public partial class Version : System.Web.UI.Page
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string vesrsiontxt = String.Empty;
            string productname = String.Empty;
            string assemblyfilePath = HttpContext.Current.Request.PhysicalApplicationPath + "App_Code\\GlobalAssemblyInfo.cs";

            if (File.Exists(assemblyfilePath))
            {
                string[] attrlines = File.ReadAllLines(assemblyfilePath);
                //[assembly: AssemblyFileVersionAttribute("6.0.7.26")]
                if (attrlines != null)
                {
                    foreach (string line in attrlines)
                    {
                        if (line.Contains("[assembly: AssemblyFileVersionAttribute("))
                        {
                            vesrsiontxt = line.Replace("[assembly: AssemblyFileVersionAttribute(", String.Empty);
                            vesrsiontxt = vesrsiontxt.Replace(")]", String.Empty);
                            vesrsiontxt = vesrsiontxt.Replace("\"", String.Empty);
                            if (vesrsiontxt.Length == 0) { vesrsiontxt = CommonLogic.GetVersion(); }
                            break;
                        }
                        //[assembly: AssemblyProduct("Interprise Suite")]
                        else if (line.Contains("[assembly: AssemblyProduct("))
                        {
                            productname = line.Replace("[assembly: AssemblyProduct(", String.Empty);
                            productname = productname.Replace(")]", String.Empty);
                            productname = productname.Replace("\"", String.Empty);
                        }

                    }

                    if (productname.Length > 0 && vesrsiontxt.Length > 0) { vesrsiontxt = String.Concat(" ", productname, " ", vesrsiontxt); }
                }
            }
            else
            {
                vesrsiontxt = CommonLogic.GetVersion();
            }

            lblVersion.Text = vesrsiontxt;
            lblDB.Text = "DB Version: ";
            //Trap error so other data are still readable.
            try
            {
                lblDB.Text += InterpriseHelper.GetISdbVersion();
                lblStoreCode.Text = "Web Store Code: " + InterpriseHelper.ConfigInstance.WebSiteCode;
            }
            catch (Exception ex)
            {
                lblDB.Text += ex.Message;
            }
        }
    }
}
