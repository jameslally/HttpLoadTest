/// <reference path="../../Scripts/jquery-1.8.2.js" />
$(function () {
    "use strict";
    var dashHub = $.connection.dashboardHub,
        messages = $("#messages"),
        statusList = $("#statusList"),
        statusTable = $("#statusTable tbody"),
        start,
        templateSource = $("#some-template").html(),
        template = Handlebars.compile(templateSource);

    var charty = createCharty("chartContainer", "Request Per Second", "Request Sequence", "Requests");

    $.connection.hub.logging = true;

    HandlebarsIntl.registerWith(Handlebars);

    function SetDataOnRow(li , data) 
    {
        li.data('status', data.Status);
        li.data('count', data.Count);
    }

    dashHub.client.displayFromHub = function (value) {
        if (value) {
            var json = eval("(" + value + ")");

            for (var i = 0; i < json.Rows.length; i++) {
                var obj = json.Rows[i];

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

            var last = json.ProcessedInLastMinute;
            charty.updateChart(last)
        }
        //tick();
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

    $.connection.hub.reconnected(function() {
        $('#hub-status').removeClass('alert-danger').addClass('alert-success').text('Reconnected');
    });
    $.connection.hub.error(function(err) {
        $('#hub-status').removeClass('alert-success').addClass('alert-danger').text(err);
    });
    $.connection.hub.disconnected(function() {
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