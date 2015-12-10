USE [ContentServices_SourceForge]
GO

/****** Object:  StoredProcedure [dbo].[UpdateCardViewedCount]    Script Date: 11/10/2013 07:52:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Tim Carroll
-- Create date: 1/10/2014
-- Description:	Build the values to select on
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCardViewedCount]
		@p_CardInstanceId uniqueidentifier,
		@p_ModifiedByGuid uniqueidentifier
		
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    DECLARE @ModifiedDateTime    DATETIME = GETDATE()

	update dbo.CardInstances 
					SET ViewCount = ViewCount + 1, 
						LastViewedDateTime = @ModifiedDateTime, 
						ModifiedByGuid = @p_ModifiedByGuid, 
						ModifiedDateTime = @ModifiedDateTime
		where CardInstanceId = @p_CardInstanceId
END 


GO


grant execute on [UpdateCardViewedCount] to ContentServicesApplication_API
GO
