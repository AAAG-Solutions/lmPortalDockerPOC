<%@ Page Title="Report Viewer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="LMWholesale.WholesaleContent.Reporting.ReportViewer" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/jsgrid-theme.min.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />
    <link type="text/css" rel="stylesheet" href="/Styles/LMGrid.css?lmV=<%Response.Write(Application["ContentVersion"]);%>" />

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Scripts/jsgrid.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

    <asp:HiddenField ID="hfMode" runat="server" />
    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>

    <script type="text/javascript" src="/Scripts/jsgrid.min.js"></script>
    <div id="grid" class="reportViewerContainer hide_scrollbar">
        <h6>&#60; &#8722; <a class='backBreadcrumb' href="javascript:window.location.href='/WholesaleContent/Reporting/Status.aspx';">Back To Dashboard</a></h6>
        <div style="text-align: center;font-weight: bold;padding-bottom:2px;">
            <asp:Label ID="lbTitle" runat="server" />
        </div>
        <div id="jsGrid"></div>
    </div>
</asp:Content>