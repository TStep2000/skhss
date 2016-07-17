$(document).ready(function () {
    $(".fancylist .fncybtn").on("click", function () {
        var ref = $(this);
        var obj = getObjectModel(ref);
        obj.Mode = "Activate";
        obj.Activated = "true";
        saveDataObject(obj, "Object", "saveitem");
        $(document).on("saved", function (a, b) {
            if (b == "saveitem") {
                location.reload();
            }
        });
    });
});