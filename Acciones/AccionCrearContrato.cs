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
    class AccionCrearContrato : Accion
    {
        Guid empleado;
        Guid empresa;
        Guid cuenta;
        Guid cargo;
        DateTime inicioVigencia;
        DateTime? finVigencia;
        bool consideraCasino = false;
        bool consideraColacion = false;
        string codigoContrato;
        public Guid Empleado { get { return empleado; } }
        public Guid Empresa { get { return empresa; } }
        public Guid Cuenta { get { return cuenta; } }
        public Guid Cargo { get { return cargo; } }
        public DateTime InicioVigencia { get { return inicioVigencia; } }
        public DateTime? FinVigencia { get { return finVigencia; } }
        public string CodigoContrato { get { return codigoContrato; } }
        public bool ManejaCasino{ get { return consideraCasino; } }
        public bool ManejaColacion{ get { return consideraColacion; } }

        public AccionCrearContrato(Guid empleado, Guid empresa, Guid cuenta, Guid cargo, DateTime inicioVigencia, DateTime? finVigencia, string pcodigoContrato, bool pConsideraColacion, bool pConsideraCasino, Form1 parent)
            : base(parent.LoggedUser.Item1, DateTime.Now, Guid.NewGuid())
        {
            this.empleado = empleado;
            this.empresa = empresa;
            this.cuenta = cuenta;
            this.cargo = cargo;
            this.inicioVigencia = inicioVigencia;
            this.finVigencia = finVigencia;
            this.codigoContrato = pcodigoContrato;
            this.consideraColacion = pConsideraColacion; 
            this.consideraCasino = pConsideraCasino;
            descripcion = string.Format("Crear contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3}. Inicio de vigencia {4}{5}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[empresa].Item1, parent.CuentaTable[cuenta], parent.CargoTable[cargo], inicioVigencia.ToString("dd/MM/yyyy"), finVigencia.HasValue ? ". Fin de vigencia " + finVigencia.Value.ToString("dd/MM/yyyy") : "");

            Aplicar(parent);
        }

        protected AccionCrearContrato(AccionCrearContrato original)
            : base(original.responsable, original.fecha, original.oid)
        {
            empleado = original.empleado;
            empresa = original.empresa;
            cuenta = original.cuenta;
            cargo = original.cargo;
            inicioVigencia = original.inicioVigencia;
            finVigencia = original.finVigencia;
            descripcion = original.descripcion;
            codigoContrato = original.codigoContrato;
            consideraColacion = original.consideraColacion;
            consideraCasino = original.consideraCasino;

        }

        public override Accion Clonar()
        {
            return new AccionCrearContrato(this);
        }

        public bool Editar(Guid empresa, Guid cuenta, Guid cargo, DateTime inicioVigencia, DateTime? finVigencia, string pcodigoContrato, bool pManejaColacion, bool pManejaCasino, Form1 parent)
        {
            if (this.empresa.Equals(empresa) && this.cuenta.Equals(cuenta) && this.cargo.Equals(cargo) && this.inicioVigencia.Equals(inicioVigencia) && this.finVigencia.Equals(finVigencia))
            {
                return false;
            }
            this.empresa = empresa;
            this.cuenta = cuenta;
            this.cargo = cargo;
            this.inicioVigencia = inicioVigencia;
            this.finVigencia = finVigencia;
            this.codigoContrato = pcodigoContrato;
            this.consideraCasino = pManejaCasino;
            this.consideraColacion = pManejaColacion;
            descripcion = string.Format("Crear contrato al empleado con RUT {0} con la empresa {1}, cuenta {2} y cargo {3}. Inicio de vigencia {4}{5}. Codigo Contrato {6}", parent.EmpleadoTable[empleado].Item2, parent.EmpresaTable[empresa].Item1, parent.CuentaTable[cuenta], parent.CargoTable[cargo], inicioVigencia.ToString("dd/MM/yyyy"), finVigencia.HasValue ? ". Fin de vigencia " + finVigencia.Value.ToString("dd/MM/yyyy") : "", CodigoContrato);

            parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?, string, Tuple<bool, bool>>(empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato, new Tuple<bool,bool>(consideraColacion, consideraCasino));
            return true;
        }

        public override async Task Enviar()
        {
            try
            {
                string error = await new EnroladorWebServices.EnroladorWebServicesClient().AccionCrearContratoAsync(responsable, oid, empleado, empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato, ManejaColacion, ManejaCasino);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("Contrato no creado: " + error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Contrato no creado: " + ex.Message);
            }
        }

        public override void Cancelar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado))
            {
                parent.EmpleadoTable[empleado].Item5.Item3.Remove(oid);
            }
            parent.ContratoTable.Remove(oid);
        }

        public override void Aplicar(Form1 parent)
        {
            if (parent.EmpleadoTable.ContainsKey(empleado))
            {
                if (!parent.EmpleadoTable[empleado].Item5.Item3.Contains(oid))
                {
                    parent.EmpleadoTable[empleado].Item5.Item3.Add(oid);
                }
                parent.ContratoTable[oid] = new Tuple<Guid, Guid, Guid, DateTime, DateTime?, string, Tuple<bool, bool>>(empresa, cuenta, cargo, inicioVigencia, finVigencia, CodigoContrato, new Tuple<bool, bool>(ManejaColacion, ManejaCasino));
            }
        }
    }
}
