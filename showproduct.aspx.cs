// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;
using System.Text;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for showproduct.
    /// </summary>
    public partial class showproduct : SkinBase
    {
        // NOTE :
        //  Since we have an issue on item code not being safe passing at the query string
        //  ProductID would be the Counter
        int _itemCounter;
        bool IsAKit;
        bool IsMatrix;
        bool RequiresReg;
        string ProductName;
        private string m_XmlPackage;

        string CategoryName;
        string SectionName;
        string ManufacturerName;
        string CategoryID;
        string DepartmentID;
        string ManufacturerID;
        EntityHelper CategoryHelper;
        EntityHelper SectionHelper;
        EntityHelper ManufacturerHelper;

        string SourceEntity = "Category";
        string SourceEntityID = string.Empty;
        string _itemCode = string.Empty;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                GoNonSecureAgain();
            }

            _itemCounter = "ProductID".ToQueryString().TryParseIntUsLocalization().Value;
            CategoryID = "CategoryID".ToQueryString();
            DepartmentID = "DepartmentID".ToQueryString();
            ManufacturerID = "ManufacturerID".ToQueryString();

            _itemCode = AppLogic.GetItemCodeByCounter(_itemCounter);

            if (AppLogic.AppConfigBool("CustomersWhoViewedThisItemAlsoViewed.Enabled"))
            {
                ProcessViewProductWhoViewed();
            }

            var eCommerceProductInfoView = AppLogic.GetProductInfoViewForShowProduct(DB.SQuote(_itemCode),
                                                                       DB.SQuote(ThisCustomer.LocaleSetting),
                                                                       DB.SQuote(InterpriseHelper.ConfigInstance.UserCode),
                                                                       DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode),
                                                                       DB.SQuote(Localization.DateTimeStringForDB(DateTime.Now)),
                                                                       DB.SQuote(ThisCustomer.ProductFilterID),
                                                                       DB.SQuote(ThisCustomer.ContactCode));

            string itemDescription = string.Empty;

            if (eCommerceProductInfoView == null) { Response.Redirect(SE.MakeDriverLink("ProductNotFound")); }

            if (eCommerceProductInfoView.CheckOutOption) { Response.Redirect(SE.MakeDriverLink("ProductNotFound")); }

            if (!eCommerceProductInfoView.IsPublished) { Response.Redirect(SE.MakeDriverLink("ProductNotFound")); }

            if(eCommerceProductInfoView.IsCBN == false && AppLogic.IsCBNMode()) { Response.Redirect(SE.MakeDriverLink("ProductNotFound")); }

            string SENameINURL = "SEName".ToQueryStringDecode();
            string ActualSEName = eCommerceProductInfoView.ItemDescription.ToMungeName().ToUrlEncode().ToSubString(90);
            if (string.IsNullOrEmpty(ActualSEName))
            {
                ActualSEName = eCommerceProductInfoView.ItemName.ToMungeName().ToUrlEncode().ToSubString(90);
            }

            if (ActualSEName != SENameINURL)
            {
                string NewURL = AppLogic.GetStoreHTTPLocation(false) + SE.MakeProductLink(_itemCounter.ToString(), ActualSEName);
                string QStr = "?";
                var keyvalues = Request.QueryString
                                   .ToPairs()
                                   .Where(q => q.Key == "productid" && q.Key == "sename")
                                   .Select(q => string.Join("=", new[] { q.Key, q.Value + "&" }))
                                   .ToArray();
                QStr += string.Join("", keyvalues);
                if (QStr.Length > 1) { NewURL += QStr; }

                HttpContext.Current.Response.Write("<html><head><title>Object Moved</title></head><body><b>Object moved to <a href=\"" + NewURL + "\">HERE</a></b></body></html>");
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", NewURL);
                HttpContext.Current.Response.End();
            }

            m_XmlPackage = eCommerceProductInfoView.XmlPackage.ToLowerInvariant();

            IsAKit = eCommerceProductInfoView.IsAKit.TryParseBool();
            IsMatrix = eCommerceProductInfoView.IsMatrix.TryParseBool();

            if (m_XmlPackage.Length == 0)
            {
                if (IsAKit)
                {
                    m_XmlPackage = AppLogic.ro_DefaultProductKitXmlPackage; // provide a default
                }
                else if (IsMatrix)
                {
                    m_XmlPackage = AppLogic.ro_DefaultProductPackXmlPackage; // provide a default
                }
                else
                {
                    m_XmlPackage = AppLogic.ro_DefaultProductXmlPackage; // provide a default
                }
            }

            RequiresReg = eCommerceProductInfoView.RequiresRegistration;
            ProductName = XmlCommon.GetLocaleEntry(eCommerceProductInfoView.ItemDescription, ThisCustomer.LocaleSetting, true);

            CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, DomainConstants.LOOKUP_HELPER_CATEGORIES);
            SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, DomainConstants.LOOKUP_HELPER_DEPARTMENT);
            ManufacturerHelper = AppLogic.LookupHelper(base.EntityHelpers, DomainConstants.LOOKUP_HELPER_MANUFACTURERS);

            itemDescription = eCommerceProductInfoView.ItemDescription;
            if (string.IsNullOrEmpty(itemDescription))
            {
                itemDescription = ProductName;
            }

            string seITitleTemp = XmlCommon.GetLocaleEntry(eCommerceProductInfoView.SETitle, ThisCustomer.LocaleSetting, true);
            SETitle = string.IsNullOrEmpty(seITitleTemp) ? (AppLogic.AppConfig("StoreName") + " - " + itemDescription).ToHtmlEncode() : seITitleTemp;

            string seDescription = XmlCommon.GetLocaleEntry(eCommerceProductInfoView.SEDescription, ThisCustomer.LocaleSetting, true);
            SEDescription = string.IsNullOrEmpty(seDescription) ? ProductName.ToHtmlEncode() : seDescription;

            string seKeywords = XmlCommon.GetLocaleEntry(eCommerceProductInfoView.SEKeywords, ThisCustomer.LocaleSetting, true);
            SEKeywords = string.IsNullOrEmpty(seKeywords) ? ProductName.ToHtmlEncode() : seKeywords;

            SENoScript = XmlCommon.GetLocaleEntry(eCommerceProductInfoView.SENoScript, ThisCustomer.LocaleSetting, true);

            CategoryName = (CategoryHelper.GetEntityField(CategoryID, "Description", ThisCustomer.LocaleSetting) != String.Empty) ?
                                CategoryHelper.GetEntityField(CategoryID, "Description", ThisCustomer.LocaleSetting) :
                                CategoryHelper.GetEntityName(CategoryID, ThisCustomer.LocaleSetting);

            SectionName = (SectionHelper.GetEntityField(DepartmentID, "Description", ThisCustomer.LocaleSetting) != String.Empty) ?
                                SectionHelper.GetEntityField(DepartmentID, "Description", ThisCustomer.LocaleSetting) :
                                SectionHelper.GetEntityName(DepartmentID, ThisCustomer.LocaleSetting);

            ManufacturerName = (ManufacturerHelper.GetEntityField(ManufacturerID, "Description", ThisCustomer.LocaleSetting) != String.Empty) ?
                                ManufacturerHelper.GetEntityField(ManufacturerID, "Description", ThisCustomer.LocaleSetting) :
                                ManufacturerHelper.GetEntityName(ManufacturerID, ThisCustomer.LocaleSetting);

            if (ManufacturerID.Length != 0)
            {
                CookieTool.Add("LastViewedEntityName", "Manufacturer", new TimeSpan(1, 0, 0, 0, 0));
                CookieTool.Add("LastViewedEntityInstanceID", ManufacturerID.ToString(), new TimeSpan(1, 0, 0, 0, 0));
                CookieTool.Add("LastViewedEntityInstanceName", ManufacturerName, new TimeSpan(1, 0, 0, 0, 0));
                String NewURL = AppLogic.GetStoreHTTPLocation(false) + SE.MakeProductLink(_itemCounter.ToString(), _itemCode);
                HttpContext.Current.Response.Write("<html><head><title>Object Moved</title></head><body><b>Object moved to <a href=\"" + NewURL + "\">HERE</a></b></body></html>");
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", NewURL);
                HttpContext.Current.Response.End();
            }
            else if (CategoryID.Length != 0)
            {
                CookieTool.Add("LastViewedEntityName", "Category", new TimeSpan(1, 0, 0, 0, 0));
                CookieTool.Add("LastViewedEntityInstanceID", CategoryID.ToString(), new TimeSpan(1, 0, 0, 0, 0));
                CookieTool.Add("LastViewedEntityInstanceName", CategoryName, new TimeSpan(1, 0, 0, 0, 0));
                string NewURL = AppLogic.GetStoreHTTPLocation(false) + SE.MakeProductLink(_itemCounter.ToString(), _itemCode);
                HttpContext.Current.Response.Write("<html><head><title>Object Moved</title></head><body><b>Object moved to <a href=\"" + NewURL + "\">HERE</a></b></body></html>");
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", NewURL);
                HttpContext.Current.Response.End();
            }
            else if (DepartmentID.Length != 0)
            {
                var cookierExpires = new TimeSpan(1, 0, 0, 0, 0);
                CookieTool.Add("LastViewedEntityName", "Department", cookierExpires);
                CookieTool.Add("LastViewedEntityInstanceID", DepartmentID.ToString(), cookierExpires);
                CookieTool.Add("LastViewedEntityInstanceName", SectionName, cookierExpires);

                String NewURL = AppLogic.GetStoreHTTPLocation(false) + SE.MakeProductLink(_itemCounter.ToString(), _itemCode);
                HttpContext.Current.Response.Write("<html><head><title>Object Moved</title></head><body><b>Object moved to <a href=\"" + NewURL + "\">HERE</a></b></body></html>");
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", NewURL);
                HttpContext.Current.Response.End();
            }

            SourceEntity = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityName", true);
            string SourceEntityInstanceName = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityInstanceName", true);
            SourceEntityID = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityInstanceID", true);

            // validate that source entity id is actually valid for this product:
            if (SourceEntityID.Length != 0)
            {
                var alE = AppLogic.GetProductEntityList(_itemCode, SourceEntity);
                if (!alE.Any(i => i == SourceEntityID.TryParseIntUsLocalization()))
                {
                    SourceEntityID = string.Empty;
                }
            }

            if (SourceEntityID.Length != 0)
            {
                PickupBreadCrumb(ref SourceEntity, ref SourceEntityInstanceName, ref SourceEntityID, false);
            }
            else
            {
                PickupBreadCrumb(ref SourceEntity, ref SourceEntityInstanceName, ref SourceEntityID, true);
            }

            AppLogic.LogEvent(ThisCustomer.CustomerCode, 10, _itemCounter.ToString());

            
        }

        private void ProcessViewProductWhoViewed()
        {
            int sessionLifetime = AppLogic.AppConfigUSInt("ViewedProductsSessionLifetime");
            if (sessionLifetime == null)
            {
                sessionLifetime = 1;
            }

            if (!ThisCustomer.HasCustomerRecord)
            {
                ThisCustomer.RequireCustomerRecord();
            }

            DateTime expirationdatetime = DateTime.Now.AddDays(-sessionLifetime);
            string updatevieweditems =
                    string.Format("exec UpdateRecordsEcommerceViewedItems @ExpirationDate = {0}, @WebSiteCode = {1}, @ContactCode = {2}, @ItemCode = {3}, @CurrentDate = {4}, @SessionID = {5}",
                    DB.SQuote(Localization.DateTimeStringForDB(expirationdatetime)),
                    DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode),
                    DB.SQuote(ThisCustomer.ContactCode),
                    DB.SQuote(_itemCode),
                    DB.SQuote(Localization.DateTimeStringForDB(DateTime.Now)),
                    ThisCustomer.CurrentSessionID);
            DB.ExecuteSQL(updatevieweditems);
        }

        /// <summary>
        /// If the cookies failed to supply required ProductEntity data for BreadCrumb picking, then let's get it from the database the hard way
        /// Important Notes: Manufacturer need not to be lookuped since it doesn't have hierarchy.
        ///                  AppLogic.GetFirstProductEntityID cannot be used here since it is not guaranteed to return the Primary ProductEntity!
        /// </summary>
        /// <param name="SourceEntity"></param>
        /// <param name="SourceEntityInstanceName"></param>
        /// <param name="SourceEntityID"></param>
        /// <param name="emptySource"></param>
        private void PickupBreadCrumb(ref string SourceEntity, ref string SourceEntityInstanceName, ref string SourceEntityID, bool emptySource)
        {
            if (CommonLogic.IsStringNullOrEmpty(SourceEntity))
            {
                SourceEntity = "Category";
            }

            EntityHelper hlp = AppLogic.LookupHelper(SourceEntity);

            if (emptySource)
            {
                SourceEntityID = EntityHelper.GetProductsFirstEntity(_itemCode, SourceEntity).ToString();
                if (!CommonLogic.IsStringNullOrEmpty(SourceEntityID) & SourceEntityID != "0")
                {
                    emptySource = false;
                    SourceEntityInstanceName = hlp.GetEntityName(SourceEntityID, ThisCustomer.LocaleSetting);
                }

                if (emptySource)
                {
                    SourceEntity = "Department";
                    SourceEntityID = EntityHelper.GetProductsFirstEntity(_itemCode, SourceEntity).ToString();
                    if (!CommonLogic.IsStringNullOrEmpty(SourceEntityID) & SourceEntityID != "0")
                    {
                        emptySource = false;
                        hlp = AppLogic.LookupHelper(SourceEntity);
                        SourceEntityInstanceName = hlp.GetEntityName(SourceEntityID, ThisCustomer.LocaleSetting);
                    }
                    SectionTitle += this.ProductName;
                    return;
                }
            }

            //Build the start of the breadcrumb trail.
            string ParentID = hlp.GetParentEntity(SourceEntityID);
            if (ParentID.Length != 0)
            {
                while (ParentID.Length != 0)
                {
                    SectionTitle = "<a class=\"SectionTitleText\" href=\"" + SE.MakeEntityLink(SourceEntity, ParentID, "") + "\">" + hlp.GetEntityField(ParentID, "Description", ThisCustomer.LocaleSetting) + "</a> &rarr; " + SectionTitle;
                    ParentID = hlp.GetParentEntity(ParentID);
                }
            }

            //Get the description of the entity for display.
            string entityDisplayName = hlp.GetEntityField(SourceEntityID, "Description", ThisCustomer.LocaleSetting);

            //If the description was null or empty the use the entity instance name.
            if (string.IsNullOrEmpty(entityDisplayName))
            {
                entityDisplayName = SourceEntityInstanceName;
            }

            //See if the source entity instance name is set.
            if (!string.IsNullOrEmpty(SourceEntityInstanceName))
            {
                //Append the link to the entity and the product name to the end.
                SectionTitle += "<a class=\"SectionTitleText\" href=\"" + SE.MakeEntityLink(SourceEntity, SourceEntityID, SourceEntityInstanceName) + "\">" + entityDisplayName + "</a> &rarr; " + this.ProductName;
            }
            else
            {
                //We have no source entity instance name so just append the product name.
                SectionTitle += this.ProductName;
            }
        }

        protected override bool EnableScriptGlobalization
        {
            get { return true; }
        }

        protected override void RegisterScriptsAndServices(System.Web.UI.ScriptManager manager)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>");
            sb.Append("$(document).ready(");
            sb.Append(" function() { ");
            sb.Append(string.Format(" ise.Configuration.registerConfig('ImageZoomLensWidth', '{0}');", AppLogic.AppConfig("ImageZoomLensWidth")));
            sb.Append(string.Format(" ise.Configuration.registerConfig('ImageZoomLensHeight', '{0}');", AppLogic.AppConfig("ImageZoomLensHeight")));
            sb.Append(" });");
            sb.Append("</script>");
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ImageZoomSize", sb.ToString(), false);

            manager.Scripts.Add(new ScriptReference("jscripts/product_ajax.js"));            
            manager.Scripts.Add(new ScriptReference("jscripts/kitProduct_ajax.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/tooltip.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/imagezoom.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/productcompare.js"));
            manager.Scripts.Add(new ScriptReference("jscripts/carousel.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            if (RequiresReg && ThisCustomer.IsNotRegistered)
            {
                writer.Write("<br/><br/><br/><br/><b><font color=red>" +
                        AppLogic.GetString("showproduct.aspx.1", SkinID, ThisCustomer.LocaleSetting) +
                        "</font></b><br/><br/><br/><a href=\"signin.aspx?returnurl=showproduct.aspx?" +
                        CommonLogic.ServerVariables("QUERY_STRING").ToUrlEncode().ToHtmlEncode() + "\">" +
                        AppLogic.GetString("showproduct.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</a> " +
                        AppLogic.GetString("showproduct.aspx.3", SkinID, ThisCustomer.LocaleSetting));
            }
            else
            {
                writer.Write("<!-- XmlPackage: " + m_XmlPackage + " -->\n");
                if (m_XmlPackage.Length == 0)
                {
                    writer.Write("<p><b><font color=red>XmlPackage format was chosen, but no XmlPackage was specified!</font></b></p>");
                }
                else
                {
                    var runtimeParams = new List<XmlPackageParam>();
                    runtimeParams.Add(new XmlPackageParam("EntityName", SourceEntity));
                    runtimeParams.Add(new XmlPackageParam("EntityID", SourceEntityID));
                    runtimeParams.Add(new XmlPackageParam("ItemCode", _itemCode));
                    runtimeParams.Add(new XmlPackageParam("EntityCode", SourceEntityID));
                    runtimeParams.Add(new XmlPackageParam("UserCode", InterpriseHelper.ConfigInstance.UserCode));

                    string contents =
                    AppLogic.RunXmlPackage(
                        m_XmlPackage,
                        base.GetParser,
                        ThisCustomer,
                        SkinID,
                        string.Empty,
                        runtimeParams,
                        true,
                        true
                    );

                    writer.Write(contents);
                }
            }
        }

    }
}