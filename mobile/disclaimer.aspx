<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.disclaimer" CodeFile="disclaimer.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="ise" TagName="Topic" Src="TopicControl.ascx" %>
<%@ Register Src="UserControls/ISEMobileButton.ascx" TagName="ISEMobileButton" TagPrefix="uc1" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Site Disclaimer</title>
    <link href="skins/Skin_1/portrait.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="signin_main">
        <div class="signin_info">
            <div class="tableHeaderArea">
                <asp:Literal runat="server" ID="litDisclamerHeaderText" Text="Disclamer" ></asp:Literal>
            </div>
            <div class="signin_info_body">
                <form runat="Server" method="POST" id="DisclaimerForm" name="DisclaimerForm">
                <asp:Panel runat="Server" ID="Panel1">
                    <div align="center">
                        <br />
                        <div class="disclamer">
                            <asp:Literal runat="server" ID="DisclaimerContents"></asp:Literal>
                        </div>
                        <br />
                        <div style="text-align: center">
                            <uc1:ISEMobileButton ID="AgreeButton" runat="server" CausesValidation="False" />
                            <uc1:ISEMobileButton ID="DoNotAgreeButton" runat="server" CausesValidation="False" />
                        </div>
                        <br />
                        <br />
                        <asp:Label ID="ReturnURL" runat="server" Text="default.aspx" Visible="False" />
                    </div>
                </asp:Panel>
                </form>
            </div>
        </div>
    </div>
</body>
</html>