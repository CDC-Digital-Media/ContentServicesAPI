USE [ContentServices_SourceForge]
GO

/****** Object:  UserDefinedFunction [dbo].[DelimitedStringToTable]    Script Date: 11/10/2013 07:59:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DelimitedStringToTable]
     (@DelimitedString NVARCHAR(MAX),
      @IncludeEmptyStrings NCHAR(1) = 'Y', 
      @Delimiter NCHAR(1) = ',')
      
RETURNS @Item TABLE
             (RowId INT IDENTITY(1,1) NOT NULL,
	          StringSegment NVARCHAR(MAX) NULL)

WITH EXECUTE AS CALLER
AS 
BEGIN
    DECLARE @StringSegment          NVARCHAR(MAX),
            @LengthOfString         INT,
            @SubStringStartPosition BIGINT,
            @SubStringEndPosition   BIGINT,
            @EndOfString            NCHAR(1)


    SELECT @SubStringStartPosition = 1,
           @SubStringEndPosition = 0,
           @LengthOfString = LEN( @DelimitedString ),
           @EndOfString = 'N'


    WHILE @EndOfString <> 'Y'
      BEGIN
        SET @SubStringEndPosition=CHARINDEX( @Delimiter, @DelimitedString, @SubStringStartPosition )

        IF @SubStringEndPosition > 0
          BEGIN
            SET @StringSegment = SUBSTRING( @DelimitedString, @SubStringStartPosition, @SubStringEndPosition - @SubStringStartPosition )
            SET @SubStringStartPosition=@SubStringEndPosition + 1
          END
        ELSE
          BEGIN
            SET @EndOfString='Y'
            SET @StringSegment = SUBSTRING( @DelimitedString, @SubStringStartPosition, @LengthOfString - ( @SubStringStartPosition - 1 ) )
          END

        IF( @StringSegment <> ''
             OR @IncludeEmptyStrings = 'Y' )
          INSERT INTO @Item(StringSegment)
                     VALUES(LTRIM(RTRIM(@StringSegment)))
      END

    RETURN
  END 

GO


