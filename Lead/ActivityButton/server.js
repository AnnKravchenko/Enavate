'use strict';

var LeadId = Xrm.Page.data.entity.getId()
var id = LeadId.slice(1, LeadId.length-1)

Xrm.WebApi.retrieveMultipleRecords("task", "?$filter=_regardingobjectid_value eq "+id.toLowerCase()).then(
    function success(result) {
        for (var i = 0; i < result.entities.length; i++) {
            var li = document.createElement("li")
            var state
            switch (result.entities[i].statecode) {
                case 0:
                    state = "Open"
                    break
                case 1:
                    state = "Completed"
                    break
                case 2:
                    state = "Canceled"
                    break
            }
            var node = document.createTextNode(result.entities[i].subject + "(" + state + ")")
            li.appendChild(node)
            document.getElementById("tasks").appendChild(li)
        }
        if (result.entities.length !== 0)
            document.getElementById("related").innerHTML = "Related Tasks: "
        else
            document.getElementById("related").innerHTML = "No related tasks yet"
    },
    function (error) {
        console.log(error.message)
    }
);
 

function button() {
    var x = document.getElementById("hide")
    if (x.style.display === "none")
        x.style.display = "block";
    else
        x.style.display = "none";
}