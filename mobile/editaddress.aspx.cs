// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Summary description for editaddress.
    /// </summary>
    public partial class editaddress : SkinBase
    {

        #region Variable Declarations

        bool checkOutMode = false;
        String AddressID = String.Empty;
        String AddressTypeString = String.Empty;
        AddressTypes AddressType = AddressTypes.Unknown;

        #endregion

        #region OnInit

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ResolveQueryStrings();
            PerformPageAccessLogic();

            InitializeAddressControls();

            InitializePageContent();
        }

        #endregion

        #region ResolveQueryStrings

        private void ResolveQueryStrings()
        {
            checkOutMode = CommonLogic.QueryStringBool("checkout");
            AddressID = CommonLogic.QueryStringCanBeDangerousContent("AddressID");
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
            if (AppLogic.AppConfigBool("UseSSL"))
            {
                if (AppLogic.OnLiveServer() && CommonLogic.ServerVariables("SERVER_PORT_SECURE") != "1")
                {
                    Response.Redirect(AppLogic.GetStoreHTTPLocation(true) + "changeaddress.aspx?" + CommonLogic.ServerVariables("QUERY_STRING"));
                }
            }

            AddressTypeString = CommonLogic.QueryStringCanBeDangerousContent("AddressType");
            if (AddressTypeString.IndexOf("<script>", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                throw new ArgumentException("SECURITY EXCEPTION");
            }
            //AddressTypeString is empty if you change language.
            if (AddressTypeString.Length != 0)
            {
                AddressType = (AddressTypes)Enum.Parse(typeof(AddressTypes), AddressTypeString, true);
            }
            if (AddressType == AddressTypes.Unknown)
            {
                AddressType = AddressTypes.Shipping;
                AddressTypeString = "Shipping";
            }
        }

        #endregion

        #region InitializeAddressControls

        private void InitializeAddressControls()
        {
            LoadAllAvailableCountriesAndAssignRegistriesForAddresses();
            LoadAvailableSalutationsForBillingAddress();
			
			//Removed for mobile design
            //ApplyAddressStyles();
			
            CheckWhichCountriesWeDontRequirePostalCodes();
            InitializeAddressDisplay();

            ctrlAddress.DataBind();

            DisplayCustomerAccountInformation();
            AssignAddressValidatorPrerequisites();
            AssignHostFormForAddresses();
            AssignErrorSummaryDisplayControlForAddresses();
            
        }

        #endregion

        #region InitializePageContent

        private void InitializePageContent()
        {
            pnlCheckoutImage.Visible = checkOutMode;
            CheckoutImage.ImageUrl = AppLogic.LocateImageURL("skins/skin_" + SkinID.ToString() + "/images/step_2.gif");
			//add new mobile resources
            litAddressPrompt.Text = CommonLogic.IIF(AddressType == AddressTypes.Shipping, AppLogic.GetString("mobile.editaddress.aspx.1", SkinID, ThisCustomer.LocaleSetting), AppLogic.GetString("mobile.editaddress.aspx.2", SkinID, ThisCustomer.LocaleSetting));

            btnReturn.Text = AppLogic.GetString("account.aspx.27", SkinID, ThisCustomer.LocaleSetting);
            btnReturn.OnClientClick = "self.location='selectaddress.aspx?checkout=" + checkOutMode.ToString() + "&AddressType=" + AddressTypeString + "';return false;";
			//add new mobile resources
            btnSaveAddress.Text = AppLogic.GetString("mobile.editaddress.aspx.3", SkinID, ThisCustomer.LocaleSetting);

            btnSaveAddress.CommandArgument = AddressID.ToString();
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

        #region DisplayCustomerAccountInformation

        private void DisplayCustomerAccountInformation()
        {
            if (!IsPostBack)
            {
                Address theAddress = Address.Get(ThisCustomer, AddressType, AddressID);

                ctrlAddress.AccountName = theAddress.Name;
                ctrlAddress.CountryCode = theAddress.Country;
                ctrlAddress.Address = theAddress.Address1;
                ctrlAddress.ResidenceType = theAddress.ResidenceType;
                ctrlAddress.TaxNumber = ThisCustomer.TaxNumber; 
                ctrlAddress.City = theAddress.City;
                ctrlAddress.State = theAddress.State;
                ctrlAddress.PostalCode = theAddress.PostalCode;
                ctrlAddress.County = theAddress.County;
                ctrlAddress.PhoneNumber = theAddress.Phone;
                ctrlAddress.BusinessType = ThisCustomer.BusinessType;
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
            ctrlAddress.HostForm = frmEditAddress;
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
            ctrlAddress.TaxNumberCaption = AppLogic.GetString("address.cs.17", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.BusinessTypeCaption = AppLogic.GetString("address.cs.18", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
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

            ctrlAddress.WithStateCityStatePostalCaption = AppLogic.GetString("createaccount.aspx.31", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
            ctrlAddress.CountyCaption = AppLogic.GetString("createaccount.aspx.32", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

            ctrlAddress.ShowResidenceType = false;
            ctrlAddress.RequireSalutation = false;
            ctrlAddress.ShowFirstName = false;
            ctrlAddress.ShowLastName = false;

            ctrlAddress.RequireEmail = false;
            ctrlAddress.RequirePassword = false;
            ctrlAddress.RequireOkToEmail = false;
            ctrlAddress.RequireOver13 = false;

            if (ThisCustomer.PrimaryBillingAddressID != AddressID)
            {
                ctrlAddress.RequireBusinessType = false;
            }
            else
            {
                ctrlAddress.RequireBusinessType = AppLogic.AppConfigBool("VAT.Enabled") && AppLogic.AppConfigBool("VAT.ShowTaxFieldOnRegistration");

                if (ctrlAddress.RequireBusinessType)
                {
                    using (SqlConnection con = DB.NewSqlConnection())
                    {
                        con.Open();
                        using (IDataReader reader = DB.GetRSFormat(con, "SELECT MessageText FROM SystemMessageManager with (NOLOCK) WHERE LanguageCode = {0} AND MessageCode IN ('LBL0022', 'LBL0023') ORDER BY MessageCode ASC", DB.SQuote(ThisCustomer.LanguageCode)))
                        {
                            if (reader.Read())
                            {
                                ctrlAddress.BusinessTypeWholeSaleDisplayText = DB.RSField(reader, "MessageText");
                            }

                            if (reader.Read())
                            {
                                ctrlAddress.BusinessTypeRetailDisplayText = DB.RSField(reader, "MessageText");
                            }
                        }
                    }
                }
            }
            
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

            ctrlAddress.RequireBusinessTypeSelectErrorMessage = AppLogic.GetString("createaccount.aspx.78", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);

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

        #region SaveAddress

        /// <summary>
        /// Saving/Updating of Editted Address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnSaveAddress_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                ThisCustomer.RequireCustomerRecord();

                Address thisAddress = Address.Get(ThisCustomer, AddressType, AddressID);

                thisAddress.Name = ctrlAddress.AccountName;
                thisAddress.Country = ctrlAddress.CountryCode;
                thisAddress.Address1 = ctrlAddress.Address;
                thisAddress.ResidenceType = ctrlAddress.ResidenceType;
                ThisCustomer.BusinessType = ctrlAddress.BusinessType;
                ThisCustomer.TaxNumber = ctrlAddress.TaxNumber;
                thisAddress.City = ctrlAddress.City;
                thisAddress.State = ctrlAddress.State;
                thisAddress.PostalCode = ctrlAddress.PostalCode;
                thisAddress.County = ctrlAddress.County;
                thisAddress.Phone = ctrlAddress.PhoneNumber;

                Address billingAddress = null;
                Address shippingAddress = null;

                if (AddressType == AddressTypes.Billing)
                {
                    thisAddress.CardName = ctrlAddress.AccountName;
                    this.hidBillCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlBillingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    CountryAddressDTO billCountry = CountryAddressDTO.Find(thisAddress.Country);

                    // Checked to see if Country selected is 'WithState'
                    //if (billCountry.withState)
                    //{
                        billingAddress = thisAddress;
                        this.hidBillCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, billCountry);
                        this.hidBlnWithState.Value = billCountry.withState.ToString();
                    //}
                }
                else
                {
                    this.hidShipCtrl.Value = string.Concat(ctrlAddress.FindControl("WithStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithStatePostalCode").ClientID.ToString(), "*ctrlShippingAddress_WithStateState", "*", ctrlAddress.FindControl("WithoutStateCity").ClientID.ToString(), "*", ctrlAddress.FindControl("WithoutStatePostalCode").ClientID.ToString());
                    CountryAddressDTO shipCountry = CountryAddressDTO.Find(thisAddress.Country);
                    shippingAddress = thisAddress;
                    this.hidShipCheck.Value = CommonLogic.CheckIfAddressIsCorrect(thisAddress, shipCountry);
                    this.hidShpWithState.Value = shipCountry.withState.ToString();
                }

                DisplayErrorIfAny(AddressType);

                // Checked if either Billing Address or Shipping Address is Correct
                if (!string.IsNullOrEmpty(this.hidBillCheck.Value) || !string.IsNullOrEmpty(this.hidShipCheck.Value))
                {
                    // Set the PopUp window to Show Up
                    hidValid.Value = "false";

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

                     

                    // No need for Billing Address if blank
                    if (billingAddress != null)
                    {
                        this.hidBillTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.87", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                    }

                    // No need for Shipping Address if blank
                    if (shippingAddress != null)
                    {
                        this.hidShipTitle.Value = string.Concat(AppLogic.GetString("createaccount.aspx.88", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                    }
                    
                }
                else
                {
                    Address.Update(ThisCustomer, thisAddress);
                    Response.Redirect(String.Format("selectaddress.aspx?Checkout={0}&AddressType={1}", checkOutMode.ToString(), AddressType));
                }
            }
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

            SectionTitle = AppLogic.GetString("editaddress.aspx.1", SkinID, ThisCustomer.LocaleSetting);


            StringBuilder script = new StringBuilder();

            script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   var form = $getElement('{0}');\n", this.frmEditAddress.ClientID);
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

        #endregion
		
		//required for postal ajax call
        protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        {
            // this is a prerequisite as we can't be sure of the ordering of jscripts called, this will be rendered on the <head> section
            writer.WriteLine("<script type=\"text/javascript\" src=\"js/jquery/jquery-ui.min.js\" ></script>");
            writer.WriteLine("<link  type=\"text/css\" rel=\"stylesheet\" href=\"skins/Skin_" + ThisCustomer.SkinID.ToString() + "/jquery-ui.css\" />");
        }

		//Removed since it's already in the header of mobile template
        //protected override void OnRenderHeader(object sender, System.IO.TextWriter writer)
        //{
            // this is a prerequisite as we can't be sure of the ordering of jscripts called, this will be rendered on the <head> section
        //    writer.WriteLine("<script type=\"text/javascript\" src=\"jscripts/core.js\" ></script>");
        //}
        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Scripts.Add(new ScriptReference("js/address_ajax.js"));
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

        #endregion
    }
}




