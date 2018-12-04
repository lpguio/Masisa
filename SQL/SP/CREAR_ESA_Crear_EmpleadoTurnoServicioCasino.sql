CREATE PROCEDURE [dbo].[ESA_Crear_EmpleadoTurnoServicioCasino]
	@LoggedUserOid uniqueidentifier,
	@Empleado uniqueidentifier,	
	@TurnoServicio uniqueidentifier,
	@Error NVARCHAR(MAX) OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @Query NVARCHAR(Max)=' EXEC [ESA_Crear_EmpleadoTurnoServicioCasino]'+''''+CAST(@LoggedUserOid AS NVARCHAR(36))+','''+CAST(@Empleado AS NVARCHAR(36))+''''+','''+CAST(@TurnoServicio AS NVARCHAR(36))+''''

	DECLARE @OidAudit UNIQUEIDENTIFIER = NEWID()

	DECLARE @EnrollID INT = (SELECT TOP 1 EnrollID FROM ESA_Empleado WHERE Oid = @Empleado)
	IF @EnrollID IS NULL
	BEGIN
		SET @Error = 'No se encontró al trabajador en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @Empleado, 0, 'ESA_Crear_EmpleadoTurnoServicioCasino', @Error,@Query,@OidAudit
		RETURN
	END

	DECLARE @Count INT = (SELECT COUNT(1) FROM TurnoServicio WHERE Oid = @TurnoServicio)
	IF @Count <> 1
	BEGIN
		SET @Error = 'No se encontró el turno de servicio en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @TurnoServicio, 0, 'ESA_Crear_EmpleadoTurnoServicioCasino', @Error,@Query,@OidAudit
		RETURN
	END


	INSERT INTO [dbo].[EmpleadoTurnoServicioCasino]
           ([Empleado]
           ,[TurnoServicio]
           ,[OptimisticLockField]
           ,[GCRecord]
           ,[ObjectType])
		 VALUES
			   (@Empleado
			   ,@TurnoServicio
			   ,0
			   ,NULL
			   ,10)--Puse aqui dispositivo porque no se han creado los objs con XPO. Hay que repensar esto

		 
	IF @@ROWCOUNT <> 1
	BEGIN
		SET @Error = 'No se pudo insertar el turno de servicio en la BD, ver registro :'+CAST(@OidAudit AS nvarchar(36))		
		EXEC ESA_Crear_Audit @LoggedUserOid, @TurnoServicio, 0, 'ESA_Crear_EmpleadoTurnoServicioCasino', @Error,@Query,@OidAudit
		RETURN
	END


	EXEC ESA_Crear_Audit @LoggedUserOid, @TurnoServicio, 0, 'ESA_Crear_EmpleadoTurnoServicioCasino', @Error

	RETURN
END

