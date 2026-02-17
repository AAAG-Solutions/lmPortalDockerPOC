<%@ Page Title="General Account Info" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.General" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/General.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <style>
        @media(max-width:786px) {
            .dealerPreferences {
                overflow-y: scroll;
                height: calc(100vh - 95px);
                padding-right: 5px;
            }
        }
    </style>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences">
        <h6 style="font-weight: bold;">Account Preferences / <div class="dropdownHover">
                <span style="text-decoration: underline">General Preferences</span>
                <div class="dropdownHoverContent">
                    <a id="alRules" href="/WholesaleContent/Preferences/AutoLaunchRules.aspx">AutoLaunch Rules</a>
                    <a id="bwInfo" href="/WholesaleContent/Preferences/BlackoutWindowRules.aspx">Blackout Window Rules</a>
                    <a id="mpInfo" href="/WholesaleContent/Preferences/MarketPlaceInfo.aspx">Market Place Info</a>
                </div>
            </div>
        </h6>
        <div id="general">
            <div id="generalPreferences" class="preferencesContainer" style="height:auto !important;">
                <fieldset id='settings' class='sectionFieldset' style='position: relative;'>
                    <legend>General Preferences</legend>
                    <div id="generalSettingTbl" style="display: table;margin: 0 auto;">
                        <div class="singleRow">
                            <div class="centerCell"><b>Account:&nbsp;</b></div>
                            <div class="centerContent"><asp:Label ID="accountName" runat="server"></asp:Label></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Display Name:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="accountDisplay" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Street:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="adressStreet" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>City:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="addressCity" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>State:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstState" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Zip Code:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="addressZip" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Country:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstCountry" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Phone:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="dealerPhone" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Timezone:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstTimeZone" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Simple Template:&nbsp;</b></div>
                            <div class="centerContent"><input id="chkTemplate" runat="server" type="checkbox" class="inputStyle" disabled="disabled" /></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Service Type:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstService" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Customer Type:&nbsp;</b></div>
                            <div class="centerContent"><asp:Label ID="customType" runat="server"></asp:Label></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Account Group:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstAccountGrp" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Account Type:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstAccountType" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Account Sub Group:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstSubAccountGrp" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Distributor:&nbsp;</b></div>
                            <div class="centerContent"><asp:DropDownList ID="lstDistributor" runat="server" CssClass="inputStyle" enabled="false"></asp:DropDownList></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Administrative Hold:&nbsp;</b></div>
                            <div class="centerContent"><input id="chkHold" runat="server" type="checkbox" class="inputStyle" disabled="disabled"/></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Demo Account:&nbsp;</b></div>
                            <div class="centerContent"><input id="chkDemo" runat="server" type="checkbox" class="inputStyle" disabled="disabled" /></div>
                        </div>
                    </div>
                    <br/>
                    <div id="contactTbl" style="display: table;margin: 0 auto;">
                        <div class="singleRow">
                            <div class="centerCell"><b>Dealer Contact First Name:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="DealerContactFName" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Dealer Contact Last Name:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="DealerContactLName" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Dealer Contact Email:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="DealerContactEmail" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Dealer Contact Phone:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="DealerContactPhone" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Rep First Name:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="OwnerFName" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Rep Last Name:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="OwnerLName" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell"><b>Rep Email:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="OwnerEmail" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                            <div class="centerCell" style="width:50px;"></div>
                            <div class="centerCell"><b>Rep Phone:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox ID="OwnerPhone" runat="server" CssClass="inputStyle" enabled="false"></asp:TextBox></div>
                        </div>

                    </div>
                    <br/>
                    <div style="display: table;margin: 0 auto;">
                        <div id="AccountProducts" class="singleRow" runat="server" style="display:none;">
                            <div class="centerCell" style="width:200px;"><b>Subscribed Products:&nbsp;</b></div>
                            <div class="centerContent"><asp:Label ID="dealerProducts" runat="server"></asp:Label></div>
                        </div>
                        <div class="singleRow">
                            <div class="centerCell" style="width:150px;"><b>Website URL:&nbsp;</b></div>
                            <div class="centerContent"><asp:TextBox id="dealerWebsite" runat="server" style="width:750px;" CssClass="inputStyle" Enabled="false"></asp:TextBox></div>
                        </div>
                    </div>
                    <br/>
                    <div style="text-align-last:center;"><input id="GeneralInfoSave" type="button" class="actionBackground" value="Save" onclick="SaveGeneral(); return false;" style="display:none;"></div>
                </fieldset>
            </div>
        </div>
    </div>
</asp:Content>
