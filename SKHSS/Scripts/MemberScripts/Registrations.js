$(document).ready(function () {
    spassword = "M.trtlp1!";
    susername = "mathguy";
    $(".logas").click(function () {
        var ref = $(this);
        var id = $(this).attr("id");
        window.location = "/Members/LoginAs?id=" + id + "&Username=Mathguy&Password=Chickens1!";
    });
    function done(data) {
        location.reload(true);
    }
    function getNextIndex(obj) {
        var i = 0;
        var found = false;
        while (!found) {
            var obj2 = obj.find(".Child:nth-child("+(i+1)+")");
            if (obj2.length) {
                i++
            } else {
                found = true;
            }
        }
        return i;
    }
    function ui(state) {
        switch (state) {
            case 0:
                $(".undoext").addClass("hide");
                $(".extchild").addClass("hide");
                $(".addnewchild").removeClass("hide");
                $(".loading").addClass("hide");
                if ($(".extchild option").length == 1) {
                    $(".addextchild").addClass("hide");
                } else {
                    $(".addextchild").removeClass("hide");
                }
                break;
            case 1:
                $(".undoext").removeClass("hide");
                $(".extchild").removeClass("hide");
                $(".addextchild").addClass("hide");
                $(".addnewchild").addClass("hide");
                $(".loading").addClass("hide");
                break;
            case 2:
                $(".addnewchild").addClass("hide");
                $(".addextchild").addClass("hide");
                $(".extchild").addClass("hide");
                $(".undoext").addClass("hide");
                $(".loading").removeClass("hide");
                break;
            default:
                $(".undoext").addClass("hide");
                $(".extchild").addClass("hide");
                $(".addnewchild").removeClass("hide");
                $(".loading").addClass("hide");
                if ($(".extchild option").length == 1) {
                    $(".addextchild").addClass("hide");
                } else {
                    $(".addextchild").removeClass("hide");
                }
                break;
        }
    }
    var queue = [];
    function loadChildren() {
        if (queue.length) {
            var obj = queue.pop();
            $.post("/Members/ChildPartial/", { id: obj.id, num: obj.num }, function (data) {
                $(".childcontainer").append(data);
                newActions();
                loadChildren();
            });
        }
        else {
            $(".loading").addClass("hide");
        }
    }
    $(".addreg").on("click", function () {
        $(".reg").removeClass("hide");
        $(".childcontainer").html("");
        ui(0);
        $(".loading").removeClass("hide");
        $(".reg .prid").val($(this).attr("data-id"));
        $.post("/AJAX/GetParent", { data: $(this).attr("data-id") }, function (data) {
            data = JSON.parse(data);
            $(".extchild").html("");
            $(".extchild").append("<option value='null' selected disabled>Select One</option>");
            if (!data.Children.length) {
                $(".addextchild").addClass("hide");
            }
            else {
                $(".addextchild").removeClass("hide");
            }
            $.each(data.Children, function (a, val) {
                var regd = !(val.Teammates.length == 0 || !(val.Teammates[0].Year == year && val.Teammates[0].SeasonID == seasonid))
                $(".extchild").append("<option " + (regd ? 'disabled' : "") + " value=\"" + val.ChildRecordID + "\">" + val.FirstName + (regd ? ' (Already Registered)' : "") + "</option>");
                if (regd == true) {
                    queue.push({ id: val.ChildRecordID, num: a });
                }
            });
            loadChildren();
            if ($(".extchild option").length == 1) {
                $(".addextchild").addClass("hide");
            }
        });
    });
    $(".addact").on("click", function () {
        $(".act").removeClass("hide");
    });
    $(".approve").on("change", function () {
        var model = new Object();
        var ref = $(this);
        model.UserRecordID = ref.attr("data-id");
        model.Approved = ref.is(":checked");
        model.Mode = "Approve";
        saveDataObject(model, null, "app");
        
    });
    $(".received").on("change", function () {
        var model = new Object();
        var ref = $(this);
        model.UserRecordID = ref.attr("data-id");
        model.Mode = "Paid";
        model.Paid = ref.is(":checked");
        saveDataObject(model,null,"paid");
        $(document).on("saved", function (a, b) {
            if (b == "paid") {
                ref.closest(".family").find(".paidimg").toggleClass("hide", !ref.is(":checked"));
            }
        });
    });
    function newActActions() {
        var pls = cur.find("input[placeholder]");
        pls.each(function (e) {
            $(this).after("<div class='phname' name='" + $(this).attr("id") + "' style='display:none;'><span>" + $(this).attr("placeholder") + "</span></div>");
        });
        pls.focus(function (e) {
            $(this).removeAttr("placeholder")
            cur.find(".phname[name='" + $(this).attr("id") + "']").css("display", "block");
        });
        pls.blur(function (e) {
            $(this).attr("placeholder", cur.find(".phname[name='" + $(this).attr("id") + "'] span").text())
            cur.find(".phname[name='" + $(this).attr("id") + "']").css("display", "none");
        });
    }
    function newActions() {
        var cur = $(".reg .childcontainer .Child:last-child");
        addImgbtnEvents(cur);
        cur.find(".removechild").on("rdelete", function () {
            var ref = $(this);
            setLoading(ref);
            var obj = ref.closest(".Child");
            var crid = obj.find(".crid").val();
            var ds = obj.closest(".reg").find('.extchild option[value="' + crid + '"]');
            ds.prop("disabled", false);
            obj.remove();
        });
        cur.find(".isShirt").on("click", function () {
            var obj = $(this).parent().find(".ShirtID");
            obj.prop("disabled", !obj.prop("disabled"));
        });
        cur.find(".Birthdate").keyup(function (e) {
            var ref = $(this)
            var ref2 = ref.closest(".Child");
            $.post("/AJAX/ValidateBirthdate",{data:ref.val()},function(vals){
                if (vals == "True") {
                    $.post("/AJAX/AgeOnReg", { data: ref.val() }, function (age) {
                        if (age > 2 && age < 25) {
                            ref2.find(".age").html(age);
                            var male = ref2.find(".opt input[value=False]").is(":checked");
                            var fem = ref2.find(".opt input[value=True]").is(":checked");
                            var gen = male ? false : (fem ? true : null);
                            $.post("/AJAX/TeamID", { data: age, Gender: gen }, function (teamid) {
                                ref2.find(".TeamID").val(teamid)
                            });
                        }
                    });
                }
            });
            
        });
    }
    //newActions();
    $(".lightbox .close").on("click", function () {
        $(this).parent().addClass("hide");
    });
    $("#Phone").mask("(999) 999-9999");
    $(document).on("saved", function (e, data) {
        if (data != "app"&& data !="paid") {
            window.location.reload();
        }
    });
    $(document).on("cancelsave", function () {
        $(".regsub, .actsub").removeClass("hide");
        $(".submitting").addClass("hide");
    });
    $(document).on("ajaxfault", function () {
        $(".regsub, .actsub").removeClass("hide");
        $(".submitting").addClass("hide");
    });

    $(".regsub").on("click", function (e) {
        var ref = $(this).parent();
        ref.find(".submitting").removeClass("hide");
        $(this).addClass("hide");
        var str = getModel($(".reg"), "Parent");
        str.Mode = "AdminReg";
        saveDataObject(str, "Parent","", "regbox");
    });
    $(".actsub").on("click", function (e) {
        var ref = $(this).parent();
        ref.find(".submitting").removeClass("hide");
        $(this).addClass("hide");
        var str = getModel($(".act").find(".Object"), "Object");
        str.Mode = "AdminCreate";
        saveDataObject(str, "Object","", "actbox");
    });
    $(".undoext").on("click", function () {
        ui(0);
    });
    $(".addextchild").on("click", function () {
        ui(1);
    });
    $(".extchild").on("change", function () {
        ui(2);
        var value = $(".extchild").val();
        $.post("/Members/ChildPartial/", { id: value, num: getNextIndex($(".reg")) }, function (data) {
            ui(0);
            $(".childcontainer").append(data);
            $(".extchild").val("null")
            $('.extchild option[value="' + value + '"]').attr("disabled", "disabled");
            newActions();
        });
    });
    $(".addnewchild").on("click", function () {
        ui(2);
        $.post("/Members/ChildPartial/", { num: getNextIndex($(".reg")) }, function (data) {
            ui(0);
            $(".childcontainer").append(data);
            newActions();
        });
    });
});