
function populateErrorsTable() {
    var errObj = {};
    errObj.templateSource = $("#failedRequestsTemplate").html();
    errObj.template = Handlebars.compile(errObj.templateSource);
    errObj.container = $("#failedRequests");
    errObj.table = errObj.container.find("tbody");

    errObj.populate = function populate(data) {
       // var theData = {
       //     exceptions: [
       //         { testName: "view", time: "11:52", message: "broken", responseCode: "500" },
       //         { testName: "update", time: "12:55", message: "big exceptions", responseCode: "401" }
       //     ]
       // };

        errObj.table.html(errObj.template(data));
    };

    return errObj;
}