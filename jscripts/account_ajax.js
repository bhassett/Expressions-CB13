Type.registerNamespace('ise.Pages');

ise.Pages._Account = function() {
    ise.Pages._Account.initializeBase(this);
    
    this.billingAddressControl = null;    
    //this.onFormSet = this.onFormSetHandler;
}
ise.Pages._Account.registerClass('ise.Pages._Account', ise.Pages.BasePage);
ise.Pages._Account.prototype = {

    setBillingAddressControlId : function(id) {
        var handler = Function.createDelegate(this, this.setBillingAddressControl);
        this.loadControl(id, ise.Controls.AddressController, handler);
    },
    
    setBillingAddressControl : function(control) {
        if(control) {
            this.billingAddressControl = control;
        }
    },    
    
    clearValidations : function() {
        this.billingAddressControl.clearValidationSummary();
    },
    
    onFormSetHandler : function() {
        if(this.form) {
            this.form.onsubmit = Function.createDelegate(this, this.validate);
        }
    },
    
    validate : function() {
        var isValid = false;
        var clear = false;
        
        this.clearValidations();
        
        isValid = this.billingAddressControl.validate(clear);
        
        return isValid;
    }
    
}

// singleton object
ise.Pages.Account = new ise.Pages._Account();

ise.Pages.Account.OrderHistoryControl = function(id){
    this.content = $getElement(id + '_Content');
    this.page = $getElement(id + '_Pager');
    
    this.pageHeader = $getElement(id + '_PagingHeader');
    this.pageFooter = $getElement(id + '_PagingFooter');
    
    this.page.style.display = 'none'; //.hide();
    this.txtPages = $getElement(id + '_Pages');
    this.lnkReset = $getElement(id + '_Reset');
    
    this.lnkReset.onclick = Function.createDelegate(this, this.onResetClick); //this.onResetClick.bind(this);
}
ise.Pages.Account.OrderHistoryControl.registerClass('ise.Pages.Account.OrderHistoryControl');
ise.Pages.Account.OrderHistoryControl.prototype = {

    onResetClick : function() {
        this.changePage(1);
    },
    
    getPages : function() {
        return this.txtPages.value;
    },
    
    setDisplay : function(html) {
        if(this.content) {
            this.content.innerHTML = html;
        }
    },
    
    changePage : function(current) {
        var pages = this.getPages();
        var params = 'Pages=' + pages + '&Current=' + current;
        
        var del = Function.createDelegate(this, this.onLoadOrderHistoryComplete);
        var service = new ActionService();        
        service.GetOrderHistory(pages, current, del);       
    },
    
    onLoadOrderHistoryComplete : function(result) {
        this.setDisplay(result.html);
        this.buildPages(this.pageHeader, result.current, result.all);
        this.prePareReOrderCapability();        
        this.page.style.display = '';
    },
    
    prePareReOrderCapability : function() {
        var reOrderLinks = getElementsByClassName('a', 'lnkReOrder');
                
        for(var ctr=0; ctr<reOrderLinks.length; ctr++) {
            var link = reOrderLinks[ctr];
            if(link.attributes['so']) {                
                link.onclick = function() {
                    var prompt = ise.StringResource.getString('account.aspx.26'); //'Are you sure you want to re-order this order?';

                    if(confirm(prompt)) {
                        var code = this.attributes['so'].value;
                        this.href = 'reorder.aspx?so=' + code;                        
                        
                        return true;
                    }
                }
            }
        }
    },
    
    buildPages : function(onPanel, current, all) {
        // clear
        onPanel.innerHTML = '';
        var range = 5;
        
        var start = current - range;
        var end = current + range;
        
        var dlgChangePage = Function.createDelegate(this, this.changePage); 
        var pages = this.getPages();
        var dlgWritePage = function(text, page, noLink){
            var p = null;
            if(noLink){
              p = document.createElement('span');
              p.className = 'OrderHistoryCurrentSelectedPage';
            }
            else {
              p = document.createElement('a');
              p.href = 'javascript:void(0);';
              p.onclick = function(){dlgChangePage(page?page:text)};
            }
            p.innerHTML = text;
            onPanel.appendChild(p);
        }
        
        var dlgWriteSpace = function() {
            var space = document.createElement('span');
            space.innerHTML = '&nbsp;';
            onPanel.appendChild(space);
        }        
        
        start = (start > 0)? start : 1;
        var writeFirst = start > 1;
        
        if(writeFirst) {
            dlgWriteSpace();
            dlgWritePage('First', 1);
            dlgWriteSpace();
        }
        
        var ctr = start;
        while(ctr < current) {
            dlgWriteSpace();
            dlgWritePage(ctr);
            dlgWriteSpace();
            ctr++;
        }
        
        dlgWriteSpace();
        dlgWritePage(current, current, true);
        dlgWriteSpace();
        
        if(end > all) {
          end = all;
        }
        
        var writeLast = end < all;
        
        ctr = current+1;
        while(ctr <= end) { 
            dlgWriteSpace();           
            dlgWritePage(ctr);
            dlgWriteSpace();
            ctr++;
        }
        
        if(writeLast) {
            dlgWriteSpace();
            dlgWritePage('Last', all);
            dlgWriteSpace();
        }        
    }
    

}
