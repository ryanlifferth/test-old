$(document).ready(function () {

    $("#TestGetApi").on("click", function () {
        $.ajax({
            url: "/api/Ryan",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (json) {
                alert("Response from server:  (" + json.StatusCode + ") " + json.StatusMessage);
            }
        });

        return false;
    });

    var testData = {
        Name: "Jackson",
        Salary: 22222
    };
    $("#TestPostApi").on("click", function () {
        $.ajax({
            url: "/api/Ryan/SpecialPost",
            data: JSON.stringify(testData),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (json) {
                alert(json.StatusMessage);
            }
        });

        return false;
    });

    var personRequest = {
        firstName: "Ryan",
        lastName: "Lifferth",
        salary: 33333
    };
    $("#AnotherTestPostApi").on("click", function () {
        $.ajax({
            url: "/api/Ryan/AnotherSpecialPost",
            data: JSON.stringify(personRequest),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            success: function (json) {
                alert(json.StatusMessage);
            }
        });

        return false;
    });

});