USE [ContentServices_SourceForge]
GO
/****** Object:  StoredProcedure [dbo].[HasInheritedValueRelationship]    Script Date: 11/10/2013 07:54:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
	and specific_name = 'HasInheritedValueRelationship')
	drop proc HasInheritedValueRelationship
GO

CREATE PROCEDURE [dbo].[HasInheritedValueRelationship] 
			@p_ValueId int, 
			@p_RelationshipTypeName nvarchar(100), 
			@p_RelatedValueId int
AS
BEGIN
		SELECT CASE
                  WHEN MIN(ValueID) IS NOT NULL THEN 'Yes'
                  ELSE 'No'
                END AS HasRelationship
           FROM dbo.GETRELATEDVALUES(@p_RelationshipTypeName, @p_RelatedValueId)
          WHERE ValueId = @p_ValueId

END
GO
grant execute on HasInheritedValueRelationship to ContentServicesApplication_API
GO


