USE [ContentServices_SourceForge]
GO

/****** Object:  StoredProcedure [dbo].[GetHierarchicalVocabulary]    Script Date: 01/06/2014 17:08:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE  [dbo].[GetHierarchicalVocabulary]
GO

-- =============================================
-- Author:		Tim Carroll
-- Create date: 7/12/2013
-- Description:	Build the values to select on
-- =============================================
CREATE PROCEDURE [dbo].[GetHierarchicalVocabulary]
		@p_MediaType	nvarchar(100) = '',	--NEW
		@p_TopLevelValueSetIds	Varchar(max) = '',
		@p_ValueSetIds		Varchar(max) = '',
		@p_Active varchar(3) = '',
		--ValueIds specify an initial set of values to select as parents.  If blank,
		--select only parentless items   
		@p_ValueIds 	varchar(max) = '',
		@p_LanguageCode Varchar(80) = ''	

AS
BEGIN

	DECLARE @RequestedValueIds table (ValueId int)	
	DECLARE @TopLevelValueSets table (ValueSetID nvarchar(100))
	DECLARE @ValueSets table (ValueSetID nvarchar(100))
		
	---------------------------
	-- Build @RequestedValueIds
	---------------------------
	insert into @RequestedValueIds
		select ValueId=value from FN_ListToTable(@p_ValueIds+ ',', ',')

	---------------------------
	-- Build @ValueSets
	---------------------------
	insert into @TopLevelValueSets
		select ValueId=ValueSetID
			from Vocabulary.ValueSets vs		
			left outer join FN_ListToTable(@p_TopLevelValueSetIds+ ',', ',') vn
				on vn.value = vs.ValueSetID
			where @p_TopLevelValueSetIds = '' or vn.value is not null
			
	insert into @ValueSets
		select ValueId=ValueSetID
			from Vocabulary.ValueSets vs		
			left outer join FN_ListToTable(@p_ValueSetIds+ ',', ',') vn
				on vn.value = vs.ValueSetID
			where @p_ValueSetIds = '' or vn.value is not null
					
				
		
			--select ValueSetID from Vocabulary.ValueSets where  
			--	@p_ValueSetNames = '' or 
			--	(@p_ValueSetNames = 'Categories' and ValueSetName in ('Categories', 'Topics')) 
			--	or ValueSetName = @p_ValueSetName

	;With ReturnAllValues AS(
		SELECT distinct V.ValueID, 
						V.LanguageCode, 
						V.ValueName, 
						V.Description,
						V.DisplayOrdinal,
						Parent.RelationshipTypeName,
						Parent.ParentValueID, 
						Parent.ParentLanguageCode,
						V.Active,
						1 as Lvl
			FROM Vocabulary.Value AS V
		
			--@p_ValueSetNames + Where
			OUTER APPLY
			(
				--Top 1
				select svs.ValueSetID from Vocabulary.ValueToValueSets vvs 
					INNER JOIN @TopLevelValueSets svs 
						ON vvs.ValueSetID = svs.ValueSetID
					where vvs.ValueID = v.ValueID and vvs.ValueLanguageCode = v.LanguageCode 
			) as VS

			--@p_ValueIds Get Only Parentless items + Where
			LEFT OUTER JOIN @RequestedValueIds rvi 
				ON v.ValueID = rvi.ValueId
			OUTER APPLY
			(
				--Top 1	
				SELECT Rel.RelatedValueID as ParentValueID, Rel.RelatedValueLanguageCode as ParentLanguageCode,
					Rel.RelationshipTypeName
					from Vocabulary.ValueRelationship Rel
					WHERE RelationshipTypeName = 'Is Child Of' 
						and Rel.ValueID = V.ValueID and Rel.ValueLanguageCode = V.LanguageCode
						and Rel.Active = 'Yes'
			) as Parent
			where 
					(@p_ValueSetIds = '' or VS.ValueSetID is not null)
				AND ((@p_ValueIds <> '' AND rvi.ValueId is not null)
						OR (@p_ValueIds = '' AND Parent.ParentValueID is null))	--Only get top level if not passing Value Ids 			   
				AND (@p_LanguageCode = '' or V.LanguageCode = @p_LanguageCode)
			--ORDER BY V.DisplayOrdinal
		UNION ALL
			select	V.ValueID, 
					V.LanguageCode, 
					V.ValueName, 
					V.Description,
					V.DisplayOrdinal,
					VR.RelationshipTypeName,
					Parent.ValueID as ParentValueId, 
					Parent.LanguageCode as ParentLanguageCode,
					V.Active,
					Parent.Lvl + 1 As Lvl
			--select V.ValueID, V.ValueName, V.LanguageCode, Parent.ParentValueID, Parent.ParentLanguageCode 
				FROM Vocabulary.Value V 
				INNER JOIN Vocabulary.ValueRelationship AS VR
					ON V.ValueID = VR.ValueID and V.LanguageCode = VR.ValueLanguageCode 
						and VR.RelationshipTypeName in ('Is Child Of', 'Used For')
				INNER JOIN ReturnAllValues AS Parent
					ON VR.RelatedValueID = Parent.ValueID AND VR.ValueLanguageCode = Parent.LanguageCode											
				    
				    
			
			--@p_ValueSetNames + Where
			OUTER APPLY
			(
				--Top 1
				select svs.ValueSetID from Vocabulary.ValueToValueSets vvs 
					INNER JOIN @ValueSets svs 
						ON vvs.ValueSetID = svs.ValueSetID
					where vvs.ValueID = v.ValueID and vvs.ValueLanguageCode = v.LanguageCode 
			) as VS
			
			where  
				   (@p_ValueSetIds = '' or VS.ValueSetID is not null)					--@p_ValueSetNames
				 --  (@p_ValueSetNames = '' or VS.ValueSetID in 
					--	(select svs.ValueSetID from Vocabulary.ValueToValueSets vvs 
					--		INNER JOIN @ValueSets svs 
					--			ON vvs.ValueSetID = svs.ValueSetID
					--		where vvs.ValueID = V.ValueID and vvs.ValueLanguageCode = V.LanguageCode)
					--)
				   
				   AND (@p_LanguageCode = '' or V.LanguageCode = @p_LanguageCode)		--@p_LanguageCode Where	   		
				   AND VR.Active = 'Yes'
				   
	       		)
	select DISTINCT 
					v.ValueID,
					V.LanguageCode, 
					V.ValueName, 
					V.Description,
					V.DisplayOrdinal,
					V.RelationshipTypeName,
					V.ParentValueID, 
					V.ParentLanguageCode,
					V.Active,
					V.Lvl,
					MC.MediaCount
			from ReturnAllValues V
			OUTER APPLY
			(
				select COUNT(*) As MediaCount from Media.MediaValues MV
					inner join CombinedMediaList M on MV.MediaId = M.MediaId
					where MV.Active = 'Yes' and M.Active = 'Yes' and M.EffectiveStatus = 'Published' and M.DisplayOnSearch = 'Yes'
					and MV.ValueId = V.ValueID and MV.ValueLanguageCode = V.LanguageCode
					and (@p_MediaType = '' OR M.MediaTypeCode = @p_MediaType)
			) as MC
			WHERE ((@p_Active = '') OR (V.Active = @p_Active))
			ORDER BY V.Lvl, V.DisplayOrdinal





END


GO


Grant execute on [dbo].[GetHierarchicalVocabulary] to ContentServicesApplication_API

GO
