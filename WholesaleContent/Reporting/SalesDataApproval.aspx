<%@ Page Title="Sales Data Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesDataApproval.aspx.cs" Inherits="LMWholesale.WholesaleContent.Reporting.SalesDataApproval" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <style type="text/css">
        .Hide {
            display: none;
        }
        .selectableLegend {
            cursor:pointer;
        }
        @media screen and (orientation: landscape) and (max-height: 450px){.smallHide {display: none;}}
    </style>

    <!-- Import Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Reporting/SalesDataApproval.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <asp:HiddenField ID="hfMode" runat="server" Value="" />
    <asp:HiddenField ID="hfViewList" runat="server" Value="" />

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="salesDataApproval hide_scrollbar" style="margin-right:10px;overflow:scroll;">
        <h6 id="breadCrumb" runat="server" style="font-weight: bold;">Reporting / <div class="dropdownHover">
                <span style="text-decoration: underline">Sales Data Approval</span>
                <div class="dropdownHoverContent">
                    <a id="gpInfo" href="/WholesaleContent/Reporting/Status.aspx">Dashboard</a>
                    <a id="alRules" href="/WholesaleContent/Reporting/CreditRequest.aspx">Credit Request</a>
                </div>
            </div>
        </h6>
        <h6 id="BackToWholesaleDefault" runat="server" style="display: none;"></h6>

        <fieldset id="fsFilters" class="sectionFieldset">
            <legend onclick="showHideSection('divFilters', 'fsFilters');" class="selectableLegend">Sales Data Filters: &#9660;</legend>
            <div id="divFilters" style="display:none">
                <div style="display: flex;justify-content: space-evenly;flex-wrap: wrap;">
                    <div style="display:table-row;">
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:200px;">
                            <asp:Label style='font-weight:bold' runat="server">Sale Transaction Status: </asp:Label>
                        </div>
                        <div class="ColRowSwap" style="padding:5px;width:20px;">
                            <asp:DropDownList ID="ddlSaleTransactions" runat="server" onchange="OnChangeIdx(this, 'kSales');" CssClass="inputStyle SingleIndent"/>
                            <asp:HiddenField ID="kSales" runat="server" Value="1"/>
                        </div>
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">
                            <asp:Label style='font-weight:bold' runat="server">Start Date: </asp:Label>
                        </div>
                        <div class="ColRowSwap" style="padding:5px;width:20px;">
                            <asp:TextBox ID="txtStartDate" textmode="Date" runat="server" CssClass="inputStyle SingleIndent"/>
                        </div>
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">
                            <asp:Label style='font-weight:bold' runat="server">End Date: </asp:Label>
                        </div>
                        <div class="ColRowSwap" style="padding:5px;width:20px;">
                            <asp:TextBox ID="txtEndDate" textmode="Date" runat="server" CssClass="inputStyle SingleIndent"/>
                        </div>
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">
                            <asp:Label style='font-weight:bold' runat="server">Marketplace: </asp:Label>
                        </div>
                        <div class="ColRowSwap" style="padding:5px;width:20px;">
                            <asp:DropDownList ID="ddlMarketplace" runat="server" onchange="OnChangeIdx(this, 'kMarket');" CssClass="inputStyle SingleIndent"/>
                            <asp:HiddenField ID="kMarket" runat="server" Value="0"/>
                        </div>
                    </div>
                </div>
                <div id='pnlButtons' style='margin:0 auto; padding-top:10px;text-align:center;'>
                    <asp:Button CssClass="actionBackground" id="btnFilters" text="Apply" runat="server" OnClientClick="ApplyFilters(); return false;" />
                </div>
            </div>
        </fieldset>
        <fieldset class="sectionFieldset">
            <legend>Sales Transactions:</legend>
            <div style="display:flex; justify-content: space-evenly;flex-wrap: wrap;">
                <div class="display:table-row;">
                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:100px;">
                        <asp:Label style="font-weight: bold;" runat="server">VIN Search: </asp:Label>
                    </div>
                    <div class="ColRowSwap">
                        <asp:TextBox id="txtSearch" style="width:150px" runat="server" CssClass="inputStyle SingleIndent"/>
                    </div>
                    <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;width:125px;">
                        <asp:Label style="font-weight: bold;" runat="server">Quick Select:</asp:Label>
                    </div>
                    <div class="ColRowSwap">
                        <asp:DropDownList name="ddlFilters" id="ddlFilters" onchange="javascript: QuickSelect(); return false;" runat="server" CssClass="inputStyle SingleIndent"/>
                    </div>
                </div>
            </div>
            <div style='margin:0 auto; padding-top:10px;text-align:center;'>
                <asp:Button CssClass="actionBackground" id="btnSearch" text="Search" runat="server" OnClientClick="HandleChangePause(null); return false;" />
            </div>
            <br />
            <div id="grid" class="startWholesaleGrid hide_scrollbar" style="height: calc(100vh - 280px);margin-left: 0px;">
                <div id="jsGrid"></div>
                <asp:HiddenField ID="txtRule" runat="server" Value="" />
            </div>
            <div style='margin:0 auto; padding-top:10px;text-align:center;'>
                <asp:Button CssClass="actionBackground" id="btnSubmit" text="Submit to AMS" OnClientClick="javascript: SubmitApprovals(0); return false;" runat="server" />
                <asp:Button CssClass="actionBackground" id="btnMark" text="Mark as Complete" OnClientClick="javascript: SubmitApprovals(1); return false;" runat="server" />
                <asp:Button CssClass="actionBackground" id="btnExport" text="Export" OnClick="ExportSalesData" runat="server" />
            </div>
        </fieldset>
    </div>

    <script type="text/javascript">
        var pause_timeout = null;

        var today = new Date();
        document.getElementById("MainContent_txtEndDate").value = today.toISOString().substring(0, 10);
        today.setDate(today.getDate() - 7)
        document.getElementById("MainContent_txtStartDate").value = today.toISOString().substring(0, 10);

        var vinInput = document.getElementById('<%=txtSearch.ClientID %>');
        OnChangePause(vinInput, HandleChangePause, 300);

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>
</asp:Content>
