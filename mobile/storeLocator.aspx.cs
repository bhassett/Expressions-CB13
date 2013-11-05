using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    public partial class storeLocator : SkinBase
    {
        protected Array strLocationArray;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            SectionTitle = AppLogic.GetString("storelocator.aspx.2", SkinID, ThisCustomer.LocaleSetting);

            if (!Page.IsPostBack)
            {
                AddStoreLocations();
            }
        }

        /// <summary>
        /// Get the data for the currently active warehouses and display the info on the page.
        /// </summary>
        private void AddStoreLocations()
        {
            //get all the latitudes and longitudes for the active warehouses
            using (Interprise.Framework.SystemManager.DatasetGateway.PostalCodeDatasetGateway zipCodeGateWay = new Interprise.Framework.SystemManager.DatasetGateway.PostalCodeDatasetGateway())
            {
                using (Interprise.Facade.SystemManager.PostalCodeFacade zipCodeFacade = new Interprise.Facade.SystemManager.PostalCodeFacade(zipCodeGateWay))
                {
                    string[][] strZipCodeCommandSet = new string[][] { new string[] { "SystemPostalCode", "eCommerceReadSystemWarehousePostalCode" } };

                    zipCodeFacade.LoadDataSet(strZipCodeCommandSet, Interprise.Framework.Base.Shared.Enum.ClearType.Specific, Interprise.Framework.Base.Shared.Enum.ConnectionStringType.Online);

                    //if we have some warehouses with zip codes, try and get the address to them
                    if (zipCodeGateWay.SystemPostalCode.Rows.Count > 0)
                    {
                        using (Interprise.Framework.Inventory.DatasetGateway.WarehouseDatasetGateway warehouseGateway = new Interprise.Framework.Inventory.DatasetGateway.WarehouseDatasetGateway())
                        {
                            using (Interprise.Facade.Inventory.WarehouseFacade warehouseFacade = new Interprise.Facade.Inventory.WarehouseFacade(warehouseGateway))
                            {
                                string[][] strWarehouseCommandSet = new string[][] { new string[] { "InventoryWarehouse", "ReadInventoryWarehouse" } };

                                warehouseFacade.LoadDataSet(strWarehouseCommandSet, Interprise.Framework.Base.Shared.Enum.ClearType.Specific, Interprise.Framework.Base.Shared.Enum.ConnectionStringType.Online);

                                AddPushPins(zipCodeGateWay.SystemPostalCode, warehouseGateway.InventoryWarehouse);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a JavaScript function to add to the page that calls a method for each warehouse location.
        /// </summary>
        /// <param name="postalCodes">The postal codes for the active warehouses.</param>
        /// <param name="warehouses">The warehouses on file. Passed in for address information.</param>
        private void AddPushPins(Interprise.Framework.SystemManager.DatasetComponent.PostalCodeDataset.SystemPostalCodeDataTable postalCodes, Interprise.Framework.Inventory.DatasetComponent.WarehouseDataset.InventoryWarehouseDataTable warehouses)
        {
            StringBuilder sbAddPins = new StringBuilder(256);
            bool blnSetFirstValidAddress = true;

            sbAddPins.Append("function AddPins(){");

            DataRow[] addressDataRows;

            //call the function to add the pins for each warehouse
            foreach (DataRow row in postalCodes.Rows)
            {
                //only add the addresses if it/they are an exact match with a zip code
                addressDataRows = warehouses.Select(string.Format("PostalCode = '{0}' AND Country = '{1}' AND City = '{2}' AND County = '{3}'", row["PostalCode"].ToString(), row["CountryCode"].ToString(), row["City"].ToString(), row["County"].ToString()));

                if (addressDataRows.Length >= 1) //found an exact match - send formatted address info
                {
                    foreach (DataRow validDataRow in addressDataRows)
                    {
                        //format the address removing bad characters and adding commas and spaces
                        string strFormattedAddress = string.Concat(validDataRow["Address"].ToString().Replace("\r\n", "<br />"), "<br />", validDataRow["City"].ToString(), ", ", validDataRow["State"].ToString(), "  ", validDataRow["PostalCode"].ToString());

                        //// added the Find method of Virtual Earth Interactive SDK reference url: http://dev.live.com/Virtualearth/sdk/
                        sbAddPins.Append("\n");
                        sbAddPins.Append("\n   var what = null;");
                        sbAddPins.Append(string.Concat("\n   var where = \"", strFormattedAddress.Replace("<br />", ""), "\";"));
                        sbAddPins.Append("\n   var startIndex = 0;");
                        sbAddPins.Append("\n   var numberOfResults = 1;");
                        sbAddPins.Append("\n   var showResult = true;");
                        sbAddPins.Append("\n   var createResult = true;");
                        sbAddPins.Append("\n   var disambiguation = true;");
                        sbAddPins.Append("\n   try");
                        sbAddPins.Append("\n   {");
                        sbAddPins.Append("\n         map.Find(what, where, null, null, startIndex, numberOfResults, showResult, createResult, disambiguation, " + blnSetFirstValidAddress.ToString().ToLower() + ", callback);");
                        sbAddPins.Append("\n   }");
                        sbAddPins.Append("\n   catch(e)");
                        sbAddPins.Append("\n   {");
                        sbAddPins.Append("\n      alert(e.message);");
                        sbAddPins.Append("\n   }");
                        sbAddPins.Append("\n");

                        if (blnSetFirstValidAddress)
                        {
                            blnSetFirstValidAddress = false;
                        }
                    }
                }
                else //couldn't find an exact match - send default stuff
                {
                    sbAddPins.Append(string.Format("AddPin(new VELatLong({0}, {1}), '{2}');", row["Latitude"].ToString(), row["Longitude"].ToString(), row["City"].ToString()));
                }
            }

            sbAddPins.Append("}");

            sbAddPins.Append("\n");
            sbAddPins.Append("\n    function callback(shapeLayer, findResult, place, blnMoreResults, strErrorMsg){");
            sbAddPins.Append("\n        if (place != null && place.length > 0){");
            sbAddPins.Append("\n            if(place.length > 1){");
            sbAddPins.Append("\n                var results='More than one location was retruned. Please select the location you were looking for:<br>';");
            sbAddPins.Append("\n                for (x=0; x < place.length; x++)");
            sbAddPins.Append("\n                {");
            sbAddPins.Append(string.Concat("\n         results += \"<a href='javascript:map.Find(null, ", @"\", "\"\" + place[x].Name + \"",
                                                    @"\", "\");'>\" + place[x].Name + \"</a><br>\""));
            sbAddPins.Append("\n                }");
            sbAddPins.Append("\n                document.getElementById('storeLocator').innerHTML = results;");
            sbAddPins.Append("\n            }");
            sbAddPins.Append("\n            else{");
            sbAddPins.Append("\n                AddPin(place[0].LatLong, place[0].Name);");
            sbAddPins.Append("\n            }");
            sbAddPins.Append("\n        }");
            sbAddPins.Append("\n    }");

            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "AddingPins", sbAddPins.ToString(), true);
        }
    }
}


