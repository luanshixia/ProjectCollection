
var showMap = function (service) {
    var slCtl = document.getElementById('slCtl');
    slCtl.Content.cityWebGis.ShowMap(service);
};

var reloadToSlCtl = function () {
    if ($('#slCtl').length > 0 && location.toString().indexOf('#slCtl') < 0) {
        location = '#slCtl';
    }
};

(function ($) {
    $.fn.preventScroll = function () {
        var _this = this.get(0);
        if (!_this) {
            return this;
        }
        if ($.browser.mozilla) {
            _this.addEventListener('DOMMouseScroll', function (e) {
                _this.scrollTop += e.detail > 0 ? 60 : -60;
                e.preventDefault();
            }, false);
        } else {
            _this.onmousewheel = function (e) {
                e = e || window.event;
                _this.scrollTop += e.wheelDelta > 0 ? -60 : 60;
                e.returnValue = false;
                return false;
            };
        }
        return this;
    };
})(jQuery);

$(document).ready(function () {
    $('#silverlightControlHost').preventScroll();
    reloadToSlCtl();
});

function onSilverlightError(sender, args) {
    var appSource = "";
    if (sender != null && sender != 0) {
        appSource = sender.getHost().Source;
    }

    var errorType = args.ErrorType;
    var iErrorCode = args.ErrorCode;

    if (errorType == "ImageError" || errorType == "MediaError") {
        return;
    }

    var errMsg = "Silverlight 应用程序中未处理的错误 " + appSource + "\n";

    errMsg += "代码: " + iErrorCode + "    \n";
    errMsg += "类别: " + errorType + "       \n";
    errMsg += "消息: " + args.ErrorMessage + "     \n";

    if (errorType == "ParserError") {
        errMsg += "文件: " + args.xamlFile + "     \n";
        errMsg += "行: " + args.lineNumber + "     \n";
        errMsg += "位置: " + args.charPosition + "     \n";
    }
    else if (errorType == "RuntimeError") {
        if (args.lineNumber != 0) {
            errMsg += "行: " + args.lineNumber + "     \n";
            errMsg += "位置: " + args.charPosition + "     \n";
        }
        errMsg += "方法名称: " + args.methodName + "     \n";
    }

    throw new Error(errMsg);
}