// Widgets init
(function () {
    Datepicker.init();
    Navigation.init();
    Validator.init();
    Alerts.init();
    Header.init();
    Lookup.init();
    Grid.init();
    Tree.init();
})();

// Read only binding
(function () {
    [].forEach.call(document.querySelectorAll('.widget-box.readonly'), function (widget) {
        [].forEach.call(widget.querySelectorAll('.mvc-lookup'), function (element) {
            new MvcLookup(element, { readonly: true });
        });

        [].forEach.call(widget.querySelectorAll('.mvc-tree'), function (element) {
            new MvcTree(element, { readonly: true });
        });

        [].forEach.call(widget.querySelectorAll('textarea'), function (textarea) {
            textarea.readOnly = true;
            textarea.tabIndex = -1;
        });

        [].forEach.call(widget.querySelectorAll('input'), function (input) {
            input.readOnly = true;
            input.tabIndex = -1;
        });
    });

    window.addEventListener('click', function (e) {
        if (e.target && e.target.readOnly) {
            e.preventDefault();
        }
    });
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
