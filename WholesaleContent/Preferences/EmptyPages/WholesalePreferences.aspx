<%@ Page Title="Liquid Motors Test Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WholesalePreferences.aspx.cs" Inherits="LMWholesale.WholesaleContent.Preferences.WholesalePreferences" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences">
        <h6 style="font-weight: bold;">
            Dealer Preferences / General / <div class="dropdownHover">
                <span style="text-decoration: underline">Wholesale Setup</span>
                <div class="dropdownHoverContent">
                    <a href="/WholesaleContent/Preferences/General.aspx">General Account Information</a>
                    <a href="/WholesaleContent/Preferences/UserManagement.aspx">User Management</a>
                    <a href="#">Copy Dealer Settings</a>
                    <a href="/WholesaleContent/Preferences/SetupPreferences.aspx">Setup Status</a>
                </div>
            </div>
        </h6>
        <div>test test test test test</div>
    </div>
</asp:Content>