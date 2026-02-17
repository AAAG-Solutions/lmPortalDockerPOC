function OtherReason(self) {
    if (self.selectedIndex == 7) {
        document.getElementById("otherReason").style.display = "table-row";
    }
    else {
        document.getElementById("otherReason").style.display = "none";
    }
}

function SendCreditRequest() {
    var vin = document.getElementById("inputVIN").value;
    var auction = document.getElementById("MainContent_lstMarketPlace");
    var reason = document.getElementById("MainContent_inputReason").value;

    if (auction.selectedIndex == 0) {
        alert("Please select the appropriate auction for the credit request!");
        return;
    }

    if (reason == "Other" && document.getElementById('inputReasonDetail').value == "") {
        alert("You must enter a valid reason for the credit request!");
        return;
    }

    if (vin == "") {
        alert("You must enter a valid VIN for a credit request!");
        return;
    }

    var json = {
        kWholesaleAuction: auction.options[auction.selectedIndex].value,
        VIN: vin,
        Reason: reason == "Other" ? "Other - " + document.getElementById('inputReasonDetail').value : reason
    }

    $.ajax({
        type: "POST",
        url: 'CreditRequest.aspx/CreditRequestSet',
        data: "{'data': '" + JSON.stringify(json) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            var gridData = $(`#jsGrid`).data("JSGrid");
            if (r.success) {
                alert("The Credit Request has been submitted!");
                gridData.refresh();
            }
            else
                alert(r.message);
        }
    });
}

// Disable Enter Key
$(document).keypress(function (e) {
    if (e.which == 13) {
        return false;
    }
});