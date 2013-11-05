using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public interface IPaymentTermResource
{
    string NoPaymentRequiredCaption { get; set; }
    string NameOnCardCaption { get; set; }
    string CardNumberCaption { get; set; }
    string CVVCaption { get; set; }
    string WhatIsCaption { get; set; }
    string CardTypeCaption { get; set; }
    string CardStartDateCaption { get; set; }
    string ExpirationDateCaption { get; set; }
    string CardIssueNumberCaption { get; set; }
    string CardIssueNumberInfoCaption { get; set; }
    string SaveCardAsCaption { get; set; }
    string SaveThisCreditCardInfoCaption { get; set; }
    string PONumberCaption { get; set; }
    string PaypalExternalCaption { get; set; }
}

public class PaymentTermControlResource : IPaymentTermResource
{
    public string NameOnCardCaption { get; set; }
    public string NoPaymentRequiredCaption { get; set; }
    public string CardNumberCaption { get; set; }
    public string CVVCaption { get; set; }
    public string WhatIsCaption { get; set; }
    public string CardTypeCaption { get; set; }
    public string CardStartDateCaption { get; set; }
    public string ExpirationDateCaption { get; set; }
    public string CardIssueNumberCaption { get; set; }
    public string CardIssueNumberInfoCaption { get; set; }
    public string SaveCardAsCaption { get; set; }
    public string SaveThisCreditCardInfoCaption { get; set; }
    public string PONumberCaption { get; set; }
    public string PaypalExternalCaption { get; set; }
}