var connect = require('connect');
var query = require('./query');

var app = connect()
    .use(query())
    .use(function (req, res) {
        var name = req.query.name;
        res.end("hello " + name);
    });

app.listen(8080);
console.log('Server started on port 8080.');