function LotLocationChange(value) {
    var lot = document.getElementById('MainContent_InvLotLocation');
    lot.value = value;
}

function GridPopup(URL, Delay) {
    window.setTimeout("window.open('" + URL + "');", Delay);
}

function VehicleTypeUpdateModal() {
    var answer = confirm("Changing the vehicle class will cause any changes you have made to be discarded.\nDo you wish to continue?");
    if (!answer)
        return false;
    toggleCssClass([['changePrompt', 'show_display']]);
    return false;
}

function SelectionSet(self) {
    var index = self.selectedIndex == -1 ? 0 : self.selectedIndex;
    if (/DriveTrainLst/g.test(self.id)) {
        document.getElementById("MainContent_kDrive").value = index != 0 ? self.selectedItem.Value : "0";
    }
    else if (/RoofLst/g.test(self.id)) {
        document.getElementById("MainContent_kRoof").value = index != 0 ? self.selectedItem.Value : "0";
    }
    else if (/DoorLst/g.test(self.id)) {
        document.getElementById("MainContent_kDoor").value = index != 0 ? self.selectedItem.Value : "0";
    }
    else if (/lstCertificationType/g.test(self.id)) {
        document.getElementById("MainContent_kCertificationType").value = index != 0 ? self.options[self.selectedIndex].value : "68";
    }
    else if (/VehicleListStatusLst/g.test(self.id)) {
        document.getElementById("MainContent_kVehicleListStatus").value = index != 0 ? self.options[self.selectedIndex].value : "32";
    }
}

function CertCheck(self) {
    var index = self.selectedIndex == -1 ? 0 : self.selectedIndex;
    var isDisabled = true;
    if (index != 0)
        isDisabled = false;

    document.getElementById("MainContent_lstCertificationType").disabled = isDisabled;
    document.getElementById("MainContent_CertificationNumber").disabled = isDisabled;
    //document.getElementById("MainContent_CertificationDate").disabled = isDisabled;
}

function VehicleTypeUpdate() {
    var info = {
        kListing: document.getElementById("MainContent_HeaderID").innerHTML,
        kVehicleClass: document.getElementById("MainContent_vehicleClassLst").value
    };

    toggleLoading(true, "");

    $.ajax({
        type: "POST",
        url: 'Update.aspx/VehicleClassUpdate',
        data: "{'data': '" + JSON.stringify(info) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                toggleCssClass([['changePrompt', 'show_display']]);
                toggleLoading(false, r.message);
                location.reload(true);
            } else {
                alert(r.message);
            }

            return false;
        }
    });
}

function intParse(input) {
    var val = document.getElementById(input);
    if (val != null) {
        num = parseInt(val.value.replace(/,/g, ''), 10);
        return !isNaN(num) ? num : 0;
    }
    return 0;
}

function buildOptionList() {
    var optionString = "";
    for (var i = 0; i < document.getElementById("MainContent_CommonOptions").children.length; i++) {
        for (var j = 0; j < document.getElementById("MainContent_CommonOptions").children[i].children.length; j++) {
            var elem = document.getElementById("MainContent_CommonOptions").children[i].children[j].children[0].children[0];
            optionString += elem.checked ? elem.value + "|" : "";
        }
    }
    for (var i = 0; i < document.getElementById("MainContent_VinExplosionoptions").children.length; i++) {
        for (var j = 0; j < document.getElementById("MainContent_VinExplosionoptions").children[i].children.length; j++) {
            var elem = document.getElementById("MainContent_VinExplosionoptions").children[i].children[j].children[0].children[0];
            optionString += elem.checked ? elem.value + "|" : "";
        }
    }
    for (var i = 0; i < document.getElementById("MainContent_ImportedOptions").children.length; i++) {
        for (var j = 0; j < document.getElementById("MainContent_ImportedOptions").children[i].children.length; j++) {
            var elem = document.getElementById("MainContent_ImportedOptions").children[i].children[j].children[0].children[0];
            optionString += elem.checked ? elem.value + "|" : "";
        }
    }
    document.getElementById("MainContent_optionsLst").value = optionString.substring(0, optionString.length - 1);

    toggleLoading(true, "");
}

function VehicleNoteSet() {
    var note = {
        kVehicleNoteId: 0,
        kListing: document.getElementById("MainContent_HeaderID").innerHTML,
        noteString: document.getElementById("MainContent_VehicleNotesText").value
    };

    $.ajax({
        type: "POST",
        url: 'Update.aspx/VehicleNoteSet',
        data: "{'note': '" + JSON.stringify(note) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r) {
                alert("Vehicle Note Saved!");
                toggleCssClass([['AddNote', 'show_display']]);
                location.reload(true);
            }
        }
    });
}

function GoToVehicleNotes() {
    var kListing = document.getElementById("MainContent_HeaderID").innerHTML;
    window.location.href = `/WholesaleContent/Vehicle/ViewNotes.aspx?kListing=${kListing}`;
}

function AddFeature(option) {
    var makeList = document.getElementById("MainContent_lstAddMake");
    var modelList = document.getElementById('MainContent_lstAddModel');
    var data = {
        op: option,
        kMake: makeList.value,
        Make: document.getElementById("MainContent_typeAddMake").value,
        Model: document.getElementById("MainContent_typeAddModel").value
    };

    if (option == "Make") {
        data["kMake"] = 0;
        data["Model"] = "";
    }
    else
        data["Make"] = "";

    $.ajax({
        type: "POST",
        url: 'Update.aspx/AddMakeModel',
        data: `{ 'data' : '${JSON.stringify(data)}' }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                if (option == "Make") {
                    alert(r.message);
                    makeList.options.length = 0;
                    BuildDropdownList(r.value, makeList);

                    for (var i = 0; i < makeList.options.length; i++) {
                        if (makeList.options[i].text == document.getElementById("MainContent_typeAddMake").value.toUpperCase()) {
                            makeList.options[i].selected = true;
                            break;
                        }
                    }

                    // Populate the next one to streamline process
                    MakeSelection(makeList.options[makeList.selectedIndex].value, "lstAddModel");
                } else {
                    alert(r.message.replace('REPLACE_MAKE', makeList[makeList.selectedIndex].text));
                    modelList.options.length = 0;
                    BuildDropdownList(r.value, modelList);
                    //toggleCssClass([['addMakeModel', 'show_display']]);
                }
            } else
                alert(r.message)

            return false;
        }
    });
}

function MakeSelection(kMake, sender) {
    var modelList = document.getElementById(`MainContent_${sender}`);
    modelList.options.length = 0;

    var data = {
        type: 'Model',
        make: kMake
    };

    $.ajax({
        type: "POST",
        url: 'Update.aspx/GetModelList',
        data: `{ 'json': '${JSON.stringify(data)}' }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                BuildDropdownList(r.value, modelList);
            }
        }
    });
}

function ClearModal() {
    document.getElementById('MainContent_lstAddMake').selectedIndex = 0;
    document.getElementById('MainContent_lstAddModel').options.length = 0;
    toggleCssClass([['addMakeModel', 'show_display']]);
}

function centerWindow(windowname) {
    var pe = document.getElementById(windowname);
    if (pe) {
        $("#" + windowname).css({
            position: 'absolute',
            top: '30%',
            left: '25%',
            width: '50%',
            height: '50%'
        });
    }
}

function showWindow(windowname, show) {
    var pe = document.getElementById(windowname);
    if (pe) {
        pe.style.display = (show) ? "block" : "none";
        var popbkg = document.getElementById("pop_overlay");
        if (popbkg) {
            if (show) {
                popbkg.style.visibility = "visible";
            } else {
                popbkg.style.visibility = "hidden";
            }
        }
    }
}

function OptionsSelect() {
    var makeLst = document.getElementById("MainContent_MakeLst");
    var modelLst = document.getElementById("MainContent_ModelLst");
    var styleLst = document.getElementById("MainContent_StyleLst");
    if (makeLst != null && makeLst.selectedIndex == 0 && !makeLst.classList.contains("hide")) {
        alert("Please select a Make!");
        return false;
    }
    else if (modelLst != null && modelLst.selectedIndex == 0 && !modelLst.classList.contains("hide")) {
        alert("Please select a Model!");
        return false;
    }
    else if (styleLst != null && styleLst.selectedIndex == 0 && !styleLst.classList.contains("hide")) {
        alert("Please select a Style!");
        return false;
    }
    CSSSelector('options');
}

function CSSSelector(tab) {
    document.getElementById("MainContent_divGeneral").style.display = (tab == "general" ? "flex" : "none");
    document.getElementById("MainContent_divListingInfo").style.display = (tab == "listing" ? "flex" : "none");
    document.getElementById("divPricing").style.display = (tab == "pricing" ? "flex" : "none");
    document.getElementById("divVehicleNotes").style.display = (tab == "notes" ? "flex" : "none");
    document.getElementById("divDescCond").style.display = (tab == "descCond" ? "flex" : "none");
    document.getElementById("divColorBody").style.display = (tab == "colorBody" ? "flex" : "none");
    document.getElementById("divDriveTrain").style.display = (tab == "driveTrain" ? "flex" : "none");
    document.getElementById("divOptions").style.display = (tab == "options" ? "flex" : "none");
}

function PhotoPopup(kListing) {
    gtag('config', 'G-3YVJ07S2NS',
        { 'user_id': document.getElementById("gtagkPerson").value, 'user_properties': { 'UserID': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value } });
    gtag('event', 'user_upload_photos')

    var popup = window.open(`/WholesaleData/UploadPhotos.aspx?kListing=${kListing}`, '_blank', 'popup,height=625,width=850');
    var timer = setInterval(function () {
        if (popup.closed) {
            clearInterval(timer);
            window.location.reload(true);
        }
    }, 1000);
}

function GetList(options, sender) {
    if (sender == "Year") {
        var dataIn = {
            year: options[options.selectedIndex].text,
            type: sender
        };

        if (options.selectedIndex == 0) {
            // Text
            document.getElementById("MainContent_MakeTxt").style.display = "initial";
            document.getElementById("MainContent_ModelTxt").style.display = "initial";
            document.getElementById("MainContent_StyleTxt").style.display = "initial";

            document.getElementById("MainContent_MakeTxt").innerText = "-- No Make Found --";
            document.getElementById("MainContent_ModelTxt").innerText = "-- No Model Found --";
            document.getElementById("MainContent_StyleTxt").innerText = "-- No Style Found --";

            // Lists
            document.getElementById("MainContent_MakeLst").style.display = "none";
            document.getElementById("MainContent_ModelLst").style.display = "none";
            document.getElementById("MainContent_StyleLst").style.display = "none";

            document.getElementById("MainContent_MakeLst").disabled = true;
            return false;
        }
        else {
            // Clear out any previous selections
            document.getElementById("MainContent_MakeLst").disabled = false;
            document.getElementById("MainContent_ModelTxt").innerText = "-- No Model Found --";
            document.getElementById("MainContent_StyleTxt").innerText = "-- No Style Found --";
            document.getElementById("MainContent_ModelTxt").style.display = "initial";
            document.getElementById("MainContent_StyleTxt").style.display = "initial";

            document.getElementById("MainContent_ModelLst") == null ? "" : document.getElementById("MainContent_ModelLst").style.display = "none";
            document.getElementById("MainContent_StyleLst") == null ? "" : document.getElementById("MainContent_StyleLst").style.display = "none";
        }

    }
    else if (sender == "Make") {
        var dataIn = {
            year: "",
            make: "",
            type: "Model"
        };

        // Year
        var year = document.getElementById('MainContent_YearLst');
        dataIn["year"] = year.options[year.selectedIndex].text;

        // Make
        var make = document.getElementById('MainContent_MakeLst');
        dataIn["make"] = make.options[make.selectedIndex].text;

        toggleLoading(true, `Getting Model info for ${dataIn["make"]}...`);
    }
    else if (sender == "Model") {
        var dataIn = {
            year: "",
            make: "",
            model: options[options.selectedIndex].text,
            type: "Style"
        };

        // Year
        var year = document.getElementById('MainContent_YearLst');
        dataIn["year"] = year.options[year.selectedIndex].text;

        // Make
        var make = document.getElementById('MainContent_MakeLst');
        dataIn["make"] = make.options[make.selectedIndex].text;
    }

    $.ajax({
        type:'POST',
        url: 'Update.aspx/GetList',
        data: `{ 'json': '${JSON.stringify(dataIn)}' }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;

            if (r.success) {

                if (sender == "Year") {
                    var makeList = document.getElementById('MainContent_MakeLst');
                    if (makeList.length != 0) {
                        makeList.length = 0;
                        document.getElementById("MainContent_MakeLst").style.display = "initial";
                        document.getElementById("MainContent_MakeTxt") == null ? "" : document.getElementById("MainContent_MakeTxt").style.display = "none";
                    }

                    BuildDropdownList(r.value, makeList);

                }
                else if (sender == "Make") {
                    var modelLst = document.getElementById(`MainContent_ModelLst`);
                    modelLst == null ? "" : modelLst.options.length = 0;

                    if (BuildDropdownList(r.value, modelLst)) {
                        toggleLoading(false, "Setting up Model info...");
                        modelLst.style.display = "initial";
                        document.getElementById("MainContent_ModelTxt").style.display = "none";
                    }
                }
                else if (sender == "Model") {
                    var styleList = document.getElementById('MainContent_StyleLst');

                    if (styleList.length != 0)
                        styleList.length = 0;

                    if (BuildDropdownList(r.value, styleList)) {
                        styleList.style.display = "initial";
                        document.getElementById("MainContent_StyleTxt").style.display = "none";
                    }
                }
            }
            else {
                alert(r.message);
            }
        }
    });
}

function StyleSelection(options) {
    var text = options[options.selectedIndex].text;

    // Check for 4DR or any DR
    var numbers = "123456789";
    if (numbers.indexOf(text.substring(0, 1)) > -1) {
        if (text.substring(1, 3).toUpperCase().localeCompare("DR") == 0) {
            text = text.substring(4).trim();
        }
    }

    // Chop the string to a bite size
    document.getElementById("MainContent_FullDesc").value = text;
    while (text.length > 12) {
        var pos = text.lastIndexOf(" ");
        if (pos > -1)
            text = text.substring(0, pos);
        else
            text = "";
    }
    document.getElementById("MainContent_LimitDesc").value = text;
}

function chkOverride(option) {
    // Need to figure out override only
    if (option == "style") {
        document.getElementById("MainContent_FullDesc").disabled = document.getElementById("MainContent_FullDesc").disabled == false ? true : false;
        document.getElementById("MainContent_LimitDesc").disabled = document.getElementById("MainContent_LimitDesc").disabled == false ? true : false;
        document.getElementById("MainContent_StyleLst").disabled = document.getElementById("MainContent_StyleLst").disabled == false ? true : false;
    }
    else {
        document.getElementById("MainContent_OverrideEngineDesc").disabled = document.getElementById("MainContent_OverrideEngineDesc").disabled == false ? true : false;
    }
}

function BuildDropdownList(stringList, htmlElement) {
    var list = [];
    if (stringList == "[]:|") {
        stringList = ":|";
    }

    var options = stringList.split('|');
    for (var i = 0; i < options.length; i++) {
        var option = options[i].split(':');
        if (option == "")
            break;
        var el = document.createElement("option");
        el.text = option[1];
        el.value = option[0];
        list.push(el);
    }

    list.forEach(l => htmlElement.appendChild(l));
    if (options.length < 1)
        return false;
    return true;
}

function GetDropdownValue(id) {
    var item = document.getElementById(id);
    return (item.selectedIndex == 0 || item.value == "") ? 0 : item.value;
}

function ToggleOptionsPane(idName) {
    var item = document.getElementById(idName);
    item.classList.toggle("closeOptions");
    var index = -1;
    for (let i = 0; i < item.parentNode.children.length; i++) {
        if (item.parentNode.children[i].id.includes(idName)) {
            index = i;
            break;
        }
    }
    var headerItem = item.parentNode.children[index - 1];
    if (item.classList.contains("closeOptions")) {
        if (!headerItem.innerHTML.includes(" 0 available"))
            headerItem.innerHTML = headerItem.innerHTML.substring(0, headerItem.innerHTML.length - 5) + "&#9660;</b>";
    }
    else if (!item.classList.contains("closeOptions")) {
        if (!headerItem.innerHTML.includes(" 0 available"))
            headerItem.innerHTML = headerItem.innerHTML.substring(0, headerItem.innerHTML.length - 5) + "&#9650;</b>";
    }
}

//$(document).keypress(function (e) {
//    if (e.which == 13 && document.activeElement.id == "MainContent_SavePass") {
//        DetailsSave();
//        return false;
//    }
//});

$(document).ready(function () {
    if (IsLiquidConnect()) {
        document.getElementById("sidebar_menu").style.display = 'none';
        document.getElementsByClassName("updateVehicle")[0].style.paddingLeft = '15px';
        document.getElementById("vehicleHeaderInfo").children[0].style.paddingLeft = "0px" 
    }
});