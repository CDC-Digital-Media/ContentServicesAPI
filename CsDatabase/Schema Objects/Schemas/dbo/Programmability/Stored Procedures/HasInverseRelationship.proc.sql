USE [ContentServices_SourceForge]
GO

/****** Object:  StoredProcedure [dbo].[HasInverseRelationship]    Script Date: 11/10/2013 07:56:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[HasInverseRelationship]
AS
  BEGIN
      DECLARE @p_ValueId              INT,
              @p_RelationshipTypeName NVARCHAR(100),
              @p_RelatedValueId       INT

      IF EXISTS (SELECT 1
                   FROM dbo.GETRELATEDVALUES(@p_RelationshipTypeName, @p_RelatedValueId)
                  WHERE ValueId = @p_ValueId)
        SELECT 'Yes' AS HasRelationship
      ELSE
        SELECT 'No' AS HasRelationship
  END 

GO


