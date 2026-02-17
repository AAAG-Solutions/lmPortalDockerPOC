<%@ Page Title="Vehicle Delete" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.Delete" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .sectionFieldset legend {
            border: 0 !important;
        }
    </style>

    <script type="text/javascript">
        function DASVehicleDelete() {
            var dataIn = {
                kListing: document.getElementById('<%=kListingValue.ClientID %>').textContent
            };

            $.ajax({
                type: "POST",
                url: 'Delete.aspx/VehicleDelete',
                data: JSON.stringify(dataIn),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(XMLHttpRequest.responseJSON.Message);
                },
                complete: function (response) {
                    var r = response.responseJSON.d;
                    if (r.success)
                        window.location.href = `/WholesaleContent/VehicleManagement.aspx`;
                    return false;
                }
            });
        }

        function GoBack() {
            var kListing = document.getElementById('<%=kListingValue.ClientID %>').textContent
            window.location.href = `/WholesaleContent/Vehicle/Update.aspx?kListing=${kListing}`;
        }

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="lblDeleteVehicle" class="addVehicle hide_scrollbar">
        <h6 style="font-weight: bold;">Inventory / Delete Vehicle</h6>
        <fieldset id="DeleteVehicle" class="sectionFieldset">
            <legend>Delete Vehicle</legend>
            <div style="font-weight:bold;text-align:center;">Are you sure you want to remove this vehicle?</div>
            <br/>
                <div style="display:table;margin:auto;">
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">VIN:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputVIN" runat="server" /></div>
                    </div>
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">Stock #:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputStock" runat="server" /></div>
                    </div>
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">Year:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputYear" runat="server" /></div>
                    </div>
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">Make:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputMake" runat="server" /></div>
                    </div>
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">Model:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputModel" runat="server" /></div>
                    </div>
                    <div style="display:table-row">
                        <div style="display:table-cell;text-align:right;font-weight:bold;">Style:&nbsp;</div>
                        <div style="display:table-cell;"><asp:Label ID="InputStyle" runat="server" /></div>
                    </div>
                </div>
            <br/>
            <div style="text-align-last:center;">
                <button class="submitButton" onclick="DASVehicleDelete()">Remove</button>
                <button class="submitButton" onclick="javascript: GoBack();return false;">Cancel</button>
            </div>
        </fieldset>
        <asp:Label ID="kListingValue" runat="server" style="visibility:hidden"/>
    </div>

</asp:Content>