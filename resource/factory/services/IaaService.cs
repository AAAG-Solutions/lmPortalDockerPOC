using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class IaaService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public IaaService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='iaaInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>IAA Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='iaaEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='iaaEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='iaaListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iaaSellerID'>IAA Seller ID:</label></div>
                                            <div><input id='iaaSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iaakContactGroup'>IAA Contact Group:</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iaaMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='iaaMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iaaSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iaaSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divIaaSaveAuction' style='text-align:center;'><input id='iaaSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""iaa"", ""13"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                                <div id='iaaCredDiv'>
                                    <div id='iaaGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='IAAJsGrid'></div>
                                        <asp:HiddenField ID='iaakCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='iaaAdd' onclick=""javascript: HandleButtonClick('IAA', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='iaaEdit' onclick=""javascript: HandleButtonClick('IAA', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 13);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "13");

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
                OnDoubleClickFunction = "iaaRowDoubleClick();",
                HTMLElement = "IAAJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "IAA" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_IAACredCount').innerHTML = $('#IAAJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#IAAJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#IAAJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Seller ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}