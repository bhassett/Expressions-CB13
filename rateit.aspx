<%@ Page language="c#" Inherits="InterpriseSuiteEcommerce.rateit" CodeFile="rateit.aspx.cs" %>
<html>
<head>
    <title>Rate Product</title>
</head>
<body bgcolor="#336699">
   <script language="javascript" type="text/javascript">
        function winClose()
        {
          document.getElementById("rating").selectedIndex = 1;
          self.close();
        }  
   </script> 
    <form runat="server" onsubmit="return FormValidator(this)">
            <table width="100%" cellpadding="5" cellspacing="0" border="0" bgcolor="#336699">
                <tr>
                    <td align="center" valign="middle">
                        <asp:Label ID="rateit_aspx_3" runat="server" style="font-family:arial,helvetica;" Font-Bold="true" ForeColor="white"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="middle">
                        <asp:Label ID="rateit_aspx_4" runat="server" Font-Bold="true" style="font-size:10px;font-family:Verdana,Geneva,Arial,Helvetica;" ForeColor="yellow" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="top">
                        <table width="100%" cellpadding="6" cellspacing="0" border="0">
                            <tr>
                                <td width="10%"></td>
                                <td align="center" valign="top" bgcolor="#FFFFCC"><asp:Label ID="lblProductName" Font-Bold="true" style="font-family:arial,helvetica;" Font-Size="Small" runat="server"></asp:Label></td>
                                <td width="10%"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="middle"><asp:Label ID="rateit_aspx_5" runat="server" Font-Bold="true" Font-Names="arial,helvetica" ForeColor="white"></asp:Label><br /></td>
                </tr>
                <tr>
                    <td align="center" valign="top">
                        <table width="100%" cellpadding="10" cellspacing="0" border="0">
                            <tr>
                                <td width="25%">
                                </td>
                                <td align="center" valign="top" bgcolor="#FFFFFF">
                                    <table border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center" valign="middle"><a href="javascript:void()" onclick="return newRatingEntered(1);"><asp:Image ID="Star1" width="30" height="30" hspace="2" runat="server" BorderWidth="0"/></a></td>
                                            <td align="center" valign="middle"><a href="javascript:void()" onclick="return newRatingEntered(2);"><asp:Image ID="Star2" width="30" height="30" hspace="2" runat="server" BorderWidth="0"/></a></td>
                                            <td align="center" valign="middle"><a href="javascript:void()" onclick="return newRatingEntered(3);"><asp:Image ID="Star3" width="30" height="30" hspace="2" runat="server" BorderWidth="0"/></a></td>
                                            <td align="center" valign="middle"><a href="javascript:void()" onclick="return newRatingEntered(4);"><asp:Image ID="Star4" width="30" height="30" hspace="2" runat="server" BorderWidth="0"/></a></td>
                                            <td align="center" valign="middle"><a href="javascript:void()" onclick="return newRatingEntered(5);"><asp:Image ID="Star5" width="30" height="30" hspace="2" runat="server" BorderWidth="0"/></a></td>
                                        </tr>
                                    </table>
                                    <asp:DropDownList ID="rating" style="font-size:10px;" runat="server" onchange="newRatingEntered(this.value)"></asp:DropDownList><br />
                                    <asp:Label ID="rateit_aspx_12" runat="server" style="font-family:arial,helvetica;" Font-Size="X-Small"></asp:Label>
                                </td>
                                <td width="25%">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="middle"><asp:Label ID="rateit_aspx_13" runat="server" Font-Bold="true" Font-Names="arial,helvetica" ForeColor="white" Font-Size="Small"></asp:Label></td>
                </tr>
                <tr>
                    <td align="center" valign="middle"><asp:TextBox ID="Comments" Columns="40" Rows="6" runat="server" TextMode="MultiLine" ></asp:TextBox></td>
                </tr>
                <tr>
                    <td valign="top" align="center"><asp:Button ID="btnSubmit" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnCancel" CssClass="site-button" OnClientClick="javascript:winClose();" runat="server" /></td>
                </tr>
            </table>
        
    </form>
</body>
</html>