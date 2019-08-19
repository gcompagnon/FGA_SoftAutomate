<xsl:stylesheet version="1.0"
    xmlns="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
 xmlns:user="urn:my-scripts"
 xmlns:o="urn:schemas-microsoft-com:office:office"
 xmlns:x="urn:schemas-microsoft-com:office:excel"
 xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" > 
 
<xsl:template match="/">
  <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:x="urn:schemas-microsoft-com:office:excel"
    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:html="http://www.w3.org/TR/REC-html40">
    <xsl:apply-templates/>
  </Workbook>
</xsl:template>


<xsl:template match="/*">
  <Worksheet>
  <xsl:attribute name="ss:Name">
  <xsl:value-of select="local-name(/*/*)"/>
  </xsl:attribute>
    <Table x:FullColumns="1" x:FullRows="1">
      <Row>
        <xsl:for-each select="*[position() = 1]/*">
          <Cell><Data ss:Type="String">
          <xsl:value-of select="local-name()"/>
          </Data></Cell>
        </xsl:for-each>
      </Row>
      <xsl:apply-templates/>
    </Table>
  </Worksheet>
</xsl:template>


<xsl:template match="/*/*">
  <Row>
    <xsl:apply-templates/>
  </Row>
</xsl:template>
    
  <xsl:template match="/*/*/*" priority="0">
    <Cell><Data ss:Type="String"><xsl:value-of select="."/></Data></Cell>
  </xsl:template>
  
  <xsl:template match="/*/*/PV1|/*/*/PV2|/*/*/TradeID|/*/*/C12_Valeur_Boursiere|/*/*/C13_Coupon_Couru|/*/*/C14_Valeur_Comptable|/*/*/C15_Coupon_Couru_Comptable|/*/*/C16_PMV|/*/*/C17_PMV_CC|/*/*/C29_duration|/*/*/C30_Cours_Close|/*/*/C31_QuantiteNominal" priority="1">
    <Cell><Data ss:Type="Number"><xsl:value-of select="." /></Data></Cell>
  </xsl:template>

  <xsl:template match="/*/*/C05_VL|/*/*/C06_ActifNet|/*/*/C08_nbParts" priority="1">
    <Cell>
      <Data ss:Type="Number">
        <xsl:value-of select="." />
      </Data>
    </Cell>
  </xsl:template>
  <xsl:template match="/*/*/C02_nbNourriciers|/*/*/B04_ActifNetCalculOmega|/*/*/C03_ActifNetTotalNourriciers|/*/*/B05_nbpartsMaitre|/*/*/C04_nbpartsNourriciers" priority="1">
    <Cell>
      <Data ss:Type="Number">
        <xsl:value-of select="." />
      </Data>
    </Cell>
  </xsl:template>

  <xsl:template match="/*/*/VB_CC|/*/*/VL|/*/*/ActifNet|/*/*/nbParts|/*/*/Valeur_Boursiere|/*/*/Coupon_Couru|/*/*/Valeur_Comptable|/*/*/Coupon_Couru_Comptable|/*/*/PMV|/*/*/PMV_CC|/*/*/duration|/*/*/Cours_Close|/*/*/QuantiteNominal" priority="1">
    <Cell>
      <Data ss:Type="Number">
        <xsl:value-of select="." />
      </Data>
    </Cell>
  </xsl:template>

</xsl:stylesheet>
