USE [ContentServices_SourceForge]
GO

/****** Object:  UserDefinedFunction [dbo].[HasRecursiveLoopRelationship]    Script Date: 11/10/2013 07:58:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[HasRecursiveLoopRelationship]
      (@p_ValueId              INT,
       @p_RelationshipTypeName NVARCHAR(100))
RETURNS VARCHAR(10)
AS
  BEGIN
      DECLARE @results VARCHAR(10)

      IF EXISTS (SELECT 1
                   FROM GETRELATEDVALUES(@p_RelationshipTypeName, @p_ValueId)
                  WHERE Validity = 'Loop')
        SET @results = 'Yes'
      ELSE
        SET @results = 'No'

      RETURN @results
  END 
GO


