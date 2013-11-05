  var minOrderQty = Number(theForm.MinOrderQuantity_%SKU%.value);
  
  if (theForm.Quantity_%SKU%.value == "")
  {
    alert("Please enter a quantity in the \"Quantity\" field. To remove an item from the cart, set its quantity to 0.");
    theForm.Quantity_%SKU%.focus();    
    return (false);
  }

  if (theForm.Quantity_%SKU%.value < 0)
  {
    alert("Only numbers 0 or higher are allowed in the \"Quantity\" field. To remove an item from the cart, set its quantity to 0.");
    if (minOrderQty > 1)
    {
        theForm.Quantity_%SKU%.value = minOrderQty;
    }
    else
    {
        theForm.Quantity_%SKU%.value = 1;
    }
    theForm.Quantity_%SKU%.focus();    
    return (false);
  }
  
  if (minOrderQty > 0)
  {
      if ((Number(theForm.Quantity_%SKU%.value) < minOrderQty) && (Number(theForm.Quantity_%SKU%.value)) > 0)
      {
            alert("The quantity you ordered is less than the min. order qty : " + minOrderQty);
            theForm.Quantity_%SKU%.value = minOrderQty;
            theForm.Quantity_%SKU%.focus();            
            return (false);
      }
  }

  if (theForm.Quantity_%SKU%.value > %MAX_QUANTITY_INPUT%)
  {
    alert("The maximum allowed quantity is %MAX_QUANTITY_INPUT%");
    theForm.Quantity_%SKU%.focus();    
    return (false);
  }

  var checkStr = theForm.Quantity_%SKU%.value;
  if (checkStr.length > 0 && checkStr.charAt(0) == '%DECIMAL_SEPARATOR%')  
  {
    checkStr = "%LOCALE_ZERO%" + checkStr;
  }

  if (!checkStr.match('%ALLOWED_QUANTITY_INPUT%')) {
    alert("Invalid input in the \"Quantity\" field.\nInput should only contain %DECIMAL_PLACE% digit decimal value.");
    theForm.Quantity_%SKU%.focus();    
     return false;
  }
  
	