using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using LMWholesale.resource.clients;

namespace LMWholesale.resource.factory
{
    public class AdesaService : IAuctionService
    {
        private readonly WholesaleClient wholesaleClient;
        private readonly LookupClient lookupClient;

        public AdesaService()
        {
            wholesaleClient = wholesaleClient ?? new WholesaleClient();
            lookupClient = lookupClient ?? new LookupClient();
        }

        public string BuildListingInfo(BLL.WholesaleContent.Preferences.MarketPlaceInfo.Auction.Info auctionInfo)
        {
            return $@"
                <fieldset id='adesaInfo' class='sectionFieldset' style='position: relative;'>
                    <legend>ADESA Listing Information</legend>
                        <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                            <div style='flex:1 1 50%;'>
                                <label for='adesaEnabled'>&nbsp;Enable:&nbsp;</label>
                                <input id='adesaEnabled' type='checkbox' {auctionInfo.Enabled} />
                                <div id='adesaListing'>
                                    <div style='display:flex;flex-direction:row;flex-wrap:wrap;'>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaSellerID'>ADESA Seller ID:&nbsp;</label></div>
                                            <div><input id='adesaSellerID' type='input' {auctionInfo.SellerID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaServiceProviderID'>Service Provider ID:&nbsp;</label></div>
                                            <div><input id='adesaServiceProviderID' type='input' {auctionInfo.ServiceProviderID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaOrganizationName'>ADESA Org. Name:&nbsp;</label></div>
                                            <div><input id='adesaOrganizationName' type='input' {auctionInfo.OrganizationName} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaServiceProviderName'>Service Provider Name:&nbsp;</label></div>
                                            <div><input id='adesaServiceProviderName' type='input' {auctionInfo.ServiceProviderName} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaBuyerGroup'>ADESA SSO ID:&nbsp;</label></div>
                                            <div><input id='adesaBuyerGroup' type='input' {auctionInfo.BuyerGroup} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaCarGroupID'>ADESA CarGroup ID:&nbsp;</label></div>
                                            <div><input id='adesaCarGroupID' type='input' {auctionInfo.CarGroupID} class='inputStyle'></div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesakContactGroup'>ADESA Contact Group:&nbsp;</label></div>
                                            <div>{auctionInfo.ContactGroup}</div>
                                        </div>
                                        <div style='flex:1;margin:0px 5px;'>
                                            <div><label for='adesaMaxMMRPct'>Max MMR:&nbsp;</label></div>
                                            <div><input id='adesaMaxMMRPct' style='width:75px;' class='inputStyle' type='input' {auctionInfo.MaxMMRPct}></div>
                                        </div>
                                    </div>
                                    <div class='marketInfoTbl'>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaAutoLaunchEnabled'>Enable AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaAutoLaunchEnabled' type='checkbox' {auctionInfo.AutoLaunchEnabled} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;'></div>
                                            <div style='display:table-cell;'></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaInvFeedOnly'>Inventory Feed Only:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaInvFeedOnly' type='checkbox' {auctionInfo.InvFeedOnly} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaDealerAccount'>Dealer Account:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaDealerAccount' type='checkbox' {auctionInfo.IsDealerAccount} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaExemptTireDamage'>Exempt Tire Damage:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaExemptTireDamage' type='checkbox' {auctionInfo.ExemptTireDamage} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaAdhocEnabled'>Enable Ad Hoc Pickup:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaAdhocEnabled' type='checkbox' {auctionInfo.AdhocEnabled} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaRequireConditionReport'>Require Condition Report:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaRequireConditionReport' type='checkbox' {auctionInfo.RequireConditionReport} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaAllowOverlay'>Allow Overlay:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaAllowOverlay' type='checkbox' {auctionInfo.AllowOverlay} /></div>
                                        </div>
                                        <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaALRequireStyle'>Require Vehicle Style for AutoLaunch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaALRequireStyle' type='checkbox' {auctionInfo.ALRequireStyle} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaIncludeOwnerName'>Include Owner Name:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaIncludeOwnerName' type='checkbox' {auctionInfo.IncludeOwnerName} /></div>
                                        </div>
                                         <div style='display:table-row;'>
                                            <div style='display:table-cell;text-align:right;'><label for='adesaSuppressManualLaunch'>Suppress Manual Launch:&nbsp;</label></div>
                                            <div style='display:table-cell;'><input id='adesaSuppressManualLaunch' type='checkbox' {auctionInfo.SuppressManualLaunch} /></div>
                                            <div style='display:table-cell;width:50px;'></div>
                                        </div>
                                    </div>
                                    <div id='divAdesaSaveAuction' style='text-align:center;'><input id='adesaSaveAuction' type='button' class='actionBackground' value='Save Auction' onclick='SaveAuction(""adesa"", ""4"")'></div>
                                </div>
                            </div>
                            <div style='flex:1 1 50%;'>
                               <div id='adesaCredDiv'>
                                   <div id='adesaGrid' class='credentialGrid hide_scrollbar'>
                                       <div id='ADESAJsGrid'></div>
                                       <asp:HiddenField ID='adesakCred' runat='server' Value="""" />
                                   </div>
                                   <div style='text-align-last:center;'>
                                       <button id='adesaAdd' onclick=""javascript: HandleButtonClick('ADESA', 'Add');return false;"" class='submitButton headerButton' style='width:150px;'>Add</button>
                                       <button id='adesaEdit' onclick=""javascript: HandleButtonClick('ADESA', 'Edit');return false;"" class='submitButton headerButton' style='width:150px;'>Edit</button>
                                   </div>
                               </div>
                            </div>
                        </div>
                </fieldset>";
        }


        public DataTable GetAuctionInfo(string kSession, int kDealer)
        {
            Wholesale.lmReturnValue result = wholesaleClient.WholesaleAuctionByDealerGet(kSession, kDealer, 4);

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
            Lookup.lmReturnValue result = lookupClient.GetAuctionCredentialsByDealerByAuction(kSession, kDealer.ToString(), "4");

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
                OnDoubleClickFunction = "adesaRowDoubleClick();",
                HTMLElement = "ADESAJsGrid",
                Filtering = false,
                ExtraParameters = new Dictionary<string, string> { { "kWholesaleAuctionName", "ADESA" } },
                PageSize = int.MaxValue
            };

            grid.ExtraFunctionality = $@"
                    document.getElementById('MainContent_ADESACredCount').innerHTML = $('#ADESAJsGrid').data('JSGrid').data.length ?? '0';
                    var gridData = $('#ADESAJsGrid').data('JSGrid').data;
                    if (gridData.length != 0) {{
                        for (let i = 0; i < gridData.length; i++) {{
                            if (gridData[i].Disable == ""1"") {{
                                $('#ADESAJsGrid')[0].children[1].children[0].children[0].children[i].className += ' strike';
                            }}
                        }};
                    }}
            ";

            grid.SetFieldListFromGridDef(":InvLotLocation:Lot Location:100|:CredentialName:Credential Name:100|:IsDealerAccount:Dealer Account:100|:SellerID:Seller ID:100|:BuyerGroup:SSO ID:100|:CarGroupID:CarGroup ID:100|:SuppressMMR:No MMR:100|:AdhocEnabled:Ad Hoc:100|", "", true);

            return grid;
        }
    }
}