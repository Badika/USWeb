Navigation = {
    init: function () {
        this.element = document.querySelector('.navigation');

        if (this.element) {
            this.search = this.element.querySelector('.menu-search > input');

            this.search.addEventListener('input', function () {
                Navigation.filter(this.value);
            });

            [].forEach.call(this.element.querySelectorAll('.navigation > ul'), function (menu) {
                menu.addEventListener('mouseleave', function () {
                    if (Navigation.element.clientWidth < 100) {
                        var submenu = $('.menu li.open');
                        submenu.children('ul').fadeOut();
                        submenu.toggleClass('open');
                    }
                });
            });

            [].forEach.call(this.element.querySelectorAll('.submenu > a'), function (action) {
                action.addEventListener('click', function (e) {
                    e.preventDefault();

                    var action = $(this);
                    var submenu = action.parent();
                    var openSiblings = submenu.siblings('.open');

                    if (Navigation.element.clientWidth >= 100) {
                        openSiblings.toggleClass('changing');
                        openSiblings.children('ul').slideUp(function () {
                            openSiblings.removeClass('changing open');
                        });

                        submenu.toggleClass('changing');
                        action.next('ul').slideToggle(function () {
                            submenu.toggleClass('changing open');
                        });
                    } else {
                        openSiblings.children('ul').fadeOut(function () {
                            openSiblings.removeClass('open');
                        });

                        action.next('ul').fadeToggle(function () {
                            submenu.toggleClass('open');
                        });
                    }
                });
            });

            window.addEventListener('resize', function () {
                if (Navigation.element.clientWidth < 100) {
                    $('.navigation .open').removeClass('open').children('ul').hide();

                    Navigation.filter('');
                }
            });

            if (this.element.clientWidth < 100) {
                [].forEach.call(this.element.querySelectorAll('li.open'), function (submenu) {
                    submenu.classList.remove('open');
                });
            }
        }
    },

    filter: function (term) {
        this.search.value = term;
        term = term.toLowerCase();

        [].forEach.call(this.element.querySelectorAll('li'), function (node) {
            if (node.textContent.toLowerCase().indexOf(term) >= 0) {
                if (node.classList.contains('submenu')) {
                    if ($(node).find('li:not(.submenu)').text().toLowerCase().indexOf(term) >= 0) {
                        $(node).show(500);
                    } else {
                        $(node).hide(500);
                    }
                } else {
                    $(node).show(500);
                }
            } else {
                $(node).hide(500);
            }
        });
    }
};
