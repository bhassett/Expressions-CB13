<%@ Page Language="C#" AutoEventWireup="true" CodeFile="category.aspx.cs" Inherits="InterpriseSuiteEcommerce.category" %>
<%@ Register TagPrefix="ise" TagName="XmlPackage" src="XmlPackageControl.ascx" %>

<ise:XmlPackage id="Package1" 
    PackageName="page.categories.xml.config" 
    runat="server" 
    EnforceDisclaimer="true" 
    EnforcePassword="true" 
    EnforceSubscription="True" 
    AllowSEPropogation="True"
/>