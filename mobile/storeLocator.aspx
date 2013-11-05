<%@ Page Language="C#" AutoEventWireup="true" CodeFile="storeLocator.aspx.cs" Inherits="InterpriseSuiteEcommerce.storeLocator" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />  
  
    <script type="text/javascript" src="http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>
    
    <script type="text/javascript">
        var map = null;
        var currentShape = null;
        var currentPinHTML = "<div class='pinStyle'></div>";
        var notOver = true;

        $(document).ready(function(){
            GetMap();
        });
                
        function GetMap() {
            map = new VEMap('storeLocator');
            mapCenter = new VELatLong(41.8756, -87.9956)
            map.LoadMap();
            map.SetMapStyle(VEMapStyle.Shaded);
            map.SetMouseWheelZoomToCenter(false);
            map.SetCenterAndZoom(mapCenter, 0);
            
            map.AttachEvent("onmouseover", mouseOverHandler);
            map.AttachEvent("onmouseout", mouseOutHandler);
            
            //call the function created server side
            AddPins();            
        }      

        function AddPin(pinLatLong, address) {
            var pin = new VEShape(VEShapeType.Pushpin, pinLatLong);
            pin.SetDescription(address);
            pin.SetCustomIcon(currentPinHTML);
            map.AddShape(pin);

            //Build out sidebar
            var itemHTML = '';
            var currentPinID = pin.GetID();
            currentPinHTML = currentPinHTML.replace('<div ', '<div id=\'sideBarMarker_' + currentPinID + '\' ');
            itemHTML += '<p><span onmouseover="mouseOverSidebarItem(\'' + pin.GetID() + '\');" onmouseout="mouseOutSidebarItem(\'' + pin.GetID() + '\');"  onclick="autoZoom(\'' + pinLatLong.Latitude + '\', \'' + pinLatLong.Longitude + '\');">';
            itemHTML += '<table><tr><td valign="top" >' + currentPinHTML + '</td><td>';
            itemHTML += address;
            itemHTML += '</td></tr><table></span><br/></p>';
            
            document.getElementById('SideBar').innerHTML += itemHTML;
        }

        function mouseOverHandler(e) {
            if (e.elementID && notOver) {
                mouseOverSidebarItem(e.elementID)
                notOver = false;
            }
        }

        function mouseOutHandler(e) {
            if (e.elementID && !notOver) {
                mouseOutSidebarItem(e.elementID)
                notOver = true;
            }
        }

        function mouseOverSidebarItem(pinId) {
            //Update pushpin
            currentShape = map.GetShapeByID(pinId);
            currentIcon = currentShape.GetCustomIcon();
            currentShape.SetCustomIcon(currentIcon.replace('pinStyle', 'pinHoverStyle'));
            map.ShowInfoBox(currentShape);

            //Update side bar icon
            var sideBarIconId = 'sideBarMarker_' + currentShape.GetID();
            document.getElementById(sideBarIconId).className = 'pinHoverStyle';
        }

        function mouseOutSidebarItem(pinId) {
            //Update pushpin
            currentShape = map.GetShapeByID(pinId);
            currentIcon = currentShape.GetCustomIcon();
            currentShape.SetCustomIcon(currentIcon.replace('pinHoverStyle', 'pinStyle'));
            map.HideInfoBox(currentShape);

            //Update side bar icon
            var sideBarIconId = 'sideBarMarker_' + currentShape.GetID();
            document.getElementById(sideBarIconId).className = 'pinStyle';
        }

        function autoZoom(latitude, longitude) {
            map.SetCenterAndZoom(new VELatLong(latitude, longitude), 14);
        }       
    </script>
</head>
   <body>    

    <div class="signin_main">
        
        <div class="signin_info">

            <div class="signin_info_body" style="position:relative;">

                <div id="SideBar" style="height:300px; width:200px;">
                    <asp:Label ID="lblHeader" runat="server"><%=InterpriseSuiteEcommerceCommon.AppLogic.GetString("storelocator.aspx.1", ThisCustomer.SkinID, ThisCustomer.LocaleSetting) %></asp:Label>
                </div>

                <div class="storeWrapper">
                    <div id="storeLocator" class="map clear">
                </div>

            </div>
        </div>

    </div>

    <%--needed for the store locator map to show up correctly--%>  
    <asp:Label ID="lblblank" runat="server" Text="" Visible="false"></asp:Label>
   </body>
</html>