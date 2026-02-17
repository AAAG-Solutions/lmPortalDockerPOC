using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class AuctionOSService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public AuctionOSService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='aosInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>AuctionOS Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='aosEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='aosEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='AuctionOSListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='aosSellerID'>AuctionOS Seller ID:</label></div>
                                            <div><input id='aosSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='aoskContactGroup'>AuctionOS Contact Group:</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='aosMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='aosMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='aosAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='aosAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='aosIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='aosRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='aosExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='aosALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='aosAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='aosSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='aosSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divaosSaveAuction' style='text-align:center;'><input id='aosSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""aos"", ""16"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                                <div id='AuctionOSCredDiv'>
                                    <div id='AuctionOSGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='AuctionOSJsGrid'></div>
                                        <asp:HiddenField ID='AuctionOSkCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='AuctionOSAdd' onclick=""javascript: HandleButtonClick('AuctionOS', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='AuctionOSEdit' onclick=""javascript: HandleButtonClick('AuctionOS', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 16);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "16");

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
                OnDoubleClickFunction = "AuctionOSRowDoubleClick();",
                HTMLElement = "AuctionOSJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "AuctionOS" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_AuctionOSCredCount').innerHTML = $('#AuctionOSJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#AuctionOSJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#AuctionOSJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Seller ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}