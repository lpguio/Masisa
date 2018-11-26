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
    class AccionCaducarContrato : Accion
    {
        DateTime? finVigencia;
        DateTime? oldFinVigencia;
        Guid empleado;
        public DateTime? FinVigencia { get { return finVigencia; } }
        public Guid Empleado { get { return empleado; } }

        public AccionCaducarContrato(Guid contrato, Guid empleado, DateTime? finVigencia, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, contrato)
        {
            this.empleado = empleado;
            this.finVigencia = finVigencia;
            oldFinVigencia = parent.ContratoTable[contrato].Item5;
            if (finVigencia.HasValue)
            {
                descripcion = string.Format("Caducar contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3} con fin de vigencia {4}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[parent.ContratoTable[contrato].Item1].Item1, parent.CuentaTable[parent.ContratoTable[contrato].Item2], parent.CargoTable[parent.ContratoTable[contrato].Item3], finVigencia.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                descripcion = string.Format("Eliminar caducidad de contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[parent.ContratoTable[contrato].Item1].Item1, parent.CuentaTable[parent.ContratoTable[contrato].Item2], parent.CargoTable[parent.ContratoTable[contrato].Item3]);
            }

            Aplicar(parent);
        }

        protected AccionCaducarContrato(AccionCaducarContrato original)
            : base(original.responsable, original.fecha, original.oid)
        {
            finVigencia = original.finVigencia;
            oldFinVigencia = original.oldFinVigencia;
            empleado = original.empleado;
            descripcion = original.descripcion;
        }

        public override Accion Clonar()
        {
            return new AccionCaducarContrato(this);
        }

        public bool Editar(DateTime? finVigencia, Form1 parent)
        {
            if (this.finVigencia.Equals(finVigencia))
            {
                return false;
            }
            this.finVigencia = finVigencia;
            if (finVigencia.HasValue)
            {
                descripcion = string.Format("Caducar contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3} con fin de vigencia {4}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[parent.ContratoTable[oid].Item1].Item1, parent.CuentaTable[parent.ContratoTable[oid].Item2], parent.CargoTable[parent.ContratoTable[oid].Item3], finVigencia.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                descripcion = string.Format("Eliminar caducidad de contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[parent.ContratoTable[oid].Item1].Item1, parent.CuentaTable[parent.ContratoTable[oid].Item2], parent.CargoTable[parent.ContratoTable[oid].Item3]);
            }

            parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?>(parent.ContratoTable[oid].Item1, parent.ContratoTable[oid].Item2, parent.ContratoTable[oid].Item3, parent.ContratoTable[oid].Item4, finVigencia);
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCaducarContratoAsync(responsable, oid, finVigencia);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Caducidad de contrato no modificada: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Caducidad de contrato no modificada: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?>(parent.ContratoTable[oid].Item1, parent.ContratoTable[oid].Item2, parent.ContratoTable[oid].Item3, parent.ContratoTable[oid].Item4, oldFinVigencia);
        }

        public override void Aplicar(Form1 parent)
        {
            parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?>(parent.ContratoTable[oid].Item1, parent.ContratoTable[oid].Item2, parent.ContratoTable[oid].Item3, parent.ContratoTable[oid].Item4, finVigencia);
        }
    }
}
