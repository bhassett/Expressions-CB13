************************************  SQL Statement and parameters for query EntityMgr  ************************************

declare @EntityName VarChar
declare @PublishedOnly TinyInt
declare @locale VarChar
declare @WebSiteCode VarChar
declare @CurrentDate DateTime

set @EntityName = 'Attribute'
set @PublishedOnly = 1
set @locale = 'en-US'
set @WebSiteCode = 'WEB-000001'
set @CurrentDate = '12/3/2013 12:00:00 AM'


exec eCommerceEntityMgr @EntityName, @PublishedOnly, @locale, @WebSiteCode, @CurrentDate

************************************  SQL Statement and parameters for query AttributeDescriptions  ************************************




SELECT	c.Counter AS ID, 
            a.SourceFilterName AS Code, 
            sl.ShortString AS Locale, 
            a.SourceFilterName AS MLField
            FROM SystemItemAttributeSourceFilterValueDescription a with (NOLOCK)					
            INNER JOIN SystemItemAttributeSourceFilterCode b with (NOLOCK) ON a.AttributeSourceFilterCode = b.AttributeSourceFilterCode AND b.IsActive = 1 AND a.IsActive = 1
            INNER JOIN SystemItemAttributeSourceFilterValue c with (NOLOCK) ON a.AttributeSourceFilterCode = c.AttributeSourceFilterCode AND c.SourceFilterName = a.SourceFilterName
            INNER JOIN SystemLanguage sl with (NOLOCK) ON a.LanguageCode = sl.LanguageCode
            INNER JOIN SystemSellingLanguage ssl with (NOLOCK) ON ssl.Languagecode = sl.LanguageCode AND ssl.IsIncluded = 1


