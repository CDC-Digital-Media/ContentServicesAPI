USE [ContentServices_SourceForge]
GO
/****** Object:  StoredProcedure [dbo].[HasInheritedMediaRelationship]    Script Date: 11/10/2013 07:54:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
	and specific_name = 'HasInheritedMediaRelationship')
	drop proc HasInheritedMediaRelationship
GO

CREATE PROCEDURE [dbo].[HasInheritedMediaRelationship] 
			@p_MediaId int, 
			@p_RelationshipTypeName nvarchar(100), 
			@p_RelatedMediaId int
AS
BEGIN
         SELECT CASE
                  WHEN MIN(MediaID) IS NOT NULL THEN 'Yes'
                  ELSE 'No'
                END AS HasRelationship
           FROM dbo.[GetRelatedMedias](@p_RelationshipTypeName, @p_RelatedMediaId)
          WHERE MediaId = @p_MediaId

END
GO
grant execute on HasInheritedMediaRelationship to ContentServicesApplication_API
GO


