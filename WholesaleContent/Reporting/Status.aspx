<%@ Page Title="Account Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="LMWholesale.WholesaleContent.Reporting.Status" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Reporting/Status.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Reporting/Status.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="StatusPage" class="StatusPage hide_scrollbar">
        <div id="divPage" style="height:100%">
            <div id="divWidgets" class="widgetsTable" style="display:table;width: 100%;table-layout:fixed;border-spacing:10px;">
                <div style="display:table-row;height:50%;">
                    <div class="ColRowSwap">
                        <fieldset id="InvHealth" class="sectionFieldset HealthSection">
                            <legend>Inventory Health</legend>
                            <div id="divHealthTbl" style="display: table; border-collapse: collapse; width: 100%;">
                                <div id="Header" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCellHeader TableCellHeaderLeft" style="border-right: 1px solid #FFF;"><asp:label runat="server">Description</asp:label></div>
                                    <div class="TableCellHeader TableCellHeaderRight"><asp:label runat="server">Used</asp:label></div>
                                    <%--<div class="TableCellHeader" style="border-right: 1px solid #FFF; width: 20%;"><asp:label runat="server">New</asp:label></div>
                                    <div class="TableCellHeader" style="width: 20%;"><asp:label runat="server">Total</asp:label></div>--%>
                                </div>
                                <div id="Photos" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles w/ Photos</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoPhotos');return false;" style="cursor: pointer;"><asp:label ID="lblVehPhotosUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehPhotosNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehPhotosTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="NoPhotos" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Avg. Days w/ No Photos</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoPhotos');return false;" style="cursor: pointer;"><asp:label ID="lblAvgDaysPhotosUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblAvgDaysPhotosNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblAvgDaysPhotosTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="ListPrice" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles w/ List Price</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoListPrice');return false;" style="cursor: pointer;"><asp:label ID="lblVehListUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehListNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehListTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="InternetPrice" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles w/ Internet Price</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoInternetPrice');return false;" style="cursor: pointer;"><asp:label ID="lblVehInternetUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehInternetNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehInternetTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="Description" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles w/ Description</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoDescription');return false;" style="cursor: pointer;"><asp:label ID="lblVehDescUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehDescNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehDescTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="NoDescription" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Avg Days w/ No Description</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoDescription');return false;" style="cursor: pointer;"><asp:label ID="lblAvgDescUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblAvgDescNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblAvgDescTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="Style" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles w/ Style</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('NoStyle');return false;" style="cursor: pointer;"><asp:label ID="lblVehStyleUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehStyleNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehStyleTotal" runat="server"></asp:label></div>--%>
                                </div>
                                <div id="Unavailable" style="display:table-row; border: 1px solid #000">
                                    <div class="TableCell"><asp:label runat="server">Vehicles Unavailable</asp:label></div>
                                    <div class="TableCell" onclick="Javascript: OpenVehicleManagement('Unavailable');return false;" style="cursor: pointer;"><asp:label ID="lblVehUnavailUsed" runat="server"></asp:label></div>
                                    <%--<div class="TableCell"><asp:label ID="lblVehUnavailNew" runat="server"></asp:label></div>
                                    <div class="TableCell"><asp:label ID="lblVehUnavailTotal" runat="server"></asp:label></div>--%>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                    <div class="ColRowSwap">
                        <fieldset id="Reports" class="sectionFieldset ReportsSection">
                            <legend>Reports</legend>
                            <div id="ReportTable" class="ReportTable">
                                <div style="display:table-row">
                                    <div class="ColRowSwapLabel" style="text-align:right;font-weight:bold;">Select a Report:</div>
                                    <div class="ColRowSwap"><asp:DropDownList ID="lstReportSelector" runat="server" CssClass="inputStyle SingleIndent" onchange="ReportSelectionChanged()"/></div>
                                </div>
                                <div style="display:table-row">
                                    <div id="RunDateLabel" class="ColRowSwapLabel" style="display:none;text-align:right;font-weight:bold;">Select Run Date:</div>
                                    <div id="RunDateSelector" style="display:none;" class="ColRowSwap"><asp:TextBox ID="tbRunDate" runat="server" CssClass="inputStyle SingleIndent" TextMode="Date"/></div>
                                </div>
                            </div>
                            <div id="ErrorDisplay" style="font-weight:bold;color:red;text-align:center;" runat="server"></div>
                            <div id="ReportsButtons" style="display:table;border-spacing:15px;margin:auto;">
                                <div style="display:table-row">
                                    <div style="display:table-cell;"><input type="submit" name="btnViewInBrowser" value="View in Browser" onclick="ViewInBrowser(); return false; " id="btnViewInBrowser2" class="actionBackground"></div>
                                    <div style="display:table-cell;"><asp:Button ID="btnExportToExcel" runat="server" OnClick="ExportToExcel" cssClass="actionBackground" Text="Export to Excel"/></div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <div style="display:table-row;height:50%;display:none;">
                    <!-- <div class="ColRowSwap" style="display:none;">
                        <div class="row ForceWidth">
                            <fieldset id="CurrentResults" class="sectionFieldset">
                                <legend>Current Results</legend>
                                <div id="CurrentResultsGrid" style="height:50%"></div>
                            </fieldset>
                        </div>
                    </div> -->
                </div>
            </div>
            <div class="row" style="display:block;">
                <fieldset id="ImportHis" class="sectionFieldset">
                    <legend>Import History</legend>
                    <div id="jsGrid" style="height:50%"></div>
                </fieldset>
            </div>
            <asp:HiddenField ID="hfThresholds" runat="server" />
            <asp:HiddenField ID="hfNewCount" runat="server" />
            <asp:HiddenField ID="hfUsedCount" runat="server" />
        </div>
    </div>
</asp:Content>