$(function () {
    $('#sidebar_menu').load('/Common/Sidebar.html', function () {
        var pref = document.getElementById("showPreferences").value;
        var internalFlag = document.getElementById("internalFlag").value;
        if (pref == "False")
            document.getElementById("preferences").style.display = "none";
    });
});

function toggleCssClass(items) {
    items.forEach(element => document.getElementById(element[0]).classList.toggle(element[1]));
    return false;
}

function toggleSidebar() {
    var sidebar_items = [
        ["menu_items", "sidepanelOpen"],
        ["sidebar_button", "openBtn"]
    ];

    toggleCssClass(sidebar_items);

    return false;
}

function ocToggleSidebar() {
    var sidebar = document.getElementById("menu_items");
    if (!(sidebar.classList.contains('sidepanelOpen'))) {
        toggleSidebar();
    }
    return false;
}

function WriteError(error) {

}

window.onclick = function (e) {
    var sidebar = document.getElementById("menu_items");
    var navMenu = document.getElementById("navContent");
    var vehicleBar = document.getElementById("vehicleActionBar");
    var updateBar = document.getElementById("updateActionsMenu");
    var isGallery = (/.jpg$/.exec(e.target.src) || /.aspx$/.exec(e.target.src)) && e.target.matches("img");

    if (navMenu != null) {

        // Close top left menu if open
        var navButton = document.getElementById("navButton");
        if (!e.target.matches('.dropdownButton') && e.target.id != "VinSearch") {
            if (navMenu.classList.contains('openContent')) {
                navMenu.classList.remove('openContent');
                navButton.classList.remove('openDropdown');
            }
        }
    }

    var isNotPref = e.target.id != "preferences" && e.target.id != "preferenceIcon";
    var isnotTraining = e.target.id != "training" && e.target.id != "trainingIcon";
    if (e.target.id != "homeIcon" && e.target.id != "homeButton" && isNotPref && isnotTraining) {
        if (sidebar != null) {

            var t = e.target;
            // Close certain items while sidebar is open
            var menu_items = ["inventory_menu", "reporting_menu"];
            if ($('a').has(t).length != 0
                && t.matches("img")
                && t.id != "LMLogo"
                && t.title == ""
                && !isGallery) {
                if (!(sidebar.classList.contains('sidepanelOpen'))) {
                    toggleSidebar();
                } else if (menu_items.includes(`${t.id}_menu`)) {
                    menu_items.forEach(item => {
                        if (document.getElementById(item).classList.contains('show')) {
                            toggleCssClass([[item, "show"]]);
                        }
                    });
                }
            } else if (!t.matches('.closeBtn') && !(t instanceof HTMLAnchorElement) && t.title != "") {
                if (sidebar.classList.contains('sidepanelOpen')) {
                    toggleSidebar();
                    menu_items.forEach(item => {
                        if (document.getElementById(item).classList.contains('show')) {
                            toggleCssClass([[item, "show"]]);
                        }
                    });
                }
            } else if (!(t instanceof HTMLAnchorElement) && t.id == "") {
                if (sidebar.classList.contains('sidepanelOpen')) {
                    toggleSidebar();
                    menu_items.forEach(item => {
                        if (document.getElementById(item).classList.contains('show')) {
                            toggleCssClass([[item, "show"]]);
                        }
                    });
                }
            } else if (t.matches('.closeBtn')) {
                menu_items.forEach(item => {
                    if (document.getElementById(item).classList.contains('show')) {
                        toggleCssClass([[item, "show"]]);
                    }
                });
            } else if (/^(auctionInfo_)/.exec(e.target.title)) {
                return false;
            } else if (menu_items.filter(m => m.includes(t.id)).length > 0) {
                // If I match a menu item, close other menu items that are open
                menu_items.forEach(item => {
                    if (document.getElementById(item).classList.contains('show')) {
                        toggleCssClass([[item, "show"]]);
                    }
                });
            } else if (!(t.matches('.closeBtn'))
                && !(t instanceof HTMLAnchorElement)
                && t.id != ""
                && t.title == "") {
                // Close sidebar if nothing else matches
                if (sidebar.classList.contains('sidepanelOpen'))
                    toggleSidebar();
                menu_items.forEach(item => {
                    if (document.getElementById(item).classList.contains('show')) {
                        toggleCssClass([[item, "show"]]);
                    }
                });
            }
        }

        if (vehicleBar != null) {

            // Closes all open vehicle action bars
            if (!e.target.matches('.vehicleActions')) {
                var actionList = document.getElementsByClassName('openActionsContent');
                var buttonList = document.getElementsByClassName('openVehicleActions');

                Array.from(actionList).forEach((action) => {
                    action.classList.remove('openActionsContent');
                });

                Array.from(buttonList).forEach((button) => {
                    button.classList.remove('openVehicleActions');
                });
            }

            // Close all open additional auction bars
            if (!e.target.matches('.auctionsMenu')) {
                var auctionList = document.getElementsByClassName('openAdditionalAuctionsContent');
                var buttonList = document.getElementsByClassName('openAdditionalAuctions');

                Array.from(auctionList).forEach((auction) => {
                    auction.classList.remove('openAdditionalAuctionsContent');
                });

                Array.from(buttonList).forEach((button) => {
                    button.classList.remove('openAdditionalAuctions');
                });
            }

            if (!e.target.matches('.auctionView')) {
                var auctionView = document.getElementsByClassName('openAuctionsContent');
                if (auctionView.length != 0)
                    auctionView[0].classList.remove("openAuctionsContent");
            }
            
            if (!e.target.matches('.errMessageView')) {
                var auctionView = document.getElementsByClassName('openErrMsgsContent');
                if (auctionView.length != 0)
                    auctionView[0].classList.remove("openErrMsgsContent");
            }
        }

        if (updateBar != null) {

            // Close Update Actions Menu
            if (!e.target.matches('.updateActions')) {
                var updateMenu = document.getElementById('updateActionsMenu');
                var updateActions = document.getElementById('MainContent_updateVehicleActions');
                if (updateActions.classList.contains('openUpdateActionsContent')) {
                    updateActions.classList.remove('openUpdateActionsContent')
                }
                if (updateMenu.classList.contains('openUpdateActions')) {
                    updateMenu.classList.remove('openUpdateActions')
                }
            }
        }
    }
}

function toggleLoading(show, customMessage, timeout = 2000) {
    document.getElementById("LoadingLabel").innerHTML = customMessage != "" ? customMessage : "Your data is now being submitted.";
    setTimeout(() => {
        document.getElementById("LoadingMask").style.visibility = show ? "visible" : "hidden";
        document.getElementById("loadingHolder").style.display = show ? "block" : "none";
    }, show == false ? timeout : 0);
}

function BuildSaveWithDropdownOptions(field) {
    var selectedValue = document.getElementById(field).value;
    var saveString = "";
    for (var i = 1; i < document.getElementById(field).options.length; i++) {
        if (selectedValue == document.getElementById(field).options[i].value)
            saveString = "[" + document.getElementById(field).options[i].innerHTML + "]" + saveString + document.getElementById(field).options[i].value + ":" + document.getElementById(field).options[i].innerHTML + "|";
        else
            saveString += document.getElementById(field).options[i].value + ":" + document.getElementById(field).options[i].innerHTML + "|";
    }
    return saveString;
}

function needImplementation() {
    alert("Functionality Coming Soon!");
    return false;
}

function IsLiquidConnect() {
    const containsLetterRegex = /[a-z]/i;
    if (!containsLetterRegex.test(window.location.href.substring(8).split("/")[0].toLowerCase()) ||
        window.location.href.substring(8).split("/")[0].toLowerCase().includes("liquidconnect") ||
        window.location.href.substring(8).split("/")[0].toLowerCase().includes("lc")) {
        return true;
    }
    else {
        return false;
    }
}