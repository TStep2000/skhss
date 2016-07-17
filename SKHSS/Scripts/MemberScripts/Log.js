$(document).ready(function () {
    $(".delete").on("click", function () {
        $.post("/AJAX/DeleteLog", function (data) {
            location.reload();
        });
    });
});