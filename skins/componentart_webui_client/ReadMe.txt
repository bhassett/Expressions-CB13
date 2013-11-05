
In order to minimize the amount of content that Web.UI controls generate, we recommend 
that you copy the folder containing the client script files to a central location on your 
web server. 

If this is done, all Web.UI controls will generate the appropriate script include tags (instead 
of the entire script content) thus reducing the overall amount of content sent over the network. 


Usage of ComponentArt Web.UI Client Script Files: 
-------------------------------------------------

You have two options for using the Web.UI Client Script Files: 

1. Copy the componentart_webui_client folder into the root of your EcommerceSite or web application. 
All Web.UI controls will be able to find and use these scripts without any additional settings. 

or 


2. If you want to place the componentart_webui_client folder into another location, 
you can point to it by setting the ClientScriptLocation property in your web.config file: 

<appSettings>
  <add key="ComponentArt.Web.UI.ClientScriptLocation" value="~/MyClientScripts" />
</appSettings> 




