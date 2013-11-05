<?xml version="1.0" encoding="UTF-8" ?>
<!-- ###################################################################################################### -->
<!-- Licensed by Interprise Solutions.					                -->
<!-- http://www.InterpriseSolutions.com														                -->
<!-- For details on this license please visit  the product homepage at the URL above.		                -->
<!-- THE ABOVE NOTICE MUST REMAIN INTACT.                                                                   -->
<!-- ###################################################################################################### -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ise="urn:ise" exclude-result-prefixes="ise">
  <xsl:output method="html" standalone="yes"/>

  <xsl:param name="entity"></xsl:param>
  <xsl:param name="ForParentEntityID"></xsl:param>
  <xsl:param name="IncludeLinks"></xsl:param>
  <xsl:param name="AffiliateID"></xsl:param>
  <xsl:param name="IncludeObjects"></xsl:param>
  <xsl:param name="CssClassName"></xsl:param>
  <xsl:param name="RecurseChildren"></xsl:param>
  <xsl:param name="OnlyExpandForThisChildID"></xsl:param>
  <xsl:param name="Prefix"></xsl:param>
  <xsl:param name="adminsite"></xsl:param>
  <xsl:param name="custlocale"></xsl:param>
  <xsl:param name="deflocale"></xsl:param>
  <xsl:param name="objName"></xsl:param>

  <xsl:key name="objectEntity" match="object" use="EntityID"/>

  <xsl:template match="root">
    <xsl:param name="prefix">
      <xsl:choose>
        <xsl:when test="$entity='Category'">
          <xsl:value-of select="'c'" />
        </xsl:when>
        <xsl:when test="$entity='Department'">
          <xsl:value-of select="'d'" />
        </xsl:when>
        <xsl:when test="$entity='Manufacturer'">
          <xsl:value-of select="'m'" />
        </xsl:when>
        <xsl:when test="$entity='Library'">
          <xsl:value-of select="'l'" />
        </xsl:when>
      </xsl:choose>
    </xsl:param>
    <ul>
      <xsl:attribute name="class">
        <xsl:value-of select="$CssClassName" />
      </xsl:attribute>
      <xsl:choose>
        <xsl:when test="$ForParentEntityID='0' or $ForParentEntityID=''">
          <xsl:apply-templates select="Entity">
            <xsl:with-param name="prefix" select="$prefix"/>
          </xsl:apply-templates>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="descendant-or-self::Entity[EntityID=$ForParentEntityID]" >
            <xsl:with-param name="prefix" select="$prefix"/>
            <xsl:with-param name="indent" select="$Prefix"/>
          </xsl:apply-templates>
        </xsl:otherwise>
      </xsl:choose>
    </ul>
    <br />
  </xsl:template>

  <xsl:template match="Entity">
    <xsl:param name="prefix"></xsl:param>
    <xsl:param name="indent"></xsl:param>
    <xsl:param name="eName">
      <xsl:choose>
        <xsl:when test="string-length(Description)>0">
          <xsl:value-of select="ise:GetMLValue(Description)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="Name"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:param>

    <xsl:variable name="containsChild" select="boolean(descendant-or-self::Entity[./EntityID=$OnlyExpandForThisChildID])"></xsl:variable>

    <li>
      <xsl:choose>
        <xsl:when test="boolean($IncludeLinks)">
          <a href="{concat($prefix, '-', EntityID, '-', ise:FormatStringForLink($eName), '.aspx')}" class="kitdetaillink" >
            <xsl:if test="./ParentEntityID!=0">
              <xsl:value-of select="$indent"  disable-output-escaping="yes"/>
            </xsl:if>
            <xsl:choose>
              <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)">
                <b>
                  &#187;<xsl:value-of select="$eName" />
                </b>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="$eName"/>
              </xsl:otherwise>
            </xsl:choose>
          </a>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="./ParentEntityID!=0">
            <xsl:value-of select="$indent"  disable-output-escaping="yes"/>
          </xsl:if>
          <xsl:choose>
            <xsl:when test="boolean($OnlyExpandForThisChildID) and boolean($containsChild)">
              <b>
                &#187;<xsl:value-of select="$eName" />
              </b>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$eName"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:if test="boolean($IncludeObjects) and count(key('objectEntity', EntityID))!=0">
        <ul class="{$CssClassName}">
          <xsl:call-template name="obj">
            <xsl:with-param name="prefix" select="$prefix"/>
            <xsl:with-param name="objEID" select="EntityID"/>
          </xsl:call-template>
        </ul>
      </xsl:if>

      <xsl:if test="boolean($RecurseChildren) or boolean($containsChild)">
        <ul class="{$CssClassName}">
          <xsl:apply-templates select="Entity">
            <xsl:with-param name="prefix" select="$prefix"/>
            <xsl:with-param name="indent" select="concat($indent, $Prefix)"/>
          </xsl:apply-templates>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template name="obj">
    <xsl:param name="prefix"></xsl:param>
    <xsl:param name="objEID"></xsl:param>

    <xsl:for-each select="key('objectEntity', $objEID)">
      <xsl:variable name="eName">
        <xsl:choose>
          <xsl:when test="string-length(ItemDescription)>0">
            <xsl:value-of select="ItemDescription"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="ItemName"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <li>
        <a href="{concat('p', '-', ObjectID, '-', ise:FormatStringForLink($eName), '.aspx')}" class="kitdetaillink">
          <xsl:value-of select="$eName"/>
        </a>
      </li>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>
