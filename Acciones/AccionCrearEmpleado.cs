using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enrolador.DataAccessLayer;

namespace EnroladorStandAlone
{
    [Serializable()]
    public class AccionCrearEmpleado : Accion
    {
        int enrollId;
        string rut;
        string contraseña;
        string firstName;
        string lastName;

        public string Correo { get; set; }
        public string NumeroTelefono { get; set; }
        public string RUT { get { return rut; } }
        public string Contraseña { get { return contraseña; } }

        public AccionCrearEmpleado(int enrollId, string rut, string contraseña, string firstName, string lastName, string Correo, string NumeroTelefono, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid()) {
            this.enrollId = enrollId;
            this.rut = rut;
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Correo = Correo;
            this.NumeroTelefono = NumeroTelefono;
            descripcion = string.Format("Crear empleado {0} {1} con RUT {2}{3}", firstName, lastName, rut, !string.IsNullOrEmpty(contraseña) ? " con contraseña" : "");

            Aplicar(parent);
        }

        protected AccionCrearEmpleado(AccionCrearEmpleado original)
            : base(original.responsable, original.fecha, original.oid)
        {
            enrollId = original.enrollId;
            rut = original.rut;
            contraseña = original.contraseña;
            firstName = original.firstName;
            lastName = original.lastName;
            Correo = original.Correo;
            NumeroTelefono = original.NumeroTelefono;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionCrearEmpleado(this);
        }

        public bool Editar(string contraseña, string firstName, string lastName, string Correo, string NumeroTelefono, Form1 parent) {
            if (((string.IsNullOrEmpty(this.contraseña) && string.IsNullOrEmpty(contraseña)) || this.contraseña.Equals(contraseña)) && this.firstName.Equals(firstName) && this.lastName.Equals(lastName)) {
                return false;
            }
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Correo = Correo;
            this.NumeroTelefono = NumeroTelefono;
            descripcion = string.Format("Crear empleado {0} {1} con RUT {2}{3}", firstName, lastName, rut, !string.IsNullOrEmpty(contraseña) ? " con contraseña" : "");

            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, Tuple<string, string, string, string>, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), new Tuple<string, string, string, string>(firstName, lastName, Correo, NumeroTelefono), parent.EmpleadoTable[oid].Item5);
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                POCOEmpleado Empleado = new POCOEmpleado() {
                    Oid = oid,
                    RUT = rut,
                    EnrollId = enrollId,
                    Nombres = firstName,
                    Apellidos = lastName,
                    Correo = Correo,
                    NumeroTelefono = NumeroTelefono,
                    Contraseña = contraseña
                };
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCrearEmpleadoYOtroDatosAsync(responsable, Empleado);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Empleado no creado: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Empleado no creado: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            parent.EmpleadoTable.Remove(oid);
            parent.EmpleadoRUTIndex.Remove(rut);
        }

        public override void Aplicar(Form1 parent)
        {
            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, Tuple<string, string, string, string>, Tuple<List<Guid>, List<Guid>, List<Guid>>>(enrollId, rut, !string.IsNullOrEmpty(contraseña), new Tuple<string, string, string, string>(firstName, lastName, Correo, NumeroTelefono), new Tuple<List<Guid>, List<Guid>, List<Guid>>(new List<Guid>(), new List<Guid>(), new List<Guid>()));
            parent.EmpleadoRUTIndex[rut] = oid;
        }
    }
}
