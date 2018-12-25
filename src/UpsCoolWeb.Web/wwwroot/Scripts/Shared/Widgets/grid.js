Grid = {
    init: function () {
        if (typeof MvcGrid == "function") {
            var lang = document.documentElement.lang;
            MvcGrid.prototype.lang = window.cultures.grid[lang];

            MvcGridNumberFilter.prototype.isValid = function (value) {
                return value == '' || !isNaN(Globalize.parseFloat(value));
            };

            [].forEach.call(document.querySelectorAll('.mvc-grid'), function (element) {
                new MvcGrid(element);
            });
        }
    }
};
