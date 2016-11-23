function populateErrorTable() {
    var theData = {
            exceptions: [
                {testName: "view", time: "11:52", message: "broken", responseCode: "500" },
                {testName: "update", time: "12:55", message: "big exceptions", responseCode: "401" }
            ]
        };

    

    var templateSource = $("#failedRequestsTemplate").html(),
        template = Handlebars.compile(templateSource);

    var container = $("#failedRequests");
    container.find("tbody").html(template(theData));
}