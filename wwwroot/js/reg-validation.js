$(function(){
    let passwordSelector = $("#password");
    
    passwordSelector.keydown(function()
    {
        let password = passwordSelector.text();
        let regCount = $("#regCount");
        if(password.length < 8)
        {
            regCount.attr("class", "invalid-feedback");
            regCount.text("Please enter " + (8 - password.length) + " more characters");
        } else
        {
            regCount.attr("class", "valid-feedback");
            regCount.text("Password length looks great!");
        }
    });
    
});