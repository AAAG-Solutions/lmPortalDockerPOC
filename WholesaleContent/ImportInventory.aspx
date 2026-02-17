<%@ Page Title="Inventory Import" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportInventory.aspx.cs" Inherits="LMWholesale.ImportInventory" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function fnFileFormatChange() {
            var Dropdownlist = document.getElementById('<%=ddlFileFormat.ClientID %>');
            var SelectedValue = Dropdownlist.options[Dropdownlist.selectedIndex].value;
            var DelimiterSelect = document.getElementById('divImportDelimiter');
            var SampleFileLink = document.getElementById('divSample');
            if (SelectedValue == '4') {
                DelimiterSelect.style.visibility = "visible";
                SampleFileLink.style.visibility = "visible";
            } else {
                DelimiterSelect.style.visibility = "hidden";
                SampleFileLink.style.visibility = "hidden";
            }
        }

        function fnSubmit() {
            if (document.getElementById('<%= uplChooseFile.ClientID %>').value == "") {
                alert("A file must be selected for upload.");
                return false;
            } else {
                return true;
            }
        }

        function AssignExampleFile(filename) {
            $('#<%=ExampleFileName.ClientID %>').val(filename);
            var submitButton = $('#<%=ExampleButton.ClientID %>');
            submitButton.click();
        }

        function importFile(skipUpload) {
            toggleLoading(true, "");

            document.getElementById("MainContent_btnUpload").click();
        }

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <style>
        .SpacedTable {
            width: 100%;
        }
        @media(max-width: 786px) {
            .actionBackground {
                width: 80px !important;
            }
        }
    </style>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="lblImportInventory" class="importVehicle hide_scrollbar">
        <h6 id="breadCrumb" style="font-weight: bold;">Inventory / Import Inventory</h6>
        <h6>&#60; &#8722; <a class='backBreadcrumb' href='javascript: window.location.href="/WholesaleContent/VehicleManagement.aspx";'>Back</a></h6>

        <fieldset id="ImportInfo" class="sectionFieldset">
            <legend>Import Status</legend>
            <div class="row">
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Imported By: </span>
                    <asp:Label ID="lblPerson" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Started: </span>
                    <asp:Label ID="lblStartTime" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Ended: </span>
                    <asp:Label ID="lblEndTime" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Status: </span>
                    <asp:Label ID="lblStatus" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>

                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory"># of Records: </span>
                    <asp:Label ID="lblRecords" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Imported: </span>
                    <asp:Label ID="lblImported" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
                <div class="col-12 col-sm-6 col-md-3">
                    <span class="lblImportInventory">Analyzed: </span>
                    <asp:Label ID="lblAnalyzed" runat="server" CssClass="lblImportInventory"></asp:Label>
                </div>
            </div>
        </fieldset>
        <fieldset id="InventoryFile" class="sectionFieldset">
            <legend>Inventory File</legend>
            <div style="display:table;width:100%;">
                <div style="display:table-row;">
                    <div class="ColRowSwap">
                        <span class="lblImportInventory">File to Import:</span>
                    </div>
                    <div class="ColRowSwap">
                        <asp:FileUpload ID="uplChooseFile" runat="server" Text="Choose File" CssClass="uplImportInventory SingleIndent" />
                    </div>
                </div>
                <div style="display:table-row">
                    <div class="ColRowSwap">
                        <span class="lblImportInventory">File Format:</span>
                    </div>
                    <div class="ColRowSwap">
                        <asp:DropDownList ID="ddlFileFormat" onchange="fnFileFormatChange()" runat="server" CssClass="ddlImportInventory SingleIndent"></asp:DropDownList>
                    </div>
                    <div id="divSample" class="ColRowSwap">
                        <span class="lblImportInventory smallHide">Download Sample File: <a href="javascript:AssignExampleFile('min')">minimum</a> | <a href="javascript:AssignExampleFile('full')">full</a></span>
                    </div>
                </div>
                <div style="display:table-row;">
                    <div class="ColRowSwap">
                        <span class="lblImportInventory">Select Delimiters: </span>
                    </div>
                    <div class="ColRowSwap" id="divImportDelimiter">
                        <asp:RadioButtonList ID="rbDelimiter" runat="server" RepeatColumns="2" CssClass="rbImportInventory SingleIndent SpacedTable">
                            <asp:ListItem Text="Comma (,)" Value="," Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Tab" Value="vbTab"></asp:ListItem>
                            <asp:ListItem Text="Period (.)" Value="."></asp:ListItem>
                            <asp:ListItem Text="Pipe (|)" Value="|"></asp:ListItem>
                            <asp:ListItem Text="Slash (/)" Value="/"></asp:ListItem>
                            <asp:ListItem Text="Other" Value="0"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </fieldset>
        <div class="row">
            <div class="col-12 text-center">
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="actionBackground" Style="width:100px"/>
                <asp:Button ID="btnSubmit" OnClientClick="if (fnSubmit() != false) { importFile(false); return(false); } return(false);" runat="server" Text="Submit" CssClass="actionBackground" Style="width:100px" AutoPostback = false />
                <asp:Button ID="btnCancel" OnClientClick="javascript: window.location.href='/WholesaleContent/VehicleManagement.aspx'; return false;" runat="server" Text="Cancel" CssClass="actionBackground" Style="width:100px" />
                <asp:Button ID="btnUpload" OnClick="Upload" runat="server" Text="Upload" CssClass="actionBackground" Style="display:none;" AutoPostback = false />
                <asp:HiddenField ID="ExampleFileName" runat="server" Value="" />
                <asp:HiddenField ID="hfFileName" runat="server" Value="" />
                <asp:HiddenField ID="hfServerPath" runat="server" Value="" />
                <asp:Button ID="ExampleButton" runat="server" OnClick="ExportExampleFile" Style="display:none;"></asp:Button>
            </div>
        </div>
    </div>

</asp:Content>