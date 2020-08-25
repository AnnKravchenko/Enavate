function showFields() {
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