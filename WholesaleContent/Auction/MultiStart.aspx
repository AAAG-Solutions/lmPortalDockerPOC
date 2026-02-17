<%@ Page Title="MultiStart Wholesale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MultiStart.aspx.cs" Inherits="LMWholesale.WholesaleContent.Auction.MultiStart" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Auction/MultiStart.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <style type='text/css'>
        .sectionFieldset legend {border: 0 !important;background-color:transparent;}
        .selectableLegend {border: 0 !important;background-color:transparent;cursor:pointer;}
        .actionBackground {width:auto !important;}
        .sectionDropdown {font-size:15px;}
        #grid #jsGrid .jsgrid {height: 335px;}
        .startWholesaleGrid .jsgrid {height: auto;}
        .jsgrid-grid-body {height: calc(100vh - 470px)}
        .filterTable {display:table;margin:0 auto;table-layout:fixed;width: 750px;}
        .filterButton {width: 100px !important;}
    </style>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>

    <div id="divMultiStart" style="display:table;">
        <div class="MultiAuctionListHeader" style="padding: 10px 30px 10px 55px;">
            <h6 id="breadCrumb" style="font-weight: bold;">Inventory / <div class="dropdownHover">
                <span style="text-decoration: underline">Start Wholesale</span>
                <div class="dropdownHoverContent">
                    <a id="startWholesale" href="/WholesaleContent/Auction/MultiEnd.aspx">End Wholesale</a>
                </div>
                </div>
            </h6>
            <h6>&#60; &#8722; <a class='backBreadcrumb' href="javascript:window.location.href='/WholesaleContent/VehicleManagement.aspx';">Back To Inventory</a></h6>

            <fieldset id="ListingFilters" class="sectionFieldset" style="position:relative;">
                <legend onclick="showHideSection('divFilters', 'ListingFilters');" class="selectableLegend">Listing Filters &#9650;</legend>
                <div id="divFilters" class="filterTable collapseTable" style="display:none;">
                    <div class="filterTable">
                        <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                            <span style="padding:2px;font-weight:bold;text-decoration:underline;text-align:center;font-size:20px;">Inventory Status</span>
                        </div>
                        <div style="display:table-row;">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Available:</div>
                            <div class="ColRowSwap" style="padding:5px;width:20px;"><input ID="StatusAvailable" type="checkbox" runat="server" value="1" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">In-Transit:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="StatusInTransit" type="checkbox" runat="server" value="2" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">&nbsp;</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;">&nbsp;</div>
                        </div>
                    </div>
                    <div class="filterTable">
                        <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                            <span style="padding:2px;font-weight:bold;text-decoration:underline;text-align:center;font-size:20px;">Vehicle Type</span>
                        </div>
                        <div style="display:table-row;">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Dealer Certified:</div>
                            <div class="ColRowSwap" style="padding:5px;width:20px;"><input ID="TypeDealerCertified" type="checkbox" runat="server" value="4" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Manufacturer Certified:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="TypeManufacturerCertified" type="checkbox" runat="server" value="3" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Pre-Owned:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="TypePreOwned" type="checkbox" runat="server" value="2" class="sectionText SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Lot Location</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstLotLocation" runat="server" CssClass="SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Listing Status</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstListingStatus" runat="server" CssClass="SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Inspection Status</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstInspectionStatus" runat="server" CssClass="SingleIndent" /></div>
                        </div>
                    </div>
                    <div style="text-align:center;">
                        <asp:Button ID="FilterGrid" Text="Set Filter" CssClass="actionBackground filterButton" runat="server" OnClientClick="filterChanged(); return false;" />
                        <asp:Button ID="ClearFilter" Text="Clear Filter" CssClass="actionBackground filterButton" runat="server" OnClientClick="fnFilterClear(); return false;" />
                    </div>
                </div>
            </fieldset>
            <fieldset id='WholesaleSetting' class='sectionFieldset' style='position:relative;'>
                <legend onclick="showHideSection('divSettings', 'WholesaleSetting');" class="selectableLegend">Wholesale Settings &#9650;</legend>
                <div id="divSettings" class="collapseTable" style='display:table;margin: 0 auto;'>
                    <div class='singleRow'>
                        <div class='centerCell'><label for='StartDate' style='font-weight:bold'>Start Date:&nbsp;</label></div>
                        <div class='centerContent'><input type="date" id='StartDate'/></div>
                        <div class='centerCell' style='width:50px;'></div>
                        <div class='centerCell'><label for='EndDate' style='font-weight:bold'>End Date:&nbsp;</label></div>
                        <div class='centerContent'><input type="date" id='EndDate'/></div>
                    </div>
                    <div class='singleRow'>
                        <div class='centerCell'><label for='ListingType' style='font-weight:bold'>Listing Type:&nbsp;</label></div>
                        <div class='centerContent'><asp:DropDownList ID="lstType" runat="server" CssClass="sectionDropdown" DataValueField="Filter" AutoPostBack="false" onchange="ListingTypeChange();"/></div>
                        <div class='centerCell' style='width:50px;'></div>
                        <div class='centerCell'><label for='ListingCat' style='font-weight:bold'>Listing Category:&nbsp;</label></div>
                        <div class='centerContent'><asp:DropDownList ID="lstCategory" runat="server" CssClass="sectionDropdown" DataValueField="Filter" AutoPostBack="false" /></div>
                    </div>
                    <div class='singleRow'>
                        <div class='centerCell'><label for='RelistCount' style='font-weight:bold'>Relist Count:&nbsp;</label></div>
                        <div class='centerContent'><input type="text" id='RelistCount' runat="server"/></div>
                        <div class='centerCell' style='width:50px;'></div>
                        <div class='centerCell' style="display:none;"><label for='ArbPledge' style='font-weight:bold;'>Limited Arbitration Powertrain Pledge:&nbsp;</label></div>
                        <div class='centerContent' style="display:none;"><input id='ArbPledge' type='checkbox' runat='server' style='font-weight:bold;'/>&nbsp;(SmartAuction Only)</div>
                    </div>
                    <div id="chkForce" runat="server" class='singleRow' style="display:none;">
                        <div class='centerCell'><label for="chkForceWholesalePrice" style="font-weight:bold;">Force Wholesale Pricing:&nbsp;</label></div>
                        <div class='centerContent'><input id='chkForceWholesalePrice' type='checkbox' runat='server' style='font-weight:bold;' onclick="chkForcePricing();"/></div>
                    </div>
                </div>
            </fieldset>
            <fieldset id='VehicleListings' class='sectionFieldset listingsSection' style='position:relative;'>
                <legend onclick="showHideSection('divISQM', 'VehicleListings');" class="selectableLegend">Inventory Search/Quick Management &#9660;</legend>
                <div id="divISQM" class="collapseTable" style="display:none;margin: 0 auto;">
                    <div style='display:table;margin: 0 auto;'>
                        <div class='singleRow'>
                            <div class='centerCell'><label for='VinSearch' style='font-weight:bold'>VIN Search:&nbsp;</label></div>
                            <div class='centerContent'><input type="text" id='VinSearch'/>&nbsp;<input id="search" type="button" class="actionBackground" value="Search" onclick="VINSearch()"></div>
                            <div class='centerCell' style='width:50px;'></div>
                            <div class='centerCell'><label for='QuickSelect' style='font-weight:bold'>Quick Select:&nbsp;</label></div>
                            <div class='centerContent'><asp:DropDownList ID="lstSelect" runat="server" CssClass="sectionDropdown" DataValueField="Filter" AutoPostBack="false" OnChange="javascript:CheckAuction(this.options[this.selectedIndex].value);return false;"/></div>
                        </div>
                    </div>
                    <div id="adesaMark" style="color:red;text-align-last:center;font-size:14px;">* Vehicles marked 'ADESA Dealer Account' will have the Start Price set equal to the Reserve Price for ADESA listings</div>
                    <div id="OveMark" style="color:red;text-align-last:center;font-size:14px;">* Vehicles marked 'OVE Dealer Account' will have the Start Price set within 16 Bid Increments of the Reserve Price of OVE Listings</div>
                    <div id="ActionMark" style="color:red;text-align-last:center;font-size:14px;">* All Quick Select and MMR Adjustment actions will only effect current page of vehicles</div>
                    <br/>
                    <div style="display:table;margin: 0 auto;">
                        <div class='singleRow'>
                            <div class="centerCell" style="font-weight:bold;">MMR Adjustments:&nbsp;</div>
                            <div class="centerCell"><input id="percent" type="text" style="text-align:center;width:150px;" value="100"/>&nbsp;%&nbsp;</div>
                            <div class="centerCell">&nbsp;+&nbsp;</div>
                            <div class="centerCell">&nbsp;$&nbsp;<input id="dollar" type="text" style="text-align:center;width:150px;" value="0"/>&nbsp;</div>
                            <div class="centerCell">&nbsp;<input runat="server" id="setStart" type="button" class="actionBackground" value="Set Start $" onclick="SetPrice('startprice')"></div>
                            <div class="centerCell">&nbsp;<input runat="server" id="setReserve" type="button" class="actionBackground" value="Set Reserve $" onclick="SetPrice('reserveprice')"></div>
                            <div class="centerCell">&nbsp;<input runat="server" id="setBIN" type="button" class="actionBackground" value="Set Buy Now $" onclick="SetPrice('binprice')"></div>
                        </div>
                    </div>
                    <br/>
                </div>
            </fieldset>
            <div style="text-align-last:center;">
                <input id="startSubmit" type="button" class="actionBackground" value="Submit" onclick="javascript:SubmitToAuctions();return false;">&nbsp;&nbsp;
                <input id="cancel" type="button" class="actionBackground" value="Cancel" onclick="javascript:history.back();return false;">
            </div>
        </div>
    
        <div id="grid" class="startWholesaleGrid hide_scrollbar">
            <div id="jsGrid"></div>
        </div>
    </div>

    <asp:HiddenField ID="MMRMaxThreshold" runat="server"/>
    <asp:HiddenField ID="MMRMinThreshold" runat="server"/>
    <asp:HiddenField ID="BidIncrement" runat="server"/>

    <script type="text/javascript">
        // Set default Start to Today
        var today = new Date();
        today.setHours(0,0,0)
        document.getElementById('StartDate').value = today.toISOString().substring(0, 10);

        // Set default End to 6 days ahead
        today.setDate(today.getDate() + 6);
        document.getElementById('EndDate').value = today.toISOString().substring(0, 10);

        // If the enter key is pressed, we attempt a VIN search of the available listings
        $(document).keypress(function (e) {
            if (e.which == 13) {
                if (document.getElementById("VinSearch") != "")
                    document.getElementById("search").click();
                return false;
            }
        });

        var inputs = ['MainContent_RelistCount', 'percent', 'dollar'];
        inputs.forEach(input => {
            var item = document.getElementById(input);
            item.setAttribute("oninput", "this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\\..*)\\./g, '$1');");
        });

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

</asp:Content>