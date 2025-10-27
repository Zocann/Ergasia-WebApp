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
        
        const hasUppercase = password.some(char => /[A-Z]/.test(char));
        const hasLowercase = password.some(char => /[a-z]/.test(char));
        const hasNumeric = password.some(char => !isNaN(char) && char !== ' ');
        const hasSpecial = password.some(char => /[!@#$%^&*(),.?":{}|<>]/.test(char));


        if (hasLowercase) {
            $("#reg-low").hide();
            passwordCounter += 1;
        } else $("#reg-low").show();

        if (hasUppercase) {
            $("#reg-up").hide();
            passwordCounter += 1;
        } else $("#reg-up").show();

        if (hasSpecial) {
            $("#reg-spec").hide();
            passwordCounter += 1;
        } else $("#reg-spec").show();

        if (hasNumeric) {
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