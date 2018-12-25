Tree = {
    init: function () {
        if (typeof MvcTree == "function") {
            [].forEach.call(document.querySelectorAll('.mvc-tree'), function (element) {
                new MvcTree(element);
            });
        }
    }
};
