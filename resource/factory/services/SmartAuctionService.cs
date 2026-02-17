using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class SmartAuctionService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public SmartAuctionService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='saInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>SmartAuction Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='saEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='saEnabled' type='checkbox' {auctionInfo.Enabled}/>
                                <div id='saListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='saSellerID'>SmartAuction Consignor ID:&nbsp;</label></div>
                                            <div><input id='saSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='sakWholesaleLocationCode'>Location ID:&nbsp;</label></div>
                                            <div>{auctionInfo.kWholesaleLocationCode}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='saBuyerGroup'>SmartAuction Buyer Group:&nbsp;</label></div>
                                            <div><input id='saBuyerGroup' type='input' {auctionInfo.BuyerGroup} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='sakContactGroup'>SmartAuction Contact Group:&nbsp;</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='saMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='saMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saInvFeedOnly'>Inventory Feed Only:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saInvFeedOnly' type='checkbox' {auctionInfo.InvFeedOnly}/></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saSuppressOptionFilter'>Allow All vehicle Options:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saSuppressOptionFilter' type='checkbox' {auctionInfo.SuppressOptionFilter}/></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport}/></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saAllowSaturdayAuction'>Include Saturday as an Auction Day:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saAllowSaturdayAuction' type='checkbox' {auctionInfo.AllowSaturdayAuction}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='saSwapStockNumber'>Swap Stock # with Dealer Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saSwapStockNumber' type='checkbox' {auctionInfo.SwapStockNumber} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='saSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='saSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divSaSaveAuction' style='text-align:center;'><input id='saSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""sa"", ""2"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                               <div id='saCredDiv'>
                                    <div id='saGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='SmartAuctionJsGrid'></div>
                                        <asp:HiddenField ID='sakCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='saAdd' onclick=""javascript: HandleButtonClick('SmartAuction', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='saEdit' onclick=""javascript: HandleButtonClick('SmartAuction', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 2);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "2");

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
                OnDoubleClickFunction = "saRowDoubleClick();",
                HTMLElement = "SmartAuctionJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "SmartAuction" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_SmartAuctionCredCount').innerHTML = $('#SmartAuctionJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#SmartAuctionJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#SmartAuctionJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Seller ID:100|:BuyerGroup:CarGroup ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|!:kWholesaleAuction:kWholesaleAuction:10|", "", true);

            return grid;
        }
    }
}