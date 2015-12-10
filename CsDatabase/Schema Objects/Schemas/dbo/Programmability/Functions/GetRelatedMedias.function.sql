USE [ContentServices_SourceForge]
GO

/****** Object:  UserDefinedFunction [dbo].[GetRelatedMedias]    Script Date: 03/19/2014 18:53:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetRelatedMedias]
      (@RelationshipTypeName   NVARCHAR(100),
       @StartingRelatedMediaId INT)
RETURNS @RtnValue TABLE (
  MediaId              INT,
  RelationshipTypeName NVARCHAR(100),
  RelatedMediaId       INT,
  Lvl                  INT,
  Validity             NVARCHAR(10))
AS
  BEGIN
      WITH ReturnAllRelated
           AS (SELECT VR.RelationshipTypeName,
                      VR.MediaId,
                      VR.RelatedMediaId,
                      1       AS Lvl,
                      'Valid' AS Validity
                 FROM Media.MediaRelationship AS VR
                WHERE VR.RelationshipTypeName = @RelationshipTypeName
                  AND VR.RelatedMediaId = @StartingRelatedMediaId
               UNION ALL
               SELECT VR2.RelationshipTypeName,
                      VR2.MediaId,
                      VR2.RelatedMediaId,
                      Lvl + 1,
                      --            CASE VR2.MediaId 
                      --WHEN @StartingRelatedMediaId THEN 'Loop'
                      --	ELSE 'Valid'
                      --END AS Validity	
                      CASE Lvl
                        WHEN 50 THEN 'Loop'
                        ELSE 'Valid'
                      END AS Validity
                 FROM Media.MediaRelationship AS VR2
                      INNER JOIN ReturnAllRelated AS R
                              ON VR2.RelatedMediaId = R.MediaId
                             AND (VR2.Active = 'Yes'
                                   OR VR2.Active = 'Unc')
                WHERE VR2.RelationshipTypeName = @RelationshipTypeName
                  AND R.Validity = 'Valid')
      INSERT INTO @RtnValue
                  (MediaId,
                   RelationshipTypeName,
                   RelatedMediaId,
                   Lvl,
                   Validity)
      SELECT RAR.MediaId,
             RAR.RelationshipTypeName,
             RAR.RelatedMediaId,
             RAR.Lvl,
             RAR.Validity
        FROM ReturnAllRelated AS RAR;

      RETURN
  END 
GO


