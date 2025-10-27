$(function () {
    let validPassword = false;
    
    let passwordSelector = $("#password");
    let passwordConfSelector = $("#confirm-password");

    //Show validations for password
    passwordSelector.focus(function () {
        $("#password-validation").show();
        
    });
    passwordSelector.focusout(function () {
        $("#password-validation").hide();
    });

    //Show validations for confirmation password
    passwordConfSelector.focus(function () {
        $("#conf-password-validation").show();
    });

    passwordConfSelector.focusout(function () {
        $("#conf-password-validation").hide();
    });

    //Validate password
    passwordSelector.on("input", function () {
        let password = $(this).val();
        
        if (validatePassword(password) === 5) {
            $("#password").attr("class", "form-control mb-3 is-valid");
            
            let passwordMatch = checkPasswordMatch(passwordSelector.val(), passwordConfSelector.val());
            toggleRegisterButton(passwordMatch);
            validPassword = true;
            
        } else {
            $("#password").attr("class", "form-control mb-3 is-invalid");
            toggleRegisterButton(false);
        }
    });

    //Validate confirmation password
    passwordConfSelector.on("input", function () {
        if (validPassword)
        {
            let passwordMatch = checkPasswordMatch(passwordSelector.val(), passwordConfSelector.val());
            toggleRegisterButton(passwordMatch);
        } else
        {
            toggleRegisterButton(false);
        }
    });
});


function validatePassword(password) {
    let passCount = 0;
    
    if (validateLower(password)) passCount++;
    if (validateUpper(password)) passCount++;
    if (validateSpecial(password)) passCount++;
    if (validateNumber(password)) passCount++;
    if (validateLength(password)) passCount++;
    
    return passCount;
}
function validateLength(password) {
    let passCount = $("#pass-count");
    if (password.length < 8) {
        passCount.show();
        passCount.text("Please enter " + (8 - password.length) + " more characters");
        return false;
    } else {
        passCount.hide();
        return true;
    }
}
function validateLower(str) {
    if (/[a-z]/.test(str)) {
        $("#pass-low").hide();
        return true;
    } 
    else {
        $("#pass-low").show();
        return false;
    }
}
function validateUpper(str) {
    if (/[A-Z]/.test(str)) {
        $("#pass-up").hide();
        return true;
    }
    else {
        $("#pass-up").show();
        return false;
    }
}
function validateSpecial(str) {
    let format = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    
    if (format.test(str)) {
        $("#pass-spec").hide();
        return true;
    }
    else {
        $("#pass-spec").show();
        return false;
    }
}
function validateNumber(str) {
    if (/[0-9]/.test(str)) {
        $("#pass-num").hide();
        return true;
    }
    else {
        $("#pass-num").show();
        return false;
    }
}




function checkPasswordMatch(password, confPassword) {
    if (password === confPassword) {
        $("#confirm-password").attr("class", "form-control mb-3 is-valid");
        return true;
    }
    else
    {
        $("#confirm-password").attr("class", "form-control mb-3 is-invalid");
        return false;
    }
}
function toggleRegisterButton(enable)
{
    if (enable) {
        $("#register-button").removeAttr("disabled");
    }
    else {
        $("#register-button").attr("disabled");
    }
}