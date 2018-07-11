import * as $ from 'jquery'
import 'jquery-validation'
import 'jquery-validation-unobtrusive'
import 'bootstrap'


//class GlobalLogsViewModel {
//    partitionKey: KnockoutObservable<string>
//    results: KnockoutObservableArray<any>
//    continuationToken: KnockoutObservable<string>

//    constructor() {
//        this.getLogs();
//        this.results = ko.observableArray<any>(new Array);

//    }

//    getMoreFileLogs(){
//       this.getLogs();
//    }; 

//    getLogs() {
//        var postData = {
//            tableContinuationToken: this.continuationToken
//        };

//        $.post("/arragro-object-history/get-global-logs", postData, function (data) {
//            this.partitionKey = ko.observable(data.partitionKey);
//            this.continuationToken = ko.observable(data.continuationToken);

//            ko.utils.arrayPushAll(this.results, data.results);
//        });

//    };
//}




//function getHref(folder) {
//    return "/ObjectHistoryDetails?folder=" + folder;
//}

//function getParameterByName(name) {
//    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
//    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
//        results = regex.exec(location.search);
//    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
//}




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
            self.partitionKey(data.partitionKey);
            self.continuationToken(data.continuationToken);
            ko.utils.arrayPushAll(self.results, data.results);
        });
    };

    self.getDownloadHref = function (folder) {
        return '/arragro-object-history/download?folder=' + folder;
    };

    self.getMoreFileLogs = function () {
        getLogs();
    };  

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

        //ko.applyBindings(new GlobalLogsViewModel());
        ko.applyBindings(new GlobalLogsViewModel());
    }
});
