window.cultures = window.cultures || {};
window.cultures.globalize = window.cultures.globalize || {};
window.cultures.globalize['en'] = {
    name: 'en',
    englishName: 'English (United Kingdom)',
    nativeName: 'English (United Kingdom)',
    numberFormat: {
        currency: {
            pattern: ['-$n', '$n'],
            symbol: '£'
        }
    },
    calendars: {
        standard: {
            firstDay: 1,
            patterns: {
                d: 'dd/MM/yyyy',
                g: 'dd/MM/yyyy HH:mm',
                D: 'dd MMMM yyyy',
                t: 'HH:mm',
                T: 'HH:mm:ss',
                f: 'dd MMMM yyyy HH:mm',
                F: 'dd MMMM yyyy HH:mm:ss',
                M: 'dd MMMM',
                Y: 'MMMM yyyy'
            }
        }
    }
};
