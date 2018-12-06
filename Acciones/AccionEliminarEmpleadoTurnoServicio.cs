using Enrolador.DataAccessLayer;
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
    class AccionEliminarEmpleadoTurnoServicio : Accion
    {
        EmpleadoTurnoServicioCasino EmpleadoTurno { get; set; }

        public AccionEliminarEmpleadoTurnoServicio(EmpleadoTurnoServicioCasino empleadoTurno, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.EmpleadoTurno = empleadoTurno;
            Aplicar(parent);
        }

        protected AccionEliminarEmpleadoTurnoServicio(AccionEliminarEmpleadoTurnoServicio original)
            : base(original.responsable, original.fecha, original.oid)
        {
            this.EmpleadoTurno = original.EmpleadoTurno;
        }

        public override Accion Clonar()
        {
            return new AccionEliminarEmpleadoTurnoServicio(this);
        }

        public bool Editar(EmpleadoTurnoServicioCasino empleadoTurno, Form1 parent)
        {
            if (this.EmpleadoTurno.Empleado.Equals(empleadoTurno.Empleado) && this.EmpleadoTurno.TurnoServicio.Equals(empleadoTurno.TurnoServicio))
            {
                return false;
            }
            this.EmpleadoTurno = empleadoTurno;
            return true;
        }

        public override async Task Enviar()
        {
            //try
            //{
            //    string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionEliminarEmpleadoTurnoServicioCasinoAsync(EmpleadoTurno);
            //    if (!string.IsNullOrEmpty(error))
            //    {
            //        throw new Exception("Eliminando a servicio de casino a un empleado no procesado: " + error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Eliminando a servicio de casino a un empleado no procesado: " + ex.Message);
            //}
        }

        public override void Cancelar(Form1 parent)
        {
            var elemento = parent.ListEmpleadoTurnoServicioCasino.FirstOrDefault(p => (p.Empleado == EmpleadoTurno.Empleado) && (p.TurnoServicio == EmpleadoTurno.TurnoServicio));
            if (elemento != null)
            {
                parent.ListEmpleadoTurnoServicioCasino.Remove(elemento);
            }
        }

        public override void Aplicar(Form1 parent)
        {
            var elemento = parent.ListEmpleadoTurnoServicioCasino.FirstOrDefault(p => (p.Empleado == EmpleadoTurno.Empleado) && (p.TurnoServicio == EmpleadoTurno.TurnoServicio));
            if (elemento == null)
            {
                parent.ListEmpleadoTurnoServicioCasino.Add(EmpleadoTurno);
            }
        }
    }
}
