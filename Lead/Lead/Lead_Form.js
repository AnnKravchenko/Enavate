function hideFieldsOnCreation() {
    var topic = Xrm.Page.getAttribute("subject").getValue()
    if (topic === null) {
        var attributes = []
        var ad = 'address1_composite'
        Xrm.Page.data.entity.attributes.forEach(function (element, index) {
            if (element.getName()!==ad)
                attributes[index] = element.getName()
        })
        attributes.forEach(function (element, index) {
            if (Xrm.Page.getControl(element) !== null || element.startsWith('address1_') ) {
                if (Xrm.Page.getAttribute(element).getRequiredLevel() === "none") {
                    if (element.startsWith('address1_')) 
                        Xrm.Page.getControl('address1_composite_compositionLinkControl_'+element).setVisible(false)
                    else
                        Xrm.Page.getControl(element).setVisible(false)
                } else {
                    if (element.startsWith('address1_'))
                        Xrm.Page.getControl('address1_composite_compositionLinkControl_' + element).setVisible(true)
                    else
                        Xrm.Page.getControl(element).setVisible(true)
                }
            }
        })
    }
};

function showFieldsOptionSet() {
    var selectedValue = Xrm.Page.getAttribute("new_optionset").getValue()
    switch (selectedValue) {
        case 100000000:
            Xrm.Page.getControl('mobilephone').setVisible(true);
            Xrm.Page.getControl('emailaddress1').setVisible(false);
            break;
        case 100000001:
            Xrm.Page.getControl('mobilephone').setVisible(false);
            Xrm.Page.getControl('emailaddress1').setVisible(true);
            break;
        case 100000002:
            Xrm.Page.getControl('mobilephone').setVisible(true);
            Xrm.Page.getControl('emailaddress1').setVisible(true);
            break;
        default:
            Xrm.Page.getControl('mobilephone').setVisible(false);
            Xrm.Page.getControl('emailaddress1').setVisible(false);
            break;
    }
};