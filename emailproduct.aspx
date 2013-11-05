<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.EMailproduct" CodeFile="EMailproduct.aspx.cs" %>
<html>
<head>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
        <div align="center">
            <asp:Label ID="emailproduct_aspx_8" Font-Bold="true" runat="server"></asp:Label> 
            <br/><br/><br/>
            <asp:HyperLink ID="ReturnToProduct" Font-Bold="true" runat="server"></asp:HyperLink>
        </div>
        </asp:Panel>    
        <asp:Panel ID="pnlRequireReg" runat="server" Visible="false">
            <asp:Literal ID="emailproduct_aspx_1" Mode="PassThrough" runat="server"></asp:Literal>
        </asp:Panel>    
        <asp:Panel ID="pnlEmailToFriend" runat="server" Visible="false">
            <table border="0" cellpadding="0" cellspacing="4" width="100%">
                <tr>
                    <td align="center" valign="top" width="40%">
                        <asp:Image ID="imgProduct" runat="server" />
                    </td>
                    <td align="left" valign="top">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="right"><asp:HyperLink ID="ProductNavLink" CssClass="ProductNavLink" runat="server"></asp:HyperLink></td>
                            </tr>
                            <tr valign="top">
                                <td height="10"></td>
                            </tr>
                            <tr valign="top">
                                <td class="ProductNameText"><asp:Label ID="emailproduct_aspx_4" runat="server"></asp:Label><br /><br /></td>
                            </tr>
                            <tr valign="top">
                                <td align="left" valign="top">
                                    <asp:Label ID="emailproduct_aspx_11" runat="server" Font-Bold="true"></asp:Label><br />
                                    <small><asp:Literal ID="emailproduct_aspx_12" runat="server"></asp:Literal></small><br />
                                    <asp:TextBox ID="txtToAddress" runat="server" MaxLength="75" Columns="40" CausesValidation="true"></asp:TextBox><br />
                                    <asp:RequiredFieldValidator id="reqToAddress" ControlToValidate="txtToAddress" runat="server" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regexToAddress" ControlToValidate="txtToAddress" Display="Dynamic" runat="server" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$"></asp:RegularExpressionValidator><br />
                                    <asp:Label ID="emailproduct_aspx_22" runat="server" Font-Bold="true"></asp:Label><br />
                                    <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Columns="40" Rows="7"></asp:TextBox><br/><br/>
                                    <asp:Label ID="emailproduct_aspx_15" runat="server" Font-Bold="true"></asp:Label><br />
                                    <asp:TextBox ID="txtFromAddress" MaxLength="75" runat="server" Columns="40" CausesValidation="true"></asp:TextBox><br />
                                    <asp:RequiredFieldValidator ID="reqFromAddress" ControlToValidate="txtFromAddress" runat="server" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regexFromAddress" ControlToValidate="txtFromAddress" Display="Dynamic" runat="server" ValidationExpression="^[a-zA-Z0-9][-\w\.\+]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$"></asp:RegularExpressionValidator><br />
                                    <asp:Label ID="emailproduct_aspx_18" runat="server" ForeColor="#CC0000"></asp:Label>&#0160;<asp:Label ID="emailproduct_aspx_19" runat="server"></asp:Label><br /><br />
                                    <asp:Button ID="btnSubmit" runat="server" CausesValidation="true" OnClick="btnSubmit_Click" class="site-button content"/>                                
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>    
    </form>
</body>
</html>