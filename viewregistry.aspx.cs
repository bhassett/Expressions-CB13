using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;

namespace InterpriseSuiteEcommerce
{
    public partial class viewregistry : SkinBase
    {

        #region Initializer

        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("giftregistry.aspx.13", SkinID, ThisCustomer.LocaleSetting, true);

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            if (!AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                CurrentContext.GoPageNotFound();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            string message = DomainConstants.NOTIFICATION_QRY_STRING_PARAM.ToQueryString();
            if(!message.IsNullOrEmptyTrimmed())
            {
                DisplayError(new List<string>() { message });
            }

            ctrlGiftRegistryViewForm.ThisCustomer = ThisCustomer;
            ctrGiftRegistryViewItemList.ThisCustomer = ThisCustomer;
            GiftRegistryViewItemOptionList.ThisCustomer = ThisCustomer;

            btnAuthenticate.Click += (sender, ex) => Authenticate();

            if (!IsPostBack)
            {
                InitAuthentication();
            }

            lblError.Text = AppLogic.GetString("editgiftregistry.error.11", SkinID, this.ThisCustomer.LocaleSetting);
        }

        #endregion

        #region Methods

        private void InitAuthentication() 
        {
            var giftRegistry = this.GiftRegistryFromQueryString;
            if (giftRegistry != null)
            {
                bool registryOwnedByCustomer = ThisCustomer.IsRegistryOwnedByCustomer(giftRegistry.RegistryID);

                if (registryOwnedByCustomer)
                {
                    LoadRegistry(giftRegistry);
                    pnlMain.Visible = true;
                    pnlPassword.Visible = false;
                    return;
                }

                if ((giftRegistry.IsPrivate && !giftRegistry.GuestPassword.IsNullOrEmptyTrimmed()))
                {
                    string viewedRegID = this.ThisCustomer.ThisCustomerSession["ViewedRegistryID"];
                    if (!viewedRegID.IsNullOrEmptyTrimmed() && viewedRegID == giftRegistry.RegistryID.ToString())
                    {
                        LoadRegistry(giftRegistry);
                        pnlMain.Visible = true;
                        pnlPassword.Visible = false;
                        return;
                    }
                    else
                    {
                        pnlMain.Visible = false;
                        pnlPassword.Visible = true;
                    }
                }
                else
                {
                    pnlMain.Visible = true;
                    pnlPassword.Visible = false;
                    LoadRegistry(giftRegistry);
                }
            }
            else
            {
                Response.Redirect(CurrentContext.FullyQualifiedApplicationPath());
            }
        }

        private void Authenticate()
        {
            var giftRegistry = this.GiftRegistryFromQueryString;
            if (giftRegistry.GuestPassword == txtPassword.Text.Trim())
            {
                pnlMain.Visible = true;
                pnlPassword.Visible = false;

                //save the viewed gift registry to the db for page refresh
                this.ThisCustomer.ThisCustomerSession["ViewedRegistryID"] = giftRegistry.RegistryID.ToString();
                LoadRegistry(giftRegistry);
            }
            else
            {
                lblError.Visible = true;
                pnlMain.Visible = false;
                pnlPassword.Visible = true;
            }
        }

        private void LoadRegistry(GiftRegistry giftRegistry)
        {
            ctrlGiftRegistryViewForm.LoadGiftRegistry(giftRegistry);

            var items = giftRegistry.GiftRegistryItems;
            if (items.Count() == 0) return;

            items = items.BuildItemsForTransaction(ThisCustomer, giftRegistry.RegistryID);

            var regItems = items.Where(item => item.GiftRegistryItemType == GiftRegistryItemType.vItem && item.Quantity > 0)
                                                                 .OrderBy(item => item.SortOrder);
            pnlItems.Visible = false;
            if (regItems.Count() > 0)
            {
                ctrGiftRegistryViewItemList.CustomUrl = giftRegistry.CustomURLPostfix;
                ctrGiftRegistryViewItemList.GiftRegistryItems = regItems;
                pnlItems.Visible = true;
            }            
           
            var regOptions = items.Where(item => item.GiftRegistryItemType == GiftRegistryItemType.vOption && item.Quantity > 0)
                                                                    .OrderByDescending(item => item.SortOrder);

            pnlOptionItems.Visible = false;
            if (regOptions.Count() > 0)
            {
                GiftRegistryViewItemOptionList.CustomUrl = giftRegistry.CustomURLPostfix;
                GiftRegistryViewItemOptionList.GiftRegistryItems = regOptions;
                pnlOptionItems.Visible = true;
            }
        }

        private void DisplayError(IEnumerable<string> errorMessages)
        {
            pnlErrorMessage.Visible = false;
            var htlm = new StringBuilder();
            if (errorMessages.Count() > 0)
            {
                htlm.Append("<ul class='error-layout'>");
                foreach (var error in errorMessages)
                {
                    htlm.AppendFormat("<li>{0}</li>", error);
                }
                htlm.Append("</ul>");
            }
            htlm.ToString();

            var lit = new Literal();
            lit.Text = htlm.ToString();
            pnlErrorMessage.Controls.Add(lit);
            pnlErrorMessage.Visible = true;
        }

        #endregion

        #region Properties  

        private GiftRegistry GiftRegistryFromQueryString
        {
            get
            {
                GiftRegistry giftregistry = null;
                string urlCustomUrlParam = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode();
                if (!urlCustomUrlParam.IsNullOrEmptyTrimmed())
                {
                    giftregistry = GiftRegistryDA.GetRegistryByCustomUrl(urlCustomUrlParam, InterpriseHelper.ConfigInstance.WebSiteCode);
                }
                return giftregistry;
            }
        }

        #endregion

    }
}