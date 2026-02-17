function OnChangePause(input, func, delay) {
    if (delay == null)
        delay = 500;
    input.onkeyup = function (e) {
        clearTimeout(pause_timeout);
        pause_timeout = setTimeout(function () { func(input.value); }, delay);
    };
}

function HandleChangePause(input) {
    userSearch();
}

function UsersGridRowSelected(item) {
    var userId = item[0].children[4].innerHTML;
    var gridData = $("#usersGrid").data("JSGrid").data;

    for (var i = 0;i < gridData.length; i++)
    {
        if (gridData[i].UserID == userId) {
            document.getElementById("MainContent_kPerson").value = gridData[i].kPerson;
            break;
        }
    }
}

function SearchGridRowSelected(item) {
    document.getElementById("MainContent_selectedUser").value = item[0].children[2].innerHTML;
}

function ClearRowSelection() { }

function EditUser() {
    var kPerson = document.getElementById("MainContent_kPerson").value == "" ? 0 : parseInt(document.getElementById("MainContent_kPerson").value);
    if (kPerson == 0) {
        alert("Please select a user to edit!");
        return false;
    }

    ClearOptions();
    $('#userSettingsTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#relationShip').find('input, textarea, button, select').prop('disabled', false);
    $('#relations').find('input, textarea, button, select').prop('disabled', false);

    // Load User Options
    toggleCssClass([['userModal', 'show_display']]);
    document.getElementById("operation").value = "editUser";
    document.getElementById("userButtons").style.display = "none";

    $.ajax({
        type: 'POST',
        url: 'UserManagement.aspx/GetUserInfo',
        data: `{'kPerson': ${kPerson}}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {

                PopulateUserInfo(r.value);
                return false;
            }

            alert(r.message);
            return false;
        }
    });
}

function PopulateUserInfo(info) {
    document.getElementById("MainContent_FName").value = info.FName;
    document.getElementById("MainContent_LName").value = info.LName;
    document.getElementById("MainContent_userEmail").value = info.Email;
    document.getElementById("MainContent_workNumber").value = info.PhoneNumber;
    document.getElementById("MainContent_cellNumber").value = info.CellNumber;
    document.getElementById("chksmsText").checked = info.SMSTexting == 1 ? true : false;
    document.getElementById("MainContent_userID").value = info.UserID;
    document.getElementById("MainContent_auctionAccessUserID").value = info.AuctionAccessID;
    document.getElementById("MainContent_manheimUserID").value = info.ManheimUserName;
    document.getElementById("MainContent_userRelationship").value = info.kRelation;
    document.getElementById("lmQueue").checked = info.LeadMember == 1 ? true : false;
    document.getElementById("lmAdmin").checked = info.LMAdmin == 1 ? true : false;
    document.getElementById("invenAdmin").checked = info.InvAdmin == 1 ? true : false;
    document.getElementById("wholesaleInspector").checked = info.WholesaleInspector == 1 ? true : false;
    document.getElementById("MainContent_lstInspectionCompany").value = info.kWholesaleInspectionCompany;
    document.getElementById("wholesaleAdmin").checked = info.WholesaleAdmin == 1 ? true : false;
    document.getElementById("mobileData").checked = info.MDCUser == 1 ? true : false;
    document.getElementById("wholesaleBuyer").checked = info.WholesaleBuyer == 1 ? true : false;
    document.getElementById("wholesaleSeller").checked = info.WholesaleSeller == 1 ? true : false;
    document.getElementById("Appraiser").checked = info.AppraisalAppraiser == 1 ? true : false;
    document.getElementById("SalesPerson").checked = info.AppraisalSales == 1 ? true : false;
    document.getElementById("retailALOverride").checked = info.SALPricingBypass == 1 ? true : false;

    toggleInspectionCompany();
}

function AddDealerUser() {
    if (document.getElementById("MainContent_selectedUser").value == "") {
        alert("Please select a user to add to dealer!");
        return false;
    }
    needImplementation();
}

function AddUser() {
    $('#userSettingsTbl').find('input, textarea, button, select').prop('disabled', false);
    $('#relationShip').find('input, textarea, button, select').prop('disabled', false);
    $('#relations').find('input, textarea, button, select').prop('disabled', false);

    ClearOptions();
    toggleOption('add');
    toggleCssClass([['userModal', 'show_display']]);

    document.getElementById("MainContent_kPerson").value = "";
    document.getElementById("operation").value = "addUser";
}

function SaveUser() {
    var text = "";
    var op = document.getElementById("operation").value;
    if (op == "addUser")
        text = "Adding new user to the sysem...";
    else if (op == "editUser")
        text = "Saving user info...";
    else if (op == "addDealerUser")
        text = "Adding user to dealer...;";
    else
        text = "Removing user...";

    // Checks for info
    var errors = [];
    if (document.getElementById("MainContent_userEmail").value == "")
        errors.push("You must enter an email address for this user!");

    if (document.getElementById("MainContent_userID").value == "")
        errors.push("You must enter a User ID for this user!");

    if (document.getElementById("chksmsText").checked && document.getElementById("MainContent_cellNumber").value == "")
        errors.push("Cell Number is required to enable SMS texting!");

    if (document.getElementById("MainContent_userRelationship").value == 0)
        errors.push("You must select a relationship for this user!");

    if (errors.length > 0) {
        alert("Please select a value for the following items:\n\t" + errors.join("\n\t"));
        return false;
    }

    toggleLoading(true, text);

    var json = {
        kPerson: op == "addUser" ? 0 : op != "addDealerUser" ? document.getElementById("MainContent_kPerson").value : document.getElementById("selectedUser").value,
        FName: document.getElementById("MainContent_FName").value,
        LName: document.getElementById("MainContent_LName").value,
        EMail: document.getElementById("MainContent_userEmail").value,
        UserID: document.getElementById("MainContent_userID").value,
        Relationship: document.getElementById("MainContent_userRelationship").value,
        AddExistingUser: op == "addUser" ? 0 : document.getElementById("selectedUser").value == "" ? 0 : 1,
        PhoneNumber: document.getElementById("MainContent_workNumber").value,
        CellNumber: document.getElementById("MainContent_cellNumber").value,
        LeadMember: document.getElementById("lmQueue").checked ? 1 : 0,
        LmAdmin: document.getElementById("lmAdmin").checked ? 1 : 0,
        InvAdmin: document.getElementById("invenAdmin").checked ? 1 : 0,
        MDC: document.getElementById("lmAdmin").checked ? 1 : 0,
        WholesaleAdmin: document.getElementById("wholesaleAdmin").checked ? 1 : 0,
        Inspector: document.getElementById("wholesaleInspector").checked ? 1 : 0,
        WholesaleBuyer: document.getElementById("wholesaleBuyer").checked ? 1 : 0,
        WholesaleSeller: document.getElementById("wholesaleSeller").checked ? 1 : 0,
        ManheimUserID: document.getElementById("MainContent_manheimUserID").value,
        AuctionAccessID: document.getElementById("MainContent_auctionAccessUserID").value,
        AppraisalAppraiser: document.getElementById("Appraiser").checked ? 1 : 0,
        AppraisalSales: document.getElementById("SalesPerson").checked ? 1 : 0,
        WholesaleInspector: document.getElementById("wholesaleInspector").checked ? 1 : 0,
        kWholesaleInspectionCompany: document.getElementById("MainContent_lstInspectionCompany").value
    };

    $.ajax({
        type: "POST",
        url: 'UserManagement.aspx/UserInfoSave',
        data: `{ 'json': '${JSON.stringify(json)}', 'op': '${op}' }`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = response.d;
            if (r.success) {
                toggleLoading(false, text);
                return false;
            }
            else {
                alert(r.message);
                toggleLoading(false, text);
                return false;
            }
        }
    });
}

function RemoveUser() {
    var kPerson = document.getElementById("MainContent_kPerson").value == "" ? 0 : parseInt(document.getElementById("MainContent_kPerson").value);
    if (kPerson == 0) {
        alert("Please select a user to remove!");
        return false;
    }

    ClearOptions();
    // Load User Options
    toggleCssClass([['userModal', 'show_display']]);
    document.getElementById("operation").value = "removeUser";
    document.getElementById("userButtons").style.display = "none";

    $('#userSettingsTbl').find('input, textarea, button, select').prop('disabled', true);
    $('#relationShip').find('input, textarea, button, select').prop('disabled', true);
    $('#relations').find('input, textarea, button, select').prop('disabled', true);

    $.ajax({
        type: 'POST',
        url: 'UserManagement.aspx/GetUserInfo',
        data: `{'kPerson': ${kPerson}}`,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        complete: function (response) {
            var r = response.responseJSON.d;
            if (r.success) {

                PopulateUserInfo(r.value);
                return false;
            }

            alert(r.message);
            return false;
        }
    });
}

function userSearch() {
    var gridData = $("#searchGrid").data("JSGrid");
    var gfilter = gridData.getFilter();

    gfilter["textFilter"] = $('#MainContent_searchUsers').val();

    gridData.search(gfilter);
    return false;
}

function toggleInspectionCompany() {
    if (document.getElementById("wholesaleInspector").checked)
        document.getElementById("MainContent_lstInspectionCompany").disabled = false;
    else {
        document.getElementById("MainContent_lstInspectionCompany").disabled = true;
        document.getElementById("MainContent_lstInspectionCompany").value = 0;
    }
    return false;
}

function toggleOption(op) {
    if (op == 'add') {
        // Switch Views
        document.getElementById("existingUser").style.display = "none";
        document.getElementById("userSettingsTbl").style.display = "table";

        // Clear out associated selections
        document.getElementById("MainContent_searchUsers").value = "";
        document.getElementById("MainContent_kPerson").value = "addUser";
    } else {
        // Switch Views
        document.getElementById("existingUser").style.display = "";
        document.getElementById("userSettingsTbl").style.display = "none";

        // Clear out associated selections
        document.getElementById("userSettingsTbl").querySelectorAll('input[type=text]:not([readonly])').forEach(node => node.value = '');
        document.getElementById("chksmsText").checked = false;
        document.getElementById("MainContent_kPerson").value = "addDealerUser"
    }

    document.getElementById("relations").querySelectorAll('input[type=checkbox]').forEach(node => node.checked = false);
    document.getElementById("MainContent_userRelationship").selectedIndex = 0;
    document.getElementById("MainContent_lstInspectionCompany").disabled = true;
    document.getElementById("MainContent_lstInspectionCompany").value = 0;
}

function ClearOptions() {
    // Clear out associated selections
    document.getElementById("userSettingsTbl").querySelectorAll('input[type=text]:not([readonly])').forEach(node => node.value = '');
    document.getElementById("chksmsText").checked = false;
    document.getElementById("relations").querySelectorAll('input[type=checkbox]').forEach(node => node.checked = false);
    document.getElementById("MainContent_userRelationship").selectedIndex = 0;
    document.getElementById("userButtons").style.display = "block";

    document.getElementById("MainContent_lstInspectionCompany").disabled = true;
    document.getElementById("MainContent_lstInspectionCompany").value = 0;
}