$(document).ready(function () {
    styleTable();
});

function styleTable() {
    var usedCount = parseInt(document.getElementById("MainContent_hfUsedCount").value);
    var newCount = parseInt(document.getElementById("MainContent_hfNewCount").value);
    var totalCount = usedCount + newCount;

    var Thresholds = document.getElementById("MainContent_hfThresholds").value.split(";");

    var usedHolder;
    var newHolder;
    var percentage;

    //newHolder = parseFloat(document.getElementById("MainContent_lblAvgDaysPhotosNew").innerHTML);
    //document.getElementById("MainContent_lblAvgDaysPhotosNew").parentElement.className = newHolder > parseFloat(Thresholds[4]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblAvgDaysPhotosUsed").innerHTML);
    document.getElementById("MainContent_lblAvgDaysPhotosUsed").parentElement.className = usedHolder > parseFloat(Thresholds[3]) ? "TableCell RedCell" : "TableCell GreenCell";
    //document.getElementById("MainContent_lblAvgDaysPhotosTotal").parentElement.className = (newHolder + usedHolder) > parseFloat(Thresholds[5]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblAvgDescNew").innerHTML);
    //document.getElementById("MainContent_lblAvgDescNew").parentElement.className = newHolder > parseFloat(Thresholds[16]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblAvgDescUsed").innerHTML);
    document.getElementById("MainContent_lblAvgDescUsed").parentElement.className = usedHolder > parseFloat(Thresholds[15]) ? "TableCell RedCell" : "TableCell GreenCell";
    //document.getElementById("MainContent_lblAvgDescTotal").parentElement.className = (newHolder + usedHolder) > parseFloat(Thresholds[17]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehDescNew").innerHTML.substr(0, document.getElementById("MainContent_lblVehDescNew").innerHTML.indexOf(" ")));
    //percentage = ((newHolder) / (newCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehDescNew").parentElement.className = percentage < parseFloat(Thresholds[13]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehDescUsed").innerHTML.substr(0, document.getElementById("MainContent_lblVehDescUsed").innerHTML.indexOf(" ")));
    percentage = ((usedHolder) / (usedCount) * 100).toFixed(2);
    document.getElementById("MainContent_lblVehDescUsed").parentElement.className = percentage < parseFloat(Thresholds[12]) ? "TableCell RedCell" : "TableCell GreenCell";
    //percentage = ((newHolder + usedHolder) / (totalCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehDescTotal").parentElement.className = percentage < parseFloat(Thresholds[14]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehInternetNew").innerHTML.substr(0, document.getElementById("MainContent_lblVehInternetNew").innerHTML.indexOf(" ")));
    //percentage = ((newHolder) / (newCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehInternetNew").parentElement.className = percentage < parseFloat(Thresholds[10]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehInternetUsed").innerHTML.substr(0, document.getElementById("MainContent_lblVehInternetUsed").innerHTML.indexOf(" ")));
    percentage = ((usedHolder) / (usedCount) * 100).toFixed(2);
    document.getElementById("MainContent_lblVehInternetUsed").parentElement.className = percentage < parseFloat(Thresholds[9]) ? "TableCell RedCell" : "TableCell GreenCell";
    //percentage = ((newHolder + usedHolder) / (totalCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehInternetTotal").parentElement.className = percentage < parseFloat(Thresholds[11]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehPhotosNew").innerHTML.substr(0, document.getElementById("MainContent_lblVehPhotosNew").innerHTML.indexOf(" ")));
    //percentage = ((newHolder) / (newCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehPhotosNew").parentElement.className = percentage < parseFloat(Thresholds[1]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehPhotosUsed").innerHTML.substr(0, document.getElementById("MainContent_lblVehPhotosUsed").innerHTML.indexOf(" ")));
    percentage = ((usedHolder) / (usedCount) * 100).toFixed(2);
    document.getElementById("MainContent_lblVehPhotosUsed").parentElement.className = percentage < parseFloat(Thresholds[0]) ? "TableCell RedCell" : "TableCell GreenCell";
    //percentage = ((newHolder + usedHolder) / (totalCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehPhotosTotal").parentElement.className = percentage < parseFloat(Thresholds[2]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehListNew").innerHTML.substr(0, document.getElementById("MainContent_lblVehListNew").innerHTML.indexOf(" ")));
    //percentage = ((newHolder) / (newCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehListNew").parentElement.className = percentage < parseFloat(Thresholds[7]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehListUsed").innerHTML.substr(0, document.getElementById("MainContent_lblVehListUsed").innerHTML.indexOf(" ")));
    percentage = ((usedHolder) / (usedCount) * 100).toFixed(2);
    document.getElementById("MainContent_lblVehListUsed").parentElement.className = percentage < parseFloat(Thresholds[6]) ? "TableCell RedCell" : "TableCell GreenCell";
    //percentage = ((newHolder + usedHolder) / (totalCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehListTotal").parentElement.className = percentage < parseFloat(Thresholds[8]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehStyleNew").innerHTML.substr(0, document.getElementById("MainContent_lblVehStyleNew").innerHTML.indexOf(" ")));
    //percentage = ((newHolder) / (newCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehStyleNew").parentElement.className = percentage < parseFloat(Thresholds[19]) ? "TableCell RedCell" : "TableCell GreenCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehStyleUsed").innerHTML.substr(0, document.getElementById("MainContent_lblVehStyleUsed").innerHTML.indexOf(" ")));
    percentage = ((usedHolder) / (usedCount) * 100).toFixed(2);
    document.getElementById("MainContent_lblVehStyleUsed").parentElement.className = percentage < parseFloat(Thresholds[18]) ? "TableCell RedCell" : "TableCell GreenCell";
    //percentage = ((newHolder + usedHolder) / (totalCount) * 100).toFixed(2);
    //document.getElementById("MainContent_lblVehStyleTotal").parentElement.className = percentage < parseFloat(Thresholds[20]) ? "TableCell RedCell" : "TableCell GreenCell";

    //newHolder = parseFloat(document.getElementById("MainContent_lblVehUnavailNew").innerHTML);
    //document.getElementById("MainContent_lblVehUnavailNew").parentElement.className = "TableCell YellowCell";
    usedHolder = parseFloat(document.getElementById("MainContent_lblVehUnavailUsed").innerHTML);
    document.getElementById("MainContent_lblVehUnavailUsed").parentElement.className = "TableCell YellowCell";
    //document.getElementById("MainContent_lblVehUnavailTotal").parentElement.className = "TableCell YellowCell";
}

function ViewInBrowser() {
    var selected = $("#MainContent_lstReportSelector").val()

    switch (selected) {
        case "AutoLaunch":
            let domain = (new URL(location.href));
            location.href = domain.origin + "/WholesaleContent/Preferences/AutoLaunchRules.aspx?ListMode=1";
            break;
        case "healthReportMin":
        case "healthReportSummary":
        case "offerReport":
        case "WholesaleActiveListings":
        case "WholesaleAuctionGroupActiveListings":
            let domain2 = (new URL(location.href));
            location.href = domain2.origin + "/WholesaleContent/Reporting/ReportViewer.aspx?Mode=" + selected;
            break;
        case "healthReportDetail":
            let domain3 = (new URL(location.href));
            location.href = domain3.origin + "/WholesaleContent/Reporting/ReportViewer.aspx?Mode=" + selected + "&Date=" + document.getElementById("MainContent_tbRunDate").value;
            break;
        default:
            alert($("#MainContent_lstReportSelector").find("option:selected").text() + " has not been configured to be viewed in browser.");
    }
}

function ExportToExcel() {
    var selected = $("#MainContent_lstReportSelector").val()

    switch (selected) {
        case "healthReport":
        case "offerReport":
        case "detailedVin":
            $.ajax({
                type: "POST",
                url: 'Status.aspx/ExportToExcel',
                data: "{'Type': '" + selected + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(XMLHttpRequest.responseJSON.Message);
                },
                complete: function (response) {
                    var r = JSON.parse(response.responseJSON.d);
                    if (r.Success != "1") {
                        alert(r.Message);
                    }

                    return false;
                }
            });
            break;
        default:
            alert($("#MainContent_lstReportSelector").find("option:selected").text() + " has not been configured to be exported to Excel.");
    }
}

function OpenVehicleManagement(FilterKey) {
    toggleLoading(true, "Applying filters...");
    $.ajax({
        type: "POST",
        url: 'Status.aspx/SetAdvFilters',
        data: "{'Filter': '" + FilterKey + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) { 
                toggleLoading(false, r.message);
                let domain = (new URL(location.href))
                location.href = domain.origin + "/WholesaleContent/VehicleManagement.aspx";
            }
            else {
                toggleLoading(false, r.message);
            }

            return false;
        }
    });
}

function ReportSelectionChanged() {
    if (document.getElementById("MainContent_lstReportSelector").options[document.getElementById("MainContent_lstReportSelector").selectedIndex].innerHTML == "Health Report Detail") {
        document.getElementById("RunDateLabel").style.display = "table-cell"
        document.getElementById("RunDateSelector").style.display = "table-cell"
    }
    else {
        document.getElementById("RunDateLabel").style.display = "none"
        document.getElementById("RunDateSelector").style.display = "none"
    }

    if (document.getElementById("MainContent_lstReportSelector").options[document.getElementById("MainContent_lstReportSelector").selectedIndex].innerHTML == "Wholesale - Auto Launch Rules") {
        document.getElementById("MainContent_btnExportToExcel").style.display = "none";
    }
    else {
        document.getElementById("MainContent_btnExportToExcel").style.display = "block";
    }
}