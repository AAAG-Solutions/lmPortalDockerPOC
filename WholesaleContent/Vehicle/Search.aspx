<%@ Page Title="Vehicle Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Search.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.Search" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/Search.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Search.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <div id="vehicleSearch" class="VehicleSearch">
        <div id="vehicleActionsBar" class="vehicleActionsBar">
            <div id="DesktopActionsHolder">
                <h6 style="font-weight: bold;padding-left:10px;">Inventory / Vehicle Search</h6>
                <h6 style="padding-left:10px;">&#60; &#8722; <a class='backBreadcrumb' href='javascript:history.back();'>Back</a></h6>
            </div>
            <div id="vehicleSearchBox" class="vehicleSearchBox" runat="server">
                <div class="vehicleSearchBoxItems">
                    <div class="searchItems">
                        <div class="searchItemContainer">
                            <asp:TextBox ID="txtSearch" AutoPostBack="false" runat="server" CssClass="txtSearch searchItem" placeholder="Search Vehicles"></asp:TextBox>
                        </div>
                        <div class="searchItemContainer">
                            <button ID="VinSearchButton" class="submitButton searchItem" onclick="SubmitVinSearch(); LCGridProcess(); return false;" style="margin-left: 10px;">Search</button>
                        </div>
                        <div class="searchItemContainer">
                            <span id="OrText" class="searchItem" style="text-align: center; font-weight: bold">**** OR ****</span>
                        </div>
                        <div class="searchItemContainer">
                            <button ID="ScannerButton" class="submitButton searchItem" onclick="ToggleScanner(); return false;" style="margin-left: 10px;">Scan Barcode</button>
                        </div>
                    </div>
                </div>
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <div id="invalidVin" style="color:red;display:none;">Invalid VIN. Please enter a valid VIN.</div>
                        <div id="invalidVinLength" style="color:red;display:none;">Invalid Search. Please enter at least 6 characters.</div>
                    </div>
                </div>
            </div>
            <div id="Scanner">
                <div id="scannerPopup" class="modalPopup">
                    <div id="ScannerHolder" class="scannerHolder">
                        <button ID="closeScanner" class="submitButton" onclick="ToggleScanner(); return false;" style="margin-left: 10px;">Close</button>
                        <div id="ScannerContent" style="text-align:center;">
                            <h1>Barcode Scanner</h1>

                            <div id="error" class="error hidden"></div>

                            <div class="controls">
                                <div class="button-group">
                                    <button id="startBtn" onclick="startScanning(); return false;" class="start-btn">
                                        📷 Start Scanning
                                    </button>
                                    <button id="stopBtn" onclick="stopScanning(); return false;" class="stop-btn hidden">
                                        ⏹ Stop Scanning
                                    </button>
                                </div>

                                <div class="video-container">
                                    <video id="video" autoplay playsinline muted class="hidden"></video>
                                    <canvas id="canvas" class="hidden"></canvas>
                                    <svg id="overlay" class="hidden"></svg>
                                    <div id="placeholder" class="placeholder">
                                        Click "Start Scanning" to begin
                                    </div>
                                </div>
                            </div>

                            <div id="results" class="results hidden">
                                <h2>Detected Barcodes</h2>
                                <div id="barcodeList"></div>
                            </div>

                            <div class="info">
                                <p>Using native Barcode Detection API</p>
                                <p>Point your camera at a barcode (QR code, EAN, UPC, etc.)</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="InfoPopup" class="modalPopup">
                <div class="modalContent" style="text-align:center">
                    <span id="InfoText" style="text-align:center"></span>
                    <br />
                    <button id="GoToAddBtn" class="submitButton" onclick="GoToAdd(); return false;">Yes</button>
                    <button id="CancelInfoBtn" class="submitButton" onclick="javascript:toggleCssClass([['InfoPopup', 'show_display']]);return false;">No</button>
                </div>
            </div>
        </div>
        <br/>
        <div id="grid">
            <div id="jsGrid"></div>
            <asp:Button ID="AccountButton" CssClass="btnSubmitDefault" runat="server" Text="Go To Wholesale" OnClick="GoToAccount" />
            <asp:Button ID="VehicleButton" CssClass="btnSubmitDefault" runat="server" Text="Go To Vehicle" OnClick="GoToVehicle" />
            <asp:HiddenField ID="DealerText" runat="server" Value="" />
            <asp:HiddenField ID="VehiclekListing" runat="server" Value="" />
            <asp:HiddenField ID="VehicleVin" runat="server" Value="" />
        </div>
    </div>

</asp:Content>