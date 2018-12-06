USE [SCCSPArauco]
GO
/****** Object:  StoredProcedure [dbo].[ESA_Crear_Empleado]    Script Date: 5/12/2018 11:42:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 ALTER PROCEDURE [dbo].[ESA_Crear_Empleado]
	@LoggedUserOid uniqueidentifier,
	@Oid uniqueidentifier,
	@RUT VARCHAR(20),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Correo NVARCHAR(250),
	@Telefono NVARCHAR(50),
	@ManejaCasino BIT = 0,
	@EnrollID INT,
	@Contraseña NVARCHAR(MAX),
	@Error NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Query NVARCHAR(Max)=' EXEC [ESA_Crear_Empleado]'+''''+CAST(@LoggedUserOid AS NVARCHAR(36))+','''+CAST(@Oid AS NVARCHAR(36))+','''+@RUT+','''+','''
	+@FirstName+','''+@LastName+','''++','''+@Correo+','''++','''+@Telefono+','''++','''+CAST(@ManejaCasino AS NVARCHAR(5))+','''+CAST(@EnrollID AS NVARCHAR)+','''+@Contraseña+''''

	DECLARE @OidAudit UNIQUEIDENTIFIER = NEWID()


	IF CHARINDEX('.', @RUT) > 0 OR CHARINDEX('-', @RUT) < 0
	BEGIN
		SET @Error = 'El RUT tiene un formato invalido, ver registro :'+CAST(@OidAudit AS NVARCHAR(36))
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Empleado', @Error,@Query,@OidAudit
		RETURN
	END

	DECLARE @Count INT = (SELECT COUNT(1) FROM ESA_Empleado WHERE RUT = @RUT)
	IF @Count > 0
	BEGIN
		SET @Error = 'El trabajador ya existe en la BD, ver registro :'+CAST(@OidAudit AS NVARCHAR(36))
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error,@Query,@OidAudit
		RETURN
	END
	
	/*
	SET @Count = (SELECT COUNT(1) FROM EXT_Trabajadores WHERE Oid = @Oid)
	WHILE(@Count > 0)
	BEGIN
		IF NOT EXISTS(SELECT NULL FROM EXT_Trabajadores WHERE Oid = @Oid)
		BEGIN
			SET @Oid = NEWID()
			SET @Count = (SELECT COUNT(1) FROM EXT_Trabajadores WHERE Oid = @Oid)
		END
		ELSE
		BEGIN
			SET @Error = 'Ya existe un empleado con este Oid, ver registro :'+CAST(@OidAudit AS NVARCHAR(36))
			EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error,@Query,@OidAudit
			RETURN
		END
	END
	*/

	EXEC @Count= CREATE_EMPLEADO @RUT, @FirstName, @LastName, @Correo, @Telefono, @ManejaCasino, @EnrollID, @Contraseña, @Oid
	IF @Count = 0
	BEGIN
		SET @Error = 'No se pudo insertar el empleado en la BD'		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error
		RETURN
	END

	EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error
	RETURN
END
