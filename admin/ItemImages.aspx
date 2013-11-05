<%@ Page Title="" Language="C#" MasterPageFile="~/admin/default.master" AutoEventWireup="true" CodeFile="ItemImages.aspx.cs" Inherits="admin_ItemImages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pnlMain" Runat="Server">
<div class="content-header">
    <div class="title">Item Images</div>
    <div class="tools">
        <div class="input-append menu">
            <input type="text" id="txtSearch" class="search" />
            <input type="button" id="btnSearch" class="btn" value="Search" />
        </div>

        <span class="no-items">
        <input type="checkbox" id="chkShowNoImages" /> Show Items Without Images
        </span>
        
        <select id="ddlCategory"></select>
        
        <input type="file" style="display:none;" class="file-upload" id="fileUpload" name="fileUpload" accept="image/jpeg,image/gif,image/png" />
    </div>
</div>

<div id="content" class="content-body">
</div>
<div id="footer" class="content-footer">
    <div>
        Page size:
        <select id="ddlPageSize">
            <option value="10">10</option>
            <option value="20">20</option>
            <option value="50">50</option>
        </select>
    </div>
    
    <div>
        Page <input type="text" id="txtCurrentPage" value="1" /> of <span id="lblPageTotal"></span>
    </div>
        
    <div>
        <input type="button" id="btnPrevious" value="Previous" class="btn" />
        <input type="button" id="btnNext" value="Next" class="btn" />
    </div>
</div>
<script src="js/jquery.tmpl.min.js" type="text/javascript"></script>
<script src="js/jquery.form.js" type="text/javascript"></script>
<script src="js/cms_editor/cms-editor.js" type="text/javascript"></script>
<script type="text/javascript">
    var templates = { ITEM_TABLE: "item_table", ITEM_TABLE_HEADER: "item_table_header", ITEM_TABLE_ROW: "item_table_row", ITEM_NOT_FOUND: "item_notfound" };
    var uploadParam = { EntityCode: "", Counter: 0, EntityType: "product", FileName: "", Size: "", SizeValue: 0, UploadType: "", IsApplyToAll: false, IsImageExists: false, ImageSrc: "", IsDeleteAll: false };
    var settings = { NoPicturePath: "images/nopictureicon.gif" };
    var supportedExtensions = ["jpg", "png", "gif"];
    var messages = { FileNotSupported: "File type not supported for upload",
        UploadTooltip: "Other image sizes (e.g. medium, icon, minicart, & mobile) will be auto-generated upon uploading large image. You may turn-off this feature by setting appconfig UseImageResize=false"
    };
    var _items = null; //this will contain unfiltered items
    var _configs = null;

    $(document).ready(function () {
        initialize();
    });
    function initialize() {
        initializeTemplates();
        initializeEvents();

        var categories = $.parseJSON('<%= GetSystemCategoriesJSON() %>');
        var items = $.parseJSON('<%= GetInventoryItemsJSON() %>');
        var configs = $.parseJSON('<%= GetImageConfigJSON() %>');

        _items = items;
        _configs = configs;

        loadItems(items);
        loadCategories(categories);
    }
    function loadItems(items) {
        var paging = itemPaging(items.length);
        
        var divContent = $("#content");
        divContent.empty();
        
        if (items.length > 0) {
            divContent.html(renderTemplate(templates.ITEM_TABLE))
        
            var tblItem = $("#tblItem");
            tblItem.append(renderTemplate(templates.ITEM_TABLE_HEADER));

            //append table row
            $.each(items, function (index, value) {
                var idx = index + 1;
                if (idx >= paging.floor && idx <= paging.ceiling) {
                    tblItem.append(renderTemplate(templates.ITEM_TABLE_ROW, value));
                    setItemImages(value.ItemCode, value.Counter);
                }
            });

            //append table style
            $("#" + tblItem.attr("id") + " tr:even").addClass("gv-altrow");

            InitCMSImageEditor();
            setUploadTooltip();
        }
        else { divContent.html(renderTemplate(templates.ITEM_NOT_FOUND)); }
    }
    function InitCMSImageEditor() {
        CMSImageEditorInit();
    }
    function setUploadTooltip() {
        var useImageResize = false;
        $.each(_configs, function (key, val) {
            if (val.Key.toLowerCase() == "useimageresize" && val.Value.toLowerCase() == "true") { useImageResize = true; }
        });
        var options = { title: messages.UploadTooltip, placement: "left" };
        if (useImageResize) { $('.upload').tooltip(options); }
    }
    function filterItems() {
        var txtSearch = $("#txtSearch");
        var ddlCategory = $("#ddlCategory");

        if (txtSearch.val() != "") { filterItemsByItemName(); }
        else if (ddlCategory.val() != "") { filterItemsByCategory(); }
        else { loadItems(_items); }
    }
    function filterItemsByItemName() {
        var searchKey =  $("#txtSearch").val().toLowerCase();
        var items = new Array();

        //append matching item
        $.each(_items, function (key, value) {
            if (value.ItemName.toLowerCase().indexOf(searchKey) != -1) { items.push(value); }
        });
        loadItems(items);
    }
    function filterItemsByCategory() {
        var searchkey = $("#ddlCategory").val().toLowerCase();
        var items = new Array();

        //append matching item
        $.each(_items, function (key, value) {
            var found = false;
            $.each(value.Categories, function (idx, catval) {
                if (catval.CategoryCode.toLowerCase().indexOf(searchkey) != -1) { found = true; return; }
            });
            if (found) { items.push(value); }
        });
        loadItems(items);
    }
    function filterItemsByNoImage() {
        var items = new Array();

        var noimages = $.parseJSON('<%= GetInventoryItemsWithNoImagesJSON() %>');

        $.each(_items, function (key, value) {
            $.each(noimages, function (index, noimage) {
                if (value.ItemCode == noimage.ItemCode) { items.push(value); }
            });
        });
        loadItems(items);
    }
    function initializeEvents() {
        $("#chkShowNoImages").click(function () {
            resetControls();
            var isChecked = $(this).is(":checked");
            if (isChecked) { filterItemsByNoImage(); }
            else { loadItems(_items); }
        });

        $("#btnSearch").click(function () {
            $("#ddlCategory").val(0);
            $("#txtCurrentPage").val("1");
            filterItemsByItemName();
        });
        $("#txtSearch").keyup(function () {
            if (event.keyCode == 13) { filterItemsByItemName(); }
        });
        $("#ddlCategory").change(function () {
            $("#txtSearch").val("");
            $("#txtCurrentPage").val("1");
            filterItemsByCategory();
        });
        $("#ddlPageSize").change(function () {
            var txtCurrentPage = $("#txtCurrentPage");
            txtCurrentPage.val("1");
            filterItems();
        });
        $("#btnNext").click(function () {
            var txtCurrentPage = $("#txtCurrentPage");
            var currentPage = parseInt(txtCurrentPage.val());
            txtCurrentPage.val(currentPage + 1);
            filterItems();
        });
        $("#btnPrevious").click(function () {
            var txtCurrentPage = $("#txtCurrentPage");
            var currentPage = parseInt(txtCurrentPage.val());
            txtCurrentPage.val(currentPage - 1);
            filterItems();
        });
        $("#txtCurrentPage").keydown(function (event) {
            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
            // Allow: Ctrl+A
            (event.keyCode == 65 && event.ctrlKey === true) ||
            // Allow: home, end, left, right
            (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) { event.preventDefault(); }
            }
        });
        $("#txtCurrentPage").keyup(function (event) {
            // Reload items on enter key
            if (event.keyCode == 13) { filterItems(); }
        });
        $("#tblItem .upload").live("click", function () {
            var itemcode = $(this).parent("td").parent("tr").attr("itemcode");
            var counter = $(this).parent("td").parent("tr").attr("counter");

            uploadParam.EntityCode = itemcode;
            uploadParam.Counter = counter;
            uploadParam.EntityType = "product";
            uploadParam.Size = "large";
            uploadParam.SizeValue = 0;
            uploadParam.UploadType = "Upload";
            uploadParam.IsImageExists = false;
            uploadParam.IsDeleteAll = false;
            uploadParam.IsApplyToAll = true;

            var useImageResize = false;
            $.each(_configs, function (key, val) {
                if (val.Key.toLowerCase() == "useimageresize" && val.Value.toLowerCase() == "true") { useImageResize = true; }
            });

            if (useImageResize) { $("#fileUpload").click(); }
            else {
                var itemContainer = $("#tblItem tr[counter=" + counter + "] td[class='images']");
                var imgFirst = $(itemContainer).children("img:first");
                if (imgFirst.length > 0) { imgFirst.click(); }
                else { $("#fileUpload").click(); }
            }
        });
        $("#fileUpload").live("change", function () {
            var filename = $(this).val().split('\\').pop();
            var fileExtension = filename.substr((filename.lastIndexOf('.') + 1));
            var src = "images/product/" + uploadParam.Size + "/" + filename;

            if ($.inArray(fileExtension, supportedExtensions) < 0) {
                alert(messages.FileNotSupported);
                return;
            }

            uploadParam.FileName = filename;
            uploadParam.ImageSrc = src;

            var form = $("form");
            form.attr("enctype", "multipart/form-data");
            form.submit();
        });
        $("form").on("submit", function (e) {
            e.preventDefault();

            //get form controls value before form submission
            var category = $("#ddlCategory").val();
            var search = $("#txtSearch").val();
            var pageSize = $("#ddlPageSize").val();
            var currentPage = $("#txtCurrentPage").val();

            var option = { url: '../FileUploadHandler.ashx?ec=' + uploadParam.EntityCode +
                                            '&pk=' + uploadParam.Counter +
                                            '&et=' + uploadParam.EntityType +
                                            '&fName=' + uploadParam.FileName +
                                            '&s=' + uploadParam.Size +
                                            '&sv=' + uploadParam.SizeValue +
                                            '&ut=' + uploadParam.UploadType +
                                            '&aa=' + uploadParam.IsApplyToAll +
                                            '&exist=' + uploadParam.IsImageExists +
                                            '&src=' + uploadParam.ImageSrc +
                                            '&da=' + uploadParam.IsDeleteAll,

                type: "post",
                dataType: 'json',
                resetForm: true,
                error: function () { alert("An error occured while uploading image. Please try again."); },
                success: function () {
                    setItemImages(uploadParam.EntityCode, uploadParam.Counter, true);

                    //set form controls value back to original (before form submission)
                    $("#ddlCategory").val(category);
                    $("#txtSearch").val(search);
                    $("#ddlPageSize").val(pageSize);
                    $("#txtCurrentPage").val(currentPage);
                }
            };

            $("form").ajaxSubmit(option);
        });
    }
    function itemPaging(itemCount) {
        var btnNext = $("#btnNext");
        var btnPrevious = $("#btnPrevious");
        var lblPageTotal = $("#lblPageTotal");
        var txtCurrentPage = $("#txtCurrentPage")
        var ddlPageSize = $("#ddlPageSize");

        var pageIndex = txtCurrentPage.val();
        var pageSize = ddlPageSize.val();
        var paging = { floor: 0, ceiling: 0 };

        var totalPage = parseInt(itemCount / pageSize);
        if ((itemCount % pageSize) > 0) { totalPage = totalPage + 1; }
        if (totalPage == 0) { totalPage = 1; }

        paging.floor = ((pageIndex - 1) * pageSize) + 1;
        paging.ceiling = pageIndex * pageSize;

        if (pageIndex == 1) { btnPrevious.prop("disabled", true); }
        else { btnPrevious.prop("disabled", false); }

        if (pageIndex == totalPage) { btnNext.prop("disabled", true); }
        else { btnNext.prop("disabled", false); }

        lblPageTotal.text(totalPage);
        return paging;
    }
    function resetControls() {
        $("#ddlCategory").val("");
        $("#txtSearch").val("");
        $("#ddlPageSize").val("");
        $("#txtCurrentPage").val("1");
    }
    function loadCategories(categories) {
        var ddlCategory = $("#ddlCategory");

        // Default category
        ddlCategory.append($("<option />").val("").html("-- All --"));

        // Append all categories
        $.each(categories, function (index, value) {
            ddlCategory.append($("<option />").val(value.CategoryCode).html(value.Description));
        });
    }
    function initializeTemplates() {
        // Item Templates
        $.template(templates.ITEM_TABLE, "<table id='tblItem' class='gv items'></table>");
        $.template(templates.ITEM_TABLE_HEADER, "<tr class='gv-header'>" +
                                                        "<th>Item Name</th>" +
                                                        "<th>Category</th>" +
                                                        "<th>Images</th>" +
                                                        "<th></th>" +
                                                    "</tr>");
        $.template(templates.ITEM_TABLE_ROW, "<tr itemcode='${ItemCode}' counter='${Counter}' class='gv-row'>" +
                                                    "<td class='itemname'><a href='../${ItemURL}' target='_blank'>${ItemName}</a></td>" +
                                                    "<td class='category'>{{each Categories}}{{if $index > 0}}, {{/if}}${Description}{{/each}}</td>" +
                                                    "<td class='images'></td>" +
                                                    "<td class='commands'><a href='javascript:void(0)' class='upload btn'><i class='icon-upload'></i> Upload</a></td>" +
                                            "</tr>");
        $.template(templates.ITEM_NOT_FOUND, "Not Found");

    }
    function renderTemplate(templateId, data) { return $.tmpl(templateId, data); }
    function setItemImages(itemcode, counter, refreshContainer) {
        var imgContainer = $("#tblItem tr[itemcode='" + itemcode + "'] td[class='images']");
        imgContainer.empty();
        $.ajax({
            type: "POST",
            url: "../ActionService.asmx/GetItemImages",
            data: JSON.stringify({ itemCode: itemcode }),
            dataType: "json",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                var jsonResult = result.d;
                if (jsonResult != '') {
                    var images = $.parseJSON(jsonResult);

                    var idx = 0;
                    $.each(images, function (index, value) {
                        var img = $("<img />").attr("src", "../images/product/icon/" + value.FileName).error(function () {
                            $(this).attr("src", settings.NoPicturePath);
                        }).hide();
                        $(img).attr("alt", "");
                        $(img).attr("data-contentkey", itemcode);
                        $(img).attr("data-contentcounter", counter);
                        $(img).attr("data-contententitytype", "product");
                        $(img).attr("data-contenttype", "image");
                        $(img).attr("data-contentindex", idx);
                        $(img).attr("class", "content");
                        imgContainer.append(img);
                        $(img).fadeIn("slow");

                        idx++;
                    });
                }

                if (refreshContainer) { InitCMSImageEditor(); }
            },
            error: function (result, textStatus, exception) { console.log(exception); }
        });
        
    }
</script>
<script src="../jscripts/core.js" type="text/javascript"></script>
<script src="../jscripts/base_ajax.js" type="text/javascript"></script>
<script src="../jscripts/imagezoom.js" type="text/javascript"></script>
</asp:Content>

