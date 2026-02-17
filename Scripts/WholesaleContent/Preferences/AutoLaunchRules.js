// Advanced Functions
function AdvancedGridRowSelected(item) {
    document.getElementById("MainContent_txtRule").value = item[0].cells[0].childNodes[0].value;
}

function AdvancedClearRowSelection() {
    document.getElementById("MainContent_txtRule").value = "";
}

function AdvancedAddAutoLaunchRule() {
    resetSelections('advanced');
    $('#advancedFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#autolaunchAddTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#autolaunchPricesTbl').find('input, textarea, button, select').prop('disabled', false);
    document.getElementById("MainContent_hfMode").value = 'add';
    document.getElementById("MainContent_btnFilters").value = 'Save Rule';
    document.getElementById("alAdvancedTitle").innerHTML = "Add Wholesale AutoLaunch Rule";
    toggleCssClass([['advancedOptions', 'show_display']]);
}

function AdvancedEditAutoLaunchRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select an AutoLaunch Rule to edit!");
        return false;
    }

    resetSelections('advanced');
    $('#advancedFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#autolaunchAddTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#autolaunchPricesTbl').find('input, textarea, button, select').prop('disabled', false);

    document.getElementById("MainContent_hfMode").value = 'edit';
    document.getElementById("MainContent_btnFilters").value = 'Save Rule';
    document.getElementById("alAdvancedTitle").innerHTML = "Edit Wholesale AutoLaunch Rule";

    // Toggle loading gif
    toggleLoading(true, "Gathering AutoLaunch rule info...");
    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/GetAutoLaunchRule',
        data: `{'kValue': ${selectedRule}, 'isSimple': 'false'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r) {
                AdvancedPopulateFilters(r.value, "edit");
            }
        }
    });
}

function AdvancedPopulatePrices(data) {
    // Min Price Adj
    document.getElementById("minPriceAdj").value = data.MinimumPricingAdjustment
    // Primary Prices
    // Start
    SetSelectedValue(document.getElementById("MainContent_lstPrimeStartPrice"), data.StartPricingType);
    document.getElementById("primeStartAdj").value = parseInt(data.StartPricingAdjustment);
    document.getElementById("primeStartPerc").value = parseInt(data.StartPricingPercentage * 100);
    // Floor
    SetSelectedValue(document.getElementById("MainContent_lstPrimeFloorPrice"), data.FloorPricingType);
    document.getElementById("primeFloorAdj").value = parseInt(data.FloorPricingAdjustment);
    document.getElementById("primeFloorPerc").value = parseInt(data.FloorPricingPercentage * 100);
    // Buy Now
    SetSelectedValue(document.getElementById("MainContent_lstPrimeBIN"), data.BuyNowPricingType);
    document.getElementById("primeBINAdj").value = parseInt(data.BuyNowPricingAdjustment);
    document.getElementById("primeBINPerc").value = parseInt(data.BuyNowPricingPercentage * 100);
    // Secondary Prices
    // Start
    SetSelectedValue(document.getElementById("MainContent_lstSecondStartPrice"), data.MMRStartType);
    document.getElementById("secondStartAdj").value = parseInt(data.MMRStartAdj);
    document.getElementById("secondStartPerc").value = parseInt(data.MMRStartPct * 100);
    // Floor
    SetSelectedValue(document.getElementById("MainContent_lstSecondFloorPrice"), data.MMRFloorType);
    document.getElementById("secondFloorAdj").value = parseInt(data.MMRFloorAdj);
    document.getElementById("secondFloorPerc").value = parseInt(data.MMRFloorPct * 100);
    // Buy Now
    SetSelectedValue(document.getElementById("MainContent_lstSecondBIN"), data.MMRBuyNowType);
    document.getElementById("secondBINAdj").value = parseInt(data.MMRBuyNowAdj);
    document.getElementById("secondBINPerc").value = parseInt(data.MMRBuyNowPct * 100);
}

function AdvancedPopulateFilters(info, op = "") {
    var data = info.filters;
    SetSelectedValue(document.getElementById("MainContent_lstAuction"), data.kWholesaleAuction);
    SetSelectedValue(document.getElementById("MainContent_lstMinYear"), data.MotorYear);
    SetSelectedValue(document.getElementById("MainContent_lstMaxYear"), data.MotorYearMax);
    SetSelectedValue(document.getElementById("MainContent_lstLotLocation"), data.InvLotLocation);
    SetSelectedValue(document.getElementById("MainContent_lstMake"), data.Make == "" ? "0" : data.Make);
    SetSelectedValue(document.getElementById("MainContent_lstCredentials"), data.kDealerWholesaleCredential);
    SetSelectedValue(document.getElementById("MainContent_lstStatus"), data.kInventoryStatus);
    SetSelectedValue(document.getElementById("MainContent_lstVehicleType"), data.kWholesaleVehicleType);
    SetSelectedValue(document.getElementById("MainContent_lstTitle"), data.kWholesaleTitleStatus);
    document.getElementById("minMile").value = data.MinMileage;
    document.getElementById("maxMile").value = data.MaxMileage;
    document.getElementById("minAge").value = data.AgeLow;
    document.getElementById("maxAge").value = data.AgeHigh;
    SetSelectedValue(document.getElementById("MainContent_lstFuelType"), data.kFuelType);
    SetSelectedValue(document.getElementById("MainContent_lstConditionRpt"), data.RequireConditionReport);
    SetSelectedValue(document.getElementById("MainContent_lstMinGrade"), data.MinimumGrade);
    SetSelectedValue(document.getElementById("MainContent_lstMaxGrade"), data.MaximumGrade);

    var lstModels = document.getElementById("MainContent_lstModel");
    if (data.Make != "0") {
        $.ajax({
            type: "POST",
            url: 'AutoLaunchRules.aspx/GetModelList',
            data: "{'year': '" + data.MotorYear + "', 'make': '" + data.Make + "'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            },
            complete: function (response) {
                var r = response.d;
                if (r) {
                    lstModels.options.length = 1;
                    BuildDropdownList(r, lstModels);
                }

                lstModels.disabled = false;
            }
        });
    } else {
        // Default to 'Any Model' if the current rule we are editing doesn't have a selected Make
        lstModels.options.length = 1;
    }

    SetSelectedValue(lstModels, data.Model);
    OnAuctionChange(op, info)

    toggleCssClass([['advancedOptions', 'show_display']]);

    PopulateAdditionalSettings(info.additional);
    AdvancedPopulatePrices(info.prices);
    OnCRChange();

    // Open up AutoLaunch rule modal
    toggleLoading(false, "Gathering AutoLaunch rule info...");
}

function PopulateAdditionalSettings(data) {
    SetSelectedValue(document.getElementById("MainContent_lstListingType"), data.kWholesaleListingTag);
    SetSelectedValue(document.getElementById("MainContent_advancedLstListingCategory"), data.kWholesaleListingCategory);
    document.getElementById("duration").value = data.Duration;
    SetSelectedValue(document.getElementById("MainContent_lstCRCat"), data.kListingCategoryNoCR);
    SetSelectedValue(document.getElementById("MainContent_lstPhysLocation"), data.kWholesaleLocationIndicator);
    SetSelectedValue(document.getElementById("MainContent_lstBidIncrement"), data.kWholesaleBidIncrement);
    SetSelectedValue(document.getElementById("MainContent_lstAuctionCode"), data.kWholesaleFacilitatedAuction);
    SetSelectedValue(document.getElementById("MainContent_lstLocationCode"), data.kWholesaleLocationCode);
    document.getElementById("arbPledge").checked = data.PostAsYellowLight == "0" ? false : true;
}

function AdvancedAutoLaunchRuleChecks() {
    var errors = [];
    if (document.getElementById("MainContent_lstAuction").value == "0")
        errors.push("Auction")
    if (document.getElementById("MainContent_lstStatus").value == "0")
        errors.push("Status");
    if (document.getElementById("MainContent_lstListingType").value == "0")
        errors.push("Listing Type");
    if (document.getElementById("MainContent_advancedLstListingCategory").value == "0")
        errors.push("Listing Category");

    if (document.getElementById("duration").value == "" || document.getElementById("duration") == "0")
        errors.push("You must have a listing duration greater than 0!");

    // Auction specific
    switch (document.getElementById("MainContent_lstAuction").value) {
        case "1":
            if (document.getElementById("MainContent_lstAuctionCode").value == "0")
                errors.push("Facilitated Auction Code must be selected for OVE!");
            if (document.getElementById("MainContent_lstLocationCode").value == "0")
                errors.push("A Location Code must be selected for OVE!");
            if (document.getElementById("MainContent_lstBidIncrement").value == "0")
                errors.push("A Bid Increment must be selected!");
            if (document.getElementById("duration").value > "3")
                errors.push("You must have a listing duration 3 days or less for OVE!");
            break;
        case "2":
            if (document.getElementById("MainContent_lstLocationCode").value == "0")
                errors.push("A Location Code must be selected for SmartAuction!");
            break;
        case "7":
            if (document.getElementById("MainContent_lstAuctionCode").value == "0")
                errors.push("Facilitated Auction Code must be selected for Auction Edge!");
            break;
        case "15":
            if (document.getElementById("MainContent_lstAuctionCode").value == "0")
                errors.push("Facilitated Auction Code must be selected for IAS!");
            break;
        default:
            // Nothing to do here. Maybe stuff for a spaceship?
            break;
    }

    if (document.getElementById("MainContent_lstStatus").value == "0")
        errors.push("An Inventory Status must be selected!");

    if (parseInt(document.getElementById("minAge").value.replace(/,/g, '')) < 0)
        errors.push("Age Low can not be negative!");

    if (parseInt(document.getElementById("maxAge").value.replace(/,/g,'')) < 0)
        errors.push("Age High can not be negative!");

    if (parseInt(document.getElementById("minAge").value.replace(/,/g, '')) > parseInt(document.getElementById("maxAge").value.replace(/,/g, '')))
        errors.push("Age Low can not be greater than Age High!");

    if (document.getElementById("duration").value == 0)
        errors.push("You must have a listing Duration greater than 0!");

    if (document.getElementById("MainContent_lstPhysLocation").value == 0)
        errors.push("A Physical Location must be selected!");

    // Pricing Checks
    // Primary
    var idxPrimeStartPrice = document.getElementById("MainContent_lstPrimeStartPrice").selectedIndex;
    var idxPrimeFloorPrice = document.getElementById("MainContent_lstPrimeFloorPrice").selectedIndex;
    var idxPrimeBINPrice = document.getElementById("MainContent_lstPrimeBIN").selectedIndex;
    var PrimeStartAdj = parseInt(document.getElementById("primeStartAdj").value.replace(/,/g, ''));
    var PrimeFloorAdj = parseInt(document.getElementById("primeFloorAdj").value.replace(/,/g, ''));
    var PrimeBINAdj = parseInt(document.getElementById("primeBINAdj").value.replace(/,/g, ''));
    var PrimeStartPerc = parseInt(document.getElementById("primeStartPerc").value.replace(/,/g, ''));
    var PrimeFloorPerc = parseInt(document.getElementById("primeFloorPerc").value.replace(/,/g, ''));
    var PrimeBINPerc = parseInt(document.getElementById("primeBINPerc").value.replace(/,/g, ''));
    //Seconday
    var idxSecondStartPrice = document.getElementById("MainContent_lstSecondStartPrice").selectedIndex;
    var idxSecondFloorPrice = document.getElementById("MainContent_lstSecondFloorPrice").selectedIndex;
    var idxSecondBINPrice = document.getElementById("MainContent_lstSecondBIN").selectedIndex;
    var SecondStartAdj = parseInt(document.getElementById("secondStartAdj").value.replace(/,/g, ''));
    var SecondFloorAdj = parseInt(document.getElementById("secondFloorAdj").value.replace(/,/g, ''));
    var SecondBINAdj = parseInt(document.getElementById("secondBINAdj").value.replace(/,/g, ''));
    var SecondStartPerc = parseInt(document.getElementById("secondStartPerc").value.replace(/,/g, ''));
    var SecondFloorPerc = parseInt(document.getElementById("secondFloorPerc").value.replace(/,/g, ''));
    var SecondBINPerc = parseInt(document.getElementById("secondBINPerc").value.replace(/,/g, ''));

    var auction = document.getElementById("MainContent_lstAuction").value;
    var listingType = document.getElementById("MainContent_lstListingType").value;
    var lstBidIncrement = document.getElementById("MainContent_lstBidIncrement");
    var bidIncrement = lstBidIncrement.options[ lstBidIncrement.selectedIndex == -1 ? 1 : lstBidIncrement.selectedIndex ].textContent;

    if (idxPrimeStartPrice == 0
        && auction != "2"
        && auction != "6"
        && auction != "12"
        && !(listingType == "2" || listingType == "6" || listingType == "7")) {
        errors.push("You must select a Primary Start Price Type!");
    }
    if (idxPrimeFloorPrice == 0
        && !(listingType == "2" || listingType == "6" || listingType == "7")) {
        errors.push("You must select a Primary Floor Price Type!");
    }
    if (idxPrimeBINPrice == 0
        && !(listingType == "1" || listingType == "4" || listingType == "7")) {
        errors.push("You must select a Primary Buy Now Price Type!");
    }

    if (idxSecondStartPrice != 0
        && (idxPrimeStartPrice == idxSecondStartPrice)
        && (PrimeStartAdj == SecondStartAdj)
        && (PrimeStartPerc == SecondStartPerc)
        && !(auction == "2")
        && !(auction == "12")
        && !(listingType == "2" || listingType == "6" || listingType == "7")) {
        errors.push("The Primary Start Price configuration cannot be the same as the Secondary Start Price configuration!");
    }

    if (idxSecondFloorPrice != 0
        && (idxPrimeFloorPrice == idxSecondFloorPrice)
        && (PrimeFloorAdj == SecondFloorAdj)
        && (PrimeFloorPerc == SecondFloorPerc)
        && !(auction == "2" || auction == "6")) {
        errors.push("The Primary Floor Price configuration cannot be the same as the Secondary Floor Price configuration!");
    }

    if (idxSecondBINPrice != 0
        && (idxPrimeBINPrice == idxSecondBINPrice)
        && (PrimeBINAdj == SecondBINAdj)
        && (PrimeBINPerc == SecondBINPerc)
        && !(auction == "2" || auction == "7" || auction == "7")) {
        errors.push("The Primary Buy Now Price configuration cannot be the same as the Secondary Buy Now Price configuration!");
    }

    if (auction == "1" && (listingType == "3" || listingType == "5")) {
        if (idxPrimeStartPrice != 0 && idxPrimeStartPrice == idxPrimeBINPrice
            && (parseInt(PrimeStartPerc) >= parseInt(PrimeBINPerc))
            && (parseInt(PrimeBINAdj) - parseInt(PrimeStartAdj)) < (2 * bidIncrement)) {
            errors.push("The Buy It Now Price must be 2 Bid Increments greater than the Start Price for OVE Bid/Buy configurations!");
        }

        if (idxSecondStartPrice != 0 && idxSecondStartPrice == idxSecondBINPrice
            && parseInt(SecondStartPerc) >= parseInt(SecondBINPerc)
            && (parseInt(SecondBINAdj) - parseInt(SecondStartAdj)) < (2 * bidIncrement)) {
            errors.push("The Secondary Buy It Now Price must be 2 Bid Increments greater than the Secondary Start Price for OVE Bid/Buy configurations!");
        }
    }

    // Max % MMR Reserve | Max % MMR BIN Price | Min MMR Threshold
    var isMMR = document.getElementById("MainContent_WholesaleMMR").value;
    var maxMMRThreshold = parseInt(document.getElementById("MainContent_MaxMMRThreshold").value);
    var minMMRThreshold = 80;

    if (isMMR && idxPrimeStartPrice == 24 && (PrimeStartPerc > maxMMRThreshold) && (PrimeStartAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }
    if (isMMR && idxPrimeFloorPrice == 24 && (PrimeFloorPerc > maxMMRThreshold) && (PrimeFloorAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }
    if (isMMR && idxPrimeBINPrice == 24 && (PrimeBINPerc > maxMMRThreshold) && (PrimeBINAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }
    if (isMMR && idxSecondStartPrice == 24 && (SecondStartPerc > maxMMRThreshold) && (SecondStartAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less\ than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }
    if (isMMR && idxSecondFloorPrice == 24 && (SecondFloorPerc > maxMMRThreshold) && (SecondFloorAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }
    if (isMMR && idxSecondBINPrice == 24 && (SecondBINPerc > maxMMRThreshold) && (SecondBINAdj >= 0)) {
        errors.push(`The Primary Start Price Percentage must be less than the Maximum MMR Reserve Percentage of ${maxMMRThreshold}!`);
    }

    if (errors.length > 0)
        return errors;

    // No hard error checks
    if (auction != "2" && auction && "6" && auction && "12"
        && !(document.getElementById("MainContent_lstListingType").value == "2"
            || document.getElementById("MainContent_lstListingType").value == "6"
            || document.getElementById("MainContent_lstListingType").value == "7")) {
        if (parseInt(document.getElementById("primeStartPerc").value) < 60) {
            var popup = confirm(`Starting Price Percentage Adjustment is under the minimum threshold (${minMMRThreshold}%), are you sure this is your intended percentage adjustment?`);
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("");
                return;
            }
        }
    }

    if (auction != "5"
        && !(document.getElementById("MainContent_lstListingType").value == "2"
            || document.getElementById("MainContent_lstListingType").value == "6"
            || document.getElementById("MainContent_lstListingType").value == "7")) {
        if (parseInt(document.getElementById("primeFloorPerc").value) < 60) {
            var popup = confirm(`Floor (Reserve) Percentage Adjustment is under the minimum threshold (${minMMRThreshold}%), are you sure this is your intended percentage adjustment?`);
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("");
                return;
            }
        }
    }

    if (parseInt(document.getElementById("primeFloorPerc").value) < 60
        && !(document.getElementById("MainContent_lstListingType").value == "1"
            || document.getElementById("MainContent_lstListingType").value == "4"
            || document.getElementById("MainContent_lstListingType").value == "7")
        && document.getElementById("MainContent_lstPrimeFloorPrice").value != "0") {
                var popup = confirm("Buy Now Percentage Adjustment is under the minimum threshold (60%), are you sure this is your intended percentage adjustment?");
        if (popup == true) {  /* confirm */ }
        else {
            errors.push("");
            return;
        }
    }

    if (auction != "2" && auction && "6" && auction && "12"
        && !(document.getElementById("MainContent_lstListingType").value == "2"
            || document.getElementById("MainContent_lstListingType").value == "6"
            || document.getElementById("MainContent_lstListingType").value == "7")) {
        if (idxSecondStartPrice != 0 && parseInt(document.getElementById("primeStartPerc").value) < 60) {
            var popup = confirm("Secondary Starting Price Percentage Adjustment is under the minimum threshold (60%), are you sure this is your intended Secondary Price percentage adjustment?");
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("");
                return;
            }
        }
    }

    if (auction != "5"
        && !(document.getElementById("MainContent_lstListingType").value == "2"
            || document.getElementById("MainContent_lstListingType").value == "6"
            || document.getElementById("MainContent_lstListingType").value == "7")) {
        if (idxSecondFloorPrice != 0 && parseInt(document.getElementById("primeFloorPerc").value) < 60) {
            var popup = confirm("Secondary Floor (Reserve) Percentage Adjustment is under the minimum threshold (60%), are you sure this is your intended Secondary Price percentage adjustment?");
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("");
                return;
            }
        }
    }

    if (idxSecondBINPrice != 0 && !(document.getElementById("MainContent_lstListingType").value == "1"
        || document.getElementById("MainContent_lstListingType").value == "4"
        || document.getElementById("MainContent_lstListingType").value == "7")) {
        if (SecondBINPerc < 60) {
            var popup = confirm("Secondary Buy Now Percentage Adjustment is under the minimum threshold (60%), are you sure this is your intended Secondary Price percentage adjustment?");
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("");
                return;
            }
        }
    }

    // We know nothing happened so just return an empty Array
    return errors;
}

function AdvancedDeleteAutoLaunchRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select an AutoLaunch Rule to delete!");
        return false;
    }
    $('#advancedFilterTbl').find('input, textarea, button, select').prop('disabled', true);
    $('#autolaunchAddTbl').find('input, textarea, button, select').prop('disabled', true);
    $('#autolaunchPricesTbl').find('input, textarea, button, select').prop('disabled', true);
    document.getElementById("MainContent_hfMode").value = 'delete';
    document.getElementById("MainContent_btnFilters").value = 'Confirm Delete';
    document.getElementById("alAdvancedTitle").innerHTML = "Delete Wholesale AutoLaunch Rule";

    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/GetAutoLaunchRule',
        data: `{'kValue': ${selectedRule}, 'isSimple': 'false'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success) {
                AdvancedPopulateFilters(r.value, "delete");
            }
        }
    });
}

function AdvancedSaveAutoLaunchRule(mode) {
    toggleLoading(true, "Checking AutoLaunch Rule data...");

    if (mode != "delete") {
        var errors = AdvancedAutoLaunchRuleChecks();
        if (errors.length > 0) {
            alert("Please select a value for the following items:\n\t" + errors.join("\n\t"));
            toggleLoading(false, "Saving AutoLaunch Rule data...");
            return false;
        }
    }

    var item = {
        WholesaleAuctionName: document.getElementById("MainContent_lstAuction").options[document.getElementById("MainContent_lstAuction").selectedIndex].textContent,
        kWholesaleAutoLaunch: (mode == "add" ? 0 : parseInt(document.getElementById("MainContent_txtRule").value)),
        kWholesaleAuction: parseInt(document.getElementById("MainContent_lstAuction").value),
        Duration: parseInt(document.getElementById("duration").value),
        AgeLow: document.getElementById("minAge").value == '' ? "0" : parseInt(document.getElementById("minAge").value.replace(/,/g, '')),
        AgeHigh: document.getElementById("maxAge").value == '' ? "9999" : parseInt(document.getElementById("maxAge").value.replace(/,/g, '')),
        Make: document.getElementById("MainContent_lstMake").value == "0" ? "" : document.getElementById("MainContent_lstMake").value,
        Model: document.getElementById("MainContent_lstModel").value == "0" ? "" : document.getElementById("MainContent_lstModel").value,
        InvLotLocation: document.getElementById("MainContent_lstLotLocation").value,
        kAASale: 0,
        kInventoryStatus: parseInt(document.getElementById("MainContent_lstStatus").value),
        kWholesaleListingTag: document.getElementById("MainContent_lstListingType").value,
        kWholesaleTitleStatus: parseInt(document.getElementById("MainContent_lstTitle").value),
        kWholesaleListingCategory: document.getElementById("MainContent_advancedLstListingCategory").value,
        kWholesaleBidIncrement: (document.getElementById("MainContent_lstAuction").value == "1" ? parseInt(document.getElementById("MainContent_lstBidIncrement").value) : 0),
        kWholesaleLocationCode: (["1", "2"].includes(document.getElementById("MainContent_lstAuction").value) && document.getElementById("MainContent_lstLocationCode").value != '' ? parseInt(document.getElementById("MainContent_lstLocationCode").value) : 0),
        kWholesaleLocationIndicator: document.getElementById("MainContent_lstPhysLocation").value,
        kWholesaleFacilitatedAuctionCode: (["1", "2"].includes(document.getElementById("MainContent_lstAuction").value) && document.getElementById("MainContent_lstAuctionCode").value != '' ? parseInt(document.getElementById("MainContent_lstAuctionCode").value) : 0),
        StartPricingType: parseInt(document.getElementById("MainContent_lstPrimeStartPrice").value),
        StartPricingAdjustment: parseInt(document.getElementById("primeStartAdj").value.replace(/,/g, '')),
        StartPricingPercentage: parseInt(document.getElementById("primeStartPerc").value.replace(/,/g, '')) / 100,
        FloorPricingType: parseInt(document.getElementById("MainContent_lstPrimeFloorPrice").value),
        FloorPricingAdjustment: parseInt(document.getElementById("primeFloorAdj").value.replace(/,/g, '')),
        FloorPricingPercentage: parseInt(document.getElementById("primeFloorPerc").value.replace(/,/g, '')) / 100,
        BuyNowPricingType: document.getElementById("MainContent_lstPrimeBIN").value,
        BuyNowPricingAdjustment: parseInt(document.getElementById("primeBINAdj").value.replace(/,/g, '')),
        BuyNowPricingPercentage: parseInt(document.getElementById("primeBINPerc").value.replace(/,/g, '')) / 100,
        AltStartType: document.getElementById("MainContent_lstSecondStartPrice").value,
        AltStartPct: parseInt(document.getElementById("secondStartPerc").value.replace(/,/g, '')) / 100,
        AltStartAdj: parseInt(document.getElementById("secondStartAdj").value.replace(/,/g, '')),
        AltFloorType: parseInt(document.getElementById("MainContent_lstSecondFloorPrice").value),
        AltFloorPct: parseInt(document.getElementById("secondFloorPerc").value.replace(/,/g, '')) / 100,
        AltFloorAdj: parseInt(document.getElementById("secondFloorAdj").value.replace(/,/g, '')),
        AltBuyNowType: parseInt(document.getElementById("MainContent_lstSecondBIN").value),
        AltBuyNowPct: parseInt(document.getElementById("secondBINPerc").value.replace(/,/g, '')) / 100,
        AltBuyNowAdj: parseInt(document.getElementById("secondBINAdj").value.replace(/,/g, '')),
        Disable: (mode != "delete" ? 0 : 1),
        PricingAdjMin: parseInt(document.getElementById("minPriceAdj").value.replace(/,/g, '')),
        kFuelType: parseInt(document.getElementById("MainContent_lstFuelType").value),
        MinMileage: document.getElementById("minMile").value == '' ? 0 : parseInt(document.getElementById("minMile").value.replace(/,/g, '')),
        MaxMileage: document.getElementById("maxMile").value == '' ? 999999 : parseInt(document.getElementById("maxMile").value.replace(/,/g, '')),
        MotorYearMin: document.getElementById("MainContent_lstMinYear").value == "0" ? "0" : parseInt(document.getElementById("MainContent_lstMinYear").value),
        kWholesaleVehicleType: parseInt(document.getElementById("MainContent_lstVehicleType").value),
        kDealerWholesaleCredential: parseInt(document.getElementById("MainContent_lstCredentials").value),
        MotorYearMax: document.getElementById("MainContent_lstMaxYear").value == "0" ? "0" : parseInt(document.getElementById("MainContent_lstMaxYear").value),
        PostAsYellowLight: document.getElementById("arbPledge").checked ? 1 : 0,
        MinGrade: document.getElementById("MainContent_lstMinGrade").value == "-1" ? "0.0" : document.getElementById("MainContent_lstMinGrade").value,
        MaxGrade: document.getElementById("MainContent_lstMaxGrade").value == "-1" ? "5.0" : document.getElementById("MainContent_lstMaxGrade").value,
        RequireCR: parseInt(document.getElementById("MainContent_lstConditionRpt").value),
        kListingCategoryNoCR: (document.getElementById("MainContent_lstConditionRpt").value == "0" ? parseInt(document.getElementById("MainContent_lstCRCat").value) : parseInt(document.getElementById("MainContent_advancedLstListingCategory").value)),
        isInternal: 1,
        kWholesaleAuctionRuleSet: 0
    };

    toggleLoading(true, "Saving AutoLaunch Rule data...");
    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/SaveAutoLaunchRule',
        data: `{'isSimple': 'false', 'op': '${mode}', 'data': '${JSON.stringify(item)}'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success == 1) {
                mode == "delete" ? alert("Rule deleted successfully") : alert("Rule saved successfully");
                location.reload(true);
                toggleLoading(false, "Saving AutoLaunch Rule data...");
                toggleCssClass([['advancedOptions', 'show_display']]);

                return false;
            }

            alert("Rule failed to save! Please contact Support!");
            toggleCssClass([['advancedOptions', 'show_display']]);
            toggleLoading(false, "Saving AutoLaunch Rule data...");
        }
    });
}
//

// Simple Functions
function SimpleGridRowSelected(item) {
    document.getElementById("MainContent_txtRule").value = item[0].cells[2].childNodes[0].value;
}

function SimpleClearRowSelection() {
    document.getElementById("MainContent_txtRule").value = "";
}

function SimpleAddAutoLaunchRuleSet() {
    resetSelections('simple');
    document.getElementById("alSimpleTitle").innerHTML = "Add Wholesale AutoLaunch Rule";
    document.getElementById("MainContent_hfMode").value = 'add';
    document.getElementById("MainContent_simpleSave").value = 'Add Rule';
    $('#simpleFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#simplePricingFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    toggleCssClass([['simpleOptions', 'show_display']]);
}

function SimpleAutoLaunchRuleChecks(mode = "") {
    var policy = document.getElementById("MainContent_simpleLstListingCategory").value;
    var selectedAuctions = document.querySelectorAll(`input[type=checkbox][id*="CheckStart"]${document.getElementById("CheckAll").checked ? "" : ":checked"}`);
    var floorPriceStrategy = document.getElementById("MainContent_adjustment").value;
    var floorAdjustment = document.getElementById("MainContent_adjustmentPrice").value;
    var floorAdjustmentAmount = parseInt(document.getElementById("MainContent_adjustmentDollar").value.replace(/,/g, ''));
    var mmrPercentage = parseInt(document.getElementById("MainContent_MMRPercentage").value.replace(/,/g, ''));
    var floorBINAdjustment = parseInt(document.getElementById("MainContent_binAdjustment").value.replace(/,/g, ''));

    var errors = [];
    if (selectedAuctions.length == 0 && (mode != "edit" || mode != "delete"))
        errors.push("Select Auction(s)");
    if (floorPriceStrategy == "0")
        errors.push("Floor Price Strategy");
    if (floorAdjustment == "0")
        errors.push("Floor Adjustment");
    if (floorAdjustment != "3" && floorAdjustmentAmount == 0)
        errors.push("Invalid Floor Adjustment Amount");
    if (floorAdjustment == "24" && mmrPercentage == 0)
        errors.push("MMR Percentage");
    if (policy == 0)
        errors.push("Arbitration Policy");
    if (floorBINAdjustment == -1)
        errors.push("Buy Now Adjustment");

    if (parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')) < 0 || document.getElementById("MainContent_ageMin").value == "")
        errors.push("Age Low can not be negative or empty!");

    if (parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')) < 0 || document.getElementById("MainContent_ageMax").value == "")
        errors.push("Age High can not be negative or empty!");

    if (parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')) > parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')))
        errors.push("Age Low can not be greater than Age High!");

    if (parseInt(document.getElementById("MainContent_maxGrade").value.replace(/,/g, '')) < parseInt(document.getElementById("MainContent_minGrade").value.replace(/,/g, ''))) {
        errors.push("Max Grade cannot be lower than Min Grade!");
    }

    // Simple returns
    if (errors.length > 0)
        return errors;

    // MMR Check for selected Auctions
    var isMMR = document.getElementById("MainContent_WholesaleMMR").value;
    var maxMMRThreshold = 0;
    var minMMRThreshold = 80;

    for (var i = 0; i < selectedAuctions.length; i++) {
        var auctionMMRValue = parseInt(document.getElementById('MaxMMRPct_' + selectedAuctions[i].value).innerHTML);
        if (maxMMRThreshold == 0)
            maxMMRThreshold = auctionMMRValue;
        else if (auctionMMRValue < maxMMRThreshold)
            maxMMRThreshold = auctionMMRValue;
    }

    if (isMMR && floorPriceStrategy == 24) {
        if (mmrPercentage > parseInt(maxMMRThreshold)) {
            alert(`Floor Pricing Strategy must be less than the Maximum MMR Percentage of ${maxMMRThreshold}!`);
            errors.push("Max MMR Violation");
        } else if (mmrPercentage < parseInt(minMMRThreshold)) {
            alert(`Floor Pricing Strategy must be more than the Minimum MMR Percentage of ${minMMRThreshold}!`);
            errors.push("Min MMR Violation");
        }

        if ((mmrPercentage >= (maxMMRThreshold - 5) && mmrPercentage <= (maxMMRThreshold - 1)) && floorAdjustmentAmount != 0) {
            var popup = confirm(`CAUTION: Prices entered MIGHT exceed the ${maxMMRThreshold}% MMR restriction on some vehicles. Do you wish to continue?`);
            if (popup == true) {  /* confirm */ }
            else {
                errors.push("Check MMR Pct Value");
            }
        }
    }
    //if (floorPriceStrategy == "5" || floorPriceStrategy == "6") {
    //    if (floorAdjustment == "0")
    //        errors.push("Please select 'Decrease -' for Retail Pricing Strategy");
    //    else if (floorAdjustmentAmount < 100 && floorAdjustment != 3)
    //        errors.push("Invalid decrement amount! Please enter a value greater than 100!");
    //}

    // For Debuging
    //errors.push("Some error");

    return errors;
}

function SimpleEditAutoLaunchRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select an AutoLaunch Rule to edit!");
        return false;
    }

    // Re-enable is disabled
    $('#simpleFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#simplePricingFilterTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#simpleAddSettings').find('input, textarea, button, select').prop('disabled', false);

    document.getElementById("alSimpleTitle").innerHTML = "Edit Wholesale AutoLaunch Rule";
    document.getElementById("MainContent_hfMode").value = 'edit';
    document.getElementById("MainContent_simpleSave").value = 'Save Rule';
    resetSelections('simple');

    toggleLoading(true, "Gathering AutoLaunch rule info...");
    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/GetAutoLaunchRule',
        data: `{'kValue': ${selectedRule}, 'isSimple': 'true'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r) {
                SimplePopulateFilters(r.value);
            }
        }
    });
}

function SimplePopulateFilters(data) {
    document.getElementById("MainContent_ageMin").value = data.MinVehicleAge;
    document.getElementById("MainContent_ageMax").value = data.MaxVehicleAge;
    document.getElementById("MainContent_minGrade").value = data.MinGrade;
    document.getElementById("MainContent_maxGrade").value = data.MaxGrade;
    document.getElementById("MainContent_simpleLstListingCategory").value = data.AuctionRuleSet;

    document.getElementById("MainContent_MinimumPricingAdjustment").value = data.MinimumPricingAdjustment;
    document.getElementById("AllowOneDayAuctions").checked = data.Allow1Day == 1 ? true : false;
    document.getElementById("AllowDefaultCR").checked = data.AllowDefaultCR == 1 ? true : false;

    // Setup Selected Auctions
    var auctions = data.selectedAuctions.split(",");
    auctions.forEach(auction => {
        if (document.getElementById(`${auction}CheckStart`) != null)
            document.getElementById(`${auction}CheckStart`).checked = true;
    });

    // Floor Price Type
    document.getElementById("MainContent_adjustment").value = data.FloorPricingStrategy;
    // Floor Adjustment
    document.getElementById("MainContent_adjustmentPrice").value = data.FloorAdjustmentType;
    // Amount
    document.getElementById("MainContent_adjustmentDollar").value = parseInt(data.FloorAdjustmentAmt);
    toggleMMRDisclosure(document.getElementById("MainContent_adjustment"));

    // MMR Pricing if selected
    if (data.FloorPricingStrategy == 24)
        document.getElementById("MainContent_MMRPercentage").value = Math.round(parseFloat(data.MMRPercentage * 100))

    // Buy Now
    SetSelectedValue(document.getElementById("MainContent_binAdjustment"), data.BuyNowAdjustment);

    if (document.getElementById("MainContent_hfMode").value == "delete") {
        $('#simpleFilterTbl').find('input, textarea, button, select').prop('disabled', true);
        $('#simplePricingFilterTbl').find('input, textarea, button, select').prop('disabled', true);
        $('#simpleAddSettings').find('input, textarea, button, select').prop('disabled', true);
    }
    else if (document.getElementById("MainContent_hfMode").value == "edit") {
        document.getElementById("MainContent_simpleLstListingCategory").disabled = true;
        if (document.getElementById("MainContent_simpleLstListingCategory").value == '') {
            toggleLoading(false, "Gathering AutoLaunch rule info...");
            alert('The selected AutoLaunch Rule Set is no longer available! Please delete and create a new record.');
            return false;
        }
    }

    toggleCssClass([['simpleOptions', 'show_display']]);
    toggleLoading(false, "Gathering AutoLaunch rule info...");
}

function SimpleDeleteAutoLaunchRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select an AutoLaunch Rule to delete!");
        return false;
    }

    document.getElementById("alSimpleTitle").innerHTML = "Delete Wholesale AutoLaunch Rule";
    document.getElementById("MainContent_hfMode").value = 'delete';
    document.getElementById("MainContent_simpleSave").value = 'Confirm Delete';

    toggleLoading(true, "Gathering AutoLaunch rule info...");
    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/GetAutoLaunchRule',
        data: `{'kValue': ${selectedRule}, 'isSimple': 'true'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r) {
                SimplePopulateFilters(r.value);
            }
        }
    });
}

function SimpleSaveAutoLaunchRule(mode) {
    var items = [];

    if (mode != "delete") {
        var errors = SimpleAutoLaunchRuleChecks(mode);
        if (errors.length > 0) {
            alert("Please select a value for the following items:\n\t" + errors.join("\n\t"));
            return false;
        }
    }
    toggleLoading(true, "Saving Rule Information...");

    var auctions = document.querySelectorAll("[id*='CheckStart']:checked");
    var minPriceAdj = document.getElementById("MainContent_MinimumPricingAdjustment").value == "" ? 0 : parseInt(document.getElementById("MainContent_MinimumPricingAdjustment").value.replace(/,/g, ''));
    minPriceAdj = minPriceAdj > 0 ? 0 - minPriceAdj : minPriceAdj;

    var item = {
        kWholesaleAuctionRuleSet: document.getElementById("MainContent_txtRule").value == "" || mode == "add" ? "0" : document.getElementById("MainContent_txtRule").value,
        AuctionRuleSet: document.getElementById("MainContent_simpleLstListingCategory").value,
        MinVehicleAge: parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')),
        MaxVehicleAge: parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')),
        MinGrade: document.getElementById("MainContent_minGrade").value == "-1" ? "0.0" : document.getElementById("MainContent_minGrade").value,
        MaxGrade: document.getElementById("MainContent_maxGrade").value == "-1" ? "5.0" : document.getElementById("MainContent_maxGrade").value,
        FloorPricingStrategy: document.getElementById("MainContent_adjustment").value,
        FloorAdjustmentType: document.getElementById("MainContent_adjustmentPrice").value,
        FloorAdjustmentAmt: parseInt(document.getElementById("MainContent_adjustmentDollar").value.replace(/,/g, '')),
        MMRPercentage: document.getElementById("MainContent_adjustment").value == "24" ? (parseInt(document.getElementById("MainContent_MMRPercentage").value) / 100).toString() : "0",
        BuyNowAdjustment: parseInt(document.getElementById("MainContent_binAdjustment").value.replace(/,/g, '')),
        MinimumPricingAdjustment: minPriceAdj,
        Allow1Day: document.getElementById("AllowOneDayAuctions").checked ? 1 : 0,
        AllowDefaultCR: document.getElementById("AllowDefaultCR").checked ? 1 : 0
    };

    auctions.forEach(
        auction => {
            var auctioName = auction.id.replace("CheckStart", "");
            if (auctioName == "IAS")
                item[`isIntegratedAuctionSolutions`] = "1";
            else
                item[`is${auctioName}`] = "1";
        }
    );

    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/SaveAutoLaunchRule',
        data: `{'isSimple': 'true', 'op': '${mode}', 'data': '${JSON.stringify(item)}'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success) {
                toggleLoading(false, "Saving Rule Information...")
                mode == "delete" ? alert("Rule deleted successfully!\n\tPlease End Wholesale on any existing listings.") : alert("Rule saved successfully!\n\tNote: Please allow up to an hour for your listings to go live.\n\tPlease verify your listings for accuracy.");
                location.reload(true);
                toggleCssClass([['simpleOptions', 'show_display']]);
                return false;
            }
            else if (r.message != "") {
                alert("Unable to create/update new AutoLaunch Rule Set:\n" + r.message);
                toggleLoading(false, "Saving Rule Information...");
            }
            else {
                alert("Rule failed to save! Please contact Support!");
                toggleLoading(false, "Saving Rule Information...")
            }
        }
    });
}
// Simple Functions

function AutoLaunchRuleTest() {
    var errors = [];
    if (parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')) > parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')))
        errors.push("Min Age can not be greater than Max Age!");

    if (parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')) < 0 || document.getElementById("MainContent_ageMin").value == "")
        errors.push("Min Age can not be less then 0 or negative!");

    if (parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')) < 0 || document.getElementById("MainContent_ageMax").value == "")
        errors.push("Max Age can not be less then 0 or negative!");

    if (parseInt(document.getElementById("MainContent_maxGrade").value.replace(/,/g, '')) < parseInt(document.getElementById("MainContent_minGrade").value.replace(/,/g, '')))
        errors.push("Max Grade cannot be lower than Min Grade!");

    if (errors.length != 0) {
        alert("Please address the following items before testing rule:\n\t" + errors.join("\n\t"));
        return false;
    }

    var json = {
        MinAge: parseInt(document.getElementById("MainContent_ageMin").value.replace(/,/g, '')),
        MaxAge: parseInt(document.getElementById("MainContent_ageMax").value.replace(/,/g, '')),
        MinGrade: document.getElementById("MainContent_minGrade").value == "-1" ? "0.0" : document.getElementById("MainContent_minGrade").value,
        MaxGrade: document.getElementById("MainContent_maxGrade").value == "-1" ? "5.0" : document.getElementById("MainContent_maxGrade").value
    }

    toggleLoading(true, "Gather inventory...Testing Age Range with Grade...");

    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/AutoLaunchRuleTest',
        data: "{'json': '" + JSON.stringify(json) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success) {
                document.getElementById("ruleCount").innerHTML = `Total Vehicles: ${r.value.filtered} of ${r.value.total}`;
            }

            toggleLoading(false, "Gather inventory...Testing Age Range with Grade...", 500);
            return false;
        }
    });
}

function BuildDropdownList(stringList, htmlElement) {
    var list = [];
    if (stringList == "[]:|") {
        stringList = ":|";
    }

    var options = stringList.split('|');
    for (var i = 0; i < options.length; i++) {
        var option = options[i].split(':');
        if (option == "")
            break;
        var el = document.createElement("option");
        el.text = option[1];
        el.value = option[0];
        list.push(el);
    }

    list.forEach(l => htmlElement.appendChild(l));
    if (options.length > 1)
        return false;
    return true;
}

function OnMakeChange() {
    var lstMake = document.getElementById("MainContent_lstMake");
    var lstModel = document.getElementById("MainContent_lstModel");
    var selectedMake = lstMake.options[lstMake.selectedIndex].value;

    if (selectedMake != "0") {
        $.ajax({
            type: "POST",
            url: 'AutoLaunchRules.aspx/GetModelList',
            data: "{'make': '" + lstMake.options[lstMake.selectedIndex].innerHTML + "', 'year': '0'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            },
            success: function (response) {
                if (response.d != "") {
                    lstModel.options.length = 1;
                    lstModel.disabled = false
                    BuildDropdownList(response.d, lstModel);
                }
                else {
                    lstModel.options.length = 1;
                    lstModel.disabled = true;
                }
            }
        });
    }
    else {
        lstModel.options.length = 1;
        document.getElementById("MainContent_lstModel").disabled = true;
    }
}

function OnAuctionChange(op = "", data = null) {
    // For Advanced View
    var lstAuctions = document.getElementById("MainContent_lstAuction");
    var selectedAuction = lstAuctions.options[lstAuctions.selectedIndex].value;
    var lstCredentials = document.getElementById("MainContent_lstCredentials");

    if (op == "")
        op = document.getElementById("MainContent_hfMode").value;

    // Toggle loading gif
    if (op == "add")
        toggleLoading(true, "Gathering Auction info...");

    $.ajax({
        type: "POST",
        url: 'AutoLaunchRules.aspx/GetAuctionCredentials',
        data: `{'kWholesaleAuction': ${selectedAuction} }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success) {
                lstCredentials.options.length = 0;
    
                BuildDropdownList(r.value, document.getElementById("MainContent_lstCredentials"));
                if (op == "edit")
                    document.getElementById("MainContent_lstCredentials").value = data.filters.kDealerWholesaleCredential;
            }
        }
    });

    if (selectedAuction == "1" && op == "edit") {
        document.getElementById("MainContent_lstAuctionCode").value = data.additional.kWholesaleFacilitatedAuction;
        document.getElementById("MainContent_lstLocationCode").value = data.additional.kWholesaleLocationCode;
        document.getElementById("MainContent_lstBidIncrement").value = data.additional.kWholesaleBidIncrement;
    } else if (selectedAuction == "1" || selectedAuction == "2") {
        $.ajax({
            type: "POST",
            url: 'AutoLaunchRules.aspx/UpdateAuctionDropdowns',
            data: `{'kWholesaleAuction': ${selectedAuction} }`,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            },
            success: function (response) {
                var r = response.d;
                if (r.success) {
                    document.getElementById("MainContent_lstLocationCode").options.length = 0;
                    BuildDropdownList(r.value.LocationCodes, document.getElementById("MainContent_lstLocationCode"));
                    document.getElementById("MainContent_lstLocationCode").value = r.value.LocationSelection;

                    document.getElementById("MainContent_lstAuctionCode").value = r.value.AuctionCode;
                    document.getElementById("MainContent_lstBidIncrement").value = r.value.BidIncrement;
                }
            }
        });
    }

    // Hide all relative auction fields
    document.getElementById("locationCode").style.display = "none";
    document.getElementById("auctionCode").style.display = "none";
    document.getElementById("MainContent_lstBidIncrement").style.display = "none";
    document.getElementById("BidIncrement").style.display = "none";
    document.getElementById("limitedArb").style.display = "none";

    // Clear out default options and add compatible listing types per auction
    //lstListingType.options.length = 1;

    var crLabel = document.getElementById("CrCat");
    var crList = document.getElementById("MainContent_lstCRCat");
    var crSelection = document.getElementById("MainContent_lstConditionRpt");
    if (selectedAuction == "2" || selectedAuction == "19") {
        crSelection.selectedIndex = 1;
        crSelection.disabled = true;

        crList.style.display = "none";
        crLabel.style.display = "none";
        
    } else {
        crList.style.display = "initial";
        crLabel.style.display = "initial";

        crSelection.selectedIndex = 0;
        crSelection.disabled = false;
    }

    var lstListingTypes = document.getElementById("MainContent_lstListingType");
    var options;
    switch (selectedAuction) {
        // OVE
        case "1":
            options = ["Bid", "Buy", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer", "Buy/Offer", "Offer Only"];

            document.getElementById("duration").value = 3;
            document.getElementById("locationCode").style.display = "table-row";
            document.getElementById("auctionCode").style.display = "table-row";
            document.getElementById("MainContent_lstBidIncrement").style.display = "block";
            document.getElementById("BidIncrement").style.display = "table-cell";
            break;
        // SmartAuction
        case "2":
            options = ["Bid", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer"];
            document.getElementById("duration").value = 5;
            //document.getElementById("limitedArb").style.display = "table-row";
            document.getElementById("locationCode").style.display = "table-row";
            // Additional Logic
            break;
        // OpenLane
        case "4":
            options = ["Bid", "Buy", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer", "Buy/Offer"];
            document.getElementById("duration").value = 5;
            break;
        // DTC
        case "5":
            options = ["Buy", "Buy/Offer"];
            
            // Additional Logic
            break;
        // COPART
        case "6":
            options = ["Bid"];
            document.getElementById("duration").value = 5;
            // Additional Logic
            break;
        // Auction Edge
        case "7":
            options = ["Buy/Offer"];
            document.getElementById("duration").value = 5;
            document.getElementById("auctionCode").style.display = "table-row";
            // Additional Logic
            break;
        // Turn Auctions
        case "10":
            options = ["Bid/Offer"];
            document.getElementById("duration").value = 5;
            // Additional Logic
            break;
        // ACV Auctions
        case "11":
            options = ["Bid", "Buy", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer", "Buy/Offer"];
            document.getElementById("duration").value = 5;
            break;
        // eDealer Direct
        case "12":
            options = ["Bid", "Bid/Buy"];
            
            document.getElementById("duration").value = 5;
            break;
        // IAA
        case "13":
            options = ["Buy"];
            document.getElementById("duration").value = 5;
            break;
        // Auction Simplified
        case "14":
            options = ["Bid", "Buy", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer", "Buy/Offer"];
            break;
        default:
            options = ["Bid", "Buy", "Bid/Buy", "Bid/Offer", "Bid/Buy/Offer", "Buy/Offer", "Offer Only"];
            document.getElementById("duration").value = 5;
    }

    // Set selections
    Array.from(lstListingTypes.options).forEach(option => {
        if (options.includes(option.textContent))
            option.disabled = false;
        else
            option.disabled = true;
    });

    // Prevent previous option, if disabled, from being submitted on accident
    if (lstListingTypes[lstListingTypes.selectedIndex].disabled == true) {
        Array.from(lstListingTypes.options).every(option => {
            if (option.disabled == false) {
                lstListingTypes.value = option.value;
                ListingTypeChange();
                return false;
            }
        });
    }

    toggleLoading(false, "Gathering Auction info...");
}

function ListingTypeChange() {
    // For Advanced View
    var type = document.getElementById("MainContent_lstListingType").value;
    var auction = document.getElementById("MainContent_lstAuction").value;
    switch(type)
    {
        case "1": //Bid
            TogglePricing('StartPrice', false);
            TogglePricing('FloorPrice', false);
            TogglePricing('BuyNowPrice', true);
            ClearPriceField('BuyNowPrice');
            SetupPriceField('StartPrice');
            SetupPriceField('FloorPrice');
            break;
        case "2": //Buy
            TogglePricing('StartPrice', true);
            TogglePricing('FloorPrice', true);
            TogglePricing('BuyNowPrice', false);
            ClearPriceField('StartPrice');
            ClearPriceField('FloorPrice');
            SetupPriceField('BuyNowPrice');
            break;
        case "3": //Bid/Buy
            TogglePricing('StartPrice', false);
            TogglePricing('FloorPrice', false);
            TogglePricing('BuyNowPrice', false);
            SetupPriceField('StartPrice');
            SetupPriceField('FloorPrice');
            SetupPriceField('BuyNowPrice');
            break;
        case "4": //Bid/Offer
            TogglePricing('StartPrice', false);
            TogglePricing('FloorPrice', false);
            TogglePricing('BuyNowPrice', true);
            ClearPriceField('BuyNowPrice');
            SetupPriceField('StartPrice');
            SetupPriceField('FloorPrice');
            break;
        case "5": //Bid/Buy/Offer
            TogglePricing('StartPrice', false);
            TogglePricing('FloorPrice', false);
            TogglePricing('BuyNowPrice', false);
            SetupPriceField('StartPrice');
            SetupPriceField('FloorPrice');
            SetupPriceField('BuyNowPrice');
            break;
        case "6": //Buy/Offer
            TogglePricing('StartPrice', true);
            TogglePricing('FloorPrice', true);
            TogglePricing('BuyNowPrice', false);
            ClearPriceField('StartPrice');
            ClearPriceField('FloorPrice');
            SetupPriceField('BuyNowPrice');
            break;
        case "7": //Offer Only
            TogglePricing('StartPrice', true);
            TogglePricing('FloorPrice', false);
            TogglePricing('BuyNowPrice', true);
            ClearPriceField('StartPrice');
            ClearPriceField('BuyNowPrice');
            SetupPriceField('FloorPrice');
            break;
    }
    switch (auction) {
        case "2": //SA
            TogglePricing('StartPrice', true);
            ClearPriceField('StartPrice');
            SetupPriceField('FloorPrice');
            SetupPriceField('BuyNowPrice');
            break;

        case "6": //COPART
            TogglePricing('StartPrice', true);
            TogglePricing('BuyNowPrice', true);
            ClearPriceField('StartPrice');
            ClearPriceField('BuyNowPrice');
            SetupPriceField('FloorPrice');
            break;

        case "13": //IAA
            TogglePricing('StartPrice', true);
            ClearPriceField('StartPrice');
            TogglePricing('FloorPrice', true);
            ClearPriceField('FloorPrice');
            break;

        case "14": //Auction Simplified
            // togglePricing('StartPrice', true);
            // ClearPriceField('StartPrice');
            // togglePricing('FloorPrice', true);
            // ClearPriceField('FloorPrice');
            break;

        case "15": //IAS
            // togglePricing('StartPrice', true);
            // ClearPriceField('StartPrice');
            // togglePricing('FloorPrice', true);
            // ClearPriceField('FloorPrice');
            break;

        case "16": //AOS
            // togglePricing('StartPrice', true);
            // ClearPriceField('StartPrice');
            // togglePricing('FloorPrice', true);
            // ClearPriceField('FloorPrice');
            break;
    }
}

function TogglePricing(type, toggle) {
    if (type == "StartPrice") {
        document.getElementById("MainContent_lstPrimeStartPrice").disabled = toggle;
        document.getElementById("primeStartAdj").disabled = toggle;
        document.getElementById("primeStartPerc").disabled = toggle;
        document.getElementById("MainContent_lstSecondStartPrice").disabled = toggle;
        document.getElementById("secondStartAdj").disabled = toggle;
        document.getElementById("secondStartPerc").disabled = toggle;
    }
    else if (type == "FloorPrice") {
        document.getElementById("MainContent_lstPrimeFloorPrice").disabled = toggle;
        document.getElementById("primeFloorAdj").disabled = toggle;
        document.getElementById("primeFloorPerc").disabled = toggle;
        document.getElementById("MainContent_lstSecondFloorPrice").disabled = toggle;
        document.getElementById("secondFloorAdj").disabled = toggle;
        document.getElementById("secondFloorPerc").disabled = toggle;
    }
    else if (type == "BuyNowPrice") {
        document.getElementById("MainContent_lstPrimeBIN").disabled = toggle;
        document.getElementById("primeBINAdj").disabled = toggle;
        document.getElementById("primeBINPerc").disabled = toggle;
        document.getElementById("MainContent_lstSecondBIN").disabled = toggle;
        document.getElementById("secondBINAdj").disabled = toggle;
        document.getElementById("secondBINPerc").disabled = toggle;
    }
}

function SetupPriceField(type) {
    if (type == "StartPrice") {
        document.getElementById("primeStartPerc").value = 100;
        document.getElementById("secondStartPerc").value = 100;
    }
    else if (type == "FloorPrice") {
        document.getElementById("primeFloorPerc").value = 100;
        document.getElementById("secondFloorPerc").value = 100;
    }
    else if (type == "BuyNowPrice") {
        document.getElementById("primeBINPerc").value = 100;
        document.getElementById("secondBINPerc").value = 100;
    }
}

function ClearPriceField(type) {
    if (type == "StartPrice") {
        document.getElementById("MainContent_lstPrimeStartPrice").value = 0;
        document.getElementById("primeStartAdj").value = 0;
        document.getElementById("primeStartPerc").value = 0;
        document.getElementById("MainContent_lstSecondStartPrice").value = 0;
        document.getElementById("secondStartAdj").value = 0;
        document.getElementById("secondStartPerc").value = 0;
    }
    else if (type == "FloorPrice") {
        document.getElementById("MainContent_lstPrimeFloorPrice").value = 0;
        document.getElementById("primeFloorAdj").value = 0;
        document.getElementById("primeFloorPerc").value = 0;
        document.getElementById("MainContent_lstSecondFloorPrice").value = 0;
        document.getElementById("secondFloorAdj").value = 0;
        document.getElementById("secondFloorPerc").value = 0;
    }
    else if (type == "BuyNowPrice") {
        document.getElementById("MainContent_lstPrimeBIN").value = 0;
        document.getElementById("primeBINAdj").value = 0;
        document.getElementById("primeBINPerc").value = 0;
        document.getElementById("MainContent_lstSecondBIN").value = 0;
        document.getElementById("secondBINAdj").value = 0;
        document.getElementById("secondBINPerc").value = 0;
    }
}

function OnCRChange() {
    // For Advanced View
    var crLabel = document.getElementById("CrCat");
    var crList = document.getElementById("MainContent_lstCRCat");
    var crSelection = document.getElementById("MainContent_lstConditionRpt");

    if (crSelection.selectedIndex == 0) {
        crList.style.display = "initial";
        crLabel.style.display = "initial";
    } else {
        crList.style.display = "none";
        crLabel.style.display = "none";
    }
}

function SetSelectedValue(control, value) {
    var opt = control.options;
    for (var i = 0; i < control.options.length; i++) {
        if (opt[i].value == value) {
            opt[i].selected = true;
            control.value = value;
        }
    }
}

function MinMaxYearChange(reset) {
    var minYear = document.getElementById("MainContent_lstMinYear");
    var maxYear = document.getElementById("MainContent_lstMaxYear");

    maxYear.options.length = 0;
    for (var i = 0; i < minYear.options.length; i++)
        maxYear.add(new Option(minYear.options[i].text, minYear.options[i].value));
    if (reset == true)
        return false;

    var maxYearArray = Array.apply(null, maxYear.options);
    var index = maxYearArray.findIndex(year => year.value == minYear.value);
    maxYear.options.length = index + 1;
    maxYear.selectedIndex = index;
}

function ListView() {
    var mode = (document.getElementById("MainContent_advancedjsGrid").className == "jsgrid" || document.getElementById("MainContent_simplejsGrid").className == "jsgrid" )
        || document.getElementById("MainContent_hfViewList").value == "1" ? true : false;
    var internalFlag = document.getElementById("MainContent_InternalFlag").value == "1" ? true : false;
    var gridName = internalFlag ? "MainContent_advancedjsGrid" : "MainContent_simplejsGrid";

    if (!internalFlag) {
        document.getElementById("MainContent_advancedjsGrid").className = "Hide"; //== document.getElementById("MainContent_simplejsGrid").className == "jsgrid" ? : "jsgrid";
        //document.getElementById("MainContent_simplejsGrid").className == document.getElementById("MainContent_simplejsGrid").className == "jsgrid" ? : "jsgrid";
    }

    if (mode) {
        document.getElementById(gridName).className = "Hide";
        document.getElementById("MainContent_AutoLaunchAdd").className = "Hide";
        document.getElementById("MainContent_AutoLaunchEdit").className = "Hide";
        document.getElementById("MainContent_AutoLaunchDelete").className = "Hide";
        document.getElementById("MainContent_AutoLaunchList").value = "Grid";
        document.getElementById("MainContent_AutoLaunchPrint").className = "submitButton";
        document.getElementById("MainContent_ListForm").className = "jsgrid print";
    }
    else {
        document.getElementById(gridName).className = "jsgrid";
        document.getElementById("MainContent_AutoLaunchAdd").className = "submitButton";
        document.getElementById("MainContent_AutoLaunchEdit").className = "submitButton";
        document.getElementById("MainContent_AutoLaunchDelete").className = "submitButton";
        document.getElementById("MainContent_AutoLaunchList").value = "List";
        document.getElementById("MainContent_AutoLaunchPrint").className = "Hide";
        document.getElementById("MainContent_ListForm").className = "Hide";
    }
}

function simpleToggle() {
    toggleCssClass([["MainContent_advancedjsGrid", "Hide"], ["MainContent_simplejsGrid", "Hide"]]);
}

function PricingCheck(self) {
    var price = parseInt(self.value.replace(/,/g, ''));

    var binIncrements = document.getElementById("MainContent_binAdjustment").options;

    if (document.getElementById("MainContent_adjustment").value == "5"
        || document.getElementById("MainContent_adjustment").value == "6") {
        if (price % 100 != 0) {
            alert("Please ensure that any Floor Adjustment amounts are in increments of $100!");
            return false;
        }

        //for (var i = 2; i < binIncrements.length; i++) {
        //    if (binIncrements[i].value < price) {
        //        binIncrements[i].disabled = false;
        //    } else {
        //        binIncrements[i].disabled = true;
        //    }
        //}
    }
    else {
        if (price % 100 != 0) {
            alert("Please ensure that any Floor Adjustment amounts are in increments of $100!");
            return false;
        }

        Array.from(binIncrements).forEach(bid => bid.disabled = false);
        binIncrements[0].selected = true;
    }

    document.getElementById("MainContent_binAdjustment").value = -1;

    return false;
}

function AuctionCheck(self) {
    if (self.id == "CheckAll" && self.checked)
        document.querySelectorAll("[id*='CheckStart']").forEach(check => { check.checked = true; });
    else if (self.id == "CheckAll" && !self.checked) {
        document.getElementById("CheckAll").checked = false;
        document.querySelectorAll("[id*='CheckStart']").forEach(check => { check.checked = false; });
    }
    else
        document.getElementById("CheckAll").checked = false;

    return false;
}

function PriceSetNone(self) {
    document.getElementById("MainContent_adjustmentDollar").value = self.selectedIndex == 3 ? '0' : '100';
}

function toggleMMRDisclosure(self) {
    document.getElementById("MainContent_adjustmentDollar").value = document.getElementById("MainContent_adjustmentDollar").value == "" ? 100 : parseInt(document.getElementById("MainContent_adjustmentDollar").value.replace(/,/g, ''));

    if (self.selectedIndex == 4) {
        document.getElementById("lblMMR").style.visibility = document.getElementById("lblMMRinput").style.visibility = "initial";
        // Prevent the user from making adjustments based on MMR Pricings
        //document.getElementById("MainContent_adjustmentPrice").disabled = true;
        //document.getElementById("MainContent_adjustmentPrice").selectedIndex = 3;
        //document.getElementById("MainContent_adjustmentDollar").value = 0;
        //document.getElementById("MainContent_adjustmentDollar").disabled = true;

        document.getElementById("mmrPricingDisclosure").style.display = "block";
        document.getElementById("pricingDisclosure").style.display = "block";
        document.getElementById("MainContent_MMRPercentage").value = document.getElementById("MainContent_MMRPercentage").value == "" ? 100 : document.getElementById("MainContent_MMRPercentage").value;
    }
    else if (self.selectedIndex == 0 || self.selectedIndex == 3){
        //document.getElementById("MainContent_adjustmentPrice").options[1].disabled = false;
        document.getElementById("lblMMR").style.visibility = document.getElementById("lblMMRinput").style.visibility = "hidden";
        //document.getElementById("MainContent_adjustmentPrice").disabled = false;
        //document.getElementById("MainContent_adjustmentDollar").disabled = false;
        document.getElementById("mmrPricingDisclosure").style.display = "none";
        document.getElementById("pricingDisclosure").style.display = "block";
        if (self.selectedIndex == 0)
            document.getElementById("pricingDisclosure").style.display = "none";
        if (self.selectedIndex == 3) {
            //document.getElementById("MainContent_adjustmentPrice").options[3].disabled = true;
            document.getElementById("MainContent_adjustmentDollar").value = document.getElementById("MainContent_adjustmentDollar").value == "" ? 100 : document.getElementById("MainContent_adjustmentDollar").value;
        }
    }
    else {
        var dollarAdj = document.getElementById("MainContent_adjustmentDollar").value == "" ? 100 : parseInt(document.getElementById("MainContent_adjustmentDollar").value.replace(/,/g, ''));
        //document.getElementById("MainContent_adjustmentPrice").options[1].disabled = true;
        //document.getElementById("MainContent_adjustmentPrice").disabled = document.getElementById("MainContent_adjustmentPrice").options[3].disabled = false;
        document.getElementById("MainContent_adjustmentPrice").selectedIndex = document.getElementById("MainContent_adjustmentPrice").selectedIndex != 0 ? document.getElementById("MainContent_adjustmentPrice").selectedIndex : 2;
        //document.getElementById("MainContent_adjustmentDollar").disabled = false;
        document.getElementById("MainContent_adjustmentDollar").value = dollarAdj;
        document.getElementById("mmrPricingDisclosure").style.display = "none";
        document.getElementById("lblMMR").style.visibility = document.getElementById("lblMMRinput").style.visibility = "hidden";
        document.getElementById("pricingDisclosure").style.display = "block";
    }

    PricingCheck(document.getElementById("MainContent_adjustmentDollar"))
    return false;
}

function resetSelections(view) {
    if (view == 'simple') {
        // Filters
        document.getElementById("simpleFilterTbl").querySelectorAll('input[type=text]:not([readonly])').forEach(node => node.value = '');
        document.getElementById("simpleFilterTbl").querySelectorAll('input[type=checkbox]').forEach(node => node.checked = false);
        document.getElementById("simpleFilterTbl").querySelectorAll('select').forEach(node => node.selectedIndex = 0);

        // Pricing simplePricingFilterTbl
        document.getElementById("simplePricingFilterTbl").querySelectorAll('input[type=text]:not([readonly])').forEach(node => node.value = '');
        document.getElementById("simplePricingFilterTbl").querySelectorAll('select').forEach(node => node.selectedIndex = 0);

        document.getElementById("pricingDisclosure").style.display = "none";
        document.getElementById("mmrPricingDisclosure").style.display = "none";
        document.getElementById("lblMMR").style.visibility = "hidden";
        document.getElementById("lblMMRinput").style.visibility = "hidden";

        document.getElementById("MainContent_MinimumPricingAdjustment").value = "-2000";
        document.getElementById("AllowDefaultCR").checked = false;
        document.getElementById("AllowOneDayAuctions").checked = false;
    } else {
        document.getElementById("advancedFilterTbl").querySelectorAll('select').forEach(node => { node.selectedIndex = node.id != "MainContent_lstStatus" ? 0 : 1; });
        document.getElementById("advancedFilterTbl").querySelectorAll('input').forEach(node => node.value = '');

        document.getElementById("alAdd").querySelectorAll('select').forEach(node => node.selectedIndex = 0);
        document.getElementById("alPrices").querySelectorAll('input').forEach(node => { node.value = node.id.includes("Perc") ? 100 : '0' });

        document.getElementById("alAdd").querySelectorAll('select').forEach(node => node.selectedIndex = 0);
        document.getElementById("alPrices").querySelectorAll('input').forEach(node => { node.value = node.id.includes("Perc") ? 100 : '0' });
    }
}

function HelpIconClick() {
    alert('herro');
}

$(document).ready(function () {
    ListView();
    document.getElementById("MainContent_hfViewList").value = "0";
});