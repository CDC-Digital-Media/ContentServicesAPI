USE [ContentServices_SourceForge]
GO

/****** Object:  StoredProcedure [dbo].[BuildValueSelectionSet]    Script Date: 11/10/2013 07:52:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Tim Carroll
-- Create date: 7/8/2013
-- Description:	Build the values to select on
-- =============================================
CREATE PROCEDURE [dbo].[BuildValueSelectionSet]
		@p_SelectionId uniqueidentifier,
		@p_SelectionType varchar(20),
		@p_ExpirationMinutes int = 120,
		@p_ValueIds 	varchar(max) = '',	--We could use ValueIds as well as RelatedValueName (search within a topic) (Resolved)
		@p_FullText	nvarchar(4000) = '""',	--FullText search of value name.  Default must be "" for SQL to work properly
		@p_ValueSetName	Varchar(400) = '',		--We may say, only give results tnat are 'Topics' --(Resolved)	
		@p_LanguageCodes Varchar(max) = '',	--(Resolved)
		@p_IncludedRelationships	nvarchar(max) = '',
		@p_ExcludedRelationships	nvarchar(max) = '',
		@p_FilterSelectionType varchar(20) = ''
		
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    DECLARE @p_CreatedDateTime    DATETIME = GETDATE(),
            @p_ExpirationDateTime DATETIME

    SET @p_ExpirationDateTime = DATEADD("MI", @p_ExpirationMinutes, @p_CreatedDateTime)

    IF NOT EXISTS(SELECT 1
                    FROM Auxiliary.SelectionSets
                   WHERE SelectionId = @p_SelectionId
                     AND SelectionType = @p_SelectionType)
      BEGIN
          INSERT INTO Auxiliary.SelectionSets
                      (SelectionId,
                       SelectionType,
                       CreatedDateTime,
                       ExpirationDateTime)
               VALUES (@p_SelectionId,
                       @p_SelectionType,
                       @p_CreatedDateTime,
                       @p_ExpirationDateTime)

          ------------------------
          --Parameters/Variables to select from
          ------------------------
          --@p_ValueIds
          DECLARE @RequestedValueIds TABLE
            (ValueId INT)
          --PARM  @p_FullText	Varchar(402)
          DECLARE @ValueSets TABLE
            (ValueSetID NVARCHAR(100))
          --PARM @p_LanguageCodes Varchar(80)
          DECLARE @LanguageCodes TABLE
            (LanguageCode VARCHAR(80))
          DECLARE @RelationshipTypeNames TABLE
            (RelationshipTypeName NVARCHAR(100))
          DECLARE @rows AS INT,
                  @lvl  AS INT

          ---------------------------
          -- Build @RequestedValueIds
          ---------------------------
          INSERT INTO @RequestedValueIds
          SELECT ValueId=value
            FROM FN_ListToTable(@p_ValueIds + ',', ',')

          --@p_ValueSetName
          ---------------------------
          -- Build @ValueSets
          ---------------------------
          INSERT INTO @ValueSets
          SELECT ValueSetID
            FROM Vocabulary.ValueSets
           WHERE @p_ValueSetName = ''
              OR (@p_ValueSetName = 'Categories'
                  AND ValueSetName IN ('Categories', 'Topics'))
              OR ValueSetName = @p_ValueSetName

          --@p_ValueSetName
          ---------------------------
          -- Build @LanguageCodes
          ---------------------------
          INSERT INTO @LanguageCodes
          SELECT LanguageCode=value
            FROM FN_ListToTable(@p_LanguageCodes + ',', ',')

          ---------------------------
          -- Build @RelationshipTypeNames
          ---------------------------
          IF @p_IncludedRelationships <> ''
            BEGIN
                --insert into @RelationshipsParm
                --	select Name=value from FN_ListToTable(@p_IncludedRelationships+ ',', ',')
                --select Name from @RelationshipsParm 
                --	where 
                INSERT INTO @RelationshipTypeNames
                SELECT r.RelationshipTypeName
                  FROM dbo.FN_ListToTable(@p_IncludedRelationships + ',', ',') p
                       INNER JOIN Auxiliary.RelationshipType r
                               ON p.Value = r.RelationshipTypeName
            END
          ELSE
            BEGIN
                INSERT INTO @RelationshipTypeNames
                SELECT r.RelationshipTypeName
                  FROM Auxiliary.RelationshipType r
                       LEFT OUTER JOIN dbo.FN_ListToTable(@p_ExcludedRelationships + ',', ',') p
                                    ON p.Value = r.RelationshipTypeName
                 WHERE p.Value IS NULL
            END

          --Anchor Condition
          INSERT INTO Auxiliary.SelectionValues
                      (SelectionId,
                       SelectionType,
                       ValueId,
                       ValueLanguageCode,
                       Lvl,
                       RelationshipTypeName,
                       RelatedValueId)
          SELECT DISTINCT @p_SelectionId            AS SelectionId,
                 @p_SelectionType          AS SelectionType,
                 v.ValueID,
                 v.LanguageCode            AS ValueLanguageCode,
                 0                         AS Lvl,
                 CAST('' AS NVARCHAR(100)) AS RelationshipTypeName,
                 CAST(NULL AS INT)         AS RelatedValueId
            FROM Vocabulary.Value AS v
                 LEFT OUTER JOIN Vocabulary.ValueToValueSets vvs
                              ON v.ValueID = vvs.ValueID
                             AND v.LanguageCode = vvs.ValueLanguageCode
                 LEFT OUTER JOIN @ValueSets svs
                              ON vvs.ValueSetID = svs.ValueSetID
                 LEFT OUTER JOIN @RequestedValueIds rvi
                              ON v.ValueID = rvi.ValueId
                 LEFT OUTER JOIN @LanguageCodes l
                              ON v.LanguageCode = l.LanguageCode
                 LEFT OUTER JOIN Auxiliary.SelectionValues fsv
							  ON fsv.SelectionId = @p_SelectionId and fsv.SelectionType = @p_FilterSelectionType and v.ValueId = fsv.ValueId
           WHERE (@p_FullText = '""'
                   OR CONTAINS(ValueName, @p_FullText))
             AND (@p_ValueIds = ''
                   OR rvi.ValueId IS NOT NULL)
             AND (@p_LanguageCodes = ''
                   OR l.LanguageCode IS NOT NULL)
             AND (@p_ValueSetName = ''
                   OR svs.ValueSetID IS NOT NULL)
             AND (@p_FilterSelectionType = ''
				   OR fsv.ValueId IS NOT NULL)
             AND Active = 'Yes'

          --Where 1 = 1
          SET @rows=@@ROWCOUNT
          SET @lvl=0

          WHILE @rows > 0
            BEGIN
                SET @lvl = @lvl + 1
                INSERT INTO Auxiliary.SelectionValues
                            (SelectionId,
                             SelectionType,
                             ValueId,
                             ValueLanguageCode,
                             Lvl,
                             RelationshipTypeName,
                             RelatedValueId)
                SELECT DISTINCT @p_SelectionId   AS SelectionId,
                       @p_SelectionType AS SelectionType,
                       VR.ValueID,
                       VR.ValueLanguageCode,
                       @lvl             AS Lvl,
                       --Don't return typeName and valueId, or uniqueness won't be guaranteed
                       --VR.RelationshipTypeName,
                       --VR.RelatedValueID,
                 CAST('' AS NVARCHAR(100)) AS RelationshipTypeName,
                 CAST(NULL AS INT)         AS RelatedValueId    
                  FROM Vocabulary.ValueRelationship AS VR
                       INNER JOIN @RelationshipTypeNames RTN
                               ON VR.RelationshipTypeName = RTN.RelationshipTypeName
                       INNER JOIN Vocabulary.Value val
                               ON val.ValueID = VR.ValueID
                              AND val.LanguageCode = VR.ValueLanguageCode
                       INNER JOIN Auxiliary.SelectionValues AS P --Parent
                               ON VR.RelatedValueID = P.ValueID
                              AND VR.ValueLanguageCode = P.ValueLanguageCode
                              AND P.SelectionId = @p_SelectionId
                       LEFT OUTER JOIN Auxiliary.SelectionValues Dups
                                    ON VR.ValueID = Dups.ValueID
                                   AND VR.ValueLanguageCode = Dups.ValueLanguageCode
                                   AND Dups.SelectionId = @p_SelectionId
                 WHERE P.Lvl = (@lvl - 1)
                   AND Dups.ValueID IS NULL
                   AND VR.Active = 'Yes'
                   AND val.Active = 'Yes'

                SET @rows=@@ROWCOUNT
            END
      END
END 


GO


grant execute on [BuildValueSelectionSet] to ContentServicesApplication_API
GO
