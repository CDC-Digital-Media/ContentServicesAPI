USE [ContentServices_SourceForge]
GO

/****** Object:  UserDefinedFunction [dbo].[HasInheritedRelationship_Alt]    Script Date: 11/10/2013 07:57:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[HasInheritedRelationship_Alt]
      (@p_ValueId              INT,
       @p_RelationshipTypeName NVARCHAR(100),
       @p_RelatedValueId       INT)
RETURNS VARCHAR(10)
AS
  BEGIN
      RETURN
        (SELECT CASE
                  WHEN MIN(ValueID) IS NOT NULL THEN 'Yes'
                  ELSE 'No'
                END AS HasRelationship
           FROM dbo.GETRELATEDVALUES(@p_RelationshipTypeName, @p_RelatedValueId)
          WHERE ValueId = @p_ValueId)
  END 
GO


