USE [SCCSPArauco]
GO
/****** Object:  StoredProcedure [dbo].[ESA_Revalidar]    Script Date: 1/12/2018 10:56:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ESA_Revalidar]
	@LoggedUserOid uniqueidentifier,
	@User NVARCHAR(MAX) OUTPUT,
	/*@Pass VARBINARY(MAX) OUTPUT*/
	@Pass NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT
		@Pass= Password,
		@User = Usuario
		--SELECT *
	FROM
		ESA_Usuario
	WHERE
		Oid = @LoggedUserOid

	--SET @Pass =
	--				(SELECT
	--					Password
	--				FROM 
	--					ESA_Usuario 
	--				WHERE
	--					Oid = @LoggedUserOid
	--				)
	--SET @User =
	--				(SELECT
	--					Usuario
	--				FROM 
	--					ESA_Usuario 
	--				WHERE
	--					Oid = @LoggedUserOid
	--				)
	RETURN
END

