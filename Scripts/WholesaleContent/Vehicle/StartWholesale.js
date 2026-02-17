// General auction OnChange
function OnChange(auctionName) {
    var startPriceField = document.getElementById("MainContent_StartPrice");
    var startPriceButton = document.getElementById("MainContent_setStart");

    var chkForce = document.getElementById("MainContent_chkForceWholesalePrice").checked;
    var chk = checkAuction($(`input[id^="${auctionName}Check"]`));
    var adesaChk = checkAuction($('input[id^="ADESACheck"]'));

    if (chkForce.checked) {
        return false;
    } else if (!chk && !adesaChk) {
        startPriceButton.style.opacity = "10%";
        startPriceField.disabled = startPriceButton.disabled = true;
    } else {
        startPriceButton.style.opacity = "100%";
        startPriceField.disabled = startPriceButton.disabled = false;
    }

}

function OnChangeBid() {
    var bidIncrement = document.getElementById("MainContent_lstBidIncrement");
    var chkOVE = checkAuction($('input[id^="OVECheck"]'));
    var chkSmartAuction = checkAuction($('input[id^="SmartAuctionCheck"]'));
    var startPriceField = document.getElementById("MainContent_StartPrice");
    var startPriceButton = document.getElementById("MainContent_setStart");

    if (!chkOVE) {
        bidIncrement.disabled = true;
        startPriceButton.style.opacity = "10%";
        startPriceField.disabled = startPriceButton.disabled = true;
    } else {
        bidIncrement.disabled = false;
        startPriceButton.style.opacity = "100%";
        startPriceField.disabled = startPriceButton.disabled = false;
    }
}

function adesaOnChange() {
    var chkADESA = checkAuction($('input[id^="ADESACheck"]'));
    var startPriceField = document.getElementById("MainContent_StartPrice").checked;
    var startPriceButton = document.getElementById("MainContent_setStart");
    var chkForce = document.getElementById("MainContent_chkForceWholesalePrice").checked;

    if (!chkADESA || chkForce) {
        startPriceButton.style.opacity = "10%";
        startPriceField.disabled = startPriceButton.disabled = true;
    } else {
        startPriceButton.style.opacity = "100%";
        startPriceField.disabled = startPriceButton.disabled = false;
    }
}

function SetPrice(priceField) {
    var percent = parseInt(document.getElementById("percent").value);
    var dollar = parseInt(document.getElementById("dollar").value.replace(/,/g, ''))
    var vehicleMMR = parseInt(document.getElementById("MainContent_MMRValue").innerText);
    var maxMMRPct = 0;
    var minMMRPct = 80;

    var auctionChecks = document.querySelectorAll('[id*="CheckStart"]:checked');
    for (var i = 0; i < auctionChecks.length; i++)
    {
        var auctionMMRValue = parseInt(document.getElementById('MaxMMRPct_' + auctionChecks[i].id.replace('CheckStart', '')).innerHTML);
        if (maxMMRPct == 0)
            maxMMRPct = auctionMMRValue;
        else if (auctionMMRValue < maxMMRPct)
            maxMMRPct = auctionMMRValue;
    }

    if (percent > maxMMRPct) {
        alert(`MMR Percentage cannot exceed ${maxMMRPct}%!`);
        return false;
    }

    if (!(isNaN(vehicleMMR)) && vehicleMMR != 0) {
        var mmrCalc = Math.floor(((vehicleMMR * percent / 100) + dollar) / 100 * 100);
        if (percent == maxMMRPct && dollar != 0) {
            alert(`MMR Percentage cannot exceed ${maxMMRPct}%! Applying ${maxMMRPct}% of MMR...`);
            mmrCalc = Math.floor(((vehicleMMR * percent / 100)) / 100 * 100);
        } else if (percent < minMMRPct) {
            alert(`MMR Percentage cannot subceed 80%! Applying 80% of MMR...`);
            mmrCalc = Math.floor(((vehicleMMR * percent / 100) + dollar) / 100 * 100);
        }

        if (priceField == "StartPrice") {
            document.getElementById('MainContent_' + priceField).value = mmrCalc;
        }
        else if (priceField == "ReservePrice") {
            document.getElementById('MainContent_' + priceField).value = mmrCalc;
        }
        else {
            document.getElementById('MainContent_' + priceField).value = mmrCalc;
        }
    }

    return false;
}

function ValidateWholesale() {
    var auctionChecks = $('[id*="CheckStart"]:checked');
    var oveList = checkAuction($('[id="OVECheckStart"]:checked'));
    var saList = checkAuction($('[id="SmartAuctionCheckStart"]:checked'));
    var olList = checkAuction($('[id="ADESACheckStart"]:checked'));
    var taList = checkAuction($('[id="TurnAuctionCheckStart"]:checked'));
    var aeList = checkAuction($('[id="AuctionEdgeCheckStart"]:checked'));
    var acvList = checkAuction($('[id="ACVAuctionsCheckStart"]:checked'));
    var ddList = checkAuction($('[id="eDealerDirectCheckStart"]:checked'));
    var cpList = checkAuction($('[id="COPARTCheckStart"]:checked'));
    var iaaList = checkAuction($('[id="IAACheckStart"]:checked'));
    var asList = checkAuction($('[id="AuctionSimplifiedCheckStart"]:checked'));
    var carmigoList = checkAuction($('[id="CarmigoCheckStart"]:checked'));
    var remarketingPlusList = checkAuction($('[id="RemarketingPlusCheckStart"]:checked'));

    if (auctionChecks.length != 0) {
        var listingCat = document.getElementById("MainContent_lstListingCategory");
        var lstListingType = document.getElementById("MainContent_lstListingType");
        var listingType = lstListingType.options[lstListingType.selectedIndex].value;
        var listingTypeName = lstListingType.options[lstListingType.selectedIndex].outerText;

        var vin = document.getElementById("MainContent_HiddenVIN").value;
        if (vin == "") {
            alert("VIN is required to list on this site. Please close this window and use the Change VIN feature to add a VIN for this vehicle.\r\n");
            return false;
        }

        if (listingTypeName == "Bid/Buy/Offer (OVE Bid/Buy)" || listingTypeName == "Bid/Buy/Offer (OVE Buy/Offer)")
            listingTypeName = "Bid/Buy/Offer";
        else if (listingTypeName == "Bid/Offer (OVE Offer Only)")
            listingTypeName = "Bid/Offer";
        else if (listingTypeName == "Buy/Offer (OVE Offer Only)")
            listingTypeName = "Buy/Offer";

        if (auctionChecks.length == 0) {
            alert("You must select at least one Wholesale Marketplace");
            return false;
        }

        if (listingCat.options[listingCat.selectedIndex].value == "0") {
            alert("You must select the Listing Category");
            return false;
        }

        // Validate Listing Type for selected auctions
        // Replace is for ',' delimited values since we are restricting to numbers
        var binPrice = parseInt(document.getElementById("MainContent_BINPrice").value.replace(/,/g, ''));
        var reservePrice = parseInt(document.getElementById("MainContent_ReservePrice").value.replace(/,/g, ''));
        var startPrice = parseInt(document.getElementById("MainContent_StartPrice").value.replace(/,/g, ''));
        if (listingType == 7 && (auctionChecks.length > 1)) {
            alert("Offer Only is only available for OVE auctions");
            return false;
        }

        if (saList && (listingType == 2 || listingType == 6 || listingType == 120)) {
            alert(`The listing type selected, ${listingTypeName}, is not supported by SmartAuction`);
            return false;
        }

        // Probably can remove this piece of logic but putting here just in case
        if ((taList || cpList) && (listingType == 2 || listingType == 6)) {
            alert(`The listing type selected, ${listingTypeName}, is not supported (must contain a Bid element)`);
            return false;
        }

        if (aeList && (listingType == 1 || listingType == 4 || listingType == 110)) {
            alert(`The listing type selected, ${listingTypeName}, is not supported by Auction Edge (must contain a Buy element)`);
            return false;
        }

        if (ddList && (listingType == 2 || listingType == 6 || listingType == 110)) {
            alert(`The listing type selected, ${listingTypeName}, is not supported by eDealer Direct`);
            return false;
        }

        if (iaaList && (listingType == 1 || listingType == 4 || listingType == 110)) {
            alert(`The listing type selected, ${listingTypeName}, is not supported by IAA (must contain a Bid element)`);
            return false;
        }

        if (document.getElementById("MainContent_OpenLaneIsDealerAccount").value == "True"
            && checkAuction($('input[id^="ADESACheck"]')) == true) {
            var popup = confirm("Credentials for this vehicles are configured as an ADESA Dealer Account. The Start Price will be set equal to the Reserve Price for ADESA listings.\rClick OK to proceed otherwise, click Cancel");
            if (!popup)
                return false;
        }

        if (document.getElementById("MainContent_OVEIsDealerAccount").value == "True"
            && checkAuction($('input[id^="OVECheck"]'))) {
            var popup = confirm("Credentials for this vehicle are configured as an OVE Dealer Account. The Start Price will be set to within 16 Bid Increments of the Reserve Price for OVE listings.\rClick OK to proceed otherwise, click Cancel");
            if (!popup)
                return false;
        }

        if (document.getElementById('StartDate').value == "") {
            alert("A start date must be entered")
            return false;
        }

        if (document.getElementById('EndDate').value == "") {
            alert("An end date must be entered")
            return false;
        }

        // We have to convert the given dates since they typically can't be parse by Date.Parse
        var startDateParts = document.getElementById('StartDate').value.split("-");
        var endDateParts = document.getElementById('EndDate').value.split("-");

        var startDate = new Date(parseInt(startDateParts[0], 10), parseInt(startDateParts[1], 10) - 1, parseInt(startDateParts[2], 10));
        startDate.setHours(0, 0, 0);
        startDate = Date.parse(startDate);

        var endDate = new Date(parseInt(endDateParts[0], 10), parseInt(endDateParts[1], 10) - 1, parseInt(endDateParts[2], 10));
        endDate.setHours(0, 0, 0);
        endDate = Date.parse(endDate);

        // Get Today's Date
        var today = new Date();
        var todayHours = today.getHours();
        var todayMinutes = today.getMinutes();
        today.setHours(0, 0, 0);
        today = Date.parse(today);

        if (startDate < today) {
            alert("The Start Date must be Today or in the future");
            return false;
        }

        if (endDate < startDate) {
            alert("The End Date must be later than the Start Date");
            return false;
        }

        if (iaaList != null && iaaList == true && startDate == today && startDate == endDate) {
            var popup = confirm("IAA listings cannot be started during the current day. No IAA listing will be posted as the selected End Date is also set for today.\rClick OK to proceed otherwise click Cancel");
            if (!popup)
                return false;
        }

        if (oveList != null && oveList == true && startDate == today && startDate == endDate && ((todayHours == 13 && todayMinutes > 29) || todayHours > 13)) {
            var popup = confirm("OVE one day listings for the current day cannot be started after 1:30PM. No OVE listing will be posted.\rClick OK to proceed otherwise click Cancel");
            if (!popup)
                return false;
        }

        // Validate Starting Bid Price is greater than 0 if BID, BID/BUY, BID/OFFER, BID/BUY/OFFER
        if (startPrice == 0 && (listingType == 1 || listingType == 3 || listingType == 4 || listingType == 5 || listingType == 200)
            && (olList || oveList || taList || cpList)) {
            alert(`The Starting Price must be greater than 0 for ${listingTypeName}`);
            return false;
        }
        if (startPrice == 0 && listingType == 100 && (olList || taList)) {
            alert(`The Starting Price must be greater than 0 for ${listingTypeName}`);
            return false;
        }
        if (startPrice == 0 && ddList && (listingType == 1 || listingType == 3)) {
            alert(`The Starting Price must be greater than 0 for ${listingTypeName}`);
            return false;
        }

        // Validate Floor Price is greater than Starting Price if BID, BID/BUY, BID/OFFER, BID/BUY/OFFER
        if ((listingType == 1 || listingType == 3 || listingType == 4 || listingType == 5 || listingType == 200) && reservePrice < startPrice
            && (olList || oveList || taList || cpList)) {
            alert(`The Reserve Price must be greater than or equal to the Starting Price for ${listingTypeName} Listing Type`);
            return false;
        }
        if (listingType == 100 && reservePrice < startPrice && (olList || taList)) {
            alert(`The Reserve Price must be greater than or equal to the Starting Price for ${listingTypeName} Listing Type`);
            return false;
        }

        // Validate Floor Price is less than Buy It Now Price if BID/BUY or BID/BUY/OFFER
        if ((listingType == 3 || listingType == 5 || listingType == 200) && reservePrice > binPrice && (olList || saList || oveList || aeList || acvList || ddList)) {
            alert(`The Reserve Price must be less than Buy It Now Price for ${listingTypeName} Listing Type`);
            return false;
        }
        if (reservePrice > binPrice && listingType == 100 && (saList || olList || taList)) {
            alert(`The Reserve Price must be less than Buy It Now Price for ${listingTypeName} Listing Type`);
            return false;
        }

        // Validate Buy It Now Price is greater than zero if BID/BUY, BID/BUY/OFFER, or BUY/OFFER
        if ((listingType == 3 || listingType == 5 || listingType == 6 || listingType == 100 || listingType == 200) && binPrice == 0) {
            alert(`The Buy It Now Price is a required field for ${listingTypeName} Listing Type`);
            return false;
        }
        if (listingType == 120 && binPrice == 0 && (saList || olList || aeList || acvList)) {
            alert(`The Buy It Now Price is a required field for ${listingTypeName} Listing Type`);
            return false;
        }

        // Validate Buy It Now Price is 2 Bid Increments higher than Starting Price if BID/BUY, BID/BUY/OFFER for OVE only
        var lstBidIncrement = document.getElementById("MainContent_lstBidIncrement");
        var bidIncrement = parseInt(lstBidIncrement.options[lstBidIncrement.selectedIndex].value);
        if (oveList && (listingType == 3 || listingType == 5 || listingType == 200) && (binPrice < (startPrice + (bidIncrement * 2)))) {
            alert(`The Buy It Now Price must be 2 Bid Increments greater than the Start Price for ${listingTypeName} Listing Type on OVE`);
            return false;
        }

        // OVE Offer Only requires Reserve / Floor
        if ((listingType == 110 || listingType == 120) && oveList && reservePrice == 0) {
            alert(`The Reserve/Floor Price is a required field for ${listingTypeName} Listing Type`);
            return false;
        }

        // OVE Bid/Offer requires Start = Floor
        if ((listingType == 4 || listingType == 5) && oveList == 1 && startPrice != reservePrice) {
            alert(`The Start Price must be equal to the Floor Price for OVE Bid/Offer configurations`);
            return false;
        }
        if ((listingType == 1 || listingType == 3 || listingType == 4 || listingType == 5 || listingType == 100 || listingType == 200)
            && saList && reservePrice < 100) {
            alert(`The Reserve Price must be in increments of $100 for SmartAuction`);
            return false;
        }

        // Determine Max MMR Threshold per auctions selected
        var mmrValue = parseInt(document.getElementById("MainContent_MMRValue").innerHTML);
        var maxMMRPct = 0;
        for (var i = 0; i < auctionChecks.length; i++) {
            var auctionMMRValue = parseInt(document.getElementById('MaxMMRPct_' + auctionChecks[i].id.replace('CheckStart', '')).innerHTML);
            if (maxMMRPct == 0)
                maxMMRPct = auctionMMRValue;
            else if (auctionMMRValue < maxMMRPct)
                maxMMRPct = auctionMMRValue;
        }

        var maxThreshold = (mmrValue * maxMMRPct) / 100;
        var minThreshold = (mmrValue * 80) / 100;
        if (saList == true) {
            if ((mmrValue > 0 && listingType != 7)
                && (mmrValue > 15000 && mmrValue < 75000)) {
                // Check Reserve Price if available
                if (minThreshold > reservePrice) {
                    var popup = confirm(`The Reserve Price is currently set lower than the Minimum MMR Threshold ($${minThreshold}).\rClick OK to proceed otherwise, click Cancel`);
                    if (!popup)
                        return false;
                } else if (maxThreshold < reservePrice) {
                    alert(`The Reserve Price is currently set higher than the Maximum MMR Threshold ($${maxThreshold}).`);
                    return false;
                }

                // Check BIN Price if available
                if (minThreshold > binPrice) {
                    var popup = confirm(`The Buy It Now Price is currently set lower than the Minimum MMR Threshold ($${minThreshold}).\rClick OK to proceed otherwise, click Cancel`);
                    if (!popup)
                        return false;
                } else if (maxThreshold < binPrice) {
                    alert(`The Buy It Now Price is currently set higher than the Maximum MMR Threshold ($${maxThreshold}).`);
                    return false;
                }
            }
        }

        // Uncomment for debugging previous section
        //alert('Don't forget to comment me out!');
        //return false;

        // After all checks and balances, we submit vehicle to list
        SubmitAuctions();
    }
    else
        alert("You either haven't selected any auctions to send to or do not have any enabled!");
}

function SubmitAuctions() {
    var auctionChecks = document.querySelectorAll('[id*="CheckStart"]:checked');
    var kListing = document.getElementById("MainContent_CurrenkListing").value;
    var lotLocation = document.getElementById("MainContent_LotLocation").value;
    var state = document.getElementById("MainContent_lstState");
    var listingCat = document.getElementById("MainContent_lstListingCategory");
    var listingType = document.getElementById("MainContent_lstListingType");
    var vehicleLocation = document.getElementById("MainContent_lstLocation");
    var bidIncrement = document.getElementById("MainContent_lstBidIncrement");
    var inspCompany = document.getElementById("MainContent_lstInspectionCompany");

    var auctions = [];
    auctionChecks.forEach(auction => {
        var auctionInfo = {
            kWholesaleAuction: auction.value,
            RemoveFromAuction: 0,
            kWholesaleListingType: listingType.options[listingType.selectedIndex].value,
            kWholesaleLocationIndicator: vehicleLocation.options[vehicleLocation.selectedIndex].value,
            kWholesaleLocationCode: "",
            kWholesaleFacilitatedAuctionCode: "",
            kWholesaleBidIncrement: bidIncrement.options[bidIncrement.selectedIndex].value,
            StartPrice: document.getElementById("MainContent_StartPrice").value,
            FloorPrice: document.getElementById("MainContent_ReservePrice").value,
            BuyNowPrice: document.getElementById("MainContent_BINPrice").value,
            SellerID: "",
            BuyerGroup: "",
            StartDate: document.getElementById("StartDate").value,
            EndDate: document.getElementById("EndDate").value,
            EmailAddress: "",
            ContactPerson: document.getElementById("MainContent_contactName").value,
            VehicleLocationAddress: document.getElementById("MainContent_addressStreet").value,
            VehicleLocationCity: document.getElementById("MainContent_addressCity").value,
            VehicleLocationState: state.options[state.selectedIndex].value,
            VehicleLocationZIP: document.getElementById("MainContent_addressZip").value,
            VehicleLocationPhone: document.getElementById("MainContent_contactPhone").value,
            VehicleLocationFAX: "", // default empty string
            TransitEstArrDate: document.getElementById("arrivalDate").value,
            RequestInspection: document.getElementById("RequestInsp").checked ? 1 : 0,
            kWholesaleListingCategory: listingCat.options[listingCat.selectedIndex].value,
            kAASale: "",
            kWholesaleInspectionCompany: document.getElementById("RequestInsp").checked ? inspCompany.options[inspCompany.selectedIndex].value : "",
            ServiceProviderName: "",
            ServiceProviderID: "",
            CarGroupID: "",
            ForceWholesalePricing: document.getElementById("MainContent_chkForceWholesalePrice").checked ? 1 : 0,
            LimitedArbitrationPowertrainPledge: 0,
            RelistCount: document.getElementById("MainContent_RelistCount").value
        };

        auctions.push(auctionInfo);
    });

    var additionalInfo = {
        kListing: kListing,
        lotLocation: escapeApostrophe(lotLocation),
        isOVEDealerAccount: document.getElementById("MainContent_OVEIsDealerAccount").value,
        isOLDealerAccount: document.getElementById("MainContent_OpenLaneIsDealerAccount").value,
        MMRGoodPrice: document.getElementById("MainContent_MMRValue").value,
    };

    $.ajax({
        type: "POST",
        url: 'StartWholesale.aspx/SubmitToListMultiAuction',
        data: "{'lstAuctions': '" + JSON.stringify(auctions) + "', 'additionalInfo': '" + JSON.stringify(additionalInfo) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = JSON.parse(response.responseJSON.d);
            if (r.success == "1") {
                alert("Successfully submitted!");
                window.location.href = `/WholesaleContent/VehicleManagement.aspx`;
            }
            else
                alert(`Please address the following error messages: ${r.errormsgs}`);

            return false;
        }
    });
}

// Show Inspection Company if Request Inspection was checked
function showInspCompany() {
    var checked = document.getElementById("RequestInsp").checked;

    document.getElementById("MainContent_lstInspectionCompany").style.display = checked ? "inline-block" : "none";
}

function chkForcePricing() {
    var setStart = document.getElementById("MainContent_StartPrice");
    var setReserve = document.getElementById("MainContent_ReservePrice");
    var setBIN = document.getElementById("MainContent_BINPrice");
    var btnStart = document.getElementById("MainContent_setStart");
    var btnReserve = document.getElementById("MainContent_setReserve");
    var btnBIN = document.getElementById("MainContent_setBIN");
    var chkForce = document.getElementById("MainContent_chkForceWholesalePrice").checked;

    if (chkForce) {
        btnStart.style.opacity = btnReserve.style.opacity = btnBIN.style.opacity = "10%";
        btnStart.disabled = btnReserve.disabled = btnBIN.disabled = true;
        setStart.disabled = setReserve.disabled = setBIN.disabled = true;
    }
    else {
        btnStart.style.opacity = btnReserve.style.opacity = btnBIN.style.opacity = "100%";
        btnStart.disabled = btnReserve.disabled = btnBIN.disabled = false;
        setStart.disabled = setReserve.disabled = setBIN.disabled = false;
    }
}

function ChangeSelectedAuctions() {
    var listingType = document.getElementById("MainContent_lstListingType");
    var type = listingType.options[listingType.selectedIndex].value;
    var list = [];

    var btnStart = document.getElementById("MainContent_setStart");
    var btnReserve = document.getElementById("MainContent_setReserve");
    var btnBIN = document.getElementById("MainContent_setBIN");
    var credDisclaimer = document.getElementById("credDisclaimer1");

    // Gather all available auction options that aren't listed
    $('input[id*="Check"][isListed!="true"]').removeAttr("disabled");

    var adesaChk = checkAuction($('input[id^="ADESACheck"]'));
    var saChk = checkAuction($('input[id^="SmartAuctionCheck"]'));
    var oveChk = checkAuction($('input[id^="OVECheck"]'));
    var copartChk = checkAuction($('input[id^="COPARTCheck"]'));
    var aeChk = checkAuction($('input[id^="AuctionEdgeCheck"]'));
    var acvChk = checkAuction($('input[id^="ACVAuctionsCheck"]'));
    var eDDChk = checkAuction($('input[id^="eDealerDirectCheck"]'));
    var iaaChk = checkAuction($('input[id^="IAACheck"]'));
    var asChk = checkAuction($('input[id^="AuctionSimplifiedCheck"]'));
    var iasChk = checkAuction($('input[id^="IASCheck"]'));
    var auctionOSChk = checkAuction($('input[id^="AuctionOSCheck"]'));

    var auctionList = $('input[id*="Check"]:checked');
    if (auctionList.length > 0)
        credDisclaimer.style.display = "block";
    else
        credDisclaimer.style.display = "none";

    if (oveChk.checked)
        document.getElementById("MainContent_lstBidIncrement").disabled = false;
    else
        document.getElementById("MainContent_lstBidIncrement").disabled = true;

    if (type == 1) { // Bid
        // Buttons
        btnReserve.removeAttribute('disabled');
        btnReserve.style.opacity = '1';

        btnBIN.setAttribute('disabled', 'disabled');
        btnBIN.style.opacity = '0.1';

        if (oveChk == false && adesaChk == false && acvChk == false && aeChk == false
            && eDDChk == false && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr('disabled', 'disabled');
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr('disabled');
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }

        // Text Boxes
        $("#MainContent_ReservePrice").removeAttr('disabled');
        $("#MainContent_BINPrice").attr('disabled', 'disabled');

        // Check Boxes
        list = ["AuctionEdgeCheck", "IAACheck"];
    } else if (type == 2) { // Buy
        // Buttons
        btnStart.setAttribute('disabled', 'disabled');
        btnStart.style.opacity = '0.1';

        btnReserve.setAttribute('disabled', 'disabled');
        btnReserve.style.opacity = '0.1';

        btnBIN.removeAttribute('disabled');
        btnBIN.style.opacity = '1';

        // Text Boxes
        $("#MainContent_StartPrice").attr("disabled", "disabled");
        $("#MainContent_ReservePrice").attr("disabled", "disabled");
        $("#MainContent_BINPrice").removeAttr("disabled");

        // Check Boxes
        list = ["SmartAuctionCheck", "COPARTCheck", "eDealer DirectCheck", "AuctionOSCheck"];
    } else if (type == 3) { // Bid/Buy
        // Buttons
        btnReserve.removeAttribute('disabled');
        btnReserve.style.opacity = '1';

        btnBIN.removeAttribute('disabled');
        btnBIN.style.opacity = '1';

        if (oveChk == false && adesaChk == false && acvChk == false && aeChk == false
            && eDDChk == false && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }

        // Text Boxes
        $("#MainContent_StartPrice").removeAttr("disabled");
        $("#MainContent_ReservePrice").removeAttr("disabled");

        // Check Boxes
        list = [ "AuctionOSCheck" ];
    } else if (type == 4) { // Bid/Offer
        // Buttons
        btnReserve.removeAttribute("disabled");
        btnReserve.style.opacity = '1';

        btnBIN.setAttribute('disabled', 'disabled');
        btnBIN.style.opacity = '0.1';

        if (oveChk == false && adesaChk == false && acvChk == false && aeChk == false
            && eDDChk == false && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }

        // Text Boxes
        $("#MainContent_ReservePrice").removeAttr("disabled");
        $("#MainContent_BINPrice").attr("disabled", "disabled");

        // Check Boxes
        list = ["COPARTCheck", "AuctionEdgeCheck", "IAACheck"];
    } else if (type == 5) { // Bid/Buy/Offer OVE Bid/Buy
        // Buttons
        btnReserve.removeAttribute('disabled');
        btnReserve.style.opacity = '1';

        btnBIN.removeAttribute('disabled');
        btnBIN.style.opacity = '1';

        if (adesaChk == false && oveChk == false && acvChk == false && aeChk == false // TurnAuctions?
            && eDDChk == false && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }

        // Text Boxes
        $("#MainContent_ReservePrice").removeAttr("disabled");
        $("#MainContent_BINPrice").removeAttr("disabled");
    } else if (type == 6) { // Buy/Offer
        // Buttons
        btnStart.setAttribute("disabled", "disabled");
        btnStart.style.opacity = '0.1';

        btnReserve.setAttribute("disabled", "disabled");
        btnReserve.style.opacity = '0.1';

        btnBIN.removeAttribute('disabled');
        btnBIN.style.opacity = '1';

        // Text Boxes
        $("#MainContent_StartPrice").attr("disabled", "disabled");
        $("#MainContent_ReservePrice").attr("disabled", "disabled");
        $("#MainContent_BINPrice").removeAttr("disabled");

        // Check Boxes
        list = ["SmartAuctionCheck", "eDealer DirectCheck", "COPARTCheck"];
    } else if (type == 7) { // Offer Only?
        // Buttons
        btnStart.setAttribute('disabled', 'disabled');
        btnStart.style.opacity = '0.1';

        btnReserve.setAttribute('disabled', 'disabled');
        btnReserve.style.opacity = '0.1';

        btnBIN.setAttribute('disabled', 'disabled');
        btnBIN.style.opacity = '0.1';

        // Text Boxes
        $("#MainContent_StartPrice").attr("disabled", "disabled");
        $("#MainContent_ReservePrice").attr("disabled", "disabled");
        $("#MainContent_BINPrice").attr("disabled", "disabled");

        // Check Boxes
        list = ["ADESACheck", "SmartAuctionCheck", "COPARTCheck", "AuctionEdgeCheck", "ACVAuctionsCheck", "eDealerDirectCheck",
            "IAACheck", "AuctionSimplifiedCheck", "AuctionOSCheck"];
    } else if (type == 100) { // Bid/Buy/Offer OVE Buy/Offer
        // Buttons
        btnReserve.removeAttribute('disabled');
        btnReserve.style.opacity = '1';

        btnBIN.removeAttribute('disabled');;
        btnBIN.style.opacity = '1';

        // Text Boxes
        $("#MainContent_BINPrice").removeAttr("disabled");
        $("#MainContent_ReservePrice").removeAttr("disabled");

        if (adesaChk == false && acvChk == false && aeChk == false && eDDChk == false// TurnAuctions?
            && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else if (adesaChk == false && saChk == false && acvChk == false && aeChk == false && eDDChk == false
            && copartChk == false && asChk == false && iasChk == false) {
            btnReserve.setAttribute('disabled', 'disabled');
            btnReserve.style.opacity = '0.1';

            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';

            $("#MainContent_ReservePrice").attr("disabled", "disabled");
            $("#MainContent_StartPrice").attr("disabled", "disabled");
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }
    } else if (type == 200) { // Bid/Buy/Offer OVE Bid/Buy
        // Buttons
        btnReserve.removeAttribute('disabled');
        btnReserve.style.opacity = '1';

        btnBIN.removeAttribute('disabled');;
        btnBIN.style.opacity = '1';

        // Text Boxes
        $("#MainContent_ReservePrice").removeAttr("disabled");
        $("#MainContent_BINPrice").removeAttr("disabled");

        if (adesaChk == false && oveChk == false && acvChk == false && aeChk == false && eDDChk == false // TurnAuctions?
            && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }
    } else if (type == 110) { // Bid/Offer OVE Offer Only
        if ((adesaChk == false && oveChk == false && acvChk == false && aeChk == false && eDDChk == false && copartChk == false && asChk == false) // TurnAuctions?
            || (adesaChk == false && saChk == false && acvChk == false && aeChk == false && eDDChk == false && copartChk == false && asChk == false && iasChk == false)) {
            $("#MainContent_StartPrice").attr("disabled", "disabled");
            btnStart.setAttribute('disabled', 'disabled');
            btnStart.style.opacity = '0.1';
        } else {
            $("#MainContent_StartPrice").removeAttr("disabled");
            btnStart.removeAttribute('disabled');
            btnStart.style.opacity = '1';
        }

        if (adesaChk == false && saChk == false && asChk == false) {// TurnAuctions?
            $("#MainContent_ReservePrice").attr("disabled", "disabled");
            btnReserve.setAttribute('disabled', 'disabled');
            btnReserve.style.opacity = '0.1';
        } else {
            $("#MainContent_ReservePrice").removeAttr("disabled");
            btnReserve.removeAttribute('disabled');
            btnReserve.style.opacity = '1';
        }

        // Buttons
        $("#MainContent_BINPrice").attr("disabled", "disabled");
        btnBIN.setAttribute('disabled', 'disabled');
        btnBIN.style.opacity = '0.1';
    } else if (type == 120) { // Buy/Offer OVE Offer Only
        $("#MainContent_ReservePrice").attr("disabled", "disabled");
        btnReserve.setAttribute('disabled', 'disabled');
        btnReserve.style.opacity = '0.1';

        $("#MainContent_StartPrice").attr("disabled", "disabled");
        btnStart.setAttribute('disabled', 'disabled');
        btnStart.style.opacity = '0.1';

        if (adesaChk == false && saChk == false && aeChk == false && acvChk == false
            && eDDChk == false && copartChk == false && asChk == false && iasChk == false) {
            $("#MainContent_BINPrice").attr("disabled", "disabled");
            btnBIN.setAttribute('disabled', 'disabled');
            btnBIN.style.opacity = '0.1';
        } else {
            $("#MainContent_BINPrice").removeAttr("disabled");
            btnBIN.removeAttribute('disabled');
            btnBIN.style.opacity = '1';
        }
    }

    // Global function for disabling Check Boxes
    list.forEach(auction => {
        $(`input[id^="${auction}"]`).prop("checked", false);
        $(`input[id^="${auction}"]`).attr("disabled", "disabled");
    });

    adesaOnChange();
    OnChangeBid();
}

function showHideSection(section, legend) {
    var item = document.getElementById(legend);
    var sectionItem = document.getElementById(section);
    item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + ((!sectionItem.classList.contains('openVehicleLocationInfo')) ? "&#9650;" : "&#9660;");
    sectionItem.classList.toggle('openVehicleLocationInfo');
}

function checkAuction(auction) {
    var chk = false;
    chk = (typeof auction[0] === "undefined") ? false : auction[0].checked;
    return chk;
}

function escapeApostrophe(item) {
    var nitem = item.replace("'", "&apos;");
    return nitem;
}

$(document).ready(function () {
    ChangeSelectedAuctions();
    adesaOnChange();
    OnChangeBid();
});