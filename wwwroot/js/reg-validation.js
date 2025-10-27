$(function () {
    $("#password").on("input", function () {
        let password = $(this).val();
        let regCount = $("#reg-count");
        let passwordCounter = 0;

        if (password.length < 8) {
            regCount.show();
            regCount.attr("class", "invalid-feedback");
            regCount.text("Please enter " + (8 - password.length) + " more characters");
        } else {
            regCount.hide();
            passwordCounter += 1;
        }
        

        if (hasLowerCase(password)) {
            $("#reg-low").hide();
            passwordCounter += 1;
        } else $("#reg-low").show();

        if (hasUpperCase(password)) {
            $("#reg-up").hide();
            passwordCounter += 1;
        } else $("#reg-up").show();

        if (hasSpecial(password)) {
            $("#reg-spec").hide();
            passwordCounter += 1;
        } else $("#reg-spec").show();

        if (hasNumber(password)) {
            $("#reg-num").hide();
            passwordCounter += 1;
        } else $("#reg-num").show();

        if (passwordCounter === 5) {
            $("password").attr("class", "form-control mb-3 is-valid");
        } else {
            $("password").attr("class", "form-control mb-3 is-invalid");
        }
    });
});

function hasLowerCase(str) {
    return (/[a-z]/.test(str));
}

function hasUpperCase(str) {
    return (/[A-Z]/.test(str));
}

function hasSpecial(str) {
    let format = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    return (format.test(str));
}
function hasNumber(str) {
    return (/[0-9]/.test(str));
}