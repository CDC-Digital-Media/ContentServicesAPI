USE [ContentServices_SourceForge]
GO
/****** Object:  StoredProcedure [dbo].[HasRecursiveLoopValueRelationship]    Script Date: 11/10/2013 07:54:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
	and specific_name = 'HasRecursiveLoopValueRelationship')
	drop proc HasRecursiveLoopValueRelationship
GO

CREATE PROCEDURE [dbo].HasRecursiveLoopValueRelationship
			@p_ValueId int, 
			@p_RelationshipTypeName nvarchar(100)
AS
BEGIN


      IF EXISTS (SELECT 1
                   FROM GETRELATEDVALUES(@p_RelationshipTypeName, @p_ValueId)
                  WHERE Validity = 'Loop')
        select 'Yes' as HasLoop
      ELSE
        select 'No' as HasLoop

END
GO
grant execute on HasRecursiveLoopValueRelationship to ContentServicesApplication_API
GO
