USE [ContentServices_SourceForge]
GO

/****** Object:  View [dbo].[CombinedMediaList]    Script Date: 11/10/2013 07:50:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

drop view [dbo].[CombinedMediaList]
GO

CREATE VIEW [dbo].[CombinedMediaList]
AS

SELECT MediaId
      ,m.MediaGuid
      ,m.ExternalIdentifier
      ,m.SourceCode
	  ,m.DomainName
      ,m.OwningBusinessUnitId
	  ,obu.Name as OwningBusinessUnitName
      ,m.MaintainingBusinessUnitId
	  ,mbu.Name as MaintainingBusinessUnitName
      ,m.LanguageCode
      ,m.MediaTypeCode
      ,m.MimeTypeCode
      ,m.CharacterEncodingCode
      ,m.Title
      ,m.SubTitle
      ,m.Description
      ,m.SourceUrl
      ,m.TargetUrl
      ,m.AlternateText
      ,m.NoScriptText
      ,m.FeaturedText
      ,m.Author
      ,m.Length
      ,m.Size
      ,m.Height
      ,m.Width
      ,m.RatingOverride
      ,m.RatingMinimum
      ,m.RatingMaximum
      ,m.Comments
      ,m.Thumbnail
      ,m.RowVersion
      ,m.EmbedParameters
      ,mt.EmbedParameters as MediaTypeEmbedParameters
      ,m.LastReviewedDateTime
      ,m.PublishedDateTime
      ,m.PersistentURLToken
      ,m.MediaStatusCode
      ,(CASE 
			WHEN m.MediaStatusCode = 'Published' and PublishedDateTime <= getutcdate() THEN 'Published'
			WHEN m.MediaStatusCode = 'Published' and PublishedDateTime > getutcdate() THEN 'Pending'
			ELSE	m.MediaStatusCode
			END) as EffectiveStatus      
      ,m.Active
      ,sa.AttributionText
      ,mrc.ChildCount
      ,mt.Display as DisplayOnSearch
      ,m.CreatedByGuid
      ,m.CreatedDateTime
      ,m.ModifiedByGuid
      ,m.ModifiedDateTime      
  FROM Media.Media m
  left outer join Media.SourceAttribution sa 
	on m.SourceCode = sa.SourceCode and m.LanguageCode = sa.LanguageCode
  left outer join Media.MediaTypes mt
    on m.MediaTypeCode = mt.MediaTypeCode
  left outer join Admin.BusinessUnit obu 
    on m.OwningBusinessUnitId = obu.BusinessUnitId
  left outer join Admin.BusinessUnit mbu 
    on m.MaintainingBusinessUnitId = mbu.BusinessUnitId
  CROSS APPLY
	(select count(*) as ChildCount from Media.MediaRelationship mr where MediaId = m.MediaId and RelationshipTypeName = 'Is Parent Of') mrc	
  


GO


