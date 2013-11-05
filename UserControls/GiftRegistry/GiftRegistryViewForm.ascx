    <%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftRegistryViewForm.ascx.cs"
    Inherits="UserControls_GiftRegistry_GiftRegistryViewForm" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.Extensions" %>
<%@ Import Namespace="InterpriseSuiteEcommerceCommon.DTO" %>
<div class="gift-registry-viewform">
    <div class="title-container">
        <h1><%= Title %></h1>
        
        <span>
            <%= StartDate.Value.ToShortDateString() %> to <%= EndDate.Value.ToShortDateString() %>
        </span>
        
        <span >
            <%= OwnerFullName %>
        </span> 
    </div>
    <div class="gift-registry-content">
        <div class="guest-message-container">
            <p><%= GuestMessage %></p>
        </div>
        <div class="picture-container">
            <a class="cloud-zoom" href="images/giftregistry/<%= PictureFileName %>" >
                <img src="images/giftregistry/<%= PictureFileName %>" alt="" />
            </a>
        </div>
    </div>

    <script type="text/javascript">

        $(".cloud-zoom").fancybox({
            'titlePosition': 'inside',
            'transitionIn': 'none',
            'transitionOut': 'fade'
        });
        
    </script>

</div>