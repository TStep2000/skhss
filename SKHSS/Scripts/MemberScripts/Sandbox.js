$(document).ready(function () {
    $(".test").on("undo", function () {
        $(".ts").html("undoing... <img width='18' src='/Content/loading.gif'/>");
        setTimeout(function () {
            $(".ts").html("");
        },500);
    });
    $(".test").on("save", function () {
        var ref = $(this);
        setLoading(ref);
        $(".ts").html("saving...");
        setTimeout(function () {
            setDone(ref)
            $(".ts").html("done!");
            setTimeout(function () {
                $(".ts").html("");
            }, 500);
        }, 500);
    });
    $(".test").on("open", function () {
        $(".ts2").toggleClass("hide");
    });
    $(".test").on("delete", function () {
        if (confirm("REAAALLY!!??!?!")) {
            $(this).remove();
        }
    });
});