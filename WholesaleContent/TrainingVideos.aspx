<%@ Page Title="Training Videos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainingVideos.aspx.cs" Inherits="LMWholesale.WholesaleContent.TrainingVideos" EnableEventValidation="false" %>
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
        <div id="trainingVideos" class="preferencesContainer" style="height:auto !important;padding-right:5px;overflow:unset;">
            <fieldset id='settings' class='sectionFieldset' style='position: relative;height:calc(100vh - 100px);'>
                <div id="settingsTbl" style="display: table;margin: 0 auto;padding:20px;">
                    <div class="singleRow">
                        <div class="centerContent" style="text-align-last:center;padding-top:10px;">
                            <div style="text-align-last:center;font-weight:bold;font-size:30px;padding:15px;">Training Videos</div>
                            <ul id="vidLst" style="list-style:circle;padding:0;list-style-position:inside;">
                                <!-- General Training -->
                                <li id="otgTraining" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=TrainingOTG">Outside the Gate Training - 54 minutes</a></li>
                                <li id="wholesaleOverview" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=WholesalePortalOverview">Wholesale Portal Overview - 37 minutes</a></li>
                                <li id="LCApp" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=LC">Liquid Connect App Training - 29 minutes</a></li>

                                <!-- Quick One-Off Subjects -->
                                <li id="addOTG" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=AddOTG">How to Add Outside the Gate Accounts - 8 minutes</a></li>
                                <li id="umalItem" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=AutoLaunchOTG">How to Add Auto Launch Rules in OTG Accounts - 13 minutes</a></li>
                                <li id="manualList" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=ListManual">How to Manually List Vehicles - 12 minutes</a></li>
                                <li id="deactivateList" style="text-align-last:left;padding-bottom:5px;"><a target="_blank" href="/WholesaleData/TrainingVideo.aspx?VideoName=DeactivateListings">How to Deactivate Listings - 5 minutes</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
    </div>
</asp:Content>
