function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input.value); }, delay);
    };
}

function HandleChangePause(input) {
    filterVehicles({} , "applyFilters");
}

function openFilters() {
    gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value });
    gtag('set', 'user_properties', { kPerson: document.getElementById("gtagkPerson").value, kDealer: document.getElementById("gtagkDealer").value });
    gtag('event', 'user_filter_click')
    toggleCssClass([['filterOptions', 'show_display']]);
}

function SetPageSize() {
    var val = document.getElementById("pageSize").value;
    if (val > 250) {
        alert('We do not support page sizes above 250!');
        return false;
    }
    $("#vehicleManagementJSGrid").jsGrid("option", "pageSize", val);

    filterVehicles({});
}

// Apply filter to jsGrid data
function filterVehicles(advancedFilters = {}, operation = "applyFilters") {
    var gridData = $("#vehicleManagementJSGrid").data("JSGrid");
    var gfilter = gridData.getFilter();

    // Get Advanced Filter
    var lot = document.getElementById('MainContent_lstLotLocation');
    var lstStatus = document.getElementById("MainContent_lstListingStatus");
    var lstInspection = document.getElementById("MainContent_lstInspectionStatus");

    if (document.getElementsByClassName("paginationClick").length != 0)
        gridData.pageSize = parseInt(document.getElementsByClassName("paginationClick")[0].innerHTML);

    if (JSON.stringify(advancedFilters) !== '{}')
        gfilter["advancedFilter"] = advancedFilters;
    else {
        advancedFilters = {
            NoStyle: $('#MainContent_NoStyle').is(":checked") ? 1 : 0,
            NoDescription: $('#MainContent_NoDescription').is(":checked") ? 1 : 0,
            NoPhotos: $('#MainContent_NoPhotos').is(":checked") ? 1 : 0,
            NoListPrice: $('#MainContent_NoListPrice').is(":checked") ? 1 : 0,
            NoInternetPrice: $('#MainContent_NoInternetPrice').is(":checked") ? 1 : 0,
            LotLocation: lot.selectedIndex == 0 ? "ALL" : lot.options[lot.selectedIndex].value,
            ListingStatus: lstStatus.options[lstStatus.selectedIndex].value,
            InspectionStatus: lstInspection.options[lstInspection.selectedIndex].value,
            StatusAvailable: $('#MainContent_StatusAvailable').is(":checked") ? 1 : 0,
            StatusUnavailable: $('#MainContent_StatusUnavailable').is(":checked") ? 1 : 0,
            StatusSalePending: $('#MainContent_StatusSalePending').is(":checked") ? 1 : 0,
            StatusInTransit: $('#MainContent_StatusInTransit').is(":checked") ? 1 : 0,
            StatusDemo: $('#MainContent_StatusDemo').is(":checked") ? 1 : 0,
            StatusSold: $('#MainContent_StatusSold').is(":checked") ? 1 : 0,
            TypeDealerCertified: $('#MainContent_TypeDealerCertified').is(":checked") ? 1 : 0,
            TypeManufacturerCertified: $('#MainContent_TypeManufacturerCertified').is(":checked") ? 1 : 0,
            TypePreOwned: $('#MainContent_TypePreOwned').is(":checked") ? 1 : 0
        };

        gfilter["advancedFilter"] = advancedFilters;
    }

    if (JSON.stringify(filters) != '{}')
        gfilter["pageSize"] = filters.pageSize;

    applySearchFilterTokens(advancedFilters, operation);

    if (operation == "clearFilters") {
        gfilter["Sort"] = "";
        gfilter["TextFilter"] = "";
    }
    else {
        // Get any text from the search box
        gfilter["TextFilter"] = $('#MainContent_txtSearch').val();
    }

    gridData.search(gfilter);
    return false;
}

function applySearchFilterTokens(advancedFilters, operation) {
    // Hide previously selected tokens in favor of newly selected filters
    document.querySelectorAll('[id*=token]').forEach(token => { token.style["display"] = "none"; });

    var statusCnt = document.querySelectorAll('input[type=checkbox][id*=Status]:checked').length;
    var typeCnt = document.querySelectorAll('[id*=Type]:checked').length;

    if (operation == "defaultFilters") {
        var statuses = Object.entries(advancedFilters).filter(filter => filter[1] == 1 && filter[0].includes("Status"));
        var types = Object.entries(advancedFilters).filter(filter => filter[1] == 1 && filter[0].includes("Type"));
        document.querySelectorAll('[id*=No],input[type=checkbox][id*=Status],[id*=Type]').forEach(checkbox => { checkbox.checked = false; });

        statusCnt = statuses.length;
        typeCnt = types.length;

        if (statusCnt != 0)
            document.getElementById(`MainContent_${statuses[0][0]}`).checked = true;
        if (typeCnt != 0)
            document.getElementById(`MainContent_${types[0][0]}`).checked = true;
    }

    Object.keys(advancedFilters).forEach(filter => {
        if (filter == "LotLocation") {
            var lot = document.getElementById("MainContent_tokenLotLocation");
            if (advancedFilters.LotLocation == "ALL") {
                lot.innerText = "Any Lot Location";
            } else {
                if (advancedFilters.LotLocation.length > 50)
                    lot.innerText = `Lot Location: ${advancedFilters.LotLocation.substring(0, 49)}`
                else
                    lot.innerText = `Lot Location: ${advancedFilters.LotLocation}`;
            }
            lot.style["display"] = "initial";
        }
        else if (filter == "ListingStatus") {
            var listing = document.getElementById("MainContent_tokenListingStatus");
            if (advancedFilters.ListingStatus == 0) {
                listing.innerText = "Any Listing Status";
            } else {
                var statuses = document.getElementById("MainContent_lstListingStatus");
                listing.innerText = statuses.options[statuses.selectedIndex].text;
            }
            listing.style["display"] = "initial";
        } else if (filter == "InspectionStatus") {
            var inspect = document.getElementById("MainContent_tokenInspectionStatus");
            if (advancedFilters.InspectionStatus == -1)
                inspect.innerText = "Any Inspection Status";
            else if (advancedFilters.InspectionStatus == 1)
                inspect.innerText = "Inspected Vehicles";
            else
                inspect.innerText = "Not Inspected Vehicles";
            inspect.style["display"] = "initial";
        } else {
            if (filter.startsWith("Status")) {
                if (statusCnt == 6 || statusCnt == 0) {
                    document.getElementById("MainContent_tokenAllInventory").style["display"] = "initial";
                    return false;
                }
                var token = document.getElementById(`MainContent_token${filter}`);
                if (advancedFilters[filter] == 1)
                    token.style["display"] = "initial";
            } else if (filter.startsWith("Type")) {
                if (typeCnt == 3 || typeCnt == 0) {
                    document.getElementById("MainContent_tokenAllVehicleTypes").style["display"] = "initial";
                    return false;
                }
                var token = document.getElementById(`MainContent_token${filter}`);
                if (advancedFilters[filter] == 1)
                    token.style["display"] = "initial";
            } else {
                var token = document.getElementById(`MainContent_token${filter}`);
                if (advancedFilters[filter] == 1)
                    token.style["display"] = "initial";
            }
        }
    });
}

// Advanced Filter Logic
function setAdvancedFilters(clearFilters = false, defaultFilters = false, doSave = false) {

    // Selected DropDown items
    var lot = document.getElementById('MainContent_lstLotLocation');
    var lstStatus = document.getElementById("MainContent_lstListingStatus");
    var lstInspection = document.getElementById("MainContent_lstInspectionStatus");

    var advancedFilter = {
        NoStyle: $('#MainContent_NoStyle').is(":checked") ? 1 : 0,
        NoDescription: $('#MainContent_NoDescription').is(":checked") ? 1 : 0,
        NoPhotos: $('#MainContent_NoPhotos').is(":checked") ? 1 : 0,
        NoListPrice: $('#MainContent_NoListPrice').is(":checked") ? 1 : 0,
        NoInternetPrice: $('#MainContent_NoInternetPrice').is(":checked") ? 1 : 0,
        LotLocation: lot.selectedIndex == 0 ? "ALL" : lot.options[lot.selectedIndex].value,
        ListingStatus: lstStatus.options[lstStatus.selectedIndex].value,
        InspectionStatus: lstInspection.options[lstInspection.selectedIndex].value,
        StatusAvailable: $('#MainContent_StatusAvailable').is(":checked") ? 1 : 0,
        StatusUnavailable: $('#MainContent_StatusUnavailable').is(":checked") ? 1 : 0,
        StatusSalePending: $('#MainContent_StatusSalePending').is(":checked") ? 1 : 0,
        StatusInTransit: $('#MainContent_StatusInTransit').is(":checked") ? 1 : 0,
        StatusDemo: $('#MainContent_StatusDemo').is(":checked") ? 1 : 0,
        StatusSold: $('#MainContent_StatusSold').is(":checked") ? 1 : 0,
        TypeDealerCertified: $('#MainContent_TypeDealerCertified').is(":checked") ? 1 : 0,
        TypeManufacturerCertified: $('#MainContent_TypeManufacturerCertified').is(":checked") ? 1 : 0,
        TypePreOwned: $('#MainContent_TypePreOwned').is(":checked") ? 1 : 0
    };

    var operation = "saveFilters";
    if (clearFilters)
        operation = "clearFilters";
    else if (defaultFilters)
        operation = "defaultFilters";
    else if (!doSave)
        operation = "applyFilters"

    if (clearFilters) {
        advancedFilter = {
            NoStyle: 0,
            NoDescription: 0,
            NoPhotos: 0,
            NoListPrice: 0,
            NoInternetPrice: 0,
            LotLocation: "ALL",
            ListingStatus: 0,
            InspectionStatus: -1,
            StatusAvailable:  0,
            StatusUnavailable: 0,
            StatusSalePending: 0,
            StatusInTransit: 0,
            StatusDemo: 0,
            StatusSold: 0,
            TypeDealerCertified: 0,
            TypeManufacturerCertified: 0,
            TypePreOwned: 0
        };
        lot.selectedIndex = 0;
        lstStatus.selectedIndex = 0;
        lstInspection.selectedIndex = 0;

        // Get any text from the search box
        $('#MainContent_txtSearch').val("");

        // This should get all of the checkboxes in filters
        document.querySelectorAll('[id*=No],input[type=checkbox][id*=Status],[id*=Type]').forEach(checkbox => { checkbox.checked = false; });
    }

    $.ajax({
        type: "POST",
        url: 'VehicleManagement.aspx/SetAdvancedFilter',
        data: "{'filter': '" + JSON.stringify(advancedFilter) + "', 'operation' : '" + operation + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;

            if (!doSave) {
                toggleCssClass([['filterOptions', 'show_display']]);
                if (operation == "applyFilters")
                    alert("Successfully applied selected filters!");
                else if (operation == "clearFilters")
                    alert("Successfully cleared selected filters!")
                else if (operation) {
                    alert("Successfully applied Account default filters!");
                    advancedFilter = JSON.parse(r.value);
                }
                filterVehicles(advancedFilter, operation);
                return false;
            }

            if (r.success) {
                alert(r.message)
            } else
                alert(r.message);

            toggleCssClass([['filterOptions', 'show_display']]);
            filterVehicles(advancedFilter, operation);
            return false;
        }
    });
}

// Navigate to UpdateVehicle page and scroll to error message section
function listingInfo(klisting) {
    window.location.href = `/WholesaleContent/Vehicle/Update.aspx?kListing=${klisting}&FocusAuction=ErrorMsg`;
}

// Little hack to keep imgs inline
function excelDownload() {
    document.getElementById('MainContent_exportBtn').click();
}

// Mark vehicle either sell or unsell
function MarkVehicle(klisting, status) {
    var dataIn = {
        kListing: klisting,
        kInventoryStatus: status
    };

    var sellUnsell = status == "1" ? "available" : "sold" ;
    toggleLoading(true, `Making Vehicle to be ${sellUnsell}...`);
    $.ajax({
        type: "POST",
        url: 'VehicleManagement.aspx/SellUnsellVehicle',
        data: JSON.stringify(dataIn),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                toggleLoading(false, "Refreshing info...");
                alert(`Successfully ${r.value} vehicle!`);
                filterVehicles({});
            }
            else
                alert(`Unable to Sell/Unsell Vehicle! Please Contact Support!\nMessge - ${r.message}`);
        }
    });
}

function PhotoPopup(kListing) {
    var popup = window.open(`/WholesaleData/UploadPhotos.aspx?kListing=${kListing}`, '_blank', 'popup,height=625,width=850');
    var timer = setInterval(function () {
        if (popup.closed) {
            clearInterval(timer);
            window.location.reload();
        }
    }, 1000);
}


function auctionClick(self, kListing, kWholesaleAuction) {
    // Close all other Auction info boxes if present
    var contents = document.getElementsByClassName("openAuctionsContent");
    Array.from(contents).forEach(e => {
        if (e.id == self)
            return;
        toggleCssClass([[e.id, 'openAuctionsContent']]);
        e.classList.remove("closeTop", "closeMid", "closeBottom");
    });

    var ypos = event.clientY - event.target.offsetTop;
    var info = document.getElementById(self);
    if (info != null) {
        if (ypos > 375)
            info.classList.add("closeTop");
        if (ypos > 475 && ypos < 575)
            info.classList.add("closeMid");
        else if (ypos > 600)
            info.classList.add("closeBottom");
    } else {
        BuildAuctionInfoPopup(self, kListing, kWholesaleAuction);
    }

    if (!document.getElementById(self).classList.contains('openAuctionsContent')) {
        toggleCssClass([[self, "openAuctionsContent"]]);
    } else {
        toggleCssClass([[self, "openAuctionsContent"]]);
        return;
    }
    var root = self.replace("_auctionInfo", "");
    if (document.getElementById(`${root}_startDate`).innerText != "")
        return;

    $.ajax({
        type: "POST",
        url: 'VehicleManagement.aspx/VehicleAuctionInfoGet',
        data: `{ 'kListing': ${kListing}, 'kWholesaleAuction': ${kWholesaleAuction} }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                PopulateAuctionInfo(root, JSON.parse(r.value), ypos);
            }
            else
                alert(`Unable to gather specific auction info! Please Contact Support!\nMessge - ${r.message}`);
        }
    });
}

function errMsgsClick(self) {
    // Close all other Auction info boxes if present
    var contents = document.getElementsByClassName("openErrMsgsContent");
    Array.from(contents).forEach(e => {
        if (e.id == self)
            return;
        toggleCssClass([[e.id, 'openErrMsgsContent']]);
    });

    if (!document.getElementById(self).classList.contains('openErrMsgsContent')) {
        toggleCssClass([[self, "openErrMsgsContent"]]);
    } else {
        toggleCssClass([[self, "openErrMsgsContent"]]);
        return;
    }
}

function PopulateAuctionInfo(root, auctionInfo) {
    for (const item in auctionInfo) {
        var element = document.getElementById(`${root}_${item}`);
        element.innerText += auctionInfo[item];
    }
}

// Little work around to doubleClick to go to selected update vehicle page
function RowDoubleClick() {
    var vinBoxClick = event.target.children.length > 0 && /^vin_/.exec(event.target.children[0].id) ? true : false;
    var vinClick = /^vin/.exec(event.target.id) ? true : false;
    if (vinBoxClick || vinClick)
        return false;
    toggleLoading(true, "Loading Vehicle information...");
    $('#MainContent_txtVehicle')[0].firstChild.click();
}

function GridRowSelected(item) {
    var rowItems = item[0].cells;
    Array.from(rowItems).forEach(cell => {
        if (cell.childNodes.length > 0 && cell.childNodes[0].className == 'updateLink')
            $('#MainContent_txtVehicle').html(cell.childNodes[0].cloneNode(true));     
    });
}

function ClearRowSelection() {
    $('#MainContent_txtVehicle').val("");
}

// Disable enter key if VinSearch doesn't have focus
$(document).keypress(function (e) {
    if (e.which == 13) {
        return false;
    }
});

$(document).ready(function () {
    if (IsLiquidConnect()) {
        document.getElementById("sidebar_menu").style.display = 'none';
        document.getElementById("grid").style.marginLeft = '0px';
        document.getElementById('ImportVehicles').style.display = 'none';
        document.getElementById('ExportVehicles').style.display = 'none';
        document.getElementById('AddVehicle').style.display = 'none';
        document.getElementById('MainContent_VehicleMarketingActionBar').parentElement.style.paddingLeft = '0px';
        document.getElementById('MainContent_btnDefault').style.display = 'none';
        document.getElementById('MainContent_btnSaveFilters').style.display = 'none';
        
    }
});