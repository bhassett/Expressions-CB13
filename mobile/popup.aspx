<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.popup" CodeFile="popup.aspx.cs" %>

<div class="signin_main">
    <div id="pnlContent" runat="server" class="signin_info" >
        <div class="tableHeaderArea" >
            <asp:Label runat="server" ID="lblTopic" ></asp:Label>
        </div>
    </div>
    <div id="pnlNoTopic" runat="server" visible="false">
        <p style="text-align:center;" >
            <asp:Label runat="server" id="lblNoTopicText" ></asp:Label>
        </p>
    </div>
</div>