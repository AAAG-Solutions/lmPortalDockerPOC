window.onload = function () {
    var path = document.referrer;
    if (path.includes("WholesaleDefault"))
        $("#sidebar_menu")[0].style["display"] = "none";
}

function OnChangeIdx(ddl, element) {
        document.getElementById(`MainContent_${element}`).value = ddl.options[ddl.selectedIndex].value;
}

function showHideSection(idName, idFieldSet) {
    var item = document.getElementById(idFieldSet).children[0];
    if (document.getElementById(idName).style.display == "none") {
        document.getElementById(idName).style.display = "block";
        item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + "&#9650;";
    }
    else if (document.getElementById(idName).style.display == "block") {
        document.getElementById(idName).style.display = "none";
        item.innerHTML = item.innerHTML.substring(0, item.innerHTML.length - 1) + "&#9660;";
    }
    if (screen.height > 1000) {
        ResizeGrid();
    }
}

function ResizeGrid() {
    var offSet = 340;
    if (document.getElementById("divFilters").style.display != "none")
        offSet = 430;
    document.getElementById("grid").style.height = "calc(100vh - " + offSet + "px)";
    document.getElementsByClassName("jsgrid-grid-body")[0].style.height = "calc(100vh - " + (offSet + 75) + "px)";
}

function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input.value); }, delay);
    };
}

function HandleChangePause(a) {
    var vin = $('#MainContent_txtSearch')[0].value;
    var gridData = $('#jsGrid').data('JSGrid').data;

    if (vin == '' && gridData.length >= 1) {
        // Little hack to bypass header issue when attempting to sort
        $('#jsGrid')[0].children[1].children[0].children[0].children[gridData.length - 1].scrollIntoView(false);
        $('#jsGrid')[0].children[1].children[0].children[0].children[0].scrollIntoView(false);
        return false;
    }

    for (let i = 0; i < gridData.length; i++) {
        if (gridData[i].VIN.indexOf(vin) >= 0) {
            $('#jsGrid').data('JSGrid').rowClick({
                event: { target: $('#chkSalesRow_' + i)[0] }, item: gridData[i]
            });
            $('#jsGrid')[0].children[1].children[0].children[0].children[Math.min(i + 3, gridData.length - 1)].scrollIntoView(false);
            break;
        }
    }
}

function ApplyFilters() {
    var gridData = $("#jsGrid").data("JSGrid");
    var gfilter = gridData.getFilter();

    var now = new Date();
    var startDate = $('#MainContent_txtStartDate').val();
    var endDate = $('#MainContent_txtEndDate').val();
    gfilter["kSaleTransactions"] = $('#MainContent_ddlSaleTransactions').val();
    gfilter["kWholesaleAuction"] = $('#MainContent_ddlMarketplace').val();

    var errorString = "";
    // Start Date
    if (Date.parse(startDate) > now)
        errorString += "\n\tThe start date must be set to today or before today.";
    if (Date.parse(startDate) < new Date().setDate(now.getDate() - 90))
        errorString += "\n\tThe start date must no earlier than 90 days ago.";
    // End Date
    if (Date.parse(endDate) > now)
        errorString += "\n\tThe end date must be set to today or before today.";
    if (Date.parse(endDate) < Date.parse(startDate))
        errorString += "\n\tThe end date must be after the start date."

    if (errorString != "") {
        alert("Error applying filters:" + errorString);
        return false;
    }

    gfilter["StartDate"] = startDate;
    if (new Date().format("yyyy-MM-dd") == endDate)
        gfilter["EndDate"] = new Date().toLocaleString().replace(',', '');
    else
        gfilter["EndDate"] = endDate;

    gfilter["filterSave"] = 1;
    gridData.search(gfilter);
    return false;
}

function QuickSelect() {
    var mode = document.getElementById("MainContent_ddlFilters").value;
    var gridData = $("#jsGrid").data("JSGrid").data;
    if (mode == 1) {
        for (var i = 0; i < gridData.length; i++) {
            var elem = document.getElementById("chkSalesRow_" + i.toString())
            if(elem != null)
                elem.checked = true;
        }
    }
    else if (mode == 2) {
        for (var i = 0; i < gridData.length; i++) {
            var elem = document.getElementById("chkSalesRow_" + i.toString())
            if (elem != null)
                elem.checked = false;
        }
    }
}

function SubmitApprovals(mark) {
    var grid = $("#jsGrid").data("JSGrid");
    var gridData = grid.data;
    var action = mark == 1 ? "mark" : "send";

    var sendSet = [];
    var alertMessage = "Error(s) found while trying to submit sales approvals:\n"
    var isError = false;
    for (var i = 0; i < gridData.length; i++) {
        var elem = document.getElementById("chkSalesRow_" + i.toString())
        if (elem != null) {
            if (elem.checked) {
                var buyerID = document.getElementById("txtBuyerAuctionAccess_" + i.toString()).value;
                var sellerID = document.getElementById("txtSellerAuctionAccess_" + i.toString()).value;
                var mileage = document.getElementById("txtMileage_" + i.toString()).value;

                if (action == "mark" && (buyerID == null || buyerID == "")) {
                    buyerID = 1234567;
                }

                if (buyerID == null || buyerID == "") {
                alertMessage += alertMessage.indexOf("Buyer") >= 0 ? "" : "\tBuyer Auction Access ID (Buyer #) is required for all sales transactions.\r\n";
                isError = true;
                }
                if (sellerID == null || sellerID == "") {
                    alertMessage += alertMessage.indexOf("Seller") >= 0 ? "" : "\tSeller Auction Access ID (Seller #) is required for all sales transactions.\r\n";
                    isError = true;
                }

                var item = {
                    "kInventory": gridData[i].kInventory,
                    "BuyerID": buyerID,
                    "kWholesaleSoldHistory": gridData[0].kWholesaleSoldHistory,
                    "SellerID": sellerID,
                    "Mileage": mileage
                };
                sendSet.push(item);
            }
        }
    }
    if (sendSet.length == 0) {
        alertMessage += '\tYou must select at least one Sales Transaction to send.\r\n'
        isError = true;
    }

    if (isError) {
        alert(alertMessage);
        return false;
    }

    $.ajax({
        type: "POST",
        url: 'SalesDataApproval.aspx/SetSalesData',
        data: "{'JsonData': '" + JSON.stringify(sendSet) + "', 'op':'" + action + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            if (response.d.success == true) {
                alert(`Successfully submitted ${response.d.value.success} records!`)
                grid.search();
            }
            else {
                alert(`Something went wrong with one or more records.\n\tSuccessful Records: ${response.d.value.success}\n\tFail Records: ${response.d.value.fail}`)
                grid.search();
            }
            return false;
        }
    });
}

function RowDoubleClick() {
}

function GridRowSelected(item) {
}

function ClearRowSelection() {
}