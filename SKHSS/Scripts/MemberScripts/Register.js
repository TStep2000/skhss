$(document).ready(function (e) {

    /* //Price Calculations
    var max = 160;
    var ppc = 40;
    var ppu = 25;
    var isMax = false;
    var numreg = $(".reg-chk:checked:not(:disabled)").length;
    var numuni = $(".reg-uni:checked:not(:disabled)").length;
    function price() {
        var price = numreg * ppc;
        isMax = (price > max);
        price = price + numuni * ppu;

        var total = (isMax ? max + numuni * ppu : price);
        if (latefee == "True" && numreg != 0) {
            total += 10;
        }
        return total;
    }
    function updatePrice() {
        numreg = $(".reg-chk:checked:not(:disabled)").length;
        numuni = $(".reg-uni:checked:not(:disabled)").length;
        var tempPrice = price();
        $("#totalcost2").text(tempPrice + ".00");
        $("#totalcost").text(tempPrice + ".00" + ((isMax == true) ? " /Family maximum" : ""));
        $("#paypalprice").val(tempPrice);
        $("#numreg").html(numreg);
        $("#numuni").html(numuni);
        $("#regcost").html(numreg * ppc + ".00");
        $("#unicost").html(numuni * ppu + ".00");
        if (isMax) {
            $("#disc,#disct").css("display", "inline");
            $("#disco").html(numreg * ppc - max + ".00");
        }
        else {
            $("#disc,#disct").css("display", "none");
        }
        //$("#note").css("display", "block");
        if ($("#registerbtn").val() == "Update") {
            $("#registerbtn").val("Update*");
            $("#payblock").removeClass("hide");
            $("#rplc").removeClass("hide");
            $("#payopt form").remove();
        }
    }
    $(".TeamSelect, .UniSize").on("change", function () {
        if ($("#registerbtn").val() == "Update") {
            $("#registerbtn").val("Update*");
            $("#payblock").removeClass("hide");
            $("#rplc").removeClass("hide");
            $("#payopt form").remove();
        }
    });
    $("#numreg").html(numreg);
    $("#numuni").html(numuni);
    $("#regcost").html(numreg * ppc + ".00");
    $("#unicost").html(numuni * ppu + ".00");
    $("#paynow").on("submit", function (e) {
        if (istest == "True") {
            $("#paynowprice").val(0.01);
        }
        else {
            $("#paynowprice").val(price());
        }
    });*/
    $("#paynow").on("submit", function (e) {
        if (ist == "True") {
            $("#paynowprice").val(0.01);
        }
        else {
            $("#paynowprice").val(price());
        }
    });
    function updatePrice() {
        numreg = $(".reg-chk:checked:not(:disabled)").length;
        numuni = $(".reg-uni:checked:not(:disabled)").length;
        $("#totalcost").html(numreg * regp + numuni * unip);
        if ($("#registerbtn").html() == "Update") {
            $("#registerbtn").html("Update*");
        }
    }
    //Checkbox mngment
    $(".ddshirt select").change(function () {
        var ref = $(this);
        var par = ref.closest(".RegChild");
        par.find(".ddshirt input").attr("value", par.find(".ddshirt select").val());
    });
    $(".reg-uni").each(function () {
        var ref = $(this);
        var par = ref.closest(".RegChild");
        if (!ref.is(":checked")) {
            par.find(".ddshirt select").attr("disabled", "disabled");
        }
    });
    $(".reg-uni").click(function () {
        var ref = $(this);
        var par = ref.closest(".RegChild");
        if (ref.is(":checked")) {
            par.find(".ddshirt select").removeAttr("disabled");
            par.find(".ddshirt input").attr("value", par.find(".ddshirt select").val());
        } else {
            par.find(".ddshirt input").attr("value", "-1");
            par.find(".ddshirt select").attr("disabled", "disabled");
        }
        updatePrice();
    });
    $(".reg-chk").on("click", function () {
        var ref = $(this);
        var par = ref.closest(".RegChild");
        if (ref.is(":checked")) {
            par.find(".reg-uni").removeAttr("disabled");
            par.find(".TeamSelect").removeAttr("disabled");
        } else {
            par.find(".reg-uni").removeAttr("checked").attr("disabled", "disabled");
            par.find(".TeamSelect").attr("disabled", "disabled");
            par.find(".ddshirt input").attr("value", "-1");
            par.find(".ddshirt select").attr("disabled", "disabled");
        }
        updatePrice();
    });

    /* Add/Remove Child */
    var ref = $(".addchild");
     ref.find(".submit").on("click", function () {
        var model = getModel($("#new"), "Child");
        saveDataObject(model, "Child", "new", "new", prid);
        $(document).on("saved", function (e, id) {
            if (id == "new") {
                location.reload();
            }
        });
    });
     ref.find(".unhide").click(function () {
        if (ref.find(".inner").css("display") == "none") {
            ref.find(".unhide").html("Hide").css("margin-bottom", "0");
            ref.find(".inner").removeClass("hide");
        }
        else {
            ref.find(".unhide").html("Add Child").css("margin-bottom", "2px");
            ref.find(".inner").addClass("hide");
        }
    });
    
    //$(".RegChild .remove").css("display", "inline-block");
    /*$(".RegChild .remove").click(function () {
        var row = $(this).closest(".RegChild");
        row.addClass("strike");
        if (confirm("Warning, this will delete any previous records for this child\nContinue?")) {
            var childid = row.find(".crid").val();
            var childobj = new Object();
            childobj.ChildRecordID = childid;
            childobj.Mode = "Delete";
            $.post("/AJAX/RDeleteChild", { data: JSON.stringify(childobj), id: prid }, function () {
                row.remove();
                location.reload();
            });
        }
        else {
            row.removeClass("strike");
        }
    });*/
    $(".RegChild .remove").on("rdelete", function () {
        $.post("/AJAX/RDeleteChildObject", { id: $(this).data("group"), prid: prid }, function () {
            $("." + $(this).data("group")).remove();
            location.reload();
        });
    });
    $("#registerbtn").on("click", function (e) {
        $(this).addClass("hide");
        $("#submitting").removeClass("hide");
        var model = getRegModel($(".Reg"));
        saveDataObject(model, "Reg", "step-one");
        $(document).on("saved", function (e, id) {
            if (id == "step-one") {
                location.reload();
            }
        });
        $(document).on("cancelsave", function () {
            $(this).removeClass("hide");
            $("#submitting").addClass("hide");
        });
    });

    /* //Misk
    $("#policylink").on("click", function (e) {
        window.open(doc_root + 'Policy.html', '1366688957503', 'width=500,height=300,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=100,top=100')
    });
    $("#medicallink").on("click", function (e) {
        window.open(doc_root + 'MedicalAgreement.html', '1366688957504', 'width=500,height=300,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=100,top=100')
    });*/
});