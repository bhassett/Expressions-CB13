// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Extensions;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for EMailproduct.
    /// </summary>
    public partial class EMailproduct : SkinBase
    {
        int productID;
        string ItemCode;
        string CategoryID;
        string DepartmentID;
        string ManufacturerID;
        string ProductName = string.Empty;
        string VariantName = string.Empty;
        string SEName = string.Empty;
        string ProductDescription = string.Empty;
        bool RequiresReg;

		//added for mobile button
        protected override void OnInit(EventArgs e)
        {
            //.net 4.0 Covariance/Contravariance
            btnSubmit.Click += (sender, evt) => DoSubmit();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            productID = CommonLogic.QueryStringUSInt("productId");
            ItemCode = InterpriseHelper.GetInventoryItemCode(productID);
            var CategoryHelper = AppLogic.LookupHelper(base.EntityHelpers, "Category");
            var SectionHelper = AppLogic.LookupHelper(base.EntityHelpers, "Department");
            var ManufacturerHelper = AppLogic.LookupHelper(base.EntityHelpers, "Manufacturer");
            CategoryID = CommonLogic.QueryStringCanBeDangerousContent("CategoryID");
            DepartmentID = CommonLogic.QueryStringCanBeDangerousContent("DepartmentID");
            ManufacturerID = CommonLogic.QueryStringCanBeDangerousContent("ManufacturerID");

            string SourceEntity = "Category";
            string SourceEntityID = string.Empty;

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }
            // DOS attack prevention:
            if (AppLogic.OnLiveServer() && (Request.UrlReferrer == null || Request.UrlReferrer.Authority != Request.Url.Authority))
            {
                Response.Redirect(SE.MakeDriverLink("EmailError")); 
            }
            if (ItemCode == string.Empty)
            {
                Response.Redirect("default.aspx");
            }
            if (AppLogic.ProductHasBeenDeleted(productID))
            {
                Response.Redirect(SE.MakeDriverLink("ProductNotFound"));
            }


            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var rs = DB.GetRSFormat(con, "SELECT * FROM EcommerceViewProduct with (NOLOCK) " + 
                                                    " WHERE Counter=" + productID + 
                                                    " AND ShortString=" + DB.SQuote(ThisCustomer.LocaleSetting) +
                                                    " AND WebSiteCode=" + DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
                {
                    if (!rs.Read())
                    {
                        Response.Redirect("default.aspx");
                    }

                    SEName = SE.MungeName(DB.RSField(rs, "SEName"));
                    if (DB.RSField(rs, "ItemDescription").ToString() != string.Empty)
                    {
                        ProductName = DB.RSField(rs, "ItemDescription");
                    }
                    else
                    {
                        ProductName = DB.RSField(rs, "ItemName");
                    }

                    RequiresReg = DB.RSFieldBool(rs, "RequiresRegistration");
                    ProductDescription = DB.RSField(rs, "ItemDescription");
                    if (AppLogic.ReplaceImageURLFromAssetMgr)
                    {
                        ProductDescription = ProductDescription.Replace("../images", "images");
                    }
                    string FileDescription = new ProductDescriptionFile(ItemCode, ThisCustomer.LocaleSetting, SkinID).Contents;
                    if (FileDescription.Length != 0)
                    {
                        ProductDescription += "<div align=\"left\">" + FileDescription + "</div>";
                    }
                }
            }            
            
            if (Convert.ToInt32(CategoryID) == 0)
            {
                // no category passed in, pick first one that this product is mapped to:
                string tmpS = CategoryHelper.GetObjectEntities(ItemCode, false);
                if (tmpS.Length != 0)
                {
                    string[] catIDs = tmpS.Split(',');
                    CategoryID = Convert.ToString(Localization.ParseUSInt(catIDs[0]));
                }
            }

            string CategoryName = CommonLogic.IIF(CategoryHelper.GetEntityField(CategoryID, "Description", ThisCustomer.LocaleSetting) != string.Empty, CategoryHelper.GetEntityField(CategoryID, "Description", ThisCustomer.LocaleSetting), CategoryHelper.GetEntityName(CategoryID, ThisCustomer.LocaleSetting));
            string SectionName = CommonLogic.IIF(SectionHelper.GetEntityField(DepartmentID, "Description", ThisCustomer.LocaleSetting) != string.Empty, SectionHelper.GetEntityField(DepartmentID, "Description", ThisCustomer.LocaleSetting), SectionHelper.GetEntityName(DepartmentID, ThisCustomer.LocaleSetting));
            string ManufacturerName = CommonLogic.IIF(ManufacturerHelper.GetEntityField(ManufacturerID, "Description", ThisCustomer.LocaleSetting) != string.Empty, ManufacturerHelper.GetEntityField(ManufacturerID, "Description", ThisCustomer.LocaleSetting), ManufacturerHelper.GetEntityName(ManufacturerID, ThisCustomer.LocaleSetting));

            SourceEntity = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityName", true);
            string SourceEntityInstanceName = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityInstanceName", true);
            SourceEntityID = CommonLogic.CookieCanBeDangerousContent("LastViewedEntityInstanceID", true);

            // validate that source entity id is actually valid for this product:
            if (SourceEntityID.Length != 0)
            {
                var alE = EntityHelper.GetProductEntityList(ItemCode, SourceEntity);
                if (alE.IndexOf(Localization.ParseNativeInt(SourceEntityID)) == -1)
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

            SectionTitle += "<span class=\"SectionTitleText\">";
            SectionTitle += ProductName;
            SectionTitle += "</span>";

            reqToAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.13", SkinID, ThisCustomer.LocaleSetting);
            regexToAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.14", SkinID, ThisCustomer.LocaleSetting);
            reqFromAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.16", SkinID, ThisCustomer.LocaleSetting);
            regexFromAddress.ErrorMessage = AppLogic.GetString("emailproduct.aspx.17", SkinID, ThisCustomer.LocaleSetting);

            if (!this.IsPostBack)
            {
                InitializePageContent();
            }
        }

        private void InitializePageContent()
        {
            bool exists = false;
            string ImgFilename = string.Empty;
            bool existing = false;

            AppLogic.LogEvent(ThisCustomer.CustomerCode, 10, ItemCode);
            pnlRequireReg.Visible = (RequiresReg && ThisCustomer.IsNotRegistered);
            this.pnlEmailToFriend.Visible = !(RequiresReg && ThisCustomer.IsNotRegistered);

            emailproduct_aspx_1.Text = "<br><br><br><br><b>" + AppLogic.GetString("emailproduct.aspx.1", SkinID, ThisCustomer.LocaleSetting) + "</b><br><br><br><a href=\"signin.aspx?returnurl=showproduct.aspx?" + Server.HtmlEncode(Server.UrlEncode(CommonLogic.ServerVariables("QUERY_STRING"))) + "\">" + AppLogic.GetString("emailproduct.aspx.2", SkinID, ThisCustomer.LocaleSetting) + "</a> " + AppLogic.GetString("emailproduct.aspx.3", SkinID, ThisCustomer.LocaleSetting);

            string ProdPic = string.Empty;
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "SELECT Filename FROM InventoryOverrideImage with (NOLOCK) WHERE ItemCode = {0} AND WebSiteCode = {1} AND IsDefaultIcon = 1", DB.SQuote(InterpriseHelper.GetInventoryItemCode(productID)), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode)))
                {
                    existing = reader.Read();
                    if (existing)
                    {
                        ImgFilename = (DB.RSField(reader, "Filename"));
                    }
                }
            }
            ProdPic = AppLogic.LocateImageFilenameUrl("Product", InterpriseHelper.GetInventoryItemCode(productID), "medium", ImgFilename, AppLogic.AppConfigBool("Watermark.Enabled"), out exists);
            imgProduct.ImageUrl = ProdPic;

            string imgAltText= "";
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, "exec EcommerceDefaultMediumImage @ItemCode={0}, @WebSiteCode={1}, @LanguageCode={2} ", DB.SQuote(InterpriseHelper.GetInventoryItemCode(productID)), DB.SQuote(InterpriseHelper.ConfigInstance.WebSiteCode), DB.SQuote(Customer.Current.LanguageCode))) 
                {
                    existing = reader.Read();
                    if (existing)
                    {
                        imgAltText = (DB.RSField(reader, "SEAltTextMedium"));
                    }
                }
            }
            
            imgProduct.AlternateText = imgAltText;

            ProductNavLink.NavigateUrl = InterpriseHelper.MakeItemLink(ItemCode);
            ProductNavLink.Text = AppLogic.GetString("emailproduct.aspx.23", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_4.Text = AppLogic.GetString("emailproduct.aspx.4", SkinID, ThisCustomer.LocaleSetting) + " " + Security.HtmlEncode(ProductName) + CommonLogic.IIF(VariantName.Length > 0, " - " + Security.HtmlEncode(VariantName), "");
            emailproduct_aspx_11.Text = AppLogic.GetString("emailproduct.aspx.11", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_12.Text = AppLogic.GetString("emailproduct.aspx.12", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_22.Text = AppLogic.GetString("emailproduct.aspx.21", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_15.Text = AppLogic.GetString("emailproduct.aspx.15", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_18.Text = AppLogic.GetString("emailproduct.aspx.18", SkinID, ThisCustomer.LocaleSetting);
            emailproduct_aspx_19.Text = AppLogic.GetString("emailproduct.aspx.19", SkinID, ThisCustomer.LocaleSetting);
            txtMessage.Text = AppLogic.GetString("emailproduct.aspx.22", SkinID, ThisCustomer.LocaleSetting);
            btnSubmit.Text = AppLogic.GetString("emailproduct.aspx.20", SkinID, ThisCustomer.LocaleSetting);
        }

        private void DoSubmit() 
        {
            Page.Validate();
            if (Page.IsValid)
            {
                string FromAddress = txtFromAddress.Text;
                string ToAddress = txtToAddress.Text;
                string BotAddress = AppLogic.AppConfig("ReceiptEMailFrom");
                string Subject = AppLogic.AppConfig("StoreName") + " - " + SE.MungeName(ProductName);
                var Body = new StringBuilder(4096);

                var runtimeParams = new List<XmlPackageParam>();
                runtimeParams.Add(new XmlPackageParam("Subject", Subject));
                runtimeParams.Add(new XmlPackageParam("ItemCode", ItemCode));
                runtimeParams.Add(new XmlPackageParam("UserCode", InterpriseHelper.ConfigInstance.UserCode));

                Body.Append(
                    AppLogic.RunXmlPackage(
                        "notification.emailproduct.xml.config",
                        null,
                        ThisCustomer,
                        SkinID,
                        string.Empty,
                        runtimeParams,
                        false,
                        false
                    )
                );

                try
                {
                    if (AppLogic.AppConfig("MailMe_Server").Trim() != string.Empty)
                    {
                        AppLogic.SendMail(Subject, Body.ToString(), true, BotAddress, BotAddress, ToAddress, ToAddress, "", AppLogic.AppConfig("MailMe_Server"));
                        emailproduct_aspx_8.Text = AppLogic.GetString("emailproduct.aspx.8", SkinID, ThisCustomer.LocaleSetting);
                        pnlSuccess.Visible = true;
                        pnlRequireReg.Visible = false;
                        pnlEmailToFriend.Visible = false;
                    }
                    else
                    {
                        emailproduct_aspx_8.Text = AppLogic.GetString("emailproduct.aspx.24", SkinID, ThisCustomer.LocaleSetting);
                        pnlSuccess.Visible = true;
                        pnlRequireReg.Visible = false;
                        pnlEmailToFriend.Visible = false;
                    }
					//mobile added design
                    divEmailBody.Visible = false;
                }
                catch (Exception ex)
                {
                    emailproduct_aspx_8.Text = string.Format(AppLogic.GetString("emailproduct.aspx.9", SkinID, ThisCustomer.LocaleSetting), CommonLogic.GetExceptionDetail(ex, "<br>"));
                    pnlSuccess.Visible = true;
                    pnlRequireReg.Visible = false;
                    pnlEmailToFriend.Visible = false;
                }
                ReturnToProduct.Text = AppLogic.GetString("emailproduct.aspx.10", SkinID, ThisCustomer.LocaleSetting);
                ReturnToProduct.NavigateUrl = SE.MakeProductLink(productID.ToString(), SEName);
            }
            else
            {
                InitializePageContent();
            }
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

            var hlp = AppLogic.LookupHelper(SourceEntity);

            if (emptySource)
            {
                SourceEntityID = EntityHelper.GetProductsFirstEntity(ItemCode, SourceEntity).ToString();
                if (!CommonLogic.IsStringNullOrEmpty(SourceEntityID) & SourceEntityID != "0")
                {
                    emptySource = false;
                    SourceEntityInstanceName = hlp.GetEntityName(SourceEntityID, ThisCustomer.LocaleSetting);
                }

                if (emptySource)
                {
                    SourceEntity = "Department";
                    SourceEntityID = EntityHelper.GetProductsFirstEntity(ItemCode, SourceEntity).ToString();
                    if (!CommonLogic.IsStringNullOrEmpty(SourceEntityID) & SourceEntityID != "0")
                    {
                        emptySource = false;
                        hlp = AppLogic.LookupHelper(SourceEntity);
                        SourceEntityInstanceName = hlp.GetEntityName(SourceEntityID, ThisCustomer.LocaleSetting);
                    }
                }
            }

            string ParentID = hlp.GetParentEntity(SourceEntityID);
            while (ParentID.Length != 0)
            {
                SectionTitle = "<a class=\"SectionTitleText\" href=\"" + SE.MakeEntityLink(SourceEntity, ParentID, "") + "\">" + hlp.GetEntityField(ParentID, "Description", ThisCustomer.LocaleSetting) + "</a> &rarr; " + SectionTitle;
                ParentID = hlp.GetParentEntity(ParentID);
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
                SectionTitle += "<a class=\"SectionTitleText\" href=\"" + SE.MakeEntityLink(SourceEntity, SourceEntityID, SourceEntityInstanceName) + "\">" + entityDisplayName + "</a> &rarr; ";
            }
        }

		//required for search
        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }
    }    
}



