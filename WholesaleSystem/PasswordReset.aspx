<%@ Page Title="Password Reset" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PasswordReset.aspx.cs" Inherits="LMWholesale.PasswordReset" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleSystem/PasswordReset.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
	<script type="text/javascript" src="/Scripts/WholesaleSystem/PasswordReset.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

	<div class="row loginContainer">
		<div class="col-12 col-md-6 col-md-offset-3 col-4 passwordWindow">
            <div id="PasswordContent" class="passwordContent" runat="server">
				<fieldset id="passwordFieldset" class="sectionFieldset">
					<legend>Password Reset Request</legend>
						<div style="display:table;margin:auto;padding:20px;">
							<div style="display:table-row;">
								<div style="display:table-cell;text-align:center;"><span id="FirstText" runat="server"></span></div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;text-align:center;"><span id="SecondText" runat="server"></span></div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;"><br/></div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;">
									<div style="display:table;margin:auto;">
										<div style="display:table-row;">
											<div class="boldTableCell">Username:</div>
											<div style="display:table-cell;"><asp:Textbox ID="UsernameInput" runat="server" style="" /></div>
										</div>
										<div id="EmailTextbox" style="display:table-row;">
											<div class="boldTableCell">Email:</div>
											<div style="display:table-cell;"><asp:Textbox ID="EmailInput" runat="server" style="" /></div>
										</div>
									</div>
								</div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;text-align:center;">
									<asp:CheckBox runat="server" ID="Identical" Text="&nbsp;&nbsp;Username and Email are identical" OnChange="IdenticalCheck();"/>
								</div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;"><br/></div>
							</div>
							<div style="display:table-row;text-align:-webkit-center;">
								<div style="display:table-cell;">
									<button onclick="SubmitRequest(); return false;" class="submitButton">Submit</button>&nbsp;&nbsp;<button class="submitButton" onclick="CancelRequest();return false;">Cancel</button>
								</div>
							</div>
						</div>
				</fieldset>
			</div>
			<div id="ResetContent" class="passwordContent" runat="server">
				<fieldset id="resetFieldset" class="sectionFieldset">
					<legend>Password Reset Confirmation</legend>
						<div class="ResetItems">
							<div style="display:table-row;">
								<div style="display:table-cell;text-align:center;"><span id="ResetText" runat="server"></span></div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;"><span id="passRequirement" runat="server"></span><br/></div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;">
									<div style="display:table;margin:auto;">
										<div style="display:table-row;">
											<div class="boldTableCell">Password:</div>
											<div style="display:table-cell;">
												<asp:Textbox ID="InitialPassword" runat="server" TextMode="Password" />
												<img id="ShowPassword1" src="/Images/baseline_visibility_black_24dp.png" class="PasswordVisibility" onclick="PasswordVisibility(1); return false;"/>
											</div>
										</div>
										<div style="display:table-row;">
											<div class="boldTableCell">Confirm Password:</div>
											<div style="display:table-cell;">
												<asp:Textbox ID="ConfirmPassword" runat="server" TextMode="Password" />
												<img id="ShowPassword2" src="/Images/baseline_visibility_black_24dp.png" class="PasswordVisibility" onclick="PasswordVisibility(2); return false;"/>
											</div>
										</div>
									</div>
								</div>
							</div>
							<div style="display:table-row;">
								<div style="display:table-cell;"><br/></div>
							</div>
							<div style="display:table-row;text-align:-webkit-center;">
								<div style="display:table-cell;">
									<button onclick="SubmitReset(); return false;" class="submitButton">Submit</button>&nbsp;&nbsp;<button class="submitButton" onclick="CancelRequest();return false;">Cancel</button>
								</div>
							</div>
						</div>
				</fieldset>
			</div>
		</div>
	</div>

	<script type="text/javascript">
		var pause_timeout = null;
		var initial = document.getElementById("MainContent_InitialPassword");
		var confirm = document.getElementById("MainContent_ConfirmPassword");
		if (initial && confirm) {
			OnChangePause(initial, RegexCheck, 500);
			OnChangePause(confirm, RegexCheck, 500);
		}
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

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
		    Password Reset
	    </div>
    </div>
</asp:Content>