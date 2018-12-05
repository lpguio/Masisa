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
    class AccionCrearAsignacion : Accion
    {
        Guid empleado;
        Guid dispositivo;
        public Guid Empleado { get { return empleado; } }

        public AccionCrearAsignacion(Guid empleado, Guid dispositivo, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.empleado = empleado;
            this.dispositivo = dispositivo;
            descripcion = string.Format("Crear asignación al empleado con RUT {0} para el dispositivo {1}", parent.EmpleadoTable[empleado].Item2, parent.DispositivoTable[dispositivo].Item1);

            Aplicar(parent);
        }

        protected AccionCrearAsignacion(AccionCrearAsignacion original)
            : base(original.responsable, original.fecha, original.oid)
        {
            empleado = original.empleado;
            dispositivo = original.dispositivo;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionCrearAsignacion(this);
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCrearAsignacionAsync(responsable, oid, empleado, dispositivo);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Asignación no creada: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Asignación no creada: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado))
            {
                parent.EmpleadoTable[empleado].Item5.Item2.Remove(dispositivo);
            }
            if (parent.DispositivoTable.ContainsKey(dispositivo))
            {
                parent.DispositivoTable[dispositivo].Item5.Remove(empleado);
            }
        }

        public override void Aplicar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado) && parent.DispositivoTable.ContainsKey(dispositivo))
            {
                if (!parent.EmpleadoTable[empleado].Item5.Item2.Contains(dispositivo))
                {
                    parent.EmpleadoTable[empleado].Item5.Item2.Add(dispositivo);
                }
                if (!parent.DispositivoTable[dispositivo].Item5.Contains(empleado))
                {
                    parent.DispositivoTable[dispositivo].Item5.Add(empleado);
                }
            }
        }
    }
}
