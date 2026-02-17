<%@ Page Title="Upload Photos" Language="C#" AutoEventWireup="true" CodeBehind="UploadPhotos.aspx.cs" Inherits="LMWholesale.WholesaleData.UploadPhotos" %>

<%@ Register Assembly="Aurigma.ImageUploaderFlash" Namespace="Aurigma.ImageUploaderFlash" TagPrefix="portalUpload" %>

<script type="text/javascript" src="/Common/Aurigma/iuembed.js"></script>
<script type="text/javascript" src="/Scripts/jquery-3.3.1.min.js"></script>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<script type="text/javascript">
    function onBeforeUpload() {
        if (!IsLiquidConnect())
            alert("Attempting to upload selected photos!");
    }

    function displayError(errorCode, httpResonseCode, errorPage, additionalInfo) {
        if (errorCode == 4) {
            var newWindow = window.open();
            with (newWindow.document) {
                open("text/html");
                write(errorPage);
                close();
            }
        }
    }
</script>
<body class="uploadBkg">
    <form id="form1" runat="server" style="margin:0;">
        <portalUpload:ImageUploaderFlash ID="PhotoUploader" runat="server"
            Type="flash|html" EnableRotation="false" EnableDescriptionEditor="false"
            Height="600" Width="815" LicenseKey="77FF1-001AD-E0D80-00080-92CED-D015B8" FolderProcessingMode="None">
            <ClientEvents>
                <portalUpload:ClientEvent EventName="BeforeUpload" HandlerName="onBeforeUpload"/>
                <portalUpload:ClientEvent EventName="Error" HandlerName="displayError"/>
            </ClientEvents>
        </portalUpload:ImageUploaderFlash>
    </form>
</body>
</html>
