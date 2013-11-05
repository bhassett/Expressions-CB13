using System;
using System.Text;
using System.Linq;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon;


namespace InterpriseSuiteEcommerce
{
    public partial class CaseHistory : SkinBase
    {
        protected override void OnLoad(EventArgs e)
        {
            if (!ThisCustomer.IsRegistered)
            {
                Response.Redirect("customersupport.aspx");
            }

            base.OnLoad(e);
            InitPageContent();
        }

        private void InitPageContent()
        {
            SectionTitle = AppLogic.GetString("customersupport.aspx.49", SkinID, ThisCustomer.LocaleSetting, true);
            if (IsPostBack) return;

            var options = new StringBuilder();

            //Dynamic Adding of Enum items
            var arrActivityStatus = EnumExtensions.GetAllItems<Interprise.Framework.Base.Shared.Enum.ActivityStatus>().Reverse();
            arrActivityStatus.ForEach(item => { options.AppendFormat("<option value='{0}'>{0}</option>", item.ToString()); });
            ActivityStats.Text = string.Format(options.ToString());
        }
     }  
}