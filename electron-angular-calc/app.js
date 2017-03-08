var app = angular.module('app', []);
app.controller('Calc', function ($scope) {
    $scope.display = '0';
    var operation = 'new';
    var cache = 0;
    var right = 0;
    var toAppend = false;
    var gotResult = false;
    var parse = function () {
        right = parseFloat($scope.display);
    };
    $scope.appendChar = function (c) {
        if (toAppend === false) {
            if (gotResult === true) {
                $scope.clear();
            }
            $scope.display = c;
            toAppend = true;
        } else {
            if ($scope.display.indexOf('.') > -1 && c === '.') {
            } else {
                if ($scope.display === '0') {
                    if (c !== '.') $scope.display = c; else $scope.display = '0.';
                } else {
                    $scope.display += c;
                }
            }
        }
    };
    $scope.setOperation = function (o) {
        if (gotResult !== true && operation !== o) {
            calculate();
        }
        operation = o;
        toAppend = false;
        gotResult = false;
    };
    $scope.result = function () {
        calculate();
        gotResult = true;
    };
    var calculate = function () {
        if (gotResult === false) {
            parse();
        }
        if (operation === 'new') {
            cache = right;
        } else if (operation === '+') {
            cache += right;
        } else if (operation === '-') {
            cache -= right;
        } else if (operation === '*') {
            cache *= right;
        } else if (operation === '/') {
            cache /= right;
        }
        $scope.display = cache.toString();
        toAppend = false;
    };
    $scope.clear = function () {
        $scope.display = '0';
        operation = 'new';
        cache = 0;
        toAppend = false;
        gotResult = false;
    };
    $scope.clearEntry = function () {
        $scope.display = '0';
        toAppend = false;
        gotResult = false;
    };
});
