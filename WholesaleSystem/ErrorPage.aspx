<%@ Page Title="Error Page" Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="LMWholesale.ErrorPage" %>

<!DOCTYPE html>

<html lang="en">
<webopt:bundlereference runat="server" path="~/Styles/css" />
<head runat="server">
    <title><%: Page.Title %> - Liquid Motors Wholesale Marketing</title>
    <link href="~/Styles/favicon.ico" rel="shortcut icon" type="image/x-icon" />
</head>
<body>
    <form runat="server">
        <div class="page-wrapper">
            <style type="text/css">
                .underline {text-decoration: underline;}
                @media (max-width: 767px) {.navbar {flex-wrap: unset;}}

                .errorDetails {
                    text-align: center;
                    height: 650px;
                }

                .errorContainer {
                    padding-top: 50px;
                    margin: 0 auto;
                    width: 750px;
                }

                .lbls {
                    text-decoration:underline;
                    font-weight:bold;
                }
            </style>

            <nav class="navbar navbar-expand-md navbar-lmi navbar-light bg-faded fixed-top navbar-lmi-border" id="WholesalePortalHeader" runat="server">
                <div class="navbar-brand navbarBranding">
                    <img id="LMLogo" src="/Images/lmi.png" class="headerLogo"/>
                    <div class="navbarProduct">
                        Wholesale Portal
                    </div>
                    <div class="navbarPageTitle">Error Page</div>
                </div>
            </nav>
            <div class="errorContainer">
                <fieldset id="errorResults" class="sectionFieldset errorDetails">
					<legend>Error Details</legend>
                    <br/>
                    <div style="text-align:left;">
                        <p>This feature is currently unavailable or not functioning properly at this time.</p>
                        <p>If reloading your last page causes the same error, please re-login and attempt the action again.</p>
                        <p>If this problem persists, please contact support with a description of the problem.</p>
                        <asp:label id="lblSession" runat="server" CssClass="lbls">Session ID:</asp:label>
                        <asp:label id="SessionId" runat="server"></asp:label>
                        <br/>
                        <asp:label id="lbluserName" runat="server" CssClass="lbls">User Name:</asp:label>
                        <asp:label id="UserName" runat="server"></asp:label>
                        <br/>
                        <asp:label id="lblPage" runat="server" CssClass="lbls">Page:</asp:label>
                        <asp:label id="PageName" runat="server"></asp:label>
                        <br/>
                        <asp:label id="lblSource" runat="server" CssClass="lbls">Source:</asp:label>
                        <asp:label id="Source" runat="server"></asp:label>
                        <br/>
                        <asp:label id="lblMessage" runat="server" CssClass="lbls">Message:</asp:label>
                        <asp:label id="Message" runat="server"></asp:label>
                        <br/>
                        <asp:label id="lblStack" runat="server" CssClass="lbls">Stack Trace:</asp:label>
                        <asp:label id="StackTrace" runat="server" Style="word-break:break-word"></asp:label>
                    </div>
				</fieldset>
            </div>
        </div>
        <footer class="page-footer row">
            <div class="col-12 col-sm-6 text-center npr-col">
                &copy; 2005 - <%: DateTime.Now.Year %> | Liquid Motors Inc. All Rights Reserved.
            </div>
            <div class="col-sm-6 text-center npr-col">
                <a href="http://www.liquidmotors.com/termsofuse.aspx" target="blank" >Terms of Use</a>
                | <a href="http://www.liquidmotors.com/copyright.aspx" target="blank">Copyright</a>
                | <a href="http://www.liquidmotors.com/privacy.aspx" target="blank">Privacy Policy</a>
            </div>
        </footer>

    </form>
</body>
</html>