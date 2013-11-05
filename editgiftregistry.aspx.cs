using System;
using System.Linq;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerce;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DataAccess;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.Tool;
using System.Collections.Generic;
using System.Text;

namespace InterpriseSuiteEcommerce
{
    public partial class editgiftregistry : SkinBase
    {
        #region Initialization

        protected void Page_Load(object sender, EventArgs e)
        {
            SectionTitle = AppLogic.GetString("giftregistry.aspx.13", SkinID, ThisCustomer.LocaleSetting, true);

            if (AppLogic.AppConfigBool("GoNonSecureAgain"))
            {
                SkinBase.GoNonSecureAgain();
            }

            if (ThisCustomer.IsNotRegistered)
            {
                string requestedPage = Security.UrlEncode(Request.Url.PathAndQuery);
                Response.Redirect("signin.aspx?returnurl=" + requestedPage);
            }

            if (!AppLogic.AppConfigBool("GiftRegistry.Enabled"))
            {
                CurrentContext.GoPageNotFound();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            ctrlGiftRegistryForm.ThisCustomer = ThisCustomer;
            ctrlGiftRegistryItemList.ThisCustomer = ThisCustomer;
            ctrlGiftRegistryItemListOptions.ThisCustomer = ThisCustomer;

             string urlRegistryId = string.Empty;

            btnSave.Click += (sender, ex) => SaveRegistry();
            btnShowAllRegistry.Click += (sender, ex) => Response.Redirect("~/giftregistry.aspx");
            btnShare.Click += (sender, ex) =>
            {
                urlRegistryId = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode();
                Response.Redirect(string.Format("~/sharegiftregistry.aspx?{0}={1}", DomainConstants.GIFTREGISTRYPARAMCHAR, urlRegistryId));
            };

            ctrlGiftRegistryForm.CustomURL = ThisCustomer.GenerateRandomCustomURLForGiftRegistry();
            urlRegistryId = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode();

            if (!urlRegistryId.IsNullOrEmptyTrimmed())
            {
                //check if invalid registryid
                if (urlRegistryId.TryParseGuid().HasValue)
                {
                    var giftRegistry = GiftRegistryFromQueryString;

                    //check if invalid registryid
                    if (giftRegistry == null) RedirectToEditRegistry();

                    //check if viewer is owner of the registry
                    if (!this.ThisCustomer.IsRegistryOwnedByCustomer(urlRegistryId.TryParseGuid().Value)) RedirectToEditRegistry();

                    //unable to share if registry is not active
                    if (giftRegistry.IsActive) { btnShare.Visible = true; }

                    ctrlGiftRegistryForm.RegistryID = giftRegistry.RegistryID;
                    ctrlGiftRegistryForm.IsEditMode = true;

                    UpdatePreview();

                    LoadRegistry(giftRegistry);
                }
            }

            InitHeaderText();

            base.OnInit(e);
        }

        #endregion

        #region Methods

        private void RedirectToEditRegistry()
        {
            Response.Redirect("~/editgiftregistry.aspx");
        }

        private void LoadRegistry(GiftRegistry giftRegistry)
        {
            ctrlGiftRegistryForm.LoadGiftRegistry(giftRegistry);
            var items = giftRegistry.GiftRegistryItems;

            pnlRegItems.Visible = false;
            pnlRegItemOptions.Visible = false;

            if (items.Count() == 0)
            {
                topicContainer.Visible = true;
                litTopic.Text = new Topic("GiftRegistry.EmptyCartInstruction", ThisCustomer.SkinID).Contents;
                return;
            } 

            items = items.BuildItemsForTransaction(ThisCustomer, giftRegistry.RegistryID);

            var regItems = items.Where(item => item.GiftRegistryItemType == GiftRegistryItemType.vItem)
                                                              .OrderBy(item => item.SortOrder);

            if (ctrlGiftRegistryForm.IsEditMode)
            {
                pnlRegItems.Visible = false;
                bool hasRegItems = (regItems.Count() > 0);
                if (hasRegItems)
                {
                    ctrlGiftRegistryItemList.GiftRegistryItems = regItems;
                    pnlRegItems.Visible = true;
                }

                var regItemsOptions = items.Where(item => item.GiftRegistryItemType == GiftRegistryItemType.vOption)
                                                                        .OrderByDescending(item => item.SortOrder);
                pnlRegItemOptions.Visible = false;
                bool hasRegOptionItems = (regItemsOptions.Count() > 0);
                if (hasRegOptionItems)
                {
                    ctrlGiftRegistryItemListOptions.GiftRegistryItems = regItemsOptions;
                    pnlRegItemOptions.Visible = true;
                }

                //filter the current registry. Should not be shown to the list.
                ddlModalRegistries.DataSource = ThisCustomer.GiftRegistries.Where(item => item.RegistryID != giftRegistry.RegistryID);
                ddlModalRegistries.DataTextField = "Title";
                ddlModalRegistries.DataValueField = "RegistryID";
                ddlModalRegistries.DataBind();
            }

        }

        private void UpdatePreview()
        {
            string giftRegistryviewerRoot = CurrentContext.FullyQualifiedApplicationPath() + "viewregistry.aspx?" + DomainConstants.GIFTREGISTRYPARAMCHAR + "=";
            ctrlGiftRegistryForm.PrimaryURLText = giftRegistryviewerRoot;

            string customURL = GiftRegistryDA.GetCustomURLByRegistryID(ctrlGiftRegistryForm.RegistryID.Value);
            ctrlGiftRegistryForm.LinkPreview.Visible = true;
            ctrlGiftRegistryForm.LinkPreview.HRef = giftRegistryviewerRoot + customURL;
            ctrlGiftRegistryForm.LinkPreview.InnerText = AppLogic.GetString("editgiftregistry.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                ctrlGiftRegistryForm.LinkPreview.Attributes.Add("data-contentKey", "editgiftregistry.aspx.16");
                ctrlGiftRegistryForm.LinkPreview.Attributes.Add("data-contentValue", AppLogic.GetString("editgiftregistry.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting, true));
                ctrlGiftRegistryForm.LinkPreview.Attributes.Add("data-contentType", "string resource");
            }

            ctrlGiftRegistryForm.CustomURL = customURL;
        }

        private void SaveRegistry()
        {
            GiftRegistry giftRegistryItem = null;

            string defaultFileName = ctrlGiftRegistryForm.PictureFileName;
            if (ctrlGiftRegistryForm.IsEditMode)
            {
                giftRegistryItem = GiftRegistryDA.GetGiftRegistryByRegistryID(ctrlGiftRegistryForm.RegistryID.Value, InterpriseHelper.ConfigInstance.WebSiteCode);
                giftRegistryItem.StartDate = ctrlGiftRegistryForm.StartDate;
                giftRegistryItem.EndDate = ctrlGiftRegistryForm.EndDate;
                giftRegistryItem.Title = ctrlGiftRegistryForm.Title;
                giftRegistryItem.Message = ctrlGiftRegistryForm.GuestMessage;
                giftRegistryItem.IsPrivate = ctrlGiftRegistryForm.PrivatePrivacy;
                giftRegistryItem.PictureFileName = (!defaultFileName.IsNullOrEmptyTrimmed()) ? defaultFileName : DomainConstants.DEFAULT_NO_PIC_FILENAME;
                giftRegistryItem.PictureStream = ctrlGiftRegistryForm.PhotoStream;
                giftRegistryItem.CustomURLPostfix = ctrlGiftRegistryForm.CustomURL.TrimEnd().ToUrlEncode();
                giftRegistryItem.RegistryID = ctrlGiftRegistryForm.RegistryID.Value;
                giftRegistryItem.GuestPassword = (ctrlGiftRegistryForm.PrivatePrivacy) ? ctrlGiftRegistryForm.GuestPassword : string.Empty;
                giftRegistryItem.SkinID = ThisCustomer.SkinID;
                giftRegistryItem.LocaleSettings = ThisCustomer.LocaleSetting;
                giftRegistryItem.IsEditMode = true;
                //giftRegistryItem.IsActive = true;
            }
            else
            {
                giftRegistryItem = new GiftRegistry(ThisCustomer.SkinID, ThisCustomer.LocaleSetting)
                {
                    ContactGUID = ThisCustomer.ContactGUID,
                    StartDate = ctrlGiftRegistryForm.StartDate,
                    EndDate = ctrlGiftRegistryForm.EndDate,
                    Title = ctrlGiftRegistryForm.Title,
                    Message = ctrlGiftRegistryForm.GuestMessage,
                    IsPrivate = ctrlGiftRegistryForm.PrivatePrivacy,
                    GuestPassword = (ctrlGiftRegistryForm.PrivatePrivacy) ? ctrlGiftRegistryForm.GuestPassword : string.Empty,
                    PictureFileName = (!defaultFileName.IsNullOrEmptyTrimmed()) ? defaultFileName : DomainConstants.DEFAULT_NO_PIC_FILENAME,
                    PictureStream = ctrlGiftRegistryForm.PhotoStream,
                    CustomURLPostfix = ctrlGiftRegistryForm.CustomURL.TrimEnd().ToUrlEncode(),
                    WebsiteCode = InterpriseHelper.ConfigInstance.WebSiteCode,
                    RegistryID = Guid.NewGuid(),
                    IsEditMode = false,
                    IsActive = true
                };
            }

            giftRegistryItem.Validate();

            if (!giftRegistryItem.HasErrors)
            {
                if (!ctrlGiftRegistryForm.IsEditMode)
                {
                    giftRegistryItem.PictureFileName = giftRegistryItem.ProcessPicture(Server.MapPath(string.Empty));
                    ThisCustomer.GiftRegistries.AddToDb(giftRegistryItem);
                    Response.Redirect(string.Format("editgiftregistry.aspx?{0}={1}", DomainConstants.GIFTREGISTRYPARAMCHAR, giftRegistryItem.RegistryID.ToString()));
                }
                else
                {
                    giftRegistryItem.PictureFileName = giftRegistryItem.ProcessPicture(Server.MapPath(string.Empty));
                    ThisCustomer.GiftRegistries.UpdateToDb(giftRegistryItem);
                    UpdatePreview();
                    LoadRegistry(giftRegistryItem);
                }

                pnlErrorMessage.Visible = false;
            }
            else
            {
                DisplayError(giftRegistryItem.GetErrorMessage());
            }
        }

        private void DisplayError(IEnumerable<string> errorMessages)
        {
            pnlErrorMessage.Visible = false;
            if (!errorMessages.IsNullOrEmptyTrimmed())
            {
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
        }

        private void InitHeaderText()
        {   
            string saveKey = String.Empty;

            var registryID = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode().TryParseGuid();
            if (!registryID.HasValue)
            {
                litRegistryHeader.Text = AppLogic.GetString("editgiftregistry.aspx.1", SkinID, ThisCustomer.LocaleSetting);
                btnSave.Text = AppLogic.GetString("editgiftregistry.aspx.13", SkinID, ThisCustomer.LocaleSetting, true);
                saveKey = "editgiftregistry.aspx.13";
            }
            else
            {
                litRegistryHeader.Text = AppLogic.GetString("editgiftregistry.aspx.2", SkinID, ThisCustomer.LocaleSetting);
                btnSave.Text = AppLogic.GetString("editgiftregistry.aspx.14", SkinID, ThisCustomer.LocaleSetting, true);
                saveKey = "editgiftregistry.aspx.14";
            }

            btnShare.Text = AppLogic.GetString("editgiftregistry.aspx.18", SkinID, ThisCustomer.LocaleSetting, true);
            btnShowAllRegistry.Text = AppLogic.GetString("editgiftregistry.aspx.22", SkinID, ThisCustomer.LocaleSetting, true);

            if (ThisCustomer.IsInEditingMode())
            {
                AppLogic.EnableButtonCaptionEditing(btnSave, saveKey);
                AppLogic.EnableButtonCaptionEditing(btnShare, "editgiftregistry.aspx.18");
                AppLogic.EnableButtonCaptionEditing(btnShowAllRegistry, "editgiftregistry.aspx.22");
            }
        }

        private GiftRegistry GiftRegistryFromQueryString
        {
            get
            {
                GiftRegistry giftregistry = null;
                Guid? registryId = DomainConstants.GIFTREGISTRYPARAMCHAR.ToQueryStringDecode().TryParseGuid();
                if (registryId.HasValue)
                {
                    giftregistry = GiftRegistryDA.GetGiftRegistryByRegistryID(registryId.Value, InterpriseHelper.ConfigInstance.WebSiteCode);
                }
                return giftregistry;
            }
        }

        #endregion
    }
}