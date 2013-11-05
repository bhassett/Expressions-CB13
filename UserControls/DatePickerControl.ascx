<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatePickerControl.ascx.cs" Inherits="UserControls_DatePickerControl" %>

<span style="position:relative">
    <asp:TextBox Runat="server" ID="txtDate" AutoPostBack="false" />
    <img src="skins/skin_<%=SkinID%>/images/calendar-icon.png" class="calendar-icon" alt="Select Date" id="img_<%=txtDate.ClientID%>" />
</span>

<script type="text/javascript" >
    $(function () {
        $('#' + '<%=txtDate.ClientID%>').datepicker({ changeYear: true });

        $("#img_<%=txtDate.ClientID%>").click(function () {
            $('#' + '<%=txtDate.ClientID%>').datepicker('show');
        });

        var showIcon = <%= ShowCalendarIcon.ToString().ToLower() %>;
        if (showIcon) {
            $("#img_<%=txtDate.ClientID%>").show();
        }
        else {
            $("#img_<%=txtDate.ClientID%>").hide();
        }
    });
</script>