using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceControls.Validators;
using InterpriseSuiteEcommerceCommon.Extensions;
using System.ComponentModel;
using System.Text;

public partial class UserControls_MobileShippingMethodControl : BaseUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Properties

    public string ShippingAddressID
    {
        get
        {
            object addressID = ViewState["ShippingAddressID"];
            if (null == addressID) { return String.Empty; }

            return addressID.ToString();
        }
        set
        {
            ViewState["ShippingAddressID"] = value;
        }
    }

    protected override List<InputValidator> ProvideValidators()
    {
        List<InputValidator> defaultValidators = new List<InputValidator>();

        defaultValidators.Add(MakeRequiredInputValidator(shippingMethod, this.ShippingMethodRequiredErrorMessage));

        return defaultValidators;
    }

    public string ShippingMethodRequiredErrorMessage
    {
        get
        {
            object savedValue = ViewState["ShippingMethodRequiredErrorMessage"];
            if (null == savedValue) { return String.Empty; }

            return savedValue.ToString();
        }
        set
        {
            ViewState["ShippingMethodRequiredErrorMessage"] = value;
        }
    }

    public bool SkipShipping
    {
        get
        {
            object booleanValue = ViewState["SkipShipping"];
            if (null == booleanValue) { return false; }

            return booleanValue is bool && (bool)booleanValue;
        }
        set
        {
            ViewState["SkipShipping"] = value;
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

    public string ShippingMethod
    {
        get { return shippingMethod.Value; }
        set { shippingMethod.Value = value; }
    }

    public string FreightCalculation
    {
        get { return freightCalculation.Value; }
        set { freightCalculation.Value = value; }
    }

    public string Freight
    {
        get { return freight.Value; }
        set { freight.Value = value; }
    }

    public Guid RealTimeRateGUID
    {
        get { return new Guid(realTimeRateGUID.Value); }
        set { realTimeRateGUID.Value = value.ToString(); }
    }

    #endregion

    protected override void Render(HtmlTextWriter writer)
    {
        var script = new StringBuilder();
        script.Append("<script type=\"text/javascript\" language=\"Javascript\" >\n");
        script.Append("$add_windowLoad(\n");
        script.Append(" function() { \n");

        script.AppendFormat("   var reg = ise.Controls.ShippingMethodController.registerControl('{0}');\n", this.ClientID);

        script.Append("   if(reg) {\n");

        if (this.SkipShipping)
        {
            script.AppendFormat("      reg.setIsSkipShipping({0});\n", true.ToString().ToLowerInvariant());
        }

        foreach (var validator in this.ProvideValidators())
        {
            script.AppendFormat("      reg.registerValidator({0});\n", validator.RenderInitialization());
        }

        if (null != this.ErrorSummaryControl)
        {
            script.AppendFormat("      reg.setValidationSummary({0});", this.ErrorSummaryControl.RenderInitialization());
            script.AppendLine();
        }

        script.AppendFormat("      reg.requestShippingMethod('{0}');\n", ShippingAddressID);

        script.Append("   }\n");
        script.Append(" }\n");
        script.Append(");\n");
        script.Append("</script>\n");


        writer.Write(script);
        base.Render(writer);
    }
}