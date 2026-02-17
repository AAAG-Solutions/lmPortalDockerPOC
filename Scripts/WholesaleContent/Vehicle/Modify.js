function ShowCloseTab(tab) {
    var items = ['modifyGeneralVehicleInfo',
        'modifyTitleVehicleInfo',
        'modifyOptionsVehicleInfo',
        'modifyWarrantyVehicleInfo'];

    items.splice(items.indexOf(tab), 1);

    var openTab = document.getElementById(tab);
    if (!openTab.classList.contains("showTab")) {
        openTab.classList.toggle("showTab");
    }

    items.forEach(item => {
        var closeTab = document.getElementById(item);
        if (closeTab.classList.contains("showTab")) {
            closeTab.classList.toggle("showTab");
        }
    });

}