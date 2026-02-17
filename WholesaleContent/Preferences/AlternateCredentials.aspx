<%@ Page Title="Account Alternate Credentials" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AlternateCredentials.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.AlternateCredentials" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <!-- <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Preferences/AlternateCredentials.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" /> -->
    <style type="text/css">
        .dealerPreferences {
            height: calc(100vh - 130px);
        }
    </style>

    <!-- Import Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/AlternateCredentials.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar noPrint"></div>
    <div class="dealerPreferences print">
        <h6 class="noPrint" style="font-weight: bold;">Account Preferences / <div class="dropdownHover">
                <span style="text-decoration: underline">Alternate Credentials</span>
                <div class="dropdownHoverContent">
                    <a id="gpInfo" href="/WholesaleContent/Preferences/General.aspx">General Preferences</a>
                    <a id="alInfo" href="/WholesaleContent/Preferences/AutoLaunchRules.aspx">AutoLaunch Rules</a>
                    <a id="bwRules" href="/WholesaleContent/Preferences/BlackoutWindowRules.aspx">Blackout Window Rules</a>
                    <a id="mpInfo" href="/WholesaleContent/Preferences/MarketPlaceInfo.aspx">Market Place Info</a>
                </div>
            </div>
            <asp:DropDownList runat="server" id="AuctionFilter" CssStyle="">
                <asp:ListItem Text="All" Value="0" />
                <asp:ListItem Text="OVE" Value="1" />
                <asp:ListItem Text="AuctionEdge" Value="7" />
            </asp:DropDownList>
        </h6>
        <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
        <div id="grid" class="preferencesContainer print">
            <div id="jsGrid"></div>
            <asp:HiddenField ID="txtCredential" runat="server" Value="" />
        </div>
    </div>
    <div class="noPrint" style='text-align:-webkit-center;'>
        <asp:Button id='CredentialAdd' runat="server" class='submitButton' style='width:150px;' Text='Add' OnClientClick="javascript: AddAuctionCredential();return false;"/>
        <asp:Button id='CredentialEdit' runat="server" class='submitButton' style='width:150px;' Text='Edit' OnClientClick="javascript: EditAuctionCredential();return false;"/>
        <asp:Button id='CredentialDisable' runat="server" class='submitButton' style='width:150px;' Text='Disable' />
    </div>

    <div id="credentialPopup">
        <div id="credentialPop" class="modalPopup" style="padding-top:75px;">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="toggleCssClass([['credentialPop','show_display']]);return false;">&times;</span>
                    <h2 id="alternateCredTitle" style="text-align:center;"></h2>
                </div>
                <div class="modalBody hide_scrollbar show_scroll" style="height:calc(100vh - 275px);overflow:scroll;">
                    <div style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;">
                        <asp:Button ID='credentialSave' OnClientClick="needImplementation();" runat='server' Text='Save Credential' CssClass='actionBackground' Style='width:125px'/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>