<%@ Page Title="Account Marketplace Info" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MarketPlaceInfo.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.MarketPlaceInfo" Async="true" AsyncTimeout="30" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Preferences/MarketPlaceInfo.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/MarketPlaceInfo.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences hide_scrollbar" style="overflow:scroll;">
        <h6 style="font-weight: bold;">Account Preferences / <div class="dropdownHover">
                <span style="text-decoration: underline">Marketplace Info</span>
                <div class="dropdownHoverContent">
                    <a id="gpInfo" href="/WholesaleContent/Preferences/General.aspx">General Preferences</a>
                    <a id="alRules" href="/WholesaleContent/Preferences/AutoLaunchRules.aspx">AutoLaunch Rules</a>
                    <a id="bwRules" href="/WholesaleContent/Preferences/BlackoutWindowRules.aspx">Blackout Window Rules</a>
                </div>
            </div>
        </h6>
        <asp:HiddenField ID="credIdx" runat="server" Value=""/>
        <asp:HiddenField ID="Auction" runat="server" Value=""/>
        <div id="generalSettings">
            <div id="generalWholesaleSettings" style="margin-right:10px;">
                <fieldset id='settings' class='sectionFieldset' style='position: relative;'>
                    <legend>General Wholesale Settings</legend>
                    <div class="marketInfoTbl">
                        <div class="singleRow">
                            <div class="centerCell">Physical Location Indicator:&nbsp;</div>
                            <div class="centerContent"><asp:DropDownList id="PhysicalLocations" runat="server" CssClass="inputStyle"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">Listing Type:&nbsp;</div>
                            <div class="centerContent"><asp:DropDownList id="ListingType" runat="server" CssClass="inputStyle"></asp:DropDownList></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Listing Duration:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="ListingDuration" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">Auto End Other Wholesale On Sale:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="AutoEndWholesale" runat="server"></asp:Checkbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Auto End eBay Listings On Sale:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="AutoEndeBay" runat="server"></asp:Checkbox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"># of Time to Auto Relist Vehicles:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="AutoRelistCount" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Requested Inspection Company:&nbsp;</div>
                            <div class="centerContent"><asp:DropDownList id="InspectionCompany" runat="server" CssClass="inputStyle"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">Pre-populate Pricing on Manual Launch:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="PricingManual" runat="server"></asp:Checkbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Force Wholesale Pricing:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="ForcePrice" runat="server"></asp:Checkbox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">MMR Region Code:&nbsp;</div>
                            <div class="centerContent"><asp:DropDownList id="MMRRegions" runat="server" CssClass="inputStyle"></asp:DropDownList></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Minimum Wholesale Price:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MinWholesale" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">MMR Default Grade:&nbsp;</div>
                            <div class="centerContent"><asp:DropDownList id="DefaultGrade" runat="server" CssClass="inputStyle"></asp:DropDownList></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Minimum MMR Threshold:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MinMMRThreshold" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                        </div>
                        <!-- <div class="singleRow">
                            <div class="centerCell">Maximum % of MMR for Reserve Price:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MaxMMRReserve" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Maximum % of MMR for Buy Now:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MaxMMRBIN" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell">Maximum % of MVG for Reserve Price:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MaxMVGReserve" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Maximum % of MVG for Buy Now:&nbsp;</div>
                            <div class="centerContent"><asp:Textbox id="MaxMVGBIN" runat="server" CssClass="inputStyle"></asp:Textbox></div>
                        </div> -->
                        <div class="singleRow">
                            <div class="centerCell">Industry Grade is AutoGrade:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="IndustryGrade" runat="server"></asp:Checkbox></div>
                            <div style="display:table-cell;width:25px;"></div>
                            <div class="centerCell">Reported Grade is NAAA Grade:&nbsp;</div>
                            <div class="centerContent"><asp:Checkbox id="NAAAGrade" runat="server"></asp:Checkbox></div>
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
        <!-- TODO: Implement condition report default options -->
        <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
        <div id="auctionInfo" style="margin-right:10px;">
            <fieldset id='auctions' class='sectionFieldset' style='position: relative;'>
                <legend>Auction Settings</legend>
                <div class="smallTable" style="display:flex;justify-content:space-evenly;">
                    <div class="ColRowSwap" style="width:33%;">
                        <div class="ColRowSwapLabel" id="divAuctionSelector" style="font-weight:bold;">Auction Selection:&nbsp;</div>
                        <div class="ColRowSwap"><asp:DropDownList ID="lstAuctionSelector" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="ChangeView()"/></div>
                    </div>
                    <div id="AuctionSaves" class="ColRowSwap" style="width:33%;">
                        <div style="margin:0 auto; padding-top:10px;text-align:center;">
                            <input id="ShowSummary" type="button" class="actionBackground headerButton" value="Auctions Summary" onclick="Modal('SummaryWindow', 'show')">
                            <input id="AllAuctionSave" type="button" class="actionBackground headerButton SingleIndent" value="Save All Auctions" onclick="SaveAllAuctions()">
                        </div>
                    </div>
                </div>
                <div id="AucitonInfo" class="preferencesContainerNoHeight" runat="server"></div> 
            </fieldset>
        </div>
        <div id="addEditAuctionCred" class="modalPopup">
            <div id="AuctionCredItems" class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="Modal('addEditAuctionCred', 'close');">&times;</span>
                    <h2 id="HeaderAddEditCred" style="text-align: center;"></h2>
                </div>
                <div class="modalBody" style="overflow:overlay;height:85%;">
                    <fieldset id="auctionCredBody" runat="server" class="sectionFieldset" style="background-color:transparent;">
                        <legend id="auctionLegend" style="color:black;border:0!important;background-color:white !important;"></legend>
                        <!-- OVE -->
                        <div id="OVESection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="OVECredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="OVESellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Buyer Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="OVEBuyerGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Bid Increment:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleBidIncrement" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Facilitated Auction Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleFacilitatedAuctionCode" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Dealer Account:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkOVEDealerAccount" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Phyiscal Location Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="OVEkWholesaleLocationCode" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkOVESuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkOVEAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- SmartAuction -->
                        <div id="SmartAuctionSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="SmartAuctionInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="SmartAuctionCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="SmartAuctionkWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Bid" Value="1" />
                                            <asp:ListItem Text="Bid/Buy" Value="3" />
                                            <asp:ListItem Text="Bid/Offer" Value="4" />
                                            <asp:ListItem Text="Bid/Buy/Offer" Value="5" />
                                        </asp:DropDownList></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="SmartAuctionkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Consignor ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="SmartAuctionSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="SmartAuctionkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Buyer Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="SmartAuctionBuyerGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Location ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="SmartAuctionkWholesaleLocationCode" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="SmartAuctionkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Include Saturday &nbsp;<br/> an Auction Day:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkSmartAuctionAllowSaturdayAuction" type="checkbox" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkSmartAuctionSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkSmartAuctionAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- ADESA -->
                        <div id="ADESASection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ADESAInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESACredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ADESAkWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ADESAkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESASellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ADESAkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Organization Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESAOrganizationName" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Service Provider ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESAServiceProviderID" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>SSO ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESABuyerGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Service Provider Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ADESAServiceProviderName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>CarGroup ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox id="ADESACarGroupID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ADESAkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Dealer Account:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkADESADealerAccount" type="checkbox" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkADESASuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkADESAAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- COPART -->
                        <div id="COPARTSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="COPARTInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="COPARTCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="COPARTkWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Bid" Value="1" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="COPARTkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Company Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="COPARTSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="COPARTkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="COPARTBuyerGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Vender Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="COPARTServiceProviderName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="COPARTkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCOPARTSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCOPARTAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- AuctionEdge -->
                        <div id="AuctionEdgeSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionEdgeInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionEdgeCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="AuctionEdgekWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Bid/Offer" Value="6" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionEdgekWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionEdgeSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionEdgekWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionEdgekContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Facilitated Auction Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionEdgekWholesaleFacilitatedAuctionCode" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionEdgeSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionEdgeAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- ACV Auctions -->
                        <div id="ACVAuctionsSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ACVAuctionsInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ACVAuctionsCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="ACVAuctionskWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Bid" Value="1" />
                                            <asp:ListItem Text="Buy" Value="2" />
                                            <asp:ListItem Text="Bid/Buy" Value="3" />
                                            <asp:ListItem Text="Bid/Offer" Value="4" />
                                            <asp:ListItem Text="Bid/Buy/Offer" Value="5" />
                                            <asp:ListItem Text="Bid/Offer" Value="6" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ACVAuctionskWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="ACVAuctionsSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ACVAuctionskWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="ACVAuctionskContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkACVAuctionsSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkACVAuctionsAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- IAA -->
                        <div id="IAASection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IAAInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="IAACredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="IAAkWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Buy" Value="2" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IAAkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="IAASellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IAAkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IAAkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkIAASuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkIAAAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- Auction Simplified -->
                        <div id="AuctionSimplifiedSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionSimplifiedInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionSimplifiedCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="AuctionSimplifiedkWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Bid" Value="1" />
                                            <asp:ListItem Text="Buy" Value="2" />
                                            <asp:ListItem Text="Bid/Buy" Value="3" />
                                            <asp:ListItem Text="Bid/Offer" Value="4" />
                                            <asp:ListItem Text="Bid/Buy/Offer" Value="5" />
                                            <asp:ListItem Text="Bid/Offer" Value="6" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionSimplifiedkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Location ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionSimplifiedSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Buyer Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionSimplifiedBuyerGroup" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionSimplifiedkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionSimplifiedkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionSimplifiedSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionSimplifiedAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- IAS -->
                        <div id="IASSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="IASCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASkWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="IASSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Facilitated Auction Code:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="IASkWholesaleFacilitatedAuctionCode" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkIASSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkIASAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- Auction OS -->
                        <div id="AuctionOSSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionOSInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionOSCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'>
                                        <asp:DropDownList ID="AuctionOSkWholesaleListingTag" runat="server" CssClass="inputStyle">
                                            <asp:ListItem Text="-- Select a Listing Type --" Value="0" />
                                            <asp:ListItem Text="Buy/Offer" Value="6" />
                                        </asp:DropDownList>
                                    </div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionOSkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="AuctionOSSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionOSkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="AuctionOSkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionOSSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkAuctionOSAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- Carmigo -->
                        <div id="CarmigoSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarmigoInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="CarmigoCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarmigokWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarmigokWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="CarmigoSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarmigokWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarmigokContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCarmigoSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCarmigoAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- CarOffer -->
                        <div id="CarOfferSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarOfferInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="CarOfferCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarOfferkWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarOfferkWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="CarOfferSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarOfferkWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="CarOfferkContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCarOfferSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkCarOfferAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                        <!-- RemarketingPlus -->
                        <div id="RemarketingPlusSection" class="row" style="padding-left:5%; padding-right: 5%; display: none;">
                            <div class='marketInfoTbl'>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Lot Location:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="RemarketingPlusInvLotLocation" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Credential Name:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="RemarketingPlusCredentialName" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Listing Type:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="RemarketingPluskWholesaleListingTag" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Physical Location Indicator:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="RemarketingPluskWholesaleLocationIndicator" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Seller ID:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:TextBox ID="RemarketingPlusSellerID" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Inspection Company:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="RemarketingPluskWholesaleInspectionCompany" runat="server" CssClass="inputStyle" /></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>Contact Group:&nbsp;</div>
                                    <div style='display:table-cell;'><asp:DropDownList ID="RemarketingPluskContactGroup" runat="server" CssClass="inputStyle" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                </div>
                                <div style='display:table-row;'>
                                    <div style='display:table-cell;text-align:right;'>No MMR Validation:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkRemarketingPlusSuppressMMR" type="checkbox" /></div>
                                    <div style='display:table-cell;width:10px;'></div>
                                    <div style='display:table-cell;text-align:right;'>Enable Ad Hoc Pickup:&nbsp;</div>
                                    <div style='display:table-cell;'><input id="chkRemarketingPlusAdhocEnabled" type="checkbox" /></div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <div id="AuctionCredButtons" style="width:100%;text-align-last:center;">
                        <input id="SaveAuctionCreds" type="button" class='actionBackground headerButton' value="Save" onclick="">
                        <input id="SwitchAuctionCreds" type="button" class='actionBackground headerButton' style="display:none;" value="" onclick="">
                        <input id="CancelAuctionCreds" type="button" class="actionBackground headerButton" value="Cancel" onclick="Modal('addEditAuctionCred', 'close');">
                    </div>
                </div>
            </div>
        </div>
        <div id="SummaryWindow" class="modalPopup">
            <div id="SummaryItems" class="modalContent">
                 <div class="modalHeader">
                    <span class="closeModalButton" onclick="Modal('SummaryWindow', 'close'); return false;">&times;</span>
                    <h2 id="HeaderSummary" style="text-align: center;">Summary of Auctions</h2>
                </div>
                <div class="modalBody" style="overflow:overlay;height:85%;">
                    <fieldset id="summaries" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">Auction Summary</legend>
                        <div class="row" style="padding-left:5%; padding-right: 5%">
                            <asp:Panel ID="AuctionSummaries" CssClass="table" runat="server">
                            </asp:Panel>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
    </div>
</asp:Content>