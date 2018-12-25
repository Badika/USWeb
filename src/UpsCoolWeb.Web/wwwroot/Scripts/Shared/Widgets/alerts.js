Alerts = {
    init: function () {
        this.element = document.querySelector('.alerts');
        [].forEach.call(this.element.children, this.bind);
    },

    show: function (alerts) {
        alerts = [].concat(alerts);
        for (var i = 0; i < alerts.length; i++) {
            var alert = document.getElementById(alerts[i].id) || emptyAlert();
            alert.setAttribute('data-timeout', alerts[i].timeout || 0);
            alert.className = 'alert alert-' + getType(alerts[i].type);
            alert.children[0].innerText = alerts[i].message || '';
            alert.id = alerts[i].id || '';

            this.element.appendChild(alert);
            this.bind(alert);
        }

        function emptyAlert() {
            var alert = document.createElement('div');
            var close = document.createElement('span');
            var message = document.createElement('span');

            close.innerHTML = '&#x00D7;';
            close.className = 'close';

            alert.append(message);
            alert.append(close);

            return alert;
        }

        function getType(id) {
            switch (id) {
                case 0:
                    return 'danger';
                case 1:
                    return 'warning';
                case 2:
                    return 'info';
                case 3:
                    return 'success';
                default:
                    return id;
            }
        }
    },
    bind: function (alert) {
        alert.lastElementChild.onclick = function () {
            Alerts.close(this.parentNode);
        };

        if (alert.dataset.timeout > 0) {
            setTimeout(this.close, alert.dataset.timeout, alert);
        }
    },
    close: function (alert) {
        if (typeof alert == 'string') {
            alert = document.getElementById(alert);
        }

        $(alert).fadeTo(300, 0).slideUp(300, function () {
            this.parentNode.removeChild(this);
        });
    },
    closeAll: function () {
        $(this.element).children().fadeTo(300, 0).slideUp(300, function () {
            this.parentNode.removeChild(this);
        });
    },

    clear: function () {
        this.element.innerHTML = '';
    }
};
