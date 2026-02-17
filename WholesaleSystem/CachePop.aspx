<%@ Page Title="Cache Pop Page" Language="C#" AutoEventWireup="true" CodeBehind="CachePop.aspx.cs" Inherits="LMWholesale.CachePop" %>

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
            <script type="text/javascript" src="/Scripts/jquery-3.3.1.min.js"></script>
            <style type="text/css">
                .underline {text-decoration: underline;}
                @media (max-width: 767px) {.navbar {flex-wrap: unset;}}

                .cacheDetails {
                    text-align: center;
                }

                .cacheContainer {
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
                    <div class="navbarPageTitle">Cache Pop Page</div>
                </div>
            </nav>
            <div class="cacheContainer">
                <fieldset id="cacheResults" class="sectionFieldset cacheDetails">
					<legend>Pop the Cache</legend>
                    <br/>
                    <div style="text-align:center;">
                        <p>This page is intended to clear the cache on either a kPerson basis or 'GLOBAL'</p>
                    </div>
                    <div style="display:table;margin:auto;padding:10px;">
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel">CacheType:&ensp;</div>
                            <div class="ColRowSwap">
                                <asp:DropDownList runat="server" ID="lstCacheType" CssClass="inputStyle" Style="width:100%;">
                                    <asp:ListItem Value="1">kPerson</asp:ListItem>
                                    <asp:ListItem Value="2">Global</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="ColRowSwapLabel">kPerson:</div>
                            <div class="ColRowSwap"><asp:Textbox ID="kPerson" runat="server" CssClass="inputStyle SingleIndent"/></div>
                        </div>
                        <div style="display:table-row">
                            <div class="ColRowSwapLabel" style="width:25px;">CacheKey:&ensp;</div>
                            <div class="ColRowSwap" style="width:25px;">
                                <asp:DropDownList runat="server" ID="lstCacheKey" CssClass="inputStyle">
                                    <asp:ListItem Value="ANY">ANY</asp:ListItem>
                                    <asp:ListItem Value="DealerUsers">DealerUsers</asp:ListItem>
                                    <asp:ListItem Value="availableAuctions">availableAuctions</asp:ListItem>
                                    <asp:ListItem Value="AutoLaunchRuleSets">AutoLaunchRuleSets</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="ColRowSwapLabel">&ensp;CacheKeyPart:&ensp;</div>
                            <div class="ColRowSwap"><asp:Textbox ID="CacheKey" runat="server" CssClass="inputStyle SingleIndent"/></div>
                        </div>
                    </div>
                    <div style="width:100%;text-align-last:center;">
                        <input id="btnPopCache" type="button" class='actionBackground headerButton' value="Pop Cache" onclick="clearCache()">
                    </div>
                    <div style="text-align:left;">
                        <p>'kPerson' Keys:
                            <br>&emsp;kDealer + 'DealerUsers'
                            <br>&emsp;'availableAuctions' + enabled ( 0 | 1 )
                            <br>&emsp;'AutoLaunchRuleSets' + kDealer + kDealerGaggle + kGaggleSubGroup
                        </p>
                        <p>'GLOBAL' Keys:
                            <br>&emsp;'DealerDashboard' - CustomHeaderDef, CustomerTypeDef, PartnerSessionDef, AllySetup
                            <br>&emsp;'PortalHome' - DescriptionBuilderXML, DealerTradeCenterDef
                        </p>
                    </div>
                    <br/>
                    <div style="width:100%;text-align-last:center;">
                        <input id="btnBackToLogin" type="button" class='actionBackground headerButton' value="Back to Login" onclick="window.location = '/WholesaleSystem/Login.aspx'">
                    </div>
				</fieldset>
            </div>
            <script type="text/javascript">
                function clearCache() {
                    if (document.getElementById("lstCacheType").value == "1" && document.getElementById("kPerson").value == "") {
                        alert("kPerson needed for 'kPerson' cache pop");
                        return false;
                    }

                    var json = {
                        cacheType: document.getElementById("lstCacheType").value,
                        kPerson: document.getElementById("kPerson").value,
                        cacheKey: document.getElementById("lstCacheKey").value,
                        cacheKeyPart: document.getElementById("CacheKey").value
                    };

                    $.ajax({
                        type: "POST",
                        url: 'CachePop.aspx/Pop',
                        data: JSON.stringify(json),
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert(XMLHttpRequest.responseJSON.Message);
                        },
                        success: function (response) {
                            if (response.d.success == true) {
                                return false;
                            }
                            else {
                                alert(response.d.message);
                                return false;
                            }
                        }
                    });
                }
            </script>
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