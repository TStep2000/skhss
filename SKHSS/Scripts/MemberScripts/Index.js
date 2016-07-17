$(document).ready(function () {
    /*$(".childrow .imgbtn").on("click", openAllergy);*/
    /*$("#tc h2 .imgbtn").on("click", function () {
        if ($(this).attr("data-status") == "close") {
            $(".childrow2 > div > div").removeAttr("style");
        } else {
            $(".childrow2 > div > div").css("width", "calc(100% + 259px)");
        }
    });*/
    //$(".editable-field").on("keydown", inputdown);
    $(".childrow .imgbtn").on("page1", openMedical);
    $(".childrow .imgbtn").on("page0", openMedical);
    function openMedical(e) {
        var ref = $(this).closest(".childrow");
        var alr = ref.next();
        if (ref.css("height") == "65px") {
            ref.css("height", 25);
            alr.css("display", "none");
            ref.find(".down").css("display", "inline");
            ref.find(".up").css("display", "none");
        } else {
            alr.css("display", "table-row");
            ref.css("height", 65);
            ref.find(".up").css("display", "inline");
            ref.find(".down").css("display", "none");
        }
    }
        
    /*$.post("@Url.Action("MyAlbums")", { id: "@User.Identity.Name" }, function (data) {
        var obj = JSON.parse(data);
        var i = 0;
        $.each(obj, function (index, ref) {
            $.post("@Url.Action("RenderAlbum")", { id: ref }, function (data) {
                $("#myalbums").append(data);
            });
            i++;
        });
        $("#myalbums").css("width", i * 210);
        if (i > 3) {
            $(".b").css("height", 185);
            $("#topheight").html(".t{height:calc(100% - 205px);}");
        }
    });*/
});