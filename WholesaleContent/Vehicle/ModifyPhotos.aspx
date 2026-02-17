<%@ Page Title="Modify Photos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModifyPhotos.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.ModifyPhotos" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <style type='text/css'>
        #ulPhotos, #ulDamagePhotos { list-style-type: none; margin: 0; float:left;padding: 0; overflow:auto;height:745px; }
        #ulPhotos li, #ulDamagePhotos li { margin: 3px 3px 3px 0; padding: 1px; float:left;text-align: center;border:2px solid black; }
        
        li.thPhotos img
        {
            height:96px;
            width:128px;            
        }
        @media (max-width: 450px) {
            .sectionFieldset {
                height: calc(100vh - 20px) !important;
            }
        }
    </style>

    <!-- Import Page Scripts -->
    <script src="/Scripts/jquery-ui.js?v=<% Response.Write(Application["ContentVersion"]);%>" type="text/javascript"></script>
    <script src="/Scripts/jquery-ui.min.js?v=<% Response.Write(Application["ContentVersion"]);%>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#ulPhotos").sortable({
                cursor: 'move',
                stop: function (eventu, ui) {
                    //set_selection_text("ulPhotos");
                    var A = $("#ulPhotos").sortable("toArray");
                    document.getElementById("MainContent_ulPhotos_list").value = A.join("|");
                }
            });
            /*  $("#ulPhotos, #ulDamagePhotos").disableSelection();
            $("#ulPhotos li, #ulDamagePhotos li").click(function () {
                toggle_select(this);
            });*/
        });
    </script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div class="dealerPreferences hide_scrollbar" style="overflow:scroll;">
        <h6 id="BackToVehicle" runat="server"></h6>
        <div id="generalPreferences" class="preferencesContainer" style="height:auto !important;padding-right:5px;overflow:unset;">
            <div class="row">
                <div class="col-md">
                    <fieldset id='photoContainer' class='sectionFieldset' style='position: relative;height:calc(100vh - 125px);'>
                        <legend>Photos</legend>
                        <asp:PlaceHolder ID="phPhotos" runat="server"></asp:PlaceHolder>
                    </fieldset>
                </div>
                <div class="col-3">
                    <fieldset id='damagesContainer' class='sectionFieldset' style='position: relative;height:calc(100vh - 125px);'>
                        <legend>Damages</legend>
                    </fieldset>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="ulPhotos_list" runat="server" Value=""/>
    </div>

    <script>
        $('#ulPhotos li').addClass("thPhotos");
    </script>
</asp:Content>
