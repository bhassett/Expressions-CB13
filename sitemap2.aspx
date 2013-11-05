<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Page Language="c#" Inherits="InterpriseSuiteEcommerce.sitemap2" CodeFile="sitemap2.aspx.cs" %>
<html>
<head>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <ComponentArt:SiteMap ID="SiteMap1" runat="server" LeafNodeCssClass="SiteMapLeafNode"
            ParentNodeCssClass="SiteMapParentNode" RootNodeCssClass="SiteMapRootNode" CssClass="SiteMap"
            TreeLineImageHeight="20" TreeLineImageWidth="19" TreeLineImagesFolderUrl="images/lines2"
            TreeShowLines="true" SiteMapLayout="Tree" Width="100%">
            <Table>
                <ComponentArt:SiteMapTableRow>
                    <ComponentArt:SiteMapTableCell RootNodes="1" valign="top" Width="25%"></ComponentArt:SiteMapTableCell>
                    <ComponentArt:SiteMapTableCell RootNodes="1" valign="top" Width="25%"></ComponentArt:SiteMapTableCell>
                    <ComponentArt:SiteMapTableCell RootNodes="1" valign="top" Width="25%"></ComponentArt:SiteMapTableCell>
                    <ComponentArt:SiteMapTableCell valign="top" Width="25%"></ComponentArt:SiteMapTableCell>
                </ComponentArt:SiteMapTableRow>
            </Table>
        </ComponentArt:SiteMap>
    </form>
</body>
</html>
