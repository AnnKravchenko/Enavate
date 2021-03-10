function OpenSelectedRecord(data) {
    var d = data[0];
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "opportunityproduct";
    entityFormOptions["entityId"] = d.Id;
    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions).then(
        function (success) {
            console.log(success);
        },
        function (error) {
            console.log(error);
        });
};