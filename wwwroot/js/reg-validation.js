$(function(){
    $("#password").keydown(function()
    {
        let password = $(this).val();
        let regCount = $("#reg-count");
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