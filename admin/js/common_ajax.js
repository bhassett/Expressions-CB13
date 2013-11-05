function $add_windowLoad(handler) {
    if (window.addEventListener) { 
        window.addEventListener('load',handler,false);
    }
    else if (document.addEventListener) {
        document.addEventListener('load',handler,false);
    }
    else if (window.attachEvent) { 
        window.attachEvent('onload',handler);
    }
    else {
        if (typeof window.onload=='function') {
            var oldload=window.onload;
            window.onload = function(){
                oldload();
                handler();
            }
        } 
        else { window.onload=init; }
    }
}


function $getElement(el) {
    //Sys.Debug.trace(typeof(el));
    return typeof(el) == 'string' ? $get(el) : el;
}


function $hasValue(el) {
    if(el.type) {
      switch(el.type.toLowerCase()) {
        case 'checkbox':          
        case 'radio':
          return el.checked;
          break;
        default:
          return el.value;
          break;
      }
    }
}

function $serializeForm(id) {
    var form = document.getElementById(id);
    var elements = form.getElementsByTagName('*');

    var inputs = new Array();

    for(var ctr=0; ctr<elements.length; ctr++) {
      var el = elements[ctr];
      if(el.name && $hasValue(el)) {                
        inputs.push(el.name + '=' + el.value);
      }
    }

    var serialized = '';
    for(var x=0; x<inputs.length; x++) {
        serialized += inputs[x];
        serialized += (x+1<inputs.length)?'&':'';
    }
    
    return serialized;
}

/*************************************/
var Settings = {
    closeImageSrc : 'images/close.gif',
    applyFilterImageSrc : 'images/apply.jpg',
    filterLoadingSrc : 'images/23-1.gif',
    modelLoadingSrc : 'images/ajax-loader.gif'
}

var OpenSalesOrderRequest = {
    control : '64FA5F72-0D6E-4B94-85BB-4D45F9EF420D',
    name : 'OpenSalesOrderForm',
    filterContainer : 'spanOpenSalesOrderFilter',
    modelContainer : 'divOpenSalesOrderModel'
}

var ActiveCustomerRequest = {
    control : '69A2ED40-48B1-41CC-AFD3-79709A3B4E67',
    name : 'ActiveCustomerForm',
    filterContainer : 'spanActiveCustomerFilter',
    modelContainer : 'divActiveCustomerModel'
}

var ElectronicDownloadItemRequest = {
    control : '2819D368-978B-4520-A765-510716C07AD5',
    name : 'ElectronicDownloadItemForm',
    filterContainer : 'spanElectronicDownloadItemFilter',
    modelContainer : 'divElectronicDownloadItemModel'
}

/*************************************/
var LoadManager = {

    loadModel: function(loadRequest, postBack, additionalParams) {
        var filterParams = 'cid=' + loadRequest.control + '&fpb=' + (postBack ? 'true' : 'false') + (additionalParams ? '&' + additionalParams : '');

        var modelDiv = $getElement(loadRequest.name + '_model');
        if (modelDiv === undefined || modelDiv === null) {
            modelDiv = document.createElement('div');
            modelDiv.id = loadRequest.name + '_model'
            $getElement(loadRequest.modelContainer).appendChild(modelDiv);
        }
        else {
            modelDiv.innerHTML = '';
        }

        modelDiv = $getElement(modelDiv);

        var modelIndicator = $getElement(loadRequest.name + '_modelIndicator');
        if (modelIndicator === undefined || modelIndicator === null) {
            modelIndicator = document.createElement('div');
            modelIndicator.id = loadRequest.name + '_modelIndicator';
            modelIndicator.align = 'center';

            var imgIndicator = document.createElement('img');
            imgIndicator.src = Settings.modelLoadingSrc;
            modelIndicator.appendChild(imgIndicator);

            $getElement(loadRequest.modelContainer).appendChild(modelIndicator);
        }

        // assert
        modelIndicator = $getElement(modelIndicator);

        if (!modelIndicator.style.display == '') {
            modelIndicator.style.display = '';
        }

        var onCompleted = function(executor, eventArgs) {
            modelIndicator.style.display = 'none';
            //  alert(executor.get_responseData());
            modelDiv.innerHTML = executor.get_responseData();
        }

        var host = new Sys.Net.WebRequest();
        host.set_url('host.aspx');
        host.set_httpVerb('POST');
        host.set_body(filterParams);
        host.add_completed(onCompleted);
        host.invoke();

    },


    loadFilter: function(loadRequest) {
        // check to see if the popup has been loaded before
        var filterPopup = $getElement(loadRequest.name + '_popup');

        if (!(filterPopup === undefined || filterPopup === null)) {
            $getElement(filterPopup).style.display = '';
            return;
        }

        var container = $getElement(loadRequest.filterContainer);
        if (container === undefined || container === null) return;

        var params = 'cid=' + loadRequest.control + '&form=' + loadRequest.name + '&hostOnForm=true&filter=true';

        // show the indicator
        var filterIndicator = $getElement(loadRequest.name + '_filterIndicator');
        if (filterIndicator === undefined || filterIndicator === null) {
            filterIndicator = document.createElement('span');
            filterIndicator.id = loadRequest.name + '_filterIndicator';

            var imgIndicator = document.createElement('img');
            imgIndicator.src = Settings.filterLoadingSrc;
            filterIndicator.appendChild(imgIndicator);

            container.appendChild(filterIndicator);
        }

        // assert
        filterIndicator = $getElement(filterIndicator);

        if (!filterIndicator.style.display == '') {
            filterIndicator.style.display = ''
        }

        var onCompleted = function(executor, eventArgs) {
            filterPopup = document.createElement('div');
            filterPopup.id = loadRequest.name + '_popup';
            filterPopup.innerHTML = executor.get_responseData();
            container.appendChild(filterPopup);

            var hideFilterPopUpRoutine = function() { $getElement(filterPopup).style.display = 'none'; };


            var btnContainer = document.createElement('div');
            btnContainer.align = 'left';

            var btnClose = document.createElement('input');
            btnClose.type = 'button';
            btnClose.value = 'close';
            btnClose.onclick = hideFilterPopUpRoutine;

            var btnApply = document.createElement('input');
            btnApply.type = 'button';
            btnApply.value = 'apply';

            btnApply.onclick = function() {
                hideFilterPopUpRoutine();
                var filterParams = $serializeForm(loadRequest.name);
                LoadManager.loadModel(loadRequest, true, filterParams);
            }

            // added closing actions 
            // redirect to Default.aspx
            btnClose.onclick = function() {
                hideFilterPopUpRoutine();
                location.href = 'Default.aspx';
            }

            btnContainer.appendChild(btnApply);
            btnContainer.appendChild(btnClose);

            filterPopup.appendChild(btnContainer);

            $getElement(btnContainer).className = 'buttonContainer';
            $getElement(btnApply).className = 'btn';
            $getElement(btnClose).className = 'btn';
            $getElement(btnClose).id = "btnClose";
            $getElement(btnApply).id = "btnApply";

            $getElement(filterPopup).className = 'popupShow';

            filterIndicator.style.display = 'none';

            //move the filter buttons
            var formFooter = container.getElementsByClassName('form-footer')[0];
            if(formFooter != null) {
                formFooter.appendChild(btnContainer);
            }
        }

        var host = new Sys.Net.WebRequest();
        host.set_url('host.aspx');
        host.set_httpVerb('POST');
        host.set_body(params);
        host.add_completed(onCompleted);
        host.invoke();
    }
}

