<%@ Page Title="Wholesale Default" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WholesaleDefault.aspx.cs" Inherits="LMWholesale.WholesaleDefault" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/WholesaleDefault.css?lmV=<%Response.Write(Application["ContentVersion"]);%>">
 
    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/WholesaleContent/WholesaleDefault.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <style type="text/css">
        .body-content {padding: 15px 15px 0 15px !important;}
        .jsgrid {padding: 10px 0px 0px 0px;}
        .jsgrid-grid-body {height:calc(100vh - 220px);}
        .btnAccountSetup {
            background-image: url(../Images/fa-icons/plus-circle.svg);
	        background-position: center;
	        background-repeat: no-repeat;
	        width: 35px;
	        height: 35px;
	        padding: 15px;
	        border: none;
        }
        .divAccountSearch {
            width: 100%;
            text-align: center;
        }
        @media (max-width: 767px) {
            .jsgrid-grid-body {
                height:calc(100vh - 400px);
            }
            .smallBottomMargin {
                margin-bottom: 10px;
            }
            .ONLYSHOWONWDMOBILE {
                display: block;
            }
        }
    </style>

	<asp:HiddenField ID="txtkDealer" runat="server" Value="" />
    <div class="DealerSelect row" id="dealerSelectSearch" runat="server">
        <div class="col-sm-4 col-md-2 d-flex smallBottomMargin" style="justify-content:center;">
            <button type="button" id="btnAccountSetup" class="ddlDealerSelect btnAccountSetup" title="Add OTG Account" onclick="window.location.replace(window.location.href.replace('WholesaleDefault', 'AccountSetup'));" runat="server"></button>
            <div>&nbsp;&nbsp;&nbsp;&nbsp;</div>
            <asp:Button ID="exportBtn" runat="server" OnClick="ExportAccounts" style="display:none;"/>
            <img src="/Images/fa-icons/file-download.svg" style="height:35px;width:35px;" class="mdIcon pointer" title="Export Dealer Accounts" OnClick="excelDownload(); return false;"/>
            <div>&nbsp;&nbsp;&nbsp;&nbsp;</div>
            <img src="/Images/fa-icons/diagram-next.svg" style="padding:4px;" class="mdIcon pointer" title="Sell Down API" OnClick="fnGoToSellDownAPI();"/>
            <div>&nbsp;&nbsp;&nbsp;&nbsp;</div>
            <img id="btnSupportTool" runat="server" src="/Images/fa-icons/tv.svg" style="padding:0px;" class="smIcon pointer" title="Support Tool Monitor" OnClick="fnGoToSupportTool();"/>
            <div id="btnSupportToolspace" runat="server">&nbsp;&nbsp;&nbsp;&nbsp;</div>
            <button type="button" id="btnAccountFilterClear" class="ddlDealerSelect btnAccountFilterClear" title="Clear Filters" onclick="fnFilterClear()"></button>
        </div>
        <div class="col-sm-4 col-md-2 d-flex justify-content-center">
            <div class="divAccountSearch">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="txtSearch" placeholder="Search Accounts"></asp:TextBox>
            </div>
        </div>
        <div d='AccountRepSelector' runat="server" class="col-sm-4 col-md-2 d-flex justify-content-center">
            <asp:DropDownList ID="ddlAccountRep" runat="server" CssClass="ddlDealerSelect" onchange="ddlChanged()"></asp:DropDownList>
        </div>
        <div id='AccountGroupSelector' runat="server" class="col-sm-4 offset-sm-4 col-md-2 offset-md-0 d-flex justify-content-center">
            <asp:DropDownList ID="ddlAccountGroup" runat="server" CssClass="ddlDealerSelect" onchange="ddlChanged()"></asp:DropDownList>
        </div>
        <div id='AccountStatusSelector' runat="server" class="col-sm-4 col-md-2 d-flex justify-content-center">
            <asp:DropDownList ID="ddlAccountStatus" runat="server" CssClass="ddlDealerSelect" onchange="ddlChanged()"></asp:DropDownList>
        </div>
        <div class="d-none d-md-block">
            
        </div>
    </div>

    <script type="text/javascript" src="/Scripts/jsGrid.js"></script>
    <div class="wholesaleDefault">
        <div id="jsGrid"></div>
        <asp:Button ID="btnSubmit" CssClass="btnSubmitDefault" runat="server" Text="Go To Wholesale" OnClientClick="return fnSubmit();" CausesValidation="False" OnClick="btnSubmit_Click" />
        <asp:Button ID="btnSellDownAPI" CssClass="btnSubmitDefault" runat="server" Text="Go To SellDown API" CausesValidation="False" OnClick="btnSellDownAPI_Click" />
    </div>
    
    <script type="text/javascript">
        var pause_timeout = null;

	    var textInput = document.getElementById('<%=txtSearch.ClientID %>');
        OnChangePause(textInput, HandleChangePause, 300);

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value });
    </script>
</asp:Content>