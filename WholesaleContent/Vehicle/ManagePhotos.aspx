<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagePhotos.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.ManagePhotos" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/ManagePhotos.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/ManagePhotos.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="TopButtonArea" class="actionsArea">
        <asp:Button ID="btnDelVehPhotos" CssClass="submitButton" text="Delete All" runat="server" OnClientClick="DeleteAllPhotos('vehicle'); return false;"/>
        <asp:Button ID="btnDelDamPhotos" CssClass="submitButton" text="Delete All" runat="server" style="display: none;" OnClientClick="DeleteAllPhotos('damage'); return false;" />
        <asp:Button ID="btnVehPhotos" CssClass="submitButton" text="Vehicle Photos" runat="server" style="display: none;" OnClientClick="SwapPhotoView(); return false;" />
        <asp:Button ID="btnDamPhotos" CssClass="submitButton" text="Damage Photos" runat="server" OnClientClick="SwapPhotoView(); return false;" />
        <asp:Button ID="btnUploadPhotos" CssClass="submitButton" text="Upload Photos" runat="server" />
    </div>
    <div id="divVehiclePhotos" style="padding: 0px 15px;">
        <fieldset id="fsVehiclePhotos" class="sectionFieldset">
        <legend>Vehicle Photos</legend>
            <asp:panel id="panVehPhotoArea" class="photoArea" runat="server">

            </asp:panel>
        </fieldset>
    </div>
    <div id="divDamagePhotos" style="padding: 0px 15px; display: none;">
        <fieldset id="fsDamagePhotos" class="sectionFieldset">
        <legend>Damage Photos</legend>
            <asp:panel id="panDamPhotoArea" class="photoArea" runat="server">

            </asp:panel>
        </fieldset>
    </div>
    <div id="BottomButtonArea" style="text-align: center; padding-top: 5px;">
        <asp:Button ID="Save" CssClass="submitButton" text="Save Photos" runat="server" OnClientClick="SubmitClick(); return false;" />
        <asp:Button ID="Cancel" CssClass="submitButton" text="Cancel" runat="server" OnClientClick="history.back(); return false;" />
    </div>
</asp:Content>