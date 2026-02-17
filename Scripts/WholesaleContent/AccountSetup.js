function RowDoubleClick(item) {
    FillData();
    ShowApprovalBox(true);
}

function GridRowSelected(row) {
    document.getElementById("MainContent_hfApprovalId").value = row[0].children[0].innerHTML;
}

function ClearRowSelection() {
    $('#MainContent_hfApprovalId').val("");
}

function ShowApprovalBox(show) {
    document.getElementById("MainContent_fsApprovalBox").style.display = show ? "block" : "none";
}

function FillData() {
    if (document.getElementById("MainContent_hfApprovalId").value == "") {
        alert("Please select a signup to review.");
        return false;
    }

    $.ajax({
        type: "POST",
        url: 'AccountSetup.aspx/GetSpecificSignup',
        data: "{'kWholesaleSignup': '" + document.getElementById("MainContent_hfApprovalId").value + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            if (response.d.success == true) {
                FillFields(response.d.value);
                return false;
            }
            else {
                alert("Unable to load selected signup\n" + response.d.message);
                return false;
            }
        }
    });
    return false;
}

function TemplateChange(self) {
    document.getElementById("MainContent_kTemplate").value = self.options[self.selectedIndex].value;
}

function AuctionChange() {
    $.ajax({
        type: "POST",
        url: 'AccountSetup.aspx/GetAuctionTemplates',
        data: "{'kDealerSubGaggle': '" + document.getElementById("MainContent_ddlAuction").value + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            if (response.d.success == true) {
                BuildTemplateDropdown(response.d.value);
                return false;
            }
            else {
                alert("Unable to load selected signup\n" + response.d.message);
                return false;
            }
        }
    });
    return false;
}

function BuildTemplateDropdown(info) {
    var templateDropdown = document.getElementById("MainContent_ddlAccountTemplate");

    if (templateDropdown.options.length > 1) {
        templateDropdown.options.length = 0;
        templateDropdown.add(new Option('-- No Account Template --',0), undefined);
    }

    if (info.length == 0)
        return false;

    var items = info.split("|");
    var first = true;
    items.forEach((element) => {
        if (element != "") {
            var item = element.split(":");
            let newOption = new Option(item[1], item[0]);
            if (first) {
                newOption.selected = first;
                first = false;
                document.getElementById("MainContent_kTemplate").value = item[0];
            }
            templateDropdown.add(newOption, undefined);
        }
    });

    if (templateDropdown.options.length > 2) {
        templateDropdown.selectedIndex = 0;
        document.getElementById("MainContent_kTemplate").value = 0;
    }

    return false;
}

function FillFields(info) {
    document.getElementById("MainContent_ddlAuction").value = info.kDealerSubGaggle;
    AuctionChange();
    document.getElementById("MainContent_txtStreet").value = info.Address1;
    document.getElementById("MainContent_txtCity").value = info.City;
    document.getElementById("MainContent_txtPhone").value = info.Phone;
    document.getElementById("MainContent_txtCellNumber").value = info.ContactCPhone;
    document.getElementById("MainContent_txtEmail").value = info.ContactEmail;
    document.getElementById("MainContent_txtName").value = info.ContactFName;
    document.getElementById("MainContent_txtOfficeNumber").value = info.ContactWPhone;
    document.getElementById("MainContent_txtZipCode").value = info.Zip;
    document.getElementById("MainContent_txtDealer").value = info.DealerName;
    document.getElementById("MainContent_txtNotes").value = info.Notes;
    document.getElementById("MainContent_ddlState").value = info.State;
    document.getElementById("MainContent_ddlAccountTemplate").value = info.kTemplateDealer;
    if (info.LiquidConnect == "1")
        document.getElementById("MainContent_chkYesLC").checked = true;
    else
        document.getElementById("MainContent_chkNoLC").checked = true;

    if (info.kWholesaleLocationIndicator == "1")
        document.getElementById("MainContent_chkDealership").checked = true;
    else
        document.getElementById("MainContent_chkAuction").checked = true;

    var companyName = document.getElementById("MainContent_ddlCompanyName");
    for (var i = 0; i < companyName.children.length; i++) {
        if (companyName.children[i].text == info.FeedCompany) {
            companyName.children[i].selected = true;
            break;
        }
    }

    var chararry = info.CopyFlags.split('');
    document.getElementById("MainContent_chkAddress").checked = chararry[0] == "1";
    document.getElementById("MainContent_chkUsers").checked = chararry[1] == "1";
    document.getElementById("MainContent_chkContactGroups").checked = chararry[2] == "1";
    document.getElementById("MainContent_chkLotLocations").checked = chararry[3] == "1";
    document.getElementById("MainContent_chkWholesaleAuctions").checked = chararry[4] == "1";
    document.getElementById("MainContent_chkAlternateCredentials").checked = chararry[5] == "1";
    document.getElementById("MainContent_chkAutoLaunchRules").checked = chararry[6] == "1";
    document.getElementById("MainContent_chkBlackoutRules").checked = chararry[7] == "1";
    document.getElementById("MainContent_chkProducts").checked = chararry[8] == "1";

    document.getElementById("MainContent_chkSuppressEmails").Checked = info.SuppressAuctionEmails == "1";

    return false;
}

function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input, input.value); }, delay);
    };
}

function HandleChangePause(control, value) {
    control.value = formatPhoneNumber(control, value);
}

function formatPhoneNumber(control, input) {
    var opos = getCaretPosition(control)
    var olen = control.value.length;

    var text = CleanPhoneNumber(input);
    input = !text[2] ? text[1] : '(' + text[1] + ') ' + text[2] + (text[3] ? '-' + text[3] : '');
    var nlen = control.value.length;
    var npos = opos + (nlen - olen);

    setCursor(control, npos);

    return input;
}

function CleanPhoneNumber(input) {
    return input.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
}

function getCaretPosition(control) {
    var iCaretPos = 0;
    if (document.selection) {
        control.focus();
        var oSel = document.selection.createRange();
        oSel.moveStart('character', -control.value.length);
        iCaretPos = oSel.text.length;
    }
    else if (control.selectionStart || control.selectionStart == '0') // firefox
        iCaretPos = control.selectionStart;

    return iCaretPos;
}

function setCursor(node, pos) {

    if (!node) {
        return false;
    }
    else if (node.createTextRange) {
        var textRange = node.createTextRange();
        textRange.collapse(true);
        textRange.moveStart('character', pos);
        textRange.moveEnd('character', 0);
        textRange.select();
        return true;
    }
    else if (node.setSelectionRange) {
        node.setSelectionRange(pos, pos);
        return true;
    }
    return false;
}

function ValidateSignup(source) {
    var sError = "";
    if (source != "deny") {
        if (document.getElementById("MainContent_ddlAuction").options[document.getElementById("MainContent_ddlAuction").selectedIndex].text == "--Select an Auction--") {
            sError += "You must select an Auction.\n";
        }
        if (document.getElementById('MainContent_txtDealer').value == "") {
            sError += "You must enter a Dealership Name.\n";
        }
        if (document.getElementById('MainContent_txtStreet').value == "") {
            sError += "You must enter a Dealership Address.\n";
        }
        if (document.getElementById('MainContent_txtStreet').value.toLowerCase().startsWith('po box') || document.getElementById('MainContent_txtStreet').value.toLowerCase().startsWith('p.o. box') || document.getElementById('MainContent_txtStreet').value.toLowerCase().startsWith('p.o box')) {
            sError += "P.O. Boxes are not allowed for Dealership Address.\n";
        }
        if (document.getElementById('MainContent_txtCity').value == "") {
            sError += "You must enter a Dealership City.\n";
        }
        if (document.getElementById('MainContent_ddlState').options[document.getElementById("MainContent_ddlState").selectedIndex].text == "--Select a State--") {
            sError += "You must enter a Dealership State.\n";
        }
        if ((document.getElementById('MainContent_txtZipCode').value == "") || (document.getElementById('MainContent_txtZipCode').value.length < 5)) {
            sError += "You must enter a 5-digit Dealership Zip Code.\n";
        }
        if (document.getElementById('MainContent_txtPhone').value.length < 14) {
            sError += "You must enter a Dealer Phone Number.\n";
        }
        if (document.getElementById('MainContent_txtName').value == "") {
            sError += "You must enter a Contact Name.\n";
        }
        if (document.getElementById('MainContent_txtOfficeNumber').value.length < 14 && document.getElementById('MainContent_txtCellNumber').value.length < 14) {
            sError += "You must enter a Contact Phone Number.\n";
        }
        if ((document.getElementById('MainContent_txtEmail').value == "") || (!isEmail(document.getElementById('MainContent_txtEmail').value))) {
            sError += "You must enter a valid Email for the Contact.\n";
        }
        if (document.getElementById('MainContent_ddlCompanyName').options[document.getElementById('MainContent_ddlCompanyName').selectedIndex].text == "Select Inventory Source") {
            sError += "You must select an Inventory Source.\n";
        }
        if (document.getElementById('MainContent_ddlCompanyName').options[document.getElementById('MainContent_ddlCompanyName').selectedIndex].text == "Other / Unknown") {
            if (document.getElementById('<%= txtInventoryOther.ClientID %>').value == "") {
                sError += "You must enter an Inventory Source Name.\n";
            }
        }
        if (!document.getElementById('MainContent_chkAuction').checked && !document.getElementById('MainContent_chkDealership').checked) {
            sError += "You must select a Vehicle Pick-up Location.\n";
        }
        if (!document.getElementById('MainContent_chkYesLC').checked && !document.getElementById('MainContent_chkNoLC').checked) {
            sError += "You must select a Liquid Connect option.\n";
        }
    }

    if (sError != "") {
        alert(sError);
        return false;
    }

    document.getElementById("MainContent_SignupSource").value = source;
    return true;
}

function isEmail(email) {
    var regex = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,6})?$/;
    return regex.test(email);
}