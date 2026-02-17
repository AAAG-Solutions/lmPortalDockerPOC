function SaveGeneral() {
    var datain = {
        kDealerService: document.getElementById("MainContent_lstService").value,
        DealerAddress1: document.getElementById("MainContent_adressStreet").value,
        DealerCity: document.getElementById("MainContent_addressCity").value,
        DealerState: document.getElementById("MainContent_lstState").value,
        DealerZip: document.getElementById("MainContent_addressZip").value,
        DealerPhone: document.getElementById("MainContent_dealerPhone").value,
        DealerCountry: document.getElementById("MainContent_lstCountry").value,
        CustomerType: 1,
        kTimeZone: document.getElementById("MainContent_lstTimeZone").value,
        WebsiteURL: document.getElementById("MainContent_dealerWebsite").value,
        DisplayDealerName: document.getElementById("MainContent_accountDisplay").value,
        kDistributor: document.getElementById("MainContent_lstDistributor").value,
        kDealerGaggle: document.getElementById("MainContent_lstAccountGrp").value,
        kAccountType: document.getElementById("MainContent_lstAccountType").value,
        kGaggleSubGroup: document.getElementById("MainContent_lstSubAccountGrp").value == "" ? 0 : document.getElementById("MainContent_lstSubAccountGrp").value
    }

    alert("Save currently disabled.");

    /*
    $.ajax({
        type: "POST",
        url: 'General.aspx/SetDealerBase',
        data: "{'jsonInfo': '" + JSON.stringify(datain) + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest.responseJSON.Message);
        },
        success: function (response) {
            var r = JSON.parse(response.d);
            if (r.Success == 1) {
                alert("Save Successful");
                location.reload();
                return false;
            }

            alert("Information failed to save");
        }
    });
    */
}