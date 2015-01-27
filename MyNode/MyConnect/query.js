var qs = require("qs");

module.exports = function (options) {
    return function (req, res, next) {
        if (!req.query) {
            req.query = ~req.url.indexOf('?')
            ? qs.parse(req._parsedUrl.query, options)
            : {};
        }
        next();
    };
};