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
    class AccionCrearHuella : Accion
    {
        Guid empleado;
        TipoHuella tipoHuella;
        string data;
        public Guid Empleado { get { return empleado; } }

        public AccionCrearHuella(Guid empleado, TipoHuella tipoHuella, string data, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.empleado = empleado;
            this.tipoHuella = tipoHuella;
            this.data = data;
            descripcion = string.Format("Crear huella al empleado con RUT {0} para el dedo {1}", parent.EmpleadoTable[empleado].Item2, tipoHuella.GetDisplayName());

            Aplicar(parent);
        }

        protected AccionCrearHuella(AccionCrearHuella original)
            : base(original.responsable, original.fecha, original.oid)
        {
            empleado = original.empleado;
            tipoHuella = original.tipoHuella;
            data = original.data;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionCrearHuella(this);
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCrearHuellaAsync(responsable, oid, empleado, (int)tipoHuella, data);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Huella no creada: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Huella no creada: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado))
            {
                parent.EmpleadoTable[empleado].Item5.Item1.Remove(oid);
            }
            parent.HuellaTable.Remove(oid);
        }

        public override void Aplicar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado))
            {
                if (!parent.EmpleadoTable[empleado].Item5.Item1.Contains(oid))
                {
                    parent.EmpleadoTable[empleado].Item5.Item1.Add(oid);
                }
                parent.HuellaTable[oid] = tipoHuella;
            }
        }
    }
}
