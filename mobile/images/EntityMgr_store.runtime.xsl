<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ise="urn:ise" exclude-result-prefixes="ise">
  <xsl:output method="xml" standalone="yes" omit-xml-declaration="yes" indent="no" />
  <xsl:key name="parentID" match="Entity" use="ParentEntityID" />
  <xsl:template match="root">
    <root>
      <xsl:for-each select="EntityMgr">
        <xsl:call-template name="entityTmpl">
          <xsl:with-param name="psectionid" select="0">
          </xsl:with-param>
        </xsl:call-template>
      </xsl:for-each>
    </root>
  </xsl:template>
  <xsl:template name="entityTmpl">
    <xsl:param name="psectionid">
    </xsl:param>
    <xsl:for-each select="key('parentID', $psectionid)">
      <xsl:sort select="SortOrder" data-type="number" />
      <xsl:sort select="Name" />
      <xsl:copy>
        <xsl:copy-of select="EntityID" />
        <xsl:copy-of select="Name" />
        <xsl:copy-of select="ColWidth" />
        <Description>
          <xsl:call-template name="GetMLField">
            <xsl:with-param name="EntityName" select="/root/Runtime/EntityName" />
            <xsl:with-param name="EntityID" select="EntityID" />
            <xsl:with-param name="Description" select="Description" />
          </xsl:call-template>
        </Description>
        <xsl:copy-of select="WebDescription" />
        <xsl:copy-of select="SEKeywords" />
        <xsl:copy-of select="SEDescription" />
        <xsl:copy-of select="SETitle" />
        <xsl:copy-of select="SENoScript" />
        <xsl:copy-of select="SEAltText" />
        <xsl:copy-of select="ParentEntityID" />
        <xsl:copy-of select="SortOrder" />
        <xsl:copy-of select="SortByLooks" />
        <xsl:copy-of select="XmlPackage" />
        <xsl:copy-of select="MobileXmlPackage" />
        <xsl:copy-of select="Published" />
        <xsl:copy-of select="SEName" />
        <xsl:copy-of select="PageSize" />
        <xsl:copy-of select="TemplateName" />
        <xsl:call-template name="entityTmpl">
          <xsl:with-param name="psectionid" select="EntityID">
          </xsl:with-param>
        </xsl:call-template>
      </xsl:copy>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="GetMLField">
    <xsl:param name="EntityName" />
    <xsl:param name="EntityID" />
    <xsl:param name="Description" />
    <xsl:variable name="entityname_upper" select="ise:ToUpper($EntityName)" />
    <xsl:choose>
      <xsl:when test="$entityname_upper = 'CATEGORY'">
        <ml>
          <xsl:for-each select="/root/CategoryDescriptions/CategoryDescription[ID = $EntityID]">
            <locale>
              <xsl:attribute name="name">
                <xsl:value-of select="ise:CheckLocaleSettingForProperCase(Locale)" disable-output-escaping="yes" />
              </xsl:attribute>
              <xsl:value-of select="ise:XmlEncode(MLField)" disable-output-escaping="yes" />
            </locale>
          </xsl:for-each>
        </ml>
      </xsl:when>
      <xsl:when test="$entityname_upper = 'MANUFACTURER'">
        <ml>
          <xsl:for-each select="/root/ManufacturerDescriptions/ManufacturerDescription[ID = $EntityID]">
            <locale>
              <xsl:attribute name="name">
                <xsl:value-of select="ise:CheckLocaleSettingForProperCase(Locale)" disable-output-escaping="yes" />
              </xsl:attribute>
              <xsl:value-of select="ise:XmlEncode(MLField)" disable-output-escaping="yes" />
            </locale>
          </xsl:for-each>
        </ml>
      </xsl:when>
      <xsl:when test="$entityname_upper = 'DEPARTMENT'">
        <ml>
          <xsl:for-each select="/root/DepartmentDescriptions/DepartmentDescription[ID = $EntityID]">
            <locale>
              <xsl:attribute name="name">
                <xsl:value-of select="ise:CheckLocaleSettingForProperCase(Locale)" disable-output-escaping="yes" />
              </xsl:attribute>
              <xsl:value-of select="ise:XmlEncode(MLField)" disable-output-escaping="yes" />
            </locale>
          </xsl:for-each>
        </ml>
      </xsl:when>
      <xsl:when test="$entityname_upper = 'ATTRIBUTE'">
        <ml>
          <xsl:for-each select="/root/AttributeDescriptions/AttributeDescription[ID = $EntityID]">
            <locale>
              <xsl:attribute name="name">
                <xsl:value-of select="ise:CheckLocaleSettingForProperCase(Locale)" disable-output-escaping="yes" />
              </xsl:attribute>
              <xsl:value-of select="ise:XmlEncode(MLField)" disable-output-escaping="yes" />
            </locale>
          </xsl:for-each>
        </ml>
      </xsl:when>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
