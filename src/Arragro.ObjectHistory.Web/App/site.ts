import * as $ from 'jquery'
import 'jquery-validation'
import 'jquery-validation-unobtrusive'
import 'bootstrap'


function getHref(folderGuid, fileName) {
    return "/ObjectHistoryDetails?folder=" + folderGuid + "/" + fileName;
}
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}




function GlobalLogsViewModel() {
    var self = this;

    self.partitionKey = ko.observable();
    self.results = ko.observableArray([]);
    self.continuationToken = ko.observable();

    var getLogs = function () {
        var postData = {
            tableContinuationToken: self.continuationToken()
        };

        $.post("/arragro-object-history/get-global-logs", postData, function (data) {
            self.continuationToken(data.continuationToken);
            ko.utils.arrayPushAll(self.results, data.results);
        });
    };

    self.getDownloadHref = function (folder) {
        return '/FileDetails/Download?folder=' + folder;
    };


    self.getMoreFileLogs = function () {
        getLogs();
    };  

    var loadFormFromUrl = function () {
        var partitionKey = getParameterByName('partitionKey');

        if (partitionKey !== null && partitionKey.length > 0)
            self.partitionKey(partitionKey);
    }

    loadFormFromUrl();
    getLogs();
}

$(function () {

    ko.bindingHandlers.date = {
        update: function (element, valueAccessor) {
            var value = valueAccessor();
            if (value !== null && value !== undefined && value.length > 5) {
                var dateconverted = Date.parse(value);
                var date = new Date(dateconverted);
                $(element).text(date.toLocaleDateString('en-NZ') + ' ' + date.toLocaleTimeString());
            }
        }
    };


    if ($('#logDetails').length > 0) {

        ko.applyBindings(new GlobalLogsViewModel());
    }
});
