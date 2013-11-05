using System;
using System.Data;
using System.Data.SqlClient;
using InterpriseSuiteEcommerceCommon;
using System.Web.UI;

namespace InterpriseSuiteEcommerce
{
    public partial class leadform : SkinBase
    {
        private string _countryCode;
        private string _salutations;

        public string salutations
        {
            set
            {
                _salutations = value;
            }
            get
            {
                return _salutations;

            }
        }

        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            this._init();

        }

        private void _init()
        {

            LeadFormPageHeader.SetContext        = this;
            LeadFormThankYouPageTopic.SetContext = this;

            this._countryCode = drpLstCountriesLF.SelectedValue.ToString();

            if (!IsPostBack)
            {
                this.populateSalutations();
                this.populateSuffixes();
                this.populateCountries();

                lLocalSettings.Text = this.ThisCustomer.LocaleSetting;
                lSkinId.Text        = this.SkinID.ToString();
            }

            this.btnSubmitLF.Attributes["onClick"] = "return false";
            this.btnSubmitLF.TheButton.Attributes["onClick"] = "return false";
            this.btnSubmitLF.Text = AppLogic.GetString("leadform.aspx.15", ThisCustomer.SkinID, ThisCustomer.LocaleSetting);
        }

        private void populateCountries()
        {
            var con = DB.NewSqlConnection();
            try
            {
                con.Open();
                var reader = DB.GetRSFormat(con, "SELECT sc.CountryCode, sc.IsWithState, sc.IsSearchablePostal, CASE WHEN sci.Country IS NOT NULL THEN 0 ELSE 1 END AS IsHomeCountry FROM SystemCountry sc with (NOLOCK) LEFT OUTER JOIN SystemCompanyInformation sci with (NOLOCK) ON sci.Country = sc.CountryCode WHERE sc.IsActive = 1 AND sc.IsShowOnWeb = 1 ORDER BY IsHomeCountry, sc.CountryCode ASC");

                drpLstCountriesLF.Items.Clear();

                while (reader.Read())
                {
                     string code = DB.RSField(reader, "CountryCode");
                   
                      if (!string.IsNullOrEmpty(code) && code.Length > 0)
                      {
                          drpLstCountriesLF.Items.Add(code.ToString());
                         
                      }
                }

                 drpLstCountriesLF.SelectedValue = this._countryCode;


            }
            catch (Exception ex)
            {
                Response.Write("populateCountries(): " + ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        private void populateSalutations()
        {
            var con = DB.NewSqlConnection();
            try
            {
                con.Open();
                var reader = DB.GetRSFormat(con, "SELECT SalutationDescription FROM SystemSalutation with (NOLOCK) WHERE IsActive = 1");

                drpLstSalutation.Items.Clear();
                drpLstSalutation.Items.Add(AppLogic.GetString("createaccount.aspx.81", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                while (reader.Read())
                {
                   drpLstSalutation.Items.Add(DB.RSField(reader, "SalutationDescription"));
                }
            }
            catch (Exception ex)
            {
                Response.Write("populateSalutations(): " + ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        private void populateSuffixes()
        {
            var con = DB.NewSqlConnection();
            try
            {
                con.Open();
                var reader = DB.GetRSFormat(con, "SELECT SuffixCode FROM SystemSuffix with (NOLOCK) WHERE IsActive = 1");
                
                drpLstSuffix.Items.Clear();
                drpLstSuffix.Items.Add(AppLogic.GetString("createaccount.aspx.81", ThisCustomer.SkinID, ThisCustomer.LocaleSetting));

                while (reader.Read())
                {
                    drpLstSuffix.Items.Add(DB.RSField(reader, "SuffixCode"));
                }
            }
            catch (Exception ex)
            {
                Response.Write("populatedrpLstSuffixes(): " + ex.Message);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

        }

        protected override void RegisterScriptsAndServices(ScriptManager manager)
        {
            manager.Services.Add(new ServiceReference("~/actionservice.asmx"));
        }

    }
}