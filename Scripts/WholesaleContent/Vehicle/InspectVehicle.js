async function ButtonAction(mode, source, rowNum) {
    if (mode == "submit") {
        if (source == "main") {
            if (Validate()) {
                toggleLoading(true, "");
                SavePhotos().done(function () {
                    var dataSet = BuildDataSet();
                    $.ajax({
                        type: "POST",
                        url: "InspectVehicle.aspx/SaveVehicleDamages",
                        data: "{'jsonData': '" + JSON.stringify(dataSet.slice(1, 3), ["Damage", "Condition", "Severity", "Description", "regionText", "neededAction", "PreviousInfo", "PhotoInfo", "PhotoFileName"]) + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        error: function (XMLHttpRequest) {
                            alert(`Error on saving Vehicle Damages: ${XMLHttpRequest.responseJSON.Message}`);
                        },
                        complete: function (response) {
                            if (response.responseJSON.d.successOrFail) {
                                $.ajax({
                                    type: "POST",
                                    url: 'InspectVehicle.aspx/SavePaintDamages',
                                    data: "{'jsonData': '" + (JSON.stringify(dataSet[3], ["Damage", "Condition", "neededAction", "PreviousInfo"]) ?? "[]") + "'}",
                                    contentType: 'application/json; charset=utf-8',
                                    dataType: 'json',
                                    error: function (XMLHttpRequest) {
                                        alert(`Error on saving Paint Damages: ${XMLHttpRequest.responseJSON.Message}`);
                                    },
                                    complete: function (response) {
                                        if (response.responseJSON.d.successOrFail) {
                                            $.ajax({
                                                type: "POST",
                                                url: 'InspectVehicle.aspx/SaveInspection',
                                                data: "{'jsonData': '" + JSON.stringify(dataSet[0]) + "'}",
                                                contentType: 'application/json; charset=utf-8',
                                                dataType: 'json',
                                                error: function (XMLHttpRequest) {
                                                    alert(`Error on saving Inspection: ${XMLHttpRequest.responseJSON.Message}`);
                                                },
                                                complete: function (response) {
                                                    if (response.responseJSON.d.successOrFail) {
                                                        toggleLoading(false, "Save Complete. Rediecting to listing.");
                                                        var kListing = document.getElementById("MainContent_kListing").value;
                                                        window.location.href = `/WholesaleContent/Vehicle/update.aspx?kListing=${kListing}`;
                                                    }
                                                    else {
                                                        toggleLoading(false, "");
                                                        alert("Saving inspection data failed");
                                                    }
                                                    return false;
                                                }
                                            });
                                        }
                                        else {
                                            toggleLoading(false, "");
                                            alert("Saving prior paint failed");
                                        }
                                    }
                                });
                            }
                            else {
                                toggleLoading(false, "");
                                alert("Saving damages failed");
                            }
                        }
                    });
                });
            }
        }
        else if (source == "extAdd")
            AddNewJSGridRow("#jsGridExtDamage","ext");
        else if (source == "intAdd")
            AddNewJSGridRow("#jsGridIntDamage","int");
        else if (source == "paintAdd")
            AddNewJSGridRow("#jsGridPaint", "paint");
        else if (source == "extEdit")
            EditJSGridRow("#jsGridExtDamage", "ext", document.getElementById("MainContent_DamageRowId").value);
        else if (source == "intEdit")
            EditJSGridRow("#jsGridIntDamage", "int", document.getElementById("MainContent_DamageRowId").value);
        else if (source == "paintEdit")
            EditJSGridRow("#jsGridPaint", "paint", document.getElementById("MainContent_DamageRowId").value);
    }
    else if (mode == "cancel") {
        if (source == "main") {
            history.back();
        }
        else if (source == "ext")
            showWindow("ExtAddContainer", false);
        else if (source == "int")
            showWindow("IntAddContainer", false);
        else if (source == "paint") {
            showWindow("PaintAddContainer", false);
            if (document.getElementById("MainContent_hfPPUndoOnCancel").value == "1") {
                document.getElementById("MainContent_cbPriorPaint").checked = false;
                document.getElementById("MainContent_hfPPUndoOnCancel").value = 0;
            }
        }
    }
    else if (mode == "edit") {
        fnShowHideSubmit(mode.charAt(0).toUpperCase() + mode.slice(1), source.charAt(0).toUpperCase() + source.slice(1));
        document.getElementById("MainContent_DamageRowId").value = rowNum;
        if (source == "ext")
            fnProcessExteriorDamage("edit", rowNum);
        else if (source == "int")
            fnProcessInteriorDamage("edit", rowNum);
        else if (source == "paint")
            fnProcessPriorPaint("edit", rowNum);
    }
    else if (mode == "delete") {
        if (source == "ext") {
            if (confirm("Are you sure you wish to remove this exterior damage item?"))
                DeleteJSGridRow("#jsGridExtDamage", rowNum);
        }
        else if (source == "int") {
            if (confirm("Are you sure you wish to remove this interior damage item?"))
                DeleteJSGridRow("#jsGridIntDamage", rowNum);
        }
        else if (source == "paint") {
            if (confirm("Are you sure you wish to remove this prior paint item?"))
                DeleteJSGridRow("#jsGridPaint", rowNum);
        }
    }
    else if (mode == "add") {
        fnShowHideSubmit(mode.charAt(0).toUpperCase() + mode.slice(1), source.charAt(0).toUpperCase() + source.slice(1));
        if (source == "ext")
            fnProcessExteriorDamage("add");
        else if (source == "int")
            fnProcessInteriorDamage("add");
        else if (source == "paint")
            fnProcessPriorPaint("add");
    }
}

function CSSSelector(mode) {
    if (mode != "general") 
        $("#GVInfoContainer")[0].style.display = "none";
    if (mode != "TW") 
        $("#TWContainer")[0].style.display = "none";
    if (mode != "exterior")
        $("#ExtContainer")[0].style.display = "none";
    if (mode != "interior") 
        $("#IntContainer")[0].style.display = "none";
    if (mode != "paint") 
        $("#PaintContainer")[0].style.display = "none";
    

    if (mode == "general") 
        $("#GVInfoContainer")[0].style.display = "";
    else if (mode == "TW") 
        $("#TWContainer")[0].style.display = "";
    else if (mode == "exterior") 
        $("#ExtContainer")[0].style.display = "";
    else if (mode == "interior") 
        $("#IntContainer")[0].style.display = "";
    else if (mode == "paint") 
        $("#PaintContainer")[0].style.display = "";
}

function fnProcessExteriorDamage(mode, rowNum) {
    if (mode == "add") {
        // Clear the data in case there was a prior edit
        document.getElementById("ExtInfoBlock").style.display = "none";
        document.getElementById("MainContent_ExteriorOutline").style.display = "block";
        document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex = 0;
        document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex = 0;
        document.getElementById("MainContent_lstExteriorDamageSeverity").selectedIndex = 0;
        document.getElementById("MainContent_tbExteriorDamageDescription").value = "";
        document.getElementById("inExtPhoto").value = "";
        document.getElementById("ExtPhotoSelector").style.display = "table-row"
        document.getElementById("ExtPhotoDisplay").style.display = "none"
    }
    else if (mode == "edit") {
        document.getElementById("ExtInfoBlock").style.display = "table";
        document.getElementById("MainContent_ExteriorOutline").style.display = "none";
        var gridData = $("#jsGridExtDamage").data("JSGrid").data[rowNum];
        for (let i = 0; i < document.getElementById("MainContent_lstExteriorDamageCategory").children.length; i++) {
            if (document.getElementById("MainContent_lstExteriorDamageCategory").children[i].textContent == gridData["Damage"]) {
                document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex = i;
                ApplyConditionFilters('e', 'A');
                for (let i = 0; i < document.getElementById("MainContent_lstExteriorDamageCondition").children.length; i++) {
                    if (document.getElementById("MainContent_lstExteriorDamageCondition").children[i].textContent == gridData["Condition"]) {
                        document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex = i;
                        ApplyConditionFilters('e', 'D');
                        for (let i = 0; i < document.getElementById("MainContent_lstExteriorDamageSeverity").children.length; i++) {
                            if (document.getElementById("MainContent_lstExteriorDamageSeverity").children[i].textContent == gridData["Severity"]) {
                                document.getElementById("MainContent_lstExteriorDamageSeverity").selectedIndex = i;
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }
        document.getElementById("MainContent_tbExteriorDamageDescription").value = gridData["Description"];
        if (gridData["PhotoInfo"] == "Existing Photo") {
            document.getElementById("ExtPhotoSelector").style.display = "none"
            document.getElementById("ExtPhotoDisplay").style.display = "table-row"
            document.getElementById("inExtPhotoExisting").src = gridData["DamagePhoto"].substring(10, gridData["DamagePhoto"].length - 4)
        }
        else {
            document.getElementById("inExtPhoto").value = "";
            document.getElementById("ExtPhotoSelector").style.display = "table-row"
            document.getElementById("ExtPhotoDisplay").style.display = "none"
        }
    }

    showWindow('ExtAddContainer', true);
    //centerWindow('ExtAddContainer');
}

function fnProcessInteriorDamage(mode, rowNum) {
    if (mode == "add") {
        // Clear the data in case there was a prior edit
        document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex = 0;
        document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex = 0;
        document.getElementById("MainContent_lstInteriorDamageSeverity").selectedIndex = 0;
        document.getElementById("MainContent_tbInteriorDamageDescription").value = "";
        document.getElementById("inIntPhoto").value = "";
        document.getElementById("IntPhotoSelector").style.display = "table-row"
        document.getElementById("IntPhotoDisplay").style.display = "none"
    }
    else if (mode == "edit") {
        var gridData = $("#jsGridIntDamage").data("JSGrid").data[rowNum];

        for (let i = 0; i < document.getElementById("MainContent_lstInteriorDamageCategory").children.length; i++) {
            if (document.getElementById("MainContent_lstInteriorDamageCategory").children[i].textContent == gridData["Damage"]) {
                document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex = i;
                ApplyConditionFilters('i', 'A');
                for (let i = 0; i < document.getElementById("MainContent_lstInteriorDamageCondition").children.length; i++) {
                    if (document.getElementById("MainContent_lstInteriorDamageCondition").children[i].textContent == gridData["Condition"]) {
                        document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex = i;
                        ApplyConditionFilters('i', 'D');
                        for (let i = 0; i < document.getElementById("MainContent_lstInteriorDamageSeverity").children.length; i++) {
                            if (document.getElementById("MainContent_lstInteriorDamageSeverity").children[i].textContent == gridData["Severity"]) {
                                document.getElementById("MainContent_lstInteriorDamageSeverity").selectedIndex = i;
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }
        document.getElementById("MainContent_tbInteriorDamageDescription").value = gridData["Description"];
        if (gridData["PhotoInfo"] == "Existing Photo") {
            document.getElementById("IntPhotoSelector").style.display = "none"
            document.getElementById("IntPhotoDisplay").style.display = "table-row"
            document.getElementById("inIntPhotoExisting").src = gridData["DamagePhoto"].substring(10, gridData["DamagePhoto"].length - 4)
        }
        else {
            document.getElementById("inIntPhoto").value = "";
            document.getElementById("IntPhotoSelector").style.display = "table-row"
            document.getElementById("IntPhotoDisplay").style.display = "none"
        }
    }

    showWindow('IntAddContainer', true);
    //centerWindow('IntAddContainer');
}

function fnProcessPriorPaint(mode, rowNum) {
    // check prior paint, ask if necessary
    var cbPriorPaint = document.getElementById("MainContent_cbPriorPaint");
    var bchecked = cbPriorPaint.checked;
    if (cbPriorPaint.checked == false) {
        if (confirm("You did not check that the vehicle has prior paint on the General tab.  Does the vehicle have prior paint work?")) {
            cbPriorPaint.checked = true;
            bchecked = true;

            var cUndo = document.getElementById("MainContent_hfPPUndoOnCancel");

            if (cUndo) {
                cUndo.value = 1;
            }
        }
    }
    if (bchecked == true) {
        currentItem = null;
        backupItem = null;

        if (mode == "add") {
            // Clear the data in case there was a prior edit
            document.getElementById("PaintInfoBlock").style.display = "none";
            document.getElementById("MainContent_PaintOutline").style.display = "block";
            document.getElementById("MainContent_lstPriorPaintCategory").value = "";
            document.getElementById("MainContent_lstPriorPaintCondition").value = "";
        }
        else if (mode == "edit") {
            var gridData = $("#jsGridPaint").data("JSGrid").data[rowNum];
            for (let i = 0; i < document.getElementById("MainContent_lstPriorPaintCategory").children.length; i++) {
                if (document.getElementById("MainContent_lstPriorPaintCategory").children[i].textContent == gridData["Damage"]) {
                    document.getElementById("MainContent_lstPriorPaintCategory").selectedIndex = i;
                    for (let i = 0; i < document.getElementById("MainContent_lstPriorPaintCondition").children.length; i++) {
                        if (document.getElementById("MainContent_lstPriorPaintCondition").children[i].textContent == gridData["Condition"]) {
                            document.getElementById("MainContent_lstPriorPaintCondition").selectedIndex = i;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        showWindow('PaintAddContainer', true);
        //centerWindow('PaintAddContainer');
    }
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

function GridRowSelected(item) {
    $('#MainContent_DamageRowId')[0].value = item[0].rowIndex;
}

function RowDoubleClickExt() {
    ButtonAction("edit", "ext", $('#MainContent_DamageRowId')[0].value)
}

function RowDoubleClickInt() {
    ButtonAction("edit", "int", $('#MainContent_DamageRowId')[0].value)
}

function RowDoubleClickPaint() {
    ButtonAction("edit", "paint", $('#MainContent_DamageRowId')[0].value)
}

function ClearRowSelection() {
    $('#MainContent_txtVehicle').val("");
}

function ApplyConditionFilters(extOrInt, catOrCond) {
    if (document.getElementById("MainContent_hfHasAutoGrade").value == "0")
        return false;

    var field = extOrInt == 'e' ? "ExteriorDamage" : "InteriorDamage";;
    var mappings = JSON.parse(document.getElementById("MainContent_ExteriorInteriorMapping").value);
    var category = document.getElementById("MainContent_lst" + field + "Category").selectedIndex;
    if (category != 0) {
        if (catOrCond == "D") {
            var condition = document.getElementById("MainContent_lst" + field + "Condition").selectedIndex;
            for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Severity").children.length; i++) {
                document.getElementById("MainContent_lst" + field + "Severity").children[i].setAttribute("hidden", "hidden");
            }
            document.getElementById("MainContent_lst" + field + "Severity").selectedIndex = 0

            if (condition != 0) {
                var mappedItems = mappings[document.getElementById("MainContent_lst" + field + "Category").children[category].value][document.getElementById("MainContent_lst" + field + "Condition").children[condition].value];

                for (let j = 0; j < mappedItems.length; j++) {
                    for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Severity").children.length; i++) {
                        if (document.getElementById("MainContent_lst" + field + "Severity").children[i].value == mappedItems[j])
                            document.getElementById("MainContent_lst" + field + "Severity").children[i].removeAttribute("hidden");
                    }
                }
            }
        }
        else {
            for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Condition").children.length; i++) {
                document.getElementById("MainContent_lst" + field + "Condition").children[i].setAttribute("hidden", "hidden");
            }
            for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Severity").children.length; i++) {
                document.getElementById("MainContent_lst" + field + "Severity").children[i].setAttribute("hidden", "hidden");
            }
            document.getElementById("MainContent_lst" + field + "Condition").selectedIndex = 0
            document.getElementById("MainContent_lst" + field + "Severity").selectedIndex = 0

            for (var key in mappings[document.getElementById("MainContent_lst" + field + "Category").children[category].value]) {
                for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Condition").children.length; i++) {
                    if (document.getElementById("MainContent_lst" + field + "Condition").children[i].value == key)
                        document.getElementById("MainContent_lst" + field + "Condition").children[i].removeAttribute("hidden");
                }
            }
        }
    }
    else {
        for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Condition").children.length; i++) {
            document.getElementById("MainContent_lst" + field + "Condition").children[i].setAttribute("hidden", "hidden");
        }
        for (let i = 1; i < document.getElementById("MainContent_lst" + field + "Severity").children.length; i++) {
            document.getElementById("MainContent_lst" + field + "Severity").children[i].setAttribute("hidden", "hidden");
        }
        document.getElementById("MainContent_lst" + field + "Condition").selectedIndex = 0
        document.getElementById("MainContent_lst" + field + "Severity").selectedIndex = 0
    }
}

async function AddNewJSGridRow(element, source) {
    var addIndex = $(element).data("JSGrid").data.length ?? 0;
    var vehicleActions = "<div class='actionsBar'><div style=\"display: table; margin: auto;\"><div style=\"display: table-row\"> <input type=\"submit\" name=\"" + addIndex + "$submitButton\" value=\"Edit\" onclick=\"ButtonAction('edit', '" + source + "', '" + addIndex + "'); return false; \" id=\"MainContent_submitButton\" class=\"actionBackground SmallActionButton\"><input type=\"submit\" name=\"" + addIndex + "$submitButton\" value=\"Delete\" onclick=\"ButtonAction('delete', '" + source + "', '" + addIndex + "'); return false;\" id=\"MainContent_submitButton\" class=\"actionBackground SmallActionButton\" ></div></div ></div >";
    var errorString = "You must select a value for:\n";
    if (source == "ext") {
        var selectedCategory = document.getElementById("MainContent_lstExteriorDamageCategory").children[document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var selectedCondition = document.getElementById("MainContent_lstExteriorDamageCondition").children[document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }
        var newPhoto = document.getElementById("inExtPhoto").files.length > 0;
        var DamagePhotoVal = "No Photo";
        var PhotoInfoVal = "";
        var PhotoFileNameVal = "";
        var PhotoDataVal = "";

        if (newPhoto) {
            WaitForFileUpload = true;
            DamagePhotoVal = "No Preview Available";
            PhotoInfoVal = "New Photo";
            PhotoFileNameVal = document.getElementById("inExtPhoto").files[0].name;
            await fileToBase64(document.getElementById("inExtPhoto").files[0]).then((data) => { PhotoDataVal = data; });
        }

        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else if (selectedCondition.indexOf("Hail") >= 0 && !document.getElementById("MainContent_cbHailDamage").checked) {
            if (confirm("You did not check that the vehicle has Hail Damage on the General tab.  Does the vehicle have Hail Damage?")) {
                document.getElementById("MainContent_cbHailDamage").checked = true;
            }
        }
        else {
            var selectedSeverity = document.getElementById("MainContent_lstExteriorDamageSeverity").children[document.getElementById("MainContent_lstExteriorDamageSeverity").selectedIndex].textContent;
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value })) {
                if (Object.keys($(element).data("JSGrid").data).length > 0)
                    $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                else
                    $(element).data("JSGrid").data = [{ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal }];
                RefreshAndHide(element)
                showWindow("ExtAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value }))) {
                var matchingRow = $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value })]
                if (newPhoto) {
                    if (matchingRow.PhotoInfo == "Existing Photo") {
                        $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                    }
                    else {
                        matchingRow.PhotoFileName = PhotoFileNameVal;
                        matchingRow.PhotoData = PhotoDataVal;
                        matchingRow.PhotoInfo = PhotoInfoVal;
                        matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value });
                    }
                }
                else {
                    matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value });
                }
                RefreshAndHide(element)
                showWindow("ExtAddContainer", false);
            }
            else {
                alert("This data entry is a duplicate of an existing damage record and will not be added to the list.");
            }
        }
    }
    else if (source == "int") {
        var selectedCategory = document.getElementById("MainContent_lstInteriorDamageCategory").children[document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var selectedCondition = document.getElementById("MainContent_lstInteriorDamageCondition").children[document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }

        var newPhoto = document.getElementById("inIntPhoto").files.length > 0;
        var DamagePhotoVal = "No Photo";
        var PhotoInfoVal = "";
        var PhotoFileNameVal = "";
        var PhotoDataVal = "";

        if (newPhoto) {
            WaitForFileUpload = true;
            DamagePhotoVal = "No Preview Available";
            PhotoInfoVal = "New Photo";
            PhotoFileNameVal = document.getElementById("inIntPhoto").files[0].name;
            await fileToBase64(document.getElementById("inIntPhoto").files[0]).then((data) => { PhotoDataVal = data; });
        }


        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else {
            var selectedSeverity = document.getElementById("MainContent_lstInteriorDamageSeverity").children[document.getElementById("MainContent_lstInteriorDamageSeverity").selectedIndex].textContent;
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value })) {
                if (Object.keys($(element).data("JSGrid").data).length > 0)
                    $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Interior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                else
                    $(element).data("JSGrid").data = [{ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Interior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal }];
                RefreshAndHide(element)
                showWindow("IntAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value }))) {
                var matchingRow = $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value })]
                if (newPhoto) {
                    if (matchingRow.PhotoInfo == "Existing Photo") {
                        $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                    }
                    else {
                        matchingRow.PhotoFileName = PhotoFileNameVal;
                        matchingRow.PhotoData = PhotoDataVal;
                        matchingRow.PhotoInfo = PhotoInfoVal;
                        matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value });
                    }
                }
                else {
                    matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition, Severity: selectedSeverity, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value });
                }
                RefreshAndHide(element)
                showWindow("IntAddContainer", false);
            }
            else {
                alert("This data entry is a duplicate of an existing damage record and will not be added to the list.");
            }
        }
    }
    else if (source == "paint") {
        var selectedCategory = document.getElementById("MainContent_lstPriorPaintCategory").children[document.getElementById("MainContent_lstPriorPaintCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstPriorPaintCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var selectedCondition = document.getElementById("MainContent_lstPriorPaintCondition").children[document.getElementById("MainContent_lstPriorPaintCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstPriorPaintCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }
        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else {
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition })) {
                if (Object.keys($(element).data("JSGrid").data).length > 0)
                    $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, neededAction: "Save", PreviousInfo: "" });
                else
                    $(element).data("JSGrid").data = [{ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, neededAction: "Save", PreviousInfo: "" }];
                RefreshAndHide(element)
                showWindow("PaintAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition }))) {
                $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition })].neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: selectedCategory, Condition: selectedCondition });
                RefreshAndHide(element)
                showWindow("PaintAddContainer", false);
            }
            else {
                alert("This data entry is a duplicate of an existing damage record and will not be added to the list.");
            }
        }
    }
}

function DeleteJSGridRow(element, rowNum) {
    $(element).data("JSGrid").data[rowNum].neededAction = ($(element).data("JSGrid").data[rowNum].neededAction == "Save" ? "NewNone" : "Delete");
    RefreshAndHide(element)
}

async function EditJSGridRow(element, source, rowNum) {
    var gridData = $(element).data("JSGrid").data[rowNum]
    var errorString = "You must select a value for:\n";
    if (source == "ext") {
        var damage = document.getElementById("MainContent_lstExteriorDamageCategory").children[document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstExteriorDamageCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var condition = document.getElementById("MainContent_lstExteriorDamageCondition").children[document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstExteriorDamageCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }

        var newPhoto = document.getElementById("inExtPhoto").files.length > 0;
        var DamagePhotoVal = "No Photo";
        var PhotoInfoVal = "";
        var PhotoFileNameVal = "";
        var PhotoDataVal = "";

        if (newPhoto) {
            WaitForFileUpload = true;
            PhotoFileNameVal = "No Preview Available";
            PhotoInfoVal = "New Photo";
            PhotoFileNameVal = document.getElementById("inExtPhoto").files[0].name;
            await fileToBase64(document.getElementById("inExtPhoto").files[0]).then((data) => { PhotoDataVal = data; });
        }

        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else {
            var severity = document.getElementById("MainContent_lstExteriorDamageSeverity").children[document.getElementById("MainContent_lstExteriorDamageSeverity").selectedIndex].text;
            var description = document.getElementById("MainContent_tbExteriorDamageDescription").value;
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description }) ||
                (damage == gridData.Damage && condition == gridData.Condition && severity == gridData.Severity && description == gridData.Description)) {
                gridData.Damage = damage;
                gridData.Condition = condition;
                gridData.Severity = severity;
                gridData.Description = description;
                gridData.neededAction = (gridData.neededAction == "Save" ? "Save" : "Update");
                if (newPhoto) {
                    gridData.PhotoFileName = PhotoFileNameVal;
                    gridData.PhotoData = PhotoDataVal;
                    gridData.PhotoInfo = PhotoInfoVal;
                    gridData.DamagePhoto = "No Preview Available";
                }
                RefreshAndHide(element)
                showWindow("ExtAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description }))) {
                var matchingRow = $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description })]
                if (newPhoto) {
                    if (matchingRow.PhotoInfo == "Existing Photo") {
                        $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbExteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: PhotoFileNameVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                    }
                    else {
                        matchingRow.PhotoFileName = PhotoFileNameVal;
                        matchingRow.PhotoData = PhotoDataVal;
                        matchingRow.PhotoInfo = PhotoInfoVal;
                        matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description });
                    }
                }
                else {
                    matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description });
                }
                $(element).data("JSGrid").data[$('#MainContent_DamageRowId')[0].value].neededAction = "Delete";
                RefreshAndHide(element)
                showWindow("ExtAddContainer", false);
            }
            else {
                alert("The changes you have made to this data item make it a duplicate of an existing damage record. Please delete this row if it is unneeded.");
            }
        }
    }
    else if (source == "int") {
        var damage = document.getElementById("MainContent_lstInteriorDamageCategory").children[document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstInteriorDamageCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var condition = document.getElementById("MainContent_lstInteriorDamageCondition").children[document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstInteriorDamageCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }

        var newPhoto = document.getElementById("inIntPhoto").files.length > 0;
        var DamagePhotoVal = "No Photo";
        var PhotoInfoVal = "";
        var PhotoFileNameVal = "";
        var PhotoDataVal = "";

        if (newPhoto) {
            WaitForFileUpload = true;
            DamagePhotoVal = "No Preview Available";
            PhotoInfoVal = "New Photo";
            PhotoFileNameVal = document.getElementById("inIntPhoto").files[0].name;
            await fileToBase64(document.getElementById("inIntPhoto").files[0]).then((data) => { PhotoDataVal = data; });
        }

        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else {
            var severity = document.getElementById("MainContent_lstInteriorDamageSeverity").children[document.getElementById("MainContent_lstInteriorDamageSeverity").selectedIndex].text;
            var description = document.getElementById("MainContent_tbInteriorDamageDescription").value;
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description }) ||
                (damage == gridData.Damage && condition == gridData.Condition && severity == gridData.Severity && description == gridData.Description)) {
                gridData.Damage = damage;
                gridData.Condition = condition;
                gridData.Severity = severity;
                gridData.Description = description;
                gridData.neededAction = (gridData.neededAction == "Save" ? "Save" : "Update");
                if (newPhoto) {
                    gridData.PhotoFileName = PhotoFileNameVal;
                    gridData.PhotoData = PhotoDataVal;
                    gridData.PhotoInfo = PhotoInfoVal;
                    gridData.DamagePhoto = "No Preview Available";
                }
                RefreshAndHide(element)
                showWindow("IntAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description }))) {
                var matchingRow = $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description })]
                if (newPhoto) {
                    if (matchingRow.PhotoInfo == "Existing Photo") {
                        $(element).data("JSGrid").data.push({ Actions: vehicleActions, Condition: selectedCondition, Damage: selectedCategory, Description: document.getElementById("MainContent_tbInteriorDamageDescription").value, Severity: selectedSeverity, regionText: "Exterior", neededAction: "Save", PreviousInfo: "", DamagePhoto: DamagePhotoVal, PhotoInfo: PhotoInfoVal, PhotoFileName: PhotoFileNameVal, PhotoData: PhotoDataVal });
                    }
                    else {
                        matchingRow.PhotoFileName = PhotoFileNameVal;
                        matchingRow.PhotoData = PhotoDataVal;
                        matchingRow.PhotoInfo = PhotoInfoVal;
                        matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description });
                    }
                }
                else {
                    matchingRow.neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: damage, Condition: condition, Severity: severity, Description: description });
                }
                $(element).data("JSGrid").data[$('#MainContent_DamageRowId')[0].value].neededAction = "Delete";
                RefreshAndHide(element)
                showWindow("IntAddContainer", false);
            }
            else {
                alert("The changes you have made to this data item make it a duplicate of an existing damage record. Please delete this row if it is unneeded.");
            }
        }
    }
    else if (source == "paint") {
        var damage = document.getElementById("MainContent_lstPriorPaintCategory").children[document.getElementById("MainContent_lstPriorPaintCategory").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstPriorPaintCategory").selectedIndex == 0) {
            errorString += "\tCategory\n";
        }
        var condition = document.getElementById("MainContent_lstPriorPaintCondition").children[document.getElementById("MainContent_lstPriorPaintCondition").selectedIndex].textContent;
        if (document.getElementById("MainContent_lstPriorPaintCondition").selectedIndex == 0) {
            errorString += "\tCondition\n";
        }
        if (errorString.indexOf("Category") > 0 || errorString.indexOf("Condition") > 0) {
            alert(errorString);
        }
        else {
            if (!AlreadyInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition }) ||
                (damage == gridData.Damage && condition == gridData.Condition)) {
                gridData.Damage = damage;
                gridData.Condition = condition;
                gridData.neededAction = (gridData.neededAction == "Save" ? "Save" : "Update");
                RefreshAndHide(element)
                showWindow("PaintAddContainer", false);
            }
            else if (["Delete", "NewNone"].includes(GetModeInGrid($(element).data("JSGrid").data, { Damage: damage, Condition: condition }))) {
                $(element).data("JSGrid").data[GetMatchingRowNum($(element).data("JSGrid").data, { Damage: damage, Condition: condition })].neededAction = GetNeededState($(element).data("JSGrid").data, { Damage: damage, Condition: condition });
                $(element).data("JSGrid").data[$('#MainContent_DamageRowId')[0].value].neededAction = "Delete";
                RefreshAndHide(element)
                showWindow("PaintAddContainer", false);
            }
            else {
                alert("The changes you have made to this data item make it a duplicate of an existing damage record. Please delete this row if it is unneeded.");
            }
        }
    }
    
}

function fnShowHideSubmit(mode, source) {
    document.getElementById("MainContent_submitExtAddButton").style.display = "none";
    document.getElementById("MainContent_submitExtEditButton").style.display = "none";
    document.getElementById("MainContent_submitIntAddButton").style.display = "none";
    document.getElementById("MainContent_submitIntEditButton").style.display = "none";
    document.getElementById("MainContent_submitPaintAddButton").style.display = "none";
    document.getElementById("MainContent_submitPaintEditButton").style.display = "none";

    document.getElementById("MainContent_submit" + source + mode + "Button").style.display = '';
}

function AlreadyInGrid(gridData, newRow) {
    for (let i = 0; i < gridData.length; i++) {
        if (JSON.stringify(gridData[i], ["Damage", "Condition", "Severity", "Description"]) == JSON.stringify(newRow))
            return true;
    }
    return false;
}

function GetModeInGrid(gridData, newRow) {
    for (let i = 0; i < gridData.length; i++) {
        if (JSON.stringify(gridData[i], ["Damage", "Condition", "Severity", "Description"]) == JSON.stringify(newRow))
            return gridData[i]["neededAction"];
    }
    return false;
}

function GetMatchingRowNum(gridData, newRow) {
    for (let i = 0; i < gridData.length; i++) {
        if (JSON.stringify(gridData[i], ["Damage", "Condition", "Severity", "Description"]) == JSON.stringify(newRow))
            return i;
    }
    return false;
}

function ShowCurrentReport() {
    window.open(document.getElementById("MainContent_CurrentReportURL").value);
}

function Validate() {
    var errorHeader = "Some fields were in error. Please select a value for the following field(s): \n";
    var errorString = "Some fields were in error. Please select a value for the following field(s): \n";

    var ddlTitleStatus = document.getElementById("MainContent_lstTitleStatus")
    if (ddlTitleStatus.value == "0") {
        errorString += "You must select a Title Status.\r\n";
    }
    var ddlOdometerStatus = document.getElementById("MainContent_lstOdoStat")
    if (ddlOdometerStatus.value == "0") {
        errorString += "You must select an Odometer Status.\r\n";
    }

    var ddlRFTireCondition = document.getElementById("MainContent_lstRFCondition")
    if (ddlRFTireCondition.value == "") {
        errorString += "You must select a Right Front Tire Condition.\r\n";
    }
    else if (ddlRFTireCondition.value != "4" && ddlRFTireCondition.value != "0") {
        var ddlRFTireDepth = document.getElementById("MainContent_lstRFTD")
        if (ddlRFTireDepth.value == "-1") {
            errorString += "You must select a Right Front Tire Depth.\r\n";
        }
        var ddlRFTireMfgr = document.getElementById("MainContent_lstRFManufact")
        if (ddlRFTireMfgr.value == "") {
            errorString += "You must select a Right Front Tire Manufacturer.\r\n";
        }
        var ddlRFWheelSize = document.getElementById("MainContent_lstRFWheelSize")
        if (ddlRFWheelSize.value == "0") {
            errorString += "You must select a Right Front Wheel Size.\r\n";
        }
    }
    var ddlLFTireCondition = document.getElementById("MainContent_lstLFCondition")
    if (ddlLFTireCondition.value == "") {
        errorString += "You must select a Left Front Tire Condition.\r\n";
    }
    else if (ddlLFTireCondition.value != "4" && ddlLFTireCondition.value != "0") {
        var ddlLFTireDepth = document.getElementById("MainContent_lstLFTD")
        if (ddlLFTireDepth.value == "-1") {
            errorString += "You must select a Left Front Tire Depth.\r\n";
        }
        var ddlLFTireMfgr = document.getElementById("MainContent_lstLFManufact")
        if (ddlLFTireMfgr.value == "") {
            errorString += "You must select a Left Front Tire Manufacturer.\r\n";
        }
        var ddlLFWheelSize = document.getElementById("MainContent_lstLFWheelSize")
        if (ddlLFWheelSize.value == "0") {
            errorString += "You must select a Left Front Wheel Size.\r\n";
        }
    }
    var ddlRRTireCondition = document.getElementById("MainContent_lstRRCondition")
    if (ddlRRTireCondition.value == "") {
        errorString += "You must select a Right Rear Tire Condition.\r\n";
    }
    else if (ddlRRTireCondition.value != "4" && ddlRRTireCondition.value != "0") {
        var ddlRRTireDepth = document.getElementById("MainContent_lstRRTD")
        if (ddlRRTireDepth.value == "-1") {
            errorString += "You must select a Right Rear Tire Depth.\r\n";
        }
        var ddlRRTireMfgr = document.getElementById("MainContent_lstRRManufact")
        if (ddlRRTireMfgr.value == "") {
            errorString += "You must select a Right Rear Tire Manufacturer.\r\n";
        }
        var ddlRRWheelSize = document.getElementById("MainContent_lstRRWheelSize")
        if (ddlRRWheelSize.value == "0") {
            errorString += "You must select a Right Rear Wheel Size.\r\n";
        }
    }
    var ddlLRTireCondition = document.getElementById("MainContent_lstLRCondition")
    if (ddlLRTireCondition.value == "") {
        errorString += "You must select a Left Rear Tire Condition.\r\n";
    }
    else if (ddlLRTireCondition.value != "4" && ddlLRTireCondition.value != "0") {
        var ddlLRTireDepth = document.getElementById("MainContent_lstLRTD")
        if (ddlLRTireDepth.value == "-1") {
            errorString += "You must select a Left Rear Tire Depth.\r\n";
        }
        var ddlLRTireMfgr = document.getElementById("MainContent_lstLRManufact")
        if (ddlLRTireMfgr.value == "") {
            errorString += "You must select a Left Rear Tire Manufacturer.\r\n";
        }
        var ddlLRWheelSize = document.getElementById("MainContent_lstLRWheelSize")
        if (ddlLRWheelSize.value == "0") {
            errorString += "You must select a Left Rear Wheel Size.\r\n";
        }
    }

    if (errorHeader != errorString) {
        alert(errorString);
        return false;
    }

    var cMatch = document.getElementById("MainContent_cbAllTiresMatch");
    if (cMatch) {
        if (cMatch.disabled == false) {
            if (cMatch.checked == false) {
                if (!confirm("You have not checked the All Tires Match box. Do you wish to submit this vehicle with mismatched tires?")) {
                    return false;
                }
            }
        }
    }

    return true;
}

function BuildDataSet() {
    var dataSet = []
    var inspectionData = { FrameDamage: document.getElementById("MainContent_cbFrameDamage").checked ? "1" : "0" };
    inspectionData["Driveable"] = document.getElementById("MainContent_cbDriveable").checked ? "1":"0";
    inspectionData["FloodDamage"] = document.getElementById("MainContent_cbFloodDamage").checked ? "1":"0";
    inspectionData["TheftRecovery"] = document.getElementById("MainContent_cbTheft").checked ? "1":"0";
    inspectionData["FireDamage"] = document.getElementById("MainContent_cbFireDamage").checked ? "1":"0";
    inspectionData["PriorPaint"] = document.getElementById("MainContent_cbPriorPaint").checked ? "1":"0";
    inspectionData["AirbagDeployed"] = document.getElementById("MainContent_cbAirbagsDep").checked ? "1":"0";
    inspectionData["AirBagLight"] = document.getElementById("MainContent_cbAirbagLight").checked ? "1":"0";
    inspectionData["AirBagMissing"] = document.getElementById("MainContent_cbAirbagMiss").checked ? "1":"0";
    inspectionData["SmokerFlag"] = document.getElementById("MainContent_cbSmoker").checked ? "1":"0";
    inspectionData["CheckEngineLight"] = document.getElementById("MainContent_cbCEL").checked ? "1":"0";
    inspectionData["OtherOdor"] = document.getElementById("MainContent_cbOdor").checked ? "1":"0";
    inspectionData["PoliceUse"] = document.getElementById("MainContent_cbPolice").checked ? "1":"0";
    inspectionData["LiveryUse"] = document.getElementById("MainContent_cbLivery").checked ? "1":"0";
    inspectionData["GreyMarket"] = document.getElementById("MainContent_cbGreyMarket").checked ? "1":"0";
    inspectionData["TaxiUse"] = document.getElementById("MainContent_cbTaxi").checked ? "1":"0";
    inspectionData["HasManuals"] = document.getElementById("MainContent_cbManuals").checked ? "1":"0";
    inspectionData["CanadianVehicle"] = document.getElementById("MainContent_cbCanadian").checked ? "1":"0";
    inspectionData["Has5thWheel"] = document.getElementById("MainContent_cb5thWheel").checked ? "1":"0";
    inspectionData["VINPlateIssue"] = document.getElementById("MainContent_cbVinPlateIss").checked ? "1":"0";
    inspectionData["WarrantyCancelled"] = document.getElementById("MainContent_cbFactoryWarr").checked ? "1":"0";
    inspectionData["ExhaustAltered"] = document.getElementById("MainContent_cbAlteredEx").checked ? "1":"0";
    inspectionData["SuspensionAltered"] = document.getElementById("MainContent_cbAlteredSus").checked ? "1":"0";
    inspectionData["TPIPresent"] = document.getElementById("MainContent_cbTirePressure").checked ? "1":"0";
    inspectionData["LtdPowerTrainArb"] = "0"; // Default to no limited arb just in case it gets set for some reason
    inspectionData["HailDamage"] = document.getElementById("MainContent_cbHailDamage").checked ? "1" : "0";
    inspectionData["LicenseNumber"] = document.getElementById("MainContent_tbLicensePlate").value;
    inspectionData["TitleState"] = document.getElementById("MainContent_lstTitleState").value;
    inspectionData["kTitleStatus"] = document.getElementById("MainContent_lstTitleStatus").value;
    inspectionData["LicenseState"] = document.getElementById("MainContent_lstLPState").value;
    inspectionData["kWholesaleOdometer"] = document.getElementById("MainContent_lstOdoStat").value;
    inspectionData["kAudioType"] = document.getElementById("MainContent_lstAudio").value;
    inspectionData["Keys"] = document.getElementById("MainContent_lstNumKeys").value;
    inspectionData["kWholesaleInteriorType"] = document.getElementById("MainContent_lstIntType").value;
    inspectionData["kWholesaleVehicleType"] = document.getElementById("MainContent_lstVehicleType").value;
    inspectionData["KeyFobs"] = document.getElementById("MainContent_lstFobs").value;
    inspectionData["IndustryGrade"] = document.getElementById("MainContent_lstNAAAGrade").value;

    inspectionData["AllTiresMatch"] = document.getElementById("MainContent_cbAllTiresMatch").checked ? "1" : "0";
    inspectionData["LFTireCond"] = document.getElementById("MainContent_lstLFCondition").value;
    inspectionData["RFTireCond"] = document.getElementById("MainContent_lstRFCondition").value;
    inspectionData["RRTireCond"] = document.getElementById("MainContent_lstRRCondition").value;
    inspectionData["LRTireCond"] = document.getElementById("MainContent_lstLRCondition").value;
    inspectionData["RRInnerCond"] = document.getElementById("MainContent_lstRRICondition").value;
    inspectionData["LRInnerCond"] = document.getElementById("MainContent_lstLRICondition").value;
    inspectionData["SpareTireCond"] = document.getElementById("MainContent_lstSPRCondition").value;
    inspectionData["kLFTireMfgr"] = document.getElementById("MainContent_lstLFManufact").value;
    inspectionData["kRFTireMfgr"] = document.getElementById("MainContent_lstRFManufact").value;
    inspectionData["kRRTireMfgr"] = document.getElementById("MainContent_lstRRManufact").value;
    inspectionData["kLRTireMfgr"] = document.getElementById("MainContent_lstLRManufact").value;
    inspectionData["kRRInnerMfgr"] = document.getElementById("MainContent_lstRRIManufact").value;
    inspectionData["kLRInnerMfgr"] = document.getElementById("MainContent_lstLRIManufact").value;
    inspectionData["kSpareTireMfgr"] = document.getElementById("MainContent_lstSPRManufact").value;
    inspectionData["nLFTireDepth"] = document.getElementById("MainContent_lstLFTD").value;
    inspectionData["nRFTireDepth"] = document.getElementById("MainContent_lstRFTD").value;
    inspectionData["nRRTireDepth"] = document.getElementById("MainContent_lstRRTD").value;
    inspectionData["nLRTireDepth"] = document.getElementById("MainContent_lstLRTD").value;
    inspectionData["nRRInnerDepth"] = document.getElementById("MainContent_lstRRITD").value;
    inspectionData["nLRInnerDepth"] = document.getElementById("MainContent_lstLRITD").value;
    inspectionData["nSpareTireDepth"] = document.getElementById("MainContent_lstSPRTD").value;
    inspectionData["nLFTireSize"] = document.getElementById("MainContent_lstLFWheelSize").value;
    inspectionData["nRFTireSize"] = document.getElementById("MainContent_lstRFWheelSize").value;
    inspectionData["nRRTireSize"] = document.getElementById("MainContent_lstRRWheelSize").value;
    inspectionData["nLRTireSize"] = document.getElementById("MainContent_lstLRWheelSize").value;
    inspectionData["nRRInnerSize"] = document.getElementById("MainContent_lstRRIWheelSize").value;
    inspectionData["nLRInnerSize"] = document.getElementById("MainContent_lstLRIWheelSize").value;
    inspectionData["nSpareTireSize"] = document.getElementById("MainContent_lstSPRWheelSize").value;

    dataSet.push(inspectionData);

    var outArray = [];
    if (Object.keys($("#jsGridExtDamage").data("JSGrid").data).length > 0) {
        for (let i = 0; i < $("#jsGridExtDamage").data("JSGrid").data.length; i++) {
            var data = JSON.parse(JSON.stringify($("#jsGridExtDamage").data("JSGrid").data[i]));
            data.Severity = data.Severity.replaceAll("\"", "\\\"");
            data.PreviousInfo = escapeQuotes(data.PreviousInfo.replaceAll("\"", "\\\""));
            data.Description = escapeQuotes(data.Description.replaceAll("\"", "\\\""));
            outArray.push(data);
        }
        dataSet.push(outArray);
    }
    else {
        dataSet.push(outArray);
    }
    var outArray2 = [];
    if (Object.keys($("#jsGridIntDamage").data("JSGrid").data).length > 0) {
        var outArray = [];
        for (let i = 0; i < $("#jsGridIntDamage").data("JSGrid").data.length; i++) {
            var data = JSON.parse(JSON.stringify($("#jsGridIntDamage").data("JSGrid").data[i]));
            data.Severity = data.Severity.replaceAll("\"", "\\\"");
            data.PreviousInfo = escapeQuotes(data.PreviousInfo.replaceAll("\"", "\\\""));
            data.Description = escapeQuotes(data.Description.replaceAll("\"", "\\\""));
            outArray2.push(data);
        }
        dataSet.push(outArray2);
    }
    else {
        dataSet.push(outArray2);
    }
    if (Object.keys($("#jsGridPaint").data("JSGrid").data).length > 0) {
        dataSet.push($("#jsGridPaint").data("JSGrid").data);
    }
    else {
        dataSet.push([]);
    }

    return dataSet;
}

function PriorPaintChanged() {
    var cPriorPaint = document.getElementById("MainContent_cbPriorPaint");

    if (cPriorPaint) {
        if (cPriorPaint.checked == false) {
            if (Object.keys($("#jsGridPaint").data("JSGrid").data).length > 0) {
                if (!confirm("Clearing the Prior Paint checkbox will cause your paint damage entries to be erased.  Click OK to continue or Cancel to keep your paint damage entries.")) {
                    cPriorPaint.checked = true;
                }
                else {
                    $("#jsGridPaint").data("JSGrid").data = [];
                    RefreshAndHide("#jsGridPaint");
                }
            }
        }
    }
}

function HailDamageChanged() {
    var cHailDamage = document.getElementById("MainContent_cbHailDamage");

    if (cHailDamage) {
        if (cHailDamage.checked == false) {
            if (Object.keys($("#jsGridExtDamage").data("JSGrid").data).length > 0) {
                var hailDamageEntries = []
                for (let i = 0; i < $("#jsGridExtDamage").data("JSGrid").data.length; i++) {
                    if ($("#jsGridExtDamage").data("JSGrid").data[i].Condition == "Hail Damage")
                        hailDamageEntries.push(i);
                }
                if (hailDamageEntries.length > 0) {
                    if (!confirm("Clearing the Hail Damage checkbox will cause your exterior damage entries whith hail damage condition to be erased.  Click OK to continue or Cancel to keep your hail damage entries.")) {
                        cHailDamage.checked = true;
                    }
                    else {
                        for (let i = hailDamageEntries.length - 1; i >= 0; i--)
                            $("#jsGridExtDamage").data("JSGrid").data.splice(i, 1);
                        RefreshAndHide("#jsGridExtDamage");
                    }
                }
            }
        }
    }
}

function RefreshAndHide(element) {
    $(element).jsGrid("refresh");
    for (let i = 0; i < $(element).data("JSGrid").data.length; i++) {
        if ($(element).data("JSGrid").data[i].neededAction == "Delete" || $(element).data("JSGrid").data[i].neededAction == "NewNone")
            document.getElementById(element.substring(1)).children[1].children[0].children[0].children[i].style.display = 'none';
    }
}

function GetNeededState(gridData, newRow) {
    for (let i = 0; i < gridData.length; i++) {
        if (JSON.stringify(gridData[i], ["Damage", "Condition", "Severity", "Description"]) == JSON.stringify(newRow)) {
            if (gridData[i].PreviousInfo == '')
                return "Save";
            else {
                if (gridData[i].PreviousInfo == newRow.Damage + "|" + newRow.Condition + (newRow.Severity != null ? "|" + newRow.Severity + "|" + newRow.Description : ""))
                    return "None";
                else
                    return "Update";
            }
        }
    }
    return "None";
}

function escapeQuotes(item) {
    var nitem = item.replaceAll("'", "&apos;");
    return nitem;
}

function handleClick(event) {
    const rect = event.target.getBoundingClientRect();
    const x = parseInt(event.clientX - rect.left);
    const y = parseInt(event.clientY - rect.top);

    const displayedWidth = parseInt(event.target.clientWidth);
    const displayedHeight = parseInt(event.target.clientHeight);

    var pickerType = "";
    var dropDown = null;
    var datablock = null;

    const bodyType = document.getElementById("MainContent_hfBodyType").value

    if (event.target.id.includes("Exterior")) {
        pickerType = "extDam";
        dropDown = document.getElementById("MainContent_lstExteriorDamageCategory");
        datablock = document.getElementById("ExtInfoBlock")
    }
    else if (event.target.id.includes("Paint")) {
        pickerType = "paint"
        dropDown = document.getElementById("MainContent_lstPriorPaintCategory")
        datablock = document.getElementById("PaintInfoBlock")
    }

    $.ajax({
        type: "POST",
        url: 'InspectVehicle.aspx/GetMappingLocation',
        data: `{'bodyType': '${bodyType}', 'pickerType': '${pickerType}', 'height': '${displayedHeight}', 'width': '${displayedWidth}', 'xCord': '${x}', 'yCord': '${y}'}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {
                dropDown.value = r.region
                if (pickerType == "extDam") {
                    ApplyConditionFilters('e', 'A');
                }
                event.target.style.display = "none";
                datablock.style.display = "table"
            } else {
                alert(r.message);
            }

            return false;
        }
    });
}

function SavePhotos() {
    const extGrid = $("#jsGridExtDamage").data("JSGrid");
    const intGrid = $("#jsGridIntDamage").data("JSGrid");

    const ajaxCalls = [];

    // ---------- Exterior ----------
    if (extGrid && extGrid.data && extGrid.data.length > 0) {
        for (let i = 0; i < extGrid.data.length; i++) {
            const row = extGrid.data[i];
            const data = JSON.parse(JSON.stringify(row));

            if (data.PhotoInfo !== "" &&
                data.PhotoInfo !== "Existing Photo" &&
                data.neededAction !== "Delete" &&
                data.neededAction !== "NewNone") {

                const call = $.ajax({
                    type: "POST",
                    url: 'InspectVehicle.aspx/DamangePhotoUpload',
                    data: JSON.stringify({
                        FileName: data.PhotoFileName,
                        photo: data.PhotoData
                    }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    error: function (XMLHttpRequest) {
                        alert(`Error on saving Photo: ${XMLHttpRequest.responseJSON?.Message}`);
                    },
                    success: function (response) {
                        if (!response.d.Success) {
                            alert('Failure on saving exterior damage photos: ' + response.d.Error);
                            return;
                        }
                        extGrid.data[i].PhotoFileName = response.d.URL;
                    }
                });

                ajaxCalls.push(call);
            }
        }
    }

    // ---------- Interior ----------
    if (intGrid && intGrid.data && intGrid.data.length > 0) {
        for (let i = 0; i < intGrid.data.length; i++) {
            const row = intGrid.data[i];
            const data = JSON.parse(JSON.stringify(row));

            if (data.PhotoInfo !== "" &&
                data.PhotoInfo !== "Existing Photo" &&
                data.neededAction !== "Delete" &&
                data.neededAction !== "NewNone") {

                const call = $.ajax({
                    type: "POST",
                    url: 'InspectVehicle.aspx/DamangePhotoUpload',
                    data: JSON.stringify({
                        FileName: data.PhotoFileName,
                        photo: data.PhotoData
                    }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    error: function (XMLHttpRequest) {
                        alert(`Error on saving Inspection: ${XMLHttpRequest.responseJSON?.Message}`);
                    },
                    success: function (response) {
                        if (!response.d.Success) {
                            alert('Failure on saving interior damage photos: ' + response.d.Error);
                            return;
                        }
                        intGrid.data[i].PhotoFileName = response.d.URL;
                    }
                });

                ajaxCalls.push(call);
            }
        }
    }

    // If nothing to upload, resolve immediately
    if (ajaxCalls.length === 0) {
        return $.Deferred().resolve().promise();
    }

    // Return a promise that resolves when ALL ajax calls are done
    return $.when.apply($, ajaxCalls);
}

function fileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        // For images, DataURL is what you want (includes "data:mime;base64,...")
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
    });
}

function DamagePhotoNameCheck(dataSet) {
    extDamages = dataset[1]
    intDamages = dataset[2]

    if (extDamages.length == 0 && intDamages.length == 0) {
        return true
    }

    for (let i = 0; i < extDamages.length; i++) {
        if ((extDamages[i].PhotoFileName.match(/_/g) || []).length != 4) {
            return false
        }
    }

    for (let i = 0; i < intDamages.length; i++) {
        if ((intDamages[i].PhotoFileName.match(/_/g) || []).length != 4) {
            return false
        }
    }

    return true
}

$(document).ready(function () {
    if (IsLiquidConnect()) {
        document.getElementById("sidebar_menu").style.display = 'none';
        document.getElementsByClassName("inspectVehicle")[0].style.paddingLeft = "0px";
    }
});