function CheckAuction(selectedItem) {
    var select = selectedItem.split("!");

    if (select.length == 1 && select[0] == 1)
    {
        var list = document.querySelectorAll('[id*="Check_"]');

        list.forEach(function (item) {
            if (!item.disabled) {
                item.checked = true;
            }
        });
    } else if (select.length == 1 && select[0] == 2)
    {
        var list = document.querySelectorAll('[id*="Check_"]');

        list.forEach(function (item) {
            if (!item.disabled) {
                item.checked = false;
            }
        });
    } else
    {
        var list = document.querySelectorAll(`[id^="${select[1]}Check_"]`);
        var bool = parseInt(select[0]) == 1 ? true : false;

        list.forEach(function (item) {
            if (!item.disabled) {
                item.checked = bool;
            }
        });
    }
}

function VINSearch() {
    // Gather data from jsGrid to loop through and find specified VIN
    var gridData = $("#jsGrid").data("JSGrid").data;
    var vin = document.getElementById("VinSearch").value.toUpperCase();
    var reg = new RegExp(vin);

    if (vin == "") {
        return false;
    }

    gridData.forEach(function (item) {
        if (item.Vin == vin || reg.exec(item.Vin)) {
            // We use a price field as a 'target' to scroll into view and select the row

            // NOTE: We will only find the first instance of a regex match. This is by design
            // and can be changed if needed/wanted
            var targetRow = $(`#VinNum_${item.kListing}`)[0];
            targetRow.scrollIntoView(false);
            $("#jsGrid").data("JSGrid").rowClick({ event: { target: targetRow }, item: item });
        }
    });
}

function RemoveFromMultipleAuctions() {
    var gridData = $("#jsGrid").data("JSGrid").data;

    var checkBoxes = $('input[id*="Check_"]:checked').toArray();
    var successCount = 0;
    var unsuccessCount = 0;

    if (checkBoxes.length != 0) {
        gridData.forEach(function (vehicle) {
            reg = new RegExp(vehicle.kListing + '$');
            var selectedAuctions = $.grep(checkBoxes, element => { return reg.exec(element.id); });

            var vehicleInfo = {
                MarkUnavailable: document.getElementById("MainContent_chkMarkUnavail").checked && document.getElementById("divMarkUnavail").style.display != 'none',
                kListing: vehicle.kListing,
                auctions: []
            };
            selectedAuctions.forEach(item => vehicleInfo.auctions.push(item.value));

            if (vehicleInfo.auctions.length > 0) {
                $.ajax({
                    type: "POST",
                    url: 'MultiEnd.aspx/RemoveFromMultipleAuctions',
                    data: `{'vehicles': '${JSON.stringify(vehicleInfo)}'}`,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    async: false,
                    error: function (request, textStatus, errorThrown) {
                        var errorData = {
                            message: request.responseJSON.Message,
                            source: 'MultiEnd',
                            lineno: request.responseJSON.ExceptionType,
                            colno: null,
                            error: request.responseJSON.StackTrace
                        };

                        // Send the error data to the server using an AJAX request
                        var xhr = new XMLHttpRequest();
                        xhr.open("POST", "/ErrorHandler", true);
                        xhr.setRequestHeader("Content-Type", "application/json");
                        xhr.send(JSON.stringify(errorData));

                        alert(errorThrown);
                    },
                    complete: function (response) {
                        var r = response.responseJSON.d;
                        if (r.success)
                            successCount++;
                        else
                            unsuccessCount++;
                    }
                });
            }
        });

        alert(`Vehicles successfully submitted to End: ${successCount}\r\nVehicles failed to End: ${unsuccessCount}`);
        location.reload();
    }
    else
        alert("You either haven't selected any auctions to send to or do not have any enabled!");
}

window.onerror = function (message, source, lineno, colno, error) {
    var errorData = {
        message: message,
        source: source,
        lineno: lineno,
        colno: colno,
        error: error ? error.stack : null
    };

    // Send the error data to the server using an AJAX request
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/ErrorHandler", true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(JSON.stringify(errorData));
}