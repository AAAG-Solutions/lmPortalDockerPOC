function SwapPhotoView() {
    document.getElementById("divVehiclePhotos").style.display = document.getElementById("divVehiclePhotos").style.display == "none" ? "block" : "none";
    document.getElementById("divDamagePhotos").style.display = document.getElementById("divDamagePhotos").style.display == "none" ? "block" : "none";
    document.getElementById("MainContent_btnVehPhotos").style.display = document.getElementById("MainContent_btnVehPhotos").style.display == "none" ? "block" : "none";
    document.getElementById("MainContent_btnDamPhotos").style.display = document.getElementById("MainContent_btnDamPhotos").style.display == "none" ? "block" : "none";
    document.getElementById("MainContent_btnDelVehPhotos").style.display = document.getElementById("MainContent_btnDelVehPhotos").style.display == "none" ? "block" : "none";
    document.getElementById("MainContent_btnDelDamPhotos").style.display = document.getElementById("MainContent_btnDelDamPhotos").style.display == "none" ? "block" : "none";
}

function DeleteAllPhotos(PhotoSet) {
    var choice = confirm("This will delete all " + PhotoSet + " photos. Do you want to continue?");
    if (choice) {
        var parentElement = document.getElementById("MainContent_panVehPhotoArea");
        if (PhotoSet == "damage") {
            parentElement = document.getElementById("MainContent_panDamPhotoArea");
        }

        for (var i = 0; i < parentElement.childElementCount; i++) {
            parentElement.children[i].style.display = 'none';
        }
    }
    return false;
}

function IncreasePhotoOrder(cardId) {
    var parentElement = document.getElementById("MainContent_panVehPhotoArea")
    for (var i = 0; i < parentElement.childElementCount; i++) {
        if (parentElement.children[i].id == cardId) {
            var moveElement = parentElement.children[i]
            var elementBefore = parentElement.children[i - 1]
            parentElement.removeChild(moveElement)
            parentElement.insertBefore(moveElement, elementBefore)
            break;
        }
    }
    ValidatePhotoOrderButtons();
}

function DecreasePhotoOrder(cardId) {
    var parentElement = document.getElementById("MainContent_panVehPhotoArea")
    for (var i = 0; i < parentElement.childElementCount; i++) {
        if (parentElement.children[i].id == cardId) {
            var moveElement = parentElement.children[i + 1]
            var elementBefore = parentElement.children[i]
            parentElement.removeChild(moveElement)
            parentElement.insertBefore(moveElement, elementBefore)
            break;
        }
    }
    ValidatePhotoOrderButtons();
}

function HidePhotoCard(cardId) {
    document.getElementById(cardId).style.display = 'none';
    ValidatePhotoOrderButtons();
}

function ValidatePhotoOrderButtons() {
    var parentElement = document.getElementById("MainContent_panVehPhotoArea")
    var firstVisible = -1;
    var lastVisible = parentElement.childElementCount - 1;
    for (var i = 0; i < parentElement.childElementCount; i++) {
        var photoCard = parentElement.children[i];
        if (photoCard.style.display != 'none') {
            firstVisible = i;
            document.getElementById(parentElement.children[i].id + "UpArrow").className = "arrow hideArrow"
            break;
        }
    }

    if (firstVisible != -1) {
        for (var i = lastVisible; i >= firstVisible; i--) {
            var photoCard = parentElement.children[i];
            if (photoCard.style.display != 'none') {
                lastVisible = i;
                document.getElementById(parentElement.children[i].id + "DwnArrow").className = "arrow hideArrow"
                break;
            }
        }

        if (firstVisible != lastVisible) {
            for (var i = firstVisible + 1; i < lastVisible; i++) {
                var photocardid = parentElement.children[i].id
                document.getElementById(photocardid + "UpArrow").className = "arrow"
                document.getElementById(photocardid + "DwnArrow").className = "arrow"
            }
        }
    }
}

function SubmitClick() {
    toggleLoading(true, "Saving Photos")
    var VehPhotoList = "";
    var DamPhotoList = "";
    var DelPhotoList = "";

    var parentElement = document.getElementById("MainContent_panVehPhotoArea")
    for (var i = 0; i < parentElement.childElementCount; i++) {
        var element = parentElement.children[i];
        var kPhoto = element.id.split("|")[1];
        if (element.style.display != "none") {
            VehPhotoList += kPhoto + "|";
        }
        else {
            DelPhotoList += "*" + kPhoto + "|";
        }
    }

    parentElement = document.getElementById("MainContent_panDamPhotoArea")
    for (var i = 0; i < parentElement.childElementCount; i++) {
        var element = parentElement.children[i];
        var kPhoto = element.id.split("|")[1];
        if (element.style.display != "none") {
            DamPhotoList += kPhoto + "|";
        }
        else {
            DelPhotoList += "*" + kPhoto + "|";
        }
    }

    var sendList = VehPhotoList + DamPhotoList + DelPhotoList;

    $.ajax({
        type: "POST",
        url: 'ManagePhotos.aspx/SavePhotoChanges',
        data: "{'PhotoList': '" + sendList + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest) {
            alert(`Error on saving photos: ${XMLHttpRequest.responseJSON.Message}`);
            toggleLoading(false, "")
        },
        complete: function (response) {
            if (!response.responseJSON.d.Success) {
                alert('Failure on saving photos: ' + response.responseJSON.d.ErrorMessage);
                toggleLoading(false, "");
                return false;
            }
            window.location.href = "/WholesaleContent/VehicleManagement.aspx"
        }
    });
}

function PhotoPopup(kListing) {
    var popup = window.open(`/WholesaleData/UploadPhotos.aspx?kListing=${kListing}`, '_blank', 'popup,height=625,width=850');
    var timer = setInterval(function () {
        if (popup.closed) {
            clearInterval(timer);
            window.location.reload();
        }
    }, 1000);
}