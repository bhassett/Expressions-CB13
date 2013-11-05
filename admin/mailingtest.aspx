<%@ Page Language="C#" CodeFile="mailingtest.aspx.cs" Inherits="mailingTest" MasterPageFile="~/admin/default.master" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ OutputCache  Duration="1"  Location="none" %>

<asp:Content ID="Content2" ContentPlaceHolderID="pnlMain" Runat="Server">

    <style type="text/css" >
    
    .SelectedTab 
    {
        background-color:#FFCC00;
        border-left:1px solid #404040;
        border-right:1px solid #404040;
        border-top:1px solid #404040;
        color:#404040;
        font-size:13px;
        font-weight:bold;
        padding:2px 5px;
    }
    
    .DefaultTab 
    {
        background-color:#F2F2F2;
        border-left:1px solid #404040;
        border-right:1px solid #404040;
        border-top:1px solid #404040;
        color:#000080;
        font-size:12px;
        padding:2px 5px;
    }
    
    </style>

    <div style="width: 100%;">
                                        
                        <div id="Div1" style="float: left;">
                            <table border="0" cellpadding="1" cellspacing="0" class="outerTable">
                                <tr>
                                    <td>
                                        <div class="wrapper">
                                            <span style="color:Red;font-weight:bold;"><asp:Literal ID="ltError" runat="server"></asp:Literal></span>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        
                        <div style="clear: both; padding-bottom: 15px;"></div>
                            
                        <div id="content" style="margin-right: 10px;">
                                                            
                            <ComponentArt:TabStrip id="TabStrip1" runat="server" AutoPostBackOnSelect="true" 
                                SiteMapXmlFile="../EntityHelper/MailingTabs.xml" 
                                MultiPageId="MultiPage1"
                                ImagesBaseUrl="images/"
                                DefaultSelectedItemLookId="SelectedTabLook" 
                                DefaultItemLookId="DefaultTabLook"
                                CssClass="TopGroup" >                            
                                <ItemLooks>
                                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="DefaultTabLook" CssClass="DefaultTab">                 </ComponentArt:ItemLook>
                                    <ComponentArt:ItemLook LabelPaddingLeft="5px" LabelPaddingRight="5px" LookId="SelectedTabLook" CssClass="SelectedTab">                </ComponentArt:ItemLook>
                                </ItemLooks>
                            </ComponentArt:TabStrip>
                            
                            <ComponentArt:MultiPage id="MultiPage1" runat="server" cssclass="MultiPage" Height="100%" Width="750">
                                <ComponentArt:PageView runat="server" ID="Pageview1">
                                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                                        <tr>
                                            <td align="left">
                                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                                    <tr>
                                                        <td>
                                                            <div class="wrapperExtraTop">
                                                                <table cellpadding="0" cellspacing="1" border="0">
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Mail Server DNS:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailMe_ServerSimple" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtMailMe_ServerSimple" ErrorMessage="Please enter the DNS of your Mail Server" ID="rfvMailMe_Server" ValidationGroup="MainSimple" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">Mail Server Username (optional):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailServerUserSimple" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">Mail Server Password (optional):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailServerPwdSimple" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Receipt EMail sends from:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtReceiptFromSimple" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtReceiptFromSimple" ErrorMessage="Please enter the email address receipts are sent from" ID="RequiredFieldValidator1" ValidationGroup="MainSimple" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*New Order Notifications send to:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtOrderNotificationToSimple" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtOrderNotificationToSimple" ErrorMessage="Please enter the admin email address new order notifications are sent to" ID="RequiredFieldValidator2" ValidationGroup="MainSimple" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Send Receipt EMails:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:RadioButtonList ID="rblSendReceiptsSimple" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                   <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Send New Order Notifications:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:RadioButtonList ID="rblSendOrderNotificationsSimple" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>                                                                    
                                                                    <tr><td>&nbsp;</td></tr>
                                                                    <tr>
                                                                        <td>
                                                                            <div style="width: 100%; text-align: left; padding-top: 10px;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Button runat="server" ID="btnSendAllSimple" Width="200" CssClass="normalButton" Text="Send Test" ValidationGroup="MainSimple" OnClick="btnSendAllSimple_Click"/>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>                    
                                                        </td>
                                                    </tr>
                                                </table> 
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView runat="server" ID="Pageview2">
                                    <table border="0" cellpadding="1" cellspacing="0" class="outerTable" width="100%">
                                        <tr>
                                            <td align="left">
                                                <table border="0" cellpadding="0" cellspacing="0" class="innerTable">
                                                    <tr>
                                                        <td>
                                                            <div class="wrapperExtraTop">
                                                                <table cellpadding="0" cellspacing="1" border="0">
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Mail Server DNS:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailMe_ServerAdvanced" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtMailMe_ServerAdvanced" ErrorMessage="Please enter the DNS of your Mail Server" ID="RequiredFieldValidator3" ValidationGroup="MainAdvanced" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">Mail Server Username (optional):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailServerUserAdvanced" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">Mail Server Password (optional):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailServerPwdAdvanced" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Mail Server TCP Port:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtMailServerPortAdvanced" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtMailServerPortAdvanced" ErrorMessage="Please enter the TCP Port of your Mail Server (default is 25)." ID="RequiredFieldValidator6" ValidationGroup="MainAdvanced" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Mail Server requires SSL:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:RadioButtonList ID="rblMailServerSSLAdvanced" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                                                <asp:ListItem Value="0" Text="No" Selected="true"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr><td>&nbsp;</td></tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Receipt EMail sends from (EMail Address):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtReceiptFromAdvanced" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtReceiptFromAdvanced" ErrorMessage="Please enter the email address receipts are sent from" ID="RequiredFieldValidator4" ValidationGroup="MainAdvanced" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">Receipt EMail sends from (Name):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtReceiptFromNameAdvanced" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Receipt EMail sends with XmlPackage:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:DropDownList ID="ddXmlPackageReceipt" Width="350" runat="Server"></asp:DropDownList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Send Receipt EMails:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:RadioButtonList ID="rblSendReceiptsAdvanced" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr><td>&nbsp;</td></tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*New Order Notifications send to (EMail Address):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtOrderNotificationToAdvanced" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtOrderNotificationToAdvanced" ErrorMessage="Please enter the admin email address new order notifications are sent to" ID="RequiredFieldValidator5" ValidationGroup="MainAdvanced" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">New Order Notifications send to (Name):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtOrderNotificationToNameAdvanced" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*New Order Notifications send from (EMail Address):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtOrderNotificationFromAdvanced" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ControlToValidate="txtOrderNotificationFromAdvanced" ErrorMessage="Please enter the admin email address new order notifications are sent to" ID="RequiredFieldValidator9" ValidationGroup="MainAdvanced" EnableClientScript="true" SetFocusOnError="true" runat="server" Display="Static">!!</asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">New Order Notifications send from (Name):</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:TextBox Width="350" CssClass="singleNormal" ID="txtOrderNotificationFromNameAdvanced" runat="server"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*New Order Notifications send with XmlPackage:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:DropDownList ID="ddXmlPackageOrderNotifications" Width="350" runat="Server"></asp:DropDownList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" valign="middle">
                                                                            <font class="subTitleSmall">*Send New Order Notifications:</font>
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:RadioButtonList ID="rblSendOrderNotificationsAdvanced" runat="server" RepeatColumns="2" RepeatDirection="horizontal">
                                                                                <asp:ListItem Value="1" Text="Yes" Selected="true"></asp:ListItem>
                                                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr><td>&nbsp;</td></tr>                                                                    
                                                                    <tr><td>&nbsp;</td></tr>
                                                                    <tr>
                                                                        <td>
                                                                            <div style="width: 100%; text-align: left; padding-top: 10px;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Button runat="server" ID="btnSendTestReceiptAdvanced" Width="200" CssClass="normalButton" Text="Send Test Receipt" ValidationGroup="MainAdvanced" OnClick="btnSendTestReceiptAdvanced_Click"/>
                                                                                            </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Button runat="server" ID="btnSendNewOrderNotificationAdvanced"  Width="200" CssClass="normalButton" Text="Send Test Order Notification" ValidationGroup="MainAdvanced" OnClick="btnSendNewOrderNotificationAdvanced_Click"/>
                                                                                            </td>
                                                                                    </tr>                                                                                   
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Button runat="server" ID="btnSendAllAdvanced" Width="200" CssClass="normalButton" Text="Test All" ValidationGroup="MainAdvanced" OnClick="btnSendAllAdvanced_Click"/>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>                    
                                                        </td>
                                                    </tr>
                                                </table> 
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                            </ComponentArt:MultiPage>                                                              
                        </div>
                        
                        <asp:ValidationSummary ID="validationSummarySimple" ValidationGroup="MainSimple" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" runat="server" />                                   
                        <asp:ValidationSummary ID="validationSummaryAdvanced" ValidationGroup="MainAdvanced" DisplayMode="BulletList" ShowMessageBox="true" ShowSummary="false" runat="server" />                    
                                  
                    </div>
</asp:Content>
