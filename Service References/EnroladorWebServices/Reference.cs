﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace EnroladorStandAlone.EnroladorWebServices {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="EnroladorWebServices.IEnroladorWebServices")]
    public interface IEnroladorWebServices {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionActualizarHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionActualizarHuellaResponse")]
        string AccionActualizarHuella(System.Guid responsable, System.Guid oid, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionActualizarHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionActualizarHuellaResponse")]
        System.Threading.Tasks.Task<string> AccionActualizarHuellaAsync(System.Guid responsable, System.Guid oid, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCaducarContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCaducarContratoResponse")]
        string AccionCaducarContrato(System.Guid responsable, System.Guid oid, System.Nullable<System.DateTime> finVigencia);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCaducarContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCaducarContratoResponse")]
        System.Threading.Tasks.Task<string> AccionCaducarContratoAsync(System.Guid responsable, System.Guid oid, System.Nullable<System.DateTime> finVigencia);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearAsignacion", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearAsignacionResponse")]
        string AccionCrearAsignacion(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid dispositivo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearAsignacion", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearAsignacionResponse")]
        System.Threading.Tasks.Task<string> AccionCrearAsignacionAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid dispositivo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearContratoResponse")]
        string AccionCrearContrato(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid empresa, System.Guid cuenta, System.Guid cargo, System.DateTime inicioVigencia, System.Nullable<System.DateTime> finVigencia, string CodigoContrato);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearContratoResponse")]
        System.Threading.Tasks.Task<string> AccionCrearContratoAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid empresa, System.Guid cuenta, System.Guid cargo, System.DateTime inicioVigencia, System.Nullable<System.DateTime> finVigencia, string CodigoContrato);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearEmpleado", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearEmpleadoResponse")]
        string AccionCrearEmpleado(System.Guid responsable, System.Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearEmpleado", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearEmpleadoResponse")]
        System.Threading.Tasks.Task<string> AccionCrearEmpleadoAsync(System.Guid responsable, System.Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearHuellaResponse")]
        string AccionCrearHuella(System.Guid responsable, System.Guid oid, System.Guid empleado, int tipoHuella, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionCrearHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionCrearHuellaResponse")]
        System.Threading.Tasks.Task<string> AccionCrearHuellaAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, int tipoHuella, string data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionModificarContraseña", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionModificarContraseñaResponse")]
        string AccionModificarContraseña(System.Guid responsable, System.Guid oid, string contraseña);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/AccionModificarContraseña", ReplyAction="http://tempuri.org/IEnroladorWebServices/AccionModificarContraseñaResponse")]
        System.Threading.Tasks.Task<string> AccionModificarContraseñaAsync(System.Guid responsable, System.Guid oid, string contraseña);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Identificar", ReplyAction="http://tempuri.org/IEnroladorWebServices/IdentificarResponse")]
        bool Identificar(System.Guid HWID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Identificar", ReplyAction="http://tempuri.org/IEnroladorWebServices/IdentificarResponse")]
        System.Threading.Tasks.Task<bool> IdentificarAsync(System.Guid HWID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Login", ReplyAction="http://tempuri.org/IEnroladorWebServices/LoginResponse")]
        System.Nullable<System.Guid> Login(string user, string pass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Login", ReplyAction="http://tempuri.org/IEnroladorWebServices/LoginResponse")]
        System.Threading.Tasks.Task<System.Nullable<System.Guid>> LoginAsync(string user, string pass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Revalidar", ReplyAction="http://tempuri.org/IEnroladorWebServices/RevalidarResponse")]
        System.Tuple<string, string> Revalidar(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/Revalidar", ReplyAction="http://tempuri.org/IEnroladorWebServices/RevalidarResponse")]
        System.Threading.Tasks.Task<System.Tuple<string, string>> RevalidarAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeHuellaUser", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeHuellaUserResponse")]
        System.Tuple<System.Guid, int, string>[] LeeHuellaUser(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeHuellaUser", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeHuellaUserResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, int, string>[]> LeeHuellaUserAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCadena", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCadenaResponse")]
        System.Tuple<System.Guid, string>[] LeeCadena(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCadena", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCadenaResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string>[]> LeeCadenaAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeInstalacion", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeInstalacionResponse")]
        System.Tuple<System.Guid, string, System.Guid>[] LeeInstalacion(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeInstalacion", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeInstalacionResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid>[]> LeeInstalacionAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeDispositivo", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeDispositivoResponse")]
        System.Tuple<System.Guid, string, string, int, System.Guid>[] LeeDispositivo(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeDispositivo", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeDispositivoResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string, string, int, System.Guid>[]> LeeDispositivoAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpresa", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpresaResponse")]
        System.Tuple<System.Guid, string>[] LeeEmpresa(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpresa", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpresaResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string>[]> LeeEmpresaAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCargo", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCargoResponse")]
        System.Tuple<System.Guid, string, System.Guid>[] LeeCargo(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCargo", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCargoResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid>[]> LeeCargoAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCuenta", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCuentaResponse")]
        System.Tuple<System.Guid, string, System.Guid, System.DateTime?>[] LeeCuenta(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeCuenta", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeCuentaResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid, System.DateTime?>[]> LeeCuentaAsync(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpleado", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpleadoResponse")]
        System.Tuple<System.Guid, int, string, bool, string, string>[] LeeEmpleado();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpleado", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpleadoResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, int, string, bool, string, string>[]> LeeEmpleadoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeHuellaResponse")]
        System.Tuple<System.Guid, int, System.Guid>[] LeeHuella();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeHuella", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeHuellaResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, int, System.Guid>[]> LeeHuellaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpleadosDispositivos", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpleadosDispositivosResponse")]
        System.Tuple<System.Guid, System.Guid>[] LeeEmpleadosDispositivos();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeEmpleadosDispositivos", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeEmpleadosDispositivosResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, System.Guid>[]> LeeEmpleadosDispositivosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeContratoResponse")]
        System.Tuple<System.Guid, System.Guid, System.Guid, System.Guid, System.DateTime, System.Nullable<System.DateTime>, System.Guid, System.Tuple<string>>[] LeeContrato(System.Guid loggedUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEnroladorWebServices/LeeContrato", ReplyAction="http://tempuri.org/IEnroladorWebServices/LeeContratoResponse")]
        System.Threading.Tasks.Task<System.Tuple<System.Guid, System.Guid, System.Guid, System.Guid, System.DateTime, System.Nullable<System.DateTime>, System.Guid, System.Tuple<string>>[]> LeeContratoAsync(System.Guid loggedUser);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEnroladorWebServicesChannel : EnroladorStandAlone.EnroladorWebServices.IEnroladorWebServices, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EnroladorWebServicesClient : System.ServiceModel.ClientBase<EnroladorStandAlone.EnroladorWebServices.IEnroladorWebServices>, EnroladorStandAlone.EnroladorWebServices.IEnroladorWebServices {
        
        public EnroladorWebServicesClient() {
        }
        
        public EnroladorWebServicesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public EnroladorWebServicesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EnroladorWebServicesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EnroladorWebServicesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AccionActualizarHuella(System.Guid responsable, System.Guid oid, string data) {
            return base.Channel.AccionActualizarHuella(responsable, oid, data);
        }
        
        public System.Threading.Tasks.Task<string> AccionActualizarHuellaAsync(System.Guid responsable, System.Guid oid, string data) {
            return base.Channel.AccionActualizarHuellaAsync(responsable, oid, data);
        }
        
        public string AccionCaducarContrato(System.Guid responsable, System.Guid oid, System.Nullable<System.DateTime> finVigencia) {
            return base.Channel.AccionCaducarContrato(responsable, oid, finVigencia);
        }
        
        public System.Threading.Tasks.Task<string> AccionCaducarContratoAsync(System.Guid responsable, System.Guid oid, System.Nullable<System.DateTime> finVigencia) {
            return base.Channel.AccionCaducarContratoAsync(responsable, oid, finVigencia);
        }
        
        public string AccionCrearAsignacion(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid dispositivo) {
            return base.Channel.AccionCrearAsignacion(responsable, oid, empleado, dispositivo);
        }
        
        public System.Threading.Tasks.Task<string> AccionCrearAsignacionAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid dispositivo) {
            return base.Channel.AccionCrearAsignacionAsync(responsable, oid, empleado, dispositivo);
        }
        
        public string AccionCrearContrato(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid empresa, System.Guid cuenta, System.Guid cargo, System.DateTime inicioVigencia, System.Nullable<System.DateTime> finVigencia, string CodigoContrato) {
            return base.Channel.AccionCrearContrato(responsable, oid, empleado, empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato);
        }
        
        public System.Threading.Tasks.Task<string> AccionCrearContratoAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, System.Guid empresa, System.Guid cuenta, System.Guid cargo, System.DateTime inicioVigencia, System.Nullable<System.DateTime> finVigencia, string CodigoContrato) {
            return base.Channel.AccionCrearContratoAsync(responsable, oid, empleado, empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato);
        }
        
        public string AccionCrearEmpleado(System.Guid responsable, System.Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña) {
            return base.Channel.AccionCrearEmpleado(responsable, oid, RUT, firstName, lastName, enrollID, contraseña);
        }
        
        public System.Threading.Tasks.Task<string> AccionCrearEmpleadoAsync(System.Guid responsable, System.Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña) {
            return base.Channel.AccionCrearEmpleadoAsync(responsable, oid, RUT, firstName, lastName, enrollID, contraseña);
        }
        
        public string AccionCrearHuella(System.Guid responsable, System.Guid oid, System.Guid empleado, int tipoHuella, string data) {
            return base.Channel.AccionCrearHuella(responsable, oid, empleado, tipoHuella, data);
        }
        
        public System.Threading.Tasks.Task<string> AccionCrearHuellaAsync(System.Guid responsable, System.Guid oid, System.Guid empleado, int tipoHuella, string data) {
            return base.Channel.AccionCrearHuellaAsync(responsable, oid, empleado, tipoHuella, data);
        }
        
        public string AccionModificarContraseña(System.Guid responsable, System.Guid oid, string contraseña) {
            return base.Channel.AccionModificarContraseña(responsable, oid, contraseña);
        }
        
        public System.Threading.Tasks.Task<string> AccionModificarContraseñaAsync(System.Guid responsable, System.Guid oid, string contraseña) {
            return base.Channel.AccionModificarContraseñaAsync(responsable, oid, contraseña);
        }
        
        public bool Identificar(System.Guid HWID) {
            return base.Channel.Identificar(HWID);
        }
        
        public System.Threading.Tasks.Task<bool> IdentificarAsync(System.Guid HWID) {
            return base.Channel.IdentificarAsync(HWID);
        }
        
        public System.Nullable<System.Guid> Login(string user, string pass) {
            return base.Channel.Login(user, pass);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<System.Guid>> LoginAsync(string user, string pass) {
            return base.Channel.LoginAsync(user, pass);
        }
        
        public System.Tuple<string, string> Revalidar(System.Guid loggedUser) {
            return base.Channel.Revalidar(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<string, string>> RevalidarAsync(System.Guid loggedUser) {
            return base.Channel.RevalidarAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, int, string>[] LeeHuellaUser(System.Guid loggedUser) {
            return base.Channel.LeeHuellaUser(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, int, string>[]> LeeHuellaUserAsync(System.Guid loggedUser) {
            return base.Channel.LeeHuellaUserAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string>[] LeeCadena(System.Guid loggedUser) {
            return base.Channel.LeeCadena(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string>[]> LeeCadenaAsync(System.Guid loggedUser) {
            return base.Channel.LeeCadenaAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string, System.Guid>[] LeeInstalacion(System.Guid loggedUser) {
            return base.Channel.LeeInstalacion(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid>[]> LeeInstalacionAsync(System.Guid loggedUser) {
            return base.Channel.LeeInstalacionAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string, string, int, System.Guid>[] LeeDispositivo(System.Guid loggedUser) {
            return base.Channel.LeeDispositivo(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string, string, int, System.Guid>[]> LeeDispositivoAsync(System.Guid loggedUser) {
            return base.Channel.LeeDispositivoAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string>[] LeeEmpresa(System.Guid loggedUser) {
            return base.Channel.LeeEmpresa(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string>[]> LeeEmpresaAsync(System.Guid loggedUser) {
            return base.Channel.LeeEmpresaAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string, System.Guid>[] LeeCargo(System.Guid loggedUser) {
            return base.Channel.LeeCargo(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid>[]> LeeCargoAsync(System.Guid loggedUser) {
            return base.Channel.LeeCargoAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, string, System.Guid, System.DateTime?>[] LeeCuenta(System.Guid loggedUser) {
            return base.Channel.LeeCuenta(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, string, System.Guid, System.DateTime?>[]> LeeCuentaAsync(System.Guid loggedUser) {
            return base.Channel.LeeCuentaAsync(loggedUser);
        }
        
        public System.Tuple<System.Guid, int, string, bool, string, string>[] LeeEmpleado() {
            return base.Channel.LeeEmpleado();
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, int, string, bool, string, string>[]> LeeEmpleadoAsync() {
            return base.Channel.LeeEmpleadoAsync();
        }
        
        public System.Tuple<System.Guid, int, System.Guid>[] LeeHuella() {
            return base.Channel.LeeHuella();
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, int, System.Guid>[]> LeeHuellaAsync() {
            return base.Channel.LeeHuellaAsync();
        }
        
        public System.Tuple<System.Guid, System.Guid>[] LeeEmpleadosDispositivos() {
            return base.Channel.LeeEmpleadosDispositivos();
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, System.Guid>[]> LeeEmpleadosDispositivosAsync() {
            return base.Channel.LeeEmpleadosDispositivosAsync();
        }
        
        public System.Tuple<System.Guid, System.Guid, System.Guid, System.Guid, System.DateTime, System.Nullable<System.DateTime>, System.Guid, System.Tuple<string>>[] LeeContrato(System.Guid loggedUser) {
            return base.Channel.LeeContrato(loggedUser);
        }
        
        public System.Threading.Tasks.Task<System.Tuple<System.Guid, System.Guid, System.Guid, System.Guid, System.DateTime, System.Nullable<System.DateTime>, System.Guid, System.Tuple<string>>[]> LeeContratoAsync(System.Guid loggedUser) {
            return base.Channel.LeeContratoAsync(loggedUser);
        }
    }
}
