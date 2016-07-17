/*function openlightbox(data,wid,height,keep) {
    if(wid==null){
        wid = "auto";
    }
    if (height == null) {
        height = "auto";
    }
    if (keep == null) {
        keep = false;
    }
    $("html").css("overflow", "hidden");
    $("body").append("<div id='black_overlay'></div><div id='lightbox'>" + data + "</div>");
    var l = $("#lightbox");
    var w = $(window);
    $("#black_overlay").css("opacity", .8);
    l.css("width", wid).css("top", 0).css("left", "calc(50% - " + (wid / 2) + "px)");//(w.width() - l.width()) / 2);
    l.css("height", height);
    if (l.height() >= w.height()) {
        l.css("top", 0);
    } else {
        l.css("top", "calc(50% - " + (height / 2) + "px)");
    }
    w.on("closelightbox", function (e) {
        //closelightbox();
        w.off("closelightbox");
    });
    $("#closelightbox").on("click", closelightbox);
    $("#closelightbox").css("transition", "0").css("position", "absolute").css("padding", "1px 3px").css("left", 6).css("top", 6);
}*/
function openlightbox(data) {
    $("html").css("overflow", "hidden");
    $("body").append("<div id='black_overlay'></div><div id='lightbox' class='hcenter'>" + data + "</div>");
    var l = $("#lightbox");
    var w = $(window);
    $("#black_overlay").css("opacity", .8);
    l.css("width","100%");//.css("left", "calc(50% - " + (l.outerWidth() / 2) + "px)");//(w.width() - l.width()) / 2);
    w.on("closelightbox", function (e) {
        w.off("closelightbox");
    });
    $("#closelightbox").on("click", closelightbox);
    $("#closelightbox").css("transition", "0").css("position", "absolute").css("padding", "1px 3px").css("left", 6).css("top", 6);
}
function closelightbox() {
    $("#black_overlay").css("opacity", 0);
    $("#lightbox").css("opacity", 0);
    setTimeout(function () {
        $("#black_overlay").remove();
        $("#lightbox").remove();
    }, 300)
    $("html").css("overflow-y", "scroll");
}
function finalmessage(data) {
    if ($("#finalmessage").length > 0) {
        $("#finalmessage").remove();
    }
    $("body").append("<div id='finalmessage'>" + data + "</div>");
    $("#finalmessage").css("top", "calc(50% - " + ($("#finalmessage").height() / 2) + "px)");
    $("#nope").on("click", closefinalmessage);
}
function closefinalmessage() {
    $("#finalmessage").css("opacity", 0);
    setTimeout(function () {
        $("#finalmessage").remove();
    }, 300);
}