<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.disclaimer" CodeFile="disclaimer.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Site Disclaimer</title>
</head>
<body>
    <form runat="Server" method="POST" id="DisclaimerForm" name="DisclaimerForm">
        <asp:Panel runat="Server" ID="Panel1">
            <div align="center">
                <asp:Literal runat="server" ID="DisclaimerContents"></asp:Literal>
                <br />
                <br />
                <asp:Button ID="AgreeButton" runat="server" Text="I Agree" CausesValidation="False" />&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button
                    ID="DoNotAgreeButton" runat="server" Text="I Do NOT Agree" CausesValidation="False"
                    OnClick="DoNotAgreeButton_Click" /><br />
                <br />
                <asp:Label ID="ReturnURL" runat="server" Text="default.aspx" Visible="False" />
            </div>
        </asp:Panel>
    </form>
</body>
</html>
