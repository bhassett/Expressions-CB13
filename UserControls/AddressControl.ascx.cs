using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon;

public partial class UserControls_AddressControl : System.Web.UI.UserControl
{
    public bool showResidenceTypes { get; set; }
    public bool showBusinessTypes { get; set; }
    public bool hideStreetAddress { get; set; }
    public bool hideCounty { get; set; }
    public int countryWidthAdjustment { get; set; }
    public string IdPrefix { get; set; }
    public string residenceTypeSelected { get; set; }
    public string countrySelected { get; set; }
    public string street
    {
        get
        {
            return txtStreet.Text;
        }
        set
        {
            txtStreet.Text = value;
        }
    }

   public string country{

       get
       {
           return drpCountry.Text;
       }
       set
       {
           drpCountry.Text = value;
       }

   }

   public string postal
   {
       get
       {
           return txtPostal.Text;
       }
       set
       {
           txtPostal.Text = value;
       }
   }

   public string city
   {
       get
       {
           return txtCity.Text;
       }
       set
       {
           txtCity.Text = value;
            
       }
   }

   public string state
   {
       get
       {
           return txtState.Text;
       }
       set
       {
           txtState.Text = value;
       }
   }

   public string county
   {
       get
       {
           return txtCounty.Text;
       }
       set
       {
           txtCounty.Text = value;
       }
   }

   public string residenceType
   {
       get
       {
           return drpType.Text;
       }
       set
       {
           drpType.Text = value;
       }
   }

   public string businessType
   {
       get
       {
           return drpBusinessType.Text;
       }
       set
       {
           drpBusinessType.Text = value;
       }
   }

   public string taxNumber
   {
       get
       {
           return txtTaxNumber.Text;
       }
       set
       {
           txtTaxNumber.Text = value;
       }
   }


    protected override void OnLoad(EventArgs e)
    {

        base.OnLoad(e);
        Initialize();

    }

    private void Initialize()
    {

        if (!IsPostBack)
        {

            drpCountry.Items.Clear();
            AppLogic.AddItemsToDropDownList(ref drpCountry, "Country", true);

            if (AppLogic.AppConfigBool("Address.ShowCounty") && !hideCounty) pnlCounty.Visible = true;

            if (showResidenceTypes)
            {
                string addressTypesLabel = AppLogic.GetString("selectaddress.aspx.6", Customer.Current.SkinID, Customer.Current.LocaleSetting, true);

                drpType.Items.Add(addressTypesLabel);

                drpType.Items.Add(ResidenceTypes.Residential.ToString());
                drpType.Items.Add(ResidenceTypes.Commercial.ToString());

                drpType.SelectedIndex = drpType.Items.IndexOf(drpType.Items.FindByText(residenceTypeSelected));

            }

            if (showBusinessTypes)
            {
                drpBusinessType.Items.Add(AppLogic.GetString("createaccount.aspx.82", Customer.Current.SkinID, Customer.Current.LocaleSetting, true));
                drpBusinessType.Items.Add(AppLogic.GetString("createaccount.aspx.79", Customer.Current.SkinID, Customer.Current.LocaleSetting, true));
                drpBusinessType.Items.Add(AppLogic.GetString("createaccount.aspx.80", Customer.Current.SkinID, Customer.Current.LocaleSetting, true));
            }

            drpCountry.SelectedIndex = drpCountry.Items.IndexOf(drpCountry.Items.FindByText(countrySelected));
        }

    }
}
