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
    class AccionModificarContraseña : Accion
    {
        bool teniaContraseña;
        string contraseña;
        public bool TeniaContraseña { get { return teniaContraseña; } }
        public string Contraseña { get { return contraseña; } }

        public AccionModificarContraseña(Guid empleado, string contraseña, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, empleado)
        {
            this.contraseña = contraseña;
            teniaContraseña = parent.EmpleadoTable[empleado].Item3;
            if (string.IsNullOrEmpty(contraseña))
            {
                descripcion = string.Format("Eliminar contraseña al empleado con RUT {0}", parent.EmpleadoTable[empleado].Item2);
            }
            else
            {
                descripcion = string.Format("Crear contraseña al empleado con RUT {0}", parent.EmpleadoTable[empleado].Item2);
            }

            Aplicar(parent);
        }

        protected AccionModificarContraseña(AccionModificarContraseña original)
            : base(original.responsable, original.fecha, original.oid)
        {
            teniaContraseña = original.teniaContraseña;
            contraseña = original.contraseña;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionModificarContraseña(this);
        }

        public bool Editar(string contraseña, Form1 parent)
        {
            if ((string.IsNullOrEmpty(this.contraseña) && string.IsNullOrEmpty(contraseña)) || this.contraseña.Equals(contraseña))
            {
                return false;
            }
            this.contraseña = contraseña;
            if (string.IsNullOrEmpty(contraseña))
            {
                descripcion = string.Format("Eliminar contraseña al empleado con RUT {0}", parent.EmpleadoTable[oid].Item2);
            }
            else
            {
                descripcion = string.Format("Crear contraseña al empleado con RUT {0}", parent.EmpleadoTable[oid].Item2);
            }

            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, Tuple<string, string, string, string, bool>, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), new Tuple<string, string, string, string, bool>(parent.EmpleadoTable[oid].Item4.Item1, parent.EmpleadoTable[oid].Item4.Item2, parent.EmpleadoTable[oid].Item4.Item3, parent.EmpleadoTable[oid].Item4.Item4, parent.EmpleadoTable[oid].Item4.Item5), parent.EmpleadoTable[oid].Item5);
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionModificarContraseñaAsync(responsable, oid, contraseña);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Contraseña no modificada: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Contraseña no modificada: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            //parent.EmpleadoTable[oid] = new Tuple<int, string, bool, string, string, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, teniaContraseña, parent.EmpleadoTable[oid].Item4, parent.EmpleadoTable[oid].Item5, parent.EmpleadoTable[oid].Item6);
            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, Tuple<string, string, string, string, bool>, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), new Tuple<string, string, string, string, bool>(parent.EmpleadoTable[oid].Item4.Item1, parent.EmpleadoTable[oid].Item4.Item2, parent.EmpleadoTable[oid].Item4.Item3, parent.EmpleadoTable[oid].Item4.Item4, parent.EmpleadoTable[oid].Item4.Item5), parent.EmpleadoTable[oid].Item5);

        }

        public override void Aplicar(Form1 parent)
        {
            parent.EmpleadoTable[oid] = new Tuple<int, string, bool, Tuple<string, string, string, string, bool>, Tuple<List<Guid>, List<Guid>, List<Guid>>>(parent.EmpleadoTable[oid].Item1, parent.EmpleadoTable[oid].Item2, !string.IsNullOrEmpty(contraseña), new Tuple<string, string, string, string, bool>(parent.EmpleadoTable[oid].Item4.Item1, parent.EmpleadoTable[oid].Item4.Item2, parent.EmpleadoTable[oid].Item4.Item3, parent.EmpleadoTable[oid].Item4.Item4, parent.EmpleadoTable[oid].Item4.Item5), parent.EmpleadoTable[oid].Item5);
        }
    }
}
