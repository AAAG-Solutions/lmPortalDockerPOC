// SmartAuction Specific functions
function saRowDoubleClick() {
    HandleButtonClick('SmartAuction', 'Edit');
    return false;
}

// ACV Auction Specific functions
function acvRowDoubleClick() {
    HandleButtonClick('ACV Auctions', 'Edit');
    return false;
}

// ADESA Specific functions
function adesaRowDoubleClick() {
    HandleButtonClick('ADESA', 'Edit');
    return false;
}

// Auction Edge Specific functions
function aeRowDoubleClick() {
    HandleButtonClick('AuctionEdge', 'Edit');
    return false;
}

// Auction Simplified Specific functions
function asRowDoubleClick() {
    HandleButtonClick('AuctionSimplified', 'Edit');
    return false;
}

// COPART Specific functions
function copartRowDoubleClick() {
    HandleButtonClick('COPART', 'Edit');
    return false;
}

// IAA Specific functions
function iaaRowDoubleClick() {
    HandleButtonClick('IAA', 'Edit');
    return false;
}

// OVE Specific functions
function oveRowDoubleClick() {
    HandleButtonClick('OVE', 'Edit');
    return false;
}

// IAS Specific functions
function iasRowDoubleClick() {
    HandleButtonClick('IAS', 'Edit');
    return false;
}

// AuctionOS Specific functions
function AuctionOSRowDoubleClick() {
    HandleButtonClick('AuctionOS', 'Edit');
    return false;
}

// Carmigo Specific functions
function carmigoRowDoubleClick() {
    HandleButtonClick('Carmigo', 'Edit');
    return false;
}

// CarOffer Specific functions
function carrofferRowDoubleClick() {
    HandleButtonClick('CarOffer', 'Edit');
    return false;
}

// RemarketingPlus Specific functions
function remarketingPlusRowDoubleClick() {
    HandleButtonClick('RemarketingPlus', 'Edit');
    return false;
}

function GridRowSelected(item) {
    $("#MainContent_credIdx")[0].value = item[0].rowIndex;
    return false;
}

function ClearRowSelection() {
    return false;
}

$(document).ready(function () {
    var children = Array.from(document.getElementById("MainContent_AucitonInfo").children);
    children.forEach(child => { if (child.id != "oveInfo") { child.style.display = "none"; }; });
});

function HandleButtonClick(auction, action) {
    var name = auction.replace(" ", "");
    // Hide all other sections when adding/editing other auction credentials
    $('div[id$=Section]').toArray().forEach(section => { section.style.display = section.id == `${name}Section` ? "initial" : "none"; });
    document.getElementById("HeaderAddEditCred").innerText = `${action} Wholesale Auction Credential`;
    document.getElementById("auctionLegend").innerText = `${auction} Credential`;

    if ($("#MainContent_credIdx")[0].value == "" && action == "Edit") {
        alert("You must have a selected auction credential to edit!");
        return false;
    }
    // Show Add/Edit Credentials modal
    Modal('addEditAuctionCred', 'show');
    DisplayAuctionCred(name, action);
}

function DisplayAuctionCred(auction, action) {
    var credBtn = document.getElementById("SwitchAuctionCreds");
    if (action == "Edit") {
        var credential = $(`#${auction}JsGrid`).data("JSGrid").data[parseInt($("#MainContent_credIdx")[0].value)];
        if (credential.Disable == "1") {
            credBtn.value = "Enable";
            credBtn.attributes["onclick"].value = `SaveCredentials('${auction}', 'Enable');`;
            credBtn.style.display = "initial";
        } else {
            credBtn.value = "Disable";
            credBtn.attributes["onclick"].value = `SaveCredentials('${auction}', 'Disable');`;
            credBtn.style.display = "initial";
        }

        switch (auction) {
            case 'OVE':
                // DropDown
                $("#MainContent_OVEkWholesaleLocationCode").val(credential.kWholesaleLocationCode);
                $("#MainContent_OVEkWholesaleFacilitatedAuctionCode").val(credential.kWholesaleFacilitatedAuctionCode);
                $("#MainContent_OVEkWholesaleBidIncrement").val(credential.kWholesaleBidIncrement);

                // TextBox
                $("#MainContent_OVEBuyerGroup").val(credential.BuyerGroup);

                // Checkbox
                document.getElementById("chkOVEDealerAccount").checked = credential.DealerAccount == "1" ? true : false;
                break;
            case 'SmartAuction':
                // DropDown
                $("#MainContent_SmartAuctionkWholesaleLocationCode").val(credential.kWholesaleLocationCode);

                // TextBox
                $("#MainContent_SmartAuctionBuyerGroup").val(credential.BuyerGroup);

                // Checkbox
                document.getElementById("chkSmartAuctionAllowSaturdayAuction").checked = credential.AllowSaturdayAuction == "1" ? true : false;
                break;
            case 'ADESA':
                // TextBox
                $("#MainContent_ADESABuyerGroup").val(credential.BuyerGroup);
                $("#MainContent_ADESAOrganizationName").val(credential.OrganizationName);
                $("#MainContent_ADESAServiceProviderID").val(credential.ServiceProviderID);
                $("#MainContent_ADESAServiceProviderName").val(credential.ServiceProviderName);
                $("#MainContent_ADESACarGroupID").val(credential.CarGroupID);

                // Checkbox
                document.getElementById("chkADESADealerAccount").checked = credential.DealerAccount== "1" ? true : false;
                break;
            case 'COPART':
                // TextBox
                $("#MainContent_COPARTBuyerGroup").val(credential.BuyerGroup);
                $("#MainContent_COPARTServiceProviderName").val(credential.ServiceProviderName);

                document.getElementById("chkCOPARTAdhocEnabled").checked = credential.AdhocEnabled == "1" ? true : false;
                break;
            case 'AuctionEdge':
                // DropDown
                $("#MainContent_AuctionEdgekWholesaleFacilitatedAuctionCode").val(credential.kWholesaleFacilitatedAuctionCode);
                break;
            case 'IAA':
                // TextBox
                $("#MainContent_IAACredentialName").val(credential.CredentialName);
                $("#MainContent_IAASellerID").val(credential.SellerID);
                break;
            case 'AuctionSimplified':
                // TextBox
                $("#MainContent_AuctionSimplifiedCredentialName").val(credential.CredentialName);
                $("#MainContent_AuctionSimplifiedSellerID").val(credential.SellerID);
                $("#MainContent_AuctionSimplifiedBuyerGroup").val(credential.BuyerGroup);
                break;
            case 'IAS':
                // DropDown
                $("#MainContent_IASkWholesaleFacilitatedAuctionCode").val(credential.kWholesaleFacilitatedAuctionCode);
                break;
            default:
                // Nothing to do here
                break;
        }

        // Common Items
        // DropDown
        $(`#MainContent_${auction}InvLotLocation`).val(credential.InvLotLocation == ("" || "Any Lot") ? "[ANY]" : credential.InvLotLocation);
        $(`#MainContent_${auction}kWholesaleListingTag`).val(credential.kWholesaleListingTag);
        $(`#MainContent_${auction}kWholesaleLocationIndicator`)[0].selectedIndex = parseInt(credential.kWholesaleLocationIndicator);
        $(`#MainContent_${auction}kWholesaleInspectionCompany`)[0].selectedIndex = parseInt(credential.kWholesaleInspectionCompany);
        $(`#MainContent_${auction}kContactGroup`).val(credential.kContactGroup);

        // TextBox
        $(`#MainContent_${auction}CredentialName`).val(credential.CredentialName);
        $(`#MainContent_${auction}SellerID`).val(credential.SellerID);

        // Checkbox
        document.getElementById(`chk${auction}SuppressMMR`).checked = credential.SuppressMMR == "1" ? true : false;
        document.getElementById(`chk${auction}AdhocEnabled`).checked = credential.AdhocEnabled == "1" ? true : false;
    } else {
        // Reset Credential info
        $(`select[id^=MainContent_${auction}]`).toArray().forEach(a => a.selectedIndex = 0);
        $(`input[id^=MainContent_${auction}][type=text]`).toArray().forEach(box => box.value = "");
        $(`input[id^=chk${auction}][type=checkbox]`).toArray().forEach(chk => chk.checked = false);
        credBtn.style.display = "none";
    }

    document.getElementById("SaveAuctionCreds").attributes["onclick"].value = `SaveCredentials('${auction}', '${action}')`;
}

function SaveCredentials(auction, action) {
    // Get credential info and then we overwrite the adjustments
    var credential = $(`#${auction}JsGrid`).data("JSGrid").data[parseInt($("#MainContent_credIdx")[0].value)];
    if (action == "Add" && credential != null)
        credential.kDealerWholesaleCredential = "0";
    else if (credential == null) {
        credential = BuildNewCredential(auction);
    }

    if (action == "Disable") {
        credential.Disable = "1";
    } else {
        credential.Disable = "0";
    }

    $(`select[id^=MainContent_${auction}]`).toArray().forEach(a => credential[a.id.replace(`MainContent_${auction}`, "")] = a.value);
    $(`input[id^=MainContent_${auction}][type=text]`).toArray().forEach(box => credential[box.id.replace(`MainContent_${auction}`, "")] = box.value);
    $(`input[id^=chk${auction}][type=checkbox]`).toArray().forEach(chk => credential[chk.id.replace(`chk${auction}`, "")] = chk.checked ? "1" : "0");

    $.ajax({
        type: 'POST',
        url: 'MarketPlaceInfo.aspx/SetAuctionCredential',
        data: `{'jsonData': '${JSON.stringify(credential)}'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                alert("Auction Credential information saved successfully");
                Modal('addEditAuctionCred', 'close');
                var gridData = $(`#${auction}JsGrid`).data("JSGrid");
                if (action != "Add")
                    gridData.refresh();
                else
                    gridData.search(gridData.getFilter());
                return false;
            }

            alert(r.message);
            return false;
        }
    });
}

function ChangeView() {
    var children = document.getElementById("MainContent_AucitonInfo").children;
    var name = document.getElementById("MainContent_lstAuctionSelector").value.replaceAll(" ", "");
    for (var i = 0; i < children.length; i++) {
        var id = document.getElementById("MainContent_AucitonInfo").children[i].children[1].children[1].children[0].children[0].children[0].id
        children[i].style = id.substring(0, id.indexOf("JsGrid")) == name ? "position: relative;" : "display:none;";
    }
}

function SaveAllAuctions() {
    var auctions = [
        { kAuction: 1, partName: "ove" },
        { kAuction: 2, partName: "sa" },
        { kAuction: 4, partName: "adesa" },
        { kAuction: 6, partName: "copart" },
        { kAuction: 7, partName: "ae" },
        //{ kAuction: 12, partName: "ed" },
        { kAuction: 11, partName: "acv" },
        { kAuction: 13, partName: "iaa" },
        { kAuction: 14, partName: "as" },
        { kAuction: 15, partName: "ias" },
        { kAuction: 16, partName: "aos" },
        { kAuction: 17, partName: "carmigo" },
        { kAuction: 18, partName: "caroffer" }
    ];

    var pushList = [];
    auctions.forEach(a => {
        var info = BuildAuctionInfo(a["partName"], a["kAuction"]);
        if (info != null)
            pushList.push(info);
    });

    $.ajax({
        type: 'POST',
        url: 'MarketPlaceInfo.aspx/SaveAllAuctions',
        data: `{'jsonData': ${JSON.stringify(pushList)}}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;

            if (r.success) {
                alert("Auction information saved successfully");
                location.reload(true);
                return false;
            }

            alert(r.message);
            return false;
        }
    });
}

function SaveAuction(auction, kWholesaleAuction) {
    var dataOut = BuildAuctionInfo(auction, kWholesaleAuction);
    if (dataOut == null) {
        alert("Failed to save Auction info!");
        return false;
    }

    //return false;
    $.ajax({
        type: 'POST',
        url: 'MarketPlaceInfo.aspx/SaveAuctionSettings',
        data: `{'jsonData': '${JSON.stringify(dataOut)}'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;

            if (r.success) {
                alert("Auction information saved successfully")
                return false;
            }

            alert("Failed to save auction information!");
            return false;
        }
    });
}

function BuildAuctionInfo(auction, kWholesaleAuction) {
    var auctionInfo = document.getElementById(auction + "Enabled");
    if (auctionInfo != null) {
        var dataOut = {
            kWholesaleAuction: kWholesaleAuction,
            kWholesaleLocationInd: document.getElementById("MainContent_PhysicalLocations").value,
            kWholesaleListingTag: document.getElementById("MainContent_ListingType").value,
            Duration: document.getElementById("MainContent_ListingDuration").value,
            AutoEndWholesale: document.getElementById("MainContent_AutoEndWholesale").checked == true ? "1" : "0",
            AutoEndeBay: document.getElementById("MainContent_AutoEndeBay").checked == true ? "1" : "0",
            RelistCount: document.getElementById("MainContent_AutoRelistCount").value,
            kWholesaleInspectionCompany: document.getElementById("MainContent_InspectionCompany").value,
            UseInventoryPrice: document.getElementById("MainContent_PricingManual").checked == true ? "1" : "0",
            ForceWholesalePricing: document.getElementById("MainContent_ForcePrice").checked == true ? "1" : "0",
            MMRRegions: document.getElementById("MainContent_MMRRegions").value,
            MinWholesalePrice: document.getElementById("MainContent_MinWholesale").value,
            MMRDefaultGrade: document.getElementById("MainContent_DefaultGrade").value,
            sMinMMRThreshold: document.getElementById("MainContent_MinMMRThreshold").value,
            //sMMRMaxReserve: document.getElementById("MainContent_MaxMMRReserve").value,
            //sMMRMaxBuyNow: document.getElementById("MainContent_MaxMMRBIN").value,
            //sMVGMaxReserve: document.getElementById("MainContent_MaxMVGReserve").value,
            //sMVGMaxBuyNow: document.getElementById("MainContent_MaxMVGBIN").value,
            IndustryGradeIsAutoGrade: document.getElementById("MainContent_IndustryGrade").checked == true ? "1" : "0",
            ReportedGradeIsNAAAGrade: document.getElementById("MainContent_NAAAGrade").checked == true ? "1" : "0"
        };

        // Textbox box
        $(`input[id^=${auction}][type=input]`).toArray().forEach(box => {
            dataOut[`${box.id.substring(auction.length)}`] = box.value;
        });

        // Dropdown drop
        $(`select[id^=${auction}]`).toArray().forEach(lst => {
            dataOut[`${lst.id.substring(auction.length)}`] = (lst.value == "--" || lst.value == "") ? "0" : lst.value;
        });

        // Checkbox check
        $(`input[id^=${auction}][type=checkbox]`).toArray().forEach(chk => {
            dataOut[`${chk.id.substring(auction.length)}`] = chk.checked ? "1" : "0";
        });

        return dataOut;
    }

    return null;
}

function BuildNewCredential(auction) {
    var auctions = {
        OVE: 1 ,
        SmartAuction: 2,
        ADESA: 4,
        COPART: 6,
        AuctionEdge: 7,
        //{ kAuction: 12, Name: "ed" },
        ACVAuctions: 11,
        IAA: 13,
        AuctionSimplified: 14,
        IAS: 15,
        AuctionOS: 16,
        Carmigo: 17,
        CarOffer: 18
    };

    var cred = {
        AdhocEnabled: "0",
        AllowSaturdayAuction: "0",
        BuyerGroup: "",
        CarGroupID: "",
        CredentialName: "",
        DealerAccount: "0",
        Disable: "0",
        InvLotLocation: "",
        OrganizationName: "",
        PDNisDealerAccount: "0",
        SellerID: "",
        ServiceProviderID: "",
        ServiceProviderName: "",
        SuppressMMR: "0",
        kAASale: "0",
        kContactGroup: "0",
        kDealerWholesaleCredential: "0",
        kWholesaleAuction: auctions[`${auction}`],
        kWholesaleBidIncrement: "0",
        kWholesaleFacilitatedAuctionCode: "0",
        kWholesaleInspectionCompany: "0",
        kWholesaleListingTag: "0",
        kWholesaleLocationCode: "0",
        kWholesaleLocationIndicator: "0"
    };

    return cred;
}


function Modal(element, op) {
    var popbkg = document.getElementById(element);
    if (popbkg) {
        if (op == 'show') {
            popbkg.style.display = "initial";
        } else {
            popbkg.style.display = "none";
        }
    }
}