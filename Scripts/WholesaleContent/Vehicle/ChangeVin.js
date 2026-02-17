function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input.value); }, delay);
    };
}

function HandleChangePause(a) {    
    VerifyVin();
}

function ButtonAction(action) {
    if (action == "submit") {
        if (document.getElementById("MainContent_hfCurrentVin").value == document.getElementById("MainContent_tbVinNumberNew").value || document.getElementById("MainContent_tbVinNumberNew").value == "") {
            alert("Vin Did not change. Either change VIN or close window.")
        }
        else if (confirm("If changing VIN causes a new vehicle configuration, listing data will be cleared. Are you sure ou want to continue?")) {

            toggleLoading(true, "");

            var stockTypeValue = document.getElementById("MainContent_hfStockType").value.replaceAll("\"", "\\\"");
            var Status = document.getElementById("MainContent_hfStatus").value.replaceAll("\"", "\\\"");

            if (stockTypeValue == "CERTIFIED PRE-OWNED")
                stockTypeValue = "32";
            else if (stockTypeValue == "PRE-OWNED")
                stockTypeValue = "30";
            else if (stockTypeValue == "New")
                stockTypeValue = "31";

            if (Status == "AVAILABLE")
                Status = "1";
            else if (Status == "UNAVAILABLE")
                Status = "2";
            else if (Status == "ON HOLD")
                Status = "3";
            else if (Status == "DEMO")
                Status = "4";
            else if (Status == "RETURNED")
                Status = "5";
            else if (Status == "SALE PENDING")
                Status = "6";
            else if (Status == "SOLD")
                Status = "7";
            else if (Status == "PENDING REVIEW")
                Status = "8";

            var dataIn = {
                kListing: document.getElementById("MainContent_hfkListing").value.replaceAll("\"", "\\\""),
                StockNumber: document.getElementById("MainContent_hfStockNumber").value.replaceAll("\"", "\\\""),
                Vin: document.getElementById("MainContent_tbVinNumberNew").value.replaceAll("\"", "\\\""),
                Miles: document.getElementById("MainContent_hfMiles").value.replaceAll("\"", "\\\""),
                Cost: document.getElementById("MainContent_hfCost").value.replaceAll("\"", "\\\""),
                List: document.getElementById("MainContent_hfListPrice").value.replaceAll("\"", "\\\""),
                Year: document.getElementById("MainContent_tbYearNew").value.replaceAll("\"", "\\\""),
                Make: document.getElementById("MainContent_tbMakeNew").value.replaceAll("\"", "\\\""),
                Model: document.getElementById("MainContent_tbModelNew").value.replaceAll("\"", "\\\""),
                Style: document.getElementById("MainContent_tbStyleNew").value.replaceAll("\"", "\\\""),
                StyleId: document.getElementById("MainContent_hfStyleIdNew").value.replaceAll("\"", "\\\""),
                DrillDown: document.getElementById("MainContent_hfDrillDown").value.replaceAll("\"", "\\\""),
                StockNumberValue: stockTypeValue,
                StatusValue: Status,
                ImportOverride: document.getElementById("MainContent_hfDMI").value.replaceAll("\"", "\\\"")
            }

            $.ajax({
                type: 'POST',
                url: 'ChangeVin.aspx/UpdateInventory',
                data: "{'jsonData': '" + JSON.stringify(dataIn) + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(XMLHttpRequest.responseJSON.Message);
                },
                complete: function (response) {
                    var r = response.responseJSON.d;
                    if (r == "Success") {
                        toggleLoading(false, "Changes saved. Redirecting to listing for this vehicle.");
                        var kListing = document.getElementById("MainContent_hfkListing").value;
                        window.location.href = `/WholesaleContent/Vehicle/update.aspx?kListing=${kListing}`;
                    }
                    else {
                        toggleLoading(false, "");
                        alert("Something has gone wrong in the save process");
                        ChangeCss("New");
                    }
                }
            });
        }
    }
    else if (action == "cancel") {
        history.back();
    }
    else if (action.includes("SetStyle")) {
        SetStyle(action.substring(action.indexOf("_") + 1));
        ChangeCss("New");
    }
}

function ChangeCss(name) {
    document.getElementById("NewHolder").style.display = "none";
    document.getElementById("OptionsTable").style.display = "none";
    document.getElementById("BaseHolder").style.display = "none";
    if (name == "Multi") {
        document.getElementById("NewHolder").style.display = "table";
        document.getElementById("OptionsTable").style.display = "table";
    }
    else if (name == "Base") {
        document.getElementById("BaseHolder").style.display = "table-row";
    }
    else if (name == "New") {
        document.getElementById("NewHolder").style.display = "table";
    }
}

function VerifyVin() {
    if (document.getElementById("MainContent_tbVinNumberNew").value == "")
        document.getElementById("MainContent_tbVinNumberNew").value = document.getElementById("MainContent_tbVinNumber").value
    var Vin = document.getElementById("MainContent_tbVinNumberNew").value;
    var StyleCode = document.getElementById("MainContent_tbStyle").value;

    var dataIn = {
        vin: Vin,
        styleCode: StyleCode
    };

    if (dataIn["vin"] != "recover" && dataIn["vin"].length < 17) {
        alert("VIN must be at least 17 digits!");
        return false;
    }

    $.ajax({
        type: 'POST',
        url: 'ChangeVin.aspx/VINCheck',
        data: JSON.stringify(dataIn),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            var parsed = JSON.parse(r);
            if (r.includes("003cdiv")) {
                for (let i = document.getElementById("OptionsTable").children.length - 1; i > 0; i--) {
                    document.getElementById("OptionsTable").removeChild(document.getElementById("OptionsTable").children[i])
                }
                document.getElementById("MainContent_tbYearNew").value = parsed.year;
                document.getElementById("MainContent_tbMakeNew").value = parsed.make;
                document.getElementById("MainContent_tbModelNew").value = parsed.model;
                document.getElementById("OptionsTable").insertAdjacentHTML('beforeend', parsed.options);
                document.getElementById("MainContent_hfDrillDown").value = 5;
                ChangeCss("Multi");
            }
            else if (r.includes("year") && parsed.year != "") {
                document.getElementById("MainContent_hfDrillDown").value = 5;
                document.getElementById("MainContent_tbYearNew").value = parsed.year;
                document.getElementById("MainContent_tbMakeNew").value = parsed.make;
                document.getElementById("MainContent_tbModelNew").value = parsed.model;
                document.getElementById("MainContent_tbStyleNew").value = parsed.style;
                document.getElementById("MainContent_hfStyleIdNew").value = parsed.styleId;
                document.getElementById("MainContent_hfDrillDown").value = 0;
                ChangeCss("New");
            }
            else {
                alert("An error has occured during the vin check process");
            }
        }
    });
}

function SetStyle(styleNum) {
    document.getElementById("MainContent_tbStyleNew").value = document.getElementById("StyleOption_" + styleNum).innerHTML;
    document.getElementById("MainContent_hfStyleIdNew").value = document.getElementById("StyleId_" + styleNum).innerHTML;
}
