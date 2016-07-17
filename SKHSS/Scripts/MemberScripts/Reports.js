$(document).ready(function () {
    if (location.hash.length) {
        $("#type").val(location.hash.substr(1, 1));
        if (location.hash.length > 2) {
            $("#team").val(location.hash.substr(2, 1));
            loadTeam();
        }
        loadreport();
    }
    $("#viewreport").css("display", "none");
    function loadreport() {
        var id = $("#type").val();
        if(id==null){
            id= "1";
        }
        $("#team").css("display", "none");
        $("#report").html('<img style="width:20px;" src="/Content/loading.gif" alt="loading" />');
        location.hash = id;
        switch ($("#type").val()) {
            case "0":
                $("#report").attr("class", "reg");
                $(".printpage").attr("href", "/Members/RegReport/");
                $.post("RegReport", function (data) {
                    $("#report").html(data);
                });
                break;
            case "1":
                $("#team").removeAttr("style");
                $("#report").attr("class", "team");
                loadTeam($("#team").val());
                break;
            case "2":
                $(".printpage").attr("href", "/Members/ShirtReport/");
                $.post("ShirtReport", function (data) {
                    $("#report").html(data);
                });
                break;
        }
    }
    $("#type").on("change", function () {
        loadreport()
    });
    function loadTeam() {
        var id = $("#team").val();
        $(".printpage").attr("href", "/Members/TeamReport/" + id);
        window.location.hash = $("#type").val() + id;
        if ($("#type").val() == undefined) {
            window.location.hash = "1" + id;
        }
        $.post("TeamReport", { id: id }, function (data) {
            $("#report").html(data);
        });
    }
    $("#team").on("change", function () {
        loadTeam($("#team").val());
    });
});