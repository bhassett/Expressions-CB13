// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
	/// <summary>
	/// Summary description for Version.
	/// </summary>
	public partial class Version : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
            string vesrsiontxt = string.Empty;
            string productname = string.Empty;

            String assemblyfilePath = CommonLogic.SafeMapPath("App_Code/GlobalAssemblyInfo.cs");
            if (CommonLogic.FileExists(assemblyfilePath))
            {
                string[] attrlines = System.IO.File.ReadAllLines(assemblyfilePath);
                //[assembly: AssemblyFileVersionAttribute("6.0.7.26")]
                if (attrlines != null)
                {
                    foreach (string line in attrlines)
	                {
		                if (line.Contains("[assembly: AssemblyFileVersionAttribute("))
                        {
                            vesrsiontxt = line.Replace("[assembly: AssemblyFileVersionAttribute(", string.Empty);
                            vesrsiontxt = vesrsiontxt.Replace(")]", string.Empty);
                            vesrsiontxt = vesrsiontxt.Replace("\"",string.Empty);
                            if (vesrsiontxt.Length == 0) { vesrsiontxt = CommonLogic.GetVersion(); }
                            break;
                        }
                        //[assembly: AssemblyProduct("Interprise Suite")]
                        else if (line.Contains("[assembly: AssemblyProduct("))
                        {
                            productname = line.Replace("[assembly: AssemblyProduct(", string.Empty);
                            productname = productname.Replace(")]", string.Empty);
                            productname = productname.Replace("\"", string.Empty);
                        }

	                }

                    if (productname.Length > 0 && vesrsiontxt.Length > 0) { vesrsiontxt = string.Concat(" ", productname, " ", vesrsiontxt); }
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

            // serial and confirmation code
            try
            { 
                string serial = String.Empty;
                string confirmation = String.Empty;
                               
                InterpriseHelper.GetMaskedClientSerailAndConfirmationCode(ref serial, ref confirmation);

                if (!String.IsNullOrEmpty(serial))
                {
                    lblSerial.Text = "Serial Number: " + serial;
                }
                else
                {
                   lblSerial.Text = "Serial Number: Not Found!";
                }

                if (!String.IsNullOrEmpty(confirmation))
                {
                    lblConfirm.Text = "Confirmation Code: " + confirmation;
                }
                else
                {
                    lblConfirm.Text = "Confirmation Code: Not Found!";
                }
                                
            }
            catch (Exception ex)
            {
                lblDB.Text += ex.Message;
            }

		}
	}
}
