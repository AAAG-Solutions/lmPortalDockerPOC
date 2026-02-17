<%@ Page Title="Update Vehicle" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Update.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.Update" Async="true" AsyncTimeout="30" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Import Style Sheets -->
    <link type="text/css" rel="stylesheet" media="all" href="/Common/LightGallery/lightGallery.css?lmV=<%Response.Write(Application["ContentVersion"]);%>">
    <!--<link type="text/css" rel="stylesheet" media="all" href="https://content.liquidmotors.com/ncr/css/bootstrap.css">-->
    <!--<link type="text/css" rel="stylesheet" media="all" href="https://content.liquidmotors.com/ncr/css/main.css">-->

    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/Update.css?lmV=<%Response.Write(Application["ContentVersion"]);%>">
    <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/PhotoGallery.css?lmV=<%Response.Write(Application["ContentVersion"]);%>">

    <!-- Import Page Scripts -->
    <script type="text/javascript" src="/Common/LightGallery/jquery-1.10.2.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Common/LightGallery/lightgallery.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Common/LightGallery/lg-thumbnail.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Common/LightGallery/lg-fullscreen.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Common/LightGallery/lg-zoom.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Common/LightGallery/lg-video.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript" src="/Scripts/WholesaleContent/Vehicle/Update.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
    <script type="text/javascript">
        function ShowAll() {
            $('#slider').flexslider({
                animation: "slide",
                controlNav: false,
                animationLoop: false,
                slideshow: false,
                itemWidth: 400,
                sync: "#carousel",
                start: function (slider) {
                    $('body').removeClass('loading');
                }
            });
            //$('#carousel').flexslider({
            //    animation: "slide",
            //    controlNav: false,
            //    animationLoop: false,
            //    slideshow: false,
            //    itemWidth: 10,
            //    itemMargin: 5,
            //    asNavFor: '#slider'
            //});
        }

        $('#lightgallery').lightGallery({
            mode: 'lg-fade',
            cssEasing: 'cubic-bezier(0.25, 0, 0.25, 1)',
            download: false
        });

        $(document).ready(function () {
            $("#lightgallery").lightGallery();
            $("#lightgallery_normal").lightGallery();
            $("#lightgallery_damage").lightGallery();

            $(".damageSlideShow").on('click', function () {
                var slideID = $(this).attr('data-slide');
                $('#' + slideID).trigger('click');
            })

            if (IsLiquidConnect()) {
                document.getElementById("MainContent_actionStartWholesale").style.display = 'none';
                document.getElementById("MainContent_actionEndWholesale").style.display = 'none';
                document.getElementById("MainContent_actionChangeVIN").style.display = 'none';
                document.getElementById("MainContent_actionDeleteVehicle").style.display = 'none';
            }
        });
    </script>
    <script src="/Common/LightGallery/jquery.flexslider-min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

    <div id="sidebar_menu" class="sidebar hide_scrollbar"></div>
    <div id="makeModel">
        <div id="addMakeModel" class="modalPopup">
            <div class="modalContent">
                <div class="modalHeader">
                    <span class="closeModalButton" onclick="ClearModal(); return false;">&times;</span>
                    <h2 style="text-align: center;">Add Make / Model</h2>
                </div>
                <div class="modalBody">
                    <div id="makeModelTbl" runat="server" style="display:table;width:100%;">
                        <div class="ColRowSwap">
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;font-weight:bold;text-decoration:underline;">Make</div>
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;">
                                <asp:DropDownList runat="server" ID="lstAddMake" CssClass="SingleIndent inputStyle" onChange="javascript:MakeSelection(this.value, 'lstAddModel'); return false;"></asp:DropDownList>
                            </div>
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;">
                                <input type="text" ID="typeAddMake" runat="server" class="inputStyle SingleIndent"/>
                                <input value="Add Make" type="submit" ID="submitMake" runat="server" class="actionBackground SingleIndent" style="width:100px;" onclick="javascript:AddFeature('Make'); return false;"/>
                            </div>
                        </div>
                        <div class="ColRowSwap">
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;font-weight:bold;text-decoration:underline;">Model</div>
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;">
                                <asp:DropDownList runat="server" CssClass="SingleIndent inputStyle" ID="lstAddModel"></asp:DropDownList>
                            </div>
                            <div class="SmallLeftAlign" style="text-align:-webkit-center;padding:2px;">
                                <input type="text" ID="typeAddModel" runat="server" class="inputStyle SingleIndent"/>
                                <input value="Add Model" type="submit" ID="submitModel" runat="server" class="actionBackground SingleIndent" style="width:100px;" onclick="javascript:AddFeature('Model'); return false;"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="updateVehicle hide_scrollbar">
        <h6 style="font-weight: bold;">Inventory / Vehicle / Update</h6>
        <h6>&#60; &#8722; <a class='backBreadcrumb' href='javascript: window.location.href="/WholesaleContent/VehicleManagement.aspx"'>Back To Inventory</a></h6>
        <div id="vehicleHeaderInfo" class="row">
            <div class="col">
                <div class="sectionFieldset" style="width: 100%; padding: 20px;">
                    <div id="DataDisplay" style="display:table;width:100%;">
                        <div id="updatePhoto" class="HeaderCol" style="vertical-align:middle; text-align:center">
                            <a id="PhotoLink" target="_blank" runat="server"><img id="PhotoItem" src="/Images/not_available.gif" alt="" runat="server" style="max-width:320px;" /></a>
                        </div>
                        <div id="Vehicle" class="HeaderCol">
                            <div id="headerVehicleInfo" class="row">
                                <div class="col" style="padding:0px;">
                                    <div class="row" style="text-align: left;">
                                        <div class="col">
                                            <h2><asp:Label ID="headerVehicle" CssClass="H2Text" runat="server"/></h2>
                                        </div>
                                    </div>
                                    <div id="HeaderInfo" class="HeaderHolder" style="display:table;width:60%;">
                                        <asp:Label ID="HeaderID" runat="server" CssClass="hide"/>
                                        <div id="HeaderLeft" class="HeaderCol">
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">VIN: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderVIN" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Year: </div>
                                                <div class="HeaderCol" style="padding:2px;height:32px;"><asp:Label ID="VehicleYear" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Make: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="VehicleMake" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Model: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="VehicleModel" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Style: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="VehicleStyle" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Mileage: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="VehicleMiles" CssClass="HeaderItemContent" runat="server"></asp:Label></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Stock Type: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="StockType" CssClass="HeaderItemContent" runat="server"></asp:Label></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Status: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="tbStatus" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                        </div>
                                        <div id="HeaderRight" class="HeaderCol">
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Age: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderAge" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Stock: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderStock" CssClass="HeaderItemContent" runat="server" /></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Account: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderLocation" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Lot Location: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="LotLocation" CssClass="HeaderItemContent" runat="server" /></div>
                                            </div>
                                            <div style="display:table-row;">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">MMR: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderMMR" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Cost: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderCost" CssClass="HeaderItemContent" runat="server"/></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Retail List Price: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="HeaderRetailListPrice" CssClass="HeaderItemContent" runat="server" /></div>
                                            </div>
                                            <div style="display:table-row">
                                                <div class="HeaderLabelCol" style="padding:2px;font-weight:bold;text-align:right;">Retail Internet Price: </div>
                                                <div class="HeaderCol" style="padding:2px;"><asp:Label ID="InternetPrice" CssClass="HeaderItemContent" runat="server"></asp:Label><asp:HiddenField ID="iPrice" runat="server" Value="" /></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="saveDetails">
                                        <div id="savePrompt" class="modalPopup">
                                            <div class="modalContent">
                                                <div class="modalHeader">
                                                    <span class="closeModalButton" onclick="toggleCssClass([['savePrompt','show_display']]); document.getElementById('MainContent_SavePass').value = '';return false;">&times;</span>
                                                    <h2 style="text-align: center;">Confirm Vehicle Detail Updates</h2>
                                                </div>
                                                <div class="modalBody">
                                                    <br/>
                                                    <div style="text-align-last:center;font-weight:bold;">Confirm changes to <u>pricing, lot location and/or vehicle status</u>, clicking this dialog closed will have the same effect as a cancel.</div>
                                                    <br/>
                                                    <div style="text-align-last:center;font-weight:bold;">Enter your password:&nbsp;<asp:TextBox ID="SavePass" runat="server" TextMode="Password"></asp:TextBox></div>
                                                    <br/>
                                                    <div style="width:100%;text-align-last:center;"><asp:Button ID='detailsSave' OnClientClick="DetailsSave(); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:125px'/></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="emptyRow" class="row">&nbsp;</div>
                    <div id="headerActions" class="row" style="justify-content: center;">
                        <div class="row" style="display:table;text-align:center;">
                            <asp:Button ID='actionStartWholesale' runat='server' Text='Start Wholesale' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                            <asp:Button ID='actionEndWholesale' OnClientClick="return false;" runat='server' Text='End Wholesale' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                            <asp:Button ID='actionCR' OnClientClick="return false;" runat='server' Text='Condition Report' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                            <asp:Button ID='actionChangeVIN' OnClientClick="return false;" runat='server' Text='Change VIN' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                            <asp:Button ID='actionModifyPhotos' runat='server' Text='Modify Photos' CssClass='actionBackground' Style='width:150px;margin-right: 5px;display:none;' AutoPostback="false"/>
                            <asp:Button ID='actionUploadPhotos' runat='server' Text='Upload Photos' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                            <asp:Button ID='actionDeleteVehicle' OnClientClick="return false;" runat='server' Text='Delete Vehicle' CssClass='actionBackground' Style='width:150px;margin-right: 5px;' AutoPostback="false"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <fieldset id="VehicleDetails" class="smallVehicleDetails sectionFieldset">
            <legend>Vehicle Details</legend>
            <div id="AdditionalInfo" style="margin:0 10px 0 0">
                <div class="row" id="divSelectors" style="display:flex;justify-content:center;">
                    <asp:Button ID='generalTab' OnClientClick="CSSSelector('general'); return false;" runat='server' Text='General' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='listingTab' OnClientClick="CSSSelector('listing'); return false;" runat='server' Text='Listing Info' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='pricingTab' OnClientClick="CSSSelector('pricing'); return false;" runat='server' Text='Pricing' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='notesTab' OnClientClick="CSSSelector('notes'); return false;" runat='server' Text='Notes' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='descCondTab' OnClientClick="CSSSelector('descCond'); return false;" runat='server' Text='Desc/Cond' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='colorBodyTab' OnClientClick="CSSSelector('colorBody'); return false;" runat='server' Text='Color/Body' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='driveTrainTab' OnClientClick="CSSSelector('driveTrain'); return false;" runat='server' Text='Drive Train' CssClass='actionBackground' Style='width:125px;margin-right: 5px;'/>
                    <asp:Button ID='optionsTab' OnClientClick="OptionsSelect(); return false;" runat='server' Text='Options' CssClass='actionBackground' Style='width:125px;margin-right:5px;'/>
                </div>
                <div id="divGeneral" runat="server" class="row">
                    <div class="col">
                        <fieldset id="GeneralInfo" class="sectionFieldset" style="position:relative;">
                            <legend>General Info</legend>
                            <div style="display:flex;flex-wrap:wrap;justify-content:space-between;">
                                <div id="LeftHolder" style="flex-basis:35%">
                                    <div style='display:table;margin: 0 auto;width:100%;border-spacing:5px;'>
                                        <div class='singleRow'>
                                            <div class='centerCell'><b>Year:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:DropDownList runat="server" Visible="false" ID="YearLst"  CssClass="inputStyle HeaderItemContent" onchange="GetList(this.options, 'Year');"></asp:DropDownList>
                                                <asp:Label runat="server" ID="YearTxt" CssClass="HeaderItemContent"></asp:Label>
                                            </div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell' style='width:50px;'></div>
                                            <div class='centerContent' style="width:100px;"><asp:Button ID='btnAddMakeModel' runat='server' CssClass='actionBackground HeaderItemContent' Text="Add Make/Model" Style='width:140px;height:30px;text-align-last:center;' OnClientClick="javascript:toggleCssClass([['addMakeModel', 'show_display']]);return false;" /></div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell'><b>Make:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:DropDownList runat="server" Visible="false" ID="MakeLst" CssClass="inputStyle HeaderItemContent" onChange="javascript:GetList(this.options, 'Make'); return false;"></asp:DropDownList>
                                                <asp:Label runat="server" ID="MakeTxt" CssClass="HeaderItemContent">-- No Make Found --</asp:Label>
                                                <asp:HiddenField ID="kMake" runat="server" Value="" />
                                            </div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell'><b>Model:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:DropDownList runat="server" Visible="false" ID="ModelLst" CssClass="inputStyle HeaderItemContent" onChange="javascript:GetList(this.options, 'Model'); return false;"></asp:DropDownList>
                                                <asp:Label runat="server" ID="ModelTxt" CssClass="HeaderItemContent">-- No Model Found --</asp:Label>
                                                <asp:HiddenField ID="kModel" runat="server" Value="" />
                                            </div>
                                        </div>
                                        <div class='singleRow'>
                                            <asp:DropDownList runat="server" ID="MasterStyleLst" style="display:none;"></asp:DropDownList>
                                            <asp:DropDownList runat="server" ID="ExpStyleLst" style="display:none;"></asp:DropDownList>
                                            <asp:DropDownList runat="server" ID="SubModelLst" style="display:none;"></asp:DropDownList>
                                            <asp:HiddenField runat="server" id="StyleHidden" Value=""/>
                                            <div class='centerCell'><b>Style:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:DropDownList runat="server" Visible="false" ID="StyleLst" CssClass="inputStyle HeaderItemContent" onChange="javascript:StyleSelection(this.options);return false;"></asp:DropDownList>
                                                <asp:Label runat="server" ID="StyleTxt" CssClass="HeaderItemContent">-- No Style Found --</asp:Label>
                                                <asp:HiddenField ID="StyleId" runat="server" Value="" />
                                            </div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell' style="width:150px;"><b>Style Override:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox ID='FullDesc' runat='server' CssClass="inputStyle HeaderItemContent" style='width:400px;'></asp:TextBox><br/>(Full Description)</div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell'><input ID='chkOverrideStyle' type='checkbox' runat='server' CssClass="HeaderItemContent" onchange="chkOverride('style');return false;"></div>
                                            <div class='centerContent'><asp:TextBox ID='LimitDesc' runat='server' CssClass="inputStyle HeaderItemContent" style='width:400px;'></asp:TextBox><br/>(Limited to 12 Characters)</div>
                                        </div>
                                    </div>
                                </div>
                                <div id="MiddleHolder">
                                    <div style='display:table;margin: 0 auto;width:100%;border-spacing:5px;'>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Vehicle Status:&nbsp;</b></div>
                                            <div class='centerContent'><asp:DropDownList ID='VehicleStatusLst' runat='server' CssClass="inputStyle HeaderItemContent" ></asp:DropDownList></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Cetrified/Used:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:HiddenField ID="kVehicleListStatus" runat="server" Value="" />
                                                <asp:DropDownList ID='VehicleListStatusLst' runat='server' CssClass="inputStyle HeaderItemContent" OnChange="javascript: SelectionSet(this);CertCheck(this);">
                                                    <asp:ListItem Text="Pre-Owned" Value="32" />
                                                    <asp:ListItem Text="Dealer Certified Pre-Owned" Value="33" />
                                                    <asp:ListItem Text="Certified Pre-Owned" Value="30" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="singleRow">
                                            <asp:HiddenField ID="kCertificationType" runat="server" Value="" />
                                            <div class='centerCell'><b>&nbsp;Certification Type:&nbsp;</b></div>
                                            <div class='centerContent'><asp:DropDownList ID='lstCertificationType' runat='server' CssClass="inputStyle HeaderItemContent" OnChange="javascript: SelectionSet(this);" /></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Certification &#x23:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox runat="server" ID="CertificationNumber" class="inputStyle SingleIndent" Value=""></asp:TextBox></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Certification Date:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox runat="server" ID="CertificationDate" class="inputStyle SingleIndent" TextMode="Date" Value=""></asp:TextBox></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Mileage:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox ID='VehicleMileage' runat='server' CssClass="inputStyle HeaderItemContent" style="text-align:right;width:150px;"></asp:TextBox></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Lot Location:&nbsp;</b></div>
                                            <div class='centerContent'>
                                                <asp:DropDownList ID="lstVehicleLotLoc" runat="server" CssClass="inputStyle HeaderItemContent" style="padding-top:1px;height:25px;" onChange="javascript:LotLocationChange(this.value);"/>
                                            </div>
                                            <div>
                                                <asp:TextBox ID="InvLotLocation" runat="server" CssClass="inputStyle HeaderItemContent" Style="height:25px;"/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="RightHolder">
                                    <div style='display:table;margin: 0 auto;width:100%;border-spacing:5px;'>
                                        <div class="singleRow">
                                            <div class='centerContent' style='text-align:end;'><label for='overImport' style='font-weight:bold;'>&nbsp;Override Import:&nbsp;</label></div>
                                            <div class='centerCell' style='text-align:-webkit-left;'><input id='overImport' type='checkbox' runat='server' class="HeaderItemContent" style='font-weight:bold;'></div>
                                        </div>
                                        <div class="singleRow">
                                            <div class='centerCell'><b>&nbsp;Key Code:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox ID='KeyCode' runat="server" CssClass="inputStyle HeaderItemContent" style="width:100px;"></asp:TextBox></div>
                                        </div>
                                        <div class='singleRow'>
                                            <div class='centerCell'><label style='font-weight:bold;'>&nbsp;Type of Vehicle:&nbsp;</label></div>
                                            <div class='centerContent'>
                                                <asp:HiddenField ID="kVehicleClass" runat="server" Value="" />
                                                <asp:Label ID='VehicleTypeName' CssClass="HeaderItemContent" runat='server'></asp:Label>&nbsp;&nbsp;
                                                <asp:Button ID='btnUpdateVehicleType' runat='server' Text='Update' CssClass='actionBackground' Style='width:75px;height:30px;text-align-last:center;' OnClientClick="VehicleTypeUpdateModal();return false;" />
                                            </div>
                                        </div>
                                        <div runat="server" class="singleRow" id="CorpInvAdded" style="display:none;">
                                            <div class='centerCell'><b>&nbsp;Corporate Date:&nbsp;</b></div>
                                            <div class='centerContent'><asp:TextBox runat="server" ID="CorpInvAddedDate" class="inputStyle SingleIndent" TextMode="Date" Value=""></asp:TextBox></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="vehicleTypeChange">
                                <div id="changePrompt" class="modalPopup">
                                    <div class="modalContent">
                                        <div class="modalHeader">
                                            <span class="closeModalButton" onclick="toggleCssClass([['changePrompt','show_display']]);return false;">&times;</span>
                                            <h2 style="text-align: center;">Change Vehicle Type</h2>
                                        </div>
                                        <div class="modalBody">
                                            <br/>
                                            <div id="classLst" style="text-align-last:center;"><label style='font-weight:bold;'>&nbsp;Type of Vehicle:&nbsp;</label>
                                                <asp:DropDownList ID="vehicleClassLst" runat="server" CssClass="dropdownselect inputStyle" Width="200px" AutoPostBack="false">
                                                    <asp:ListItem Selected="True" Value="1">Passenger Vehicle</asp:ListItem>
                                                    <asp:ListItem Value="2">Motorcycle</asp:ListItem>
                                                    <asp:ListItem Value="3">Commercial Vehicle</asp:ListItem>
                                                    <asp:ListItem Value="4">Miscellaneous</asp:ListItem>
                                                    <asp:ListItem Value="5">RV</asp:ListItem>
                                                    <asp:ListItem Value="6">Boat</asp:ListItem>
                                                    <asp:ListItem Value="7">ATV</asp:ListItem>
                                                    <asp:ListItem Value="8">Airplane</asp:ListItem>
                                                    <asp:ListItem Value="9">Watercraft</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div style="width:100%;text-align-last:center;"><asp:Button ID='changeTypeButton' OnClientClick="VehicleTypeUpdate(); return false;" runat='server' Text='Submit' CssClass='actionBackground' Style='width:125px'/></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <div id="divListingInfo" runat="server" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="ListingInfo" class="sectionFieldset" style="position:relative;">
                            <legend>Listing Information</legend>
                            <div id="AuctionInfo" runat="server" style="height:400px;overflow:overlay;"></div>
                            <br/>
                        </fieldset>
                    </div>
                </div>
                <div id="divPricing" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="Pricing" class="sectionFieldset" style="position:relative;">
                            <legend>Pricing</legend>
                            <div style='display:table;width:100%;'>
                                    <div class='singleRow'>
                                        <div class='centerCell'><b>Cost:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='VehicleCostGeneral' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;MSRP:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='MSRP' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;Retail List Price:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='ListPriceGeneral' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;Retail Internet Price:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='InternetPriceGeneral' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                    </div>
                                    <div class='singleRow'>
                                        <div class='centerCell'><b>Invoice:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='InvoicePrice' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;Payment:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='Payment' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                    </div>
                                    <div class='singleRow'>
                                        <div class='centerCell'><b>Wholesale Start:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='WholesaleStart' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;Wholesale Floor:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='WholesaleFloor' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;Wholesale Buy-It-Now:&nbsp;</b></div>
                                        <div class='centerContent'><asp:TextBox ID='WholesaleBIN' runat="server" style='width:100px;text-align:right;' CssClass="inputStyle HeaderItemContent"></asp:TextBox></div>
                                        <div class='centerCell'><b>&nbsp;MMR Value:&nbsp;</b></div>
                                        <div class='centerContent'><asp:Label id='MMRGoodPrice' runat='server' CssClass="HeaderItemContent" style='width:100px;text-align:right;'></asp:Label></div>
                                    </div>
                                </div>
                            <fieldset id='customPrice' class='sectionFieldset' runat="server" style='position:relative;padding:10px;'>
                                <legend>Custom Price Fields</legend>
                                <div style='display:table;width:90%'>
                                    <div class='singleRow'>
                                        <div id="cp1" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName1" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue1' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp2" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName2" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue2' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp3" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName3" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue3' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                    </div>
                                    <div class='singleRow'>
                                        <div id="cp4" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName4" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue4' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp5" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName5" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue5' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp6" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName6" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue6' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                    </div>
                                    <div class='singleRow'>
                                        <div id="cp7" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName7" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue7' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp8" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName8" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue8' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                        <div id="cp9" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName9" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue9' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                    </div>
                                    <div class='singleRow'>
                                        <div id="cp10" runat="server" class='centerCell' style="display: none;">
                                            <asp:Label ID="CustomPriceName10" runat="server" CssClass="HeaderCol" Style="display: none;font-weight:bold"/>&nbsp;
                                            <asp:TextBox ID='CustomPriceValue10' runat="server" style='width:100px;text-align:right;display: none;' CssClass="inputStyle HeaderItemContent HeaderCol" />
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </fieldset>
                    </div>
                </div>
                <div class="row" id="divVehicleNotes" style="display:none;">
                    <div class="col">
                        <fieldset class="sectionFieldset" style="position:relative;">
                            <legend>Vehicle Notes</legend>
                            <div id="VehicleNotes" runat="server" style="height:350px;overflow:overlay;"></div>
                            <div id="vehicleNotesButtons" class="SmallLeftAlign" style="text-align:center;">
                                <button id="AddVehicleNote" onclick="javascript:toggleCssClass([['AddNote','show_display']]);return false;" class="submitButton" style="width:150px;">Add New Note</button>
                                <button id="ViewVehicleNotes" onclick="javascript: GoToVehicleNotes();return false;" class="submitButton" style="width:125px;">View Notes</button>
                            </div>
                        </fieldset>
                        <div id="AddNote" class="modalPopup">
                            <div class="modalContent">
                                <div class="modalHeader">
                                    <span class="closeModalButton" onclick="toggleCssClass([['AddNote','show_display']]); return false;">&times;</span>
                                    <h2 style="text-align: center;">Add Vehicle Note</h2>
                                </div>
                                <div class="modalBody">
                                    <div style="display:table;margin: 0 auto;">
                                        <div class="singleRow">
                                            <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Vehicle:</div>
                                            <div class="centerContent">&nbsp;&nbsp;<asp:Label ID="VehicleNoteTitle" runat="server"></asp:Label></div>
                                        </div>
                                        <div class="singleRow" style="height:150px;">
                                            <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;vertical-align:top;">Note:</div>
                                            <div class="centerContent">&nbsp;&nbsp;
                                                <asp:textbox id="VehicleNotesText" placeholder="Type, paste, cut text here..." cols="50" textmode="MultiLine" runat="server" Style="width:90%">
                                                </asp:textbox></div>
                                        </div>
                                    </div>
                                    <br/>
                                    <div style="width:100%;text-align-last:center;"><asp:Button ID="btnSaveNote" OnClientClick="VehicleNoteSet(); return false;" runat='server' Text='Save Vehicle Note' CssClass='actionBackground' Style='width:175px'/></div>
                                    <br/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="divDescCond" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="DescCond" class="sectionFieldset" style="position:relative;">
                            <legend>Description/General Condition</legend>
                            <div class="row">
                                <div class="col">
                                    <div style="display:none;">
                                        <div class='centerCell'><b>Listing Title:&nbsp;</b></div>
                                        <div class='centerContent'>
                                            <asp:TextBox ID="ListingTitle" runat="server" style='width:500px;'></asp:TextBox>&nbsp;&nbsp;
                                        </div>
                                    </div>
                                    <div>
                                        <div><b>&nbsp;Vehicle Description:&nbsp;</b></div>
                                        <div style="padding:2%;"><asp:textbox id="VehicleDescriptionGeneral" runat="server" placeholder="Type, paste, cut text here..." cols="50" TextMode="MultiLine" CssClass="UpdateTextArea inputStyle" /></div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div>
                                        <b>&nbsp;General Condition Notes:&nbsp;</b>
                                    </div>
                                    <div style="padding:2%;">
                                        <asp:textbox id="txtConditionNotes" runat="server" placeholder="Type, paste, cut text here..." cols="50" TextMode="MultiLine" CssClass="UpdateTextArea inputStyle" />
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <div id="divColorBody" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="ColorBody" class="sectionFieldset" style="position:relative;">
                            <legend>Color/Body Info</legend>
                            <div style='display:table;width:100%;'>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Exterior Color:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='ExtColorLst' runat="server" CssClass="HeaderItemContent inputStyle Header" /></div>
                                    <div class='centerCell'><b>&nbsp;Body:&nbsp;</b></div>
                                    <div class='centerContent'>
                                        <asp:DropDownList ID='BodyLst' runat="server" CssClass="HeaderItemContent inputStyle Header" />
                                        <asp:Label ID='VehicleBody' runat='server' CssClass="HeaderItemContent"></asp:Label>
                                    </div>
                                </div>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Interior Color:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='IntColorLst' runat="server" CssClass="HeaderItemContent inputStyle" /></div>
                                    <div class='centerCell'><b>&nbsp;Vehicle Type:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='VehicleTypeLst' runat="server" CssClass="HeaderItemContent inputStyle" /><asp:Label ID='VehicleTypeTxt' runat='server' CssClass="SingleIndent"></asp:Label></div>
                                    <asp:HiddenField ID="kBodyType" runat="server" Value="" />
                                </div>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Doors:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='DoorLst' runat="server" CssClass="HeaderItemContent inputStyle" OnChange="javascript: SelectionSet(this);"/><asp:Label ID='DoorText' runat='server' CssClass="SingleIndent"></asp:Label></div>
                                    <asp:HiddenField ID="kDoor" runat="server" Value="" />
                                </div>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Roof:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='RoofLst' runat="server" CssClass="HeaderItemContent inputStyle" OnChange="javascript: SelectionSet(this);"/><asp:Label ID='RoofText' runat='server' CssClass="SingleIndent"></asp:Label></div>
                                    <asp:HiddenField ID="kRoof" runat="server" Value="" />
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <div id="divDriveTrain" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="DriveTrain" class="sectionFieldset" style="position:relative;">
                            <legend>Drive Train</legend>
                            <div style='display:table;width:100%;'>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Engine:&nbsp;</b></div>
                                    <div class='centerContent'>
                                        <asp:DropDownList runat="server" Visible="false" ID="EngineLst" CssClass="inputStyle HeaderItemContent"></asp:DropDownList>
                                        <asp:Label ID="VehicleEngine" CssClass="HeaderItemContent" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <div class='singleRow'>
                                    <div class='centerCell'><b>Engine Override:&nbsp;</b></div>
                                    <div class='centerContent'><input ID='chkOverrideEngine' type='checkbox' runat='server' CssClass="HeaderItemContent" style='width:25px;' onchange="chkOverride('engine');return false;"><asp:TextBox ID="OverrideEngineDesc" CssClass="HeaderItemContent inputStyle" runat="server" Enabled="true" Style="width:100%"></asp:TextBox></div>
                                </div>
                                <!-- kVehicleClass 1 Info -->
                                <div ID="VehicleClass1Drive" class='singleRow' runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Drive:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class1DriveTrainLst' runat='server' OnChange="javascript: SelectionSet(this);" CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class1DriveTrainTxt" runat="server" CssClass="HeaderItemContent" style='width:100px;' /></div>
                                    <asp:HiddenField ID="kDrive" runat="server" />
                                    <div class='centerCell'><b>&nbsp;Fuel:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class1FuelLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID='Class1FuelText' runat="server" CssClass="HeaderItemContent"/></div>
                                    <asp:HiddenField ID="kFuel" runat="server" />
                                </div>
                                <div ID="VehicleClass1Cyl" class='singleRow' runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Cylinders:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class1CylinderLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID='Class1CylindersTxt' runat="server" CssClass="HeaderItemContent"/></div>
                                    <asp:HiddenField ID="kCyl" runat="server" />
                                </div>
                                <div ID="VehicleClass1Trans" class='singleRow' runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Transmission:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class1TransmissionLst' runat="server" CssClass="HeaderItemContent inputStyle transmissionStyle"/></div>
                                </div>
                                <!-- kVehicleClass 2 Info -->
                                <div ID="VehicleClass2Cycle" class='singleRow' runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Cycle Type:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class2Cycle' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class2CycleTxt" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Engine Size (cc):&nbsp;</b></div>
                                    <div class='centerContent'><asp:TextBox ID="Class2EngineSizeTxt" runat="server" CssClass="HeaderItemContent inputStyle" style='width:125px;'></asp:TextBox></div>
                                </div>
                                <div ID="VehicleClass2Color" class='singleRow' runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Color:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class2Color' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class2ColorTxt" runat="server" style='width:100px;' /></div>
                                </div>
                                <!-- kVehicleClass 3 Info -->
                                <div ID="VehicleClass3Type" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Truck Type:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3TruckTypeLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3TruckTxt" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Truck Class:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3TruckClassLst' runat='server' CssClass="HeaderItemContent inputStyle"/></div>
                                </div>
                                <div ID="VehicleClass3Engine" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Engine Make:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3EngineMakeLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3EngineTxt" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Engine Size (HP):&nbsp;</b></div>
                                    <div class='centerContent'><asp:TextBox ID="Class3EngineSizeTxt" runat="server" CssClass="HeaderItemContent inputStyle" style='width:125px;'></asp:TextBox></div>
                                </div>
                                <div ID="VehicleClass3Suspension" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Axles:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3AxleLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3AxleTxt" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Suspension Type:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3SuspensionTypeLst' runat='server' CssClass="HeaderItemContent inputStyle"/></div>
                                </div>
                                <div ID="VehicleClass3Fuel" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Fuel Type:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3FuelLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3FuelText" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Tire Size:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3TireSizeLst' runat='server' CssClass="HeaderItemContent inputStyle"/></div>
                                </div>
                                <div ID="VehicleClass3Trans" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Transmission:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3TransmissionLst' runat='server' CssClass="HeaderItemContent inputStyle" style="max-width:100%"/><asp:Label ID="Class3TransmissionTxt" runat="server" style='width:100px;' /></div>
                                </div>
                                <div ID="VehicleClass3Cyl" class="singleRow" runat="server" style="display:none !important;">
                                    <div class='centerCell'><b>Transmission Speed:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3TransmissionSpeedLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3TransmissionSpeedTxt" runat="server" style='width:100px;' /></div>
                                    <div class='centerCell'><b>&nbsp;Cylinders:&nbsp;</b></div>
                                    <div class='centerContent'><asp:DropDownList ID='Class3CylnderLst' runat='server' CssClass="HeaderItemContent inputStyle"/><asp:Label ID="Class3CylinderText" runat="server" style='width:100px;' /></div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
                <div id="divOptions" class="row" style="display:none;">
                    <div class="col">
                        <fieldset id="Options" class="sectionFieldset" style="position:relative;">
                            <legend>Options</legend>
                            <asp:HiddenField ID="optionsLst" runat="server" Value="" />
                            <div id="availableCommonOptions" runat="server" class="tableHeader" style="width:10%;display: table-cell;" onclick="javascript:ToggleOptionsPane('tblCommonOptions')"></div>
                            <div id ="tblCommonOptions" class="openOptions">
                                <div id="CommonOptions" runat="server" style='display:table;margin: 0 auto;width:100%;'></div>
                            </div>
                            <br/>
                            <div id="availableImportedOptions" runat="server" class="tableHeader" style="width:10%;display: table-cell;" onclick="javascript:ToggleOptionsPane('tblImportedOptions')"></div>
                            <div id ="tblImportedOptions" class="optionOptions closeOptions">
                                <div id="ImportedOptions" runat="server" style='display:table;margin: 0 auto;width:100%;'></div>
                            </div>
                            <br/>
                            <div id="availableVinOptions" runat="server" class="tableHeader" style="width:10%;display: table-cell;" onclick="javascript:ToggleOptionsPane('tblVinExplosionOptions')"></div>
                            <div id ="tblVinExplosionOptions" class="optionOptions closeOptions">
                                <div id="VinExplosionoptions" runat="server" style='display:table;margin: 0 auto;width:100%;'></div>
                            </div>
                            <br/>
                            <div style='display:table;margin: 0 auto;width:100%;'>
                                <div class="singleRow">
                                    <div class="centerCell" style="width:150px;"><b>Mechanical Notes:&nbsp;</b></div>
                                    <div class="centerContent"><asp:Label ID="MechanicalNotes" runat="server"></asp:Label></div>
                                </div>
                                <div class="singleRow">
                                    <div class="centerCell" style="width:150px;"><b>Safety Notes:&nbsp;</b></div>
                                    <div class="centerContent"><asp:Label ID="SafetyNotes" runat="server"></asp:Label></div>
                                </div>
                                <div class="singleRow">
                                    <div class="centerCell" style="width:150px;"><b>Exterior Notes:&nbsp;</b></div>
                                    <div class="centerContent"><asp:Label ID="ExtNotes" runat="server"></asp:Label></div>
                                </div>
                                <div class="singleRow">
                                    <div class="centerCell" style="width:150px;"><b>Interior Notes:&nbsp;</b></div>
                                    <div class="centerContent"><asp:Label ID="IntNotes" runat="server"></asp:Label></div>
                                </div>
                                <div class="singleRow">
                                    <div class="centerCell" style="width:150px;"><b>Option Notes:&nbsp;</b></div>
                                    <div class="centerContent"><asp:Textbox ID="OptionNotes" runat="server" style="width:100%;max-width:100%;overflow:scroll" TextMode="MultiLine" CssClass="inputStyle"></asp:Textbox></div>
                                </div>
                            </div>
                        </fieldset>
                    </div>
                </div>
            </div>
            <div id="divSaveButton">
                <div style="text-align:center"><asp:Button ID='submitButton' OnClientClick="buildOptionList();" OnClick="btnSubmitButton" runat='server' Text='Save Details' CssClass='actionBackground' Style='width:125px' UseSubmitBehavior="false"/></div>
            </div>
        </fieldset>
        <div id="KBBAndBB" style="display:none;">
            <div class="row">
                <div class="col">
                    <fieldset id="bbInfo" class="sectionFieldset" style="position:relative;">
                        <legend>Blackbook Data</legend>
                        <div style="display:table;margin: 0 10px 0 0;width:90%;">
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Condition:</div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWholeXClean" runat="server">Extra Clean</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWholeClean" runat="server">Clean</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWholeAverage" runat="server">Average</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWholeRough" runat="server">Rough</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Wholesale:</div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWXCleanValue" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWCleanValue" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWAverageValue" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWRoughValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Retail:</div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBRXClean" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBRClean" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBRAverage" runat="server">N/A</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBRRough" runat="server">N/A</asp:Label></div>
                            </div>
                        </div>
                        <div style="display:table;margin: 0 10px 0 0;width:90%;">
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Blackbook Wholesale Value:</div>
                                <div class="centerContent" style="width:100px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBWholesaleValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Mileage Adjustment:</div>
                                <div class="centerContent" style="width:100px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBMileageValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Options Adjustment:</div>
                                <div class="centerContent" style="width:100px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBOptionsValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Region Adjustment:</div>
                                <div class="centerContent" style="width:100px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBRegionValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Adjusted Wholesale Value:</div>
                                <div class="centerContent" style="width:100px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="BBAdjustedValue" runat="server">N/A</asp:Label></div>
                            </div>
                        </div>
                    </fieldset>
                </div>
                <div class="col">
                    <fieldset id="kbbInfo" class="sectionFieldset" style="position:relative;">
                        <legend>Kelly Blue Book Data</legend>
                        <div style="display:table;margin: 0 10px 0 0;width:90%;">
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Condition:</div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBExcellent" runat="server">Excellent</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBGood" runat="server">Good</asp:Label></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBFair" runat="server">Fair</asp:Label></div>
                            </div>
                        </div>
                        <div style="display:table;margin: 0 10px 0 0;width:90%;">
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Kelly Blue Book Wholesale Value:</div>
                                <div class="centerContent" style="width:50px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBWholesaleValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Mileage Adjustment:</div>
                                <div class="centerContent" style="width:50px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBMileageValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Options Adjustment:</div>
                                <div class="centerContent" style="width:50px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBOptionsValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Region Adjustment:</div>
                                <div class="centerContent" style="width:50px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBRegionValue" runat="server">N/A</asp:Label></div>
                            </div>
                            <div class="singleRow">
                                <div class="centerCell" style="padding:2px;font-weight:bold;text-decoration:underline;">Adjusted Wholesale Value:</div>
                                <div class="centerContent" style="width:50px;"></div>
                                <div class="centerContent" style="padding:2px;font-weight:bold;text-align:right;">&nbsp;&nbsp;<asp:Label ID="KBBAdjustedValue" runat="server">N/A</asp:Label></div>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let lotLocation = document.getElementById('MainContent_lstLotLocation');
        if (lotLocation != null) { lotLocation.addEventListener('change', LotLocationChange, false); }

        $(window).load(function () {
            ShowAll();
        });

        CertCheck(document.getElementById("MainContent_VehicleListStatusLst"));

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': document.getElementById("gtagkPerson").value, 'kDealer': document.getElementById("gtagkDealer").value });
    </script>

</asp:Content>
