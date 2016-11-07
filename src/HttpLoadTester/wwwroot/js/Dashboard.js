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

    $.connection.hub.logging = true;

    HandlebarsIntl.registerWith(Handlebars);

    function SetDataOnRow(li , data) 
    {
        li.data('processed', data.Processed);
        li.data('unprocessed', data.Unprocessed);
        li.data('patientCount', data.PatientCount);

        var b = li.find("#btnStart" + data.Code);
        b.data('code', data.Code);
        b.data('state', data.State);

        if (data.Unprocessed > 0) {
            li.addClass("warning");
            b.attr('disabled', 'disabled').find('span').text('Running');
        }
        else {
            li.removeClass("warning");
            b.removeAttr('disabled').find('span').text('Start');
        }
    }

    dashHub.client.displayFromHub = function (value) {
        alert(value);
        var json = eval("(" + value + ")");

        for (var i = 0; i < json.length; i++) {
            var obj = json[i];

            var li = statusTable.find("#org-" + obj.Code);

            var data = {
                Code: obj.Code,
                Name: obj.Name,
                Processed: obj.Processed,
                Unprocessed: obj.Unprocessed,
                PatientCount: obj.PatientCount,
                State: obj.State,
                LastDuration: obj.LastDuration,
                PercentDone: obj.PercentDone,
                PercentDoneDisplay: obj.PercentDoneDisplay,
                Started: obj.Started,
                Ended: obj.Ended,
                UseHesData: obj.UseHesData
            };

            if (li.length === 0) {
                li = $(template(data));
                li.appendTo(statusTable);

                SetDataOnRow(li, data);

                var b = li.find("#btnStart" + data.Code);

                b.click(function () {
                    $(this).find('i').collapse('show');
                    dashHub.server.sendStatusToHub($(this).data('code'), 'run').done(function (value) { });
                });
            }

            if (li.data('processed') !== data.Processed
                    || li.data('unprocessed') !== data.Unprocessed
                    || li.data('patientCount') !== data.PatientCount) {

                li.html($(template(data)).html());

                SetDataOnRow(li, data);
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