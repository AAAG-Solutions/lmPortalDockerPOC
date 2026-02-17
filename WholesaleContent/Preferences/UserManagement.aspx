<%@ Page Title="User Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.UserManagement" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Preferences/UserManagement.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <style type='text/css'>
        #usersGrid .jsgrid-grid-body {height: calc(100vh - 275px);}
        #existingUser #searchGrid .jsgrid-grid-body {height: calc(100vh - 800px);}
    </style>

    <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences hide_scrollbar" style="overflow:scroll;">
        <div id="generalPreferences" class="preferencesContainer" style="height:auto !important;padding-right:5px;">
            <fieldset id='settings' class='sectionFieldset' style='position: relative;'>
                <legend>Users Management</legend>
                <div id="settingsTbl" style="display: table;margin: 0 auto;">
                    <div id="usersGrid"></div>
                    <asp:HiddenField ID='kPerson' runat='server' Value="" />
                </div>
                <div style="text-align-last:center;">
                    <asp:Button ID='btnAdd' OnClientClick="AddUser();return false;" runat='server' Text='Add' CssClass='actionBackground' Style='width:125px'/>
                    <asp:Button ID='btnEdit' OnClientClick="EditUser();return false;" runat='server' Text='Edit' CssClass='actionBackground' Style='width:125px'/>
                    <asp:Button ID='btnRemove' OnClientClick="RemoveUser();return false;" runat='server' Text='Remove' CssClass='actionBackground' Style='width:125px'/>
                </div>
            </fieldset>
        </div>
    </div>

    <div id="umPopups">
        <div id="userModal" class="modalPopup" style="padding-top:75px;">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="toggleCssClass([['userModal','show_display']]);return false;">&times;</span>
                    <h2 id="umPopupTitle" style="text-align: center;">User Management</h2>
                </div>
                <div class="modalBody hide_scrollbar show_scroll" style="height:calc(100vh - 275px);overflow:scroll;">
                    <label id="operation" value="" style="display:none;"></label>
                    <fieldset id="userSettings" class="sectionFieldset" style="background-color:transparent;">
                        <legend style="color:black;border:0!important;background-color:white !important;">User Settings</legend>
                        <div id="userButtons" style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;">
                            <asp:Button ID='addNewUser' OnClientClick="toggleOption('add');return false;" runat='server' Text='New User' CssClass='actionBackground' Style='width:125px'/>
                            <asp:Button ID='addExistingUser' OnClientClick="toggleOption('existing');return false;" runat='server' Text='Existing User' CssClass='actionBackground' Style='width:125px'/>
                        </div>
                        <div id="accountName" runat="server" style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;font-size: 20px;font-weight: bold;text-decoration: underline;"></div>
                        <div id="userSettingsTbl" style="margin: 0 auto;display:table;table-layout:fixed;">
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">First Name:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="FName" runat="server" placeholder="User First Name" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Last Name:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="LName" runat="server" placeholder="User Last Name" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Email:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="userEmail" runat="server" placeholder="User Email" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Work Number:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="workNumber" runat="server" placeholder="Work Number" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Cell Number (#s only):</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="cellNumber" runat="server" placeholder="Cell Number" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;width:55%;"></div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <div id="smsTexting"><input type="checkbox" id='chksmsText' class="SingleIndent"/><label for='chksmsText' style="font-weight:bold;text-decoration:underline;">&nbsp;Allow SMS Texting</label></div>
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">User ID:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="userID" runat="server" placeholder="User ID" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Auction Access ID:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="auctionAccessUserID" runat="server" placeholder="Auction Access ID" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                            <div style="display:flex;flex-direction:row;">
                                <div class="ColRowSwapLabel" style="padding:2px;font-weight:bold;text-decoration:underline;margin:auto 0 auto auto;text-align:right;">Manheim User ID:</div>
                                <div class="ColRowSwap" style="padding:2px;">
                                    <asp:Textbox id="manheimUserID" runat="server" placeholder="Manheim ID" CssClass="inputStyle" Style="text-align:center;" />
                                </div>
                            </div>
                        </div>
                        <div id="existingUser" style="display:none;">
                            <div style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;font-weight:bold;">
                                Search Users: <asp:Textbox id="searchUsers" runat="server" CssClass="inputStyle" Style="text-align:center;" />
                            </div>
                            <div id="searchGrid"></div>
                            <asp:HiddenField ID='selectedUser' runat='server' Value="" />
                        </div>
                        <div id="relationShip" style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;font-weight:bold;">
                            Relationship: <asp:DropDownList id="userRelationship" runat="server" CssClass="inputStyle"></asp:DropDownList>
                        </div>
                        <div id="relations" style="margin: 0 auto;display:table;table-layout:fixed;">
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel">
                                    <label for='lmQueue' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Lead Management Queue:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='lmQueue' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel">
                                    <label for='lmAdmin' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Lead Management Admin:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='lmAdmin' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel">
                                    <label for='invenAdmin' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Inventory Administrator:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='invenAdmin' class="SingleIndent"/>
                                </div>
                            </div>
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel">
                                    <label for='wholesaleAdmin' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Wholesale Admin:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='wholesaleAdmin' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel">
                                    <label for='wholesaleInspector' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Wholesale Inspector:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='wholesaleInspector' class="SingleIndent" onchange="toggleInspectionCompany();return false;"/>
                                </div>
                                <div class="ColRowSwapLabel" style="padding:5px;font-weight:bold;text-decoration:underline;text-align:right;">
                                    Inspection Company:&nbsp;
                                </div>
                                <div class="ColRowSwap" style="padding:5px;font-weight:bold;text-decoration:underline;flex:1;">
                                    <asp:DropDownList id="lstInspectionCompany" runat="server" disabled="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div style="display:table-row;">
                                <div class="ColRowSwapLabel">
                                    <label for='mobileData' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Mobile Data Collector:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='mobileData' class="SingleIndent"/>
                                </div>
                                <div id="alOverrideLbl" runat="server" class="ColRowSwapLabel">
                                    &nbsp;&nbsp;<label for='retailALOverride' style="font-weight:bold;text-decoration:underline;">Enable UMAL Retail Override:&nbsp;</label>
                                </div>
                                <div id="alOverrideChk" runat="server" class="ColRowSwap" style="padding:5px;display:none;">
                                    <input type="checkbox" id='retailALOverride' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel"></div><div class="ColRowSwap" style="padding:5px;"> </div>
                            </div>
                            <div id="Wholesale" runat="server" style="display:table-row;display:none;">
                                <div class="ColRowSwapLabel"></div><div class="ColRowSwap" style="padding:5px;"> </div>
                                <div class="ColRowSwapLabel">
                                    <label for='wholesaleBuyer' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Wholesale Buyer:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='wholesaleBuyer' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel">
                                    <label for='WholesaleSeller' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Wholesale Seller:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='wholesaleSeller' class="SingleIndent"/>
                                </div>
                            </div>
                            <div id="Appraisal" runat="server" style="display:table-row;display:none;">
                                <div class="ColRowSwapLabel"></div><div class="ColRowSwap" style="padding:5px;"> </div>
                                <div class="ColRowSwapLabel">
                                    <label for='Appraiser' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">Appraiser:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='Appraiser' class="SingleIndent"/>
                                </div>
                                <div class="ColRowSwapLabel">
                                    <label for='SalesPerson' style="font-weight:bold;text-decoration:underline;text-align:right;width:100%;">SalesPerson:&nbsp;</label>
                                </div>
                                <div class="ColRowSwap" style="padding:5px;">
                                    <input type="checkbox" id='SalesPerson' class="SingleIndent"/>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                    <div id="btnOptions" style="width:100%;text-align-last:center;padding-top:5px;padding-bottom:5px;">
                        <asp:Button ID='btnAddSave' OnClientClick="SaveUser();return false;" runat='server' Text='Save' CssClass='actionBackground' Style='width:125px'/>
                        <asp:Button ID='btnAddClose' OnClientClick="toggleCssClass([['userModal','show_display']]);return false;" runat='server' Text='Close' CssClass='actionBackground' Style='width:125px'/>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var pause_timeout = null;

        var textInput = document.getElementById('<%=searchUsers.ClientID %>');
        OnChangePause(textInput, HandleChangePause, 300);
    </script>
</asp:Content>