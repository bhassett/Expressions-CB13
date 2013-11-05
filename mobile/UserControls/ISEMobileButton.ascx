<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ISEMobileButton.ascx.cs" Inherits="mobile_UserControls_ISEMobileButton" %>

<asp:LinkButton runat="server" ID="lnkMobileButton" CssClass="button">
    <asp:Label runat="server" CssClass="slide" ID="LabelText">
        <asp:Literal ID="litText" runat="server" />
    </asp:Label>
</asp:LinkButton>