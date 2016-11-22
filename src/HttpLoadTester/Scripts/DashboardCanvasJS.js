
function createCharty(containerSelector) {

    var charty = {};
    charty.dataPoints = [];
    charty.dataPointsFailed = [];
    charty.chart = new Chart(containerSelector, {
        type: 'line',
        data: {
            datasets: [{
                label: "Successful",
                data: charty.dataPoints,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)'
            }
            ,{
                label: "Failed",
                data: charty.dataPointsFailed,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255,99,132,1)'
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
                    min: 0,
                    stacked: true
                }]
            }
        }
    });

    charty.updateChart = function (successData,failedData) {

        charty.dataPoints.length = 0;
        charty.dataPointsFailed.length = 0;


        successData.forEach(function(element) {
            charty.dataPoints.push({
                x: moment(element.x,"HH:mm"),
                y: element.y});
        }, this);

        failedData.forEach(function(element) {
            charty.dataPointsFailed.push({
                x: moment(element.x,"HH:mm"),
                y: element.y});
        }, this);

        charty.chart.update();
    };


    return charty;
}

function createDurationCharty(containerSelector) {

    var charty = {};
    charty.dataPoints = [];

    charty.chart = new Chart(containerSelector, {
        type: 'line',
        data: {
            datasets: [{
                label: "Avg Response Time (ms)",
                data: charty.dataPoints,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)'
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