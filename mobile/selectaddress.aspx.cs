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
            InitializeAddressControls();
            DisplayAddressList();
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
        
        #region CheckWhichCountriesWeDontRequirePostalCodes

        private void CheckWhichCountriesWeDontRequirePostalCodes()
        {
            string postalCodeNotRequiredCountries = AppLogic.AppConfig("PostalCodeNotRequiredCountries");
            string[] countriesThatDontRequirePostalCodes = postalCodeNotRequiredCountries.Split(',');
            foreach (string country in countriesThatDontRequirePostalCodes)
            {
                ctrlAddress.PostalCodeOptionalCountryCodes.Add(country.Trim());
            }
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
			//removed mobile changes
            //liAdd.Visible = (!addMode);

            if (addMode)
            {
                ctrlAddress.Visible = true;
                btnNewAddress.Text = AppLogic.GetString("selectaddress.aspx.2", SkinID, ThisCustomer.LocaleSetting);
            }
        }

        #endregion

        #region InitializeAddressControls

        private void InitializeAddressControls()
        {
            LoadAllAvailableCountriesAndAssignRegistriesForAddresses();

			//mobile changes
            this.ctrlAddress.RenderStateDropDownBasedOnCurrentSelectedCountry();

            LoadAvailableSalutationsForBillingAddress();
            ApplyAddressStyles();
            CheckWhichCountriesWeDontRequirePostalCodes();
            InitializeAddressDisplay();
            AssignAddressValidatorPrerequisites();
            AssignHostFormForAddresses();
            AssignErrorSummaryDisplayControlForAddresses();
        }

        #endregion

        #region DisplayAddressList

        private void DisplayAddressList()
        {
            LoadAddresses();
        }

        #endregion

        #region LoadAvailableSalutationsForBillingAddress

        private void LoadAvailableSalutationsForBillingAddress()
        {
            List<KeyValuePair<string, string>> salutations = new List<KeyValuePair<string, string>>();

            salutations.Add(new KeyValuePair<string, string>("Select One", string.Empty));
            
            using (SqlConnection con = DB.NewSqlConnection())
            {
                con.Open();
                using (IDataReader reader = DB.GetRSFormat(con, "SELECT SalutationDescription FROM SystemSalutation with (NOLOCK) WHERE IsActive = 1"))
                {
                    while (reader.Read())
                    {
                        salutations.Add(new KeyValuePair<string, string>(DB.RSField(reader, "SalutationDescription"), DB.RSField(reader, "SalutationDescription")));
                    }
                }
            }

            ctrlAddress.SetSalutations(salutations);
        }

        #endregion

        #region LoadAllAvailableCountriesAndAssignRegistriesForAddresses

        private void LoadAllAvailableCountriesAndAssignRegistriesForAddresses()
        {
            List<CountryAddressDTO> countries = CountryAddressDTO.GetAllCountries();

            ctrlAddress.Countries = countries;
            ctrlAddress.RegisterCountries = true;
        }

        #endregion

        #region ApplyAddressStyles

        private void ApplyAddressStyles()
        {
			//change for mobile layout
            litHeaderText.Text = AppLogic.GetString("mobile.createaccount.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region InitializeAddressDisplay

        private void InitializeAddressDisplay()
        {
            if (AddressType == AddressTypes.Shipping)
            {
                InitShippingAddress();
            }
            else
            {
                InitBillingAddress();
            }
        }

        #endregion

        #region AssignAddressValidatorPrerequisites

        private void AssignAddressValidatorPrerequisites()
        {
            if (AddressType == AddressTypes.Shipping)
            {
                AssignShippingAddressValidatorPrerequisites();
            }
            else
            {
                AssignBillingAddressValidatorPrerequisites();
            }
        }

        #endregion

        #region AssignHostFormForAddresses

        private void AssignHostFormForAddresses()
        {
            ctrlAddress.HostForm = frmAddAddress;
        }

        #endregion

        #region AssignErrorSummaryDisplayControlForAddresses

        private void AssignErrorSummaryDisplayControlForAddresses()
        {
            ctrlAddress.ErrorSummaryControl = this.InputValidatorySummary1;
        }

        #endregion

        #region InitBillingAddress

        private void InitBillingAddress()
        {
            ctrlAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.34", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressCaption = AppLogic.GetString("createaccount.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.BusinessTypeCaption = AppLogic.GetString("address.cs.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.TaxNumberCaption = AppLogic.GetString("address.cs.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            bool _postalCodeOptionalCountry = false;
            foreach (string country in ctrlAddress.PostalCodeOptionalCountryCodes)
            {
                if (country == ctrlAddress.CountryCode) { _postalCodeOptionalCountry = true; }
            }
            if (_postalCodeOptionalCountry)
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            
            ctrlAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.ShowResidenceType = false;
            ctrlAddress.RequireSalutation = false;
            ctrlAddress.ShowFirstName = false;
            ctrlAddress.ShowLastName = false;

            ctrlAddress.RequireEmail = false;
            ctrlAddress.RequirePassword = false;
            ctrlAddress.RequireOkToEmail = false;
            ctrlAddress.RequireOver13 = false;
            ctrlAddress.RequireBusinessType = false; 
            ctrlAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
        }

        #endregion

        #region InitShippingAddress

        private void InitShippingAddress()
        {
            ctrlAddress.AccountNameCaption = AppLogic.GetString("createaccount.aspx.34", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.SalutationCaption = AppLogic.GetString("createaccount.aspx.35", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressCaption = AppLogic.GetString("createaccount.aspx.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.ResidenceTypeCaption = AppLogic.GetString("address.cs.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.CountryCaption = AppLogic.GetString("createaccount.aspx.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.FirstNameCaption = AppLogic.GetString("createaccount.aspx.6", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameCaption = AppLogic.GetString("createaccount.aspx.7", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneNumberCaption = AppLogic.GetString("createaccount.aspx.16", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.WithOutStateCityCaption = AppLogic.GetString("createaccount.aspx.33", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            bool _postalCodeOptionalCountry = false;
            foreach (string country in ctrlAddress.PostalCodeOptionalCountryCodes)
            {
                if (country == ctrlAddress.CountryCode) { _postalCodeOptionalCountry = true; }
            }
            if (_postalCodeOptionalCountry)
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.30", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }
            else
            {
                ctrlAddress.WithOutStatePostalCaption = AppLogic.GetString("createaccount.aspx.77", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
                ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            }

            ctrlAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.RequireSalutation = false;
            ctrlAddress.ShowFirstName = false;
            ctrlAddress.ShowLastName = false;

            ctrlAddress.RequireEmail = false;
            ctrlAddress.RequirePassword = false;
            ctrlAddress.RequireOkToEmail = false;
            ctrlAddress.RequireOver13 = false;
            ctrlAddress.RequireBusinessType = false;
            ctrlAddress.ShowCounty = AppLogic.AppConfigBool("Address.ShowCounty");
        }

        #endregion

        #region AssignBillingAddressValidatorPrerequisites

        private void AssignBillingAddressValidatorPrerequisites()
        {
            ctrlAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.37", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.38", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.39", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.40", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.41", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.FirstNameMaximumCharacterLength = 50;
            ctrlAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.42", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameMaximumCharacterLength = 50;
            ctrlAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.43", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AccountNameMaximumCharacterLength = 100;
            ctrlAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.44", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressMaximumCharacterLength = 200;
            ctrlAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.45", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneMaximumCharacterLength = 50;
            ctrlAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.46", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.69", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.CityMaximumCharacterLength = 50;
            ctrlAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.73", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.70", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.74", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.19", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region AssignShippingAddressValidatorPrerequisites

        private void AssignShippingAddressValidatorPrerequisites()
        {
            ctrlAddress.FirstNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.56", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.57", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AccountNameRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.58", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.59", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.60", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.FirstNameMaximumCharacterLength = 50;
            ctrlAddress.FirstNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.61", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.LastNameMaximumCharacterLength = 50;
            ctrlAddress.LastNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.62", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AccountNameMaximumCharacterLength = 100;
            ctrlAddress.AccountNameMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.63", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.AddressMaximumCharacterLength = 200;
            ctrlAddress.AddressMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.64", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PhoneMaximumCharacterLength = 50;
            ctrlAddress.PhoneMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.65", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.CityRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.71", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.CityMaximumCharacterLength = 50;
            ctrlAddress.CityMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.75", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.PostalCodeRequiredErrorMessage = AppLogic.GetString("createaccount.aspx.72", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.PostalCodeMaximumCharacterLength = 10;
            ctrlAddress.PostalCodeMaximumCharacterLengthErrorMessage = AppLogic.GetString("createaccount.aspx.76", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.POBoxAddressNotAllowedErrorMessage = AppLogic.GetString("address.cs.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        #endregion

        #region LoadAddresses

        private void LoadAddresses()
        {
            using (var con = DB.NewSqlConnection())
            {
                con.Open();
                using (var reader = DB.GetRSFormat(con, string.Format("exec EcommerceGetAddressList @CustomerCode = {0}, @AddressType = {1}, @ContactCode = {2} ", DB.SQuote(ThisCustomer.CustomerCode), (int)AddressType, DB.SQuote(ThisCustomer.ContactCode))))
                {
                    AddressList.DataSource = reader;
                    AddressList.DataBind();
                    reader.Close();
                    
                    btnReturn.Text = AppLogic.GetString("account.aspx.25", SkinID, ThisCustomer.LocaleSetting);
                    btnReturn.OnClientClick = "self.location='account.aspx?checkout=" + checkOutMode.ToString() + "';return false";

                    btnCheckOut.Visible = checkOutMode;
                    btnCheckOut.Text = AppLogic.GetString("account.aspx.24", SkinID, ThisCustomer.LocaleSetting);
                    btnCheckOut.OnClientClick = "self.location='checkoutshipping.aspx';return false;";
                }
            }
        }

        #endregion

        #region AddressList ItemDataBound

        void AddressList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            
            var MakePrimaryBtn = e.Item.FindByParse<mobile_UserControls_ISEMobileButton>("btnMakePrimary");
            var EditBtn = e.Item.FindByParse<mobile_UserControls_ISEMobileButton>("btnEdit");
			
			//mobile changes
            EditBtn.Text = AppLogic.GetString("mobile.selectaddress.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            MakePrimaryBtn.Text = AppLogic.GetString("mobile.selectaddress.aspx.2", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            MakePrimaryBtn.Visible = (((DbDataRecord)e.Item.DataItem)["PrimaryAddress"].ToString() == "0");
        }

        #endregion

        #region AddressList ItemCommand

        void AddressList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    Response.Redirect(string.Format("editaddress.aspx?Checkout={0}&AddressType={1}&AddressID={2}&ReturnURL={3}", checkOutMode.ToString(), AddressType, e.CommandArgument, ReturnURL));
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
                    Response.Redirect(string.Format("selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}", checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL)));
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Checked Address Information based on Address Type
        /// And Validate
        /// </summary>
        /// <param name="thisAddress">Address Information</param>
        /// <param name="type">[Enum] Address Type</param>
        /// <returns>bool: true/false</returns>
        public bool CheckToValidate(Address thisAddress, AddressTypes type)
        {
            // For AddressType Objects
            Address billingAddress = null;
            Address shippingAddress = null;

            bool retCheck = false;

                if (type == AddressTypes.Billing)
                {
                    thisAddress.CardName = ctrlAddress.AccountName;
                    this.hidBillCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlBillingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    var billCountry = CountryAddressDTO.Find(thisAddress.Country);
                    billingAddress = thisAddress;
                    this.hidBillCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, billCountry);
                    this.hidBlnWithState.Value = billCountry.withState.ToString();
                }
                else
                {
                    this.hidShipCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlShippingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    var shipCountry = CountryAddressDTO.Find(thisAddress.Country);
                    shippingAddress = thisAddress;
                    this.hidShipCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, shipCountry);
                    this.hidShpWithState.Value = shipCountry.withState.ToString();
                }

                DisplayErrorIfAny(type);

                // Checked if either Billing Address or Shipping Address is Correct
                if (!string.IsNullOrEmpty(this.hidBillCheck.Value) || !string.IsNullOrEmpty(this.hidShipCheck.Value))
                {
                    // Set the PopUp window to Show Up
                    hidValid.Value = "false";
                    retCheck = true;

                    hidSkinID.Value = ThisCustomer.SkinID.ToString();
                    hidLocale.Value = ThisCustomer.LocaleSetting.ToString();

                    if (AddressType == AddressTypes.Billing)
                    {
                        this.hidBillTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.87", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                        
                        this.hidBlnState.Value = billingAddress.State;
                        this.hidBlnPostalCode.Value = billingAddress.PostalCode;
                        this.hidBlnCountry.Value = billingAddress.Country;
                        this.hidBlnCity.Value = billingAddress.City;
                    }
                    else
                    {
                        this.hidShipTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.88", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                        
                        this.hidShpState.Value = shippingAddress.State;
                        this.hidShpPostalCode.Value = shippingAddress.PostalCode;
                        this.hidShpCountry.Value = shippingAddress.Country;
                        this.hidShpCity.Value = shippingAddress.City;     
                    }
                }

            return retCheck;
        }

        void DisplayErrorIfAny(AddressTypes typeToValidate) 
        {
            var cityTextBoxControl = (ctrlAddress.FindControl("WithStateCity") as WebControl);
            var postalTextBoxControl = (ctrlAddress.FindControl("WithStatePostalCode") as WebControl);

            if (this.hidBillCheck.Value.IsNullOrEmptyTrimmed() && this.hidShipCheck.Value.IsNullOrEmptyTrimmed()) return;

            cityTextBoxControl.CssClass = cityTextBoxControl.CssClass.Replace(" mobile-text-error", String.Empty);
            postalTextBoxControl.CssClass = postalTextBoxControl.CssClass.Replace(" mobile-text-error", String.Empty);

            if (typeToValidate == AddressTypes.Billing)
            {
                switch (this.hidBillCheck.Value)
                {
                    case "IsInvalidCityOnly":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalOnly":
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalAndCity":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                }

            }
            else
            {
                switch (this.hidShipCheck.Value)
                {
                    case "IsInvalidCityOnly":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalOnly":
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                    case "IsInvalidPostalAndCity":
                        cityTextBoxControl.CssClass = cityTextBoxControl.CssClass + " mobile-text-error";
                        postalTextBoxControl.CssClass = postalTextBoxControl.CssClass + " mobile-text-error";
                        break;
                }
            }
        }

        #region NewAddress

        public void btnNewAddress_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                var AddressType = AddressTypeString.TryParseEnum<AddressTypes>();
                int OriginalRecurringOrderNumber = CommonLogic.QueryStringUSInt("OriginalRecurringOrderNumber");
                bool AllowShipToDifferentThanBillTo = AppLogic.AppConfigBool("AllowShipToDifferentThanBillTo");

                if (!AllowShipToDifferentThanBillTo)
                {
                    //Shipping and Billing address must be the same so save both
                    AddressType = AddressTypes.Shared;
                }

				//changes for mobile design
                var thisAddress = ctrlAddress.ExtractAddress(ThisCustomer);
                thisAddress.CustomerCode = ThisCustomer.CustomerCode;
                thisAddress.Name = ctrlAddress.AccountName;

                if (!CheckToValidate(thisAddress, AddressType))
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Shared:

                            InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                            InterpriseHelper.AddCustomerShipTo(thisAddress);
                            break;
                        case AddressTypes.Billing:

                            InterpriseHelper.AddCustomerBillToInfo(ThisCustomer.CustomerCode, thisAddress, setPrimary);
                            break;
                        case AddressTypes.Shipping:

                            InterpriseHelper.AddCustomerShipTo(thisAddress);
                            break;
                    }

                    string url = "selectaddress.aspx?Checkout={0}&AddressType={1}&ReturnURL={2}".FormatWith(checkOutMode.ToString(), AddressTypeString, Server.UrlEncode(ReturnURL));
                    Response.Redirect(url);
                }
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

            if (addMode)
            {
                StringBuilder script = new StringBuilder();

                script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
                script.Append("Sys.Application.add_load(\n");                
                script.Append(" function() { \n");

                script.AppendFormat("   var form = $getElement('{0}');\n", this.frmAddAddress.ClientID);
                script.AppendFormat("   var addressId = '{0}';\n", this.ctrlAddress.ClientID);

                script.Append("   var delAttachHandler = function(ctrl){ form.onsubmit = function(){ return ctrl.validate(true); }}\n");

                script.Append("   var ctrl = ise.Controls.AddressController.getControl(addressId);\n");
                
                script.Append("   if(ctrl) {\n");
                script.Append("       delAttachHandler(ctrl);\n");
                script.Append("   }\n");
                script.Append("   else {\n");
                script.Append("       var observer = {\n");
                script.Append("           notify : function(ctrl) {\n");
                script.Append("               if(ctrl.id == addressId) {\n");
                script.Append("                   delAttachHandler(ctrl);\n");
                script.Append("               }\n");
                script.Append("           }\n");
                script.Append("       }\n");
                script.Append("       ise.Controls.AddressController.addObserver(observer);\n");
                script.Append("   }\n");

                script.Append(" }\n");
                script.Append(");\n");
                script.Append("</script>\n");

                Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script.ToString());
            }
        }

        #endregion

        #region OnRenderHeader

		//required for mobile ajax postal 
        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
            // this is a prerequisite as we can't be sure of the ordering of jscripts called, this will be rendered on the <head> section
            writer.WriteLine("<script type=\"text/javascript\" src=\"js/jquery/jquery-ui.min.js\" ></script>");
            writer.WriteLine("<link  type=\"text/css\" rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/jquery-ui.css\" />");
        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/address_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));  
        }

        #endregion

        #endregion
        
    }
}