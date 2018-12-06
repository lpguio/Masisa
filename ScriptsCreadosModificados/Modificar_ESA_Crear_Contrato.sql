ALTER PROCEDURE [dbo].[ESA_Crear_Contrato]
	@LoggedUserOid uniqueidentifier,
	@Oid uniqueidentifier,
	@Empleado uniqueidentifier,	
	@Empresa uniqueidentifier,
	@Cuenta uniqueidentifier,
	@Cargo uniqueidentifier,
	@InicioVigencia DATETIME,
	@FinVigencia DATETIME = NULL,
	@CodigoContrato varchar(100),
	@Error NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Query NVARCHAR(Max)=' EXEC [ESA_Crear_Contrato]'+''''+CAST(@LoggedUserOid AS NVARCHAR(36))+','''+CAST(@Oid AS NVARCHAR(36))+','''+CAST(@Empleado AS NVARCHAR(36))+','''
	+CAST(@Empresa AS NVARCHAR(36))+','''+CAST(@Cuenta AS NVARCHAR(36))+','''+CAST(@Cargo AS NVARCHAR(36))+','''+CONVERT(NVARCHAR(10), @InicioVigencia,112)+''''+','''+CASE WHEN @FINVIGENCIA IS NOT NULL THEN CONVERT(NVARCHAR(10), @FinVigencia,112) END+''''
	+ ',' + '''' + @CodigoContrato + ''''

	DECLARE @OidAudit UNIQUEIDENTIFIER = NEWID()

	DECLARE @EnrollID INT = (SELECT TOP 1 EnrollId FROM ESA_Empleado WHERE Oid = @Empleado)
	IF @EnrollID IS NULL
	BEGIN
		SET @Error = 'No se encontró al trabajador en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error,@Query,@OidAudit
		RETURN
	END

	DECLARE @Count INT = (SELECT COUNT(1) FROM ESA_Empresa WHERE Oid = @Empresa AND Usuario = @LoggedUserOid)
	IF @Count <> 1
	BEGIN
		SET @Error = 'No se encontró la empresa en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error,@Query,@OidAudit
		RETURN
	END

	SET @Count = (SELECT COUNT(1) FROM ESA_Cuenta WHERE Oid = @Cuenta AND Usuario = @LoggedUserOid)
	IF @Count <> 1
	BEGIN
		SET @Error = 'No se encontró la cuenta en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error,@Query,@OidAudit
		RETURN
	END

	SET @Count = (SELECT COUNT(1) FROM ESA_Cargo WHERE Oid = @Cargo AND Usuario = @LoggedUserOid)
	IF @Count <> 1
	BEGIN
		SET @Error = 'No se encontró el cargo en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error,@Query,@OidAudit
		RETURN
	END

	SET @Count = (SELECT COUNT(1) FROM ESA_Contrato WHERE Oid = @Oid)
	WHILE(@Count > 0)
	BEGIN
		IF NOT EXISTS(SELECT NULL FROM ESA_Contrato WHERE OID= @Oid AND Empleado= @Empleado AND Empresa= @Empresa)
		BEGIN
			SET @Oid = NEWID()
			SET @Count = (SELECT COUNT(1) FROM ESA_Contrato WHERE Oid = @Oid)
		END
		ELSE
		BEGIN
			SET @Error = 'Ya existe el mismo contrato, ver registro :'+CAST(@OidAudit AS NVARCHAR(36))
			EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Autorizacion', @Error,@Query,@OidAudit
			RETURN
		END
	END


     INSERT INTO Contrato(Oid, Empleado, Empresa, Cuenta, Cargo, InicioVigencia, FinVigencia, OptimisticLockField, GCRecord, ObjectType, Supervisor, TipoContrato, Usuario, ConsideraColacion, EsSupervisor, Codigo)
     SELECT 
		@oid, @Empleado, @Empresa, @Cuenta, @Cargo, @InicioVigencia, @FinVigencia, 0 OptimisticLockField,NULL GCRecord,15 ObjectType,NULL Supervisor,0 TipoContrato,NULL Usuario,1 ConsideraColacion,0 EsSupervisor, @CodigoContrato
		 
	IF @@ROWCOUNT <> 1
	BEGIN
		SET @Error = 'No se pudo insertar el contrato en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error,@Query,@OidAudit
		RETURN
	END


	EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0, 'ESA_Crear_Contrato', @Error

	RETURN
END
