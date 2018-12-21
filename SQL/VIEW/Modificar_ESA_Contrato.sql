ALTER VIEW [dbo].[ESA_Contrato]
AS
SELECT    
	DISTINCT    
	Contrato.Oid, 
	Contrato.Empresa, 
	Contrato.Cuenta, 
	Contrato.Cargo, 
	Contrato.Empleado, 
	SecuritySystemUser.Oid AS Usuario, 
	ISNULL(Contrato.InicioVigencia, '19000101') InicioVigencia,
    Contrato.FinVigencia,
	Contrato.Codigo,
	Contrato.ConsideraColacion,
	Contrato.ConsideraCasino
-- SELECT *
FROM            
	RolesUserExtensionEnroladorStandAloneUserExtension_CuentaCuenta Rol_Cuenta
	JOIN Cuenta
		ON Rol_Cuenta.Cuenta = Cuenta.Oid
	JOIN Contrato
		ON Contrato.Cuenta = Cuenta.Oid
	JOIN Cargo
		ON Contrato.Cargo = Cargo.Oid
	JOIN Empleado
		ON Contrato.Empleado = Empleado.Oid
	JOIN BioCoreUserExtension
		ON Rol_Cuenta.EnroladorStandAloneUserExtension = BioCoreUserExtension.Oid
	JOIN SecuritySystemUser
		ON BioCoreUserExtension.BioCoreExtensibleUser = SecuritySystemUser.Oid
WHERE 
	Contrato.GCRecord IS NULL
--SELECT        
--	EXT_Trabajadores.Oid, 
--	EXT_Trabajadores.Empresa, 
--	EXT_Trabajadores.Cuenta, 
--	EXT_Trabajadores.Cargo, 
--	EXT_Trabajadores.Empleado, 
--	ESA_Usuario.Oid AS Usuario, 
--	EXT_Trabajadores.InicioVigencia, 
--    EXT_Trabajadores.FinVigencia
--FROM            
--	EXT_Trabajadores 
--	CROSS JOIN ESA_Usuario
--WHERE 
--	EXT_Trabajadores.GCRecord IS NULL
