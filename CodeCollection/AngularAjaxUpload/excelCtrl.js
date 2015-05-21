(function () {
    'use strict';

    angular
        .module('app')
        .controller('ExcelCtrl', function ($scope, $http) {

            var importServiceUrl = "api/import/excel",
                exportServiceUrl = "api/export/excel";

            $scope.json = "";

            $scope.import = function () {
                var input = $("#fileinput")[0], 
                    fd = new FormData();
                //Take the first selected file
                fd.append("file", input.files[0]);

                $http.post(importServiceUrl, fd, {
                    withCredentials: true,
                    headers: {'Content-Type': undefined },
                    transformRequest: angular.identity
                }).success(function (data) {
                    $scope.json = JSON.stringify(data);
                }).error(function () {
                });
            };

            $scope.export = function () {
                if (!$scope.json) {
                    return;
                }

                var exporter = new wijmo.grid.ExcelExporter();
                exporter.requestExport(JSON.parse($scope.json), exportServiceUrl, {
                    fileName: "export",
                    type: wijmo.ExportFileType.Xlsx
                });
            };
        });
})();