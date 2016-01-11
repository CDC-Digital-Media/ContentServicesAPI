SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser2]'
GO
ALTER TABLE [Audit].[Registrations_TempUser2] DROP CONSTRAINT [PK_Audit_Registrations_TempUser2]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser2]'
GO
ALTER TABLE [Audit].[Registrations_TempUser2] DROP CONSTRAINT [DF__Registrat__Audit__6849492E]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser2]'
GO
ALTER TABLE [Audit].[Registrations_TempUser2] DROP CONSTRAINT [DF__Registrat__Audit__693D6D67]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser]'
GO
ALTER TABLE [Audit].[Registrations_TempUser] DROP CONSTRAINT [PK_Audit_Registrations_TempUser]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser]'
GO
ALTER TABLE [Audit].[Registrations_TempUser] DROP CONSTRAINT [DF__Registrat__Audit__60A82766]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [Audit].[Registrations_TempUser]'
GO
ALTER TABLE [Audit].[Registrations_TempUser] DROP CONSTRAINT [DF__Registrat__Audit__619C4B9F]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping constraints from [dbo].[MTree]'
GO
ALTER TABLE [dbo].[MTree] DROP CONSTRAINT [DF__MTree__ModifiedD__4AB8E647]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_Feed26_DeleteAudit] from [dbo].[Feed26]'
GO
DROP TRIGGER [dbo].[TR_dbo_Feed26_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_Feed26_InsertAudit] from [dbo].[Feed26]'
GO
DROP TRIGGER [dbo].[TR_dbo_Feed26_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_FeedItem26_DeleteAudit] from [dbo].[FeedItem26]'
GO
DROP TRIGGER [dbo].[TR_dbo_FeedItem26_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_FeedItem26_InsertAudit] from [dbo].[FeedItem26]'
GO
DROP TRIGGER [dbo].[TR_dbo_FeedItem26_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MTree_DeleteAudit] from [dbo].[MTree]'
GO
DROP TRIGGER [dbo].[TR_dbo_MTree_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MTree_InsertAudit] from [dbo].[MTree]'
GO
DROP TRIGGER [dbo].[TR_dbo_MTree_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgDomains_DeleteAudit] from [dbo].[MigrateOrgDomains]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgDomains_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgDomains_InsertAudit] from [dbo].[MigrateOrgDomains]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgDomains_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgSyndication2_DeleteAudit] from [dbo].[MigrateOrgSyndication2]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgSyndication2_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgSyndication2_InsertAudit] from [dbo].[MigrateOrgSyndication2]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgSyndication2_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgSyndication_DeleteAudit] from [dbo].[MigrateOrgSyndication]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgSyndication_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgSyndication_InsertAudit] from [dbo].[MigrateOrgSyndication]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgSyndication_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgs_DeleteAudit] from [dbo].[MigrateOrgs]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgs_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateOrgs_InsertAudit] from [dbo].[MigrateOrgs]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateOrgs_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateUsers_DeleteAudit] from [dbo].[MigrateUsers]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateUsers_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_MigrateUsers_InsertAudit] from [dbo].[MigrateUsers]'
GO
DROP TRIGGER [dbo].[TR_dbo_MigrateUsers_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_OrdDataExcel_DeleteAudit] from [dbo].[OrdDataExcel]'
GO
DROP TRIGGER [dbo].[TR_dbo_OrdDataExcel_DeleteAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping trigger [dbo].[TR_dbo_OrdDataExcel_InsertAudit] from [dbo].[OrdDataExcel]'
GO
DROP TRIGGER [dbo].[TR_dbo_OrdDataExcel_InsertAudit]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping DDL triggers'
GO
DROP TRIGGER [ddlDatabaseTriggerLog] ON DATABASE
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [Auxiliary].[FlattenedJSON]'
GO
DROP FUNCTION [Auxiliary].[FlattenedJSON]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[OrdDataExcel]'
GO
DROP TABLE [dbo].[OrdDataExcel]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MigrateUsers]'
GO
DROP TABLE [dbo].[MigrateUsers]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MigrateOrgSyndication2]'
GO
DROP TABLE [dbo].[MigrateOrgSyndication2]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MigrateOrgSyndication]'
GO
DROP TABLE [dbo].[MigrateOrgSyndication]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MigrateOrgs]'
GO
DROP TABLE [dbo].[MigrateOrgs]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MigrateOrgDomains]'
GO
DROP TABLE [dbo].[MigrateOrgDomains]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[FeedItem26]'
GO
DROP TABLE [dbo].[FeedItem26]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[Feed26]'
GO
DROP TABLE [dbo].[Feed26]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [Audit].[Registrations_TempUser2]'
GO
DROP TABLE [Audit].[Registrations_TempUser2]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [Audit].[Registrations_TempUser]'
GO
DROP TABLE [Audit].[Registrations_TempUser]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [dbo].[MTree]'
GO
DROP TABLE [dbo].[MTree]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Dropping [Audit].[History_Media_Media]'
GO
DROP VIEW [Audit].[History_Media_Media]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [Admin].[EvaluateMediaSets]'
GO
CREATE PROCEDURE [Admin].[EvaluateMediaSets]
      (@UserGUID     UNIQUEIDENTIFIER = NULL,
       @MediaSetName NVARCHAR(100) = NULL)
AS
/***********************************************************************************************
     DESCRIPTION
       This procedure returns a JSON string containing all of the MediaID a user hase access to
         
     PARAMETERS 
        @UserGUID (Required)
        This parameter is used to identify which user is to be evaluated.
        
        @MediaSetName (Optional)
        This parameter is used specify a specific mediaset to evaluate, used primarily for 
        testing.
        
     PROGRAMMING NOTES 
         This query uses a cursor to loop through all of the MediaSet Search XML, if performance
         becomes an issue, then set logic may be needed.
         
     DEPENDANCIES
        Tables/Views used in procedure:
  	      Media.Media
          Admin.MediaSet_Combined
  	         
     EXAMPLE CALL(S)
       
     EXEC Admin.EvaluateMediaSets @UserGuid = 'FE7369EE-8821-E411-9030-0017A477681A'
     
     EXEC Admin.EvaluateMediaSets @UserGuid = 'FE7369EE-8821-E411-9030-0017A477681A',
                                  @MediaSetName  = 'More Eyeballs'
     
     
     CHANGE HISTORY 
         2015-10-09 - Created by S. Frigard (sfi0@cdc.gov)
    ***********************************************************************************************/

--DECLARE @UserGUID     UNIQUEIDENTIFIER = 'FE7369EE-8821-E411-9030-0017A477681A',
--        @MediaSetName NVARCHAR(100) = 'More Eyeballs'

DECLARE @SearchString XML,
        @MediaIDCount INT 

DECLARE @MediaIDs TABLE
      (MediaID INT)

DECLARE GetSearchStrings CURSOR READ_ONLY
    FOR SELECT MSC.DynamicSearchCriteria
          FROM Admin.MediaSet_Combined AS MSC
         WHERE MSC.UserGUID     = @UserGUID --Returns Data if UserGUID is provided
           AND (MSC.MediaSetName = @MediaSetName --Optionally limits data to one specific Search
                 OR @MediaSetName IS NULL)
        UNION ALL
        SELECT MSD.SearchCriteria
          FROM Admin.MediaSetDynamic AS MSD
         WHERE @UserGUID IS NULL --If user GUID is not provided
           AND MSD.MediaSetName = @MediaSetName --Return search string for just requested Media Set Name

OPEN GetSearchStrings

FETCH NEXT FROM GetSearchStrings INTO @SearchString

WHILE (@@fetch_status <> -1)
  BEGIN
    IF (@@fetch_status <> -2)
      BEGIN
        INSERT @MediaIDs
               (MediaID)
        SELECT M.MediaId
          FROM Media.Media AS M
               CROSS JOIN (SELECT MediaID,
                                  MediaType,
                                  Language,
                                  SourceURL,
                                  OwningOrganization,
                                  MaintainingOrganization,
                                  MediaStatusCode
                             FROM Media.ShredSearchXML(@SearchString)
                            WHERE COALESCE(CAST(MediaId AS VARCHAR(16)), 
                                                MediaType, 
                                                Language, 
                                                SourceURL, 
                                                CAST(OwningOrganization AS VARCHAR(16)), 
                                                CAST(MaintainingOrganization AS VARCHAR(16)), 
                                                MediaStatusCode) IS NOT NULL) AS SE
         WHERE (M.MediaId                   = SE.MediaID
                 OR SE.MediaID IS NULL)
           AND (M.MediaTypeCode             = SE.MediaType
                 OR SE.MediaType IS NULL)
           AND (M.MediaStatusCode           = SE.MediaStatusCode
                 OR SE.MediaStatusCode IS NULL)
           AND (M.LanguageCode              = SE.Language
                 OR SE.Language IS NULL)
           AND (M.SourceUrl LIKE '%' + SE.SourceURL + '%'
                 OR SE.SourceURL IS NULL)
           AND (M.MaintainingBusinessUnitId = SE.MaintainingOrganization
                 OR SE.MaintainingOrganization IS NULL)
           AND (M.OwningBusinessUnitId      = SE.OwningOrganization
                 OR SE.OwningOrganization IS NULL)
      END

    FETCH NEXT FROM GetSearchStrings INTO @SearchString
  END

CLOSE GetSearchStrings

DEALLOCATE GetSearchStrings

SELECT @MediaIDCount = COUNT(*)
  FROM @MediaIDs AS MID

SELECT @UserGUID AS UserGUID,
       @MediaIDCount AS MediaIDCount,
       MediaIDsJSON
  FROM (SELECT '[' + STUFF(( SELECT ',{"MediaID":' + LU.MediaID +'}' 
                      FROM (SELECT CAST(MediaID AS VARCHAR(16)) AS MediaID 
                              FROM @MediaIDs) AS LU 
                               FOR XML PATH(''), TYPE ).value('.', 'VARCHAR(MAX)'), 1, 1, '') + ']' AS MediaIDsJSON) AS Static 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Altering [Media].[SearchMediaXML]'
GO
--USE [ContentServices_Dev]
--GO
--/****** Object:  StoredProcedure [Media].[SearchMediaXML]    Script Date: 12/09/2015 15:30:20 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

ALTER PROCEDURE [Media].[SearchMediaXML]
     (@XML XML = NULL)
AS
SET FMTONLY OFF
SET NOCOUNT ON
SET DEADLOCK_PRIORITY HIGH

--DECLARE @XML XML = 
--'<SearchCriteria>  <Status>published</Status>  <PublishDateFrom>7/28/2014</PublishDateFrom>  <PublishDateTo>7/28/2014</PublishDateTo>  <PageNumber>1</PageNumber>  <RowsPerPage>10</RowsPerPage>  <Sort>modifieddatetime|Desc,mediaid|Asc</Sort>  <SearchedFrom>AdminApi</SearchedFrom></SearchCriteria> 
--'

DECLARE @TimerStart          TIME,
        @TimerTotal          TIME = GETDATE(),
        @FTDuration          TIME,
        @NonFTDuration       TIME,
        @ResultDuration      TIME,
        @SortColumn          VARCHAR(100),
        @SortDirection       VARCHAR(4),
        @PageNumber          INT,
        @RowsPerPage         INT,
        @PageOffset          INT,
        @PageCount           INT,
        @RowCount            INT,
        @RowNumberLowerBound INT,
        @RowNumberUpperBound INT,
        @ID                  INT,
        @SearchElements      SearchXMLElements,
        @FT_ResultsPre       SearchXMLMediaRank,
        @SearchResult        SearchXMLMediaRank,
        @FT_Results          SearchXMLMediaRank,
        @SearchResultPre     SearchXMLMediaRank,
        @Topics              SearchXMLMediaRank

DECLARE @VocabAttributeValue TABLE
       (AttributeID  INT,
        ValueName    VARCHAR(400),
        ValueID      INT,
        LanguageCode VARCHAR(80))        

INSERT @SearchElements
       (TitleFT,
        TopicFT,
        MediaID,
        ParentID,
        MediaType,
        MediaStatusCode,
        Language,
        Audience,
        FullTextSearch,
        ExactTitle,
        Title,
        Description,
        ExactDescription,
        TopicID,
        VocabAttributeValue,
        ExactTopic,
        Topic,
        ContentGroup,
        LanguageISOCode,
        SourceName,
        SourceNameExact,
        SourceAcronym,
        SourceAcronymExact,
        SourceURL,
        SourceURLExact,
        ShowChildLevel,
        ShowParentLevel,
        GeoName,
        GeoNameID,
        GeoParentID,
        CountryCode,
        Latitude,
        Longitude,
        DateContentAuthored,
        ContentAuthoredSinceDate,
        ContentAuthoredBeforeDate,
        ContentAuthoredInRange,
        ContentAuthoredInRangeLow,
        ContentAuthoredInRangeHigh,
        DateContentUpdated,
        ContentUpdatedSinceDate,
        ContentUpdatedBeforeDate,
        ContentUpdatedInRange,
        ContentUpdatedInRangeLow,
        ContentUpdatedInRangeHigh,
        DateContentPublished,
        ContentPublishedSinceDate,
        ContentPublishedBeforeDate,
        ContentPublishedInRange,
        ContentPublishedInRangeLow,
        ContentPublishedInRangeHigh,
        DateContentReviewed,
        ContentReviewedSinceDate,
        ContentReviewedBeforeDate,
        ContentReviewedInRange,
        ContentReviewedInRangeLow,
        ContentReviewedInRangeHigh,
        DateSyndicationCaptured,
        SyndicationCapturedSinceDate,
        SyndicationCapturedBeforeDate,
        SyndicationCapturedInRange,
        SyndicationCapturedInRangeLow,
        SyndicationCapturedInRangeHigh,
        DateSyndicationUpdated,
        SyndicationUpdatedSinceDate,
        SyndicationUpdatedBeforeDate,
        SyndicationUpdatedInRange,
        SyndicationUpdatedInRangeLow,
        SyndicationUpdatedInRangeHigh,
        DateSyndicationVisible,
        SyndicationVisibleSinceDate,
        SyndicationVisibleBeforeDate,
        SyndicationVisibleInRange,
        SyndicationVisibleInRangeLow,
        SyndicationVisibleInRangeHigh,
        PublishDateFrom,
        PublishDateTo,
        ModifiedDateFrom,
        ModifiedDateTo,
        OwningOrganization,
        MaintainingOrganization,
        PersistentURLToken,
        MediaDisplay,
        PageNumber,
        RowsPerPage,
        PageOffset,
        SortColumn,
        SortDirection)
SELECT TitleFT,
       TopicFT,
       MediaID,
       ParentID,
       MediaType,
       MediaStatusCode,
       Language,
       Audience,
       FullTextSearch,
       ExactTitle,
       Title,
       Description,
       ExactDescription,
       TopicID,
       VocabAttributeValue,
       ExactTopic,
       Topic,
       ContentGroup,
       LanguageISOCode,
       SourceName,
       SourceNameExact,
       SourceAcronym,
       SourceAcronymExact,
       SourceURL,
       SourceURLExact,
       ShowChildLevel,
       ShowParentLevel,
       GeoName,
       GeoNameID,
       GeoParentID,
       CountryCode,
       Latitude,
       Longitude,
       CAST(DateContentAuthored AS DATETIME2(0))           AS DateContentAuthored,
       CAST(ContentAuthoredSinceDate AS DATETIME2(0))      AS ContentAuthoredSinceDate,
       CAST(ContentAuthoredBeforeDate AS DATETIME2(0))     AS ContentAuthoredBeforeDate,
       ContentAuthoredInRange,
       ContentAuthoredInRangeLow,
       ContentAuthoredInRangeHigh,
       CAST(DateContentUpdated AS DATETIME2(0))            AS DateContentUpdated,
       CAST(ContentUpdatedSinceDate AS DATETIME2(0))       AS ContentUpdatedSinceDate,
       CAST(ContentUpdatedBeforeDate AS DATETIME2(0))      AS ContentUpdatedBeforeDate,
       ContentUpdatedInRange,
       ContentUpdatedInRangeLow,
       ContentUpdatedInRangeHigh,
       CAST(DateContentPublished AS DATETIME2(0))          AS DateContentPublished,
       CAST(ContentPublishedSinceDate AS DATETIME2(0))     AS ContentPublishedSinceDate,
       CAST(ContentPublishedBeforeDate AS DATETIME2(0))    AS ContentPublishedBeforeDate,
       ContentPublishedInRange,
       ContentPublishedInRangeLow,
       ContentPublishedInRangeHigh,
       CAST(DateContentReviewed AS DATETIME2(0))           AS DateContentReviewed,
       CAST(ContentReviewedSinceDate AS DATETIME2(0))      AS ContentReviewedSinceDate,
       CAST(ContentReviewedBeforeDate AS DATETIME2(0))     AS ContentReviewedBeforeDate,
       ContentReviewedInRange,
       ContentReviewedInRangeLow,
       ContentReviewedInRangeHigh,
       CAST(DateSyndicationCaptured AS DATETIME2(0))       AS DateSyndicationCaptured,
       CAST(SyndicationCapturedSinceDate AS DATETIME2(0))  AS SyndicationCapturedSinceDate,
       CAST(SyndicationCapturedBeforeDate AS DATETIME2(0)) AS SyndicationCapturedBeforeDate,
       SyndicationCapturedInRange,
       SyndicationCapturedInRangeLow,
       SyndicationCapturedInRangeHigh,
       CAST(DateSyndicationUpdated AS DATETIME2(0))        AS DateSyndicationUpdated,
       CAST(SyndicationUpdatedSinceDate AS DATETIME2(0))   AS SyndicationUpdatedSinceDate,
       CAST(SyndicationUpdatedBeforeDate AS DATETIME2(0))  AS SyndicationUpdatedBeforeDate,
       SyndicationUpdatedInRange,
       SyndicationUpdatedInRangeLow,
       SyndicationUpdatedInRangeHigh,
       CAST(DateSyndicationVisible AS DATETIME2(0))        AS DateSyndicationVisible,
       CAST(SyndicationVisibleSinceDate AS DATETIME2(0))   AS SyndicationVisibleSinceDate,
       CAST(SyndicationVisibleBeforeDate AS DATETIME2(0))  AS SyndicationVisibleBeforeDate,
       SyndicationVisibleInRange,
       SyndicationVisibleInRangeLow,
       SyndicationVisibleInRangeHigh,
       CAST(PublishDateFrom AS DATETIME2(0))               AS PublishDateFrom,
       CAST(PublishDateTo AS DATETIME2(0))                 AS PublishDateTo,
       CAST(ModifiedDateFrom AS DATETIME2(0))              AS ModifiedDateFrom,
       CAST(ModifiedDateTo AS DATETIME2(0))                AS ModifiedDateTo,
       OwningOrganization,
       MaintainingOrganization,
       PersistentURLToken,
       MediaDisplay,
       PageNumber,
       RowsPerPage,
       PageOffset,
       SortColumn,
       SortDirection
  FROM Media.ShredSearchXML(@XML) 
  

IF NOT EXISTS (SELECT 1
             FROM Media.Media AS M
                  INNER JOIN Media.FeedAggregate AS FA
                          ON FA.MediaID = M.MediaId
                  INNER JOIN @SearchElements AS SE
                          ON M.MediaId IN (SELECT CAST(StringSegment AS INT)
                                                   FROM DelimitedStringToTable(SE.MediaID, 'No', ','))
                          OR M.MediaId IN (SELECT CAST(StringSegment AS INT)
                                                   FROM DelimitedStringToTable(SE.ParentID, 'No', ','))) 
  GOTO NonFeedAggregateSearch

ELSE EXEC Media.SearchMediaXML_FeedAggregate @XML = @XML 

GOTO EndOfCode
  
NonFeedAggregateSearch:
  
--SELECT * FROM @SearchElements AS SE  
  
--Overrided Display whene ContentGroup search is being made
UPDATE @SearchElements
   SET MediaDisplay = NULL 
 WHERE ContentGroup IS NOT NULL


IF NOT EXISTS(SELECT * 
                FROM @SearchElements AS SE)
  INSERT @SearchElements (TitleFT) VALUES(NULL)
  

--Convert JSON to Table for @VocabAttributeValue
INSERT @VocabAttributeValue
        ( AttributeID ,
          ValueName ,
          LanguageCode
        )
SELECT MIN(CASE
             WHEN DSTT.Attribute = 'attributeID'
               THEN Value
             ELSE NULL
           END) AS AttributeID,
       MIN(CASE
             WHEN DSTT.Attribute = 'ValueName'
               THEN Value
             ELSE NULL
           END) AS ValueName,
       MIN(CASE
             WHEN DSTT.Attribute = 'language'
               THEN Value
             ELSE NULL
           END) AS LanguageCode
  FROM DelimitedStringToTable((SELECT SE.VocabAttributeValue
                                 FROM @SearchElements AS SE
                                WHERE SE.VocabAttributeValue IS NOT NULL), 'N', '}') AS LU
       CROSS APPLY (SELECT RowId,
                           REPLACE(REPLACE(REPLACE(LEFT(StringSegment, CHARINDEX(':', StringSegment) - 1), '"', ''), '{', ''), '[', '') AS Attribute,
                           REPLACE(SUBSTRING(StringSegment, CHARINDEX(':', StringSegment) + 1, LEN(StringSegment)), '"', '')            AS Value
                      FROM DelimitedStringToTable(LU.StringSegment, 'N', ',')) AS DSTT
 WHERE LU.StringSegment <> ']'
 GROUP BY LU.RowId 
 
 UPDATE VAV 
    SET ValueID = V.ValueID
   FROM @VocabAttributeValue AS VAV
   INNER JOIN Vocabulary.Value AS V
   ON VAV.ValueName = V.ValueName
   AND VAV.LanguageCode = V.LanguageCode
   
--Pagination and sort Setup
SELECT @PageNumber = ISNULL(SE.PageNumber, ISNULL(FLOOR(SE.PageOffset/SE.RowsPerPage) + 1,1)),
       @RowsPerPage = ISNULL(SE.RowsPerPage, 100),
       @SortColumn = SE.SortColumn,
       @SortDirection = SE.SortDirection,
       @PageOffset = SE.PageOffset
  FROM @SearchElements AS SE 


SELECT @RowNumberLowerBound = CASE
                                WHEN @PageOffset IS NULL
                                  THEN (@PageNumber - 1) * @RowsPerPage + 1
                                ELSE @PageOffset + 1
                              END,
       @RowNumberUpperBound = CASE
                                WHEN @PageOffset IS NULL
                                  THEN @PageNumber * @RowsPerPage
                                ELSE (@PageOffset + @RowsPerPage) 
                              END 

--FT search Setup
SET @TimerStart = GETDATE()

IF EXISTS(SELECT *
            FROM @SearchElements AS SE
           WHERE SE.TitleFT IS NOT NULL
              OR SE.TopicFT IS NOT NULL
              OR SE.FullTextSearch IS NOT NULL) 

  BEGIN
    --if there is a search requested for FT the populate, else put all in.
    IF EXISTS(SELECT *
                FROM @SearchElements AS SE
               WHERE SE.TitleFT IS NOT NULL)
      BEGIN
        DECLARE @SearchTitle NVARCHAR(250)

        SELECT @SearchTitle = 'FORMSOF (INFLECTIONAL, "' + IsNull(XSB.TitleFT, '') + '")'
          FROM @SearchElements AS XSB

        INSERT @FT_ResultsPre
               (MediaID,
                Rank)
        SELECT M.MediaId,
               ISNULL(CT.RANK, 0) AS Rank
          FROM Media.Media AS M
               INNER JOIN CONTAINSTABLE(Media.Media, (Title, Description), @SearchTitle) AS CT
                       ON M.MediaId = CT.[KEY]
      END

    IF EXISTS(SELECT *
                FROM @SearchElements AS SE
               WHERE SE.TopicFT IS NOT NULL)
      BEGIN
        DECLARE @SearchTopic NVARCHAR(250)

        SELECT @SearchTopic = 'FORMSOF (INFLECTIONAL, "' + IsNull(XSB.TopicFT, '') + '")'
          FROM @SearchElements AS XSB

        INSERT @FT_ResultsPre
               (MediaID,
                Rank)
        SELECT MV.MediaId,
               ISNULL(CT.RANK, 0)
          FROM Vocabulary.Value AS V
               INNER JOIN CONTAINSTABLE(Vocabulary.Value, (ValueName), @SearchTopic) AS CT
                       ON V.ValueID = CT.[KEY]
               INNER JOIN Auxiliary.DescendentsLookup AS D
                       ON D.ValueID = V.ValueID
               INNER JOIN Media.MediaValues AS MV
                       ON MV.ValueId = D.DescendentValueID
               INNER JOIN Vocabulary.Attributes AS A
                       ON A.AttributeID = MV.AttributeID
                      AND A.AttributeName = 'Topic'
                      
                      
      END

    IF EXISTS(SELECT *
                FROM @SearchElements AS SE
               WHERE SE.FullTextSearch IS NOT NULL)
      BEGIN
        DECLARE @SearchAll NVARCHAR(250)

        SELECT @SearchAll = 'FORMSOF (INFLECTIONAL, "' + IsNull(XSB.FullTextSearch, '') + '")'
          FROM @SearchElements AS XSB

        INSERT @FT_ResultsPre
               (MediaID,
                Rank)
        SELECT LU.MediaId,
               MAX(LU.Rank) AS Rank
          FROM (SELECT M.MediaId,
                       ISNULL(CT.RANK, 0) AS Rank
                  FROM Media.Media AS M
                       INNER JOIN CONTAINSTABLE(Media.Media, (Title, Description), @SearchAll) AS CT
                               ON M.MediaId = CT.[KEY]
                UNION
                SELECT MV.MediaId,
                       ISNULL(CT.RANK, 0)
                  FROM Vocabulary.Value AS V
                       INNER JOIN CONTAINSTABLE(Vocabulary.Value, (ValueName), @SearchAll) AS CT
                               ON V.ValueID = CT.[KEY]
                       INNER JOIN Auxiliary.DescendentsLookup AS D
                               ON D.ValueID = V.ValueID
                       INNER JOIN Media.MediaValues AS MV
                               ON MV.ValueId = D.DescendentValueID
                       INNER JOIN Vocabulary.Attributes AS A
                               ON A.AttributeID = MV.AttributeID
                              AND A.AttributeName = 'Topic') AS LU
         GROUP BY LU.MediaId 
      END
  END
ELSE
  BEGIN
    INSERT @FT_ResultsPre
           (MediaId,
            Rank)
    SELECT M.MediaId,
           0
      FROM Media.Media AS M
           CROSS JOIN @SearchElements AS SE
     --Limit on Media   
     WHERE (M.MediaId IN (SELECT CAST(StringSegment AS INT)
                            FROM DelimitedStringToTable(SE.MediaID, 'No', ','))
             OR SE.MediaID IS NULL)
       AND (M.MediaID IN (SELECT MR.RelatedMediaId
                            FROM Media.MediaRelationship AS MR
                           WHERE MR.MediaId = SE.ParentID
                             AND MR.RelationshipTypeName = 'Is Parent Of')
             OR SE.ParentID IS NULL)
  END 


--Test to see if ParentID is not present. If it is not, then delete all feed Item elements from @FT_ResultsPre
IF EXISTS (SELECT *
             FROM @SearchElements AS SE
            WHERE SE.ParentID IS NULL
              AND SE.MediaID IS NULL
              AND SE.ContentGroup IS NULL)
  BEGIN
    DELETE @FT_ResultsPre
     WHERE MediaID IN (SELECT M.MediaId
                         FROM Media.Media AS M
                        WHERE M.MediaTypeCode IN ('Feed Item','Podcast'))
  END 

    
--Consolidate all of the mediaID and use the highest ranking discovered, filter any media not tied to a vocab item
INSERT @FT_Results
        ( MediaID, Rank )
SELECT FRP.MediaId,
       MAX(FRP.Rank) AS Rank
  FROM @FT_ResultsPre AS FRP 
 --WHERE FRP.MediaID IN (SELECT MV.MediaId
 --                        FROM Media.MediaValues AS MV)
 GROUP BY FRP.MediaId         

SET @FTDuration = GETDATE() - @TimerStart

SET @TimerStart = GETDATE()

--Identify MediaIDs that belong to a specific topic
IF EXISTS(SELECT *
            FROM @SearchElements AS SE
           WHERE SE.TopicID IS NOT NULL)
 BEGIN
   INSERT @Topics
         (MediaID)
   SELECT DISTINCT 
          MV.MediaID
     FROM Media.MediaValues AS MV
          INNER JOIN Auxiliary.DescendentsLookup AS DL
                  ON MV.ValueId = DL.DescendentValueID
          INNER JOIN Vocabulary.Attributes AS A
                  ON A.AttributeID = MV.AttributeID
                 AND A.AttributeName = 'Topic'
          CROSS JOIN @SearchElements AS SE
    WHERE DL.ValueID IN (SELECT CAST(StringSegment AS INT)
                           FROM DelimitedStringToTable(SE.TopicID, 'No', ','))
  END


INSERT @SearchResultPre
        ( MediaID, Rank )
SELECT M.MediaId,
       FR.Rank
  FROM Media.Media AS M  --Remove most of the Joins and perform IN searches like TopicID below
       INNER JOIN @FT_Results AS FR
               ON FR.MediaId = M.MediaId
       CROSS JOIN @SearchElements AS SE       
   --Search Media   
 WHERE (CHARINDEX(M.MediaTypeCode + ',', SE.MediaType + ',') <> 0
         OR SE.MediaType IS NULL)
   AND (CHARINDEX(M.MediaStatusCode, SE.MediaStatusCode) <> 0
         OR SE.MediaStatusCode IS NULL)
   AND (CHARINDEX(M.LanguageCode, SE.Language) <> 0
         OR SE.Language IS NULL)
   AND (M.Title = SE.ExactTitle
         OR SE.ExactTitle IS NULL)
   AND (M.Title LIKE '%' + SE.Title + '%'
         OR SE.Title IS NULL)
   AND (M.Description = SE.ExactDescription
         OR SE.ExactDescription IS NULL)
   AND (M.Description LIKE '%' + SE.Description + '%'
         OR SE.Description IS NULL)
   AND (M.SourceCode = SE.SourceNameExact
         OR SE.SourceNameExact IS NULL)
   AND (M.SourceCode LIKE '%' + SE.SourceName + '%'
         OR SE.SourceName IS NULL)
   AND (M.SourceUrl = SE.SourceURLExact
         OR SE.SourceURLExact IS NULL)
   AND (M.SourceUrl LIKE '%' + SE.SourceURL + '%'
         OR SE.SourceURL IS NULL)
   AND (M.MaintainingBusinessUnitId = SE.MaintainingOrganization
         OR SE.MaintainingOrganization IS NULL)
   AND (M.OwningBusinessUnitId = SE.OwningOrganization
         OR SE.OwningOrganization IS NULL)
   AND (M.PersistentURLToken = SE.PersistentURLToken
         OR SE.PersistentURLToken IS NULL)
--ContentGroup Filter         
   AND (M.MediaId IN (SELECT MV.MediaId
                        FROM Media.MediaValues AS MV
                             INNER JOIN Vocabulary.Attributes
                                ON MV.AttributeID = Attributes.AttributeID
                             INNER JOIN Vocabulary.Value
                                ON MV.ValueId = Value.ValueID
                       WHERE ValueName = SE.ContentGroup
                         AND AttributeName = 'ContentGroup'
                       UNION
                      SELECT MR.RelatedMediaId
                        FROM Media.MediaRelationship AS MR
                       WHERE MR.MediaId IN (SELECT MV.MediaId
                                              FROM Media.MediaValues AS MV
                                                   INNER JOIN Vocabulary.Attributes
                                                           ON MV.AttributeID = Attributes.AttributeID
                                                   INNER JOIN Vocabulary.Value
                                                           ON MV.ValueId = Value.ValueID
                                             WHERE ValueName = SE.ContentGroup
                                               AND AttributeName = 'ContentGroup'
                                               AND MR.RelationshipTypeName = 'Is Parent Of') )
         OR SE.ContentGroup IS NULL) 
   --Search VocabAttributeValue
   AND (M.MediaId IN (SELECT MV.MediaId
                        FROM Media.MediaValues AS MV
                             INNER JOIN Vocabulary.Attributes AS A
                                     ON A.AttributeID = MV.AttributeID
                                    AND A.AttributeName = 'Topic'
                             INNER JOIN @VocabAttributeValue AS VAV
                                     ON MV.ValueId = VAV.ValueID
                                    AND MV.AttributeID = VAV.AttributeID)
         OR SE.VocabAttributeValue IS NULL) 
   --Search Source
   AND (M.SourceCode IN (SELECT S.SourceCode 
                           FROM Media.Source AS S 
                          WHERE S.Acronym = SE.SourceAcronym)
         OR SE.SourceAcronym IS NULL)
   AND (M.SourceCode IN (SELECT S.SourceCode 
                           FROM Media.Source AS S 
                          WHERE S.Acronym LIKE '%' + SE.SourceAcronymExact + '%')
         OR SE.SourceAcronymExact IS NULL)
   --Search Geo Data
   AND (M.MediaId IN (SELECT GD.MediaId
                        FROM Media.GeoData AS GD
                       WHERE GD.GeoNameID = SE.GeoNameID)
         OR SE.GeoNameID IS NULL)
   AND (M.MediaId IN (SELECT GD.MediaId
                        FROM Media.GeoData AS GD
                             INNER JOIN Auxiliary.GeoNames AS GN
                                ON GN.GeoNameID = GD.GeoNameID
                       WHERE GN.Name = SE.GeoName)
         OR SE.GeoName IS NULL)
   AND (M.MediaId IN (SELECT GD.MediaId
                        FROM Media.GeoData AS GD
                             INNER JOIN Auxiliary.GeoNames AS GN
                                ON GN.GeoNameID = GD.GeoNameID
                       WHERE GN.Country_Code = SE.CountryCode)
         OR SE.CountryCode IS NULL)
   AND (M.MediaId IN (SELECT GD.MediaId
                        FROM Media.GeoData AS GD
                             INNER JOIN Auxiliary.GeoNames AS GN
                                ON GN.GeoNameID = GD.GeoNameID
                       WHERE GN.Latitude = SE.Latitude)
         OR SE.Latitude IS NULL) 
   AND (M.MediaId IN (SELECT GD.MediaId
                        FROM Media.GeoData AS GD
                             INNER JOIN Auxiliary.GeoNames AS GN
                                ON GN.GeoNameID = GD.GeoNameID
                       WHERE GN.Longitude = SE.Longitude)
         OR SE.Longitude IS NULL)
   --Search Language
   AND (M.LanguageCode IN (SELECT L.LanguageCode
                             FROM Auxiliary.Language AS L WHERE CHARINDEX(L.ISOLanguageCode3, SE.LanguageISOCode) <> 0)
         OR SE.LanguageISOCode IS NULL)
   --Authored Date
   AND (M.DateContentAuthored BETWEEN SE.DateContentAuthored AND DateAdd(DAY,1,SE.DateContentAuthored)
         OR SE.DateContentAuthored IS NULL)
   AND (M.DateContentAuthored > SE.ContentAuthoredSinceDate  
         OR SE.ContentAuthoredSinceDate IS NULL)
   AND (M.DateContentAuthored < SE.ContentAuthoredBeforeDate
         OR SE.ContentAuthoredBeforeDate IS NULL)
   AND (M.DateContentAuthored BETWEEN SE.ContentAuthoredInRangeLow AND SE.ContentAuthoredInRangeHigh
         OR SE.ContentAuthoredInRange IS NULL)
   --UpdatedDate
   AND (M.DateContentUpdated BETWEEN SE.DateContentUpdated AND DateAdd(DAY,1,SE.DateContentUpdated)
         OR SE.DateContentUpdated IS NULL)
   AND (M.DateContentUpdated > SE.ContentUpdatedSinceDate
         OR SE.ContentUpdatedSinceDate IS NULL)
   AND (M.DateContentUpdated < SE.ContentUpdatedBeforeDate
         OR SE.ContentUpdatedBeforeDate IS NULL)
   AND (M.DateContentUpdated BETWEEN SE.ContentUpdatedInRangeLow AND SE.ContentUpdatedInRangeHigh
         OR SE.ContentUpdatedInRange IS NULL)
   --Published
   AND (M.DateContentPublished BETWEEN SE.DateContentPublished AND DateAdd(DAY,1,SE.DateContentPublished)
         OR SE.DateContentPublished IS NULL)
   AND (M.DateContentPublished > SE.ContentPublishedSinceDate
         OR SE.ContentPublishedSinceDate IS NULL)
   AND (M.DateContentPublished < SE.ContentPublishedBeforeDate
         OR SE.ContentPublishedBeforeDate IS NULL)
   AND (M.DateContentPublished BETWEEN SE.ContentPublishedInRangeLow AND SE.ContentPublishedInRangeHigh
         OR SE.ContentPublishedInRange IS NULL)
   --Reviewed
   AND (M.DateContentReviewed BETWEEN SE.DateContentReviewed AND DateAdd(DAY,1,SE.DateContentReviewed)
         OR SE.DateContentReviewed IS NULL)
   AND (M.DateContentReviewed > SE.ContentPublishedSinceDate
         OR SE.ContentPublishedSinceDate IS NULL)
   AND (M.DateContentReviewed < SE.ContentPublishedBeforeDate
         OR SE.ContentPublishedBeforeDate IS NULL)
   AND (M.DateContentReviewed BETWEEN SE.ContentReviewedInRangeLow AND SE.ContentReviewedInRangeHigh
         OR SE.ContentReviewedInRange IS NULL)
   --Syndication captured
   AND (M.DateSyndicationCaptured BETWEEN SE.DateSyndicationCaptured AND DateAdd(DAY,1,SE.DateSyndicationCaptured)
         OR SE.DateSyndicationCaptured IS NULL)
   AND (M.DateSyndicationCaptured > SE.SyndicationCapturedSinceDate
         OR SE.SyndicationCapturedSinceDate IS NULL)
   AND (M.DateSyndicationCaptured < SE.SyndicationCapturedBeforeDate
         OR SE.SyndicationCapturedBeforeDate IS NULL)
   AND (M.DateSyndicationCaptured BETWEEN SE.SyndicationCapturedInRangeLow AND SE.SyndicationCapturedInRangeHigh
         OR SE.SyndicationCapturedInRange IS NULL)         
   --Syndication Updated
   AND (M.DateSyndicationUpdated BETWEEN SE.DateSyndicationUpdated AND DateAdd(DAY,1,SE.DateSyndicationUpdated)
        OR SE.DateSyndicationUpdated IS NULL)
   AND (M.DateSyndicationUpdated > SE.SyndicationUpdatedSinceDate
         OR SE.SyndicationUpdatedSinceDate IS NULL)
   AND (M.DateSyndicationUpdated < SE.SyndicationUpdatedBeforeDate
         OR SE.SyndicationUpdatedBeforeDate IS NULL)
   AND (M.DateSyndicationUpdated BETWEEN SE.SyndicationUpdatedInRangeLow AND SE.SyndicationUpdatedInRangeHigh
         OR SE.SyndicationUpdatedInRange IS NULL)
   --Syndication visible
   AND (M.DateSyndicationVisible BETWEEN SE.DateSyndicationVisible AND DateAdd(DAY,1,SE.DateSyndicationVisible)
         OR SE.DateSyndicationVisible IS NULL)
   AND (M.DateSyndicationVisible > SE.SyndicationVisibleSinceDate
         OR SE.SyndicationVisibleSinceDate IS NULL)
   AND (M.DateSyndicationVisible < SE.SyndicationVisibleBeforeDate
         OR SE.SyndicationVisibleBeforeDate IS NULL)
   AND (M.DateSyndicationVisible BETWEEN SE.SyndicationVisibleInRangeLow AND SE.SyndicationVisibleInRangeHigh
         OR SE.SyndicationVisibleInRange IS NULL)
   AND (M.ModifiedDateTime BETWEEN SE.ModifiedDateFrom AND DateAdd(DAY,1,SE.ModifiedDateTo)
         OR SE.ModifiedDateFrom IS NULL)
   AND (M.PublishedDateTime BETWEEN SE.PublishDateFrom AND DateAdd(DAY,1,SE.PublishDateTo)
         OR SE.PublishDateFrom IS NULL)
   -- ModifiedDateTime
      AND (M.ModifiedDateTime BETWEEN SE.ModifiedDateFrom AND SE.ModifiedDateTo
         OR SE.ModifiedDateFrom IS NULL)
   --PublshDateTime
   AND (M.PublishedDateTime BETWEEN SE.PublishDateFrom AND SE.PublishDateTo
         OR SE.PublishDateFrom IS NULL)         
   --TopicID search
   AND (M.MediaId in (SELECT T.MediaID
                        FROM @Topics AS T)
        OR SE.TopicID IS NULL)          
         
--SELECT * FROM @SearchResultPre AS SRP         
         
--Replaced above. If works, remove this section         
--IF EXISTS(SELECT *
--            FROM @SearchElements AS SE
--           WHERE SE.TopicID IS NOT NULL)
--  BEGIN
--    DELETE @SearchResultPre
--     WHERE MediaId NOT IN (SELECT LU1.MediaId
--                             FROM dbo.DelimitedStringToTable((SELECT TOP 1 TopicID
--                                                                FROM @SearchElements), 'N', ',') AS DSTT
--                                  INNER JOIN Auxiliary.DescendentsLookup AS D
--                                          ON D.ValueID = DSTT.StringSegment
--                                  INNER JOIN Media.MediaValues AS LU1
--                                          ON D.DescendentValueID = LU1.ValueId)
--END

IF EXISTS(SELECT *
            FROM @SearchElements AS SE
           WHERE SE.ExactTopic IS NOT NULL)
  BEGIN
    DELETE @SearchResultPre
     WHERE MediaId NOT IN (SELECT LU1.MediaId
                             FROM Vocabulary.Value AS V
                                  INNER JOIN @SearchElements AS SE2
                                          ON V.ValueName = SE2.ExactTopic
                                  INNER JOIN Auxiliary.DescendentsLookup AS D
                                          ON D.ValueID = V.ValueID
                                  INNER JOIN Media.MediaValues AS LU1
                                          ON D.DescendentValueID = LU1.ValueId
                                  INNER JOIN Vocabulary.Attributes AS A
                                          ON A.AttributeID = LU1.AttributeID
                                         AND A.AttributeName = 'Topic')
  END

IF EXISTS(SELECT *
            FROM @SearchElements AS SE
           WHERE SE.Topic IS NOT NULL)
  BEGIN
    DELETE @SearchResultPre
     WHERE MediaId NOT IN (SELECT LU1.MediaId
                             FROM Vocabulary.Value AS V
                                  INNER JOIN @SearchElements AS SE2
                                          ON V.ValueName LIKE '%' + SE2.Topic + '%'
                                  INNER JOIN Auxiliary.DescendentsLookup AS D
                                          ON D.ValueID = V.ValueID
                                  INNER JOIN Media.MediaValues AS LU1
                                          ON D.DescendentValueID = LU1.ValueId
                                  INNER JOIN Vocabulary.Attributes AS A
                                          ON A.AttributeID = LU1.AttributeID
                                         AND A.AttributeName = 'Topic')
  END 

--SELECT *   FROM @SearchResultPre AS SRP

IF EXISTS(SELECT *
            FROM @SearchElements AS SE
           WHERE SE.ContentGroup IS NOT NULL)
  BEGIN
    DELETE @SearchResultPre
     WHERE MediaId NOT IN (SELECT MV.MediaId
                             FROM Media.MediaValues AS MV
                                  INNER JOIN Vocabulary.Attributes AS A
                                          ON MV.AttributeID = A.AttributeID
                                         AND AttributeName = 'ContentGroup'
                            UNION
                           SELECT MR.RelatedMediaId
                             FROM Media.MediaRelationship AS MR
                            WHERE MR.MediaId IN (SELECT MV.MediaId
                                                   FROM Media.MediaValues AS MV
                                                        CROSS JOIN @SearchElements AS SE
                                                        INNER JOIN Vocabulary.Attributes
                                                                ON MV.AttributeID = Attributes.AttributeID
                                                        INNER JOIN Vocabulary.Value
                                                                ON MV.ValueId = Value.ValueID
                                                  WHERE ValueName = SE.ContentGroup
                                                    AND AttributeName = 'ContentGroup'
                                                    AND MR.RelationshipTypeName = 'Is Parent Of')) 

   DELETE @SearchResultPre
     WHERE MediaID IN (SELECT M.MediaId
                         FROM Media.Media AS M
                        WHERE M.MediaTypeCode IN ('Feed','Feed - Import','Feed - Proxy') )                                   
  END 

--SELECT *   FROM @SearchResultPre AS SRP


INSERT @SearchResult
       (MediaID,
        Rank)
SELECT MediaID,
       MAX(Rank) AS Rank
  FROM @SearchResultPre
 GROUP BY MediaID 
 
DELETE SR
  FROM @SearchResult AS SR
       INNER JOIN Media.Media AS M
               ON M.MediaId = SR.MediaId
       INNER JOIN Media.MediaTypes AS MT
               ON MT.MediaTypeCode = M.MediaTypeCode 
WHERE MT.Display NOT IN (SELECT MediaDisplay
                           FROM @SearchElements)
  AND EXISTS (SELECT MediaDisplay
                FROM @SearchElements                               
              WHERE MediaDisplay IS NOT NULL)
              
--Remove Audio
DELETE @SearchResult
 WHERE MediaID IN (SELECT M.MediaId
                     FROM Media.Media AS M
                    WHERE M.MediaTypeCode = 'Audio')            
                    
--if Content Group, delete anything not mapped with the correct Attribute
--IF EXISTS(SELECT * 
--            FROM @SearchElements AS SE
--           WHERE SE.ContentGroup IS NOT NULL)
--  BEGIN
--    DELETE @SearchResult
--     WHERE MediaID NOT IN (SELECT MV.MediaId
--                             FROM Media.MediaValues AS MV
--                                  INNER JOIN Vocabulary.Attributes
--                                          ON MV.AttributeID = Attributes.AttributeID
--                            WHERE AttributeName = 'ContentGroup') 
--  END           
                                 

SET @NonFTDuration = GETDATE() - @TimerStart

SET @TimerStart = GETDATE()

SELECT @PageCount = FLOOR(CAST( COUNT( * ) AS FLOAT ) / @RowsPerPage + .99 ),
       @RowCount = COUNT(*)
  FROM @SearchResult AS c

SET @RowsPerPage = CASE
                     WHEN @RowsPerPage > @RowCount
                       THEN @Rowcount
                     ELSE @RowsPerPage
                   END 
    
    
EXEC Media.SearchXML_Results
  @SearchResults       = @SearchResult,
  @SortColumn          = @SortColumn,
  @SortDirection       = @SortDirection,
  @PageNumber          = @PageNumber,
  @RowsPerPage         = @RowsPerPage,
  @PageOffset          = @PageOffset,
  @PageCount           = @PageCount,
  @RowCount            = @RowCount,
  @RowNumberLowerBound = @RowNumberLowerBound,
  @RowNumberUpperBound = @RowNumberUpperBound 
 

SET @ResultDuration = GETDATE() - @TimerStart

INSERT Auxiliary.SearchLog
       (SearchXML,
        CreatedDateTime,
        FTSearchDuration,
        NonFTSearchDuration,
        ResultDuration,
        TotalSearchDuration)
SELECT @XML AS XMLString,
       GETDATE() AS RunTime,
       @FTDuration AS FTDuration,
       @NonFTDuration AS NonFTDuration,
       @ResultDuration AS ResultDuration,
       GETDATE() - @TimerTotal AS TotalDuration

EndOfCode:
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_Media_MediaStatusCode_MediaTypeCode] on [Media].[Media]'
GO
CREATE NONCLUSTERED INDEX [IX_Media_MediaStatusCode_MediaTypeCode] ON [Media].[Media] ([MediaStatusCode], [MediaTypeCode])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_Media_MediaTypeCode2] on [Media].[Media]'
GO
CREATE NONCLUSTERED INDEX [IX_Media_MediaTypeCode2] ON [Media].[Media] ([MediaTypeCode]) INCLUDE ([CreatedDateTime], [EmbedCode], [MediaId], [ModifiedDateTime], [Title])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO