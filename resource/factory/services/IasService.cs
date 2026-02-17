using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory.services
{
    public class IasService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public IasService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='iasInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>IAS Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='iasEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='iasEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='iasListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iasSellerID'>IAS Seller ID:</label></div>
                                            <div><input id='iasSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iaskWholesaleFacilitatedAuctionCode'>Facilitated Auction Code:</label></div>
                                            <div>{auctionInfo.kWholesaleFacilitatedAuctionCode}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iaskContactGroup'>IAS Contact Group:</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='iasMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='iasMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iasAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iasAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iasIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasIncludeOwnerName' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iasRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iasExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iasALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='iasAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='iasSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='iasSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                        </div>
                                    </div>
                                    <div id='divIasSaveAuction' style='text-align:center;'><input id='iasSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""ias"", ""15"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                                <div id='iasCredDiv'>
                                    <div id='iasGrid' class='credentialGrid hide_scrollbar'>
                                        <div id='IASJsGrid'></div>
                                        <asp:HiddenField ID='iaskCred' runat='server' Value="""" />
                                    </div>
                                    <div style='text-align-last:center;'>
                                        <button id='iasAdd' onclick=""javascript: HandleButtonClick('IAS', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                        <button id='iasEdit' onclick=""javascript: HandleButtonClick('IAS', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>";
        }

        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 15);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "15");

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
                OnDoubleClickFunction = "iasRowDoubleClick();",
                HTMLElement = "IASJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "IAS" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_IASCredCount').innerHTML = $('#IASJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#IASJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#IASJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:SellerID:Seller ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}