$(document).ready(function () {

    //var headerOffset = $("header").offset().top;  // reference from http://jsfiddle.net/gxRC9/502/ AND http://stackoverflow.com/questions/19158559/how-to-fix-a-header-on-scroll
    var headerOffset = $("#topLevel").offset().top + $("#topLevel").height();
    var scrollOk = true;

    // TODO:  Look at http://stackoverflow.com/questions/28676253/changing-div-height-on-scroll

    $(window).scroll(function (e) {
        if (!scrollOk) {
            scrollOk = true;
            return;
        }

        var scroll = $(window).scrollTop();

        if (scroll >= headerOffset) {
            $("header").addClass("fixed");
            scrollOk = false;
        } else {
            $("header").removeClass("fixed");
        }
    });

});