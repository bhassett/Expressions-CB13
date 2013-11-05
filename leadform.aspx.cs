using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;

using InterpriseSuiteEcommerceCommon;

namespace InterpriseSuiteEcommerce
{

    public partial class LeadForm : SkinBase
    {

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitPageContent();
        }

        private void InitPageContent()
        {
            SectionTitle = AppLogic.GetString("leadform.aspx.30", SkinID, ThisCustomer.LocaleSetting, true);
            LeadFormHelpfulTipsTopic.SetContext = this;
   
            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnLeadForm")) pnlSecurityCode.Visible = false;
        }

        protected void btnSaveLead_Click(object sender, EventArgs e)
        {
            try
            {   
                SaveLead();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected void SaveLead()
        {

            if (!IsSecurityCodeGood(txtCaptcha.Text))
            {
                errorSummary.DisplayErrorMessage(AppLogic.GetString("createaccount.aspx.126", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
            }
            else
            {
                List<string> list = new List<string>();

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
                        state = string.Empty;
                        city = _cityState[0].Trim();
                    }

                }
                else
                {
                    state = AddressControl.state;
                    city = AddressControl.city;
                }

                string salutation = ProfileControl.salutation;
                if (salutation == AppLogic.GetString("createaccount.aspx.81", AppLogic.GetCurrentSkinID(), Customer.Current.LocaleSetting)) salutation = string.Empty;
  
                list.Add(salutation);
                list.Add(ProfileControl.firstName);
                list.Add(String.Empty);
                list.Add(ProfileControl.lastName);
                list.Add(ProfileControl.email);
                list.Add(AddressControl.street);
                list.Add(AddressControl.country);
                list.Add(state);
                list.Add(city);
                list.Add(ProfileControl.contactNumber);
                list.Add(txtMessage.Text);
                list.Add(ThisCustomer.SkinID.ToString());
                list.Add(ThisCustomer.LocaleSetting);
                list.Add(AddressControl.postal);

                if (AppLogic.AppConfigBool("Address.ShowCounty")) { list.Add(AddressControl.county);}
                else { list.Add(string.Empty); }

                bool emailHasDuplicates = InterpriseHelper.IsLeadEmailDuplicate(list[4]);
                bool leadHasDuplicates  = InterpriseHelper.IsLeadDuplicate(list[1], string.Empty, list[3]);

                if (emailHasDuplicates) errorSummary.DisplayErrorMessage(AppLogic.GetString("leadform.aspx.29", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));
                if (leadHasDuplicates) errorSummary.DisplayErrorMessage(AppLogic.GetString("leadform.aspx.20", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                if (!emailHasDuplicates && !leadHasDuplicates)
                {
                    string status = InterpriseHelper.CreateNewLead(list);
                    Response.Redirect("t-ConnectedBusinessLeadThankYouPage.aspx");
                }

            }
        }

        protected bool IsSecurityCodeGood(string code)
        {

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnLeadForm")) return true;

            if (Session["SecurityCode"] != null)
            {

                string sCode = Session["SecurityCode"].ToString();
                string fCode = code;

                if (AppLogic.AppConfigBool("Captcha.CaseSensitive"))
                {
                    if (fCode.Equals(sCode)) return true;
                }
                else
                {
                    if (fCode.Equals(sCode, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            return true;

        }

    }

}