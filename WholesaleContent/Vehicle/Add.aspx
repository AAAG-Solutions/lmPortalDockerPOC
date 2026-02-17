<%@ Page Title="Vehicle Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Add.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.Add" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/Add.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/Add.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <asp:HiddenField ID="styleCode" runat="server" />
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>

    <div class="addVehicle">
        <div id="lblAddVehicle" class="hide_scrollbar">
            <h6 id="breadCrumb" style="font-weight: bold;">Inventory / Add Vehicle</h6>
            <h6>&#60; &#8722; <a class='backBreadcrumb' href="javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx';">Back</a></h6>

            <div class="Content">
                <fieldset id="AddVehicle" class="sectionFieldset">
                    <legend>Add Vehicle</legend>
                        <div class="ContentTable">
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel">Account Name:</div>
                                <div class="ColRowSwap" style="padding: 2px;"><asp:Label runat="server" ID="AccountName" CssClass="SingleIndent" /></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">VIN: </div>
                                <div class="ColRowSwap">
                                    <div><asp:Textbox ID="InputVIN" runat="server" CssClass="inputStyle SingleIndent"/></div>
                                </div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Stock Number: </div>
                                <div class="ColRowSwap"><asp:Textbox ID="InputStock" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            </div>
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel">Override Import: </div>
                                <div class="ColRowSwap" style="padding: 2px;"><asp:CheckBox runat="server" ID="OverrideImport" Font-Bold="True" CssClass="SingleIndent"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Vehicle Type: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="VehicleType" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;">
                                        <asp:ListItem Selected="true" Value="1">Passenger Vehicle</asp:ListItem>
                                        <asp:ListItem Value="2">Motorcycle</asp:ListItem>
                                        <asp:ListItem Value="3">Commercial Vehicle</asp:ListItem>
                                        <asp:ListItem Value="4">Miscellaneous</asp:ListItem>
                                        <asp:ListItem Value="5">RV</asp:ListItem>
                                        <asp:ListItem Value="6">Boat</asp:ListItem>
                                        <asp:ListItem Value="7">ATV</asp:ListItem>
                                        <asp:ListItem Value="8">Airplane</asp:ListItem>
                                        <asp:ListItem Value="9">Watercraft</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Classification: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="Classification" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;">
                                        <asp:ListItem Selected="true" Value="-- Select Inventory Type --">-- Select Inventory Type --</asp:ListItem>
                                        <asp:ListItem Value="32">Certified Pre-Owned</asp:ListItem>
                                        <asp:ListItem Value="33">Dealer Certified Pre-Owned</asp:ListItem>
                                        <asp:ListItem Value="31">New</asp:ListItem>
                                        <asp:ListItem Value="30">Pre-Owned</asp:ListItem>
                                    </asp:DropDownList></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Vehicle Status: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="VehicleStatus" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;">
                                        <asp:ListItem Selected="true" Value="-- Select Inventory Status --">-- Select Inventory Status --</asp:ListItem>
                                        <asp:ListItem Value="1">Available</asp:ListItem>
                                        <asp:ListItem Value="2">Unavailable</asp:ListItem>
                                        <asp:ListItem Value="3">On Hold</asp:ListItem>
                                        <asp:ListItem Value="4">Demo</asp:ListItem>
                                        <asp:ListItem Value="5">Returned</asp:ListItem>
                                        <asp:ListItem Value="9">In-Transit</asp:ListItem>
                                    </asp:DropDownList></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">No VIN Explosion: </div>
                                <div class="ColRowSwap" style="padding: 5px 2px 0px 2px;">
                                    <asp:CheckBox Id="vinExplosion" runat="server" Font-Bold="True" OnChange="VinExplosion();" CssClass="SingleIndent"/>
                                </div>
                            </div>
                            <div ID="YearListDropdown" style="display:none;">
                                <div class="ColRowSwapLabel">Select a Year: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="YearList" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;" onchange="GetMakeList();" ></asp:DropDownList></div>
                            </div>
                            <div ID="MakeListDropdown" style="display:table-row;display:none;">
                                <div class="ColRowSwapLabel">Select a Make: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="MakeList" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;" onchange="GetModelList();" ></asp:DropDownList></div>
                            </div>
                            <div ID="ModelListDropdown" style="display:table-row;display:none;">
                                <div class="ColRowSwapLabel">Select a Model: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="ModelList" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;" onchange="GetStyleList();"></asp:DropDownList></div>
                            </div>
                            <div ID="StyleListDropdown" style="display:table-row;display:none;">
                                <div class="ColRowSwapLabel">Select a Style: </div>
                                <div class="ColRowSwap"><asp:DropDownList ID="StyleList" runat="server" CssClass="inputStyle SingleIndent" style="width: 250px;"></asp:DropDownList></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Cost: </div>
                                <div class="ColRowSwap"><asp:Textbox ID="InputCost" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Mileage: </div>
                                <div class="ColRowSwap"><asp:Textbox ID="InputMileage" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Retail List Price: </div>
                                <div class="ColRowSwap"><asp:Textbox ID="InputPrice" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            </div>
                            <div style="display:table-row">
                                <div class="ColRowSwapLabel">Retail Internet Price: </div>
                                <div class="ColRowSwap"><asp:Textbox ID="SpecialPrice" runat="server" CssClass="inputStyle SingleIndent"/></div>
                            </div>
                        </div>
                    <div class="ContentFinal">
                        <button class="submitButton" onclick="checkVIN(); return false;">Save</button>
                    </div>
                </fieldset>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var pause_timeout = null;

        function OnChangePause(input, func, delay) {
            if (delay == null)
                delay = 500;
            input.onkeyup = function (e) {
                clearTimeout(pause_timeout);
                pause_timeout = setTimeout(function () { func(input.value); }, delay);
            };
        }

        function DefaultStock(InputVin) {
            // Default Stock # is given based off VIN
            // User can always override it
            var defaultStock = InputVin.substring(9);
            var textBox = document.getElementById('MainContent_InputStock');
            textBox.value = defaultStock;
        }

        var textInput = document.getElementById('<%=InputVIN.ClientID %>');
        OnChangePause(textInput, DefaultStock, 500);

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>
</asp:Content>