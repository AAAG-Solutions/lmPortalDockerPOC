<%@ Page Title="Vehicle Notes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="ViewNotes.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.ViewNotes" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <!-- Import Page Scripts -->

    <div id="sidebar_menu" class="sidebar hide_scrollbar noPrint"></div>
    <div class="vehicleNotes print hide_scrollbar">
        <h6 class="noPrint" style="font-weight: bold;">Inventory / Vehicle / Vehicle Notes</h6>
        <h6 class="noPrint" id="BackToVehicle" runat="server"></h6>
        <div id="Notes" class="print" style="text-align:-webkit-center;">
            <div class="row" style="width:100%;">
                <div class="col" style="font-weight:bold;">Account Name: <asp:Label ID="DealerName" runat="server" Style="text-decoration:underline"></asp:Label></div>
                <div class="col" style="font-weight:bold;">VIN: <asp:Label ID="VehicleVIN" runat="server" Style="text-decoration:underline"></asp:Label></div>
                <div class="col" style="font-weight:bold;">Stock Number: <asp:Label ID="StockNumber" runat="server" Style="text-decoration:underline"></asp:Label></div>
            </div>
            <div id="VehicleNotes" class="print" runat="server" style="height:calc(100vh - 250px);overflow:overlay;"></div>
            <div class="noPrint" style="padding:2px;text-align-last:center;"><input id="printWindow" type="button" class="actionBackground" value="Print" onclick="javascript: printNotes();return false;"></div>
        </div>
    </div>

    <script type="text/javascript">
        function printNotes() {
            window.print();
            return false;
        }

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

</asp:Content>