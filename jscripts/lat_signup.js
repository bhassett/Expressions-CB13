function isNullOrEmpty(val){
    return val == null || val == '';
}

/* global alert mesages */
var _headerAlert = '';
var _fnameAlert = '';
var _lnameAlert = '';
var _emailAlert = '';
var _emailFormatAlert = '';
var _pwdAlert = '';
var _cpwdAlert = '';
var _pwdMatchAlert = '';
var _companyAlert = '';
var _addressAlert = '';
var _cityAlert = '';
var _stateAlert = '';
var _postalAlert = '';
var _countryAlert = '';
var _phoneAlert = '';
var _websiteAlert = '';

function enableJoin(aggreeId, joinId) {
    $(joinId).disabled = ($(aggreeId).checked)? '':'true';
}

function validate(msgboxId, focusId, fnameId, lnameId, emailId, pwdId, cpwdId, companyId, addressId, cityId, stateId, postalId, countryId, phoneId, websiteId){
    var msgbox = $(msgboxId);
    var fname = $(fnameId);
    var lname = $(lnameId);
    var email = $(emailId);
    var pwd = $(pwdId);
    var cpwd = $(cpwdId);
    var company = $(companyId);
    var address = $(addressId);
    var city = $(cityId);
    var state = $(stateId);
    var postal = $(postalId);
    var country = $(countryId);
    var phone = $(phoneId);
    var EcommerceSite = $(websiteId);
    
    var valid = true;
    var err = _headerAlert + "<BR/>";
    
    if(isNullOrEmpty(fname.value)) {
        err += "<BR/>" + "&bull; " + _fnameAlert;
        valid = false;
    }
    
    if(isNullOrEmpty(lname.value)) {
        err += "<BR/>" + "&bull; " + _lnameAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(email.value)) {
        err += "<BR/>" + "&bull; " + _emailAlert;
        valid = false;        
    }
    else {
        var ergx = new RegExp('\\w+([-+.\']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*');
        if( !ergx.test(email.value) ) {
            err += "<BR/>" + "&bull; " + _emailFormatAlert;
            valid = false;
        }
    }
    
    if(isNullOrEmpty(pwd.value)) {
        err += "<BR/>" + "&bull; " + _pwdAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(cpwd.value)) {
        err += "<BR/>" + "&bull; " + _cpwdAlert;
        valid = false;        
    }
    else {
        if(pwd.value != cpwd.value) {
            err += "<BR/>" + "&bull; " + _pwdMatchAlert;
            valid = false;
        }
    }
    
    if(isNullOrEmpty(company.value)) {
        err += "<BR/>" + "&bull; " + _companyAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(address.value)) {
        err += "<BR/>" + "&bull; " + _addressAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(city.value)) {
        err += "<BR/>" + "&bull; " + _cityAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(state.value)) {
        err += "<BR/>" + "&bull; " + _stateAlert;
        valid = true;        
    }
    
    if(isNullOrEmpty(postal.value)) {
        err += "<BR/>" + "&bull; " + _postalAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(country.value)) {
        err += "<BR/>" + "&bull; " + _countryAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(phone.value)) {
        err += "<BR/>" + "&bull; " + _phoneAlert;
        valid = false;        
    }
    
    if(isNullOrEmpty(EcommerceSite.value)) {
        err += "<BR/>" + "&bull; " + _websiteAlert;
        valid = false;        
    }
    
    if(!valid) {
        msgbox.innerHTML = err;
        Field.focus($(focusId));
        return false;
    }
        
    Page_ValidationActive=true;
    return true;
}


