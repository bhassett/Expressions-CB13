<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.EMailproduct" CodeFile="EMailproduct.aspx.cs" %>

<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>
<html>
<head>
</head>
<body>
    <form id="form1" runat="server">
    <div class="signin_main">
        <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
            <div class="signin_info removeMarginTop">
                <div class="signin_info_body">
                    <b>
                        <asp:Label ID="emailproduct_aspx_8" runat="server" CssClass="errorLg" />
                    </b>
                    <br />
                    <br />
                    <asp:HyperLink ID="ReturnToProduct" Font-Bold="true" runat="server" CssClass="kitdetaillink"></asp:HyperLink>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlRequireReg" runat="server" Visible="false">
            <asp:Literal ID="emailproduct_aspx_1" Mode="PassThrough" runat="server"></asp:Literal>
        </asp:Panel>
        <div class="signin_info" runat="server" id="divEmailBody">
            <div class="tableHeaderArea">
                <asp:Label ID="emailproduct_aspx_4" runat="server"></asp:Label>
            </div>
            <div class="signin_info_body">
                <asp:Panel ID="pnlEmailToFriend" runat="server" Visible="false">
                    <div class="cart_item">
                        <div class="cart_picture_layout_wrapper">
                            <div class="cart_picture_layout">
                                <asp:Image ID="imgProduct" runat="server" CssClass="mobileimagesize" />
                            </div>
                            <div class="cart_producttitle_layout_70">
                                <br />
                                <b>
                                    <asp:HyperLink ID="ProductNavLink" CssClass="kitdetaillink" runat="server" />
                                </b>
                                <br />
                                <br />
                                <br />
                                <b>
                                    <asp:Label ID="emailproduct_aspx_11" runat="server" /></b>
                                <br />
                                <br />
                                <asp:Literal ID="emailproduct_aspx_12" runat="server" />
                                <br />
                                <br />
                                <asp:RequiredFieldValidator ID="reqToAddress" ControlToValidate="txtToAddress" runat="server" Display="Dynamic" CssClass="errorLg" />
                                <asp:RegularExpressionValidator ID="regexToAddress" ControlToValidate="txtToAddress" Display="Dynamic" runat="server" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$" CssClass="errorLg" />
                                <asp:TextBox ID="txtToAddress" runat="server" MaxLength="75" Columns="40" CausesValidation="true" />
                                <br />
                                <br />
                                <asp:Label ID="emailproduct_aspx_22" runat="server" />
                                <br />
                                <br />
                                <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Columns="40" Rows="7" />
                                <br />
                                <br />
                                <asp:Label ID="emailproduct_aspx_15" runat="server" />
                                <br />
                                <br />
                                <asp:RequiredFieldValidator ID="reqFromAddress" ControlToValidate="txtFromAddress" runat="server" Display="Dynamic" CssClass="errorLg" />
                                <asp:RegularExpressionValidator ID="regexFromAddress" ControlToValidate="txtFromAddress" CssClass="errorLg"  Display="Dynamic" runat="server" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$" />
                                <asp:TextBox ID="txtFromAddress" MaxLength="75" runat="server" Columns="40" CausesValidation="true" />
                                <br />
                                <br />
                                <b>
                                    <asp:Label ID="emailproduct_aspx_18" runat="server" />
                                </b>
                                <br />
                                <br />
                                <asp:Label ID="emailproduct_aspx_19" runat="server" CssClass="notificationText" />
                                <br />
                                <br />
                                <hr />
                                <div class="button_layout">
                                    <uc1:ISEMobileButton ID="btnSubmit" runat="server" CausesValidation="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
