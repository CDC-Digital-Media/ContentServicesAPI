USE [ContentServices_SourceForge]
GO
/****** Object:  StoredProcedure [dbo].[HasRecursiveLoopMediaRelationship]    Script Date: 11/10/2013 07:54:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
	and specific_name = 'HasRecursiveLoopMediaRelationship')
	drop proc HasRecursiveLoopMediaRelationship
GO

CREATE PROCEDURE [dbo].HasRecursiveLoopMediaRelationship
			@p_MediaId int, 
			@p_RelationshipTypeName nvarchar(100)
AS
BEGIN

      IF EXISTS (SELECT 1
                   FROM GETRELATEDMEDIAS(@p_RelationshipTypeName, @p_MediaId)
                  WHERE Validity = 'Loop')
        select 'Yes' as HasLoop
      ELSE
        select 'No' as HasLoop

END
GO
grant execute on HasRecursiveLoopMediaRelationship to ContentServicesApplication_API
GO


