using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EnroladorWebServices
{
    [ServiceContract]
    public interface IEnroladorWebServices
    {
        [OperationContract]
        string AccionActualizarHuella(Guid responsable, Guid oid, string data);

        [OperationContract]
        string AccionCaducarContrato(Guid responsable, Guid oid, DateTime? finVigencia);

        [OperationContract]
        string AccionCrearAsignacion(Guid responsable, Guid oid, Guid empleado, Guid dispositivo);

        [OperationContract]
        string AccionCrearContrato(Guid responsable, Guid oid, Guid empleado, Guid empresa, Guid cuenta, Guid cargo, DateTime inicioVigencia, DateTime? finVigencia, string CodigoContrato);

        [OperationContract]
        string AccionCrearEmpleado(Guid responsable, Guid oid, string RUT, string firstName, string lastName, int enrollID, string contraseña);

        [OperationContract]
        string AccionCrearHuella(Guid responsable, Guid oid, Guid empleado, int tipoHuella, string data);

        [OperationContract]
        string AccionModificarContraseña(Guid responsable, Guid oid, string contraseña);

        [OperationContract]
        bool Identificar(Guid HWID);

        [OperationContract]
        Guid? Login(string user, string pass);

        [OperationContract]
        Tuple<string,string> Revalidar(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid, int, string>> LeeHuellaUser(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid, string>> LeeCadena(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,string,Guid>> LeeInstalacion(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,string,string,int,Guid>> LeeDispositivo(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,string>> LeeEmpresa(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,string,Guid>> LeeCargo(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,string,Guid,DateTime?>> LeeCuenta(Guid loggedUser);

        [OperationContract]
        List<Tuple<Guid,int,string,bool,string,string>> LeeEmpleado();

        [OperationContract]
        List<Tuple<Guid,int,Guid>> LeeHuella();

        [OperationContract]
        List<Tuple<Guid,Guid>> LeeEmpleadosDispositivos();

        [OperationContract]
        List<Tuple<Guid,Guid,Guid,Guid,DateTime,DateTime?,Guid, Tuple<string>>> LeeContrato(Guid loggedUser);

    }
}
