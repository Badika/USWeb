Header = {
    init: function () {
        Header.languages = document.querySelector('.header .dropdown');

        if (Header.languages) {
            Header.languages.addEventListener('mouseleave', function () {
                if (this.classList.contains('show')) {
                    this.children[0].Dropdown.toggle();
                }
            });
        }
    }
};
