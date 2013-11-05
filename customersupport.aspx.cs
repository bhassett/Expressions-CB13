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

    public partial class CustomerSupport : SkinBase
    {


        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            InitPageContent();

        }

        private void InitPageContent()
        {
            SectionTitle = AppLogic.GetString("customersupport.aspx.49", SkinID, ThisCustomer.LocaleSetting, true);
            CaseFormHelpFulTipsTopic.SetContext = this;

            if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCaseForm")) pnlSecurityCode.Visible = false;

        }

        protected void btnSendCaseForm_Click(object sender, EventArgs e)
        {
            try
            {
                SendCaseForm();
            }
            catch (Exception ex)
            {
                errorSummary.DisplayErrorMessage(ex.Message);
            }
        }

        protected void SendCaseForm()
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
                        city  = _cityState[1].Trim();
                    }
                    else
                    {
                        city  = _cityState[0].Trim();
                        state = string.Empty;
                    }
                    
                }
                else
                {
                    state = AddressControl.state;
                    city = AddressControl.city;
                }

                list.Add(txtContactName.Text);
                list.Add(txtEmail.Text);
                list.Add(txtContactNumber.Text);
                list.Add(AddressControl.country);
                list.Add(state);
                list.Add(AddressControl.postal);
                list.Add(city);
                list.Add(txtSubject.Text);
                list.Add(AddressControl.street);
                list.Add(txtCaseDetails.Text);

                if (AppLogic.AppConfigBool("Address.ShowCounty")) { list.Add(AddressControl.county); }
                else { list.Add(string.Empty); }

                string msg = InterpriseHelper.SaveCaseForm(list);

                if (msg == "True")
                {
                    Response.Redirect("t-CaseFormThankYouPage.aspx");
                }
                else
                {
                    errorSummary.DisplayErrorMessage(msg);
                }
            }
        }

        protected bool IsSecurityCodeGood(string code)
        {

           if (!AppLogic.AppConfigBool("SecurityCodeRequiredOnCaseForm")) return true;

           if (Session["SecurityCode"] != null){

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