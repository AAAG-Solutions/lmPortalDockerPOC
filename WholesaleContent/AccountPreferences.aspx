<%@ Page Title="Account Preferences" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountPreferences.aspx.cs" Inherits="LMWholesale.WholesaleContent.AccountPreferences" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <style type='text/css'>

        @media (max-width: 450px) {
            .sectionFieldset {
                height: calc(100vh - 20px) !important;
            }
        }
    </style>

    <!-- Import Page Scripts -->
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences hide_scrollbar" style="overflow:scroll;">
        <div id="generalPreferences" class="preferencesContainer" style="height:auto !important;padding-right:5px;overflow:unset;">
            <fieldset id='settings' class='sectionFieldset' style='position: relative;height:calc(100vh - 100px);'>
                <legend>Account Preferences</legend>
                <div id="settingsTbl" style="display: table;margin: 0 auto;padding:50px;">
                    <div class="singleRow">
                        <div id="GeneralPref" runat="server" class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/General.aspx">
                                <img id="preferenceIcon" title="General Preferences" src="/Images/fa-icons/cog.svg" style="height:60px;width:60px;"/>
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">General Preferences</div>
                        </div>
                        <div class="centerCell" style="width:20px;"></div>
                        <div id="AutoLaunchRules" runat="server" class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/AutoLaunchRules.aspx">
                                <img id="autoLaunchIcon" title="AutoLaunch Rules" src='/Images/fa-icons/list-check.svg' style='height:60px;width:60px;' />
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">AutoLaunch Rules</div>
                        </div>
                        <div class="centerCell" style="width:20px;"></div>
                        <div id="BlackoutWindows" runat="server"  class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/BlackoutWindowRules.aspx">
                                <img id="blackoutIcon" title="BlackOut Window Rules" src='/Images/fa-icons/rectangle-list.svg' style='height:60px;width:60px;' />
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">Blackout Window Rules</div>
                        </div>
                        <div class="centerCell" style="width:20px;"></div>
                        <div id="MarketPlaceInfo" runat="server" class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/MarketPlaceInfo.aspx">
                                <img id="marketplaceIcon" title="Market Place Info" src='/Images/fa-icons/shop.svg' style='height:60px;width:60px;' />
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">Market Place Info</div>
                        </div>
                    </div>
                    <div class="singleRow" style="padding-top:20px;display:none;">
                        <div id="AlternateCredentials" runat="server" class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/AlternateCredentials.aspx">
                                <img id="altCredIcon" title="Alternate Credentials" src='/Images/fa-icons/id-card.svg' style='height:60px;width:60px;' />
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">Alternate Credentials</div>
                        </div>
                        <div class="centerCell" style="width:20px;"></div>
                        <div id="UserManagement" runat="server" class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <a href="/WholesaleContent/Preferences/UserManagement.aspx">
                                <img id="usermanagementIcon" title="User Management" src='/Images/fa-icons/user-pen.svg' style='height:60px;width:60px;' />
                            </a>
                            <div style="text-align-last:center;font-weight:bold;font-size:20px;padding-top:15px;">User Management</div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
</asp:Content>
