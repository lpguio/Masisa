USE [SCCSPArauco]
GO
/****** Object:  StoredProcedure [dbo].[ESA_Crear_Empleado]    Script Date: 12/7/2018 9:01:14 PM ******/
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
       @EnrollID INT,
       @Contraseña NVARCHAR(MAX),
       @Error NVARCHAR(MAX) OUTPUT
AS
BEGIN
       SET NOCOUNT ON;

       IF NOT EXISTS(SELECT * FROM ESA_Empleado WHERE RUT = @RUT) --Lo inserto porque no existe
       BEGIN
                     DECLARE @Query NVARCHAR(Max)=' EXEC [ESA_Crear_Empleado]'+''''+CAST(@LoggedUserOid AS NVARCHAR(36))+','''+CAST(@Oid AS NVARCHAR(36))+','''+@RUT+','''+','''
              +@FirstName+','''+@LastName+','''++','''+@Correo+','''++','''+@Telefono+','''+','''+CAST(@EnrollID AS NVARCHAR)+','''+@Contraseña+''''

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

                     EXEC @Count= CREATE_EMPLEADO @RUT, @FirstName, @LastName, @Correo, @Telefono, @EnrollID, @Contraseña, @Oid
                     IF @Count = 0
                     BEGIN
                           SET @Error = 'No se pudo insertar el empleado en la BD'              
                           EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error
                           RETURN
                     END

                     EXEC ESA_Crear_Audit @LoggedUserOid, @Oid, 0,'ESA_Crear_Empleado', @Error
                     RETURN
       END
       ELSE
       BEGIN --Si existe actualizo...
              DECLARE @myGUIForUpdate uniqueidentifier
              SET @myGUIForUpdate = (SELECT TOP 1 Oid FROM ESA_Empleado WHERE RUT = @RUT)
              
              --Aqui actualizamos primero en persona
              UPDATE Person
              SET Email = @Correo
              WHERE Oid = @myGUIForUpdate

              --Ahora los telefonos
              DECLARE @myPhoneNumberForUpdate uniqueidentifier
              SET @myPhoneNumberForUpdate = (SELECT TOP 1 Oid FROM PhoneNumber WHERE Party = @myGUIForUpdate)

              IF (@myPhoneNumberForUpdate IS NULL)
              BEGIN
                     --No existe ni en la tabla de números. Hay que crearlos.

                     INSERT INTO [dbo].[PhoneNumber]
                              ([Oid]
                              ,[Number]
                              ,[Party]
                              ,[PhoneType]
                              ,[OptimisticLockField]
                              ,[GCRecord])
                     VALUES
                              (NEWID() 
                              ,@Telefono
                              ,@myGUIForUpdate
                              ,'Telefono personal'
                              ,0
                              ,NULL)
              END
              ELSE
              BEGIN
                     --Existe por lo que actualizamos

                     UPDATE [dbo].[PhoneNumber]
                        SET [Number] = @Telefono
                     WHERE Oid = @myPhoneNumberForUpdate

              END
       END
END
