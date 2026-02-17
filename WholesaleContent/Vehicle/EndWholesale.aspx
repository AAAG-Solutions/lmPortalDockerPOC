<%@ Page Title="End Vehicle Wholesale" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="EndWholesale.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.EndWholesale" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<style type="text/css">
	    .jsgrid{height: calc(100vh - 40%) !important;}
        .PadLeft{padding-left: 2px;}
        .AuctionList{padding:2px;display:flex;flex-wrap:wrap;}
        .AuctionHeader{display:table;margin: 0 auto;}
        @media(max-width:768px){.AuctionList{display:table;}.AuctionHeader{margin: 0px !important}}
	</style>

    <script type="text/javascript">
        function RemoveAuctions() {
            var auctions = [];
            document.querySelectorAll('[id*="CheckEnd"]:checked').forEach(auction => { auctions.push(auction.value); });
            var kListing = document.getElementById("MainContent_CurrenkListing").value;
            var dataIn = {
                Auctions: auctions,
                MarkUnavailable: document.getElementById("MainContent_chkMarkUnavail").checked && document.getElementById("divMarkUnavail").style.display != 'none'
            }

            if (auctions.length != 0) {
                $.ajax({
                    type: "POST",
                    url: 'EndWholesale.aspx/SubmitToRemoveMultiAuction',
                    data: "{'info': '" + JSON.stringify(dataIn) + "', 'kListing':'" + kListing + "'}",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(XMLHttpRequest.responseJSON.Message);
                    },
                    complete: function (response) {
                        var r = JSON.parse(response.responseJSON.d);
                        if (r.success) {
                            alert("Vehicle submitted to be removed from selected auctions!");
                            window.location.href = `/WholesaleContent/VehicleManagement.aspx`;
                        }
                        else
                            alert(`Unable to remove vehicle from selected auctions! Please contact support!\n\t\Message:${r.errormsgs}`);

                        return false;
                    }
                });
            }
            else
                alert("You either haven't selected any auctions to send to or do not have any enabled!");
        }

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>    
    <div id="endWholesale" class="endWholesale hide_scrollbar">
        <h6 id="breadCrumb" style="font-weight: bold;">Inventory / Vehicle / <div class="dropdownHover">
            <span style="text-decoration: underline">End Wholesale</span>
                <div class="dropdownHoverContent">
                    <a id="startWholesale" runat="server">Start Wholesale</a>
                </div>
            </div>
        </h6>
        <h6 id="BackToVehicle" runat="server"></h6>

        <fieldset id="martketPlaces" class="sectionFieldset" style="background-color:transparent;">
            <legend style="color:black;border:0!important;background-color:white !important;">End Wholesale Listing</legend>
            <div class="AuctionHeader">
                <div class="singleRow">
                    <div class="centerContent" style="font-weight:bold;">Select Wholesale Marketplaces:</div>
                    <div id="endAuctionList" class="AuctionList" runat="server"></div>
                </div>
            </div>
            <div class="AuctionHeader" >
                <div style="display:table-row">
                    <div class="ColRowSwap">
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">VIN:</div>
                        <div class="ColRowSwap" style="padding:2px;"><asp:Label runat="server" ID="endVIN" CssClass="SingleIndent" /></div>
                    </div>
                    <div class="ColRowSwap">
                        <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle:</div>
                        <div class="ColRowSwap" style="padding:2px;"><asp:Label runat="server" ID="endDesc" CssClass="SingleIndent" /></div>
                    </div>
                </div>
            </div>
            <div id="divMarkUnavail" class="AuctionHeader">
                <div class="ColRowSwap"><asp:Label ID="lblMarkUnavail" Text="Mark Unavailable:" runat="server" CssClass="ColRowSwapLabel font-weight-bold" /></div>
                <div class="ColRowSwap"><asp:CheckBox ID="chkMarkUnavail" runat="server" CssClass="SingleIndent PadLeft" /></div>
            </div>
        </fieldset>
        <div style="display:table;margin:0 auto;">
            <div class="centerContent" style="padding:2px;"><input id="endSubmit" type="button" class="actionBackground" value="Submit" onclick="javascript: RemoveAuctions();return false;"></div>
            <div id="Cancel" class="centerContent" style="padding:2px;" runat="server"></div>
            <asp:HiddenField ID="CurrenkListing" runat="server" Value="" />
        </div>
    </div>

    <script type="text/javascript">
    </script>

</asp:Content>