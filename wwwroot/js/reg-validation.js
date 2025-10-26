$(function(){
    $("#password").keydown(function()
    {
        let password = $(this).val();
        let regCount = $("#reg-count");
        let passwordCounter = 0;
        
        if(password.length < 8)
        {
            regCount.attr("class", "invalid-feedback");
            regCount.text("Please enter " + (8 - password.length) + " more characters");
        } else
        {
            regCount.hide();
            passwordCounter += 1;
        }
        
        if (hasLowerCase(password)) $("reg-low").hide();
        else passwordCounter += 1;
        
        if (hasUpperCase(password)) $("reg-up").hide();
        else passwordCounter += 1;
        
        if (hasSpecialCharacter(password)) $("reg-down").hide();
        else passwordCounter += 1;
        
        if (hasNumber(password)) $("reg-num").hide();
        else passwordCounter += 1;
        
        if (passwordCounter === 5)
        {
            $("password").attr("class", "form-control mb-3 is-valid");
        }
        else
        {
            $("password").attr("class", "form-control mb-3 is-invalid");
        }
    });
    
});

function hasLowerCase(str) {
    return str.toUpperCase() !== str;
}

function hasUpperCase(str) {
    return str.toLowerCase() !== str;
}

function hasSpecialCharacter(str) {
    const specialChars =
        /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    return specialChars.test(str);
}

function hasNumber(myString) {
    return /\d/.test(myString);
}