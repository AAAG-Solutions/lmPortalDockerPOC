<%@ Page Title="Start Wholesale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="StartWholesale.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.StartWholesale" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/StartWholesale.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/StartWholesale.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>    
    <div id="startWholesale" class="startWholesale hide_scrollbar">
        <h6 id="breadCrumb" style="font-weight: bold;">Inventory / Vehicle / <div class="dropdownHover">
            <span style="text-decoration: underline">Start Wholesale</span>
                <div class="dropdownHoverContent">
                    <a id="endWholesale" runat="server">End Wholesale</a>
                </div>
            </div>
        </h6>
        <h6 id="BackToVehicle" runat="server"></h6>

        <div class="hide_scrollbar" style="overflow:auto;height:calc(100vh - 150px);padding-bottom:25px;">
            <fieldset id="listingInfo" class="sectionFieldset">
                <legend>General Listing Information:</legend>
                <div class="GLITable">
                    <div class="singleRow">
                        <div class="centerCell" style="font-weight:bold;">Select Wholesale Marketplaces:</div>
                        <div id="startAuctionList" runat="server"></div>
                    </div>
                </div>
                <div class="GLITable">
                    <div class="singleRow">
                        <asp:HiddenField runat="server" ID="HiddenVIN" Value="" />
                        <div class='SmallLeftAlign centerCell' style='font-weight:bold'>VIN:&nbsp;</div>
                        <div class='SmallLeftAlign centerContent'><asp:Label runat="server" ID="startVIN" class="SingleIndent" /></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell' style='font-weight:bold'>Vehicle:&nbsp;</div>
                        <div class='SmallLeftAlign centerContent'><asp:Label runat="server" ID="startDesc" class="SingleIndent" /></div>
                    </div>
                    <div class="singleRow">
                        <div class='SmallLeftAlign centerCell' style='font-weight:bold'>Cost:&nbsp;</div>
                        <div class='SmallLeftAlign centerContent'><asp:Label runat="server" ID="StartCost" class="SingleIndent" /></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell' style='font-weight:bold'>Grade:&nbsp;</div>
                        <div class='SmallLeftAlign centerContent'><asp:Label runat="server" ID="VehicleGrade" class="SingleIndent" /></div>
                    </div>
                </div>
                <br/>
                <div class="GLITable">
                    <div class='singleRow'>
                        <div class='SmallLeftAlign centerCell'><label for='StartDate' style='font-weight:bold'>Start Date:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><input type="date" id='StartDate' class="inputStyle SingleIndent"/></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell'><label for='EndDate' style='font-weight:bold'>End Date:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><input type="date" id='EndDate' class="inputStyle SingleIndent"/></div>
                    </div>
                    <div class='singleRow'>
                        <div class='SmallLeftAlign centerCell'><label for='ListingType' style='font-weight:bold'>Listing Type:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><asp:DropDownList ID="lstListingType" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="ChangeSelectedAuctions();"/></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell'><label for='ListingCat' style='font-weight:bold'>Listing Category:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><asp:DropDownList ID="lstListingCategory" runat="server" CssClass="inputStyle SingleIndent" DataValueField="Filter" AutoPostBack="false" /></div>
                    </div>
                    <div class='singleRow'>
                        <div class='SmallLeftAlign centerCell'><label for='BidIncrement' style='font-weight:bold'>Bid Increment:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><asp:DropDownList ID="lstBidIncrement" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" disabled="true"/></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell' style="display:none;"><label for='ArbPledge' style='font-weight:bold'>Limited Arbitration Powertrain Pledge:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent' style="display:none;"><input id='ArbPledge' type='checkbox' runat='server' style='font-weight:bold;' class="SingleIndent" />&nbsp;<b>( SmartAuction Only )</b>&nbsp;</div>
                    </div>
                    <div class='singleRow'>
                        <div class='SmallLeftAlign centerCell'><label for='RelistCount' style='font-weight:bold'># of times to Auto Relist:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><input runat="server" type="text" id='RelistCount' style="text-align:center;width:100px;" class="inputStyle SingleIndent" value="1"/></div>
                        <div class='centerCell' style='width:25px;'></div>
                        <div class='SmallLeftAlign centerCell'><label for='RequestInsp' style='font-weight:bold'>Request Inspection:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><input id='RequestInsp' type='checkbox' class="SingleIndent" style='font-weight:bold;' onclick="showInspCompany();"/>&nbsp;
                            <asp:DropDownList ID="lstInspectionCompany" runat="server" CssClass="inputStyle SingleIndent" AutoPostBack="false" Style="width:215px;display:none;"/>
                        </div>
                    </div>
                    <div class='singleRow'>
                        <div class='SmallLeftAlign centerCell'><label id='lblChkForce' for="chkForceWholesalePrice" style="font-weight:bold;display:none;" runat="server">Force Wholesale Pricing:&nbsp;</label></div>
                        <div class='SmallLeftAlign centerContent'><input id='chkForceWholesalePrice' type='checkbox' runat='server' class="SingleIndent" style='font-weight:bold;display:none;' onclick="chkForcePricing();"/></div>
                        <div class='centerCell' style='width:25px;'></div>
                    </div>
                </div>
                <div class="GLITable">
                    <div class='singleRow'>
                        <div class="SmallLeftAlign centerCell" style="font-weight:bold;">MMR Adjustments:&nbsp;</div>
                        <div class="ColRowSwap">
                            <div class="centerCell"><input id="percent" type="text" style="text-align:center;width:100px;" value="100" class="inputStyle SingleIndent"/>&nbsp;%</div>
                            <div class="centerCell">&nbsp;+&nbsp;</div>
                            <div class="centerCell">$&nbsp;<input id="dollar" type="text" style="text-align:center;width:100px;" value="0" class="inputStyle"/>&nbsp;</div>
                        </div>
                    </div>
                </div>
                <div class="MMRButtonSection">
                    <div class="mmrButtonCell">
                        <input id="setStart" type="button" class="actionBackground MMRButton" disabled style="opacity:0.1;" value="Set Start $" onclick="SetPrice('StartPrice')" runat="server">
                    </div>
                    <div class="mmrButtonCell">
                        <input id="setReserve" type="button" class="actionBackground MMRButton" value="Set Reserve $" onclick="SetPrice('ReservePrice')" runat="server">
                    </div>
                    <div class="mmrButtonCell">
                        <input id="setBIN" type="button" class="actionBackground MMRButton" value="Set Buy Now $" onclick="SetPrice('BINPrice')" runat="server">
                    </div>
                </div>
                <div id="MMRVal" class="SmallLeftAlign" style="margin-top:10px;text-align:center;font-weight:bold;">MMR Value: <br class="smallShowBlock" style="display:none;" />
                    <div style="display:inline;" class="SingleIndent">$<asp:Label runat="server" id="MMRValue"></asp:Label></div>
                </div>
                <div class="GLITable">
                    <div class='singleRow'>
                        <div class="SmallLeftAlign centerCell" style="font-weight:bold;">&nbsp;Start Price:&nbsp;</div>
                        <div class="SmallLeftAlign centerContent"><input runat="server" id='StartPrice' disabled style="text-align:center;width:100px;" class="inputStyle SingleIndent"/></div>
                        <div class="SmallLeftAlign centerCell" style="font-weight:bold;">&nbsp;Reserve Price:&nbsp;</div>
                        <div class="SmallLeftAlign centerContent"><input runat="server" id='ReservePrice' style="text-align:center;width:100px;" class="inputStyle SingleIndent"/></div>
                        <div class="SmallLeftAlign centerCell" style="font-weight:bold;">&nbsp;Buy It Now Price:&nbsp;</div>
                        <div class="SmallLeftAlign centerContent"><input runat="server" id='BINPrice' style="text-align:center;width:100px;" class="inputStyle SingleIndent"/></div>
                    </div>
                </div>
                <br/>
                <asp:HiddenField runat="server" ID="OpenLaneIsDealerAccount" Value="False"/>
                <asp:HiddenField runat="server" ID="OVEIsDealerAccount" Value="False"/>
                <asp:HiddenField runat="server" ID="MaxMMRPct" Value="" />
                <asp:HiddenField runat="server" ID="LotLocation" Value=""/>
                <div style="text-align-last:center;">
                    <div id='credDisclaimer1' style="color:red;display:none;">Wholesale Credentials may be enabled for this Auction and Lot Location; some fields may not be editable.</div>
                    <div id='credDisclaimer2' runat="server" style="color:red;display:none;">Credentials for this vehicle are configured as an OVE Dealer Account. The Start Price will be set within 16 Bid Increments of the Reserve Price for OVE listings.</div>
                    <div id='credDisclaimer3' runat="server" style="color:red;display:none;">Credentials for this vehicle are configured as an ADESA Dealer Account. The Start Price will be set equal to the Reserve Price for ADESA listings.</div>
                </div>
            </fieldset>
            <fieldset id="locationInfo" class="sectionFieldset">
                <legend id="vehicleInfoLegend" class="selectableLegend" onclick="javascript:showHideSection('vehicleLocationInfo','vehicleInfoLegend');return false;">Vehicle Location Information: &#9660;</legend>
                <div id="vehicleLocationInfo" class="vehicleLocationInfo">
                    <div class="GLITable vehicleLocationInfo">
                        <div class="singleRow">
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle Location:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><asp:DropdownList id="lstLocation" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            <div class='centerCell' style='width:25px;'></div>
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Estimated Arrival Date:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input type="date" id='arrivalDate' class="inputStyle SingleIndent"/></div>
                        </div>
                        <div class="singleRow">
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Contact Name:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input runat="server" type="text" id='contactName' class="inputStyle SingleIndent"/></div>
                        </div>
                        <div class="singleRow">
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Street:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input runat="server" type="text" id='addressStreet' class="inputStyle SingleIndent"/></div>
                            <div class='centerCell' style='width:25px;'></div>
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Contact Phone:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input runat="server" type="text" id='contactPhone' class="inputStyle SingleIndent"/></div>
                        </div>
                        <div class="singleRow">
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">City:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input runat="server" type="text" id='addressCity' class="inputStyle SingleIndent"/></div>
                            <div class='centerCell' style='width:25px;'></div>
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Zip:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><input runat="server" type="text" id='addressZip' class="inputStyle SingleIndent"/></div>
                        </div>
                        <div class="singleRow">
                            <div class="SmallLeftAlign centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">State:</div>
                            <div class="SmallLeftAlign centerContent" style="padding:2px;"><asp:DropdownList id="lstState" runat="server" CssClass="inputStyle SingleIndent"/></div>
                        </div>
                    </div>
                </div>
            </fieldset>
            <div style="display: table; margin: 0 auto;">
                <div class="centerContent" style="padding:2px;"><input id="startSubmit" type="button" class="actionBackground" value="Submit" onclick="ValidateWholesale();return false;"></div>
                <div id="Cancel" class="centerContent" style="padding:2px;" runat="server"></div>
                <asp:HiddenField ID="CurrenkListing" runat="server" Value="" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var today = new Date();
        document.getElementById('StartDate').value = today.toISOString().substring(0, 10);
        today.setDate(today.getDate() + 1);
        document.getElementById('arrivalDate').value = today.toISOString().substring(0, 10);

        // Set default End to 6 days ahead
        today.setDate(today.getDate() + 5);
        document.getElementById('EndDate').value = today.toISOString().substring(0, 10);

        var inputs = ["percent", "dollar"];
        inputs.forEach(input => {
            document.getElementById(input).setAttribute("oninput", "this.value = Number(this.value.replace(/[^0-9.]/g, '').replace(/(\\..*)\\./g, '$1')).toLocaleString('en-US');");
        });

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

</asp:Content>