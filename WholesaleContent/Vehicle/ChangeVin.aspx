<%@ Page Title="VIN Change" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangeVin.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.ChangeVin" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Page Styles -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/ChangeVin.css">

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsGrid.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/ChangeVin.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <asp:HiddenField ID="hfkListing" runat="server" Value="" />
    <asp:HiddenField ID="hfStyleId" runat="server" Value="" />
    <asp:HiddenField ID="hfStyleIdNew" runat="server" Value="" />
    <asp:HiddenField ID="hfCurrentVin" runat="server" Value="" />
    <asp:HiddenField ID="hfCost" runat="server" Value="" />
    <asp:HiddenField ID="hfMiles" runat="server" Value="" />
    <asp:HiddenField ID="hfListPrice" runat="server" Value="" />
    <asp:HiddenField ID="hfStockNumber" runat="server" Value="" />
    <asp:HiddenField ID="hfDrillDown" runat="server" Value="" />
    <asp:HiddenField ID="hfStockType" runat="server" Value="" />
    <asp:HiddenField ID="hfStatus" runat="server" Value="" />
    <asp:HiddenField ID="hfDMI" runat="server" Value="" />

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="ChangeVin" style="padding-left: 45px;padding-right: 5px;padding-top: 10px;overflow-y: scroll;overflow-x: hidden;height: calc(100vh - 75px);">
        <div class="col">
            <fieldset id="VehicleDetails" class="sectionFieldset">
                <legend style="background-color: #ffffff;border: 1px solid #94a1b6 !important;">Vehicle Details</legend>
                    <div class="col">
                        <div style="text-align:center;margin:auto;">
                            <div style="padding-bottom:3%;"><asp:Label ID="Label1" runat="server" Text="WARNING: <br /> Changing VIN may result in loss of vehicle data. <br />If the vehicle configuration from vin explosion does not match the current configuration." ForeColor="Red" Font-Bold="True" Font-Italic="True"/></div>
                        </div>
                        <div id="BaseHolder" style="display:table;margin:auto;">
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Vin: </div>
                                <div style="display:table-cell;padding:2px"><asp:TextBox ID="tbVinNumber" runat="server" CssClass="inputStyle"/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Year: </div>
                                <div style="display:table-cell;padding:2px"><asp:TextBox ID="tbYear" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Make: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbMake" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Model: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbModel" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Style: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbStyle" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                        </div>
                        <div id="NewHolder" style="display:none;margin:auto;">
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Vin: </div>
                                <div style="display:table-cell;padding:2px"><asp:TextBox ID="tbVinNumberNew" runat="server" CssClass="inputStyle"/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Year: </div>
                                <div style="display:table-cell;padding:2px"><asp:TextBox ID="tbYearNew" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Make: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbMakeNew" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Model: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbModelNew" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;">Style: </div>
                                <div style="display:table-cell;padding:2px;"><asp:TextBox ID="tbStyleNew" runat="server" CssClass="inputStyleReadOnly" ReadOnly=true/></div>
                            </div>
                        </div>
                        <div id="OptionsTable" style="margin:auto;display:none;padding-top:2%">
                            <div style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;border-bottom:1px solid #517B97;">Selection</div>
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;border-left:1px solid #517B97;border-bottom:1px solid #517B97;">Style Option</div>
                                <div style="display:none;padding:2px;font-weight:bold;text-align:center;border-bottom:1px solid #517B97;">Style</div>
                            </div>
                        </div>
                    </div>
                    <div style="margin:0 auto; padding-top:10px;text-align:center;">
                        <asp:Button ID='submitButton' OnClientClick="ButtonAction('submit'); return false;" runat='server' Text='Submit' CssClass='actionBackground' />
                        <asp:Button ID='cancelButton' OnClientClick="history.back(); return false;" runat='server' Text='Cancel' CssClass='actionBackground' />
                    </div>
            </fieldset>
        </div>
    </div>
    <script type="text/javascript">
        var pause_timeout = null;

        var vinInput = document.getElementById('MainContent_tbVinNumber');
        var vinInputNew = document.getElementById('MainContent_tbVinNumberNew');
        OnChangePause(vinInput, HandleChangePause, 2000);
        OnChangePause(vinInputNew, HandleChangePause, 2000);

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>
</asp:Content>