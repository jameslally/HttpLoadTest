
$(function () {
    "use strict";
    var dashHub = $.connection.dashboardHub,
        messages = $("#messages"),
        statusList = $("#statusList"),
        start,
        templateSource = $("#some-template").html(),
        template = Handlebars.compile(templateSource),
        testContainer = {};

    $.connection.hub.logging = true;
    HandlebarsIntl.registerWith(Handlebars);

    $(".testContainer").each(function () {
        var testName = $(this).data('testname');
        var chartContainer = $(this).find(".chartContainer");
        var chartDurationContainer = $(this).find(".chartDurationContainer");


        testContainer[testName] = {};
        testContainer[testName].charty = createCharty(chartContainer);
        testContainer[testName].durationCharty = createDurationCharty(chartDurationContainer);

        testContainer[testName].statusTable = $(this).find("tbody");

        $(this).find(".sendStartToHub").click(function () { dashHub.server.sendStartToHub(testName); });
        $(this).find(".sendStopToHub").click(function () { dashHub.server.sendStopToHub(testName); });
    })




    function SetDataOnRow(li, data) {
        li.data('status', data.Status);
        li.data('count', data.Count);
    }

    dashHub.client.displayDurationReportFromHub = function (value) {
        if (value) {
            var json = eval("(" + value + ")");
            for (var reportId = 0; reportId < json.length; reportId++) {
                var report = json[reportId]
                var testName = report.Name;
                var dataPoints = [];
                var successData = [];
                var failedData = [];

                for (var i = 0; i < report.Items.length; i++) {
                    var obj = report.Items[i];
                    dataPoints.push({
                        x: obj.EventTime,
                        y: obj.AverageDuration
                    });

                    successData.push({
                        x: obj.EventTime,
                        y: obj.SuccessfulRequests
                    });

                    failedData.push({
                        x: obj.EventTime,
                        y: obj.FailedRequests
                    });
                };
                testContainer[testName].durationCharty.updateChart(dataPoints);


                //NumberOfRequests
                testContainer[testName].charty.updateChart(successData,failedData);
            }
        }

    };

    dashHub.client.displayFromHub = function (value) {
        if (value) {
            var json = eval("(" + value + ")");
            for (var reportId = 0; reportId < json.length; reportId++) {
                var report = json[reportId]
                var testName = report.Name;
                var statusTable = testContainer[testName].statusTable;

                for (var i = 0; i < report.Rows.length; i++) {
                    var obj = report.Rows[i];

                    var li = statusTable.find("#org-" + obj.Status);

                    var data = {
                        Status: obj.Status,
                        Count: obj.Count,
                        AverageDuration: obj.AverageDuration
                    };

                    if (li.length === 0) {
                        li = $(template(data));
                        li.appendTo(statusTable);

                        SetDataOnRow(li, data);
                    }

                    if (li.data('count') !== data.Count) {

                        li.html($(template(data)).html());

                        SetDataOnRow(li, data);
                    }
                }

                var last = report.ProcessedInLastMinute;

            }
        }
    };

    $.connection.hub.stateChanged(function (change) {
        var oldState = null,
            newState = null;

        for (var state in $.signalR.connectionState) {
            if ($.signalR.connectionState[state] === change.oldState) {
                oldState = state;
            }
            if ($.signalR.connectionState[state] === change.newState) {
                newState = state;
            }
        }

        //$("<li/>").html("[" + new Date().toTimeString() + "]: " + oldState + " => " + newState + " " + $.connection.hub.id)
        //            .appendTo(messages);
    });

    $.connection.hub.reconnected(function () {
        $('#hub-status').removeClass('alert-danger').addClass('alert-success').text('Reconnected');
    });
    $.connection.hub.error(function (err) {
        $('#hub-status').removeClass('alert-success').addClass('alert-danger').text(err);
    });
    $.connection.hub.disconnected(function () {
        $('#hub-status').removeClass('alert-success').addClass('alert-warning').text('Disconnected');
    });

    start = function () {
        $.connection.hub.start({ transport: activeTransport, jsonp: isJsonp })
            .done(function () {
                //$("<li/>").html("Started transport: " + $.connection.hub.transport.name + " " + $.connection.hub.id)
                //    .appendTo(messages);
                $('#hub-status').addClass('alert-success').text('Connected');
            });
    };

    start();
});