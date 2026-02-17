<%@ Page Title="Vehicle Photo Gallery" Language="C#" AutoEventWireup="true" CodeBehind="PhotoGallery.aspx.cs" Inherits="LMWholesale.WholesaleContent.Vehicle.PhotoGallery" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Google tag (gtag.js) -->
    <script async src="https://www.googletagmanager.com/gtag/js?id=G-3YVJ07S2NS"></script>
    <script>
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag('js', new Date());

        gtag('config', 'G-3YVJ07S2NS', { 'user_id': '<%Response.Write(Session["kPerson"]);%>', 'kDealer': '<%Response.Write(Session["kDealer"]);%>' });
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Import Style Sheets -->
        <link type="text/css" rel="stylesheet" media="all" href="/Common/LightGallery/lightGallery.css?lmV=<%Response.Write(Application["ContentVersion"]);%>"/>
        <link type="text/css" rel="stylesheet" href="/Styles/WholesaleContent/Vehicle/PhotoGallery.css?lmV=<%Response.Write(Application["ContentVersion"]);%>"/>

        <!-- Import Page Scripts -->
        <script type="text/javascript" src="/Common/LightGallery/jquery-1.10.2.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Common/LightGallery/lightgallery.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Common/LightGallery/lg-thumbnail.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Common/LightGallery/lg-fullscreen.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Common/LightGallery/lg-zoom.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Common/LightGallery/lg-video.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>
        <script type="text/javascript" src="/Scripts/Sortable.min.js?lmV=<%Response.Write(Application["ContentVersion"]);%>"></script>

        <script type="text/javascript">
            function ShowAll() {
                $('#carousel').flexslider({
                    animation: "slide",
                    controlNav: false,
                    animationLoop: false,
                    slideshow: false,
                    itemWidth: 150,
                    itemMargin: 10,
                    asNavFor: '#slider'
                });

                $('#slider').flexslider({
                    animation: "slide",
                    controlNav: false,
                    animationLoop: false,
                    slideshow: false,
                    itemWidth: 600,
                    sync: "#carousel",
                    start: function (slider) {
                        $('body').removeClass('loading');
                    }
                });
            }


            $('#lightgallery').lightGallery({
                mode: 'lg-fade',
                cssEasing: 'cubic-bezier(0.25, 0, 0.25, 1)',
                download: false
            });

            function closeWindow() {
                window.close();
            }

            $(document).ready(function () {
                $("#lightgallery").lightGallery();
                $("#lightgallery_normal").lightGallery();
                $("#lightgallery_damage").lightGallery();

                $(".damageSlideShow").on('click', function () {
                    var slideID = $(this).attr('data-slide');
                    $('#' + slideID).trigger('click');
                })

                // Since there should always be a PhotoItem1, we will click it to show a clean PhotoGallery
                document.getElementById("PhotoItem1").click();

                // Find 'X' button and window.close() on 'click' event
                document.getElementsByClassName('lg-close')[0].addEventListener("click", closeWindow, false);

                new Sortable(document.getElementById('carousel'), {
                    animation: 150,
                    onEnd: function () {
                        const orderedIds = Array.from(document.querySelectorAll('.photo-item'))
                            .map(el => el.dataset.id);

                        // Save order to server
                        //fetch('/photos/updateorder', {
                        //    method: 'POST',
                        //    headers: { 'Content-Type': 'application/json' },
                        //    body: JSON.stringify({ orderedIds: orderedIds })
                        //}).then(response => {
                        //    if (response.ok) {
                        //        console.log('Order saved');
                        //    } else {
                        //        alert('Error saving order');
                        //    }
                        //});
                        alert(orderedIds)
                    }
                });
            });
        </script>
        <script src="/Common/LightGallery/jquery.flexslider-min.js"></script>

        <div class="photoGallery">
            <div class="photoslideshow" style="padding:10px;">
                <!--slider-->
                <div id="slides_all" style="display:block;">
                    <div id="lightGallery" class="slider">
                        <div id="slider" class="flexslider">
                            <div id="lightgallerySlider" class="flex-viewport" style="overflow: hidden; position: relative;" runat="server"></div>
                            <ul class="flex-direction-nav">
                                <li><a class="flex-prev flex-disabled" href="#" tabindex="-1">Previous</a></li>
                                <li><a class="flex-next" href="#" tabindex="-1">Next</a></li>
                            </ul>
                        </div>
                        <div id="carousel" class="flexslider hidden-xs">
                            <div id="lightgalleryCarousel" class="flex-viewport" style="overflow: hidden; position: relative;" runat="server"></div>
                            <ul class="flex-direction-nav">
                                <li><a class="flex-prev flex-disabled" href="#" tabindex="-1">Previous</a></li>
                                <li><a class="flex-next" href="#" tabindex="-1">Next</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script type="text/javascript">
            $(window).load(function () {
                ShowAll();
            });
        </script>
    </form>
</body>
</html>
