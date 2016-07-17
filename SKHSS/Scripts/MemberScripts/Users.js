$(document).ready(function () {
    
    $(document).on("saved", function (e, id) {
        $(".savebtn").each(function () {
            if (!$(this).find(".load").hasClass("hide")) {
                setDone($(this));
            }
        });
    });
    $(".openbtn").on("open", function () {
        $(this).closest(".node").children(".cont").children(".node").removeClass("hide");
    });
    $(".openbtn").on("close", function () {
        $(this).closest(".node").children(".cont").children(".node").addClass("hide");
    });
    $(".isShirt").on("click", function () {
        var obj = $(this).parent().children(".ShirtID");
        obj.prop("disabled", !obj.prop("disabled"));
    });
    /*//$(".data fieldset input").autosizeInput();
    $(".saveimg").on("click", function () {
        var group = $(this).attr("data-group");
        var obj = getObjectModel($("#" + group));
        obj.Parent.LastName = $("#" + group + " .LastName input").val();
        saveDataObject(obj, "Object", group);
        $(document).on("saved", function (a, b) {
            if (b == group) {
                //location.reload();
            }
        });
    });
    //$("#membercontent").removeClass("fillmiddle");
    function getRole(n) {
        switch (n) {
            case "0":
                return "Me";
                break;
            case "1":
                return "WebAdmin";
                break;
            case "2":
                return "Admin";
                break;
            case "3":
                return "Sports Dir";
                break;
            case "4":
                return "Registrar";
                break;
            case "5":
                return "Coach";
                break;
            case "6":
                return "Parent";
                break;
            default:
                return "";
                break;
        }
    }
    function getTeam(n) {
        switch (n) {
            case "0":
                return "PeeWees";
                break;
            case "1":
                return "Middlers";
                break;
            case "2":
                return "Intermediates";
                break;
            case "3":
                return "Juniors";
                break;
            case "4":
                return "Seniors";
                break;
            case "5":
                return "Jr/Sr Girls";
                break;
            default:
                return "";
                break;
        }
    }
    function getSeason(n) {
        switch (n) {
            case "1":
                return "Basketball";
                break;
            case "2":
                return "Baseball";
                break;
            case "3":
                return "Soccer";
                break;
            default:
                return "";
                break;
        }
    }
    function getGender(n) {
        switch (n) {
            case "False":
                return "Male";
                break;
            case "True":
                return "Female";
                break;
            default:
                return "";
                break;
        }
    }
    function getShirt(n) {
        switch (n) {
            case "0":
                return "Youth Small"
                break;
            case "1":
                return "Youth Medium"
                break;
            case "2":
                return "Youth Large"
                break;
            case "3":
                return "Adult Small"
                break;
            case "4":
                return "Adult Medium"
                break;
            case "5":
                return "Adult Large"
                break;
            case "6":
                return "Adult XL"
                break;
            case "7":
                return "Adult XXL"
                break;
            default:
                return "";
                break;
        }
    }
    $(".RoleID input,.TeamID input,.SeasonID input, .ShirtID input, .Gender input").on("keyup", onechange);
    function onechange(ref) {
        if (ref.target) {
            ref = $(this).parent();
        }
        if (ref.hasClass("RoleID")) {
            ref.find("span").text(getRole($(this).val()));
        }
        else if (ref.hasClass("TeamID")) {
            ref.find("span").text(getTeam($(this).val()));
        }
        else if (ref.hasClass("Season")) {
            ref.find("span").text(getSeason($(this).val()));
        }
        else if (ref.hasClass("ShirtID")) {
            ref.find("span").text(getShirt($(this).val()));
        }
        else if (ref.hasClass("Gender")) {
            ref.find("span").text(getGender($(this).val()));
        }
    }

    $(".newchild").on("click", newchild);
    $(".newteammate").on("click", newteammate);
    function newchild() {
        var ref = $(this).parent();
        if (ref.find(".openrole img:first-child").hasClass("hide") && ref.find(".openrole img:nth-child(2)").hasClass("hide")) {
            ref.find(".openrole").on("click", open);
            ref.find(".openrole img:first-child").removeClass("hide");
        }
        ref.find(".moredetails").prepend(getChildContent());
        if (ref.find(".openrole").hasClass("disabled")) {
            ref.find(".openrole").on("click", open);
            ref.find(".openrole").removeClass("disabled");
        }
        ref.find(".moredetails .child:first-child .newteammate").on("click", newteammate);
        if (ref.find(".moredetails").hasClass("hide")) {
            ref.find(".openrole").click();
        }
    }
    function newteammate() {
        ref = $(this).parent();
        if (ref.find(".openteammate img:first-child").hasClass("hide") && ref.find(".openteammate img:nth-child(2)").hasClass("hide")) {
            ref.find(".openteammate").on("click", open);
            ref.find(".openteammate img:first-child").removeClass("hide");
        }
        ref.find(".teammatedetails").prepend(getTeammateContent());
        if (ref.find(".openteammate").hasClass("disabled")) {
            ref.find(".openteammate").on("click", open);
            ref.find(".openteammate").removeClass("disabled");
        }
        ref.find(".teammatedetails .teammate:first-child").find(".TeamID input,.SeasonID input,.ShirtID input").on("keyup", onechange);
        if (ref.find(".teammatedetails").hasClass("hide")) {
            ref.find(".openteammate").click();
        }
    }
    $(".newuser").on("click", function () {
        $(this).remove();
        $(".top").after(getUserContent());

        $(".user:nth-child(2) .userid img").on("click", showid);
        $(".user:nth-child(2) .save").on("click", saverow);
        $(".user:nth-child(2) .RoleID input").on("keyup", onechange);
        $(".user:nth-child(2) .newrole").on("click", function () {
            $(".user:nth-child(2) .open").on("click", open);
            $(".user:nth-child(2) .open").find("img:first-child").removeClass("hide");
            switch ($(".user:nth-child(2) .RoleID input").val()) {
                case "0":
                    $(".user:nth-child(2) .roledetails").prepend(getParentContent());
                    $(".user:nth-child(2) .roledetails .newchild").on("click", newchild);
                    if ($(".user:nth-child(2) .roledetails").hasClass("hide")) {
                        $(".user:nth-child(2) .open").click();
                    }
                    break;
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
                    break;
            }
            $(this).remove();
        });
    });
    
    $(".openbtn:not(.disabled)").on("click", open);
    $(".userid img").on("click", showid);
    $(".loginas").on("click", loginas);
    $(".delete").on("click", deleterow);
    function showid() {
        $(this).parent().siblings(".ids").toggleClass("hide");
    }
    function open() {
        $(this).siblings(".details").toggleClass("hide");
        $(this).siblings(".push").toggleClass("hide");
        $(this).find("img:nth-child(1)").toggleClass("hide");
        $(this).find("img:nth-child(2)").toggleClass("hide");
    }
    $(".top .open").on("click", function () {
        if ($(this).find("img:nth-child(2)").hasClass("hide")) {
            $(".roledetails").removeClass("hide");
            $(".user:nth-child(n+2) .open img:nth-child(1)").addClass("hide");
            $(".user:nth-child(n+2) .open img:nth-child(2)").removeClass("hide");
        }
        else {
            $(".roledetails").addClass("hide");
            $(".user:nth-child(n+2) .open img:nth-child(2)").addClass("hide");
            $(".user:nth-child(n+2) .open img:nth-child(1)").removeClass("hide");
        }
    });
    function deleterow() {
        //var rows = $(this).closest(".node").parent().find(".node");
        var strikes = $(this).closest(".node").find(".strike");
        strikes.removeClass("hide");
        //rows.addClass("strike");
        if (confirm("Confirm Deletion?")) {
            var parent = $(this).closest(".node");
            var type = "Object";
            var obj = new Object();
            if (parent.hasClass("Object")) {
                obj.UserRecordID = parent.find(".urid").val();
            } else if (parent.hasClass("Parent")) {
                type = "Parent";
                obj.ParentRecordID = parent.find(".prid").val();
            } else if (parent.hasClass("Child")) {
                type = "Child";
                obj.ChildRecordID = parent.find(".crid").val();
            } else if (parent.hasClass("Teammate")) {
                type = "Teammate";
                obj.TeammateRecordID = parent.find(".trid").val();
            }
            obj.Mode = "Delete";
            saveDataObject(obj, type);
            $(document).on("saved", function () {
                window.location.reload();
            });
        }
        else {
            strikes.addClass("hide");
        }
    }
    */
});