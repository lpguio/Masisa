USE [SCCSPArauco]
GO
/****** Object:  StoredProcedure [dbo].[CREATE_EMPLEADO]    Script Date: 5/12/2018 11:49:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[CREATE_EMPLEADO]
	@RUT varchar(MAX),
	@FirstName varchar(MAX),
	@LastName varchar(MAX),
	@Correo varchar(MAX),
	@Telefono varchar(MAX),
	@EnrollID INT,
	@Contraseña NVARCHAR(MAX),
	@Oid2 NVARCHAR(36) = NULL,
	@Forzar BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE  @Oid UNIQUEIDENTIFIER = NULL

	IF @Oid2 IS NULL
	BEGIN
		SET @Oid = NEWID()
	END
	ELSE
	BEGIN
		SET @Oid= CONVERT(UNIQUEIDENTIFIER,@OID2)
	END

	IF @RUT = '' OR @RUT IS NULL
	BEGIN
		PRINT 'El parametro @RUT está vacio'
		RETURN
	END

	IF 	(SELECT COUNT(1) FROM [Empleado] INNER JOIN [Party] ON Empleado.Oid = Party.Oid WHERE LOWER(RUT) = LOWER(@RUT) AND GCRecord IS NULL) > 0 AND @Forzar = 0
	BEGIN
		PRINT 'El empleado ' + @RUT + ' ya existe'
		RETURN
	END

	IF(SELECT [dbo].[fn_get_digito_verificador](SUBSTRING(@RUT, 0, CHARINDEX('-', @RUT)))) <> RIGHT(@RUT,1)
	BEGIN
		PRINT 'Rut '+ @RUT+ ' no Es valido'
		RETURN 
	END

 	DECLARE @ObjectType INT = (SELECT OID  FROM XPObjectType WHERE TypeName =  'SCCS.Module.BusinessObjects.Empleados.EmpleadoSCCS')
	IF ISNULL(@OBJECTTYPE,0)=0
	BEGIN
		PRINT 'EmpleadoSCCS No encontrado'
		RETURN
	END
		
	DECLARE @Alias int
	SELECT @Alias = MAX(Alias) + 1 
	FROM 
		Empleado 
	WHERE
		Alias IS NOT NULL

	INSERT 
	INTO [dbo].[Party]
		([Oid]
		,[Photo]
		,[Address1]
		,[Address2]
		,[OptimisticLockField]
		,[GCRecord]
		,[ObjectType])
	VALUES
		(@OID
		,NULL
		,NULL
		,NULL
		,0
		,NULL
		,@ObjectType)

	IF @@ROWCOUNT = 0
	BEGIN
		PRINT 'Falló la creación del empleado ' + @RUT + 'en la tabla [Party]'
		RETURN
	END

	INSERT INTO [dbo].[Person]
	([Oid]
	,[FirstName]
	,[LastName]
	,[MiddleName]
	,[Birthday]
	,[Email]
	)
     VALUES
		(@OID
		,@FirstName
		,@LastName
		,null
		,null
		,@Correo)


	IF @@ROWCOUNT = 0
	BEGIN
		PRINT 'Falló la creación del empleado ' + @RUT + ' en la tabla [Person]'
		RETURN
	END

	INSERT INTO [dbo].[PhoneNumber]
	([Oid]
	,[Number]
	,[Party]
	,[PhoneType]
	,[OptimisticLockField]
	,[GCRecord])
     VALUES
		(@OID
		,@Telefono
		,null
		,null
		,null
		,null)

	IF @@ROWCOUNT = 0
	BEGIN
		PRINT 'Falló la creación del empleado ' + @RUT + ' en la tabla [PhoneNumber]'
		RETURN
	END

	INSERT
	INTO [dbo].[Empleado]
           ([Oid]
           ,[IdCheckpoint]
		   ,[EnrollID]
           ,[ConsiderarTipoMarca]
           ,[Alias]
           ,[RUT]
		   ,[Contraseña]
		   )
     VALUES
           (@OID
           ,@EnrollID
		   ,@EnrollID
           ,0
           ,@Alias
           ,@RUT
		   ,@Contraseña
		   )

	IF @@ROWCOUNT = 0
	BEGIN
		PRINT 'Falló la creación del empleado ' + @RUT + ' en la tabla [Empleado]'
		RETURN
	END

	PRINT 'Se creó el empleado ' + @RUT + ' en  el sistema'
	RETURN 100

END