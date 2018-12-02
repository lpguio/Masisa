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

