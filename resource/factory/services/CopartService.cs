using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class CopartService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public CopartService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='copartInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>COPART Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='copartEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='copartEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='copartListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='copartSellerID'>COPART Company Code:&nbsp;</label></div>
                                            <div><input id='copartSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='copartBuyerGroup'>COPART Seller Code:&nbsp;</label></div>
                                            <div><input id='copartBuyerGroup' type='input' {auctionInfo.BuyerGroup} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='copartkContactGroup'>COPART Contact Group:&nbsp;</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='copartServiceProviderName'>COPART Vender Code:&nbsp;</label></div>
                                            <div><input id='copartServiceProviderName' type='input' {auctionInfo.ServiceProviderName} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='copartMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='copartMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='copartAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='copartRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='copartALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='copartAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='cpoartExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='copartIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='copartAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='copartSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='copartSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divCopartSaveAuction' style='text-align:center;'><input id='copartSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""copart"", ""6"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                               <div id='copartCredDiv'>
                                   <div id='copartGrid' class='credentialGrid hide_scrollbar'>
                                       <div id='COPARTJsGrid'></div>
                                       <asp:HiddenField ID='copartkCred' runat='server' Value="""" />
                                   </div>
                                   <div style='text-align-last:center;'>
                                       <button id='copartAdd' onclick=""javascript: HandleButtonClick('COPART', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                       <button id='copartEdit' onclick=""javascript: HandleButtonClick('COPART', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                   </div>
                               </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 6);

            DataTable dt = new DataTable();
            if (result.Result == Wholesale.ReturnCode.LM_SUCCESS)
            {
                DataTable auctionInfoTbl = result.Data.Tables[0];
                return auctionInfoTbl;
            }

            // Return empty DataTable if we fail for some reason
            return dt;
        }

        public DataTable GetCredentials(string kSession, int kDealer)
        {
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "6");

            if (result.Result == Lookup.ReturnCode.LM_SUCCESS)
                return result.Data.Tables[0];

            // Return empty DataTable if we fail for some reason
            return new DataTable();
        }

        public jsGridBuilder GetJsGridBuilderInfo(string methodUrl)
        {
            jsGridBuilder grid = new jsGridBuilder
            {
                MethodURL = methodUrl,
                OnRowSelectFunction = "GridRowSelected",
                OnClearRowSelectFunction = "ClearRowSelection",
                OnDoubleClickFunction = "copartRowDoubleClick();",
                HTMLElement = "COPARTJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "COPART" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_COPARTCredCount').innerHTML = $('#COPARTJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#COPARTJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#COPARTJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Company Code:100|:BuyerGroup:Seller Code:100|:ServiceProviderName:Vendor Code:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}