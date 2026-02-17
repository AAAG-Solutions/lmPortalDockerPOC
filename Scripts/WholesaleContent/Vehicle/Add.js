function VinExplosion() {
    var vinExplosion = document.getElementById('MainContent_vinExplosion').checked;

    var yearListItem = $('#YearListDropdown');
    var yearList = document.getElementById('MainContent_YearList');
    var makeListItem = $('#MakeListDropdown');
    var makeList = document.getElementById('MainContent_MakeList');
    var modelListItem = $('#ModelListDropdown');
    var modelList = document.getElementById('MainContent_ModelList')
    var styleListItem = $('#StyleListDropdown');
    var styleList = document.getElementById('MainContent_StyleList');

    if (vinExplosion) {
        yearListItem.css("display", "table-row");
        makeListItem.css("display", "table-row");
        modelListItem.css("display", "table-row");
        styleListItem.css("display", "table-row");
    } else {
        yearList.selectedIndex = 0;
        yearListItem.css("display", "none");
        makeList.length = 0;
        makeListItem.css("display", "none");
        modelList.length = 0;
        modelListItem.css("display", "none");
        styleList.length = 0;
        styleListItem.css("display", "none");
    }
}

function GetMakeList() {
    var dataIn = {
        year: ""
    };

    // Year
    var year = document.getElementById('MainContent_YearList');
    dataIn["year"] = year.options[year.selectedIndex].text;

    if (year.selectedIndex == 0) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "Add.aspx/GetListMake",
        data: JSON.stringify(dataIn),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            var rList = BuildDropdownList(r);
            var makeList = document.getElementById('MainContent_MakeList');

            if (makeList.length != 0) {
                makeList.length = 0;
                document.getElementById('MainContent_ModelList').length = 0;
                document.getElementById('MainContent_StyleList').length = 0;
            }

            rList.forEach(element => makeList.appendChild(element));
        }
    });
}

function GetModelList() {
    var dataIn = {
        year: "",
        make: ""
    };

    // Year
    var year = document.getElementById('MainContent_YearList');
    dataIn["year"] = year.options[year.selectedIndex].text;

    // Make
    var make = document.getElementById('MainContent_MakeList');
    dataIn["make"] = make.options[make.selectedIndex].text;

    $.ajax({
        type: "POST",
        url: "Add.aspx/GetListModel",
        data: JSON.stringify(dataIn),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            var rList = BuildDropdownList(r);
            var modelList = document.getElementById('MainContent_ModelList');

            if (modelList.length != 0) {
                modelList.length = 0;
                document.getElementById('MainContent_StyleList').length = 0;
            }

            rList.forEach(element => modelList.appendChild(element));
        }
    });
}

function GetStyleList() {
    var dataIn = {
        year: "",
        make: "",
        model: ""
    };

    // Year
    var year = document.getElementById('MainContent_YearList');
    dataIn["year"] = year.options[year.selectedIndex].text;

    // Make
    var make = document.getElementById('MainContent_MakeList');
    dataIn["make"] = make.options[make.selectedIndex].text;

    // Model
    var model = document.getElementById('MainContent_ModelList');
    dataIn["model"] = model.options[model.selectedIndex].text;

    $.ajax({
        type: "POST",
        url: "Add.aspx/GetListStyle",
        data: JSON.stringify(dataIn),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            var rList = BuildDropdownList(r);
            var styleList = document.getElementById('MainContent_StyleList');

            if (styleList.length != 0)
                styleList.length = 0;

            rList.forEach(element => styleList.appendChild(element));
        }
    });
}

function BuildDropdownList(stringList) {
    var list = [];
    if (stringList.length == 0) {
        return list;
    }

    var options = stringList.split('|');
    for (var i = 0; i < options.length; i++) {
        var option = options[i].split(':');
        var el = document.createElement("option");
        el.text = option[1];
        el.value = option[0];
        list.push(el);
    }

    return list;
}

function checkVIN() {
    var vin = document.getElementById("MainContent_InputVIN").value;
    var stock = document.getElementById("MainContent_InputStock").value;
    var StyleCode = document.getElementById("MainContent_styleCode").value;

    var dataIn = {
        vin: vin,
        styleCode: StyleCode
    };

    if (dataIn["vin"].toLowerCase() != "recover" && dataIn["vin"].length < 6) {
        alert("VIN must be at least 6 digits!");
        return false;
    }

    if (vin.toLowerCase() == "recover" || stock.toLowerCase() == "recover") {
        // Ensure that we are passing lowercase 'recover' for the proc
        if (vin.toLowerCase() == "recover")
            dataIn["vin"] = vin.toLowerCase();
        if (stock.toLowerCase() == "recover")
            document.getElementById("MainContent_InputStock").value = stock.toLowerCase();

        SaveInventory();
    }
    else {
        $.ajax({
            type: 'POST',
            url: 'Add.aspx/VINCheck',
            data: JSON.stringify(dataIn),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseJSON.Message);
            },
            complete: function (response) {
                var r = response.responseJSON.d;
                var parent = document.getElementById("lblAddVehicle");

                if (r.includes("fieldset")) {
                    document.getElementById("breadCrumb").innerHTML = "Inventory / Add Vehicle / Vehicle Configuration"
                    document.getElementById("AddVehicle").style.display = "none";
                    parent.insertAdjacentHTML('beforeend', r);
                }
                else if (r.includes("year")) {
                    SaveInventory(JSON.parse(r));
                }
            }
        });
    }
}

function SetStyle(SelectedNumber) {
    var Style = document.getElementById("StyleOption_" + SelectedNumber).innerText;
    var Id = document.getElementById("StyleId_" + SelectedNumber).innerText;

    var vehicleInfo = {
        year: document.getElementById("configYear").innerText,
        make: document.getElementById("configMake").innerText,
        model: document.getElementById("configModel").innerText,
        style: Style,
        styleId: Id
    };

    SaveInventory(vehicleInfo);
}

function SetTrim(SelectedNumber) {
    var Style = document.getElementById("TrimOption_" + SelectedNumber).innerText;
    var Id = document.getElementById("VehicleId_" + SelectedNumber).innerText;

    var vehicleInfo = {
        year: document.getElementById("configYear").innerText,
        make: document.getElementById("configMake").innerText,
        model: document.getElementById("configModel").innerText,
        style: Style,
        styleId: Id
    };

    SaveInventory(vehicleInfo);
}

function SaveInventory(data) {
    toggleLoading(true, "");
    var vin = document.getElementById("MainContent_InputVIN").value;
    var stock = document.getElementById("MainContent_InputStock").value;

    var fields = ValidateFields();
    if (fields != "") {
        var fieldsSplit = fields.split("|");
        var fieldsOutput = "You must enter values into the following fields:";
        for (var i = 0; i < fieldsSplit.length; i++)
            fieldsOutput += "\n\t" + fieldsSplit[i];
        alert(fieldsOutput);

        toggleLoading(false, "");
        return false;
    }

    var dataIn = {
        vin: "",
        StockNumber: "",
        year: 0,
        make: "",
        model: "",
        style: "",
        StyleId: 0,
        drilldown: 0,
        ImportOverride: 0,
        status: 0,
        StockType: 0,
        mileage: 0,
        InvCost: 0,
        InvListPrice: 0,
        InternetPrice: 0
    };

    dataIn["StockNumber"] = document.getElementById("MainContent_InputStock").value;
    dataIn["vin"] = document.getElementById("MainContent_InputVIN").value;
    dataIn["mileage"] = parseInt(document.getElementById('MainContent_InputMileage').value) || 0;
    dataIn["InvCost"] = parseInt(document.getElementById('MainContent_InputCost').value) || 0;
    dataIn["InvListPrice"] = parseInt(document.getElementById('MainContent_InputPrice').value) || 0;
    dataIn["InternetPrice"] = parseInt(document.getElementById('MainContent_SpecialPrice').value) || 0;

    if (document.getElementById('MainContent_OverrideImport').checked) {
        dataIn["importOverride"] = 1;
    }

    // VehicleType
    var stockType = document.getElementById('MainContent_Classification');
    dataIn["StockType"] = stockType.options[stockType.selectedIndex].value;

    // VehicleStatus
    var status = document.getElementById('MainContent_VehicleStatus');
    dataIn["status"] = status.options[status.selectedIndex].value;

    if (data) {
        dataIn["year"] = data["year"];
        dataIn["make"] = data["make"];
        dataIn["model"] = data["model"];
        dataIn["style"] = escapeQuotes(data["style"]);
        dataIn["StyleId"] = data["styleId"];
    } else if (vin == "recover" || stock == "recover") {
        dataIn["year"] = dataIn["make"] = dataIn["model"] = dataIn["style"] = dataIn["StyleId"] = "0";
    }
    else {

        if (document.getElementById('MainContent_vinExplosion').checked) {
            // Year
            var year = document.getElementById('MainContent_YearList');
            dataIn["year"] = parseInt(year.options[year.selectedIndex].text) || 0;

            // Make
            var make = document.getElementById('MainContent_MakeList');
            dataIn["make"] = make.options[make.selectedIndex].text;

            // Model
            var model = document.getElementById('MainContent_ModelList');
            dataIn["model"] = model.options[model.selectedIndex].text;

            // Style
            var style = document.getElementById('MainContent_StyleList');
            dataIn["style"] = escapeQuotes(style.options[style.selectedIndex].text);
            dataIn["styldId"] = parseInt(style.options[style.selectedIndex].value) || 0;

            dataIn["drilldown"] = 1;
        }
    }

    $.ajax({
        type: 'POST',
        url: 'Add.aspx/AddInventory',
        data: "{ 'vehicleInfo': '" + JSON.stringify(dataIn) + "' }",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.value != "" && r.success) {
                toggleLoading(false, "Vehicle successfully added. Redirecting to new listing.");
                alert(r.message);
                window.location.href = `/WholesaleContent/Vehicle/Update.aspx?kListing=${r.value}`;
            } else {
                toggleLoading(false, "");
                alert("Unable to add specified vehicle! Please contact support");
            }
        },
    });
    
}

function ValidateFields() {
    var outputString = "";

    if (document.getElementById("MainContent_InputVIN").value == "")
        outputString += "VIN|";
    if (document.getElementById("MainContent_InputStock").value == "")
        outputString += "Stock Number|";
    if (document.getElementById("MainContent_Classification").selectedIndex == 0)
        outputString += "Classification|";
    if (document.getElementById("MainContent_VehicleStatus").selectedIndex == 0)
        outputString += "Vehicle Status|";
    if (document.getElementById("MainContent_InputMileage").value == "")
        outputString += "Milage|";

    return outputString == "" ? outputString : outputString.substring(0, outputString.length - 1);
}

function escapeQuotes(item) {
    var nitem = item.replaceAll("'", "&apos;").replaceAll("\"", "&quot;");
    return nitem;
}