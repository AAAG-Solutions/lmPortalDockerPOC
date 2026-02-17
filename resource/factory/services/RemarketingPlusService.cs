using LMWholesale.resource.clients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace LMWholesale.resource.factory.services
{
    public class RemarketingPlusService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public RemarketingPlusService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='remarketingPlusInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>Remarketing+ Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='Enabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='remarketingPlusEnabled' type='checkbox' {auctionInfo.Enabled}/>
                                <div id='remarketingPlusListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='remarketingPlusSellerID'>Remarketing+ Seller ID:&nbsp;</label></div>
                                            <div><input id='remarketingPlusSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='remarketingPluskContactGroup'>Remarketing+ Contact Group:&nbsp;</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='remarketingPlusMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='remarketingPlusMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport}/></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled}/></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusSwapStockNumber'>Swap Stock # with Dealer Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusSwapStockNumber' type='checkbox' {auctionInfo.SwapStockNumber} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='remarketingPlusSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='remarketingPlusSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divRemarketingPlusSaveAuction' style='text-align:center;'><input id='remarketingPlusSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""remarketingPlus"", ""19"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                               <div id='remarketingPlusCredDiv'>
                                    <div id='remarketingPlusGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='RemarketingPlusJsGrid'></div>
                                        <asp:HiddenField ID='remarketingPluskCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='remarketingPlusAdd' onclick=""javascript: HandleButtonClick('RemarketingPlus', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='remarketingPlusEdit' onclick=""javascript: HandleButtonClick('RemarketingPlus', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 19);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "19");

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
                OnDoubleClickFunction = "remarketingPlusRowDoubleClick();",
                HTMLElement = "RemarketingPlusJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "RemarketingPlus" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_RemarketingPlusCredCount').innerHTML = $('#RemarketingPlusJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#RemarketingPlusJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#RemarketingPlusJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Seller ID:100|:BuyerGroup:CarGroup ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|!:kWholesaleAuction:kWholesaleAuction:10|", "", true);

            return grid;
        }
    }
}