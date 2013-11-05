// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DataAccess;

namespace InterpriseSuiteEcommerce
{
    /// <summary>
    /// Summary description for selectaddress.
    /// </summary>
    public partial class selectaddress : SkinBase
    {

        #region Variable Declarations

        bool addMode = false;
        bool checkOutMode = false;
        public AddressTypes AddressType = AddressTypes.Unknown;
        public Addresses custAddresses;
        public Boolean setPrimary = false;
        public String AddressTypeString = String.Empty;
        public String ButtonImage = String.Empty;
        public String PaymentMethodPrompt = String.Empty;
        public String Prompt = String.Empty;
        public String ReturnURL = String.Empty;

        #endregion

        #region OnInit

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();

            ResolveQueryStrings();
            PerformPageAccessLogic();
            InitializePageContent();
            DisplayAddressList();

            AddressBookHelpfulTips.SetContext = this;
        }

        #endregion

        #region InitiliazeComponent

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddressList.ItemDataBound += new RepeaterItemEventHandler(AddressList_ItemDataBound);
            this.AddressList.ItemCommand += new RepeaterCommandEventHandler(AddressList_ItemCommand);
        }

        #endregion

        #region ResolveQueryStrings

        private void ResolveQueryStrings()
        {
            addMode = CommonLogic.QueryStringBool("add");
            checkOutMode = CommonLogic.QueryStringBool("checkout");
            setPrimary = CommonLogic.QueryStringBool("SetPrimary");
        }

        #endregion
        
        #region PerformPageAccessLogic

        private void PerformPageAccessLogic()
        {

            ReturnURL = CommonLogic.QueryStringCanBeDangerousContent("ReturnURL");
            if (ReturnURL.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            if (AddressTypeString.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }

            ThisCustomer.RequireCustomerRecord();

            if (CommonLogic.QueryStringBool("editaddress") && !ThisCustomer.IsRegistered)
            {
                string url = CommonLogic.IIF(AppLogic.AppConfigBool("Checkout.UseOnePageCheckout"), "checkout1.aspx", String.Format("createaccount.aspx?checkout=true&skipreg=true&editaddress=true"));
                Response.Redirect(url);
            }

            if (!Shipping.MultiShipEnabled())
            {
                RequiresLogin(CommonLogic.GetThisPageName(false) + "?" + CommonLogic.ServerVariables("QUERY_STRING"));
            }

            if (AddressTypeString.Length != 0)
            {
                AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);
            }
            if (AddressType == AddressTypes.Unknown)
            {
                AddressType = AddressTypes.Shipping;
                AddressTypeString = "Shipping";
            }

            custAddresses = new Addresses();
            custAddresses.LoadCustomer(ThisCustomer, AddressType);

            if (AddressType == AddressTypes.Shipping)
            {
                ButtonImage = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/usethisshippingaddress.gif", ThisCustomer.LocaleSetting);
            }
            else
            {
                ButtonImage = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/usethisbillingaddress.gif", ThisCustomer.LocaleSetting);
            }
        }

        #endregion

        #region InitializePageContent

        private void InitializePageContent()
        {
            pnlCheckoutImage.Visible = checkOutMode;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");

            pnlAddressList.Visible = (custAddresses.Count > 0 || addMode);
            pnlAddressListMain.Visible = (!addMode);
            pnlNewAddress.Visible = (addMode);

            lnkAddAddress.Text = AppLogic.GetString("selectaddress.aspx.3", SkinID, ThisCustomer.LocaleSetting);
            lnkAddAddress.NavigateUrl = "selectaddress.aspx?add=true&checkout=" + checkOutMode.ToString() + "&addressType=" + AddressType.ToString() + "&returnURL=" + Server.UrlEncode(ReturnURL);
            lnkAddAddress.Visible = (!addMode);
            liAdd.Visible = (!addMode);

            if (addMode)
            {
                pnlSaveAddress.Visible = true;

                string addressType = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
                if (addressType.ToLower() != "shipping")
                {
                    AddressControl.showResidenceTypes = false;
                }
                else
                {
                    AddressControl.showResidenceTypes = true;
                }
            }
        }

        #endregion

        #region DisplayAddressList

        private void DisplayAddressList()
        {
            LoadAddresses();
        }

        #endregion

        #region LoadAddresses

        private void LoadAddresses()
        {
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), (int)AddressType, DB.SQuote(ThisCustomer.ContactCode))))
                {
                    AddressList.DataSource = reader;
                    AddressList.DataBind();
                    reader.Close();
                    btnReturn.Text = AppLogic.GetString("account.aspx.25", SkinID, ThisCustomer.LocaleSetting, true);
                    btnReturn.OnClientClick = "self.location='account.aspx?checkout=" + checkOutMode.ToString() + "';return false";
                    btnCheckOut.Visible = checkOutMode;
                    btnCheckOut.Text = AppLogic.GetString("account.aspx.24", SkinID, ThisCustomer.LocaleSetting);

                    string redirecTo = (checkOutMode && ReturnURL.IsNullOrEmpty()) ? "checkoutshipping.aspx" : ReturnURL;
                    if (checkOutMode && AppLogic.AppConfigBool("Checkout.UseOnePageCheckout")) redirecTo = "checkout1.aspx";

                    btnCheckOut.OnClientClick = String.Format("self.location='{0}';return false;", redirecTo);

                    if (ThisCustomer.IsInEditingMode())
                    {
                        AppLogic.EnableButtonCaptionEditing(btnReturn, "account.aspx.25");
                        AppLogic.EnableButtonCaptionEditing(btnCheckOut, "account.aspx.24");
                    }
                }
            }
        }

        #endregion

        #region AddressList ItemDataBound

        void AddressList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ImageButton MakePrimaryBtn = (ImageButton)e.Item.FindControl("btnMakePrimary");
                ImageButton EditBtn = (ImageButton)e.Item.FindControl("btnEdit");

                MakePrimaryBtn.Visible = (((DbDataRecord)e.Item.DataItem)["PrimaryAddress"].ToString() == "0");

                MakePrimaryBtn.ImageUrl = ButtonImage;
                EditBtn.ImageUrl = AppLogic.LocateImageURL("skins/Skin_" + SkinID.ToString() + "/images/edit.gif");
            }

        }

        #endregion

        #region AddressList ItemCommand

        void AddressList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    Response.Redirect(String.Format("editaddress.aspx?Checkout={0}&AddressType={1}&AddressID={2}&ReturnURL={3}", checkOutMode.ToString(), AddressType, e.CommandArgument, ReturnURL));
                    break;
                case "makeprimary":
                    InterpriseHelper.MakeDefaultAddress(ThisCustomer.ContactCode, e.CommandArgument.ToString(), AddressType);
                    //Update customer default address.
                    if (AddressType == AddressTypes.Shipping)
                    {
                        ThisCustomer.PrimaryShippingAddressID = e.CommandArgument.ToString();
                    }
                    else
                    {
                        ThisCustomer.PrimaryBillingAddressID = e.CommandArgument.ToString();
                    }
                    Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
                    break;
            }
        }

        #endregion
        
        #region NewAddress

        public void btnNewAddress_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                var AddressType = AddressTypeString.TryParseEnum<AddressTypes>();
                int OriginalRecurringOrderNumber    = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
                bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

                if (!AllowShipToDifferentThanBillTo)
                {
                    //Shipping and Billing address must be the same so save both
                    AddressType = AddressTypes.Shared;
                }

                Address thisAddress = new Address();

                thisAddress.ThisCustomer  = ThisCustomer;
                thisAddress.CustomerCode  = ThisCustomer.CustomerCode;


                string bCityStates = txtCityStates.Text;
                string city = string.Empty;
                string state = string.Empty;

                if (!string.IsNullOrEmpty(bCityStates))
                {
                    var _cityState = bCityStates.Split(',');

                    if (_cityState.Length > 1)
                    {
                        state = _cityState[0].Trim();
                        city = _cityState[1].Trim();
                    }
                    else
                    {
                        city = _cityState[0].Trim();
                        state = string.Empty;
                    }

                }
                else
                {
                    state = AddressControl.state;
                    city = AddressControl.city;
                }

                thisAddress.Name = txtContactName.Text;
                thisAddress.Address1 = AddressControl.street;
                thisAddress.City = city;
                thisAddress.State = state;
                thisAddress.PostalCode = AddressControl.postal;
                thisAddress.Country = AddressControl.country;
                thisAddress.Phone = txtContactNumber.Text;
             
                if (AppLogic.AppConfigBool("Address.ShowCounty")) thisAddress.County = AddressControl.county;

                switch (AddressType)
                {
                    case AddressTypes.Shared:

                        thisAddress.ResidenceType = ResidenceTypes.Residential;

                        InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                        InterpriseHelper.AddCustomerShipTo(thisAddress);

                        break;
                    case AddressTypes.Billing:

                        thisAddress.ResidenceType = ResidenceTypes.Residential;
                        InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                       

                        break;
                    case AddressTypes.Shipping:

                        if (AddressControl.residenceType == ResidenceTypes.Residential.ToString())
                        {
                          thisAddress.ResidenceType = ResidenceTypes.Residential;
                        }else{
                          thisAddress.ResidenceType = ResidenceTypes.Commercial;
                        }

                        InterpriseHelper.AddCustomerShipTo(thisAddress);

                        break;
                }

                AppLogic.SavePostalCode(thisAddress);
                Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
                
            }
        }

        #endregion

        #region Event Handlers

        #region Page_Load

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            RequireSecurePage();

            SectionTitle = String.Format(AppLogic.GetString("selectaddress.aspx.1", SkinID, ThisCustomer.LocaleSetting), AddressTypeString);

        }

        #endregion

        #region OnRenderHeader

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("~/jscripts/address_ajax.js"));            
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));  
        }

        #endregion

        #endregion
        
    }
}