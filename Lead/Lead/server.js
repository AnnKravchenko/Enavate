function hideFields() {
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