USE ContentServices_DataAPI
GO

/****** Object:  StoredProcedure [dbo].[BuildMediaSelectionSet]    Script Date: 10/31/2013 13:31:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

if exists (select 1 from information_schema.routines where ROUTINE_TYPE = 'PROCEDURE' 
and specific_name = 'BuildMediaSelectionSet')
	drop proc BuildMediaSelectionSet
GO

-- =============================================
-- Author:		Tim Carroll
-- Create date: 7/8/2013
-- Description:	Build the values to select on
-- =============================================
CREATE PROCEDURE [dbo].[BuildMediaSelectionSet]
		@p_SelectionId uniqueidentifier,
		@p_SelectionType varchar(20),
		@p_ExpirationMinutes int = 120,
		--@p_ValueIds 	varchar(max) = '',	--We could use ValueIds as well as RelatedValueName (search within a topic) (Resolved)
		--@p_RelatedValueName	Varchar(402) = '',	--Enough to allow for '%' + valueName + '%' --(Resolved)
		--@p_ValueSetName	Varchar(400) = '',		--We may say, only give results tnat are 'Topics' --(Resolved)	
		@p_LanguageCodes Varchar(max) = '',	--(Resolved)
		--@p_IncludedRelationships	nvarchar(max) = '',
		--@p_ExcludedRelationships	nvarchar(max) = ''
		@p_FullText nvarchar(4000) = '""',  --FullText search of Title and Description.  Default must be "" for SQL to work properly
		@p_SearchTitle varchar(3) = 'Yes',
		@p_SearchDescription varchar(3) = 'Yes'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @p_CreatedDateTime datetime = getdate(),
			@p_ExpirationDateTime datetime
		 

	set @p_ExpirationDateTime = DATEADD("MI", @p_ExpirationMinutes, @p_CreatedDateTime)

	if not exists(select 1 from Auxiliary.SelectionSets where SelectionId = @p_SelectionId and SelectionType = @p_SelectionType)
	BEGIN
		insert into Auxiliary.SelectionSets (SelectionId, SelectionType, CreatedDateTime, ExpirationDateTime)
			values (@p_SelectionId, @p_SelectionType, @p_CreatedDateTime, @p_ExpirationDateTime)

		------------------------
		--Parameters/Variables to select from
		------------------------
		DECLARE @LanguageCodes table (LanguageCode Varchar(80))

		DECLARE @rows as int, @lvl as int

		
		----@p_ValueSetName
		-----------------------------
		---- Build @ValueSets
		-----------------------------
		--	insert into @ValueSets
		--		select ValueSetID from Vocabulary.ValueSets where  
		--			@p_ValueSetName = '' or 
		--			(@p_ValueSetName = 'Categories' and ValueSetName in ('Categories', 'Topics')) 
		--			or ValueSetName = @p_ValueSetName

		--@p_ValueSetName
		---------------------------
		-- Build @LanguageCodes
		---------------------------
		insert into @LanguageCodes
			select LanguageCode=value from FN_ListToTable(@p_LanguageCodes+ ',', ',')
			
		--Anchor Condition
		--if( @p_SearchTitle = 'Yes' and @p_SearchDescription = 'Yes')
		BEGIN
		Insert into Auxiliary.SelectionIds (SelectionId, SelectionType, Id)
			select @p_SelectionId as SelectionId, 
				   @p_SelectionType as SelectionType,
				   m.MediaId

				FROM Media.Media AS m
					LEFT OUTER JOIN @LanguageCodes l
						 on m.LanguageCode = l.LanguageCode
			  WHERE (@p_LanguageCodes = '' or l.LanguageCode is not null) 
			  AND
			  (
					--(Contains((Title, Description), @p_FullText)) 
					(@p_SearchTitle = 'Yes' AND Contains((Title), @p_FullText)) OR
					(@p_SearchDescription = 'Yes' AND Contains((Description), @p_FullText))
			  )
		END



			  
	END

END


GO



select * from Auxiliary.SelectionIds where SelectionId = 'AF7E9D2F-7BA3-41EC-A742-00C2E20DE5FB' and SelectionType = 'temp1'