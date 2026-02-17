function GridRowSelected(item) {
    document.getElementById("MainContent_txtCredential").value = item[0].cells[0].childNodes[0].value;
}

function ClearRowSelection() {
    document.getElementById("MainContent_txtCredential").value = "";
}

function AddAuctionCredential() {
    document.getElementById("MainContent_txtCredential").value = "";
    document.getElementById("alternateCredTitle").innerHTML = "Add Alternate Credential";

    toggleCssClass([['credentialPop', 'show_display']]);
    return false;
}

function EditAuctionCredential() {
    if (document.getElementById("MainContent_txtCredential").value == "") {
        alert("Please select an Alternate Credential to edit!");
        return false;
    }

    document.getElementById("alternateCredTitle").innerHTML = "Edit Alternate Credential";
    var kcred = document.getElementById("MainContent_txtCredential").value;

    toggleCssClass([['credentialPop', 'show_display']]);
    return false;
}