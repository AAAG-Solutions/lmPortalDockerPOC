<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LMWholesale.Login" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        @media(max-width:768px){
            .loginContent {
                width: 90% !important;
            }
            .loginText {
                font-size: 1rem !important;
                font-weight: bold;
            }

            .loginpagelogo {
                height:50px;
            }
            
            .loginBanner {
                height: 50px;
            }

            .page-wrapper-login {
                height: calc(100vh - 185px) !important;
                margin-top: 50px !important;
            }

            .loginTitle {
                line-height: 50px;
            }
        }
    </style>

    <script type="text/javascript">

        window.onload = function () {
            $('#<%= txtUname.ClientID %>').focus();
        }

        function fnSubmit() {
            $('#<%=btnSubmit.ClientID %>').attr('disabled', 'disabled');
            var sError = "";
            if ($('#<%= txtUname.ClientID %>').val() == "") {
                sError += "You must enter your User Name.\n";
            }
            if ($('#<%= txtPwd.ClientID %>').val() == "") {
                sError += "You must enter your Password.\n";
            }

            if (sError != "") {
                alert(sError);
                $('#<%=btnSubmit.ClientID %>').removeAttr('disabled');
            } else {
                $('#<%=btnSubmit.ClientID %>').removeAttr('disabled');
                fnDoLogin();
                //return true;
            }
            $('#<%=btnSubmit.ClientID %>').removeAttr('disabled');
        }

        function fnDoLogin() {
            var userName = $('#<%= txtUname.ClientID %>').val();
            var txtPwd = $('#<%= txtPwd.ClientID %>').val();
            var platform = window.location.href.toLowerCase().includes("wholesaleportal") || window.location.href.toLowerCase().includes("wp") ? "wholesaleportal" : "liquidconnect";
            toggleLoading(true, "Logging into " + (platform == "wholesaleportal" ? "Wholesale Portal..." : "Liquid Connect..."));

            $.ajax({
                method: "POST",
                url: '/WholesaleSystem/Login.aspx/UserLogin',
                data: `{'username':'${userName}', 'password':'${txtPwd}', 'platform': '${platform}'}`,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json'
            }).done(function (response) {
                var r = response.d;

                if (r.success) {
                    toggleLoading(true, "Success, loading " + (platform == "wholesaleportal" ? "Wholesale Portal..." : "Liquid Connect..."));
                    if (!IsLiquidConnect()) {
                        if (r.value == "SingleDealer")
                            window.location.href = "/WholesaleContent/VehicleManagement.aspx";
                        else if (r.value == "MultiDealer")
                            window.location.href = "/WholesaleContent/WholesaleDefault.aspx";
                    }
                    else {
                        if (r.value == "SingleDealer")
                            window.location.href = "/WholesaleContent/Vehicle/Search.aspx";
                        else if (r.value == "MultiDealer")
                            window.location.href = "/WholesaleContent/WholesaleDefault.aspx";
                    }

                } else {
                    $('#<%= lblMsg.ClientID %>').html(r.message);
                    toggleLoading(false, "");
                }
            });
        }

        function PasswordVisibility() {
            var icon = document.getElementById("ShowPassword");
            var passwordBox = document.getElementById("MainContent_txtPwd");

            if (passwordBox.getAttribute("type") == "password") {
                passwordBox.setAttribute("type", "text");
                icon.src = icon.src.replace("visibility_", "visibility_off_");
            }
            else {
                passwordBox.setAttribute("type", "password");
                icon.src = icon.src.replace("visibility_off_", "visibility_");
            }
        }

        $(document).ready(function () {
            if (IsLiquidConnect()) {
                document.getElementsByClassName("loginText")[0].innerHTML = "Liquid Connect Login";
                document.getElementsByClassName("d-none d-md-block col-sm-6 loginTitle")[0].innerHTML = '\n\t\t    Liquid Connect\n\t    ';
            }
        });
    </script>

    <div style="display:table;width:100%;">
        <div style="display:table-row">
            <div class="row maintenanceTitle" style="margin-top: 15px;"><asp:label ID="MaintenanceTitle" runat="server"/></div>
        </div>
        <div style="display:table-row;">
            <div class="row maintenanceMessage"><asp:label ID="MaintenanceMessage" runat="server" Style="text-align-last:center;"/></div>
        </div>
        <div style="display:table-row;">
            <div class="loginContent" style="margin-top:70px">
                <asp:Image CssClass="loginImage" runat="server" ImageUrl="~/Images/login.png" />
                <div class="loginValues">
                    <div class="loginText">Wholesale Portal Login</div>
                    <div class="col-xs-8 col-xs-offset-2 text-center">
                        <asp:textbox id="txtUname" runat="server" cssclass="textinput logintextbox" placeholder="User Name"></asp:textbox>
                    </div>
                    <div class="col-xs-8 col-xs-offset-2 text-center" style="display:table;width:100%">
                        <div style="display:table-row;text-align:center">
                            <asp:textbox id="txtPwd" runat="server" TextMode="Password" cssclass="textinput logintextbox" placeholder="Password"></asp:textbox>
                            <img id="ShowPassword" src="/Images/baseline_visibility_black_24dp.png" style="position: absolute;transform:translate(-120%,50%);cursor: pointer;height: 22px;opacity:50%" onclick="PasswordVisibility(); return false;"/>
                        </div>
                    </div>
                    <div class="col-xs-12 loginError">
                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                    </div>
                    <div class="col-xs-12 text-center">
                        <asp:Button ID="btnSubmit" CssClass="submitButton" Text="Log In" runat="server" OnClientClick="if (!fnSubmit()) {return false;}" />
                    </div>
                    <div class="col-xs-12 loginForgotPwd" runat="server"><a href="\WholesaleSystem\PasswordReset.aspx">Forgot your password?</a></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="FooterContent" ContentPlaceHolderID="FooterExtrasHolder" runat="server">
    <div class="row loginInfoPane">
        <div class="col-sm-4 col-xs-6 text-center">
		    Need Help?<br/>
            <span class="smallHide">&nbsp;<br/></span>
		    <a href="http://www.liquidmotors.com/register.aspx">Need an Account?</a><br/>
	    </div>
        <div class="col-sm-4 col-xs-6 text-center">
            Contact Support<br/>
		    Phone: (800) 260-1031<br/>
		    <a href="mailto:support@liquidmotors.com">support@liquidmotors.com</a><br/>
	    </div>
        <div class="d-none d-sm-block col-sm-4 text-center">
            Contact Sales<br/>
		    Phone: (877) 573-6877<br/>
		    <a href="mailto:sales@liquidmotors.com">sales@liquidmotors.com</a><br/>
	    </div>
    </div>
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <div class="row loginBanner">
        <div class="col-12 col-md-6 loginLogo">
            <img class="loginpagelogo" src='/Images/LMI_logo.png' />
        </div>
	    <div class="d-none d-md-block col-sm-6 loginTitle">
		    Wholesale Portal
	    </div>
    </div>
</asp:Content>
