var loadDelegate = function(){
    
    LoadManager.loadModel(
        OpenSalesOrderRequest, 
        false,
        null
    );
    
    LoadManager.loadModel(
        ActiveCustomerRequest, 
        false,
        null
    );
    
    LoadManager.loadModel(
        ElectronicDownloadItemRequest, 
        false,
        null
    );
}

$add_windowLoad(loadDelegate);
