using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.DTO;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceControls.Validators.Special;
using System.Text;
using System.ComponentModel;

public partial class UserControls_PaymentTermControl : BaseUserControl, IPaymentTermResource
{
    #region Variables

    private const string PAYMENT_TERM_TEMP_ATTRIBUTE = "pr";
    private const string PAYMENT_METHOD_TEMP_ATTRIBUTE = "pm";
    private const string PAYMENT_TERM_REQUIRED_ERROR_MESSAGE = "PaymentTermRequiredErrorMessage";
    private const string NAME_ON_CARD_REQUIRED_ERROR_MESSAGE = "NameOnCardRequiredErrorMessage";
    private const string CARD_NUMBER_REQUIRED_ERROR_MESSAGE = "CardNumberRequiredErrorMessage";
    private const string CVV_REQUIRED_ERROR_MESSAGE = "CVVRequiredErrorMessage";
    private const string CARD_TYPE_INVALID_ERROR_MESSAGE = "CardTypeInvalidErrorMessage";
    private const string START_MONTH_INVALID_ERROR_MESSAGE = "StartMonthInvalidErrorMessage";
    private const string START_YEAR_INVALID_ERROR_MESSAGE = "StartYearInvalidErrorMessage";
    private const string EXPIRATION_MONTH_INVALID_ERROR_MESSAGE = "ExpirationMonthInvalidErrorMessage";
    private const string EXPIRATION_YEAR_INVALID_ERROR_MESSAGE = "ExpirationYearInvalidErrorMessage";
    private const string UNKNOWN_CARD_TYPE_ERROR_MESSAGE = "UnknownCardTypeErrorMessage";
    private const string NO_CARD_NUMBER_PROVIDED_ERROR_MESSAGE = "NoCardNumberProvidedErrorMessage";
    private const string CARD_NUMBER_INVALID_FORMAT_ERROR_MESSAGE = "CardNumberInvalidFormatErrorMessage";
    private const string CARD_NUMBER_INVALID_ERROR_MESSAGE = "CardNumberInvalidErrorMessage";
    private const string STORED_CARD_NUMBER_INVALID_ERROR_MESSAGE = "StoredCardNumberInvalidErrorMessage";
    private const string CARD_NUMBER_INAPPROPRIATE_NUMBER_OF_DIGITS_ERROR_MESSAGE = "CardNumberInAppropriateNumberOfDigitsErrorMessage";
    private const string NO_PAYMENT_REQUIRED = "NoPaymentRequired";
    private const string SHOW_CARD_START_DATE = "ShowCardStartDate";
    private const string TERMS_AND_CONDITIONS_HTML = "TermsAndConditionsHTML";
    private const string REQUIRE_TERMS_AND_CONDITIONS = "RequireTermsAndConditions";
    private const string TERMS_AND_CONDITIONS_PROMPT = "TermsAndConditionsPrompt";
    private IEnumerable<PaymentTermDTO> _termOptions = null;

    #endregion

    #region Properties

    public string NoPaymentRequiredCaption
    {
        get { return litNoPaymentRequired.Text; }
        set { litNoPaymentRequired.Text = value; }
    }
    
    public string NameOnCardCaption
    { 
        get { return litNameOnCard.Text; }
        set { litNameOnCard.Text = value; }
    }

    public string CardNumberCaption
    {
        get { return litCardNumber.Text; }
        set { litCardNumber.Text = value; }
    }

    public string CVVCaption
    {
        get { return litCVV.Text; }
        set { litCVV.Text = value; }
    }

    public string WhatIsCaption
    {
        get { return lnkWhatIsCvv.Text; }
        set { lnkWhatIsCvv.Text = value; }
    }

    public string CardTypeCaption
    {
        get { return litCardType.Text; }
        set { litCardType.Text = value; }
    }

    public string CardStartDateCaption
    {
        get { return litCardStartDate.Text; }
        set { litCardStartDate.Text = value; }
    }

    public string ExpirationDateCaption
    {
        get { return litExpirationDate.Text; }
        set { litExpirationDate.Text = value; }
    }

    public string CardIssueNumberCaption
    {
        get { return litCardIssueNumber.Text; }
        set { litCardIssueNumber.Text = value; }
    }

    public string CardIssueNumberInfoCaption
    {
        get { return litCardIssueNumberInfo.Text; }
        set { litCardIssueNumberInfo.Text = value; }
    }

    public string SaveCardAsCaption
    {
        get { return litSaveCardAs.Text; }
        set { litSaveCardAs.Text = value; }
    }

    public string SaveThisCreditCardInfoCaption
    {
        get { return litSaveThisCreditCardInfo.Text; }
        set { litSaveThisCreditCardInfo.Text = value; }
    }

    public string PONumberCaption
    {
        get { return litPONumber.Text; }
        set { litPONumber.Text = value; }
    }

    public string PaypalExternalCaption
    {
        get { return litPayPalExternal.Text; }
        set { litPayPalExternal.Text = value; }
    }

    public string CardDescription
    {
        get { return cardDescription.Text; }
        set { cardDescription.Text = value; }
    }

    public string NameOnCard
    {
        get { return nameOnCard.Text; }
        set { nameOnCard.Text = value; }
    }

    public string CardNumber
    {
        get { return cardNumber.Text; }
        set { cardNumber.Text = value; }
    }

    public string CVV
    {
        get { return cvv.Text; }
        set { cvv.Text = value; }
    }

    public bool SaveCreditCreditCardInfo
    {
        get { return chkSaveCreditCardInfo.Checked; }
        set { chkSaveCreditCardInfo.Checked = value; }
    }

    public TextBox CardNumberControl
    {
        get { return cardNumber; }
    }

    public TextBox CCVCControl
    {
        get { return cvv; }
    }

    public string CardType
    {
        get
        {
            if (null != cardType.SelectedItem)
            {
                return cardType.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            try
            {
                cardType.SelectedValue = value;
            }
            catch
            {
                cardType.SelectedValue = null;
            }
        }
    }

    public string CardStartMonth
    {
        get
        {
            if (null != startMonth.SelectedItem)
            {
                return startMonth.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            try
            {
                startMonth.SelectedValue = value;
            }
            catch
            {
                startMonth.SelectedValue = null;
            }
        }
    }

    public string CardStartYear
    {
        get
        {
            if (null != startYear.SelectedItem)
            {
                return startYear.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            try
            {
                startYear.SelectedValue = value;
            }
            catch
            {
                startYear.SelectedValue = null;
            }
        }
    }

    public string CardIssueNumber
    {
        get { return cardIssueNumber.Text; }
        set { cardIssueNumber.Text = value; }
    }

    public string CardExpiryMonth
    {
        get
        {
            if (null != expirationMonth.SelectedItem)
            {
                return expirationMonth.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            try
            {
                expirationMonth.SelectedValue = value;
            }
            catch
            {
                expirationMonth.SelectedValue = null;
            }
        }
    }

    public string CardExpiryYear
    {
        get
        {
            if (null != expirationYear.SelectedItem)
            {
                return expirationYear.SelectedValue;
            }
            else
            {
                return String.Empty;
            }
        }
        set
        {
            try
            {
                expirationYear.SelectedValue = value;
            }
            catch
            {
                expirationYear.SelectedValue = null;
            }
        }
    }

    public string PONumber
    {
        get { return poNumber.Text; }
        set { poNumber.Text = value; }
    }

    public IEnumerable CardTypeDataSource
    {
        get { return cardType.DataSource as IEnumerable; }
        set
        {
            cardType.DataSource = value;
            cardType.DataBind();
        }
    }

    public System.Data.DataView CardTypeViewDataSource
    {
        get { return cardType.DataSource as System.Data.DataView; }
        set
        {
            cardType.DataSource = value;
            cardType.DataTextField = "CreditCardTypeDescription";
            cardType.DataValueField = "CreditCardType";
            cardType.DataBind();
        }
    }

    public IEnumerable StartMonthDataSource
    {
        get { return startMonth.DataSource as IEnumerable; }
        set
        {
            startMonth.DataSource = value;
            startMonth.DataBind();
        }
    }

    public IEnumerable StartYearDataSource
    {
        get { return startYear.DataSource as IEnumerable; }
        set
        {
            startYear.DataSource = value;
            startYear.DataBind();
        }
    }

    public IEnumerable ExpiryMonthDataSource
    {
        get { return expirationMonth.DataSource as IEnumerable; }
        set
        {
            expirationMonth.DataSource = value;
            expirationMonth.DataBind();
        }
    }

    public IEnumerable ExpiryYearDataSource
    {
        get { return expirationYear.DataSource as IEnumerable; }
        set
        {
            expirationYear.DataSource = value;
            expirationYear.DataBind();
        }
    }

    public string PaymentTermRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState[PAYMENT_TERM_REQUIRED_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[PAYMENT_TERM_REQUIRED_ERROR_MESSAGE] = value;
        }
    }

    public string NameOnCardRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState[NAME_ON_CARD_REQUIRED_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[NAME_ON_CARD_REQUIRED_ERROR_MESSAGE] = value;
        }
    }

    public string CardNumberRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState[CARD_NUMBER_REQUIRED_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CARD_NUMBER_REQUIRED_ERROR_MESSAGE] = value;
        }
    }

    public string CVVRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState[CVV_REQUIRED_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CVV_REQUIRED_ERROR_MESSAGE] = value;
        }
    }

    public string CardTypeInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[CARD_TYPE_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CARD_TYPE_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string StartMonthInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[START_MONTH_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[START_MONTH_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string StartYearInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[START_YEAR_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[START_YEAR_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string ExpirationMonthInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[EXPIRATION_MONTH_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[EXPIRATION_MONTH_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string ExpirationYearInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[EXPIRATION_YEAR_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[EXPIRATION_YEAR_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string UnknownCardTypeErrorMessage
    {
        get
        {
            object savedValue = ViewState[UNKNOWN_CARD_TYPE_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[UNKNOWN_CARD_TYPE_ERROR_MESSAGE] = value;
        }
    }

    public string NoCardNumberProvidedErrorMessage
    {
        get
        {
            object savedValue = ViewState[NO_CARD_NUMBER_PROVIDED_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[NO_CARD_NUMBER_PROVIDED_ERROR_MESSAGE] = value;
        }
    }

    public string CardNumberInvalidFormatErrorMessage
    {
        get
        {
            object savedValue = ViewState[CARD_NUMBER_INVALID_FORMAT_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CARD_NUMBER_INVALID_FORMAT_ERROR_MESSAGE] = value;
        }
    }

    public string CardNumberInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[CARD_NUMBER_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CARD_NUMBER_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string StoredCardNumberInvalidErrorMessage
    {
        get
        {
            object savedValue = ViewState[STORED_CARD_NUMBER_INVALID_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[STORED_CARD_NUMBER_INVALID_ERROR_MESSAGE] = value;
        }
    }

    public string CardNumberInAppropriateNumberOfDigitsErrorMessage
    {
        get
        {
            object savedValue = ViewState[CARD_NUMBER_INAPPROPRIATE_NUMBER_OF_DIGITS_ERROR_MESSAGE];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[CARD_NUMBER_INAPPROPRIATE_NUMBER_OF_DIGITS_ERROR_MESSAGE] = value;
        }
    }

    public string TermsAndConditionsHTML
    {
        get { return litTermsAndConditionsHTML.Text; }
        set { litTermsAndConditionsHTML.Text = value; }
    }

    public String RequireTermsAndConditionsPrompt
    {
        get
        {
            object savedValue = ViewState[TERMS_AND_CONDITIONS_PROMPT];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState[TERMS_AND_CONDITIONS_PROMPT] = value;
        }
    }

    public bool RequireTermsAndConditions
    {
        get
        {
            object savedValue = ViewState[REQUIRE_TERMS_AND_CONDITIONS];
            return null != savedValue && savedValue is bool && (bool)savedValue;
        }
        set
        {
            ViewState[REQUIRE_TERMS_AND_CONDITIONS] = value;
        }
    }

    public bool ShowCardStarDate
    {
        get
        {
            object savedValue = ViewState[SHOW_CARD_START_DATE];
            return null != savedValue && savedValue is bool && (bool)savedValue;
        }
        set
        {
            ViewState[SHOW_CARD_START_DATE] = value;
        }
    }

    public bool NoPaymentRequired
    {
        get
        {
            object savedValue = ViewState[NO_PAYMENT_REQUIRED];
            return null != savedValue && savedValue is bool && (bool)savedValue;
        }
        set
        {
            ViewState[NO_PAYMENT_REQUIRED] = value;
        }
    }

    public IEnumerable<PaymentTermDTO> PaymentTermOptions
    {
        get { return _termOptions; }
        set
        {
            _termOptions = value;
            rptPaymentOptions.DataSource = value;
            rptPaymentOptions.DataBind();

            DisplayPaymentOptionSubContent(value.FirstOrDefault(m => m.IsSelected == true));

        }
    }

    public string PaymentTerm
    {
        get { return paymentTerm.Value; }
        set { paymentTerm.Value = value; }
    }

    public string PaymentMethod
    {
        get { return paymentMethod.Value; }
        set { paymentMethod.Value = value; }
    }

    public bool IsTokenization
    {
        get
        {

            if (ViewState["IsTokenization"] != null)
            {
                return bool.Parse(ViewState["IsTokenization"].ToString());
            }
            return false;
        }
        set
        {
            ViewState["IsTokenization"] = value;
        }
    }

    public bool IsInOnePageCheckOut
    {
        get
        {
            if (ViewState["IsInOnePageCheckOut"] != null)
            {
                return bool.Parse(ViewState["IsInOnePageCheckOut"].ToString());
            }
            return false;
        }
        set
        {
            ViewState["IsInOnePageCheckOut"] = value;
        }
    }

    public Customer ThisCustomer
    {
        get
        {
            Customer customer = null;
            if (ViewState["ThisCustomer"] != null)
            {
                customer = ViewState["ThisCustomer"] as Customer;
            }
            return customer;
        }
        set
        {
            ViewState["ThisCustomer"] = value;
        }
    }

    public bool ShowPaypalPaymentOption
    {
        get
        {

            if (ViewState["ShowPaypalPaymentOption"] != null)
            {
                return bool.Parse(ViewState["ShowPaypalPaymentOption"].ToString());
            }
            return false;
        }
        set
        {
            ViewState["ShowPaypalPaymentOption"] = value;
        }
    }

    #endregion

    #region Methods

    protected override void Render(HtmlTextWriter writer)
    {
        if (Visible)
        {
            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
            script.Append("$add_windowLoad(\n");
            script.Append(" function() { \n");

            script.AppendFormat("   var reg = ise.Controls.PaymentTermController.registerControl('{0}');\n", this.ClientID);
            script.Append("   if(reg) {\n");
            script.AppendFormat("      reg.IsTokenization = {0}; \n", IsTokenization.ToString().ToLower());
            script.AppendFormat("      reg.IsInOnePageCheckout = {0}; \n", IsInOnePageCheckOut.ToString().ToLower());
            script.AppendFormat("      reg.CvvErrorMessage = '{0}'; \n", AppLogic.GetString("checkout1.aspx.13", Customer.Current.SkinID, Customer.Current.LocaleSetting, true));
            script.AppendFormat("      reg.setNoPaymentRequired({0});\n", this.NoPaymentRequired.ToString().ToLowerInvariant());

            foreach (var options in this.PaymentTermOptions)
            {
                script.AppendFormat("      reg.registerOption(new ise.Controls.PaymentTermOption('{0}', '{1}', '{2}'));\n", this.ClientID + "_" + this.ClientID + "_" + options.Counter, options.PaymentTermCode, options.PaymentMethod);
                if (options.PaymentTermCode == "Credit Card")
                {
                    script.AppendFormat("      reg.registerOption(new ise.Controls.PaymentTermOption('{0}', '{1}', '{2}'));\n", this.ClientID + "_" + this.ClientID + "_" + "PaypalOption", DomainConstants.PAYMENT_METHOD_PAYPALX, DomainConstants.PAYMENT_METHOD_PAYPALX);
                }

            }

            script.AppendLine();

            if (null != this.ErrorSummaryControl)
            {
                script.AppendFormat("      reg.setValidationSummary({0});", this.ErrorSummaryControl.RenderInitialization());
                script.AppendLine();
            }

            script.AppendLine();

            script.AppendFormat("      var evaluateCardDetails = function(){{ return reg.getCurrentOption().getPaymentTerm().toUpperCase() !=  '{0}' && reg.getCurrentOption().getPaymentTerm().toUpperCase() !=  '{1}' && reg.getCurrentOption().getPaymentMethod() !=  '{2}' && reg.getCurrentOption().getPaymentMethod() == '{3}'; }}; \n", "PURCHASE ORDER", "REQUEST QUOTE", DomainConstants.PAYMENT_METHOD_PAYPALX, DomainConstants.PAYMENT_METHOD_CREDITCARD);

            script.AppendLine();

            // register the validators
            List<InputValidator> validators = this.ProvideValidators();
            {
                for (int ctr = 1; ctr <= validators.Count; ctr++)
                {
                    InputValidator current = validators[ctr - 1];
                    if (current.SupportsClientSideValidation)
                    {
                        script.AppendFormat("      var val_{0} = {1};", ctr, current.RenderInitialization());
                        script.AppendLine();
                        script.AppendFormat("      val_{0}.setEvaluationDelegate(evaluateCardDetails);", ctr);
                        script.AppendLine();
                        script.AppendFormat("      reg.registerValidator(val_{0});", ctr);
                        script.AppendLine();
                    }
                }
            }

            script.AppendLine();

            if (this.RequireTermsAndConditions)
            {
                script.AppendFormat("      ise.StringResource.registerString('checkoutpayment.aspx.5', '{0}');\n", this.RequireTermsAndConditionsPrompt);
                script.AppendFormat("      reg.setRequireTermsAndConditions({0});\n", true.ToString().ToLowerInvariant());
            }

            script.Append("   }\n");

            string displayText = Security.HtmlEncode("<img src=" + AppLogic.LocateImageURL("skins/skin_" + Customer.Current.SkinID.ToString() + "/images/verificationnumber.gif") + ">");
            script.AppendFormat("    new ToolTip('{0}', 'cvv2_ToolTip', '{1}');\n", lnkWhatIsCvv.ClientID, displayText);

            script.Append(" }\n");
            script.Append(");\n");
            script.AppendLine();

            script.Append("</script>\n");

            writer.Write(script.ToString());
        }

        base.Render(writer);
    }

    public string CreatePaymentMethodAttribute(PaymentTermDTO dto)
    {
        return String.Format("{0} = '{1}'", PAYMENT_METHOD_TEMP_ATTRIBUTE, dto.PaymentMethod);
    }

    public string CreatePaymentTermAttribute(PaymentTermDTO dto)
    {
        return String.Format("{0} = '{1}'", PAYMENT_TERM_TEMP_ATTRIBUTE, dto.PaymentTermCode);
    }

    private void DisplayPaymentOptionSubContent(PaymentTermDTO dto)
    {
        if (dto == null) return;

        if (!dto.IsSelected) return;

        if (dto.PaymentTermCode.ToUpperInvariant() == "PURCHASE ORDER")
        {
            pnlPONumberInfo.Visible = true;
        }
        else if (dto.PaymentTermCode.ToUpperInvariant() == DomainConstants.PAYMENT_METHOD_PAYPALX)
        {
            pnlPayPalRedirectInfo.Visible = true;
            paymentMethod.Value = DomainConstants.PAYMENT_METHOD_PAYPALX;
        }
        else if (dto.PaymentTermCode.ToUpperInvariant() == DomainConstants.PAYMENT_METHOD_CREDITCARD)
        {
            pnlCreditCardInfo.Visible = true;
            paymentMethod.Value = DomainConstants.PAYMENT_METHOD_CREDITCARD;
        }
    }

    private void InitializeVisibleControls()
    {
        if (this.NoPaymentRequired)
        {
            pnlNoPayment.Visible = true;
            pnlCreditCardInfo.Visible = false;
            pnlPayPalRedirectInfo.Visible = false;
            pnlPaymentTermOptions.Visible = false;
            pnlPONumberInfo.Visible = false;
        }
        else
        {
            if (this.ShowCardStarDate)
            {
                pnlStartDate.Visible = true;
                pnlCardIssueNumber.Visible = true;
                pnlNoPayment.Visible = false;
            }

            if (this.RequireTermsAndConditions)
            {
                pnlTerms.Visible = true;
                pnlNoPayment.Visible = false;
            }
            else
            {
                pnlTerms.Visible = false;
            }

            if (this.IsTokenization)
            {
                pnlTokenization.Visible = true;
                pnlNoPayment.Visible = false;
                if (AppLogic.AppConfigBool("ForceCreditCardInfoSaving"))
                {
                    chkSaveCreditCardInfo.Checked = true;
                    chkSaveCreditCardInfo.Enabled = false;
                }
            }
        }
    }

    private void AttachValidators()
    {
        List<InputValidator> validatorsToAttach = this.ProvideValidators();

        foreach (InputValidator validator in validatorsToAttach)
        {
            this.Controls.Add(validator);
        }
    }

    protected override List<InputValidator> ProvideValidators()
    {
        List<InputValidator> defaultValidators = new List<InputValidator>();
        defaultValidators.Add(MakeRequiredInputValidator(paymentTerm, this.PaymentTermRequiredErrorMessage));

        if (this.PaymentTerm == DomainConstants.PAYMENT_METHOD_CREDITCARD)
        {
            RequiredInputValidator requireNameOnCard = MakeRequiredInputValidator(nameOnCard, this.NameOnCardRequiredErrorMessage);
            RequiredInputValidator requireCardNumber = MakeRequiredInputValidator(cardNumber, this.CardNumberRequiredErrorMessage);
            RequiredInputValidator requireCVV = MakeRequiredInputValidator(cvv, this.CVVRequiredErrorMessage);
            CreditCardNumberByTypeValidator validateCardFormat =
            new CreditCardNumberByTypeValidator(cardNumber,
                cardType,
                this.UnknownCardTypeErrorMessage,
                this.NoCardNumberProvidedErrorMessage,
                this.CardNumberInvalidFormatErrorMessage,
                this.CardNumberInvalidErrorMessage,
                this.CardNumberInAppropriateNumberOfDigitsErrorMessage);

            DropDownValidator validateCardType = new DropDownValidator(cardType, this.CardTypeInvalidErrorMessage);
            DropDownValidator validateExpMonth = new DropDownValidator(expirationMonth, this.ExpirationMonthInvalidErrorMessage);
            DropDownValidator validateExpYear = new DropDownValidator(expirationYear, this.ExpirationYearInvalidErrorMessage);

            requireNameOnCard.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            requireCardNumber.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            validateCardFormat.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            requireCVV.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            validateCardType.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            validateExpMonth.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);
            validateExpYear.Evaluate += new CancelEventHandler(CreditCardField_Evaluate);

            base.HandleValidationErrorEvent(validateCardFormat);
            base.HandleValidationErrorEvent(validateCardType);
            base.HandleValidationErrorEvent(validateExpMonth);
            base.HandleValidationErrorEvent(validateExpYear);

            defaultValidators.Add(requireNameOnCard);
            defaultValidators.Add(requireCardNumber);
            defaultValidators.Add(requireCVV);
            defaultValidators.Add(validateCardFormat);
            defaultValidators.Add(validateExpMonth);
            defaultValidators.Add(validateCardType);
            defaultValidators.Add(validateExpYear);
        }
        return defaultValidators;
    }

    private void CreditCardField_Evaluate(object sender, CancelEventArgs e)
    {
        e.Cancel = this.PaymentMethod != DomainConstants.PAYMENT_METHOD_CREDITCARD ||
                    this.PaymentTerm.Equals("PURCHASE ORDER", StringComparison.InvariantCultureIgnoreCase) ||
                    this.PaymentTerm.Equals("REQUEST QUOTE", StringComparison.InvariantCultureIgnoreCase) ||
                    this.PaymentMethod.Equals(DomainConstants.PAYMENT_METHOD_PAYPALX);
    }

    public void LoadStringResources(IPaymentTermResource resource)
    {
        NoPaymentRequiredCaption = resource.NoPaymentRequiredCaption;
        NameOnCardCaption = resource.NameOnCardCaption;
        CardNumberCaption = resource.CardNumberCaption;
        CVVCaption = resource.CVVCaption;
        WhatIsCaption = resource.WhatIsCaption;
        CardTypeCaption = resource.CardTypeCaption;
        CardStartDateCaption = resource.CardStartDateCaption;
        ExpirationDateCaption = resource.ExpirationDateCaption;
        CardIssueNumberCaption = resource.CardIssueNumberCaption;
        CardIssueNumberInfoCaption = resource.CardIssueNumberInfoCaption;
        SaveCardAsCaption = resource.SaveCardAsCaption;
        SaveThisCreditCardInfoCaption = resource.SaveThisCreditCardInfoCaption;
        PONumberCaption = resource.PONumberCaption;
        PaypalExternalCaption = resource.PaypalExternalCaption;
    }

    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeVisibleControls();
    }

    protected override void OnInit(EventArgs e)
    {
        rptPaymentOptions.ItemDataBound += rptPaymentOptions_ItemDataBound;
        ShowPaypalPaymentOption = true;
        base.OnInit(e);
    }

    void rptPaymentOptions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var dtoPaymentTerm = (e.Item.DataItem as PaymentTermDTO);

            if (dtoPaymentTerm.PaymentTermCode == "Credit Card")
            {
                var labelControl = e.Item.FindControl("litCreditCardImages") as Label;
                if (labelControl != null) { labelControl.Visible = true; }
            }

            if (dtoPaymentTerm.PaymentTermCode == "Credit Card" && AppLogic.AppConfigBool("PayPalCheckout.ShowOnCartPage") && ShowPaypalPaymentOption)
            {
                var rowControl = e.Item.FindControl("paypalRow") as HtmlGenericControl;
                if (rowControl != null) { rowControl.Visible = true; }
            }
        }
    }

    #endregion
}