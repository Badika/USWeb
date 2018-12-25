Lookup = {
    init: function () {
        if (typeof MvcLookup == "function") {
            var lang = document.documentElement.lang;
            MvcLookup.prototype.lang = window.cultures.lookup[lang];

            [].forEach.call(document.querySelectorAll('.mvc-lookup'), function (element) {
                new MvcLookup(element);
            });
        }
    }
};
