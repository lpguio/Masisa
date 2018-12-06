ALTER PROCEDURE [dbo].[ESA_Login]
	@User NVARCHAR(50),
	@Pass VARCHAR(50),
	@OidUsuario UNIQUEIDENTIFIER OUTPUT,
	@SHAPassword NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Parametros		VARCHAR(max)			= (ISNULL(@User,'')+';'+ISNULL(@Pass,''))
	DECLARE	@Null_Oid		UNIQUEIDENTIFIER= '00000000-0000-0000-0000-000000000000'
	
	DECLARE @saltedPassword VARCHAR(MAX)	= (SELECT Password FROM ESA_Usuario WHERE Usuario = @User)

	DECLARE @delimPos INT = CHARINDEX('*', @saltedPassword)--El delimitador es * segun el codigo de XAF

	IF @delimPos > 0
	BEGIN
		DECLARE @SaltB64 VARCHAR(MAX) = SUBSTRING (@saltedPassword ,0 , @delimPos)  

		DECLARE @calculatedEncPassword VARBINARY(MAX) = HASHBYTES('SHA2_512', @SaltB64 + @Pass)
		DECLARE @calculatedEncPasswordB64 VARCHAR(MAX) = cast('' as xml).value('xs:base64Binary(sql:variable("@calculatedEncPassword"))', 'varchar(max)')
		DECLARE @expectedSaltedPasswordB64 VARCHAR(MAX) = SUBSTRING (@saltedPassword ,@delimPos +1 , LEN(@saltedPassword))
		
		IF @expectedSaltedPasswordB64 = @calculatedEncPasswordB64
		BEGIN
			SET @OidUsuario = (SELECT Oid FROM ESA_Usuario WHERE Usuario = @User)
			SET @SHAPassword = (SELECT Password FROM ESA_Usuario WHERE Usuario = @User)
		END
	END	

	

	IF @OidUsuario IS NOT NULL
	BEGIN
		EXEC ESA_Crear_Audit @OidUsuario, @OidUsuario, 1, 'ESA_Login', NULL,''
	END
	ELSE
	BEGIN
		EXEC ESA_Crear_Audit @Null_Oid, @Null_Oid, 0, 'ESA_Login', NULL,@Parametros
	END

	RETURN
	--SET @OidUsuario =
	--				(SELECT
	--					Oid
	--				FROM 
	--					ESA_Usuario 
	--				WHERE
	--					Usuario = @User
	--					AND HASHBYTES('SHA2_512', @Pass + UPPER(CAST(Oid AS VARCHAR(36)))) = Password
	--				)



	--RETURN
END

