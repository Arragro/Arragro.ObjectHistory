import * as $ from 'jquery'
import 'jquery-validation'
import 'jquery-validation-unobtrusive'
import 'bootstrap'
import { diff, formatters } from 'jsondiffpatch';

class ObjectHistoryFile{
    objectName: string;
    folder: string;
    modifiedBy: KnockoutObservable<string>;
    modifiedDate: KnockoutObservable<Date>;
    newJson: KnockoutObservable<string>;
    oldJson: KnockoutObservable<string>;
    diff: KnockoutObservable<any>;
    fileDownloaded: boolean;
    isAdd: boolean;

    constructor(model: any) {
        this.objectName = model.objectName;
        this.folder = model.folder;
        this.modifiedBy = ko.observable(model.modifiedBy);
        this.modifiedDate = ko.observable(model.modifiedDate);
        this.fileDownloaded = false;
        this.oldJson = ko.observable();
        this.newJson = ko.observable();
        this.diff = ko.observable();
        this.isAdd = model.isAdd;
    }
}

function GlobalLogsViewModel() {
    var self = this;

    self.partitionKey = ko.observable();
    self.objectHistoryFiles = ko.observableArray([]);
    self.relatedObjectLogs = ko.observableArray([]);
    self.continuationToken = ko.observable();

    var getLogs = function () {
        var postData = {
            tableContinuationToken: self.continuationToken()
        };

        $.post("/arragro-object-history/get-global-logs", postData, function (data) {
            self.partitionKey(data.partitionKey);
            self.continuationToken(data.continuationToken);
            for (var i = 0; i < data.results.length; i++) {
                var objectHistoryFile = new ObjectHistoryFile(data.results[i]);
                self.objectHistoryFiles.push(objectHistoryFile);
            }
        });
    };

    self.getRelatedObjectLogs = function (data) {

            var postData = {
                partitionKey: data.objectName
            }


        $.post("/arragro-object-history/get-object-logs", postData, function (postResults) {
            self.partitionKey(postResults.partitionKey);
            self.continuationToken(postResults.continuationToken);
            for (var i = 0; i < postResults.results.length; i++) {
                var objectHistoryFile = new ObjectHistoryFile(postResults.results[i]);
                self.objectHistoryFiles.push(objectHistoryFile);
            }
        });
    };

    self.getDownloadHref = function (folder) {
        return '/arragro-object-history/diff?folder=' + folder;
    };


    self.getfile = function (data) {

        var objectHistoryFile = ko.utils.arrayFirst(self.objectHistoryFiles(), (item: ObjectHistoryFile) => { return item.folder === data.folder; });

        if (objectHistoryFile.fileDownloaded === false) {
            var postParameters = {
                partitionKey: data.folder
            }

            $.post("/arragro-object-history/get-object-log", postParameters, function (jsonFile) {
                objectHistoryFile.diff(formatters.html.format(jsonFile.diff, jsonFile.oldJson));
                objectHistoryFile.oldJson(jsonFile.oldJson);
                objectHistoryFile.newJson(jsonFile.newJson);
                objectHistoryFile.fileDownloaded = true;
                self.objectHistoryFiles.valueHasMutated();
            });
        }
    }

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
        ko.options.useOnlyNativeEvents = true;
        ko.applyBindings(new GlobalLogsViewModel());
    }
});
