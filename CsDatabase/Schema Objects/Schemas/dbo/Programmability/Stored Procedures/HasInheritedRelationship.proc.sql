USE [ContentServices_SourceForge]
GO
/****** Object:  StoredProcedure [dbo].[HasInheritedRelationship]    Script Date: 11/10/2013 07:54:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[HasInheritedRelationship] 
AS
BEGIN
		declare
			@p_ValueId int, 
			@p_RelationshipTypeName nvarchar(100), 
			@p_RelatedValueId int

		if exists (select 1 from dbo.GetRelatedValues(@p_RelationshipTypeName, @p_RelatedValueId)
			where ValueId = @p_ValueId)
				select 'Yes' as HasRelationship
	 	else
	 		select 'No' as HasRelationship

END

