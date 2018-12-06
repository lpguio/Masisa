using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EnroladorStandAlone
{
    [Serializable()]
    class AccionActualizarHuella : Accion
    {
        Guid empleado;
        string data;
        public Guid Empleado { get { return empleado; } }

        public AccionActualizarHuella(Guid oldHuella, Guid empleado, string data, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, oldHuella)
        {
            this.data = data;
            this.empleado = empleado;
            descripcion = string.Format("Actualizar huella al empleado con RUT {0} para el dedo {1}", parent.EmpleadoTable[empleado].Item2, parent.HuellaTable[oid].GetDisplayName());

            Aplicar(parent);
        }

        protected AccionActualizarHuella(AccionActualizarHuella original)
            : base(original.responsable, original.fecha, original.oid)
        {
            data = original.data;
            empleado = original.empleado;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionActualizarHuella(this);
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionActualizarHuellaAsync(responsable, oid, data);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Huella no actualizada: " + error);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Huella no actualizada: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
        }

        public override void Aplicar(Form1 parent)
        {
        }
    }
}
