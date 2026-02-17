<%@ Page Title="Account AutoLaunch Rules" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AutoLaunchRules.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.AutoLaunchRules" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Preferences/AutoLaunchRules.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <style type="text/css">
        .alertIcon {
            padding:0 !important;
            display:none;
            filter: invert(12%) sepia(92%) saturate(5782%) hue-rotate(358deg) brightness(100%) contrast(112%);
        }
        .Hide {
            display: none;
        }

        .PricingBox {
            max-width: 240px;
        }

        .ColRowSwapLabel {
            text-align: right;
        }

        .minadjtooltip .tooltiptext {
            visibility: hidden;
            width: 175px;
            margin-left: -125px;
            margin-top: 20px;
            background-color: black;
            color: #fff;
            text-align: center;
            padding: 5px 0;
            border-radius: 6px;
            position: absolute;
            z-index: 1;
        }

        .minadjtooltip:hover .tooltiptext {
          visibility: visible;
        }

        @media(max-width:768px) {
            .submitButton {
                width: 80px !important;
                margin-top: 2px !important;
            }
            
            .preferencesContainer {
                height: calc(100vh - 180px) !important;
            }

            .modalContent {
                position: static !important;
                height: calc(100vh - 75px) !important;
                padding-bottom: 20px !important;
            }

            .modalPopup {
                padding-top: 52px !important;
            }

            .PricingBox {
                max-width: 175px !important;
            }

            .inputStyle {
                max-width: 185px;
            }

            .closeModalButton {
                font-size: 1rem;
            }

            h2 {
                font-size: 1rem;
                margin-bottom: 0px;
            }

            .jsgrid-grid-body {
                height: calc(100vh - 240px);
            }
        }
    </style>

    <!-- Import Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/AutoLaunchRules.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <asp:HiddenField ID="hfMode" runat="server" Value="" />
    <asp:HiddenField ID="hfViewList" runat="server" Value="" />
    <asp:HiddenField ID="InternalFlag" runat="server" Value="1" />

    <asp:HiddenField ID="WholesaleMMR" runat="server" Value="false" />
    <asp:HiddenField ID="MaxMMRThreshold" runat="server" Value="" />
    <asp:HiddenField ID="MinMMRThreshold" runat="server" Value="" />
    <asp:HiddenField id="SimpleRuleBypass" runat="server" Value="false" />

    <div id="sidebar_menu" class="sidebar hide_scrollbar noPrint"></div>
    <div class="dealerPreferences print">
        <h6 class="noPrint" style="font-weight: bold;">Account Preferences / <div class="dropdownHover">
                <span style="text-decoration: underline">AutoLaunch Rules</span>
                <div class="dropdownHoverContent">
                    <a id="gpInfo" href="/WholesaleContent/Preferences/General.aspx">General Preferences</a>
                    <a id="bwRules" href="/WholesaleContent/Preferences/BlackoutWindowRules.aspx">Blackout Window Rules</a>
                    <a id="mpInfo" href="/WholesaleContent/Preferences/MarketPlaceInfo.aspx">Market Place Info</a>
                </div>
            </div>
            <img runat="server" id="noticeIcon" src="/Images/fa-icons/circle-exclamation.svg" class="xtraSmIcon alertIcon" title="Caution: Rules were created by NonInternal Users"/>
        </h6>
        <div class="printOnly">
            <div style="font-weight:bold;">Account Name: <asp:Label ID="DealerName" runat="server" Style="text-decoration:underline"></asp:Label></div>
        </div>

        <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
        <div id="grid" class="preferencesContainer print" style="height: calc(100vh - 160px);">
            <div runat="server" id="simplejsGrid"></div>
            <div runat="server" id="advancedjsGrid"></div>
            <asp:HiddenField ID="txtRule" runat="server" Value="" />
            <div id="ListForm" class="Hide print" runat="server">
            </div>
        </div>
        <div class="noPrint" style='text-align:-webkit-center;'>
            <asp:Button id='AutoLaunchAdd' runat="server" class='submitButton' style='width:150px;' Text='Add' />
            <asp:Button id='AutoLaunchEdit' runat="server" class='submitButton' style='width:150px;' Text='Edit' />
            <asp:Button id='AutoLaunchDelete' runat="server" OnClientClick="javascript: toggleCssClass([['alOptions','show_display']]); return false;" class='submitButton' style='width:150px;' Text='Delete' />
            <asp:Button id='AutoLaunchList' runat="server" OnClientClick="javascript: ListView(); return false;" class='submitButton' style='width:150px;' Text='List' />
            <!--<asp:Button id='AutoLaunchSimple' runat="server" OnClientClick="javascript: simpleToggle(); return false;" class='submitButton' style='width:150px' Text='Simple View' />-->
            <asp:Button id='AutoLaunchPrint' runat="server" OnClientClick="javascript: window.print(); return false;" class='Hide' style='width:150px' Text='Print' />
        </div>
    </div>

    <div id="alPopup">
        <div id="simpleOptions" class="modalPopup" style="padding-top:75px;">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="toggleCssClass([['simpleOptions','show_display']]); MinMaxYearChange(true); return false;">&times;</span>
                    <h2 id="alSimpleTitle" style="text-align: center;">This is a test</h2>
                </div>
                <div class="modalBody hide_scrollbar show_scroll" style="height:calc(100vh - 275px);overflow:scroll;">
                    <fieldset id="simpleFilters" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">AutoLaunch Filters</legend>
                            <div id="simpleFilterTbl" class="alFilterTbl" style="display:flex;flex-direction:row;flex-wrap:wrap;">
                                <div style='flex:1 1 50%;'>
                                    <div style="display:flex;flex-direction:row;">
                                        <div id="Auction" style="flex:1 1 50%;text-align:-webkit-right;padding-right:25px;">
                                            <div style="display:table;">
                                                <div style="display:table-row;">
                                                <div style="font-weight:bold;text-decoration:underline;padding-bottom:10px;" class="ColRowSwap">
                                                    Auctions:
                                                </div>
                                            </div>
                                                <div style="display:table-row;text-align:center;">
                                                <div class="ColRowSwap">
                                                    &nbsp;<input id="CheckAll" type="checkbox" value="0" class="SingleIndent" style="font-weight:bold;" onclick="AuctionCheck(this);">
                                                    &nbsp;<label for="CheckAll" style="font-weight:bold">ALL</label>
                                                </div>
                                            </div>
                                            </div>
                                        </div>
                                        <div id="AuctionSelect" runat="server" style="display:table;"></div>
                                    </div>
                                </div>
                                <div style='flex:1 1 50%;padding-left:25px;'>
                                    <div id="AuctionInfo" style="display:table;">
                                        <div style="display:table-row;">
                                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle Age:</div>
                                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;">
                                                <asp:Textbox ID="ageMin" runat="server" placeholder="Min Age" CssClass="inputStyle" Style="text-align:center;" />
                                                 to
                                                <asp:Textbox ID="ageMax" runat="server" placeholder="Max Age" CssClass="inputStyle" Style="text-align:center;" />
                                            </div>
                                        </div>
                                        <div id="GradeRow" runat="server" style="display:table-row;">
                                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Grade Range:</div>
                                            <div class="ColRowSwap" style="padding:2px;font-weight:bold;">
                                                <asp:DropdownList id="minGrade" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" Style="text-align:center;" />
                                                 to
                                                <asp:DropdownList id="maxGrade" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" Style="text-align:center;" />
                                            </div>
                                        </div>
                                        <div style="display:table-row;">
                                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Listing Type:</div>
                                            <div class="ColRowSwap" style="padding:2px;">
                                                <span ID="AuctionListingType" class="SingleIndent" style="text-align:center;padding:0 20px 0 20px;text-decoration:underline;font-weight:bold;">Buy/Offer ( SmartAuction Bid/Buy/Offer )</span>
                                            </div>
                                        </div>
                                        <div style="display:table-row;">
                                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Arbitration Policy:</div>
                                            <div class="ColRowSwap" style="padding:2px;">
                                                <asp:DropdownList id="simpleLstListingCategory" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false">
                                                    <asp:ListItem Value="0">-- Select Policy --</asp:ListItem>
                                                    <asp:ListItem Value="1">All Standard</asp:ListItem>
                                                    <asp:ListItem Value="2">All As-Is</asp:ListItem>
                                                    <asp:ListItem Value="3">Aggressive</asp:ListItem>
                                                    <asp:ListItem Value="4">Cautious</asp:ListItem>
                                                    <asp:ListItem Value="5">Conventional</asp:ListItem>
                                                    <asp:ListItem Value="6">All Standard with LAPP</asp:ListItem>
                                                    <asp:ListItem Value="7">Aggressive with LAPP</asp:ListItem>
                                                    <asp:ListItem Value="8">Cautious with LAPP</asp:ListItem>
                                                    <asp:ListItem Value="9">Conventional with LAPP</asp:ListItem>
                                                </asp:DropdownList>
                                            </div>
                                        </div>
                                        <div style="display:table-row;">
                                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;"></div>
                                            <div class="ColRowSwap" style="padding:10px 10px 10px 2px;">
                                                <asp:Button ID='simpleTest' OnClientClick="AutoLaunchRuleTest();return false;" runat='server' Text='Test Rule' CssClass='actionBackground' Style='width:125px'/>
                                                &nbsp;<span id="ruleCount" style="padding:2px;font-weight:bold;text-decoration:underline;"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                    </fieldset>
                    <fieldset id="simplePricingFilter" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">Primary Pricing Values</legend>
                            <div id="pricingDisclosure" style="padding:5px;font-weight:bold;text-decoration:underline;color:red;text-align:center;border:double;background:white;display:none;">
                                Please verify that you have the appropriate pricings in your feed. <a id="vm_dealer" href="/WholesaleContent/VehicleManagement.aspx" target="_blank"> Click here to verify in Vehicle Management</a>.
                            </div>
                            <div id="mmrPricingDisclosure" style="padding:5px;font-weight:bold;text-decoration:underline;color:red;text-align:center;display:none;border:double;background:white;">
                                Please be advised MMR pricing can be volatile and cause pricing variations
                            </div>
                            <div id="simplePricingFilterTbl" class="alFilterTbl" style="margin: 0 auto;display:table;table-layout:fixed;margin-top:10px;">
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">Floor Price:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:DropdownList id="adjustment" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" Style="width:100%;" onchange="javascript: toggleMMRDisclosure(this);">
                                            <asp:ListItem Value="0">-- Select Pricing --</asp:ListItem>
                                            <asp:ListItem Value="5">Retail List Price</asp:ListItem>
                                            <asp:ListItem Value="6">Retail Internet Price</asp:ListItem>
                                            <asp:ListItem Value="4">Inventory Cost</asp:ListItem>
                                            <asp:ListItem Value="24">MMR Pricing</asp:ListItem>
                                        </asp:DropdownList>
                                    </div>
                                    <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">Floor Adjustment:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:DropdownList id="adjustmentPrice" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" onchange="javascript: PriceSetNone(this);return false;">
                                            <asp:ListItem Value="0">-- Select Pricing Adjustment --</asp:ListItem>
                                            <asp:ListItem Value="1">Increase +</asp:ListItem>
                                            <asp:ListItem Value="2">Decrease -</asp:ListItem>
                                            <asp:ListItem Value="3">None</asp:ListItem>
                                        </asp:DropdownList>
                                    </div>
                                    <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">Amount:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:Textbox ID="adjustmentDollar" runat="server" placeholder="$" CssClass="inputStyle" Style="text-align:center;" onchange="PricingCheck(this);" />
                                    </div>
                                </div>
                                <div style="display:table-row;text-align:center;">
                                    <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">Buy Now Amount Above Floor:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:DropdownList id="binAdjustment" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" >
                                            <asp:ListItem Value="-1">-- Select Adjustment --</asp:ListItem>
                                            <asp:ListItem Value="0">$0</asp:ListItem>
                                            <asp:ListItem Value="100">$100</asp:ListItem>
                                            <asp:ListItem Value="200">$200</asp:ListItem>
                                            <asp:ListItem Value="300">$300</asp:ListItem>
                                            <asp:ListItem Value="400">$400</asp:ListItem>
                                            <asp:ListItem Value="500">$500</asp:ListItem>
                                            <asp:ListItem Value="600">$600</asp:ListItem>
                                            <asp:ListItem Value="700">$700</asp:ListItem>
                                            <asp:ListItem Value="800">$800</asp:ListItem>
                                            <asp:ListItem Value="900">$900</asp:ListItem>
                                            <asp:ListItem Value="1000">$1000</asp:ListItem>
                                        </asp:DropdownList>
                                    </div>
                                    <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;"></div>
                                    <div class="ColRowSwap" style="padding:5px;font-weight:bold;text-decoration:underline;"></div>
                                    <div id="lblMMR" class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;visibility:hidden;">MMR Percentage:</div>
                                    <div id="lblMMRinput" class="ColRowSwap" style="font-weight:bold;text-decoration:underline;visibility:hidden;">
                                        <asp:Textbox ID="MMRPercentage" runat="server" placeholder="%" CssClass="inputStyle" Style="text-align:center;" />
                                    </div>
                                </div>
                            </div>
                    </fieldset>
                    <fieldset id="simpleAdditionalSettings" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">Additional AutoLaunch Settings</legend>
                        <div id="simpleAddSettings" class="alFilterTbl" style="margin: 0 auto;display:table;table-layout:fixed;">
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">
                                    <div class="minadjtooltip">Maximum Negative Gross (ex. -1000):<span class="tooltiptext">How much is the dealer willing to sell under Cost?</span></div>
                                </div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox ID="MinimumPricingAdjustment" runat="server" placeholder="$" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;"></div>
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;display:none;">Require Condition Report:</div>
                                <div class="ColRowSwap" style="padding:2px;display:none;">
                                    <input type="checkbox" id='AllowDefaultCR' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;"></div>
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;">&nbsp;Allow 1-Day Auctions:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <input type="checkbox" id='AllowOneDayAuctions' class="SingleIndent"/>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <div style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;">
                        <asp:Button ID='simpleSave' OnClientClick="SimpleSaveAutoLaunchRule(document.getElementById('MainContent_hfMode').value);return false;" runat='server' Text='Save Rule' CssClass='actionBackground' Style='width:125px'/>
                    </div>
                </div>
            </div>
        </div>
        <div id="advancedOptions" class="modalPopup" style="padding-top:75px;">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="toggleCssClass([['advancedOptions','show_display']]); MinMaxYearChange(true); return false;">&times;</span>
                    <h2 id="alAdvancedTitle" style="text-align: center;"></h2>
                </div>
                <div class="modalBody hide_scrollbar" style="height:calc(100vh - 275px);overflow:scroll;">
                    <fieldset id="advancedFilters" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">AutoLaunch Filters</legend>
                        <div id="advancedFilterTbl" class="alFilterTbl">
                            <div style="display:table;margin: 0 auto;" class="">
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Auction:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstAuction" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" onchange="OnAuctionChange();return false;"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Year:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstMinYear" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" onchange="MinMaxYearChange();return false;"/> to <asp:DropdownList id="lstMaxYear" runat="server" CssClass="inputStyle" AutoPostBack="false"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Lot Location:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstLotLocation" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Make:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstMake" runat="server" CssClass="inputStyle SingleIndent" onchange="OnMakeChange()" AutoPostBack="false" /></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Credential:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstCredentials" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Model:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:DropdownList id="lstModel" runat="server" CssClass="inputStyle SingleIndent" Disabled="True" AutoPostBack="false">
                                            <asp:ListItem Value="0">Any Model</asp:ListItem>
                                        </asp:DropdownList>
                                    </div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Status:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstStatus" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle Type:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstVehicleType" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Title Status:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstTitle" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Mileage:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><input type="text" id='minMile' class="inputStyle SingleIndent" placeholder="Any" style="width:75px;text-align:center;" value="0" oninput="this.value = this.value.replace(/[^0-9]/g, '').replace(/(\\..*)\\./g, '$1');"/> to <input type="text" id='maxMile' class="inputStyle" placeholder="Any" style="width:75px;text-align:center;" value="0"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle Age:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><input type="text" id='minAge' class="inputStyle SingleIndent" placeholder="Any" style="width:75px;text-align:center;" value="0"/> to <input type="text" id='maxAge' class="inputStyle" placeholder="Any" style="width:75px;text-align:center;" value="0"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Fuel Type:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstFuelType" runat="server" CssClass="inputStyle SingleIndent" placeholder="ANY" AutoPostBack="false"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Condition Report:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstConditionRpt" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" onchange="OnCRChange();return false;"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Grade:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstMinGrade" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/> to <asp:DropdownList id="lstMaxGrade" runat="server" CssClass="inputStyle" AutoPostBack="false"/></div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <fieldset id="alAdd" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">AutoLaunch Additional Settings</legend>
                        <div id="autolaunchAddTbl" class="alAddTbl">
                            <div style="display:table;margin: 0 auto;" class="">
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Listing Type:</div>
                                    <div class="ColRowSwap" style="padding:2px;">
                                        <asp:DropdownList id="lstListingType" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" onchange="ListingTypeChange();">
                                            <asp:ListItem Value="1">Bid</asp:ListItem>
                                            <asp:ListItem Value="2">Buy</asp:ListItem>
                                            <asp:ListItem Value="3">Bid/Buy</asp:ListItem>
                                            <asp:ListItem Value="4">Bid/Offer</asp:ListItem>
                                            <asp:ListItem Value="5">Bid/Buy/Offer</asp:ListItem>
                                            <asp:ListItem Value="6">Buy/Offer</asp:ListItem>
                                            <asp:ListItem Value="7">Offer Only</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Listing Category:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="advancedLstListingCategory" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Duration:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><input type="text" id='duration' class="inputStyle SingleIndent" style="width:50px;"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div id="CrCat" class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">No CR Category:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstCRCat" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                </div>
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Physical Location Indicator:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstPhysLocation" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                    <div class='centerCell smallHide' style='width:25px;'></div>
                                    <div id="BidIncrement" class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;display:none;">Bid Increment:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstBidIncrement" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" Style="display:none;"/></div>
                                </div>
                                <div id="auctionCode" style="display:none;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Facilitated Auction Code:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstAuctionCode" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                </div>
                                <div id="locationCode" style="display:none;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Location Code:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstLocationCode" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false"/></div>
                                </div>
                                <div id="limitedArb" style="display:none;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Limited Abritration PowerTrain Pledge:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" id='arbPledge' class="SingleIndent"/></div>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <fieldset id="alPrices" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">AutoLaunch Pricing Settings</legend>
                        <div id="autolaunchPricesTbl" class="alPricesTbl">
                            <div style="display:table;margin: 0 auto;" class="">
                                <div style="display:table-row;">
                                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Minimum Pricing Adjustment:</div>
                                    <div class="ColRowSwap" style="padding:2px;"><input type="text" id='minPriceAdj' class="inputStyle SingleIndent" value="0" style="width:100px;"/></div>
                                </div>
                            </div>
                            <fieldset id="primaryPrices" class="sectionFieldset" style="background-color:transparent;">
                                <legend style="color:black;border:0!important;background-color:white !important;">Primary Pricing Values</legend>
                                <div style="display:table;margin: 0 auto;" class="">
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstPrimeStartPrice" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='primeStartAdj' class="inputStyle PricingBox" style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='primeStartPerc' class="inputStyle PricingBox SingleIndent" style="width:100px;" value="100"/> %</div>
                                    </div>
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstPrimeFloorPrice" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='primeFloorAdj' class="inputStyle PricingBox" style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='primeFloorPerc' class="inputStyle PricingBox SingleIndent" style="width:100px;" value="100"/> %</div>
                                    </div>
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstPrimeBIN" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='primeBINAdj' class="inputStyle PricingBox" style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='primeBINPerc' class="inputStyle PricingBox SingleIndent" style="width:100px;" value="100"/> %</div>
                                    </div>
                                </div>
                            </fieldset>
                            <fieldset id="secondaryPrices" class="sectionFieldset" style="background-color:transparent;">
                                <legend style="color:black;border:0!important;background-color:white !important;">Secondary Pricing Values</legend>
                                <div style="display:table;margin: 0 auto;" class="">
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstSecondStartPrice" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='secondStartAdj' class="inputStyle PricingBox"  style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Start Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='secondStartPerc' class="inputStyle PricingBox SingleIndent"  style="width:100px;" value="100"/> %</div>
                                    </div>
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstSecondFloorPrice" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='secondFloorAdj' class="inputStyle PricingBox" style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Floor Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='secondFloorPerc' class="inputStyle PricingBox SingleIndent" style="width:100px;" value="100"/> %</div>
                                    </div>
                                    <div style="display:table-row;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Price Type:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropdownList id="lstSecondBIN" runat="server" CssClass="inputStyle PricingBox SingleIndent" AutoPostBack="false"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Adjustment:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><span class="SingleIndent">$ </span><input type="text" id='secondBINAdj' class="inputStyle PricingBox" style="width:100px;" value="0"/></div>
                                        <div class='centerCell smallHide' style='width:25px;'></div>
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Buy Now Percentage:</div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="text" id='secondBINPerc' class="inputStyle PricingBox SingleIndent" style="width:100px;" value="100"/> %</div>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                    </fieldset>
                    <div style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;">
                        <asp:Button ID='btnFilters' OnClientClick="AdvancedSaveAutoLaunchRule(document.getElementById('MainContent_hfMode').value);return false;" runat='server' Text='Save Rule' CssClass='actionBackground' Style='width:125px'/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
