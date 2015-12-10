USE [ContentServices_SourceForge]
GO

/****** Object:  StoredProcedure [dbo].[GetMediaRelationships]    Script Date: 11/10/2013 07:55:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
	and specific_name = 'GetMediaRelationships')
	drop proc GetMediaRelationships
GO

-- =============================================
-- Author:		Tim Carroll
-- Create date: 10/21/2013
-- Description:	Get parent and child media relationships
-- =============================================
CREATE PROCEDURE [dbo].[GetMediaRelationships]
		@p_MediaId int,
		@p_ToChildLevel int, 
		@p_ToParentLevel int
		

AS
BEGIN
		
	declare @NegParentLevel int
	select @NegParentLevel = @p_ToParentLevel * -1

		;With ChildrenCTE AS(
			SELECT  m.MediaId,
					cast(null as nvarchar(100)) as RelationshipTypeName,
					cast(null as int) as RelatedMediaId, 
					0 as Lvl
				FROM dbo.CombinedMediaList AS m
					where m.MediaId = @p_MediaId
			UNION ALL
				select	mr.RelatedMediaId as MediaId, 
						mr.RelationshipTypeName,
						Parent.MediaId as RelatedMediaId, 
						Parent.Lvl + 1 As Lvl
				--select m.MediaId, m.Title, M.LanguageCode, Parent.RelatedMediaId, Parent.ParentLanguageCode 
					FROM Media.MediaRelationship mr
					INNER JOIN ChildrenCTE AS Parent
						ON mr.MediaId = Parent.MediaId
				where mr.Active = 'Yes' and Parent.Lvl < @p_ToChildLevel and mr.RelationshipTypeName = 'Is Parent Of'
				),
			ParentCTE AS(
			SELECT  m.MediaId,
					cast(null as nvarchar(100)) as RelationshipTypeName,
					cast(null as int) as RelatedMediaId, 
					0 as Lvl
				FROM dbo.CombinedMediaList AS m
					where m.MediaId = @p_MediaId
			UNION ALL
				select	mr.MediaId as MediaId, 
						mr.RelationshipTypeName,
						Parent.MediaId as RelatedMediaId, 
						Parent.Lvl - 1 As Lvl
				--select m.MediaId, m.Title, M.LanguageCode, Parent.RelatedMediaId, Parent.ParentLanguageCode 
					FROM Media.MediaRelationship mr
					INNER JOIN ParentCTE AS Parent
						ON mr.RelatedMediaId = Parent.MediaId
				where mr.Active = 'Yes' and Parent.Lvl > @NegParentLevel and mr.RelationshipTypeName = 'Is Parent Of'
				),
				AllCTE AS(
				select * from ChildrenCTE
				UNION
				select * from ParentCTE
				)
	select	v.MediaId, 
			v.RelationshipTypeName, 
			v.RelatedMediaId, 
			v.Lvl, 
			m.LanguageCode, 
			m.MediaTypeCode,
			m.MimeTypeCode,
			m.Title, 
			m.Description, 
			m.SourceUrl,
			m.TargetUrl,
			m.EffectiveStatus,
			m.DisplayOnSearch,
			m.PublishedDateTime,
			m.CreatedDateTime,
			m.ModifiedDateTime,
			m.Active  
		from AllCTE v
		inner join dbo.CombinedMediaList m
			on v.MediaId = m.MediaId
		order by m.Title
			
	--select * from media.media m
	--inner join media.MediaRelationship mr on m.MediaId = mr.RelatedMediaid
	--inner join media.MediaRelationship mr2 on m.MediaId = mr2.Mediaid
	--where MediaTypeCode like 'podcast%'

END

GO

grant execute on GetMediaRelationships to ContentServicesApplication_API
GO
