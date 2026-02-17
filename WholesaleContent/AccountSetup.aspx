<%@ Page Title="Account Setup" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountSetup.aspx.cs" Inherits="LMWholesale.WholesaleContent.AccountSetup" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css" />
 
    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/AccountSetup.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <style type="text/css">
        .jsgrid {padding: 10px 0px 0px 0px;}
        .jsgrid-grid-body {height:calc(100vh - 220px);}
        .setupTable {display:table;width:1100px;margin:0px auto;table-layout:fixed;}
        .notesBox {width:1040px;max-width:1040px !important;}
        .blocks {width:1100px; margin:0px auto;}
        .textAlignLeft {text-align:left !important;}
        .inputWidth {width:200px}
        .ddlStyle {height:32px;font-size:1rem}
        @media (max-width: 767px) {
            .jsgrid-grid-body {
                height:calc(100vh - 400px);
            }
            .notesBox {width:250px;}
            .blocks {width:300px;}
            .smallDisplayTable {display:table !important;}
            .smallDisplayTR {display:table-row !important;}
            .ddlStyle{height:27px !important;font-size:.8rem;}
        }
    </style>

    <div id="divAccountSetup" class="accountSetup hide_scrollbar">
        <asp:HiddenField ID="hfApprovalId" Value="" runat="server" />
        <div id="divApproval">
            <fieldset id="fsApproval" class='sectionFieldset' runat="server" >
                <legend>Account Setup Approval</legend>
                <div id="grid" class="hide_scrollbar">
                    <div id="jsGrid" style="padding: 0px 5px 0px 0px;height:33%"></div>
                </div>
                <div style="text-align:center">
                    <asp:Button ID="btnReviewRequest" Text="Review Request" CssClass="SingleIndent actionBackground smallDisplayTR" runat="server" OnClientClick="Javascript: ShowApprovalBox(true); return FillData();"/>
                </div>
            </fieldset>
        </div>
        <div id="divFeedSetup">
            <fieldset id="fsFeedSetup" class='sectionFieldset'>
                <legend>Dealer Feed Setup for Auction</legend>
                <div id="divTablePt1" class="setupTable">
                    <div style="display:table-row;">
                        <div id="divLblAuction" class="ColRowSwapLabel" style="text-align:right;"><label id="lblAuction" for="ddlAuction" style='font-weight:bold'>Auction:&nbsp;</label></div>
                        <div id="divDdlAuction" class="ColRowSwap"><asp:DropDownList ID="ddlAuction" runat="server" CssClass="ddlStyle inputWidth inputStyle sectionDropdown SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="AuctionChange()"/></div>
                        <div id="divLblDealer" class="ColRowSwapLabel" style="text-align:right;"><label id="lblDealer" for="txtDealer" style='font-weight:bold'>Dealership Name:&nbsp;</label></div>
                        <div id="divTxtDealer" class="ColRowSwap"><asp:TextBox ID="txtDealer" runat="server" CssClass="inputWidth inputStyle SingleIndent"></asp:TextBox></div>
                    </div>
                </div>
                <div class="setupTable">
                    <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                        <span style="width:100%;">* Add Auction Prefix to Dealership Name if Applicable *</span>
                    </div>
                </div>
                <div id="divTablePt2" class="setupTable">
                    <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                        <span style="width:100%;font-weight:bold;text-decoration:underline;">Dealership Address</span>
                    </div>
                    <div style="display:table-row;">
                        <div id="divLblStreet" class="ColRowSwapLabel" style="text-align:right;"><label id="lblStreet" for="txtStreet" class="SingleIndent" style='font-weight:bold'>Street:&nbsp;</label></div>
                        <div id="divTxtStreet" class="ColRowSwap"><asp:TextBox ID="txtStreet" runat="server" CssClass="inputWidth inputStyle DoubleIndent"></asp:TextBox></div>
                        <div id="divLblCity" class="ColRowSwapLabel" style="text-align:right;"><label id="lblCity" for="txtCity" class="SingleIndent" style='font-weight:bold'>City:&nbsp;</label></div>
                        <div id="divTxtCity" class="ColRowSwap"><asp:TextBox ID="txtCity" runat="server" CssClass="inputWidth inputStyle DoubleIndent"></asp:TextBox></div>
                    </div>
                    <div style="display:table-row;">
                        <div id="divLblState" class="ColRowSwapLabel" style="text-align:right;"><label id="lblState" for="ddlState" class="SingleIndent" style='font-weight:bold'>State:&nbsp;</label></div>
                        <div id="divDdlState" class="ColRowSwap"><asp:DropDownList ID="ddlState" runat="server" CssClass="ddlStyle inputWidth inputStyle sectionDropdown DoubleIndent" DataValueField="Filter" AutoPostBack="false"/></div>
                        <div id="divLblZipCode" class="ColRowSwapLabel" style="text-align:right;"><label id="lblZipCode" for="txtZipCode" class="SingleIndent" style='font-weight:bold'>Zip Code:&nbsp;</label></div>
                        <div id="divTxtZipCode" class="ColRowSwap"><asp:TextBox ID="txtZipCode" runat="server" CssClass="inputWidth inputStyle DoubleIndent"></asp:TextBox></div>
                    </div>
                    <div style="display:table-row;">
                        <div id="divLblPhone" class="ColRowSwapLabel" style="text-align:right;"><label id="lblPhone" for="txtPhone" class="SingleIndent" style='font-weight:bold'>Phone:&nbsp;</label></div>
                        <div id="divTxtPhone" class="ColRowSwap"><asp:TextBox ID="txtPhone" runat="server" CssClass="inputWidth inputStyle DoubleIndent" placeholder="(123) 456-7890"></asp:TextBox></div>
                        <div class="smallHide" style="display:table-cell">&nbsp;</div>
                        <div class="smallHide" style="display:table-cell">&nbsp;</div>
                    </div>
                </div>
                <div id="divTablePt3" class="setupTable">
                    <div class="SmallLeftAlign" style="display:table-caption;text-align:center;">
                        <span style="width:100%;font-weight:bold;text-decoration:underline;">Main Contact Information</span>
                    </div>
                    <div style="display:table-row;">
                            <div id="divLblName" class="ColRowSwapLabel" style="text-align:right;"><label id="lblName" for="txtName" class="SingleIndent" style='font-weight:bold'>Name:&nbsp;</label></div>
                            <div id="divTxtName" class="ColRowSwap"><asp:TextBox ID="txtName" runat="server" CssClass="inputWidth inputStyle DoubleIndent"></asp:TextBox></div>
                            <div id="divLblOfficeNumber" class="ColRowSwapLabel" style="text-align:right;"><label id="lblOfficeNumber" for="txtOfficeNumber" class="SingleIndent" style='font-weight:bold'>Office Number:&nbsp;</label></div>
                            <div id="divTxtOfficeNumber" class="ColRowSwap"><asp:TextBox ID="txtOfficeNumber" runat="server" CssClass="inputWidth inputStyle DoubleIndent" placeholder="(123) 456-7890"></asp:TextBox></div>
                    </div>
                    <div style="display:table-row;">
                            <div id="divLblCellNumber" class="ColRowSwapLabel" style="text-align:right;"><label id="lblCellNumber" for="txtCellNumber" class="SingleIndent" style='font-weight:bold'>Cell Number:&nbsp;</label></div>
                            <div id="divTxtCellNumber" class="ColRowSwap"><asp:TextBox ID="txtCellNumber" runat="server" CssClass="inputWidth inputStyle DoubleIndent" placeholder="(123) 456-7890"></asp:TextBox></div>
                            <div id="divLblEmail" class="ColRowSwapLabel" style="text-align:right;"><label id="lblEmail" for="txtEmail" class="SingleIndent" style='font-weight:bold'>Email:&nbsp;</label></div>
                            <div id="divTxtEmail" class="ColRowSwap"><asp:TextBox ID="txtEmail" runat="server" CssClass="inputWidth inputStyle DoubleIndent"></asp:TextBox></div>
                    </div>
                </div>
                <div id="divNonTable" class="SmallLeftAlign blocks" style="text-align:center;">
                    <div id="divFeed">
                        <span style="font-weight:bold;">Inventory Feed Received From:</span>
                        <br class="smallShowBlock" style="display:none;" />
                        <label id="lblCompanyName" for="ddlCompanyName" class="SingleIndent">Company Name</label>
                        <br class="smallShowBlock" style="display:none;" />
                        <asp:DropDownList ID="ddlCompanyName" runat="server" CssClass="ddlStyle inputStyle sectionDropdown DoubleIndent" DataValueField="Filter" AutoPostBack="false"/>
                    </div>
                    <div id="divPickup">
                        <span style="font-weight:bold;">Vehicle Pick-up Location:</span>
                        <br class="smallShowBlock" style="display:none;" />
                        <asp:RadioButton ID="chkAuction" groupName="pickupLocation" Text="At Auction" runat="server" CssClass="SingleIndent" />
                        <br class="smallShowBlock" style="display:none;" />
                        <asp:RadioButton ID="chkDealership" groupName="pickupLocation" Text="At Dealership" runat="server" CssClass="SingleIndent" />
                    </div>
                    <div id="divLC">
                        <span style="font-weight:bold;">Add Liquid Connect App for this Account:</span>
                        <br class="smallShowBlock" style="display:none;" />
                        <asp:RadioButton ID="chkYesLC" groupName="LiquidConnect" Text="Yes" runat="server" CssClass="SingleIndent" />
                        <br class="smallShowBlock" style="display:none;" />
                        <asp:RadioButton ID="chkNoLC" groupName="LiquidConnect" Text="No" runat="server" CssClass="SingleIndent" />
                        <br />
                        <span style="word-wrap:break-word; color:red;">Your Manheim sellerIDs are added automatically. If you need additional IDs added, please include them in the notes section below.</span>
                    </div>
                </div>
                <div id="divNotes" class="blocks SmallLeftAlign" style="text-align:center;">
                    <label id="lblNotes" for="txtNotes" style='font-weight:bold'>Notes:&nbsp;</label>
                    <asp:TextBox ID="txtNotes" runat="server" CssClass="inputStyle SingleIndent notesBox"></asp:TextBox>
                    <br />
                    <span>* Please indicate any special requests, including the use of Cost for wholesale pricing, in the Notes section.</span>
                    <br />
                    <span style="color:red;">*** THE SETUP PROCESS TAKES A MINIMUM 5-10 BUSINESS DAYS ***<br />WE WILL NOTIFY YOU BY EMAIL WHEN YOUR ACCOUNT IS COMPLETED</span>
                </div>
                <div id="divButtons" class="blocks SmallLeftAlign" style="text-align:center;" >
                    <asp:HiddenField runat="server" ID="SignupSource" Value="" />
                    <asp:Button ID="btnSubmit" Text="Submit" CssClass="SingleIndent actionBackground" runat="server" OnClientClick="if(!(ValidateSignup('submit'))){return false;}" OnClick="bntSubmitSignup" UseSubmitBehavior="false" />
                    <asp:Button ID="btnCancel" Text="Cancel" CssClass="SingleIndent actionBackground smallDisplayTR" runat="server" OnClientClick="javascript: window.location.replace(window.location.href.replace('AccountSetup', 'WholesaleDefault')); return false;"/>
                </div>
            </fieldset>
            <fieldset id="fsApprovalBox" class='sectionFieldset' style="display:none;" runat="server">
                <legend>Approval Box</legend>
                <div id="divAccountTemplate" class="SmallLeftAlign" style="text-align:center;">
                    <span style="font-weight:bold;">Copy Default Account Template:</span>
                    <br />
                    <asp:DropDownList ID="ddlAccountTemplate" runat="server" CssClass="sectionDropdown SingleIndent" DataValueField="Filter" AutoPostBack="false" onchange="TemplateChange(this)" />
                    <asp:HiddenField ID="kTemplate" runat="server" Value="" />
                    <div id="divCheckboxes" class="setupTable" style="margin-top:10px;">
                        <div style="display:table-row;">
                            <label id="lblAddress" for="chkAddress" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Address:&nbsp;</label>
                            <asp:CheckBox ID="chkAddress" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                            <label id="lblUsers" for="chkUsers" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Users:&nbsp;</label>
                            <asp:CheckBox ID="chkUsers" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                            <label id="lblContactGroups" for="chkContactGroups" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Contact Groups:&nbsp;</label>
                            <asp:CheckBox ID="chkContactGroups" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                        </div>
                        <div style="display:table-row;">
                            <label id="lblLotLocations" for="chkLotLocations" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Lot Locations:&nbsp;</label>
                            <asp:CheckBox ID="chkLotLocations" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                            <label id="lblWholesaleAuctions" for="chkWholesaleAuctions" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Wholesale Auctions:&nbsp;</label>
                            <asp:CheckBox ID="chkWholesaleAuctions" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                            <label id="lblAlternateCredentials" for="chkAlternateCredentials" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Alternate Credentials:&nbsp;</label>
                            <asp:CheckBox ID="chkAlternateCredentials" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft"  Checked="true"/>
                        </div>
                        <div style="display:table-row;">
                            <label id="lblAutoLaunchRules" for="chkAutoLaunchRules" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>AutoLaunch Rules:&nbsp;</label>
                            <asp:CheckBox ID="chkAutoLaunchRules" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" />
                            <label id="lblBlackoutRules" for="chkBlackoutRules" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Blackout Rules:&nbsp;</label>
                            <asp:CheckBox ID="chkBlackoutRules" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" />
                            <label id="lblProducts" for="chkProducts" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>Products:&nbsp;</label>
                            <asp:CheckBox ID="chkProducts" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" Checked="true" />
                        </div>
                    </div>
                    <div class="setupTable SmallLeftAlign" style="text-align:center">
                        <label id="lblSuppressEmails" for="chkSuppressEmails" class="ColRowSwapLabel" style='font-weight:bold;text-align:right;'>SUPPRESS AUCTION EMAILS:&nbsp;</label>
                        <asp:CheckBox ID="chkSuppressEmails" runat="server" CssClass="ColRowSwap SingleIndent textAlignLeft" />
                    </div>
                    <div class="blocks smallDisplayTable" style="text-align:center;display:flex;justify-content:space-between;margin-top:10px;">
                        <asp:Button ID="btnApprove" Text="Approve Account Request" CssClass="SingleIndent actionBackground smallDisplayTR" runat="server" OnClientClick="ValidateSignup('approve');" OnClick="bntSubmitSignup" UseSubmitBehavior="false" />
                        <asp:Button ID="btnDeny" Text="Deny Account Request" CssClass="SingleIndent actionBackground smallDisplayTR" runat="server" OnClientClick="ValidateSignup('deny');" OnClick="bntSubmitSignup" UseSubmitBehavior="false" />
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
    <script type="text/javascript">
        var pause_timeout = null;

        var mainPhoneTextInput = document.getElementById('<%=txtPhone.ClientID %>');
        var officeNumberTextInput = document.getElementById('<%=txtOfficeNumber.ClientID %>');
        var cellNumberTextInput = document.getElementById('<%=txtCellNumber.ClientID %>');
        OnChangePause(mainPhoneTextInput, HandleChangePause, 100);
        OnChangePause(officeNumberTextInput, HandleChangePause, 100);
        OnChangePause(cellNumberTextInput, HandleChangePause, 100);
    </script>
</asp:Content>
