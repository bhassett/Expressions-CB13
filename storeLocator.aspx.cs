using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon.StoreLocator;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{
    public partial class storeLocator : SkinBase
    {
        protected override void OnInit(EventArgs e)
        {
            if (!IsPostBack)
            {
                SectionTitle = AppLogic.GetString("menu.StoreLocator", SkinID, ThisCustomer.LocaleSetting, true);

                var locatorSettings = new GooleMapLocatorSetting(AppLogic.AppConfig("Storelocator.GoogleAPIKey"), false, "3.8");
                ctrlStoreLocator.LoadLocatorSetting(locatorSettings);

                //Force the databind to be executed
                DataBind();
            }

            ctrlStoreLocator.ThisCustomer = ThisCustomer;
            base.OnInit(e);
        }
    }
}


