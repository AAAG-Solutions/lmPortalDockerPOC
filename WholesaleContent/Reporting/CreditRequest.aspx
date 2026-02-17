<%@ Page Title="Credit Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="CreditRequest.aspx.cs" Inherits="LMWholesale.WholesaleContent.Reporting.CreditRequest" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Reporting/CreditRequest.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <style type="text/css">
        .jsgrid-grid-body { height: calc(100vh - 415px); }
        .options { display:flex;flex-wrap:wrap;justify-content:space-evenly; }
        @media(max-width:786px) {
            .options {
                display:table;
            }
            .actionBackground {
                width: 100px !important;
            }
        }
    </style>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="CreditApprovalContainer" class="creditApproval hide_scrollbar">
        <h6 class="noPrint" style="font-weight: bold;">Reporting / <div class="dropdownHover">
                <span style="text-decoration: underline">Credit Request</span>
                <div class="dropdownHoverContent">
                    <a id="dbInfo" href="/WholesaleContent/Reporting/Status.aspx">Dashboard</a>
                    <a id="sdaInfo" href="/WholesaleContent/Reporting/SalesDataApproval.aspx">Sales Data Approval</a>
                </div>
            </div>
        </h6>
        <div id="auctionInfo" style="margin-right:15px;">
            <fieldset id='VehicleDetails' class='sectionFieldset' style='position: relative;'>
            <legend id="top">Vehicle Details</legend>
            <legend id="disclosure" style="text-align:center;">
                Please enter the VIN, the marketplace it sold on, and the reason for the credit request. This screen will only process credit requests for the last billing cycle or for the current billing cycle.
            </legend>
            <br />
            <div class="options">
                <div class="ColRowSwap">
                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">VIN:</div>
                    <div class="ColRowSwap"><input type="text" id="inputVIN" runatstyle="display:table-cell;padding:2px;" oninput="this.value = this.value.replace(/[^A-Za-z0-9-]/g, '');" class="SingleIndent"/></div>
                </div>
                <div class="ColRowSwap">
                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Marketplace:</div>
                    <div class="ColRowSwap"><asp:DropDownList id="lstMarketPlace" runat="server" style="display:table-cell;padding:2px;" CssClass="SingleIndent" /></div>
                </div>
                <div class="ColRowSwap">
                    <div class="ColRowSwapLabel" style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Credit Reason:</div>
                    <div class="ColRowSwap">
                        <asp:DropDownList id="inputReason" runat="server" style="display:table-cell;padding:2px;" OnChange="OtherReason(this);return false;" CssClass="SingleIndent">
                            <asp:ListItem Text="Arbitration - Exterior Damages" Value="Arbitration - Exterior Damages" />
                            <asp:ListItem Text="Arbitration - Interior Damages" Value="Arbitration - Interior Damages" />
                            <asp:ListItem Text="Arbitration - Mechanical" Value="Arbitration - Mechanical" />
                            <asp:ListItem Text="Arbitration - Structural" Value="Arbitration - Structural" />
                            <asp:ListItem Text="Dealer Retailed Vehicle" Value="Dealer Retailed Vehicle" />
                            <asp:ListItem Text="Title Issue" Value="Title Issue" />
                            <asp:ListItem Text="Pricing Error" Value="Pricing Error" />
                            <asp:ListItem Text="Other" Value="Other" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div id="otherReason" style="display:none;">
                    <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Reason Details:</div>
                    <div style="display:table-cell"><input type="text" id="inputReasonDetail" style="display:table-cell;padding:2px;" oninput="this.value = this.value.replace(/[^A-Za-z0-9- ]/g, '');"/></div>
                </div>
            </div>
            <br />
            <div class="row" style="display:flex;flex-wrap:wrap;justify-content:center">
                <asp:Button runat="server" id="Save" OnClientClick="SendCreditRequest();return false;" Text="Save" CssClass="actionBackground" style="margin-right:5px;" />
                <asp:Button runat="server" id="Cancel" OnClientClick="javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';return false;" Text="Cancel" CssClass="actionBackground" style="margin-left:5px;" />
            </div>
            </fieldset>
            <fieldset id='requests' class='sectionFieldset' style='position: relative;'>
            <legend>Recent Credit Requests</legend>
                <div id="jsGrid"></div>
            </fieldset>
        </div>
    </div>
</asp:Content>