var nextXPoint = 0;

var charty = {};


window.onload = function () {

    charty.dps = []; // dataPoints

    charty.chart = new CanvasJS.Chart("chartContainer", {
        title: {
            text: "Request Per Second"
        },
        axisX:{
        title:"Request Sequence",
        },
        axisY: {
            title: "Requests",
        },
        data: [{
            type: "line",
            dataPoints: charty.dps
        }]
    });

    charty.xVal = 0;
    charty.yVal = 100;
    charty.updateInterval = 100;
    charty.dataLength = 500; // number of dataPoints visible at any point

    charty.updateChart = function (yVal) {

        charty.dps.push({
            x: charty.xVal,
            y: yVal
        });
        charty.xVal++;

        if (charty.dps.length > charty.dataLength) {
            charty.dps.shift();
        }

        charty.chart.render();

    };

    // generates first set of dataPoints
    //updateChart(dataLength);

    // update chart after specified time. 
    //setInterval(function () { updateChart() }, updateInterval);

}