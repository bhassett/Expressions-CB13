using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InterpriseSuiteEcommerceCommon.Domain.Infrastructure;
using InterpriseSuiteEcommerceCommon;
using InterpriseSuiteEcommerceCommon.Tool;
using InterpriseSuiteEcommerceCommon.Domain.Model;
using InterpriseSuiteEcommerceCommon.Extensions;
using InterpriseSuiteEcommerceCommon.DTO;

public partial class admin_ItemImages : System.Web.UI.Page
{
    #region Initialization

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion

    #region Methods

    public string GetInventoryItemsJSON()
    {
        var items = ServiceFactory.GetInstance<IProductService>().GetInventoryItems();
        var categories = ServiceFactory.GetInstance<IProductService>().GetInventoryCategories();
        foreach (var item in items)
        {
            item.Categories = categories.Where(c => c.ItemCode == item.ItemCode).ToList();
            
            string seName = (item.ItemDescription.IsNullOrEmptyTrimmed()) ? item.ItemName : item.ItemDescription;
            string entityID = item.Counter.ToString();
            item.ItemURL = SE.MakeEntityLink(DomainConstants.EntityProduct, entityID, seName);
        }
        return items.OrderBy(i => i.ItemName).ToList().ToJSON();
    }

    public string GetInventoryItemsWithNoImagesJSON()
    {
        var items = ServiceFactory.GetInstance<IProductService>().GetInventoryItemsWithNoImages();
        return items.OrderBy(i => i.ItemName).ToList().ToJSON();
    }


    public string GetSystemCategoriesJSON()
    {
        var categories = ServiceFactory.GetInstance<IProductService>().GetSystemCategories().OrderBy(c => c.Description).ToList();
        return categories.ToJSON();
    }

    public string GetImageConfigJSON()
    {
        var configs = new List<GlobalConfig>();

        string key = string.Empty;
        string value = string.Empty;

        key = "UseImageResize"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultHeight_icon"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));
        
        key = "DefaultHeight_medium"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultHeight_large"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_icon"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_medium"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        key = "DefaultWidth_large"; value = AppLogic.AppConfig(key);
        configs.Add(new GlobalConfig(key, value.ToLower()));

        return configs.ToJSON();
    }

    #endregion
}