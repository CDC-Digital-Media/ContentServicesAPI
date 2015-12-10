USE [ContentServices_SourceForge]
GO

/****** Object:  UserDefinedFunction [dbo].[GetRelatedValues]    Script Date: 11/10/2013 07:56:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetRelatedValues]
      (@RelationshipTypeName   NVARCHAR(100),
       @StartingRelatedValueId INT)
RETURNS @RtnValue TABLE (
  ValueId              INT,
  RelationshipTypeName NVARCHAR(100),
  RelatedValueId       INT,
  Lvl                  INT,
  Validity             NVARCHAR(10))
AS
  BEGIN
      WITH ReturnAllRelated
           AS (SELECT VR.RelationshipTypeName,
                      VR.ValueId,
                      VR.RelatedValueID,
                      VR.ValueLanguageCode,
                      1       AS Lvl,
                      'Valid' AS Validity
                 FROM Vocabulary.ValueRelationship AS VR
                WHERE VR.RelationshipTypeName = @RelationshipTypeName
                  AND VR.RelatedValueID = @StartingRelatedValueId
               UNION ALL
               SELECT VR2.RelationshipTypeName,
                      VR2.ValueID,
                      VR2.RelatedValueID,
                      VR2.RelatedValueLanguageCode,
                      Lvl + 1,
                      --            CASE VR2.ValueID 
                      --WHEN @StartingRelatedValueId THEN 'Loop'
                      --	ELSE 'Valid'
                      --END AS Validity	
                      CASE Lvl
                        WHEN 50 THEN 'Loop'
                        ELSE 'Valid'
                      END AS Validity
                 FROM Vocabulary.ValueRelationship AS VR2
                      INNER JOIN ReturnAllRelated AS R
                              ON VR2.RelatedValueID = R.ValueID
                             AND VR2.ValueLanguageCode = R.ValueLanguageCode
                             AND (VR2.Active = 'Yes'
                                   OR VR2.Active = 'Unc')
                WHERE VR2.RelationshipTypeName = @RelationshipTypeName
                  AND R.Validity = 'Valid')
      INSERT INTO @RtnValue
                  (ValueId,
                   RelationshipTypeName,
                   RelatedValueId,
                   Lvl,
                   Validity)
      SELECT RAR.ValueID,
             RAR.RelationshipTypeName,
             RAR.RelatedValueID,
             RAR.Lvl,
             RAR.Validity
        FROM ReturnAllRelated AS RAR;

      RETURN
  END 
GO


