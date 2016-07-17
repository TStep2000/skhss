function cancelField(id) {
    var ref = $("#ef" + id);
    var edit = ref.find(".edit");
    var disp = ref.find(".display");
    edit.addClass("hide");
    disp.removeClass("hide");

    if (edit.attr("type") == "text") {
        edit.val(edit.attr("value"));
    }
    if (edit.attr("type") == "select") {
        //find a way to reselect correct one...
    }
    edit.keydown();
    return edit;
}
function editField(id) {
    var ref = $("#ef" + id);
    var edit = ref.find(".edit");
    var disp = ref.find(".display");
    edit.removeClass("hide");
    disp.addClass("hide");
    return edit;
}
function saveField(id) {
    var ref = $("#ef" + id);
    var edit = ref.find(".edit");
    var disp = ref.find(".display");
    var title = ref.find(".title").val();
    var type = edit.attr("type");
    edit.addClass("hide");
    disp.removeClass("hide");
    if (type == "text") {
        disp.html(edit.val());
        edit.attr("value", edit.val());
    }
    if (type == "select") {
        disp.html(edit.text());
        if (title == "TeamID") {
            disp.html('<a href="/Members/Team/' + edit.val() + '">' + edit.children(':selected').text() + "</a>");
        }
    }
    if (title == "Birthdate") {
        disp.html("<b>" + calcAge(edit.val()) + "</b>");
    }
    edit.keydown();
    return edit;
}
function cancelFieldGroup(gid) {
    var group = $("." + gid);
    group.each(function (i) {
        cancelField(gid + i);
    });
    hideErrors();
}
function editFieldGroup(gid) {
    var group = $("." + gid);
    group.each(function (i) {
        editField(gid + i);
    });
}
function beginSaveFieldGroup(gid, model) {
    var group = $("." + gid);
    group.each(function (i) {
        var ref = $("#ef" + gid + i);
        ///switch "prefix" and create Teammates object on model, and expand to "Teammates"/"Child" etc
        var editobj = ref.find(".edit")
        var type = ref.find(".title").val();

        if (ref.hasClass("tem")) {
            if (!model.Teammates) {
                model.Teammates = [];
            }
            var tem = new Object();
            tem[type] = editobj.val();
            var temrc = ref.parent().find("#temr" + gid).val();
            tem["TeammateRecordID"] = temrc;
            model.Teammates.push(tem);
        } else {
            model[type] = editobj.val();
        }
    });
    return model;
}
function saveFieldGroup(gid) {
    var group = $("." + gid);
    group.each(function (i) {
        var editobj = saveField(gid + i);
    });
    if ($(".childrow.reg").has(".editable-field .display.hide").length == 0) {
        var ref = $("#edit");
        var edit = ref.find(".edit");
        var cancel = ref.find(".cancel");
        var save = ref.find(".save");
        edit.removeClass("hide");
        cancel.addClass("hide");
        save.addClass("hide");
    }
}

//$("#add-child").on("click", addChild);
/*$("#membercontent").on("click", ".save", saveeditablerow);
$("#membercontent").on("click", ".close", closerow);
$("#membercontent").on("click", ".editable-row:not(.selected):not(.clickbtn)", rowclick);
$("#membercontent").on("click", ".editable-row.clickbtn:not(.selected) .edit", rowclick);
$("#membercontent").on("keydown", ".editable-input", inputdown);*/
$(".btn").on("click", openAllergy);

$("#edit .edit").on("click", editAll);
$("#edit .cancel").on("click", cancelAll);
$("#edit .save").on("click", saveAll);

function editAll() {
    var ref = $(this).parent();
    $(".childrow.reg").each(function (i) {
        editFieldGroup("c" + i);
    });
    var edit = ref.find(".edit");
    var cancel = ref.find(".cancel");
    var save = ref.find(".save");
    edit.addClass("hide");
    cancel.removeClass("hide");
    save.removeClass("hide");
}
function cancelAll() {
    var ref = $(this).parent();
    $(".error").removeClass("error");
    $(".childrow.reg").each(function (i) {
        cancelFieldGroup("c" + i);
    });
    var edit = ref.find(".edit");
    var cancel = ref.find(".cancel");
    var save = ref.find(".save");
    edit.removeClass("hide");
    cancel.addClass("hide");
    save.addClass("hide");
}
function saveAll() {
    var ref = $(this).parent();
    var model = new Object();
    $(".error").removeClass("error");
    $(".childrow.reg").each(function (i) {
        beginSaveFieldGroup("c" + i, model);
        model.ChildRecordID = $("#rc" + i).val();
        saveDataObject(model, "Child", "c" + i);
    });
}
function saveDataObject(data, type, id) {
    $.post("/AJAX/Validate" + type + "Object", { data: JSON.stringify(data), id: id }, function (retdata) {
        var savedata = retdata.substr(retdata.indexOf("||&||") + 5);
        retdata = retdata.substr(0, retdata.indexOf("||&||"));
        var val = false;
        switch (type) {
            case "":
                val = validateObject(retdata);
                break;
            case "Parent":
                val = validateParentObject(retdata);
                break;
            case "Child":
                val = validateChildObject(retdata);
                break;
            case "Teammate":
                val = validateTeammateObject(retdata);
                break;
            default:
                val = validateObject(retdata);
                break;
        }
        if (val) {
            $.post("/AJAX/Save" + type + "Object", { data: savedata }, function () {
                saveFieldGroup(id);
            });
        }
    });
}

function openAllergy(e) {
    var ref = $(this).closest(".childrow");
    var alr = ref.next();
    if (ref.css("height") == "65px") {
        ref.css("height", 25);
        alr.css("display", "none");
        ref.find(".down").css("display", "inline");
        ref.find(".up").css("display", "none");
    } else {
        ref.css("height", 65);
        alr.css("display", "table-row");
        ref.find(".up").css("display", "inline");
        ref.find(".down").css("display", "none");
    }
}