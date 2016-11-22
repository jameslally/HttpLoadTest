﻿
function createCharty(containerSelector, title, xAxis, yAxis) {

    var charty = {};
    charty.dataPoints = [];

    charty.chart = new Chart(containerSelector, {
        type: 'line',
        data: {
            datasets: [{
                label: title,
                data: charty.dataPoints
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'linear',
                    position: 'bottom'
                }]
            }
        }
    });
    charty.xVal = 0;
    charty.dataLength = 50; // number of dataPoints visible at any point

    charty.updateChart = function (yVal) {

        charty.dataPoints.push({
            x: charty.xVal,
            y: yVal
        });
        charty.xVal++;

        if (charty.dataPoints.length > charty.dataLength) {
            charty.dataPoints.shift();
        }

        charty.chart.update();
    };


    return charty;
}

function createDurationCharty(containerSelector, title, xAxis, yAxis) {

    var charty = {};
    charty.dataPoints = [];

    charty.chart = new Chart(containerSelector, {
        type: 'line',
        data: {
            datasets: [{
                label: title,
                data: charty.dataPoints
            }]
        },
        options: {
            scales: {
                xAxes: [{
                    type: 'time',
                    time: {
                        displayFormats: {
                            minute: 'hh:mm'
                        }
                    }
                }],
                yAxes: [{
                    min: 0
                }]
            }
        }
    });
    
    charty.updateChart = function (data) {

        charty.dataPoints.length = 0;
        data.forEach(function(element) {
            charty.dataPoints.push({
                x: moment(element.x,"HH:mm"),
                y: element.y});
        }, this);

        charty.chart.update();
    };


    return charty;
}