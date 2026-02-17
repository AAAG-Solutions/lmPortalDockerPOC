<%@ Page Title="Vehicle Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VehicleManagement.aspx.cs" Inherits="LMWholesale.VehicleManagement" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/VehicleManagement.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/VehicleManagement.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
 
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="vehicleActionsBar">
        <div id="VehicleMarketingActionBar" runat="server" class="row d-sm-flex col-sm-12 VehicleMarketingActionBar">
            <div class="divSearch">
                <asp:TextBox ID="txtSearch" AutoPostBack="false" runat="server" CssClass="txtSearch" placeholder="Search Vehicles" ></asp:TextBox>
            </div>
            <div ID="vehicleActionBar" class="VehicleMarketingButtons">
                <a class='smallHide' ID="MStart" runat="server" href="/WholesaleContent/Auction/MultiStart.aspx"><img src="/Images/fa-icons/calendar-plus.svg" class="mdIcon" title="Multi Start Wholesale Auction" /></a>
                <a class='smallHide' ID="MEnd" runat="server" href="/WholesaleContent/Auction/MultiEnd.aspx"><img src="/Images/fa-icons/calendar-minus.svg" class="mdIcon" title="Multi End Wholesale Auction" /></a>
                <a id="ImportVehicles" href="/WholesaleContent/ImportInventory.aspx"><img src="/Images/fa-icons/file-upload.svg" class="mdIcon" title="Import Vehicles"/></a>
                <asp:Button ID="exportBtn" runat="server" OnClick="ExportInventory" style="display:none;"/>
                <img id="ExportVehicles" src="/Images/fa-icons/file-download.svg" class="mdIcon exportInventory" title="Export Vehicles" OnClick="excelDownload(); return false;"/>
                <a id="AddVehicle" href="/WholesaleContent/Vehicle/Add.aspx"><img src="/Images/fa-icons/plus-circle.svg" class="mdIcon" title="Add Vehicle" /></a>
                <a class='smallHide' href="/WholesaleContent/Vehicle/Search.aspx"><img src="/Images/fa-icons/search.svg" class="mdIcon" title="Vehicle Search" /></a>
                <img ID="filterVehicle" runat='server' src="/Images/fa-icons/filter.svg" class="mdIcon filterVehicles" title="Filter Vehicles" OnClick="javascript:openFilters();return false;"/>
            </div>
            <div id="filterTokens" runat="server" class="row filterTokenRow smallHide">
                <b>Selected Filters:&nbsp;</b>
                <div id="tokenAllInventory" runat="server" class="filterToken hideToken">All Inventory Statuses</div>
                <div id="tokenStatusAvailable" runat="server" class="filterToken hideToken">Available</div>
                <div id="tokenStatusUnavailable" runat="server" class="filterToken hideToken">Unavailable</div>
                <div id="tokenStatusSalePending" runat="server" class="filterToken hideToken">Sale Pending</div>
                <div id="tokenStatusInTransit" runat="server" class="filterToken hideToken">In-Transit</div>
                <div id="tokenStatusDemo" runat="server" class="filterToken hideToken">Demo</div>
                <div id="tokenStatusSold" runat="server" class="filterToken hideToken">Sold</div>
                <div id="tokenAllVehicleTypes" runat="server" class="filterToken hideToken">All Vehicle Types</div>
                <div id="tokenTypeDealerCertified" runat="server" class="filterToken hideToken">Dealer Certified</div>
                <div id="tokenTypeManufacturerCertified" runat="server" class="filterToken hideToken">Manufacturer Certified</div>
                <div id="tokenTypePreOwned" runat="server" class="filterToken hideToken">Pre-Owned</div>
                <div id="tokenNoStyle" runat="server" class="filterToken hideToken">No Style</div>
                <div id="tokenNoDescription" runat="server" class="filterToken hideToken">No Description</div>
                <div id="tokenNoPhotos" runat="server" class="filterToken hideToken">No Photos</div>
                <div id="tokenNoInternetPrice" runat="server" class="filterToken hideToken">No Retail Internet Price</div>
                <div id="tokenNoListPrice" runat="server" class="filterToken hideToken">No Retail Listing Price</div>
                <div id="tokenLotLocation" runat="server" class="filterToken"></div>
                <div id="tokenListingStatus" runat="server" class="filterToken"></div>
                <div id="tokenInspectionStatus" runat="server" class="filterToken"></div>
            </div>
        </div>
    </div>
    <div id="filters">
        <div id="filterOptions" class="modalPopup">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="toggleCssClass([['filterOptions','show_display']]); return false;">&times;</span>
                    <h2 style="text-align: center;">Filter Options</h2>
                </div>
                <div class="modalBody">
                    <div class="filterTable">
                        <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                            <span style="padding:2px;font-weight:bold;text-decoration:underline;text-align:center;font-size:20px;">Inventory Status</span>
                        </div>
                        <div style="display:table-row;">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Available:</div>
                            <div class="ColRowSwap" style="padding:5px;width:20px;"><input ID="StatusAvailable" type="checkbox" runat="server" value="1" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Unavailable:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="StatusUnavailable" type="checkbox" runat="server" value="2" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">Sale Pending:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="StatusSalePending" type="checkbox" runat="server" value="6" class="sectionText SingleIndent"  /></div>
                        </div>
                        <div style="display:table-row;">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">In-Transit:</div>
                            <div class="ColRowSwap" style="padding:5px;"><input ID="StatusInTransit" type="checkbox" runat="server" value="9" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Demo:</div>
                            <div class="ColRowSwap" style="padding:2px;"><input ID="StatusDemo" type="checkbox" runat="server" value="4" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Sold:</div>
                            <div class="ColRowSwap" style="padding:2px;"><input ID="StatusSold" type="checkbox" runat="server" value="7" class="sectionText SingleIndent" /></div>
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
                    </div>
                    <div class="filterTable">
                        <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                            <span style="padding:2px;font-weight:bold;text-decoration:underline;text-align:center;font-size:20px;">System Status</span>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">No Style:</div>
                            <div class="ColRowSwap" style="padding:5px;width:20px;"><input ID="NoStyle" type="checkbox" runat="server" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">No Description:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="NoDescription" type="checkbox" runat="server" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">No Photos:</div>
                            <div class="ColRowSwap" style="padding:2px;width:20px;"><input ID="NoPhotos" type="checkbox" runat="server" class="sectionText SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">No Retail List Price:</div>
                            <div class="ColRowSwap" style="padding:5px;"><input ID="NoListPrice" type="checkbox" runat="server" class="sectionText SingleIndent" /></div>
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">No Retail Internet Price:</div>
                            <div class="ColRowSwap" style="padding:2px;"><input ID="NoInternetPrice" type="checkbox" runat="server" class="sectionText SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Lot Location</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstLotLocation" runat="server" CssClass="inputStyle SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Listing Status</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstListingStatus" runat="server" CssClass="inputStyle SingleIndent" /></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;text-align:right;">Inspection Status</div>
                            <div class="ColRowSwap smallHide"></div>
                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;"><asp:DropDownList ID="lstInspectionStatus" runat="server" CssClass="inputStyle SingleIndent" /></div>
                        </div>
                    </div>
                    <br/>
                    <div style="width:100%;text-align:center;">
                        <asp:Button ID='btnFilters' OnClientClick="setAdvancedFilters(); return false;" runat='server' Text='Apply Filters' CssClass='actionBackground FilterButton' Style='width:125px'/>
                        <asp:Button ID='btnClear' OnClientClick="setAdvancedFilters(true); return false;" runat='server' Text='Clear Filters' CssClass='actionBackground FilterButton' Style='width:125px'/>
                        <asp:Button ID='btnDefault' OnClientClick="setAdvancedFilters(false,true); return false;" runat='server' Text='Set Account Default Filters' CssClass='actionBackground FilterButton' Style='width:225px'/>
                        <asp:Button ID='btnSaveFilters' OnClientClick="setAdvancedFilters(false,false,true); return false;" runat='server' Text='Save User Filters' CssClass='actionBackground FilterButton' Style='width:150px'/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="CurrentkListing" runat="server" Value="" />
    <asp:HiddenField ID="SelectedkListing" runat="server" Value="" />
    <script type="text/javascript" src="/Scripts/jsGrid.js"></script>

    <div id="grid" class="vehicleManagement hide_scrollbar">
        <div id="vehicleManagementJSGrid"></div>
        <asp:HiddenField ID="txtVehicle" runat="server" Value="" />
    </div>

    <script type="text/javascript">
        var pause_timeout = null;
        let isMouseOver = false;

        var vinInput = document.getElementById('<%=txtSearch.ClientID %>');
        OnChangePause(vinInput, HandleChangePause, 300);

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>
</asp:Content>