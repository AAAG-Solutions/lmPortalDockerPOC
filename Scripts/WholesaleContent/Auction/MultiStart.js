function SetPrice(priceField) {
    var percent = parseInt(document.getElementById("percent").value);
    var dollar = parseInt(document.getElementById("dollar").value);

    // Gather data from jsGrid to loop through and update given priceField
    var gridData = $("#jsGrid").data("JSGrid").data;

    gridData.forEach(function (item) {
        var vehicleMMR = parseInt(item.MMRGoodPrice);

        if (!(isNaN(vehicleMMR)) && vehicleMMR != 0) {
            var mmrCalc = (vehicleMMR * (percent / 100)) + dollar;
            document.getElementById(`${priceField}_${item.kListing}`).value = mmrCalc;
        }
    });
}

function chkForcePricing() {
    var setStart = document.getElementById("MainContent_setStart");
    var setReserve = document.getElementById("MainContent_setReserve");
    var setBIN = document.getElementById("MainContent_setBIN");
    var chkForce = document.getElementById("MainContent_chkForceWholesalePrice").checked;

    if (chkForce) {
        setStart.style.opacity = setReserve.style.opacity = setBIN.style.opacity = "10%";
        setStart.disabled = setReserve.disabled = setBIN.disabled = true;
        $("input[id^='startprice_']").attr('disabled', 'disabled');
        $("input[id^='reserveprice_']").attr('disabled', 'disabled');
        $("input[id^='binprice_']").attr('disabled', 'disabled');
    }
    //else {
    //    setStart.style.opacity = setReserve.style.opacity = setBIN.style.opacity = "100%";
    //    $("input[id^='startprice_']").removeAttr('disabled');
    //    $("input[id^='reserveprice_']").removeAttr('disabled');
    //    $("input[id^='binprice_']").removeAttr('disabled');
    //}
}

function ListingTypeChange() {
    var kListingType = document.getElementById("MainContent_lstType");
    var type = kListingType.options[kListingType.selectedIndex].value;
    var list = [];

    // Gather all available auction options that aren't listed
    $('input[id^="ADESACheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="SmartAuctionCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="OVECheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="COPARTCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="AuctionEdgeCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="ACVAuctionsCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="eDealerDirectCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="IAACheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="AuctionSimplifiedCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="IASCheck"][isListed!="true"]').removeAttr("disabled");
    $('input[id^="CarmigoCheck"][isListed!="true"]').removeAttr("disabled");

    var setStart = document.getElementById("MainContent_setStart");
    var setReserve = document.getElementById("MainContent_setReserve");
    var setBIN = document.getElementById("MainContent_setBIN");

    if (type == 1) { // Bid
        // Buttons
        setStart.removeAttribute('disabled');
        setReserve.removeAttribute('disabled');
        setBIN.setAttribute('disabled','disabled');

        // Text Boxes
        $("input[id^='startprice_']").removeAttr('disabled');
        $("input[id^='reserveprice_']").removeAttr('disabled');
        $("input[id^='binprice_']").attr('disabled', 'disabled');

        // Check Boxes
        list = ["AuctionEdgeCheck", "IAACheck"];
    } else if (type == 2) { // Buy
        // Buttons
        setStart.setAttribute('disabled', 'disabled');
        setReserve.setAttribute('disabled', 'disabled');
        setBIN.removeAttribute('disabled');

        // Text Boxes
        $("input[id^='startprice_']").attr("disabled", "disabled");
        $("input[id^='reserveprice_']").attr("disabled", "disabled");
        $("input[id^='binprice_']").removeAttr("disabled");

        // Check Boxes
        list = ["SmartAuctionCheck", "COPARTCheck", "eDealerDirectCheck", "AuctionOS"];
    } else if (type == 3) { // Bid/Buy
        // Buttons
        setStart.removeAttribute('disabled');
        setReserve.removeAttribute('disabled');
        setBIN.removeAttribute('disabled');

        // Text Boxes
        $("input[id^='startprice_']").removeAttr("disabled");
        $("input[id^='reserveprice_']").removeAttr("disabled");
        $("input[id^='binprice_']").removeAttr("disabled");

        list = ["AuctionOS"];
    } else if (type == 4 || type == 110) { // Bid/Offer
        // Buttons
        setStart.removeAttribute("disabled");
        setReserve.removeAttribute("disabled");
        setBIN.setAttribute('disabled', 'disabled');

        // Text Boxes
        $("input[id^='startprice_']").removeAttr("disabled");
        $("input[id^='reserveprice_']").removeAttr("disabled");
        $("input[id^='binprice_']").attr("disabled", "disabled");

        // Check Boxes
        list = ["COPARTCheck", "AuctionEdgeCheck", "IAACheck"];
    } else if (type == 5 || type == 100 || type == 200) { // Bid/Buy/Offer
        // Buttons
        setStart.removeAttribute('disabled');
        setReserve.removeAttribute('disabled');
        setBIN.removeAttribute('disabled');

        // Text Boxes
        $("input[id^='startprice_']").removeAttr("disabled");
        $("input[id^='reserveprice_']").removeAttr("disabled");
        $("input[id^='binprice_']").removeAttr("disabled");
    } else if (type == 6 || type == 120) { // Buy/Offer
        // Buttons
        setStart.setAttribute("disabled", "disabled");
        setReserve.setAttribute("disabled", "disabled");
        setBIN.removeAttribute('disabled');

        // Text Boxes
        $("input[id^='startprice_']").attr("disabled", "disabled");
        $("input[id^='reserveprice_']").attr("disabled", "disabled");
        $("input[id^='binprice_']").removeAttr("disabled");

        // Check Boxes
        list = ["SmartAuctionCheck", "eDealerDirectCheck", "COPARTCheck"];
    } else if (type == 7) { // Offer Only
        // Buttons
        setStart.setAttribute('disabled', 'disabled');
        setReserve.setAttribute('disabled', 'disabled');
        setBIN.setAttribute('disabled', 'disabled');

        // Text Boxes
        $("input[id^='startprice_']").attr("disabled", "disabled");
        $("input[id^='reserveprice_']").attr("disabled", "disabled");
        $("input[id^='binprice_']").attr("disabled", "disabled");

        // Check Boxes
        list = ["ADESACheck", "SmartAuctionCheck", "COPARTCheck", "AuctionEdgeCheck", "ACVAuctionsCheck", "eDealerDirectCheck",
            "IAACheck", "AuctionSimplifiedCheck", "AuctionOS", "CarmigoCheck"];
    }

    // Global function for disabling Check Boxes
    list.forEach(auction => {
        $(`input[id^="${auction}"]`).prop("checked", false);
        $(`input[id^="${auction}"]`).attr("disabled", "disabled");
    });

    chkForcePricing();
}

function CheckAuction(selectedItem) {
    var select = selectedItem.split("!");

    if (select.length == 1 && select[0] == 1) {
        var list = $('input[id*="Check_"][isListed!="true"]');

        list.each(function (item) {
            if (list[item].disabled) {
                return;
            } else {
                list[item].checked = true;
            }
        });
    } else if (select.length == 1 && select[0] == 2) {
        var list = $('input[id*="Check_"][isListed!="true"]');

        list.each(function (item) {
            list[item].checked = false;
        });
    } else {
        var list = [];
        if (select == "0")
            list = $(`input[id*="Check_"][isListed!="true"]`);
        else
            list = $(`input[id^="${select[1]}Check_"][isListed!="true"]`);
        var bool = parseInt(select[0]) == 1 ? true : false;

        list.each(function (item) {
            if (list[item].disabled) {
                return;
            } else {
                list[item].checked = bool;
            }
        });
    }
}

function VINSearch() {
    // Gather data from jsGrid to loop through and find specified VIN
    var gridData = $("#jsGrid").data("JSGrid").data;
    var vin = document.getElementById("VinSearch").value.toUpperCase();
    var reg = new RegExp(vin);
    var nextTargetRow = gridData[0];

    if (vin == "") {
        return false;
    }

    for (let i = 0; i < gridData.length; i++) {
        if (gridData[i].Vin == vin || reg.exec(gridData[i].Vin)) {
            if (i + 1 < gridData.length) {
                nextTargetRow = $("#startprice_" + gridData[i + 1].kListing)[0];
            }
            else {
                nextTargetRow = $("#startprice_" + gridData[i].kListing)[0];
            }
            nextTargetRow.scrollIntoView(false);
            $("#jsGrid").data("JSGrid").rowClick({ event: { target: $("#startprice_" + gridData[i].kListing)[0] }, item: gridData[i] });
            $("#jsGrid")[0].children[0].scrollIntoView(false);
            break;
        }
    }
}

function SubmitToAuctions() {
    var gridData = $("#jsGrid").data("JSGrid").data;
    var checkBoxes = $('input[id*="Check_"][isListed!="true"]:checked').toArray();
    var kListingTypeList = document.getElementById('MainContent_lstType');
    var listingCatList = document.getElementById('MainContent_lstCategory');

    var auctionsInfo = [];
    var isOLIsDealerAccount = 0;
    var isOVEIsDealerAccount = 0;
    var reg;

    if (checkBoxes.length != 0) {
        gridData.forEach(function (item) {
            reg = new RegExp(item.kListing + '$');

            checkBoxes.forEach(function (auction) {
                if (reg.exec(auction.id)) {
                    var auctionInfo = {
                        kWholesaleAuction: parseInt(auction.value),
                        kListing: parseInt(item.kListing),
                        StartPrice: parseInt(document.getElementById("startprice_" + item.kListing).value),
                        FloorPrice: parseInt(document.getElementById("reserveprice_" + item.kListing).value),
                        BuyNowPrice: parseInt(document.getElementById("binprice_" + item.kListing).value),
                        RelistCount: parseInt(document.getElementById("MainContent_RelistCount").value),
                        kWholesaleListingType: parseInt(kListingTypeList.options[kListingTypeList.selectedIndex].value),
                        kWholesaleListingCategory: parseInt(listingCatList.options[listingCatList.selectedIndex].value),
                        StartDate: document.getElementById("StartDate").value,
                        EndDate: document.getElementById("EndDate").value,
                        CarGroupID: auction.value == "4" ? item.CarGroupID : "",
                        ForceWholesalePricing: document.getElementById("MainContent_chkForceWholesalePrice").checked ? 1 : 0,
                        LimitedArbitrationPowertrainPledge: auction.value == "2" ? document.getElementById("MainContent_ArbPledge").checked ? 1 : 0 : 0,

                        // auction.kWholesaleListingTypes/Values needed for validation before listing
                        OLSuprressMMR: item.OVESuppressMMR,
                        OVESuppressMMR: item.OLSuprressMMR,
                        SASuppressMMR: item.SASuppressMMR,
                        HasMMRExclusion: item.HasMMRExclusion,
                        MMROverride: item.MMROverride,
                        MMRValue: item.MMRGoodPrice
                    };

                    if (item.OLIsDealerAccount == "1") {
                        if (((auctionInfo.kWholesaleListingType == 1) || (auctionInfo.kWholesaleListingType == 3) || (auctionInfo.kWholesaleListingType == 4)
                            || (auctionInfo.kWholesaleListingType == 5) || (auctionInfo.kWholesaleListingType == 100) || (auctionInfo.kWholesaleListingType == 200))
                            && auctionInfo.FloorPrice > auctionInfo.StartPrice) {
                            $('#startprice_' + auction.kListing).parent().parent().children().css('background-color', '#B0E2FF');
                            isOLIsDealerAccount = 1;
                        }
                    }

                    if (item.OVEIsDealerAccount == "1") {
                        if (((auctionInfo.kWholesaleListingType == 1) || (auctionInfo.kWholesaleListingType == 3) ||
                            (auctionInfo.kWholesaleListingType == 4) || (auctionInfo.kWholesaleListingType == 5) || (auctionInfo.kWholesaleListingType == 200))
                            && auctionInfo.FloorPrice > (auctionInfo.StartPrice + (parseInt(document.getElementById("MainContent_BidIncrement").value) * 16))) {
                            $('#startprice_' + auction.kListing).parent().parent().children().css('background-color', '#B0E2FF');
                            isOVEIsDealerAccount = 1;
                        }
                    }

                    auctionsInfo.push(auctionInfo);
                }
            });
        });
        var returnValues = ValidateAuctionInfo(isOLIsDealerAccount, isOVEIsDealerAccount, auctionsInfo);
        // If we fail one check, we fail all attempts for other selected auctions
        // #TODO: Figure out if we should only fail to list on selective auctions and push other successful
        var isPass = returnValues[0];
        var validRecords = returnValues[1];
        if (!isPass) {
            alert("There are some issues with the selected auctions! Please review highlighted vehicles!");
        }

        if (validRecords.length == 0)
            return false;

        // Remove Un-needed keys
        validRecords.forEach(auctionInfo => {
            delete auctionInfo["OLSuprressMMR"];
            delete auctionInfo["OVESuppressMMR"];
            delete auctionInfo["SASuppressMMR"];
            delete auctionInfo["HasMMRExclusion"];
            delete auctionInfo["MMROverride"];
        });

        var additionalInfo = {
            isOLDealerAccount: isOLIsDealerAccount,
            isOVEDealerAccount: isOVEIsDealerAccount
        }

        if (validRecords.length > 0) {
            toggleLoading(true, "Submitting Multiple Vehicles to selected auctions...");

            $.ajax({
                type: "POST",
                url: 'MultiStart.aspx/SubmitToMultipleAuctions',
                data: `{'info': '${JSON.stringify(validRecords)}', 'additionalInfo': '${JSON.stringify(additionalInfo)}'}`,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(XMLHttpRequest.responseJSON.Message);
                },
                complete: function (response) {
                    var r = response.responseJSON.d;
                    toggleLoading(false, "Submitting Multiple Vehicles to selected auctions...");

                    alert(r.message);
                    location.reload();

                    return false;
                }
            });
        }
    }
    else
        alert("You either haven't selected any auctions to send to or do not have any enabled!");
}

function ValidateAuctionInfo(isOL, isOVE, auctionsInfo) {
    var pass = true;
    var validRecords = [];

    if (isOL) {
        var response = confirm("Credentials for one or more vehicles are configured as an ADESA Dealer Account. The Start Price will be set equal to the Reserve Price for ADESA listings.\rClick OK to proceed otherwise click Cancel");
        if (!response)
            return false;
    }

    if (isOVE) {
        var response = confirm("Credentials for one or more vehicles are configured as an OVE Dealer Account. The Start Price will be set to within 16 Bid Increments of the Reserve Price for OVE listings.\rClick OK to proceed otherwise click Cancel");
        if (!response)
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

    // When attempting to list again, we need to clear the error messages just in case the same ones show up
    var errList = $("[id^='errmsg_']");
    errList.each(msg => { errList[msg].innerText = '' });

    auctionsInfo.forEach(auction => {
        // Used to place error messages in the appropriate row
        var startPrice = $('#startprice_' + auction.kListing);
        var MinMMRThreshold = auction.MMRValue * 0.80;
        var MaxMMRThreshold = auction.MMRValue * (parseFloat(document.getElementById(`MaxMMRPct_${auction.kWholesaleAuction}_${auction.kListing}`).innerHTML) / 100);

        // ADESA
        if (auction.kWholesaleAuction == 4) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'ADESA: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'ADESA: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 4) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 110)
                || (auction.kWholesaleListingType == 200)) && auction.StartPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'The Starting Price must be greater than 0 for this Listing Type for ADESA\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 4) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 110)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice < auction.StartPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ADESA: The Reserve Price must be greater than or Equal to the Starting Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 200))
                && auction.FloorPrice > auction.BuyNowPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ADESA: The Reserve Price must be less than the Buy It Now Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 200)
                || (auction.kWholesaleListingType == 6)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ADESA: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // OVE
        if (auction.kWholesaleAuction == 1) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 4) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 200)) && auction.StartPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Starting Price must be greater than 0 for this Listing Type for OVE\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if ((auction.kWholesaleListingType == 4 || auction.kWholesaleListingType == 5)
                && auction.StartPrice != auction.FloorPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Start Price must be equal to the Floor Price for OVE Bid/Offer configurations\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            } else if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 4) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice < auction.StartPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Reserve Price must be greater than or Equal to the Starting Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice > auction.BuyNowPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Reserve Price must be less than the Buy It Now Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 2) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 120) || (auction.kWholesaleListingType == 200)
                || (auction.kWholesaleListingType == 6)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'OVE: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // SmartAuction
        if (auction.kWholesaleAuction == 2) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 200))
                && auction.FloorPrice > auction.BuyNowPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: The Reserve Price must be less than the Buy It Now Price for this Listing Type\r\n';
                pass = false;
                startPrice.parent().parent().children().css('background-color', '#FF8080');
            }

            if (((auction.kWholesaleListingType == 2) || (auction.kWholesaleListingType == 3)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 120) || (auction.kWholesaleListingType == 200)
                || (auction.kWholesaleListingType == 6)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (auction.FloorPrice < 100 && ((auction.kWholesaleListingType == 1)
                || (auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 4)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 110) || (auction.kWholesaleListingType == 200))) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'SmartAuction: The Reserve Price must be in increments of $100 for SmartAuction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // AuctionEdge
        if (auction.kWholesaleAuction == 7) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'AuctionEdge: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'AuctionEdge: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'AuctionEdge: Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 2) || (auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5)
                || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 120) || (auction.kWholesaleListingType == 200)
                || (auction.kWholesaleListingType == 6)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'AuctionEdge: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // ACV Auctions
        if (auction.kWholesaleAuction == 11) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 4)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 110)
                || (auction.kWholesaleListingType == 200)) && auction.StartPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Starting Price must be greater than 0 for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 4)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 110)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice < auction.StartPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Reserve Price must be greater than or Equal to the Starting Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice > auction.BuyNowPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Reserve Price must be less than the Buy It Now Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (((auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 200) || (auction.kWholesaleListingType == 6)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'ACV Auctions: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // eDealer Direct
        if (auction.kWholesaleAuction == 12) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'eDealer Direct: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'eDealer Direct: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 2 || auction.kWholesaleListingType == 6 || auction.kWholesaleListingType == 120 || auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'eDealer Direct must include a Bid element in the Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if ((auction.kWholesaleListingType == 3 || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice > auction.BuyNowPrice) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'eDealer Direct: The Reserve Price must be less than the Buy It Now Price for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if ((auction.kWholesaleListingType == 3 || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100)
                || (auction.kWholesaleListingType == 200)) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'eDealer Direct: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // COPART
        if (auction.kWholesaleAuction == 6) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'COPART: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'COPART: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType == 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'COPART: Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
            if (auction.kWholesaleListingType == 2 || auction.kWholesaleListingType == 6) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'COPART must include a Bid element in the Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
            if (((auction.kWholesaleListingType == 1) || (auction.kWholesaleListingType == 3) || (auction.kWholesaleListingType == 4)
                || (auction.kWholesaleListingType == 5) || (auction.kWholesaleListingType == 100) || (auction.kWholesaleListingType == 110)
                || (auction.kWholesaleListingType == 200)) && auction.FloorPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'COPART: The Reserve Price must be greater than 0 for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // IAA
        if (auction.kWholesaleAuction == 13) {
            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'IAA: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'IAA: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.kWholesaleListingType != 7) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'IAA: Offer Only is not available for this auction\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if (auction.kWholesaleListingType == 1 || auction.kWholesaleListingType == 4 || auction.kWholesaleListingType == 110) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'IAA must include a Buy element in the Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }

            if ((auction.kWholesaleListingType == 2 || auction.kWholesaleListingType == 3 || auction.kWholesaleListingType == 5
                || auction.kWholesaleListingType == 100 || auction.kWholesaleListingType == 200 || auction.kWholesaleListingType == 6
                || auction.kWholesaleListingType == 120) && auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'IAA: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        // Carmigo
        if (auction.kWholesaleAuction == 17) {
            //if (auction.kWholesaleListingType != 6 && auction.kWholesaleListingType != 7) {
            //    $('#errmsg_' + auction.kListing)[0].innerText += 'Carmigo: Buy/Offer are only available for this auction\r\n';
            //    startPrice.parent().parent().children().css('background-color', '#FF8080');
            //    pass = false;
            //}

            if (auction.MMRValue > 15000 && auction.MMRValue < 75000) {
                if (auction.FloorPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'Carmigo: The Reserve Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (auction.BuyNowPrice > MaxMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'Carmigo: The Buy It Now Price is currently set higher than the Maximum MMR Threshold of $' + MaxMMRThreshold + '\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }

            if (auction.BuyNowPrice == 0) {
                $('#errmsg_' + auction.kListing)[0].innerText += 'Carmigo: The Buy It Now Price is a required field for this Listing Type\r\n';
                startPrice.parent().parent().children().css('background-color', '#FF8080');
                pass = false;
            }
        }

        if (auction.kWholesaleAuction == 4) {
            var suppressMMR = 0;
            var skipMMR = 0;

            if (auction.OVESuppressMMR == 1 || auction.OLSuprressMMR == 1 || auction.SASuppressMMR == 1 || auction.HasMMRExclusion == 1)
                suppressMMR = 1;

            if (((auction.kWholesaleListingType == 2 || auction.kWholesaleListingType == 3 || auction.kWholesaleListingType == 5 ||
                auction.kWholesaleListingType == 100 || auction.kWholesaleListingType == 6 || auction.kWholesaleListingType == 200) && auction.BuyNowPrice < 10000)
                || ((auction.kWholesaleListingType == 1 || auction.kWholesaleListingType == 4) && auction.FloorPrice < 10000))
                skipMMR = 1;

            if (suppressMMR == 0 && skipMMR == 0 && auction.MMROverride == 0) {
                if (MinMMRThreshold > 0 && (auction.kWholesaleListingType == 1 || auction.kWholesaleListingType == 3 ||
                    auction.kWholesaleListingType == 4 || auction.kWholesaleListingType == 5 || auction.kWholesaleListingType == 100 ||
                    auction.kWholesaleListingType == 110 || auction.kWholesaleListingType == 200) && auction.FloorPrice < MinMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'The Reserve Price is currently set lower than the Minimum MMR Threshold.\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
                if (MinMMRThreshold > 0 && (auction.kWholesaleListingType == 2 || auction.kWholesaleListingType == 6 ||
                    auction.kWholesaleListingType == 120) && auction.BuyNowPrice < MinMMRThreshold) {
                    $('#errmsg_' + auction.kListing)[0].innerText += 'The Buy It Now Price is currently set lower than the Minimum MMR Threshold.\r\n';
                    startPrice.parent().parent().children().css('background-color', '#FF8080');
                    pass = false;
                }
            }
        }

        if (pass)
            validRecords.push(auction);
    });

    // Return a true value if there are no errors or issues
    return [pass, validRecords];
}

function showHideSection(idName, idFieldSet) {
    var fieldSets = document.getElementsByClassName('sectionFieldset');
    var bodies = document.getElementsByClassName('collapseTable');
    for (var i = 0; i < fieldSets.length; i++) {
        var item = fieldSets[i].children[0];
        if (fieldSets[i].id == idFieldSet) {
            if (bodies[i].style.display == "none") {
                bodies[i].style.display = "table";
                item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + "&#9650;";
            }
            else if (bodies[i].style.display == "table") {
                bodies[i].style.display = "none";
                item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + "&#9660;";
            }
        }
        else {
            bodies[i].style.display = "none";
            item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + "&#9660;";
        }
    }
    
    if (screen.height > 600) {
        ResizeGrid();
    }
}

function ResizeGrid() {
    var offSet = 310;
    if (document.getElementById("divFilters").style.display != "none")
        offSet += 242;
    if (document.getElementById("divSettings").style.display != "none")
        offSet += 102;
    if (document.getElementById("divISQM").style.display != "none")
        offSet += 176;
    document.getElementById("grid").style.height = "calc(100vh - " + offSet + "px)";
    document.getElementsByClassName("jsgrid-grid-body")[0].style.height = "calc(100vh - " + (offSet + 65) + "px)";
}

function filterChanged() {
    var grid = $("#jsGrid").data("JSGrid");
    var gfilter = grid.getFilter();

    var filter = {
        InvStatusAvail: $('#MainContent_StatusAvailable')[0].checked ? "1" : "0",
        InvStatusInTransit: $('#MainContent_StatusInTransit')[0].checked ? "1" : "0",
        DealerCert: $('#MainContent_TypeDealerCertified')[0].checked ? "1" : "0",
        ManufacturerCert: $('#MainContent_TypeManufacturerCertified')[0].checked ? "1" : "0",
        PreOwned: $('#MainContent_TypePreOwned')[0].checked ? "1" : "0",
        LotLocation: $('#MainContent_lstLotLocation').val(),
        ListingStatus: $('#MainContent_lstListingStatus').val(),
        InspectionStatus: $('#MainContent_lstInspectionStatus').val()
    }

    gfilter["PageFilter"] = filter;
    grid.search(gfilter);
}

function fnFilterClear() {
    $('#MainContent_StatusAvailable')[0].checked = false;
    $('#MainContent_StatusInTransit')[0].checked = false;
    $('#MainContent_TypeDealerCertified')[0].checked = false;
    $('#MainContent_TypeManufacturerCertified')[0].checked = false;
    $('#MainContent_TypePreOwned')[0].checked = false;
    $('#MainContent_lstLotLocation')[0].selectedIndex = 0;
    $('#MainContent_lstListingStatus')[0].selectedIndex = 0;
    $('#MainContent_lstInspectionStatus')[0].selectedIndex = 0;
    filterChanged();
}

var wasSmall = false;
$(document).ready(function () {
    if (screen.height <= 600) {
        showHideSection('divSettings', 'WholesaleSetting');
        wasSmall = true;
    }

    addEventListener('resize', (event) => {
        var height = screen.height;
        if (height <= 600 && !wasSmall) {
            showHideSection('divSettings', 'WholesaleSetting');
            wasSmall = true;
        }
        if (height > 600 && wasSmall) {
            showHideSection('divSettings', 'WholesaleSetting');
            ResizeGrid();
            wasSmall = false;
        }
    });
});