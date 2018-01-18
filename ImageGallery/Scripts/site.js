$(document).ready(function () {
    $("#file").on('change', function () {
        $("#form").submit();
    });
    $("img").unveil(200, function () {
        $(this).on('load', function () {
            this.style.opacity = 1;
        });
    });
});