

//
// Javascript and jQuery extensions
//

Object.beget = function (o) {
    var F = function () { };
    F.prototype = o;
    return new F();
};

Function.prototype.method = function (name, func) {
    if (!this.prototype[name]) {
        this.prototype[name] = func;
    }
};

jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        var path = options.path ? '; path=' + options.path : '';
        var domain = options.domain ? '; domain=' + options.domain : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

$.reduce = function (arr, func) {
    var temp = arr[0];
    for (var i = 1; i < arr.length; i++) {
        temp = func(temp, arr[i]);
    }
    return temp;
};

//
// tj functions
//

var tj = {};

tj.timerAction = function (id, action) {
    var update = function () {
        var s = $(id).text() - 1;
        if (s > 0) {
            $(id).text(s);
        } else if (s == 0) {
            action();
        }
    };

    var countTimer = function () {
        setInterval(update, 1000);
    };

    countTimer();
};

tj.timerJump = function (id, url) {
    tj.timerAction(id, function () {
        location = url;
    });
};

tj.timerClose = function (id) {
    tj.timerAction(id, function () {
        window.close();
    });
};

tj.timeoutJump = function (ms, url) {
    setTimeout(function () {
        location = url;
    }, ms);
};

tj.nav = function (url) {
    location = url;
};

tj.confirmGoTo = function (msg, url) {
    if (confirm(msg)) {
        window.location = url;
    }
};

tj.setTableStyle = function () {
    $('table:not(.layout-table) tr:even').css({ 'background-color': '#FEEFD1' });
    $('table:not(.layout-table) tr:odd').css({ 'background-color': '#EEEEEE' });
    $('table:not(.layout-table) th').css({ 'background-color': '#DDDDDD', 'color': '#777777' });

    $('table.layout-table tr:even').css({ 'background-color': 'transparent' });
    $('table.layout-table tr:odd').css({ 'background-color': 'transparent' });
    $('table.layout-table th').css({ 'background-color': 'transparent' });
};

tj.setTableStyleById = function (id) {
    if (id.indexOf('#') !== 0) {
        id = '#' + id;
    }
    $(id + ' tr:first').addClass('header');
    $(id + ' tr:even').addClass('even');
    $(id + ' tr:odd').addClass('odd');
};

tj.setTableColumnGroupStarWidth = function (id, stars) {
    var sumStar = $.reduce(stars, function (x, y) { return x + y });
    var percentages = $.map(stars, function (x) { return x / sumStar * 100 });
    var colgroup = $('<colgroup>');
    $.each(percentages, function (i, p) {
        colgroup.append($('<col>').attr('width', p + '%'));
    });
    $(id).prepend(colgroup);
};

//
// tj.Notificatior
//

tj.Notificator = function (messages, container) {
    this.messages = messages;
    this.container = container;
};

tj.Notificator.show = function (message, container, callback) {
    container.html(decodeURI(message)).stop(true).animate({ 'margin-top': '0px', 'opacity': '0.7' }, 400);
    setTimeout(function () {
        container.stop(true).animate({ 'margin-top': '-292px', 'opacity': '0' }, 600);
        setTimeout(function () {
            if (callback) {
                callback();
            }
        }, 800);
    }, 4000);
};

tj.Notificator.prototype.showNext = function () {
    var message = this.messages.shift();
    tj.Notificator.show(message, this.container);
};

tj.Notificator.prototype.showAll = function () {
    var that = this;
    var recfunc = function () {
        var message = that.messages.shift();
        if (message) {
            tj.Notificator.show(message, that.container, recfunc);
        }
    }
    recfunc();
};

tj.Notificator.send = function (serviceUrl, user, message) {
    $.ajaxSetup({ cache: false });
    $.get(serviceUrl, { username: user, message: message });
};


// Animations

$(document).ready(function () {
    var n = $('#nav li').size();
    var total = 2000;
    var a = total / (n * n + n);
    $('#nav li').css('font-size', '21px').each(function (i) {
        var that = this;
        setTimeout(function () {
            $(that).animate({
                fontSize: '16px'
            }, i * i * a, 'easeOutCubic');
        }, i * a);
    });
});

$(document).ready(function () {
    var n = $('p').size();
    var total = 2000;
    var a = total / (n * n + n);
    $('p').css('margin-top', '30px').each(function (i) {
        var that = this;
        setTimeout(function () {
            $(that).animate({
                marginTop: '20px'
            }, i * i * a, 'easeOutCubic');
        }, i * a);
    });
});

$(document).ready(function () {
    var n = $('tr').size();
    if (n > 100) return;
    var total = 2000;
    var a = total / (n * n + n);
    $('tr').css('opacity', '0').each(function (i) {
        var that = this;
        setTimeout(function () {
            $(that).animate({
                opacity: '1'
            }, i * i * a, 'easeOutCubic');
        }, i * a);
    });
});

$(document).ready(function () {
    var n = $('form div').size();
    var total = 2000;
    var a = total / (n * n + n);
    $('form div').css('margin-top', '15px').each(function (i) {
        var that = this;
        setTimeout(function () {
            $(that).animate({
                marginTop: '5px'
            }, i * i * a, 'easeOutCubic');
        }, i * a);
    });
});

// Framed Layout

tj.showDialog = function (title, url) {
    $("#frame-dim").show();
    $("#frame-dialog").show().stop().css('top', '30px').css('opacity', '0.5').animate({
        top: '0',
        opacity: '1'
    }, 500, 'easeOutCubic');
    $("#dialog-title").text(title);
    $("#dialog-content").html('loading...');
    $.ajaxSetup({ cache: false });
    $("#dialog-content").load(url);
};

tj.showDialogDiv = function (title, div) {
    $("#frame-dim").show();
    $("#frame-dialog").show().stop().css('top', '30px').css('opacity', '0.5').animate({
        top: '0',
        opacity: '1'
    }, 500, 'easeOutCubic');
    $("#dialog-title").text(title);
    $("#dialog-content").html($('#' + div).html());
};

tj.hideDialog = function () {
    $("#frame-dim").hide();
    $("#frame-dialog").stop().animate({
        top: '-30px',
        opacity: '0'
    }, 500, 'easeOutCubic', function () {
        $(this).hide();
    });
};