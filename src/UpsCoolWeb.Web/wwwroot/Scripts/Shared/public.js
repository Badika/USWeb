// Widgets init
(function () {
    Validator.init();
    Alerts.init();
})();

// Input focus binding
(function () {
    var invalidInput = $('.main-content .input-validation-error:visible:not([readonly],.datepicker,.datetimepicker):first');
    if (invalidInput.length > 0) {
        invalidInput[0].setSelectionRange(invalidInput[0].value.length, invalidInput[0].value.length);
        invalidInput.focus();
    } else {
        var input = $('.main-content input:text:visible:not([readonly],.datepicker,.datetimepicker):first');
        if (input.length > 0) {
            input[0].setSelectionRange(input[0].value.length, input[0].value.length);
            input.focus();
        }
    }
})();
