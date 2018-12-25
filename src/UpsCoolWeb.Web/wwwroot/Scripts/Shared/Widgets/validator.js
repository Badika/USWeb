Validator = {
    init: function () {
        var lang = document.documentElement.lang;

        $.validator.methods.date = function (value, element) {
            return this.optional(element) || Globalize.parseDate(value);
        };

        $.validator.methods.number = function (value, element) {
            return this.optional(element) || !isNaN(Globalize.parseFloat(value));
        };

        $.validator.methods.min = function (value, element, param) {
            return this.optional(element) || Globalize.parseFloat(value) >= parseFloat(param);
        };

        $.validator.methods.max = function (value, element, param) {
            return this.optional(element) || Globalize.parseFloat(value) <= parseFloat(param);
        };

        $.validator.methods.range = function (value, element, param) {
            return this.optional(element) || (Globalize.parseFloat(value) >= parseFloat(param[0]) && Globalize.parseFloat(value) <= parseFloat(param[1]));
        };

        $.validator.addMethod('greater', function (value, element, param) {
            return this.optional(element) || Globalize.parseFloat(value) > parseFloat(param);
        });
        $.validator.unobtrusive.adapters.add('greater', ['min'], function (options) {
            options.rules['greater'] = options.params.min;
            options.messages['greater'] = options.message;
        });

        $.validator.addMethod('integer', function (value, element) {
            return this.optional(element) || /^[+-]?\d+$/.test(value);
        });
        $.validator.unobtrusive.adapters.addBool('integer');

        $.validator.addMethod('filesize', function (value, element, param) {
            if (this.optional(element) || !element.files)
                return true;

            var bytes = 0;
            for (var i = 0; i < element.files.length; i++) {
                bytes += element.files[i].size;
            }

            return bytes <= parseFloat(param);
        });
        $.validator.unobtrusive.adapters.add('filesize', ['max'], function (options) {
            options.rules['filesize'] = options.params.max;
            options.messages['filesize'] = options.message;
        });
        $(document).on('change', '[type="file"]', function () {
            this.blur();
        });

        $.validator.addMethod('acceptfiles', function (value, element, param) {
            if (this.optional(element))
                return true;

            var params = param.split(',');
            var extensions = [].map.call(element.files, function (file) {
                var extension = file.name.split('.').pop();

                return extension == file.name ? null : '.' + extension;
            });

            for (var i = 0; i < extensions.length; i++) {
                if (params.indexOf(extensions[i]) < 0) {
                    return false;
                }
            }

            return true;
        });
        $.validator.unobtrusive.adapters.add('acceptfiles', ['extensions'], function (options) {
            options.rules['acceptfiles'] = options.params.extensions;
            options.messages['acceptfiles'] = options.message;
        });

        $('.mvc-lookup-value').on('change', function () {
            var validator = $(this).parents('form').validate();
            var control = new MvcLookup(this).control;

            if (validator.element(this)) {
                control.classList.remove('input-validation-error');
            } else {
                control.classList.add('input-validation-error');
            }
        });

        $('form').on('invalid-form', function (e, validator) {
            $(this).find('.mvc-lookup-values').each(function (i, values) {
                [].forEach.call(values.children, function (value) {
                    var control = new MvcLookup(values).control;
                    if (validator.invalid[value.name]) {
                        control.classList.add('input-validation-error');

                        return;
                    } else {
                        control.classList.remove('input-validation-error');
                    }
                });
            });
        });

        $('.mvc-lookup-value.input-validation-error').each(function (i, element) {
            new MvcLookup(element).control.classList.add('input-validation-error');
        });

        var currentIgnore = $.validator.defaults.ignore;
        $.validator.setDefaults({
            ignore: function () {
                return $(this).is(currentIgnore) && !this.classList.contains('mvc-lookup-value');
            }
        });

        Globalize.cultures.en = null;
        Globalize.addCultureInfo(lang, window.cultures.globalize[lang]);
        Globalize.culture(lang);
    }
};
