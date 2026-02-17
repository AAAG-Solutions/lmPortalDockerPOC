<%@ Page Title="MultiEnd Wholesale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MultiEnd.aspx.cs" Inherits="LMWholesale.WholesaleContent.Auction.MultiEnd" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Auction/MultiEnd.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <style type='text/css'>
        .sectionFieldset legend {border: 0 !important;background-color:transparent;}
        .actionBackground {width:auto !important;}
        .sectionDropdown {font-size:15px;}
        .jsgrid-grid-body {height: calc(100vh - 440px);}
        .AuctionHeader{display:table;margin: 0 auto;}
        .PadLeft{padding-left: 2px;}
        .PadLeftLarge{padding-left: 2px;}
        @media(max-width:768px){.AuctionList{display:table;}.AuctionHeader{margin: 0px !important}}
    </style>

    <script type="text/javascript">
        // If the enter key is pressed, we attempt a VIN search of the available listings
        $(document).keypress(function (e) {
            if (e.which == 13) {
                if (document.getElementById("VinSearch").value != "")
                    document.getElementById("search").click();
                return false;
            }
        });

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="lblEndWholesale" class="multiEndWholesale hide_scrollbar">
        <div>
            <h6 id="breadCrumb" style="font-weight: bold;">Inventory / <div class="dropdownHover">
                    <span style="text-decoration: underline">End Wholesale</span>
                    <div class="dropdownHoverContent">
                        <a id="stopWholesale" href="/WholesaleContent/Auction/MultiStart.aspx">Start Wholesale</a>
                    </div>
                </div>
            </h6>
            <h6>&#60; &#8722; <a class='backBreadcrumb' href="javascript:window.location.href='/WholesaleContent/VehicleManagement.aspx';">Back To Inventory</a></h6>

            <div class="MultiAuctionListHeader">
                <fieldset id='WholesaleSetting' class='sectionFieldset' style='position:relative;'>
                    <legend>Inventory Selection</legend>
                    <div class="AuctionHeader">
                        <div class='ColRowSwap'>
                            <div class='ColRowSwapLabel'><label for='VinSearch' style='font-weight:bold'>VIN Search:&nbsp;</label></div>
                            <div class='ColRowSwap'><input type="text" id='VinSearch' class="SingleIndent"/>&nbsp;<input id="search" type="button" class="actionBackground" value="Search" onclick="VINSearch()"></div>
                            <div class='ColRowSwapLabel' style="padding-left: 20px"><label for='QuickSelect' style='font-weight:bold'>Quick Select:&nbsp;</label></div>
                            <div class='ColRowSwap'><asp:DropDownList ID="lstSelect" runat="server" CssClass="sectionDropdown SingleIndent" DataValueField="Filter" AutoPostBack="false" OnChange="javascript:CheckAuction(this.options[this.selectedIndex].value);return false;"/></div>
                        </div>
                    </div>
                    <div id="alDisclosure" class="SmallLeftAlign" style="color:red;text-align:center;font-size:14px;">Any vehicle meeting Auto-Launch criteria will be re-listed to auction automatically</div>
                    <div id="disclosure" class="SmallLeftAlign" style="color:red;text-align:center;font-size:20px;">*** Submitting will only end what is displayed ***<br />*** Please refresh this page after submitting to end remaining listings ***</div>
                    <div id="divMarkUnavail" class="AuctionHeader">
                        <div class="ColRowSwap"><asp:Label ID="lblMarkUnavail" Text="Mark All Selected Unavailable:" runat="server" CssClass="ColRowSwapLabel font-weight-bold" /></div>
                        <div class="ColRowSwap"><asp:CheckBox ID="chkMarkUnavail" runat="server" CssClass="SingleIndent PadLeft" /></div>
                    </div>
                    <div style="text-align-last:center;">
                        <input id="endSubmit" type="button" class="actionBackground" value="Submit" onclick="javascript:RemoveFromMultipleAuctions();return false;">&nbsp;&nbsp;
                        <input id="endCancel" type="button" class="actionBackground" value="Cancel" onclick="javascript:window.location.href='/WholesaleContent/VehicleManagement.aspx';return false;">
                    </div>
                </fieldset>
            </div>
        </div>

        <script type="text/javascript" src="/Scripts/jsGrid.js"></script>
        <div id="grid" class="hide_scrollbar">
            <div id="jsGrid" style="padding: 0px 5px 0px 0px;"></div>
        </div>
    </div>
</asp:Content>
