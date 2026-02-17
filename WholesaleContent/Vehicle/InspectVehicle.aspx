<%@ Page Title="Condition Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InspectVehicle.aspx.cs" Inherits="LMWholesale.InspectVehicle" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/InspectVehicle.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsGrid.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/InspectVehicle.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <asp:HiddenField ID="kListing" runat="server" Value="" />
    <asp:HiddenField ID="hfPPUndoOnCancel" runat="server" Value="" />
    <asp:HiddenField ID="DamageRowId" runat="server" Value="" />
    <asp:HiddenField ID="ExteriorInteriorMapping" runat="server" Value="" />
    <asp:HiddenField ID="ExtGridData" runat="server" Value="" />
    <asp:HiddenField ID="IntGridData" runat="server" Value="" />
    <asp:HiddenField ID="PaintGridData" runat="server" Value="" />
    <asp:HiddenField ID="CurrentReportURL" runat="server" Value="" />
    <asp:HiddenField ID="hfHasAutoGrade" runat="server" Value="" />
    <asp:HiddenField ID="hfBodyType" runat="server" Value="" />

    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>
                     
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="inspectVehicle hide_scrollbar">
        <div style="display:flex;flex-wrap:wrap;justify-content:center;width:100%;">
            <asp:Button ID='generalTab' OnClientClick="CSSSelector('general'); return false;" runat='server' Text='General' CssClass='actionBackground' Style='width:125px;margin-right:5px;'/>
            <asp:Button ID='TWTab' OnClientClick="CSSSelector('TW'); return false;" runat='server' Text='Tires/Wheels' CssClass='actionBackground' Style='width:125px;margin-right:5px;'/>
            <asp:Button ID='exteriorTab' OnClientClick="CSSSelector('exterior'); return false;" runat='server' Text='Exterior' CssClass='actionBackground' Style='width:125px;margin-right:5px;'/>
            <asp:Button ID='interiorTab' OnClientClick="CSSSelector('interior'); return false;" runat='server' Text='Interior' CssClass='actionBackground' Style='width:125px;margin-right:5px;'/>
            <asp:Button ID='paintTab' OnClientClick="CSSSelector('paint'); return false;" runat='server' Text='Paint' CssClass='actionBackground' Style='width:125px'/>
        </div>
        <div id="GVInfoContainer" class="row" style="width:100%">
            <div class="col">
                <fieldset id="CurrentConditionReport" class="sectionFieldset">
                    <legend>Current Condition Report</legend>
                    <div class="row">
                        <div style="display:table;">
                            <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">View Current Report: </div>
                            <div class="SingleIndent" style="padding:2px;"><asp:Label ID="NoReportLabel" runat="server" CssClass="HideItem" Text="Vehicle does not have a current condition report." /><asp:Button ID="currentReportButton" OnClientClick="ShowCurrentReport(); return false;" runat="server" Text="Open" CssClass="actionBackground SingleIndent" Style='width:125px' /></div>
                        </div>
                    </div>
                </fieldset>
                <fieldset id="VehicleInfo" class="sectionFieldset">
                    <legend>General Vehicle Information</legend>
                    <div class="GVInfo">
                        <div style="display: table;width: 100%;">
                            <div class="ColRowSwap">
                                <div style="display:table;">
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">VIN: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:Label ID="labVin" CssClass="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Title State: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstTitleState" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">License Plate State: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstLPState" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Odometer Status: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstOdoStat" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Driveable: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbDriveable" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Theft Recovery: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbTheft" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Prior Paint: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbPriorPaint" class="SingleIndent" runat="server" onchange="javascript:PriorPaintChanged(); return false;"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Airbag Light: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbAirbagLight" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Smoker Flag: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbSmoker" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Odor Flag: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbOdor" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Livery Use: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbLivery" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Taxi Use: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbTaxi" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Canadian Vehicle: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbCanadian" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Vin Plate Issue: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbVinPlateIss" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Altered Exhaust: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbAlteredEx" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Tire Pressure Indicator Present: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbTirePressure" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Audio Type: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstAudio" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;"># of Keys: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstNumKeys" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Vehicle Type: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstVehicleType" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">NAAA Grade: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstNAAAGrade" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" Enabled="false"/></div>
                                    </div>
                                </div>
                            </div>
                            <div class="ColRowSwap">
                                <div style="display:table;">
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Vehicle: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:Label ID="labVehicle" CssClass="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Title Status: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstTitleStatus" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">License Plate: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:TextBox ID="tbLicensePlate" runat="server" CssClass="inputStyle SingleIndent"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Frame Damage: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbFrameDamage" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Flood Damage: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbFloodDamage" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Fire Damage: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbFireDamage" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Airbags Deployed: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbAirbagsDep" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Airbag Missing: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbAirbagMiss" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Check Engine Light: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbCEL" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Police Use: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbPolice" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Grey Market: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbGreyMarket" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Has Manuals: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbManuals" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">5th Wheel: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cb5thWheel" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Factory Warranty Cancelled: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbFactoryWarr" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Altered Suspension: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbAlteredSus" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:none;">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Limited Arbitration PowerTrain Pledge: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbPowerTrain" class="SingleIndent" runat="server"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Hail Damage: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><input type="checkbox" Id="cbHailDamage" class="SingleIndent" runat="server" onchange="javascript:HailDamageChanged(); return false;"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Interior Type: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstIntType" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                    <div style="display:table-row">
                                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;"># of Fobs/Remotes: </div>
                                        <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstFobs" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div id="TWContainer" class="row" style="width:100%;display:none">
            <div class="col">
                <fieldset id="TWInfo" class="sectionFieldset">
                <legend>Tire Condition</legend>
                <div class="TWInfo">
                    <div class="row">
                        <div class="TireTable">
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">All Tires Match: </div>
                                <div class="ColRowSwap" style="padding:2px;"><asp:CheckBox Id="cbAllTiresMatch" runat="server" CssClass="SingleIndent"/></div>
                            </div>
                            <div class="smallHide" style="display:table-row">
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:right;"><br /></div>
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;Margin-left:2px;Margin-Right:2px;">Condition</div>
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;Margin-left:2px;Margin-Right:2px;">Tread Depth (32nds)</div>
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;Margin-left:2px;Margin-Right:2px;">Manufacturer</div>
                                <div style="display:table-cell;padding:2px;font-weight:bold;text-align:center;Margin-left:2px;">Wheel Size</div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Right Front:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstRFCondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstRFTD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstRFManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstRFWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Left Front:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstLFCondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstLFTD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstLFManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstLFWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Right Rear:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstRRCondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstRRTD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstRRManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstRRWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Left Rear:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstLRCondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstLRTD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstLRManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstLRWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Right Rear Inner:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstRRICondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstRRITD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstRRIManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstRRIWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Left Rear Inner:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstLRICondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstLRITD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstLRIManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstLRIWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-align:right;">Spare:</div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Condition:</div><asp:DropDownList ID="lstSPRCondition" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Tread Depth (32nds):</div><asp:DropDownList ID="lstSPRTD" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;Margin-Right:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Manufacturer:</div><asp:DropDownList ID="lstSPRManufact" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                                <div class="ColRowSwapLabel" style="padding:2px;Margin-left:2px;"><div class="smallShowBlock SingleIndent" style="display:none;">Wheel Size:</div><asp:DropDownList ID="lstSPRWheelSize" runat="server" CssClass="inputStyle DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
            </div>
        </div>
        <div id="ExtContainer" class="CRContainer row" style="width:100%;display:none;">
            <div class="col">
                <fieldset id="ExtInfo" class="sectionFieldset">
                    <legend>Exterior Damage</legend>
                    <br />
                    <asp:Button ID='ExtAddButton' OnClientClick="ButtonAction('add', 'ext', null); return false;" runat='server' Text='Add' CssClass='actionBackground' Style='width:125px;margin-left:5%;margin-bottom:1%'/>
                    <div id="ExtDamage" class="CRGrid hide_scrollbar">
                        <div id="jsGridExtDamage"></div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div id="IntContainer" class="CRContainer row" style="width:100%;display:none;">
            <div class="col">
                <fieldset id="IntInfo" class="sectionFieldset">
                    <legend>Interior Damage</legend>
                    <br />
                    <asp:Button ID='IntAddButton' OnClientClick="ButtonAction('add', 'int', null); return false;" runat='server' Text='Add' CssClass='actionBackground' Style='width:125px;margin-left:5%;margin-bottom:1%'/>
                    <div id="IntDamage" class="CRGrid hide_scrollbar">
                        <div id="jsGridIntDamage"></div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div id="PaintContainer" class="CRContainer row" style="width:100%;display:none;">
            <div class="col">
                <fieldset id="PaintInfo" class="sectionFieldset">
                    <legend>Prior Paint</legend>
                    <br />
                    <asp:Button ID='PaintAddButton' OnClientClick="ButtonAction('add', 'paint', null); return false;" runat='server' Text='Add' CssClass='actionBackground' Style='width:125px;margin-left:5%;margin-bottom:1%'/>
                    <div id="Paint" class="CRGrid hide_scrollbar">
                        <div id="jsGridPaint"></div>
                    </div>
                </fieldset>
            </div>
        </div>
        <div class="row" style="width:100%">
            <div class="col">
                <div style="width:100%;text-align-last:center;">
                    <asp:Button ID='submitButton' OnClientClick="ButtonAction('submit', 'main', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:125px'/>
                    <asp:Button ID='cancelButton' OnClientClick="history.back(); return false;" runat='server' Text='Cancel' CssClass='actionBackground' Style='width:125px'/>
                </div>
            </div>
        </div>
    </div>
    <div id="pop_overlay">
        <div id="ExtAddContainer" class="row CROverlay" style="width:100%;display:none">
            <div class="col">
                <fieldset id="ExtAddInfo" class="sectionFieldset">
                    <legend>Add Exterior Damage</legend>
                    <div id="ExtSelector">
                        <img id="ExteriorOutline" src="/Images/DamagePicker/sedan.png" onclick="handleClick(event)" class="outlineImage" runat="server" />
                    </div>
                    <div id="ExtInfoBlock" style="display:none;margin:auto;">
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Location: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstExteriorDamageCategory" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="javascript:ApplyConditionFilters('e', 'A'); return false;"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Condition: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstExteriorDamageCondition" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="javascript:ApplyConditionFilters('e', 'D'); return false;"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Severity: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstExteriorDamageSeverity" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Description: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:TextBox ID="tbExteriorDamageDescription" runat="server" CssClass="inputStyle SingleIndent"/></div>
                        </div>
                        <div id="ExtPhotoSelector" style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Photo: </div>
                            <div class="ColRowSwap" style="padding:2px;"><input id="inExtPhoto" type="file" accept=".jpeg,.jpg"></div>
                        </div>
                        <div id="ExtPhotoDisplay" style="display:none">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Photo: </div>
                            <div class="ColRowSwap" style="padding:2px;"><img id="inExtPhotoExisting" src="/Images/loading_icon.gif"></div>
                        </div>
                    </div>
                    <div class="row" style="margin-top:2%">
                        <asp:Button ID='submitExtAddButton' OnClientClick="ButtonAction('submit', 'extAdd', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='submitExtEditButton' OnClientClick="ButtonAction('submit', 'extEdit', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='cancelExtButton' OnClientClick="ButtonAction('cancel', 'ext', null); return false;" runat='server' Text='Cancel' CssClass='actionBackground' Style='width:20%;margin-left:10%;margin-right:20%'/>
                    </div>
                </fieldset>
            </div>
        </div>
        <div id="IntAddContainer" class="row CROverlay" style="width:100%;display:none">
            <div class="col">
                <fieldset id="IntAddInfo" class="sectionFieldset">
                    <legend>Add Interior Damage</legend>
                    <div id="IntInfoBlock" style="display:table;margin:auto;">
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Location: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstInteriorDamageCategory" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="javascript:ApplyConditionFilters('i', 'A'); return false;"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Condition: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstInteriorDamageCondition" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="javascript:ApplyConditionFilters('i', 'D'); return false;"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Severity: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstInteriorDamageSeverity" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Description: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:TextBox ID="tbInteriorDamageDescription" runat="server" CssClass="inputStyle SingleIndent"/></div>
                        </div>
                        <div id="IntPhotoSelector" style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Photo: </div>
                            <div class="ColRowSwap" style="padding:2px;"><input id="inIntPhoto" type="file" accept="image/*" capture="environment"></div>
                        </div>
                        <div id="IntPhotoDisplay" style="display:none">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Photo: </div>
                            <div class="ColRowSwap" style="padding:2px;"><img id="inIntPhotoExisting" src="/Images/loading_icon.gif"></div>
                        </div>
                    </div>
                    <div class="row" style="margin-top:2%">
                        <asp:Button ID='submitIntAddButton' OnClientClick="ButtonAction('submit', 'intAdd', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='submitIntEditButton' OnClientClick="ButtonAction('submit', 'intEdit', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='cancelIntButton' OnClientClick="ButtonAction('cancel', 'int', null); return false;" runat='server' Text='Cancel' CssClass='actionBackground' Style='width:20%;margin-left:10%;margin-right:20%'/>
                    </div>
                </fieldset>
            </div>
        </div>
        <div id="PaintAddContainer" class="row CROverlay" style="width:100%;display:none">
            <div class="col">
                <fieldset id="PaintAddInfo" class="sectionFieldset">
                    <legend>Add Paint Damage</legend>
                    <div id="PaintSelector">
                        <img id="PaintOutline" src="/Images/DamagePicker/sedan.png" onclick="handleClick(event)" class="outlineImage" runat="server" />
                    </div>
                    <div id="PaintInfoBlock" style="display:none;margin:auto;">
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Location: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstPriorPaintCategory" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwap SmallLeftAlign" style="padding:2px;font-weight:bold;text-align:right;">Damage Condition: </div>
                            <div class="ColRowSwap" style="padding:2px;"><asp:DropDownList ID="lstPriorPaintCondition" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                        </div>
                    </div>
                    <div class="row" style="margin-top:2%">
                        <asp:Button ID='submitPaintAddButton' OnClientClick="ButtonAction('submit', 'paintAdd', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='submitPaintEditButton' OnClientClick="ButtonAction('submit', 'paintEdit', null); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:20%;margin-left:20%;margin-right:10%;display:none'/>
                        <asp:Button ID='cancelPaintButton' OnClientClick="ButtonAction('cancel', 'paint', null); return false;" runat='server' Text='Cancel' CssClass='actionBackground' Style='width:20%;margin-left:10%;margin-right:20%'/>
                    </div>
                </fieldset>
            </div>
        </div>
    </div>
</asp:Content>