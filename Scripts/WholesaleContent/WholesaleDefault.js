function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input.value); }, delay);
    };
}

function fnSubmit() {
    var submitButton = $('#MainContent_btnSubmit');
    if (submitButton) { submitButton.disabled = true; };
    if ($('#MainContent_txtkDealer').val() == "") {
        alert('You must select a dealer.');
        if (submitButton) { submitButton.disabled = false; };
        return false;
    }
    return true;
}

function GridRowSelected(row) {
    $('#MainContent_txtkDealer').val(row[0].children[0].innerHTML);
}

function ClearRowSelection() {
    $('#MainContent_txtkDealer').val("");
}

function HandleChangePause(a) {
    ddlChanged();
}

function ddlChanged() {
    var grid = $("#jsGrid").data("JSGrid");
    var gfilter = grid.getFilter();
    gfilter.DealerName = $('#MainContent_txtSearch').val();
    gfilter.AccountStatus = $('#MainContent_ddlAccountStatus').val();
    gfilter.CustomerType = $('#MainContent_ddlCustomerType').val();
    gfilter.AccountGroup = $('#MainContent_ddlAccountGroup').val();
    gfilter.AccountRep = $('#MainContent_ddlAccountRep').val();
    grid.search(gfilter);
}

function fnFilterClear() {
    $('#MainContent_txtSearch').val('');
    $("#MainContent_ddlAccountStatus")[0].selectedIndex = 0;
    $("#MainContent_ddlCustomerType")[0].selectedIndex = 0;
    $("#MainContent_ddlAccountGroup")[0].selectedIndex = 0;
    $("#MainContent_ddlAccountRep")[0].selectedIndex = 0;
    ddlChanged();
}

function RowDoubleClick(item) {
    toggleLoading(true, "Loading Dealer info...");
    var submitButton = $('#MainContent_btnSubmit');
    submitButton.click();
}

function fnGoToWholesale(kDealer) {
    toggleLoading(true, "Loading Dealer info...");
    $('#MainContent_txtkDealer').val(kDealer);
    var submitButton = $('#MainContent_btnSubmit');
    submitButton.click();
}

function fnGoToSupportTool() {
    window.open('https://supporttool.liquidmotors.com/monitor', '_blank');
}


function fnGoToSellDownAPI() {
    document.getElementById('MainContent_btnSellDownAPI').click();
}

// Little hack to keep imgs inline
function excelDownload() {
    document.getElementById('MainContent_exportBtn').click();
}

// Disable enter key
$(document).keypress(function (e) {
    if (e.which == 13) {
        return false;
    }
});