$(document).ready(function () {
    var names = { "ul": "UserLogin", "p": "Parent", "o": "Order", "pm": "Payment", "c": "Child", "t": "Teammate", "ch": "Coach" }
    var ids = { "ul": "urid", "p": "prid", "o": "orid", "pm": "pmrid", "c": "crid", "t": "trid", "ch": "chrid" }
    /*$(".imgbtn").on("save", function () {
        alert($(this).attr("class"));
    });*/
    $(".datas").on("change", function () {
        var str = $(this).val();
        $.post("/AJAX/Get" + str + "Data", function (data) {
            $(".data").html("");
            normalize($(".data"), data);
        });
    });
    $.post("/AJAX/GetUserLoginsData", function (data) {
        $(".preloading").addClass("hide");
        normalize($(".data"), data);
    });
    function addToList() {
        var s = names[$(this).data("type")];
        var ref = $(this).closest(".fancylist");
        var id = $(this).data("id");
        var obj = ref.children(".container");
        if (id == "" || obj.closest("#" + id).length == 0) {
            if (obj.html() == "") {
                $.post("/AJAX/Get" + s + "Data", { id: id }, function (data) {
                    if (data.indexOf("none") > -1) {
                        ref.children(".fk").removeClass("highlight");
                        ref.children(".fk").off("click");
                    } else {
                        finishLoad(ref, data);
                    }
                });
            }
            else {
                obj.html("");
            }
        }
        else {
            var nobj = obj.closest("#" + id);
            nobj.addClass("highlight");
            setTimeout(function () {
                nobj.removeClass("highlight");
            }, 1000);
        }
    }
    function loadMultiple() {
        var type = $(this).data("type");
        var ref = $(this).closest(".fancylist");
        $.post("/AJAX/Get" + type + "Data", { id: ref.attr("id"), type: ref.data("type") }, function (data) {
            finishLoad(ref, data);
        });
    }
    function finishLoad(ref, data) {
        var cont = ref.children(".container");
        if (cont.html() == "") {
            normalize(cont, data);
        } else {
            cont.html("");
        }
    }
    function normalize(cont, data) {
        cont.append(data);
        $(".data fieldset input").autosizeInput();
        cont.find(".fk").on("click", addToList);
        cont.find('.rightm .fncybtn2').on("click", loadMultiple);
        cont.find(".fancylist").css("max-height", "1000px").css("padding-top", "31px");
        addImgbtnEvents(cont);
        cont.find(".imgbtn").on("delete", function (a,b,c) {
            var gid = $(this).data("group");
            var group = $("." + gid);
            group.each(function () {
                deleteObject(names[$(this).data("type")], $(this).find("." + ids[$(this).data("type")]).val(), b, c);
                var ref = $(this);
                $(document).on("finishdelete", function () {
                    if (ref.parent().children.length == 1) {
                        ref.parent().html();
                    }
                    else {
                        ref.remove();
                    }
                });
            });
            
        });
        cont.find(".imgbtn").on("show", function () {
            $(this).closest(".fancylist").find(".settings").toggleClass("hide");
        });
        cont.find('fieldset input[type="button"]').on("click", function () {
            var ref = $(this);
            if (ref.val() == "True") { ref.val("False"); }
            else { ref.val("True"); }
        });
        cont.find(".isShirt").on("click", function () {
            var obj = $(this).parent().children(".ShirtID");
            obj.prop("disabled", !obj.prop("disabled"));
        });
    }
    $(".data").on("click",".dot", function () {
        var ref = $(this);
        if (ref.data("error") != null) {
            $.post("/AJAX/FixError", { id: ref.data("id"), error: ref.data("error") }, function (data) {
                if (data == "True") {
                    ref.closest(".data-error").remove();
                } else {
                    postError(null, "", "The error was not fixed");
                }
            });
        }
    });
});