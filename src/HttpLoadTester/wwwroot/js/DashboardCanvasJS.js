
function createCharty(containerSelector , title , xAxis , yAxis) {

    var charty = {};
    charty.dataPoints = []; // dataPoints
    //createCharty("chartContainer","Request Per Second","Request Sequence","Requests");
    charty.chart = new CanvasJS.Chart(containerSelector, {
        title: {text: title},
        axisX:{title: xAxis,},
        axisY: {title: yAxis,},
        data: [{
            type: "line",
            dataPoints: charty.dataPoints
        }]
    });

    charty.xVal = 0;
    charty.dataLength = 500; // number of dataPoints visible at any point

    charty.updateChart = function (yVal) {

        charty.dataPoints.push({
            x: charty.xVal,
            y: yVal
        });
        charty.xVal++;

        if (charty.dataPoints.length > charty.dataLength) {
            charty.dataPoints.shift();
        }

        charty.chart.render();
    };

    return charty;
}