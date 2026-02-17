function HandleButtonPress(sender) {
    if (sender == "add") {
        AddBlackoutWindowRule();
    }
    else if (sender == "edit") {
        EditBlackoutWindowRule();
    }
    else if (sender == "delete") {
        DeleteBlackoutWindowRule();
    }
    else if (sender == "submit")
        SubmitRule();
    else if (sender == "cancel")
        showWindow(false);
    else
        SwitchRule();
}

function SwitchRule() {
    var op = document.getElementById("BWAction").innerText.toLowerCase();
    var selectedRule = document.getElementById("MainContent_txtRule").value;

    var gridData = $("#jsGrid").data("JSGrid").data;
    var row = null
    for (var i = 0; i < gridData.length; i++) {
        if (gridData[i].kWholesaleBlackOutWindow == selectedRule) {
            row = gridData[i];
            break;
        }
    }

    ClearAllFields();
    loadFields(row);

    document.getElementById("MainContent_cbBlackOutWindowSuspend").checked = op == "suspend" ? true : false;
    SubmitRule();
}

function AddBlackoutWindowRule() {
    $('#BlackoutWindowAuctions').find('input, textarea, button, select').prop('disabled', false);
    $('#BlackoutWindowSettings').find('input, textarea, button, select').prop('disabled', false);
    ClearAllFields();
    FrequencyChange();
    onIntervalChange();
    document.getElementById("MainContent_hfMode").value = 'add';
    document.getElementById("HeaderText").innerHTML = 'Add Blackout Window Rule';
    showMultiSelect(true);
    showWindow(true);
    //openRule(document.getElementById("MainContent_txtRule").value);
}

function EditBlackoutWindowRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select a Blackout Rule to edit!");
        return false;
    }

    $('#BlackoutWindowAuctions').find('input, textarea, button, select').prop('disabled', false);
    $('#BlackoutWindowSettings').find('input, textarea, button, select').prop('disabled', false);

    document.getElementById("MainContent_hfMode").value = 'edit';
    document.getElementById("HeaderText").innerHTML = 'Edit Blackout Window Rule';

    var gridData = $("#jsGrid").data("JSGrid").data;
    var row = null
    for (var i = 0; i < gridData.length; i++) {
        if (gridData[i].kWholesaleBlackOutWindow == selectedRule) {
            row = gridData[i];
            break;
        }
    }

    ClearAllFields();
    loadFields(row);
    showMultiSelect(false)
    showWindow(true);
}

function DeleteBlackoutWindowRule() {
    var selectedRule = document.getElementById("MainContent_txtRule").value;
    if (selectedRule == "") {
        alert("Please select an Blackout Rule to delete!");
        return false;
    }

    $('#BlackoutWindowAuctions').find('input, textarea, button, select').prop('disabled', true);
    $('#BlackoutWindowSettings').find('input, textarea, button, select').prop('disabled', true);

    document.getElementById("MainContent_hfMode").value = 'delete';
    document.getElementById("HeaderText").innerHTML = 'Delete Blackout Window Rule';

    var gridData = $("#jsGrid").data("JSGrid").data;
    var row = null
    for (var i = 0; i < gridData.length; i++) {
        if (gridData[i].kWholesaleBlackOutWindow == selectedRule) {
            row = gridData[i];
            break;
        }
    }

    ClearAllFields();
    loadFields(row);
    showMultiSelect(false)
    showWindow(true);
}

function showWindow(show) {
    var popbkg = document.getElementById("pop_overlay");
    if (popbkg) {
        if (show) {
            popbkg.style.display = "block";
        } else {
            popbkg.style.display = "none";
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

function showMultiSelect(toShow) {
    document.getElementById("BlackoutWindowAuctions").className = toShow ? "sectionFieldset" : "hidden";
    document.getElementById("AuctionDropdown").className = toShow ? "hidden" : "tableRow";
}

function loadFields(row) {
    SetSelectedValue(document.getElementById("MainContent_lstAuction"), row.kWholesaleAuction);
    SetSelectedValue(document.getElementById("MainContent_lstMinYear"), row.MinYear);
    SetSelectedValue(document.getElementById("MainContent_lstMaxYear"), row.MaxYear);
    SetSelectedValue(document.getElementById("MainContent_lstStartDay"), row.StartDOW);
    SetSelectedValue(document.getElementById("MainContent_lstEndDay"), row.EndDOW);
    SetSelectedValueByDisplay(document.getElementById("MainContent_lstFrequency"), row.Type);
    FrequencyChange();
    if (row.Type == "Weekly")
        SetSelectedValue(document.getElementById("MainContent_lstIntervalWeek"), row.Interval);
    else {
        SetSelectedValue(document.getElementById("MainContent_lstIntervalMonth"), row.Interval);
        SetSelectedValue(document.getElementById("MainContent_lstIntervalMonthDay"), row.IntervalDOW);
    }
    onIntervalChange();
    SetSelectedValue(document.getElementById("MainContent_lstLotLocation"), row.InvLotLocation);
    SetSelectedValue(document.getElementById("MainContent_lstVehicleMake"), row.Make);
    document.getElementById("MainContent_tbStartTime").value = row.StartTime;
    document.getElementById("MainContent_tbEndTime").value = row.EndTime;
    document.getElementById("MainContent_tbInitialIntervalDay").value = new Date(row.IntervalStart).toISOString().split('T')[0];
    document.getElementById("MainContent_cbALAuctionRemove").checked = row.RemoveAutoLaunchAuction == "0" ? false : true;
    document.getElementById("MainContent_cbManualAuctionRemove").checked = row.RemoveManualAuction == "0" ? false : true;
    document.getElementById("MainContent_cbBlackOutWindowSuspend").checked = row.Suspended == "0" ? false : true;
}

function SetSelectedValue(control, value) {
    var opt = control.options;
    for (var i = 0; i < control.options.length; i++) {
        if (opt[i].value == value) {
            opt[i].selected = true;
            control.value = value;
            break;
        }
    }
}

function SetSelectedValueByDisplay(control, value) {
    var opt = control.options;
    for (var i = 0; i < control.options.length; i++) {
        if (opt[i].innerHTML == value) {
            opt[i].selected = true;
            control.value = opt[i].value;
            break;
        }
    }
}

function FrequencyChange() {
    if (document.getElementById("MainContent_lstFrequency").value == "1") {
        document.getElementById("divBlackoutWeek").style.setProperty("display", "table-cell");
        document.getElementById("divBlackoutMonth").style.setProperty("display", "none", "important");
    }
    else {
        document.getElementById("divBlackoutWeek").style.setProperty("display", "none", "important");
        document.getElementById("divBlackoutMonth").style.setProperty("display", "table-cell");
    }
}

function ClearAllFields() {
    for (var i = 0; i < document.getElementById("MainContent_AuctionChecks").children.length; i++) {
        var item = document.getElementById("MainContent_AuctionChecks").children[i];
        for (var j = 1; j < item.children.length; j += 2) {
            item.children[j].children[0].checked = false;
        }
    }

    document.getElementById("MainContent_lstAuction").options[0].selected = true;
    document.getElementById("MainContent_lstAuction").value = document.getElementById("MainContent_lstAuction").options[0].value;
    document.getElementById("MainContent_lstMinYear").options[0].selected = true;
    document.getElementById("MainContent_lstMinYear").value = document.getElementById("MainContent_lstMinYear").options[0].value;
    document.getElementById("MainContent_lstMaxYear").options[0].selected = true;
    document.getElementById("MainContent_lstMaxYear").value = document.getElementById("MainContent_lstMaxYear").options[0].value;
    document.getElementById("MainContent_lstStartDay").options[0].selected = true;
    document.getElementById("MainContent_lstStartDay").value = document.getElementById("MainContent_lstStartDay").options[0].value;
    document.getElementById("MainContent_lstEndDay").options[0].selected = true;
    document.getElementById("MainContent_lstEndDay").value = document.getElementById("MainContent_lstEndDay").options[0].value;
    document.getElementById("MainContent_lstFrequency").options[0].selected = true;
    document.getElementById("MainContent_lstFrequency").value = document.getElementById("MainContent_lstFrequency").options[0].value;
    document.getElementById("MainContent_lstIntervalWeek").options[0].selected = true;
    document.getElementById("MainContent_lstIntervalWeek").value = document.getElementById("MainContent_lstIntervalWeek").options[0].value;
    document.getElementById("MainContent_lstIntervalMonth").options[0].selected = true;
    document.getElementById("MainContent_lstIntervalMonth").value = document.getElementById("MainContent_lstIntervalMonth").options[0].value;
    document.getElementById("MainContent_lstIntervalMonthDay").options[0].selected = true;
    document.getElementById("MainContent_lstIntervalMonthDay").value = document.getElementById("MainContent_lstIntervalMonthDay").options[0].value;
    document.getElementById("MainContent_lstLotLocation").options[0].selected = true;
    document.getElementById("MainContent_lstLotLocation").value = document.getElementById("MainContent_lstLotLocation").options[0].value;
    document.getElementById("MainContent_lstVehicleMake").options[0].selected = true;
    document.getElementById("MainContent_lstVehicleMake").value = document.getElementById("MainContent_lstVehicleMake").options[0].value;
    document.getElementById("MainContent_tbStartTime").value = "";
    document.getElementById("MainContent_tbEndTime").value = "";
    document.getElementById("MainContent_tbInitialIntervalDay").value = "";
    document.getElementById("MainContent_cbALAuctionRemove").checked = false;
    document.getElementById("MainContent_cbManualAuctionRemove").checked = false;
    document.getElementById("MainContent_cbBlackOutWindowSuspend").checked = false;
    
}

function SubmitRule() {
    var checked = [];
    var mode = document.getElementById("MainContent_hfMode").value;

    if (mode != "delete") {
        var canContinue = RunValidations();
        if (!canContinue)
            return false;
    }

    if (mode == "add") {
        for (var i = 0; i < document.getElementById("MainContent_AuctionChecks").children.length; i++) {
            var item = document.getElementById("MainContent_AuctionChecks").children[i];
            for (var j = 1; j < item.children.length; j += 2) {
                if (item.children[j].children[0].checked == true)
                    checked.push(item.children[j].children[0].id.substring(item.children[j].children[0].id.indexOf("_") + 3));
            }
        }
    }

    var row = null;
    if (document.getElementById("MainContent_txtRule").value != "") {
        var gridData = $("#jsGrid").data("JSGrid").data;
        for (var i = 0; i < gridData.length; i++) {
            if (gridData[i].kWholesaleBlackOutWindow == document.getElementById("MainContent_txtRule").value) {
                row = gridData[i];
                break;
            }
        }
    }

    var datain = {
        Auctions: checked.length == 0 ? null : checked,
        Auction: checked.length == 0 ? document.getElementById("MainContent_lstAuction").value : null,
        kWholesaleBlackOutWindow: row == null ? null : row.kWholesaleBlackOutWindow,
        kWholesaleFacilitatedAuctionCode: row == null ? null : row.kWholesaleFacilitatedAuction,
        kAASale: row == null ? null : row.kAASale,
        MinYear: document.getElementById("MainContent_lstMinYear").value,
        MaxYear: document.getElementById("MainContent_lstMaxYear").value,
        StartDay: document.getElementById("MainContent_lstStartDay").value,
        EndDay: document.getElementById("MainContent_lstEndDay").value,
        Frequency: document.getElementById("MainContent_lstFrequency").value,
        Interval: document.getElementById("MainContent_lstFrequency").value == "1" ? document.getElementById("MainContent_lstIntervalWeek").value : document.getElementById("MainContent_lstIntervalMonth").value,
        InitialIntervalDay: document.getElementById("MainContent_tbInitialIntervalDay").value,
        IntervalDay: document.getElementById("MainContent_lstFrequency").value == "1" ? null : document.getElementById("MainContent_lstIntervalMonthDay").value,
        LotLocation: document.getElementById("MainContent_lstLotLocation").value,
        Make: document.getElementById("MainContent_lstVehicleMake").value,
        StartTime: document.getElementById("MainContent_tbStartTime").value,
        EndTime: document.getElementById("MainContent_tbEndTime").value,
        RemoveAutoLaunchAuction: document.getElementById("MainContent_cbALAuctionRemove").checked == true ? 1 : 0,
        RemoveManualAuction: document.getElementById("MainContent_cbManualAuctionRemove").checked == true ? 1 : 0,
        Suspended: document.getElementById("MainContent_cbBlackOutWindowSuspend").checked == true ? 1 : 0,
        Disable: document.getElementById("MainContent_hfMode").value == "delete" ? "1" : "0"
    };

    $.ajax({
        type: "POST",
        url: 'BlackoutWindowRules.aspx/SaveRules',
        data: "{'DataIn':'" + JSON.stringify(datain) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = JSON.parse(response.d);
            if (r.Success == 1) {
                mode == "delete" ? alert("Rule deleted successfully") : alert("Rule(s) saved successfully");
                location.reload();
                return false;
            }

            mode == "add" ? alert("Rule(s) failed to save") : alert("Changes failed to save");
        }
    });
}

function onIntervalChange() {
    document.getElementById("divIIDay").className = document.getElementById("MainContent_lstIntervalWeek").value == "1" ? "hide tableRow" : "tableRow";
}

function RunValidations() {
    var alertStr = "";
    var mode = document.getElementById("MainContent_hfMode").value;
    var anyChecked = false;
    if (mode == "add") {
        for (var i = 0; i < document.getElementById("MainContent_AuctionChecks").children.length; i++) {
            var item = document.getElementById("MainContent_AuctionChecks").children[i];
            for (var j = 1; j < item.children.length; j += 2) {
                if (item.children[j].children[0].checked == true) {
                    anyChecked = true;
                    break;
                }
            }
        }
        if (anyChecked == false)
            alertStr += "You must select at least 1 auction for the rule to apply to.\n";
    }

    var baseTimeString = "You must set a time for the following fields:\n";
    if (document.getElementById("MainContent_tbStartTime").value == "")
        baseTimeString += "\tStart Time\n";
    if (document.getElementById("MainContent_tbEndTime").value == "")
        baseTimeString += "\tEnd Time\n";

    if (baseTimeString != "You must set a time for the following fields:\n")
        alertStr += baseTimeString;

    if (document.getElementById("MainContent_lstFrequency").value == "1" && document.getElementById("MainContent_lstIntervalWeek").value != "1" && document.getElementById("MainContent_tbInitialIntervalDay").value == "")
        alertStr += "You must set a value for the Initial Interval Day";

    // #TODO: add validations to make sure rules don't overlap for the same auctions

    if (alertStr.length != 0) {
        alert(alertStr);
        return false;
    }

    return true;
}

function ListView() {
    var mode = document.getElementById("jsGrid").className == "jsgrid" || document.getElementById("MainContent_hfViewList").value == "1" ? true : false;
    if (mode) {
        document.getElementById("jsGrid").className = "Hide";
        document.getElementById("BWAdd").className = "Hide";
        document.getElementById("BWEdit").className = "Hide";
        document.getElementById("BWAction").className = "Hide";
        document.getElementById("BWDelete").className = "Hide";
        document.getElementById("BWList").innerText = "Grid";
        document.getElementById("BWPrint").className = "submitButton";
        document.getElementById("MainContent_ListForm").className = "jsgrid print";
    }
    else {
        document.getElementById("jsGrid").className = "jsgrid";
        document.getElementById("BWAdd").className = "submitButton";
        document.getElementById("BWEdit").className = "submitButton";
        document.getElementById("BWAction").className = "submitButton";
        document.getElementById("BWDelete").className = "submitButton";
        document.getElementById("BWList").innerText = "List";
        document.getElementById("BWPrint").className = "Hide";
        document.getElementById("MainContent_ListForm").className = "Hide";
    }
}


function RowDoubleClick() {
    EditBlackoutWindowRule();
}

function ClearRowSelection() {
    document.getElementById("MainContent_txtRule").value = "";
}

function GridRowSelected(item) {
    document.getElementById("MainContent_txtRule").value = item.data("JSGridItem").kWholesaleBlackOutWindow;
    document.getElementById("BWAction").innerText = item.data("JSGridItem").Suspended == "0" ? "Suspend" : "Resume";
}

$(document).ready(function () {
    ListView();
    document.getElementById("MainContent_hfViewList").value = "0";
});