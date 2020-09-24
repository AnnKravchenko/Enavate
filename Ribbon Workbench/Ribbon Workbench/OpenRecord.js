function OpenSelectedRecord(data) {
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "opportunityproduct";
    entityFormOptions["entityId"] = data;
    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions).then(
        function (success) {
            console.log(success);
        },
        function (error) {
            console.log(error);
        });
};