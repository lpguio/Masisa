using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool ManejaCasino { get; set; }


        public string RUT { get { return rut; } }
        public string Contraseña { get { return contraseña; } }

        public AccionCrearEmpleado(int enrollId, string rut, string contraseña, string firstName, string lastName, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.enrollId = enrollId;
            this.rut = rut;
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            descripcion = string.Format("Crear empleado {0} {1} con RUT {2}{3}", firstName, lastName, rut, !string.IsNullOrEmpty(contraseña) ? " con contraseña" : "");

            Aplicar(parent);
        }

        public AccionCrearEmpleado(int enrollId, string rut, string contraseña, string firstName, string lastName, string Email, string Telefono, bool ManejaCasino, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid()) {
            this.enrollId = enrollId;
            this.rut = rut;
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Email = Email;
            this.Telefono = Telefono;
            this.ManejaCasino = ManejaCasino;
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
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionCrearEmpleado(this);
        }

        public bool Editar(string contraseña, string firstName, string lastName, Form1 parent)
        {
            if (((string.IsNullOrEmpty(this.contraseña) && string.IsNullOrEmpty(contraseña)) || this.contraseña.Equals(contraseña)) && this.firstName.Equals(firstName) && this.lastName.Equals(lastName))
            {
                return false;
            }
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            descripcion = string.Format("Crear empleado {0} {1} con RUT {2}{3}", firstName, lastName, rut, !string.IsNullOrEmpty(contraseña) ? " con contraseña" : "");

            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), firstName, lastName, parent.EmpleadoTable[oid].Item6);
            return true;
        }

        public bool Editar(string contraseña, string firstName, string lastName, string Email, string Telefono, bool ManejaCasino, Form1 parent) {
            if (((string.IsNullOrEmpty(this.contraseña) && string.IsNullOrEmpty(contraseña)) || this.contraseña.Equals(contraseña)) && this.firstName.Equals(firstName) && this.lastName.Equals(lastName)) {
                return false;
            }
            this.contraseña = contraseña;
            this.firstName = firstName;
            this.lastName = lastName;
            this.Email = Email;
            this.Telefono = Telefono;
            this.ManejaCasino = ManejaCasino;
            descripcion = string.Format("Crear empleado {0} {1} con RUT {2}{3}", firstName, lastName, rut, !string.IsNullOrEmpty(contraseña) ? " con contraseña" : "");

            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), firstName, lastName, parent.EmpleadoTable[oid].Item6);
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCrearEmpleadoAsync(responsable, oid, rut, firstName, lastName, enrollId, contraseña);
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
            //ACCM
            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>(enrollId, rut, !string.IsNullOrEmpty(contraseña), firstName, lastName, new Tuple<List<Guid>, List<Guid>, List<Guid>>(new List<Guid>(), new List<Guid>(), new List<Guid>()));
            parent.EmpleadoRUTIndex[rut] = oid;
        }
    }
}
