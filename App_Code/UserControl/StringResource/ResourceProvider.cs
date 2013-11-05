using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterpriseSuiteEcommerceCommon;

/// <summary>
/// Summary description for ResourceProvider
/// </summary>
public static class ResourceProvider
{
    public static PaymentTermControlResource GetPaymentTermControlDefaultResources()
    {
        var resource = new PaymentTermControlResource()
        {
            NameOnCardCaption = AppLogic.GetString("checkoutpayment.aspx.15", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            NoPaymentRequiredCaption = AppLogic.GetString("checkoutpayment.aspx.8", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardNumberCaption = AppLogic.GetString("checkoutpayment.aspx.16", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CVVCaption = AppLogic.GetString("checkoutpayment.aspx.17", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            WhatIsCaption = AppLogic.GetString("checkoutpayment.aspx.23", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardTypeCaption = AppLogic.GetString("checkoutpayment.aspx.18", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardStartDateCaption = AppLogic.GetString("checkoutpayment.aspx.19", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            ExpirationDateCaption = AppLogic.GetString("checkoutpayment.aspx.20", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardIssueNumberCaption = AppLogic.GetString("checkoutpayment.aspx.21", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardIssueNumberInfoCaption = AppLogic.GetString("checkoutpayment.aspx.22", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            SaveCardAsCaption = AppLogic.GetString("checkoutpayment.aspx.13", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            SaveThisCreditCardInfoCaption = AppLogic.GetString("checkoutpayment.aspx.14", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            PONumberCaption = AppLogic.GetString("checkoutpayment.aspx.24", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            PaypalExternalCaption = AppLogic.GetString("checkoutpayment.aspx.25", Customer.Current.SkinID, Customer.Current.LocaleSetting)
        };
        return resource;
    }

    public static PaymentTermControlResource GetMobilePaymentTermControlDefaultResources()
    {
        var resource = new PaymentTermControlResource()
        {
            NameOnCardCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.2", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            NoPaymentRequiredCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.15", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.3", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CVVCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.4", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            WhatIsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.11", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardTypeCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.5", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardStartDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.6", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            ExpirationDateCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.7", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardIssueNumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.8", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            CardIssueNumberInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.9", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            SaveCardAsCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.10", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            SaveThisCreditCardInfoCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.12", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            PONumberCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.13", Customer.Current.SkinID, Customer.Current.LocaleSetting),
            PaypalExternalCaption = AppLogic.GetString("mobile.checkoutpayment.aspx.14", Customer.Current.SkinID, Customer.Current.LocaleSetting)
        };
        return resource;
    }
}