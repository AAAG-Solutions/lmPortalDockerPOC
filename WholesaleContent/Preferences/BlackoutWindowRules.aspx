<%@ Page Title="Account Blackout Window Rules" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BlackoutWindowRules.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.BlackoutWindowRules" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Preferences/BlackoutWindowRules.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <style type="text/css">
        .Hide {
            display: none;
        }
        .strike {
            color: red;
        }

        .ModalCheckbox {
            margin-right: 50px;
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
                height: calc(100vh - 260px) !important;
            }
        }
    </style>

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/BlackoutWindowRules.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <asp:HiddenField ID="hfMode" runat="server" Value="" />    
    <asp:HiddenField ID="hfViewList" runat="server" Value="" />

    <div id="sidebar_menu" class="sidebar hide_scrollbar noPrint"></div>
    <div class="dealerPreferences print">
        <h6 class="noPrint" style="font-weight: bold;margin-left:10px;">Account Preferences / <div class="dropdownHover">
                <span style="text-decoration: underline">Blackout Window Rules</span>
                <div class="dropdownHoverContent">
                    <a id="gpInfo" href="/WholesaleContent/Preferences/General.aspx">General Preferences</a>
                    <a id="alRules" href="/WholesaleContent/Preferences/AutoLaunchRules.aspx">AutoLaunch Rules</a>
                    <a id="mpInfo" href="/WholesaleContent/Preferences/MarketPlaceInfo.aspx">Market Place Info</a>
                </div>
            </div>
        </h6>

        <div class="printOnly">
            <div style="font-weight:bold;">Account Name: <asp:Label ID="DealerName" runat="server" Style="text-decoration:underline"></asp:Label></div>
        </div>

        <div id="grid" class="preferencesContainer print" style="height:calc(100vh - 165px);">
            <div id="jsGrid"></div>
            <asp:HiddenField ID="txtRule" runat="server" Value="" />
            <div id="ListForm" class="Hide print" runat="server">
            </div>
        </div>
        <div class="noPrint" style='text-align:-webkit-center;'>
            <button id='BWAdd' onclick="javascript: HandleButtonPress('add');return false;" class='submitButton' style='width:150px;'>Add</button>
            <button id='BWEdit' onclick="javascript: HandleButtonPress('edit');return false;" class='submitButton' style='width:150px;'>Edit</button>
            <button id='BWAction' onclick="javascript: SwitchRule();return false;" class='submitButton' style='width:150px;'>Suspend</button>
            <button id='BWDelete' onclick="javascript: HandleButtonPress('delete');return false;" class='submitButton' style='width:150px;'>Delete</button>
            <button id='BWList' onclick="javascript: ListView(); return false;" class='submitButton' style='width:150px;'>List</button>
            <button id='BWPrint' onclick="javascript: window.print(); return false;" class='Hide' style='width:150px'>Print</button>
        </div>
    </div>
    <div id="pop_overlay" class="modalPopup">
        <div id="BlackoutFields" class="modalContent">
            <div class="modalHeader">
                <span class="closeModalButton" onclick="HandleButtonPress('cancel'); MinMaxYearChange(true);return false;">&times;</span>
                <h2 id="HeaderText" style="text-align: center;">Blackout Window Rule</h2>
            </div>
            <div class="modalBody">
                <fieldset id="BlackoutWindowAuctions" class="sectionFieldset" style="background-color:transparent;">
                    <legend style="color:black;border:0!important;background-color:white !important;">Applicable Auctions</legend>
                    <div class="row" style="padding-left:5%; padding-right: 5%">
                        <asp:Panel ID="AuctionChecks" CssClass="table" runat="server">
                        </asp:Panel>
                    </div>
                </fieldset>
                <fieldset id="BlackoutWindowSettings" class="sectionFieldset" style="background-color:transparent;">
                    <legend style="color:black;border:0!important;background-color:white !important;">Blackout Window Settings</legend>
                    <div class="table">
                        <div class="ColRowSwap">
                            <div id="AuctionDropdown" class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Auction: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstAuction" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Vehicle Year: </div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:DropDownList ID="lstMinYear" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="MinMaxYearChange();return false;"/>
                                    to
                                    <asp:DropDownList ID="lstMaxYear" runat="server" CssClass="inputStyle" DataValueField="Filter" AutoPostBack="false"/>
                                </div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Blackout Start Day: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstStartDay" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Blackout End Day: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstEndDay" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Blackout Frequency: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstFrequency" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="javascript: FrequencyChange(); return false;"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Blackout Interval: </div>
                                <div id="divBlackoutWeek" class="ColRowSwap" style="padding:2px;">
                                    <asp:DropDownList ID="lstIntervalWeek" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onclick="javascript: onIntervalChange(); return false;"/>
                                </div>
                                <div id="divBlackoutMonth" class="ColRowSwap" style="display:none;padding:2px;">
                                    <asp:DropDownList ID="lstIntervalMonth" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/>
                                    <asp:DropDownList ID="lstIntervalMonthDay" runat="server" CssClass="inputStyle" DataValueField="Filter" AutoPostBack="false"/>
                                </div>
                            </div>
                        </div>
                        <div class="ColRowSwap">
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Lot Location: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstLotLocation" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Vehicle Make: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstVehicleMake" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Start Time: </div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:TextBox ID="tbStartTime" runat="server" CssClass="inputStyle SingleIndent" TextMode="Time" step="1" format="HH:mm:ss"/>
                                </div>
                            </div>
                            <div class="tableRow">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">End Time: </div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:TextBox ID="tbEndTime" runat="server" CssClass="inputStyle SingleIndent" TextMode="Time" step="1" format="HH:mm:ss"/>
                                </div>
                            </div>
                            <div id="divIIDay" class="hide tableRow">
                                <div class="tableCell" style="text-decoration:underline">Initial interval Day: </div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:TextBox ID="tbInitialIntervalDay" runat="server" CssClass="inputStyle SingleIndent" TextMode="Date"/>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div style="display:table;margin:0 auto;">
                        <div class="tableRow">
                            <div class="ColRowSwap">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Remove AutoLaunch Auction: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:Checkbox ID="cbALAuctionRemove" runat="server" CssClass="ModalCheckbox SingleIndent"/></div>
                            </div>
                            <div class="ColRowSwap">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Remove Manual Auction: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:Checkbox ID="cbManualAuctionRemove" runat="server" CssClass="ModalCheckbox SingleIndent"/></div>
                            </div>
                            <div class="ColRowSwap">
                                <div class="tableCell ColRowSwapLabel" style="text-decoration:underline">Suspend Blackout Window: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:Checkbox ID="cbBlackOutWindowSuspend" runat="server" CssClass="ModalCheckbox SingleIndent"/></div>
                            </div>
                        </div>
                    </div>
                </fieldset>
            </div>
            <div style="width:100%;text-align-last:center;padding-top:5px;"><button id='popupSubmit' onclick="javascript: HandleButtonPress('submit');return false;" class='submitButton' Style='width:125px'>Submit</button></div>
        </div>
    </div>
</asp:Content>