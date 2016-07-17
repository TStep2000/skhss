$(document).ready(function () {
    $(".selcchbtn").on("click", function () {
        var ref = $(this);
        $(".select").removeClass("hide");
        $(".teamname").text(ref.attr("data-team"));
        $(".teamid").val(ref.attr("data-teamid"));
    });
    $(".selparbtn").on("click", function () {
        var ref = $(this);
        $(".coachform .coachname").text(ref.parent().find(".pname").text());
        $(".coachform .urid").val(ref.attr("data-urid"));
        $(".coachform .prid").val(ref.attr("data-id"));
        $(".coachform .parphone").val(ref.closest(".parent").find(".Phone").val());
        $(".coachform .paremail").val(ref.closest(".parent").find(".Email").val());
        $(".coachform").removeClass("hide");
    });
    $(".confirm").on("click", function () {
        var ref = $(this).closest(".teamselect");
        var model = new Object();
        model.TeamID = ref.find(".teamid").val();
        model.PositionID = ref.find(".positionid").val();
        model.PRTID = ref.find(".prid").val();
        saveDataObject(model, "Coach", "newcoach", "newcoach", ref.find(".urid").val());
        $(document).on("saved", function (a, b) {
            if (b == "newcoach") {
                location.reload();
            }
        });
    });
    $(".delcoach").on("delete", function (a,b,c) {
        $.post("/AJAX/DeleteCoachObject", { id: $(this).data("id"), username: b, password: c }, function () {
            location.reload();
        });
    });
    $(".closesel").on("click", function () {
        $(".select").addClass("hide");
        $(".coachform").addClass("hide");
    });
});