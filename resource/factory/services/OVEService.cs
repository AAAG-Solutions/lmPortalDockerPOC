using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class OVEService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public OVEService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='oveInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>OVE Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                            <label for='oveEnabled'>&nbsp;Enable:&nbsp;</label>
                            <input id='oveEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='oveListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='oveSellerID'>OVE Seller ID:&nbsp;</label></div>
                                            <div><input id='oveSellerID' class='inputStyle' type='input' {auctionInfo.SellerID}></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='oveBuyerGroup'>OVE Buyer Group:&nbsp;</label></div>
                                            <div><input id='oveBuyerGroup' class='inputStyle' type='input' {auctionInfo.BuyerGroup}></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='ovekContactGroup'>OVE Contact Group:&nbsp;</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='ovekWholesaleFacilitatedAuctionCode'>Facilitated Auction Code:&nbsp;</label></div>
                                            <div>{auctionInfo.kWholesaleFacilitatedAuctionCode}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='ovekWholesaleLocationCode'>Physical Location Code:&nbsp;</label></div>
                                            <div>{auctionInfo.kWholesaleLocationCode}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='ovekWholesaleBidIncrement'>Bid Increment:&nbsp;</label></div>
                                            <div>{auctionInfo.WholesaleBidIncrement}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='oveMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='oveMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveUseLMIURL'>Always use LMI CR URL:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveUseLMIURL' type='checkbox' {auctionInfo.UseLMIURL} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveInvFeedOnly'>Inventory Feed Only:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveInvFeedOnly' type='checkbox' {auctionInfo.InvFeedOnly} /></div>
                                            <div style='display:table-cell;width:5px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveUsePartialInventory'>Send Partial Vehicle Information:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveUsePartialInventory' type='checkbox' {auctionInfo.UsePartialInventory} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveDealerAccount'>Dealer Account:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveDealerAccount' type='checkbox' {auctionInfo.IsDealerAccount} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveSendURLasCR'>Send URL as Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveSendURLasCR' type='checkbox' {auctionInfo.SendURLasCR} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='oveIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='oveSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='oveSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                        </div>
                                    </div>
                                    <div id='divOveSaveAuction' style='text-align:center;'><input id='oveSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""ove"", ""1"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                                <div id='oveCredDiv'>
                                    <div id='oveGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='OVEJsGrid'></div>
                                        <asp:HiddenField ID='ovekCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='oveAdd' onclick=""javascript: HandleButtonClick('OVE', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='oveEdit' onclick=""javascript: HandleButtonClick('OVE', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }

        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 1);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "1");

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
                OnDoubleClickFunction = "oveRowDoubleClick();",
                HTMLElement = "OVEJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "OVE" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_OVECredCount').innerHTML = $('#OVEJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#OVEJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#OVEJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:DealerAccount:Dealer Account:100|:SellerID:Seller ID:100|:BuyerGroup:CarGroup ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}