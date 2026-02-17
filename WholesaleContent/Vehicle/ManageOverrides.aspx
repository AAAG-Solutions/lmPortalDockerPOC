<%@ Page Title='Manage Inventory Overrides' Language='C#' MasterPageFile='~/Site.Master' AutoEventWireup='true' CodeBehind='ManageOverrides.aspx.cs' Inherits='LMWholesale.WholesaleContent.Vehicle.ManageOverrides' %>
<asp:Content ID='BodyContent' ContentPlaceHolderID='MainContent' runat='server'>
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/ManageOverrides.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Javascript Scripts -->
    <script type="text/javascript">
        function ButtonAction(action) {
            if (action == "submit") {
                var dataOut = {
                    kListing: document.getElementById("MainContent_hfkListing").value,
                    ImportOverride: document.getElementById("MainContent_DisFeed").checked ? "1" : "0",
                    ListPriceOverride: document.getElementById("MainContent_DisList").checked ? "1" : "0",
                    DetailDescOverride: document.getElementById("MainContent_DisDetail").checked ? "1" : "0",
                    InternetPriceOverride: document.getElementById("MainContent_DisInternet").checked ? "1" : "0",
                    InventoryStatusOverride: document.getElementById("MainContent_DisStatus").checked ? "1" : "0",
                    InvAdded: document.getElementById("MainContent_InventoryAddDate").value,
                    InvAddImport: document.getElementById("MainContent_DisAge").checked ? "1" : "0",
                    WholesaleFloorOverride: document.getElementById("MainContent_DisFloor").checked ? "1" : "0",
                    WholesaleBuyNowOverride: document.getElementById("MainContent_DisBIN").checked ? "1" : "0",
                    WholesaleStartPriceOverride: document.getElementById("MainContent_DisStart").checked ? "1" : "0",
                    MsrpOverride: document.getElementById("MainContent_DisMSRP").checked ? "1" : "0",
                    CostOverride: document.getElementById("MainContent_DisCost").checked ? "1" : "0",
                    MMROverride: document.getElementById("MainContent_DisMMR").checked ? "1" : "0",
                    ALPriceOverride: document.getElementById("MainContent_OvrAutoLaunch").checked ? "1" : "0"
                }

                toggleLoading(true, "");
                $.ajax({
                    type: 'POST',
                    url: 'ManageOverrides.aspx/SaveOverrides',
                    data: "{'jsonData': '" + JSON.stringify(dataOut) + "'}",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(XMLHttpRequest.responseJSON.Message);
                    },
                    complete: function (response) {
                        var r = JSON.parse(response.responseJSON.d);
                        if (r.Success == "1") {
                            toggleLoading(false, "Changes saved. Redirecting to Vehicle Management.");
                            window.location.href = `/WholesaleContent/VehicleManagement.aspx`;
                        }
                        else {
                            toggleLoading(false, "");
                            alert("Something has gone wrong in the save process");
                        }
                    }
                });
            }
            else if (action == "cancel") {
                history.back();
            }

            gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
        }
    </script>

    <asp:HiddenField runat="server" ID="hfkListing" />

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="ManageOverrides">
        <div style="display:table;margin:0 auto;">
            <fieldset id="ManageOverrides" class="sectionFieldset">
                <legend>Override Settings</legend>
                <div style="display:table;width:100%;border-collapse:collapse;">
                    <div style="display:table-row;border-bottom: 2px solid gray;padding-bottom:5px;">
                        <asp:Label runat="server" Text="Disable Feed Updates to General Import Data:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisFeed" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable List Price Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisList" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Internet Price Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisInternet" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable MSRP Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisMSRP" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Cost Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisCost" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Detail Description Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisDetail" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Inventory Status Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisStatus" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Inventory Age Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisAge" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Wholsale Floor Price Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisFloor" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Wholesale Buy-It-Now Price Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisBIN" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="Disable Wholesale Start Price Feed Updates:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisStart" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <asp:Label runat="server" Text="AutoLaunch Pricing Override:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="OvrAutoLaunch" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;border-bottom: 2px solid gray">
                        <asp:Label runat="server" Text="Disable MMR Restrictions:" CssClass="ColRowSwap LabelItem ColRowSwapLabel" />
                        <div class="ColRowSwap"><asp:checkbox runat="server" ID="DisMMR" CssClass="SingleIndent" /></div>
                    </div>
                    <div style="display:table-row;">
                        <div style="display:table-cell; padding-top:5px;">
                            <div style="font-weight:bold" class="LabelItem ColRowSwapLabel">
                                <span>Inventory Add Date:</span>
                                <br class="smallShowBlock" style="display:none" />
                                <asp:TextBox ID="InventoryAddDate" textmode="Date" runat="server" CssClass="SingleIndent" />
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
            <div class="row" style="justify-content:center">
                <asp:Button ID='submitButton' OnClientClick="ButtonAction('submit'); return false;" runat='server' Text='Submit' CssClass='actionBackground ActionButton' />
                <asp:Button ID='cancelButton' OnClientClick="history.back(); return false;" runat='server' Text='Cancel' CssClass='actionBackground ActionButton'/>
            </div>
        </div>
    </div>
</asp:Content>