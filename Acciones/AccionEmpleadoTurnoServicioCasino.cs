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
    class AccionEmpleadoTurnoServicioCasino : Accion
    {
        public EmpleadoTurnoServicioCasino EmpleadoTurno { get; set; }

        public AccionEmpleadoTurnoServicioCasino(EmpleadoTurnoServicioCasino empleadoTurno, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.EmpleadoTurno = empleadoTurno;
            Aplicar(parent);
        }

        protected AccionEmpleadoTurnoServicioCasino(AccionEmpleadoTurnoServicioCasino original)
            : base(original.responsable, original.fecha, original.oid)
        {
            this.EmpleadoTurno = original.EmpleadoTurno;
        }

        public override Accion Clonar()
        {
            return new AccionEmpleadoTurnoServicioCasino(this);
        }

        public bool Editar(EmpleadoTurnoServicioCasino empleadoTurno, Form1 parent)
        {
            if (this.EmpleadoTurno.Empleado.Equals(empleadoTurno.Empleado) && this.EmpleadoTurno.TurnoServicio.Equals(empleadoTurno.TurnoServicio))
            {
                return false;
            }
            this.EmpleadoTurno = empleadoTurno;
            //descripcion = string.Format("Crear contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3}. Inicio de vigencia {4}{5}. Codigo Contrato {6}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[empresa].Item1, parent.CuentaTable[cuenta], parent.CargoTable[cargo], inicioVigencia.ToString("dd/MM/yyyy"), finVigencia.HasValue ? ". Fin de vigencia " + finVigencia.Value.ToString("dd/MM/yyyy") : "", CodigoContrato);

            //parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?, string>(empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato);
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionInsertarEmpleadoTurnoServicioCasinoAsync(EmpleadoTurno, Oid);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Empleado adicionado a servicio de casino no creado: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Empleado adicionado a servicio de casino no creado: " + ex.Message);
            }
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
