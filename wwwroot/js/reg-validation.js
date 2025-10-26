$(function(){
    let passwordSelector = $("#password");
    
    let password = passwordSelector.text();
    
    passwordSelector.keypress(function()
    {
        if(password.length < 8)
        {
            $("#reg-count")
                .attr("class", "invalid-feedback")
                .text("Please enter " + (8 - password.length) + " more characters");
        } else
        {
            $("#reg-count")
                .attr("class", "valid-feedback")
                .text("Password length looks great!");
        }
    });
    
});