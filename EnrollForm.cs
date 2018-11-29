using DevExpress.XtraEditors.Controls;
using DevExpress.XtraWizard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnroladorStandAlone
{
    public partial class EnrollForm : Form
    {
        private Form1 parent;
        private string RUT;
        private BindingList<Tuple<string, string, string, DateTime, DateTime?, Guid, TipoAccion?>> bnlContratos = new BindingList<Tuple<string, string, string, DateTime, DateTime?, Guid, EnrollForm.TipoAccion?>>();
        private BindingList<Tuple<string, bool, TipoHuella, Guid?>> bnlHuellas = new BindingList<Tuple<string, bool, TipoHuella, Guid?>>();
        private BindingList<Tuple<string, string, string>> bnlAsignaciones = new BindingList<Tuple<string, string, string>>();
        private BindingList<Guid> bnlAsignacionesTemporales = new BindingList<Guid>();
        private BindingList<Tuple<string, string, string, Guid>> bnlNuevaAsignacion = new BindingList<Tuple<string, string, string, Guid>>();
        private bool enrolando = false;
        private int fingers = 0;
        private TipoHuella enrolledFingerType;
        private string enrolledFingerData;

        private Guid? actualizarHuella = null;      // Indica si la huella que se está enrolando está actualizando una huella ya registrada

        private WizardPage nextPage = null;

        private Stack<Tuple<Accion, TipoAccion>> accionesActuales = new Stack<Tuple<Accion, TipoAccion>>(); // Accion, TipoAccion
        private List<Accion> acciones = new List<Accion>();
        public List<Accion> Acciones { get { return acciones; } }

        private Accion accionEditada = null;
        private Guid caducandoContrato;

        public EnrollForm(Form1 parent, List<Accion> acciones, string rut)
        {
            InitializeComponent();
            this.parent = parent;
            gdcContratos.DataSource = bnlContratos;
            gdcHuellas.DataSource = bnlHuellas;
            gdcAsignaciones.DataSource = bnlAsignaciones;
            gdcNuevaAsignacion.DataSource = bnlNuevaAsignacion;
            parent.Huellero.FingerFeature += Huellero_FingerFeature;
            parent.Huellero.Enrolled += Huellero_Enrolled;

            foreach (Accion accion in acciones)
            {
                this.acciones.Add(accion.Clonar());
            }
            if (!string.IsNullOrEmpty(rut))
            {
                txtRUT.Text = rut;
                wzrEnroll.SetNextPage();
            }
        }

        private void EnrollForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Huellero.FingerFeature -= Huellero_FingerFeature;
            parent.Huellero.Enrolled -= Huellero_Enrolled;
            if (DialogResult == DialogResult.Cancel)
            {
                foreach (Tuple<Accion, TipoAccion> accion in accionesActuales)
                {
                    if (accion.Item2 != TipoAccion.Eliminada)
                    {
                        accion.Item1.Cancelar(parent);
                    }
                }
                acciones.Clear();
            }
        }

        #region PageInits
        private void wpRUT_PageInit(object sender, EventArgs e)
        {
            txtRUT.Focus();
        }

        private void wpNuevoEmpleado_PageInit(object sender, EventArgs e)
        {
            txtNuevoNombre.Focus();
            chkNuevoClave.Checked = false;
        }

        private void chkNuevoClave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNuevoClave.Checked && MessageBox.Show("Al crear una clave se desactivará la creación de huellas. Crear clave?", "Crear clave", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                chkNuevoClave.CheckedChanged -= chkEditarClave_CheckedChanged;
                chkNuevoClave.Checked = false;
                chkNuevoClave.CheckedChanged += chkEditarClave_CheckedChanged;
            }

            txtNuevoClave.Enabled = chkNuevoClave.Checked;
            if (!chkNuevoClave.Checked)
            {
                txtNuevoClave.Text = "";
            }
        }

        private void wpEditarEmpleado_PageInit(object sender, EventArgs e)
        {
            txtEditarNombre.Focus();
            var empleado = parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]];
            txtEditarNombre.Text = empleado.Item4;
            txtEditarApellidos.Text = empleado.Item5;

            chkEditarClave.CheckedChanged -= chkEditarClave_CheckedChanged;
            if (empleado.Item3)
            {
                chkEditarClave.Checked = true;
                txtEditarClave.Enabled = true;
                txtEditarClave.Text = ((AccionCrearEmpleado)accionEditada).Contraseña;
            }
            else
            {
                chkEditarClave.Checked = false;
                txtEditarClave.Enabled = false;
                txtEditarClave.Text = "";
            }
            chkEditarClave.CheckedChanged += chkEditarClave_CheckedChanged;
        }

        private void chkEditarClave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEditarClave.Checked && MessageBox.Show("Al crear una clave se desactivará la creación de huellas. Crear clave?", "Crear clave", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                chkEditarClave.CheckedChanged -= chkEditarClave_CheckedChanged;
                chkEditarClave.Checked = false;
                chkEditarClave.CheckedChanged += chkEditarClave_CheckedChanged;
            }

            txtEditarClave.Enabled = chkEditarClave.Checked;
            if (chkEditarClave.Checked)
            {
                txtEditarClave.Text = ((AccionCrearEmpleado)accionEditada).Contraseña;
            }
            else
            {
                txtEditarClave.Text = "";
            }
        }

        private void wpMostrarEmpleado_PageInit(object sender, EventArgs e)
        {
            txtMostrarNombre.Focus();
            var empleado = parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]];
            txtMostrarNombre.Text = empleado.Item4;
            txtMostrarApellidos.Text = empleado.Item5;

            chkMostrarClave.CheckedChanged -= chkMostrarClave_CheckedChanged;
            if (empleado.Item3)
            {
                chkMostrarClave.Checked = true;
                txtMostrarClave.Enabled = true;
                if (accionEditada != null && !string.IsNullOrEmpty(((AccionModificarContraseña)accionEditada).Contraseña))
                {
                    txtMostrarClave.Text = ((AccionModificarContraseña)accionEditada).Contraseña;
                }
                else
                {
                    txtMostrarClave.Text = "****";
                }
            }
            else
            {
                chkMostrarClave.Checked = false;
                txtMostrarClave.Enabled = false;
                txtMostrarClave.Text = "";
            }
            chkMostrarClave.CheckedChanged += chkMostrarClave_CheckedChanged;
        }

        private void chkMostrarClave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMostrarClave.Checked && MessageBox.Show("Al crear una clave se desactivará la creación de huellas. Crear clave?", "Crear clave", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                chkMostrarClave.CheckedChanged -= chkEditarClave_CheckedChanged;
                chkMostrarClave.Checked = false;
                chkMostrarClave.CheckedChanged += chkEditarClave_CheckedChanged;
            }

            txtMostrarClave.Enabled = chkMostrarClave.Checked;
            if (chkMostrarClave.Checked)
            {
                if (accionEditada != null && !string.IsNullOrEmpty(((AccionModificarContraseña)accionEditada).Contraseña))
                {
                    txtMostrarClave.Text = ((AccionModificarContraseña)accionEditada).Contraseña;
                }
                else if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3)
                {
                    txtMostrarClave.Text = "****";
                }
            }
            else
            {
                txtMostrarClave.Text = "";
            }
        }

        private void wpNuevoContrato_PageInit(object sender, EventArgs e)
        {
            cmbNuevoEmpresa.SelectedIndexChanged -= cmbNuevoEmpresa_SelectedIndexChanged;

            cmbNuevoEmpresa.Focus();
            ComboBoxItemCollection coll = cmbNuevoEmpresa.Properties.Items;
            coll.BeginUpdate();
            coll.Clear();
            foreach (KeyValuePair<Guid, Tuple<string, List<Guid>, List<Guid>>> empresa in parent.EmpresaTable)
            {
                coll.Add(new ComboBoxItem(empresa.Key, empresa.Value.Item1));
            }
            coll.EndUpdate();
            cmbNuevoEmpresa.SelectedIndex = -1;

            coll = cmbNuevoCuenta.Properties.Items;
            coll.Clear();
            coll = cmbNuevoCargo.Properties.Items;
            coll.Clear();
            cmbNuevoCuenta.SelectedIndex = -1;
            cmbNuevoCuenta.Enabled = false;
            cmbNuevoCargo.SelectedIndex = -1;
            cmbNuevoCargo.Enabled = false;

            cmbNuevoEmpresa.SelectedIndexChanged += cmbNuevoEmpresa_SelectedIndexChanged;

            dteNuevoInicioVigencia.DateTime = DateTime.Today;
        }

        private void cmbNuevoEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItemCollection collCuenta = cmbNuevoCuenta.Properties.Items;
            ComboBoxItemCollection collCargo = cmbNuevoCargo.Properties.Items;
            if (cmbNuevoEmpresa.SelectedIndex == -1)
            {
                collCuenta.Clear();
                collCargo.Clear();
                cmbNuevoCuenta.SelectedIndex = -1;
                cmbNuevoCuenta.Enabled = false;
                cmbNuevoCargo.SelectedIndex = -1;
                cmbNuevoCargo.Enabled = false;
            }
            else
            {
                Guid empresa = ((ComboBoxItem)cmbNuevoEmpresa.SelectedItem).Oid;
                collCuenta.BeginUpdate();
                collCuenta.Clear();
                foreach (KeyValuePair<Guid, Tuple<string, DateTime?>> cuenta in parent.CuentaTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item3.Contains(cuenta.Key))
                    {
                        collCuenta.Add(new ComboBoxItem(cuenta.Key, cuenta.Value.Item1));
                    }
                }
                collCuenta.EndUpdate();
                collCargo.BeginUpdate();
                collCargo.Clear();
                foreach (KeyValuePair<Guid, string> cargo in parent.CargoTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item2.Contains(cargo.Key))
                    {
                        collCargo.Add(new ComboBoxItem(cargo.Key, cargo.Value));
                    }
                }
                collCargo.EndUpdate();
                cmbNuevoCuenta.SelectedIndex = -1;
                cmbNuevoCuenta.Enabled = true;
                cmbNuevoCargo.SelectedIndex = -1;
                cmbNuevoCargo.Enabled = true;
            }
        }

        private void wpEditarContrato_PageInit(object sender, EventArgs e)
        {
            cmbEditarEmpresa.SelectedIndexChanged -= cmbEditarEmpresa_SelectedIndexChanged;
            cmbEditarEmpresa.Focus();
            ComboBoxItemCollection coll = cmbEditarEmpresa.Properties.Items;
            coll.BeginUpdate();
            coll.Clear();
            ComboBoxItem select = null;
            foreach (KeyValuePair<Guid, Tuple<string, List<Guid>, List<Guid>>> empresa in parent.EmpresaTable)
            {
                ComboBoxItem item = new ComboBoxItem(empresa.Key, empresa.Value.Item1);
                coll.Add(item);
                if (empresa.Key.Equals(((AccionCrearContrato)accionEditada).Empresa))
                {
                    select = item;
                }
            }
            coll.EndUpdate();
            cmbEditarEmpresa.SelectedItem = select;

            ComboBoxItemCollection collCuenta = cmbEditarCuenta.Properties.Items;
            ComboBoxItemCollection collCargo = cmbEditarCargo.Properties.Items;
            if (select == null)
            {
                collCuenta.Clear();
                collCargo.Clear();
                cmbEditarCuenta.SelectedIndex = -1;
                cmbEditarCuenta.Enabled = false;
                cmbEditarCargo.SelectedIndex = -1;
                cmbEditarCargo.Enabled = false;
            }
            else
            {
                Guid empresa = select.Oid;
                select = null;
                collCuenta.BeginUpdate();
                collCuenta.Clear();
                foreach (KeyValuePair<Guid, Tuple<string, DateTime?>> cuenta in parent.CuentaTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item3.Contains(cuenta.Key))
                    {
                        ComboBoxItem item = new ComboBoxItem(cuenta.Key, cuenta.Value.Item1);
                        collCuenta.Add(item);
                        if (cuenta.Key.Equals(((AccionCrearContrato)accionEditada).Cuenta))
                        {
                            select = item;
                        }
                    }
                }
                collCuenta.EndUpdate();
                cmbEditarCuenta.SelectedItem = select;
                cmbEditarCuenta.Enabled = true;

                select = null;
                collCargo.BeginUpdate();
                collCargo.Clear();
                foreach (KeyValuePair<Guid, string> cargo in parent.CargoTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item2.Contains(cargo.Key))
                    {
                        collCargo.Add(new ComboBoxItem(cargo.Key, cargo.Value));
                        ComboBoxItem item = new ComboBoxItem(cargo.Key, cargo.Value);
                        collCargo.Add(item);
                        if (cargo.Key.Equals(((AccionCrearContrato)accionEditada).Cargo))
                        {
                            select = item;
                        }
                    }
                }
                collCargo.EndUpdate();
                cmbEditarCargo.SelectedItem = select;
                cmbEditarCargo.Enabled = true;
            }

            cmbEditarEmpresa.SelectedIndexChanged += cmbEditarEmpresa_SelectedIndexChanged;

            dteEditarInicioVigencia.DateTime = ((AccionCrearContrato)accionEditada).InicioVigencia;
        }

        private void cmbEditarEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItemCollection collCuenta = cmbEditarCuenta.Properties.Items;
            ComboBoxItemCollection collCargo = cmbEditarCargo.Properties.Items;
            if (cmbEditarEmpresa.SelectedIndex == -1)
            {
                collCuenta.Clear();
                collCargo.Clear();
                cmbEditarCuenta.SelectedIndex = -1;
                cmbEditarCuenta.Enabled = false;
                cmbEditarCargo.SelectedIndex = -1;
                cmbEditarCargo.Enabled = false;
            }
            else
            {
                Guid empresa = ((ComboBoxItem)cmbEditarEmpresa.SelectedItem).Oid;
                collCuenta.BeginUpdate();
                collCuenta.Clear();
                foreach (KeyValuePair<Guid, Tuple<string, DateTime?>> cuenta in parent.CuentaTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item3.Contains(cuenta.Key))
                    {
                        collCuenta.Add(new ComboBoxItem(cuenta.Key, cuenta.Value.Item1));
                    }
                }
                collCuenta.EndUpdate();
                collCargo.BeginUpdate();
                collCargo.Clear();
                foreach (KeyValuePair<Guid, string> cargo in parent.CargoTable)
                {
                    if (parent.EmpresaTable.ContainsKey(empresa) && parent.EmpresaTable[empresa].Item2.Contains(cargo.Key))
                    {
                        collCargo.Add(new ComboBoxItem(cargo.Key, cargo.Value));
                    }
                }
                collCargo.EndUpdate();
                cmbEditarCuenta.SelectedIndex = -1;
                cmbEditarCuenta.Enabled = true;
                cmbEditarCargo.SelectedIndex = -1;
                cmbEditarCargo.Enabled = true;
            }
        }

        private void wpCaducarContrato_PageInit(object sender, EventArgs e)
        {
            dteCaducarContrato.DateTime = DateTime.Today;
        }

        private void wpMostrarContratos_PageInit(object sender, EventArgs e)
        {
            RecargarContratos();
            gridView1.Focus();
        }

        private void RecargarContratos()
        {
            gdcContratos.BeginUpdate();
            bnlContratos.Clear();
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                foreach (Guid contrato in parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item3)
                {
                    if (parent.ContratoTable.ContainsKey(contrato))
                    {
                        var contratoItem = parent.ContratoTable[contrato];
                        Guid empresa = contratoItem.Item1;
                        Guid cuenta = contratoItem.Item2;
                        Guid cargo = contratoItem.Item3;
                        DateTime inicio = contratoItem.Item4;
                        DateTime? fin = contratoItem.Item5;
                        if (parent.EmpresaTable.ContainsKey(empresa) && parent.CuentaTable.ContainsKey(cuenta) && parent.CargoTable.ContainsKey(cargo))
                        {
                            TipoAccion? tipoAccion = null;
                            foreach (Accion accion in acciones)
                            {
                                if (accion is AccionCrearContrato && ((AccionCrearContrato)accion).Oid.Equals(contrato))
                                {
                                    tipoAccion = TipoAccion.Nueva;
                                    break;
                                }
                                if (accion is AccionCaducarContrato)
                                {
                                    AccionCaducarContrato accion2 = (AccionCaducarContrato)accion;
                                    if (accion2.Oid.Equals(contrato))
                                    {
                                        tipoAccion = accion2.FinVigencia.HasValue ? TipoAccion.Eliminada : TipoAccion.Modificada;
                                        break;
                                    }
                                }
                            }
                            bnlContratos.Add(new Tuple<string, string, string, DateTime, DateTime?, Guid, TipoAccion?>(parent.EmpresaTable[empresa].Item1, parent.CuentaTable[cuenta].Item1, parent.CargoTable[cargo], inicio, fin, contrato, tipoAccion));
                        }
                    }
                }
            }
            gdcContratos.EndUpdate();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            btnEliminarContrato.Enabled = false;
            btnEditarContrato.Enabled = false;
            btnCaducarContrato.Text = "Caducar Contrato";

            int[] selection = gridView1.GetSelectedRows();
            if (selection.Length > 0)
            {
                int rowHandle = gridView1.GetSelectedRows().First<int>();
                Guid contrato = (Guid)gridView1.GetRowCellValue(rowHandle, "Item6");
                if (parent.ContratoTable[contrato].Item5.HasValue)
                {
                    btnCaducarContrato.Text = "Eliminar Caducidad";
                }
                if (ContratoRecienCreado(contrato) != null)
                {
                    btnEliminarContrato.Enabled = true;
                    btnEditarContrato.Enabled = true;
                }
            }
        }

        private void btnNuevoContrato_Click(object sender, EventArgs e)
        {
            nextPage = wpNuevoContrato;
            wzrEnroll.SetNextPage();
        }

        private void btnEliminarContrato_Click(object sender, EventArgs e)
        {
            int[] selection = gridView1.GetSelectedRows();
            if (selection.Length > 0)
            {
                int rowHandle = selection.First<int>();
                Guid contrato = (Guid)gridView1.GetRowCellValue(rowHandle, "Item6");
                AccionCrearContrato accion = ContratoRecienCreado(contrato);
                if (accion != null)
                {
                    EliminarAccion(accion);
                    RecargarContratos();
                }
            }
        }

        private void btnCaducarContrato_Click(object sender, EventArgs e)
        {
            int[] selection = gridView1.GetSelectedRows();
            if (selection.Length > 0)
            {
                int rowHandle = selection.First<int>();
                caducandoContrato = (Guid)gridView1.GetRowCellValue(rowHandle, "Item6");
                
                if (parent.ContratoTable[caducandoContrato].Item5.HasValue)
                {
                    AccionCrearContrato accionCrearContrato = ContratoRecienCreado(caducandoContrato);
                    //dteCaducarContrato.Properties.MinValue = Cuenta.Name accionCrearContrato.Cuenta. accionCrearContrato.InicioVigencia; 
                    if (accionCrearContrato != null)
                    {
                        if (accionCrearContrato.Editar(accionCrearContrato.Empresa, accionCrearContrato.Cuenta, accionCrearContrato.Cargo, accionCrearContrato.InicioVigencia, null, parent))
                        {
                            ModificarAccion(accionCrearContrato);
                        }
                    }
                    else
                    {
                        AccionCaducarContrato accionCaducarContrato = ContratoRecienCaducado(caducandoContrato);
                        if (accionCaducarContrato != null)
                        {
                            EliminarAccion(accionCaducarContrato);
                        }
                        else
                        {
                            Accion accion = new AccionCaducarContrato(caducandoContrato, parent.EmpleadoRUTIndex[RUT], null, parent);
                            acciones.Add(accion);
                            accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                        }
                    }
                    RecargarContratos();
                }
                else
                {
                    accionEditada = ContratoRecienCreado(caducandoContrato);
                    if (accionEditada == null)
                    {
                        accionEditada = ContratoRecienCaducado(caducandoContrato);
                    }
                    nextPage = wpCaducarContrato;
                    wzrEnroll.SetNextPage();
                }
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                EnrollForm.TipoAccion? tipo = (EnrollForm.TipoAccion?)gridView1.GetRowCellValue(e.RowHandle, "Item7");
                if (tipo.HasValue)
                {
                    switch (tipo.Value)
                    {
                        case EnrollForm.TipoAccion.Nueva:
                            e.Appearance.BackColor = Color.Green;
                            break;
                        case EnrollForm.TipoAccion.Modificada:
                            e.Appearance.BackColor = Color.Yellow;
                            break;
                        case EnrollForm.TipoAccion.Eliminada:
                            e.Appearance.BackColor = Color.Red;
                            break;
                    }
                }
                else
                {
                    e.Appearance.BackColor = Color.White;
                }
            }
        }

        private void btnEditarContrato_Click(object sender, EventArgs e)
        {
            nextPage = wpEditarContrato;
            wzrEnroll.SetNextPage();
        }

        private void wpNuevaHuella_PageInit(object sender, EventArgs e)
        {
            lblHuellaDescription.Text = string.Format("Debe leer tres veces el dedo {0} del empleado", enrolledFingerType.GetDisplayName());
            wpNuevaHuella.AllowNext = false;
            fingers = 0;
            enrolando = true;
            ActualizaStatusHuellas();

            parent.Huellero.Enroll();
        }

        private void ActualizaStatusHuellas()
        {
            lblHuellaStatus.Text = string.Format("Se han leido {0} huellas", fingers);
        }

        private async void Huellero_Enrolled(object sender, EnrolledEventArgs e)
        {
            if (enrolando)
            {
                if (e.Success)
                {
                    lblHuellaStatus.Text = "Enrolado con éxito";
                    enrolledFingerData = e.Template;
                    wpNuevaHuella.AllowNext = true;
                }
                else
                {
                    lblHuellaStatus.Text = "Error al enrolar. Vuelva atras e intente nuevamente";
                    await parent.Huellero.Sonido(HuelleroSonidos.Incorrecto);
                }
            }
        }

        private async void Huellero_FingerFeature(object sender, FingerFeatureEventArgs e)
        {
            if (enrolando)
            {
                fingers = e.Count;
                ActualizaStatusHuellas();
                await parent.Huellero.Sonido(HuelleroSonidos.Correcto);
            }
        }

        private void wpMostrarHuellas_PageInit(object sender, EventArgs e)
        {
            gdcHuellas.BeginUpdate();
            bnlHuellas.Clear();
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                Dictionary<TipoHuella, Guid> huellas = new Dictionary<TipoHuella, Guid>();
                foreach (Guid huella in parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item1)
                {
                    if (parent.HuellaTable.ContainsKey(huella))
                    {
                        huellas[parent.HuellaTable[huella]] = huella;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    TipoHuella tipoHuella = (TipoHuella)i;
                    Guid? huella = null;
                    if (huellas.ContainsKey(tipoHuella))
                    {
                        huella = huellas[tipoHuella];
                    }
                    bnlHuellas.Add(new Tuple<string, bool, TipoHuella, Guid?>(tipoHuella.GetDisplayName(), huellas.ContainsKey(tipoHuella), tipoHuella, huella));
                }
            }
            gdcHuellas.EndUpdate();
            gridView2.Focus();
        }

        private void btnNuevaHuella_Click(object sender, EventArgs e)
        {
            nextPage = wpNuevaHuella;
            wzrEnroll.SetNextPage();
        }

        private void wpNuevaAsignacion_PageInit(object sender, EventArgs e)
        {
            gdcNuevaAsignacion.BeginUpdate();
            bnlNuevaAsignacion.Clear();
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                List<Guid> asignaciones = parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2;
                foreach (KeyValuePair<Guid, Tuple<string, List<Guid>>> cadena in parent.CadenaTable)
                {
                    foreach (Guid instalacionOid in cadena.Value.Item2)
                    {
                        if (parent.InstalacionTable.ContainsKey(instalacionOid))
                        {
                            Tuple<string, List<Guid>> instalacion = parent.InstalacionTable[instalacionOid];
                            foreach (Guid dispositivoOid in instalacion.Item2)
                            {
                                if (!asignaciones.Contains(dispositivoOid) && parent.DispositivoTable.ContainsKey(dispositivoOid))
                                {
                                    Tuple<string, string, int, TipoDispositivo, List<Guid>> dispositivo = parent.DispositivoTable[dispositivoOid];
                                    //if (dispositivo.Item4 == TipoDispositivo.PuntoAcceso)
                                    {
                                        bnlNuevaAsignacion.Add(new Tuple<string, string, string, Guid>(cadena.Value.Item1, instalacion.Item1, dispositivo.Item1, dispositivoOid));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            gdcNuevaAsignacion.EndUpdate();
        }

        private void wpMostrarAsignaciones_PageInit(object sender, EventArgs e)
        {
            RecargarAsignaciones();
        }

        private void RecargarAsignaciones()
        {
            gdcAsignaciones.BeginUpdate();
            bnlAsignaciones.Clear();
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                //foreach (Guid contrato in parent. EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item3)
                foreach (Guid dispositivo in parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2)
                {
                    if (parent.DispositivoTable.ContainsKey(dispositivo))
                    {
                        var dispositivoItem = parent.DispositivoTable[dispositivo];

                        var instalacion = parent.InstalacionTable.FirstOrDefault(p => p.Value.Item2.Exists(q => q.Equals(dispositivo)));
                        var cadena = parent.CadenaTable.FirstOrDefault(p => p.Value.Item2.Exists(q => q.Equals(instalacion.Key)));

                        bnlAsignaciones.Add(new Tuple<string, string, string>(cadena.Value.Item1, instalacion.Value.Item1, dispositivoItem.Item1));
                    }
                }
            }
            gdcAsignaciones.EndUpdate();
        }

        private void btnNuevaAsignacion_Click(object sender, EventArgs e)
        {
            nextPage = wpNuevaAsignacion;
            wzrEnroll.SetNextPage();
        }

        private void wpResumen_PageInit(object sender, EventArgs e)
        {
            if (accionesActuales.Count == 0)
            {
                memResumen.Text = string.Format("Ninguna acción realizada");
                btnAutorizar.Enabled = false;
            }
            else
            {
                List<string> descripciones = new List<string>(acciones.Count);
                foreach (Tuple<Accion, TipoAccion> accion in accionesActuales)
                {
                    descripciones.Add(string.Format("- {0}{1}", accion.Item2 == TipoAccion.Modificada ? "Modificar: " : accion.Item2 == TipoAccion.Eliminada ? "Eliminar: " : "", accion.Item1.Descripcion));
                }
                memResumen.Text = string.Format("Se realizaron las siguientes acciones:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, descripciones));
                btnAutorizar.Enabled = true;
            }
        }

        private async void btnAutorizar_Click(object sender, EventArgs e)
        {
            //ResultadoAutorizacion res = await parent.PedirAutorizacion("Por favor autorice la creación de las acciones realizadas en el proceso\r\nde enrolamiento", this);
            //if (res == ResultadoAutorizacion.Rechazado)
            //{
            //    MessageBox.Show("Cancelando enrolamiento. Creación de huellas no autorizada", "No autorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    DialogResult = DialogResult.Abort;
            //}
            //else if (res == ResultadoAutorizacion.Aceptado)
            //{
                DialogResult = DialogResult.OK;
            //}
        }
        #endregion

        #region PageValidatings
        private void wpRUT_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                e.Valid = ValidadorRUT.Validar(txtRUT.Text, out RUT);
                if (!e.Valid)
                {
                    e.ErrorText = "RUT invalido";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
            }
            e.Valid = true;
        }

        private void wpNuevoEmpleado_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                if (string.IsNullOrEmpty(txtNuevoNombre.Text))
                {
                    e.ErrorText = "Debe ingresar nombre";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (string.IsNullOrEmpty(txtNuevoApellidos.Text))
                {
                    e.ErrorText = "Debe ingresar apellidos";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (chkNuevoClave.Checked)
                {
                    int n;
                    if (string.IsNullOrEmpty(txtNuevoClave.Text))
                    {
                        e.ErrorText = "Debe ingresar una Clave";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                    else if (!int.TryParse(txtNuevoClave.Text, out n))
                    {
                        e.ErrorText = "Clave invalida";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                }
            }
            e.Valid = true;
        }

        private void wpEditarEmpleado_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                if (string.IsNullOrEmpty(txtEditarNombre.Text))
                {
                    e.ErrorText = "Debe ingresar nombre";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (string.IsNullOrEmpty(txtEditarApellidos.Text))
                {
                    e.ErrorText = "Debe ingresar apellidos";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (chkEditarClave.Checked)
                {
                    int n;
                    if (string.IsNullOrEmpty(txtEditarClave.Text))
                    {
                        e.ErrorText = "Debe ingresar una Clave";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                    else if (!int.TryParse(txtEditarClave.Text, out n))
                    {
                        e.ErrorText = "Clave invalida";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                }
            }
            e.Valid = true;
        }

        private void wpMostrarEmpleado_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                if (chkMostrarClave.Checked)
                {
                    int n;
                    if (string.IsNullOrEmpty(txtMostrarClave.Text))
                    {
                        e.ErrorText = "Debe ingresar una Clave";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                    else if (!int.TryParse(txtMostrarClave.Text, out n) && (!txtMostrarClave.Text.Equals("****") || (accionEditada != null && !string.IsNullOrEmpty(((AccionModificarContraseña)accionEditada).Contraseña)) || !parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3))
                    {
                        e.ErrorText = "Clave invalida";
                        e.ErrorIconType = MessageBoxIcon.Hand;
                        e.Valid = false;
                        return;
                    }
                }
            }
            e.Valid = true;
        }

        private void wpNuevoContrato_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                if (cmbNuevoEmpresa.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar empresa";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (cmbNuevoCuenta.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar cuenta";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (cmbNuevoCargo.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar cargo";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (dteNuevoInicioVigencia.DateTime == null)
                {
                    e.ErrorText = "Debe ingresar inicio de vigencia";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else
                {
                    //Validar en nuevo Contrato la fecha de cierre de la empresa
                    dynamic Oid = cmbNuevoCuenta.EditValue;
                    var aDt = (parent.CuentaTable[Oid.Oid] as Tuple<string, DateTime?>).Item2 as DateTime?;
                    var aDtActual = dteNuevoInicioVigencia.DateTime.Date;
                    if (aDt != null)
                    {
                        if (aDtActual < Convert.ToDateTime(aDt))
                        {
                            e.ErrorText = string.Format("La fecha de inicio de vigencia no puede ser mayor que la de la fecha de cierre de la empresa: {0} ", Convert.ToDateTime(aDt).ToShortDateString());
                            e.ErrorIconType = MessageBoxIcon.Hand;
                            e.Valid = false;
                            return;
                        }
                    }
                }
            }
            e.Valid = true;
        }

        private void wpEditarContrato_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                if (cmbEditarEmpresa.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar empresa";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (cmbEditarCuenta.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar cuenta";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (cmbEditarCargo.SelectedIndex == -1)
                {
                    e.ErrorText = "Debe seleccionar cargo";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else if (dteEditarInicioVigencia.DateTime == null)
                {
                    e.ErrorText = "Debe ingresar inicio de vigencia";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
                else
                {
                    //Validar en nuevo Contrato la fecha de cierre de la empresa
                    dynamic Oid = cmbNuevoCuenta.EditValue;
                    var aDt = (parent.CuentaTable[Oid.Oid] as Tuple<string, DateTime?>).Item2 as DateTime?;
                    var aDtActual = dteNuevoInicioVigencia.DateTime.Date;
                    if (aDt != null)
                    {
                        if (aDtActual < Convert.ToDateTime(aDt))
                        {
                            e.ErrorText = string.Format("La fecha de inicio de vigencia no puede ser mayor que la de la fecha de cierre de la empresa: {0} ", Convert.ToDateTime(aDt).ToShortDateString());
                            e.ErrorIconType = MessageBoxIcon.Hand;
                            e.Valid = false;
                            return;
                        }
                    }
                }
            }
            e.Valid = true;
        }
        private
            void wpMostrarHuellas_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (nextPage == wpNuevaHuella)
            {
                if (gridView2.SelectedRowsCount == 0)
                {
                    nextPage = null;
                    e.ErrorText = "Debe seleccionar una huella para registrar";
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
            }
            else if (e.Direction == Direction.Forward && bnlHuellas.Count<Tuple<string, bool, TipoHuella, Guid?>>((huella) => { return huella.Item2; }) == 0)
            {
                e.ErrorText = "Debe registrar al menos una huella";
                e.ErrorIconType = MessageBoxIcon.Hand;
                e.Valid = false;
                return;
            }
            e.Valid = true;
        }

        private void wpNuevaAsignacion_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            /*if (e.Direction == Direction.Forward && gridView4.SelectedRowsCount == 0)
            {
                e.ErrorText = "Debe seleccionar al menos un dispositivo";
                e.ErrorIconType = MessageBoxIcon.Hand;
                e.Valid = false;
                return;
            }*/
            e.Valid = true;
        }
        #endregion

        #region PageCommits
        private void wpNuevoEmpleado_PageCommit(object sender, EventArgs e)
        {
            Accion accion = new AccionCrearEmpleado(int.Parse(RUT.Substring(0, RUT.Length - 2).Replace(".", "")), RUT, txtNuevoClave.Text, txtNuevoNombre.Text, txtNuevoApellidos.Text, parent);
            acciones.Add(accion);
            accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
        }

        private void wpEditarEmpleado_PageCommit(object sender, EventArgs e)
        {
            AccionCrearEmpleado accionEditadaCrearEmpleado = (AccionCrearEmpleado)accionEditada;
            if (string.IsNullOrEmpty(accionEditadaCrearEmpleado.Contraseña) && !string.IsNullOrEmpty(txtEditarClave.Text))
            {
                EliminarHuellasRecienCreadas(accionEditadaCrearEmpleado.Oid);
            }
            if (accionEditadaCrearEmpleado.Editar(txtEditarClave.Text, txtEditarNombre.Text, txtEditarApellidos.Text, parent))
            {
                ModificarAccion(accionEditadaCrearEmpleado);
            }
        }

        private void wpMostrarEmpleado_PageCommit(object sender, EventArgs e)
        {
            AccionModificarContraseña accionEditadaModificarContraseña = (AccionModificarContraseña)accionEditada;
            if (chkMostrarClave.Checked != parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3)
            {
                // Cambio de estado de clave
                if (accionEditadaModificarContraseña != null)
                {
                    if (!accionEditadaModificarContraseña.TeniaContraseña && !chkMostrarClave.Checked)
                    {
                        // Originalmente no tenía contraseña y volvimos a dejarlo sin contraseña
                        EliminarAccion(accionEditadaModificarContraseña);
                    }
                    else if (string.IsNullOrEmpty(accionEditadaModificarContraseña.Contraseña) && txtMostrarClave.Text.Equals("****"))
                    {
                        // Originalmente tenía contraseña y volvimos a dejarle la original
                        EliminarAccion(accionEditadaModificarContraseña);
                    }
                    else
                    {
                        // Editar accion existente con el nuevo estado
                        if (!accionEditadaModificarContraseña.Editar(txtMostrarClave.Text, parent))
                        {
                            // Estaba borrada con el mismo estado. Se agrega a la lista de acciones y se reaplica en las tablas
                            acciones.Add(accionEditadaModificarContraseña);
                            accionEditadaModificarContraseña.Aplicar(parent);
                        }
                        // Marcar acción como modificada
                        ModificarAccion(accionEditadaModificarContraseña);
                    }
                }
                else
                {
                    // Crear acción nueva
                    Accion accion = new AccionModificarContraseña(parent.EmpleadoRUTIndex[RUT], txtMostrarClave.Text, parent);
                    acciones.Add(accion);
                    accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                }

                if (chkMostrarClave.Checked)
                {
                    EliminarHuellasRecienCreadas(parent.EmpleadoRUTIndex[RUT]);
                }
            }
            else if (chkMostrarClave.Checked)
            {
                // Revisar si se creó una clave diferente
                if ((accionEditadaModificarContraseña == null || string.IsNullOrEmpty(accionEditadaModificarContraseña.Contraseña)) && !txtMostrarClave.Text.Equals("****"))
                {
                    // No había accion anterior o es una acción para eliminar la contraseña borrada y el texto es diferente
                    // al placeholder, entonces es una clave nueva
                    Accion accion = new AccionModificarContraseña(parent.EmpleadoRUTIndex[RUT], txtMostrarClave.Text, parent);
                    acciones.Add(accion);
                    accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                }
                else if (accionEditadaModificarContraseña != null && accionEditadaModificarContraseña.Editar(txtMostrarClave.Text, parent))
                {
                    if (!acciones.Contains(accionEditadaModificarContraseña))
                    {
                        // Estaba borrada. Se agrega a la lista de acciones
                        acciones.Add(accionEditadaModificarContraseña);
                    }
                    // Marcar acción como modificada
                    ModificarAccion(accionEditadaModificarContraseña);
                }
                else if (accionEditadaModificarContraseña != null && !acciones.Contains(accionEditadaModificarContraseña))
                {
                    // Estaba borrada con el mismo estado. Se agrega a la lista de acciones y se reaplica en las tablas
                    acciones.Add(accionEditadaModificarContraseña);
                    accionEditadaModificarContraseña.Aplicar(parent);

                    // Marcar acción como modificada
                    ModificarAccion(accionEditadaModificarContraseña);
                }
            }
        }

        private void wpNuevoContrato_PageCommit(object sender, EventArgs e)
        {
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                Accion accion = new AccionCrearContrato(parent.EmpleadoRUTIndex[RUT], ((ComboBoxItem)cmbNuevoEmpresa.SelectedItem).Oid, ((ComboBoxItem)cmbNuevoCuenta.SelectedItem).Oid, ((ComboBoxItem)cmbNuevoCargo.SelectedItem).Oid, dteNuevoInicioVigencia.DateTime, null, parent);
                acciones.Add(accion);
                accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
            }
            else
            {
                // TODO: Error
            }
        }

        private void wpEditarContrato_PageCommit(object sender, EventArgs e)
        {
            AccionCrearContrato accionEditadaCrearContrato = (AccionCrearContrato)accionEditada;
            if (accionEditadaCrearContrato.Editar(((ComboBoxItem)cmbEditarEmpresa.SelectedItem).Oid, ((ComboBoxItem)cmbEditarCuenta.SelectedItem).Oid, ((ComboBoxItem)cmbEditarCargo.SelectedItem).Oid, dteEditarInicioVigencia.DateTime, accionEditadaCrearContrato.FinVigencia, parent))
            {
                ModificarAccion(accionEditadaCrearContrato);
            }
        }

        private void wpCaducarContrato_PageCommit(object sender, EventArgs e)
        {
            if (accionEditada == null)
            {
                Accion accion = new AccionCaducarContrato(caducandoContrato, parent.EmpleadoRUTIndex[RUT], dteCaducarContrato.DateTime, parent);
                acciones.Add(accion);
                accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
            }
            else if (accionEditada is AccionCrearContrato)
            {
                AccionCrearContrato accionEditadaCrearContrato = (AccionCrearContrato)accionEditada;
                if (accionEditadaCrearContrato.Editar(accionEditadaCrearContrato.Empresa, accionEditadaCrearContrato.Cuenta, accionEditadaCrearContrato.Cargo, accionEditadaCrearContrato.InicioVigencia, dteCaducarContrato.DateTime, parent))
                {
                    ModificarAccion(accionEditadaCrearContrato);
                }
            }
            else if (accionEditada is AccionCaducarContrato)
            {
                AccionCaducarContrato accionEditadaCaducarContrato = (AccionCaducarContrato)accionEditada;
                if (accionEditadaCaducarContrato.Editar(dteCaducarContrato.DateTime, parent))
                {
                    ModificarAccion(accionEditadaCaducarContrato);
                }
            }
        }

        private void wpNuevaHuella_PageCommit(object sender, EventArgs e)
        {
            if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
            {
                if (actualizarHuella.HasValue)
                {
                    Accion accion = new AccionActualizarHuella(actualizarHuella.Value, parent.EmpleadoRUTIndex[RUT], enrolledFingerData, parent);
                    acciones.Add(accion);
                    accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                }
                else
                {
                    Accion accion = new AccionCrearHuella(parent.EmpleadoRUTIndex[RUT], enrolledFingerType, enrolledFingerData, parent);
                    acciones.Add(accion);
                    accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                }
            }
            else
            {
                // TODO: Error
            }
        }

        private void wpNuevaAsignacion_PageCommit(object sender, EventArgs e)
        {
            foreach (int rowHandle in gridView4.GetSelectedRows())
            {
                if (gridView4.IsDataRow(rowHandle))
                {
                    Guid dispositivo = (Guid)gridView4.GetRowCellValue(rowHandle, "Item4");
                    if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]) && parent.DispositivoTable.ContainsKey(dispositivo))
                    {
                        Accion accion = new AccionCrearAsignacion(parent.EmpleadoRUTIndex[RUT], dispositivo, parent);
                        acciones.Add(accion);
                        accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Nueva));
                        bnlAsignacionesTemporales.Add(dispositivo);
                    }
                    else
                    {
                        // TODO: Error
                    }
                }
            }
        }
        #endregion

        private void wzrEnroll_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
        {
            if (nextPage != null)
            {
                if (nextPage == wpNuevaHuella)
                {
                    int rowHandle = gridView2.GetSelectedRows().First<int>();
                    enrolledFingerType = (TipoHuella)gridView2.GetRowCellValue(rowHandle, "Item3");
                    actualizarHuella = (Guid?)gridView2.GetRowCellValue(rowHandle, "Item4");
                }
                else if (nextPage == wpEditarContrato)
                {
                    int rowHandle = gridView1.GetSelectedRows().First<int>();
                    Guid contrato = (Guid)gridView1.GetRowCellValue(rowHandle, "Item6");
                    AccionCrearContrato accion = ContratoRecienCreado(contrato);
                    if (accion != null)
                    {
                        accionEditada = accion;
                    }
                    else
                    {
                        nextPage = wpMostrarContratos;
                    }
                }
                e.Page = nextPage;
                nextPage = null;
            }
            else if (e.Direction == Direction.Forward)
            {
                if (e.PrevPage == wpRUT)
                {
                    // Verifica si existe un empleado con el rut ingresado
                    if (parent.EmpleadoRUTIndex.ContainsKey(RUT) && parent.EmpleadoTable.ContainsKey(parent.EmpleadoRUTIndex[RUT]))
                    {
                        // El empleado ya existe. Revisar si es un empleado recien creado
                        AccionCrearEmpleado accion = EmpleadoRecienCreado();
                        if (accion != null)
                        {
                            // Editar empleado
                            accionEditada = accion;
                            e.Page = wpEditarEmpleado;
                        }
                        else
                        {
                            // Mostrar empleado
                            accionEditada = ClaveRecienModificada();
                            e.Page = wpMostrarEmpleado;
                        }
                    }
                    else
                    {
                        // Crear nuevo empleado
                        e.Page = wpNuevoEmpleado;
                    }
                }
                else if (e.PrevPage == wpNuevoEmpleado)
                {
                    // Crear nuevo contrato
                    e.Page = wpNuevoContrato;
                }
                else if (e.PrevPage == wpEditarEmpleado || e.PrevPage == wpMostrarEmpleado)
                {
                    // Revisar si el empleado tiene contratos
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item3.Count > 0)
                    {
                        // El empleado ya tiene contratos. Mostrar contratos
                        e.Page = wpMostrarContratos;
                    }
                    else
                    {
                        // Crear nuevo contrato
                        e.Page = wpNuevoContrato;
                    }
                }
                else if (e.PrevPage == wpNuevoContrato || e.PrevPage == wpEditarContrato || e.PrevPage == wpCaducarContrato)
                {
                    // El empleado ya creo un contrato. Mostrar contratos
                    e.Page = wpMostrarContratos;
                }
                else if (e.PrevPage == wpMostrarContratos)
                {
                    // Revisar si tiene clave creada
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3)
                    {
                        // Tiene clave creada. Revisar si el empleado tiene asignaciones
                        if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2.Count > 0)
                        {
                            // Mostrar asignaciones
                            e.Page = wpMostrarAsignaciones;
                        }
                        else
                        {
                            // Crear nueva asignacion
                            e.Page = wpNuevaAsignacion;
                        }
                    }
                    else
                    {
                        // No tiene clave. Mostrar huellas
                        e.Page = wpMostrarHuellas;
                    }
                }
                else if (e.PrevPage == wpNuevaHuella)
                {
                    // Mostrar huellas
                    e.Page = wpMostrarHuellas;
                }
                else if (e.PrevPage == wpMostrarHuellas)
                {
                    // Revisar si el empleado tiene asignaciones
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2.Count > 0)
                    {
                        // Mostrar asignaciones
                        e.Page = wpMostrarAsignaciones;
                    }
                    else
                    {
                        // Crear nueva asignacion
                        e.Page = wpNuevaAsignacion;
                    }
                }
                else if (e.PrevPage == wpNuevaAsignacion)
                {
                    // Mostrar asignaciones
                    e.Page = wpMostrarAsignaciones;
                }
                else if (e.PrevPage == wpMostrarAsignaciones)
                {
                    // Mostrar resumen
                    e.Page = wpResumen;
                }
            }
            // Retrocediendo
            else
            {
                if (e.PrevPage == wpNuevoEmpleado || e.PrevPage == wpEditarEmpleado || e.PrevPage == wpMostrarEmpleado)
                {
                    // Volver a RUT
                    e.Page = wpRUT;
                }
                else if (e.PrevPage == wpNuevoContrato)
                {
                    // Tiene contratos?
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item3.Count > 0)
                    {
                        // Mostrar contratos
                        e.Page = wpMostrarContratos;
                    }
                    else
                    {
                        // Revisar si es un empleado recien creado
                        AccionCrearEmpleado accion = EmpleadoRecienCreado();
                        if (accion != null)
                        {
                            // Editar empleado
                            accionEditada = accion;
                            e.Page = wpEditarEmpleado;
                        }
                        else
                        {
                            // Mostrar empleado
                            accionEditada = ClaveRecienModificada();
                            e.Page = wpMostrarEmpleado;
                        }
                    }
                }
                else if (e.PrevPage == wpEditarContrato || e.PrevPage == wpCaducarContrato)
                {
                    // Mostrar contratos
                    e.Page = wpMostrarContratos;
                }
                else if (e.PrevPage == wpMostrarContratos)
                {
                    // Revisar si es un empleado recien creado
                    AccionCrearEmpleado accion = EmpleadoRecienCreado();
                    if (accion != null)
                    {
                        // Editar empleado
                        accionEditada = accion;
                        e.Page = wpEditarEmpleado;
                    }
                    else
                    {
                        // Mostrar empleado
                        accionEditada = ClaveRecienModificada();
                        e.Page = wpMostrarEmpleado;
                    }
                }
                else if (e.PrevPage == wpNuevaHuella)
                {
                    // Cancelar enrolamiento
                    parent.Huellero.CancelarEnrolamiento();

                    // Mostrar huellas
                    e.Page = wpMostrarHuellas;
                }
                else if (e.PrevPage == wpMostrarHuellas)
                {
                    // Mostrar contratos
                    e.Page = wpMostrarContratos;
                }
                else if (e.PrevPage == wpNuevaAsignacion)
                {
                    // Tiene asignaciones?
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2.Count > 0)
                    {
                        // Mostrar asignaciones
                        e.Page = wpMostrarAsignaciones;
                    }
                    else
                    {
                        // Revisar si tiene clave creada
                        if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3)
                        {
                            // Tiene clave creada. Mostrar contratos
                            e.Page = wpMostrarContratos;
                        }
                        else
                        {
                            // No tiene clave. Mostrar huellas
                            e.Page = wpMostrarHuellas;
                        }
                    }
                }
                else if (e.PrevPage == wpMostrarAsignaciones)
                {
                    // Revisar si tiene clave creada
                    if (parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item3)
                    {
                        // Tiene clave creada. Mostrar contratos
                        e.Page = wpMostrarContratos;
                    }
                    else
                    {
                        // No tiene clave. Mostrar huellas
                        e.Page = wpMostrarHuellas;
                    }
                }
            }
        }

        private AccionCrearEmpleado EmpleadoRecienCreado()
        {
            foreach (Accion accion in acciones)
            {
                if (accion is AccionCrearEmpleado && ((AccionCrearEmpleado)accion).RUT.Equals(RUT))
                {
                    return (AccionCrearEmpleado)accion;
                }
            }
            return null;
        }

        private Accion ClaveRecienModificada()
        {
            foreach (Accion accion in acciones)
            {
                if (accion is AccionModificarContraseña && accion.Oid.Equals(parent.EmpleadoRUTIndex[RUT]))
                {
                    return accion;
                }
            }
            // Retornar acciones eliminadas también
            foreach (Tuple<Accion, TipoAccion> accion in accionesActuales)
            {
                if (accion.Item1 is AccionModificarContraseña && accion.Item1.Oid.Equals(parent.EmpleadoRUTIndex[RUT]))
                {
                    return accion.Item1;
                }
            }
            return null;
        }

        private AccionCrearContrato ContratoRecienCreado(Guid contrato)
        {
            foreach (Accion accion in acciones)
            {
                if (accion is AccionCrearContrato && ((AccionCrearContrato)accion).Oid.Equals(contrato))
                {
                    return (AccionCrearContrato)accion;
                }
            }
            return null;
        }

        private AccionCaducarContrato ContratoRecienCaducado(Guid contrato)
        {
            foreach (Accion accion in acciones)
            {
                if (accion is AccionCaducarContrato && ((AccionCaducarContrato)accion).Oid.Equals(contrato))
                {
                    return (AccionCaducarContrato)accion;
                }
            }
            return null;
        }

        private void ModificarAccion(Accion accionModificada)
        {
            bool presente = false;
            Stack<Tuple<Accion, TipoAccion>> newAccionesActuales = new Stack<Tuple<Accion, TipoAccion>>(accionesActuales.Count);
            foreach (Tuple<Accion, TipoAccion> accionActual in accionesActuales.ToArray())
            {
                if (accionActual.Item1.Equals(accionModificada))
                {
                    presente = true;
                }
                else
                {
                    newAccionesActuales.Push(accionActual);
                }
            }

            if (presente)
            {
                accionesActuales = newAccionesActuales;
            }
            accionesActuales.Push(new Tuple<Accion, TipoAccion>(accionModificada, TipoAccion.Modificada));
        }

        private void EliminarHuellasRecienCreadas(Guid empleado)
        {
            List<Accion> remover = new List<Accion>(acciones.Count);
            foreach (Accion accion in acciones)
            {
                if (accion is AccionCrearHuella && ((AccionCrearHuella)accion).Empleado.Equals(empleado))
                {
                    remover.Add(accion);
                }
                else if (accion is AccionActualizarHuella && ((AccionActualizarHuella)accion).Empleado.Equals(empleado))
                {
                    remover.Add(accion);
                }
            }
            foreach (Accion accion in remover)
            {
                EliminarAccion(accion);
            }
        }

        private void EliminarAccion(Accion accion)
        {
            accion.Cancelar(parent);
            acciones.Remove(accion);

            bool presente = false;
            Stack<Tuple<Accion, TipoAccion>> newAccionesActuales = new Stack<Tuple<Accion, TipoAccion>>(accionesActuales.Count);
            foreach (Tuple<Accion, TipoAccion> accionActual in accionesActuales.ToArray())
            {
                if (accionActual.Item1.Equals(accion))
                {
                    presente = true;
                }
                else
                {
                    newAccionesActuales.Push(accionActual);
                }
            }

            if (presente)
            {
                accionesActuales = newAccionesActuales;
            }
            else
            {
                accionesActuales.Push(new Tuple<Accion, TipoAccion>(accion, TipoAccion.Eliminada));
            }
        }

        private class ComboBoxItem : IComparable
        {
            private Guid _oid;
            public Guid Oid { get { return _oid; } }
            private string _nombre;

            public ComboBoxItem(Guid Oid, string Nombre)
            {
                _oid = Oid;
                _nombre = Nombre;
            }

            public int CompareTo(object obj)
            {
                return string.Compare(this._nombre, obj.ToString());
            }

            public override string ToString()
            {
                return _nombre;
            }
        }

        public enum TipoAccion
        {
            Nueva,
            Modificada,
            Eliminada
        }

        private void wpCaducarContrato_PageValidating(object sender, WizardPageValidatingEventArgs e)
        {
            if (e.Direction == Direction.Forward)
            {
                var contrato = gridView1.GetFocusedRow() as Tuple<string, string, string, DateTime, DateTime?, Guid, TipoAccion?>;

                var inicioVigencia = contrato.Item4;
                var fechaCaducidad = dteCaducarContrato.DateTime.Date;

                if (fechaCaducidad <= inicioVigencia) //La caducidad debe de ser mayor que el inicio de la vigencia
                {
                    e.ErrorText = string.Format("El fin de vigencia {0}, debe de ser mayor que el inicio de vigencia {1} del contrato", fechaCaducidad.Date.ToShortDateString(), inicioVigencia.Date.ToShortDateString());
                    e.ErrorIconType = MessageBoxIcon.Hand;
                    e.Valid = false;
                    return;
                }
            }
        }

        private void gridView4_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            


        }

        private void btnEliminarAsignacion_Click(object sender, EventArgs e)
        {
            foreach (int rowHandle in gridView3.GetSelectedRows())
            {
                if (gridView3.IsDataRow(rowHandle))
                {
                    var nombreDispositivo = gridView3.GetRowCellValue(rowHandle, "Item3").ToString();

                    var dispositivo = parent.DispositivoTable.FirstOrDefault(p => p.Value.Item1 == nombreDispositivo);

                    if (bnlAsignacionesTemporales.FirstOrDefault(p => p == dispositivo.Key) != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        bnlAsignacionesTemporales.Remove(dispositivo.Key);
                        bnlAsignaciones.Remove(bnlAsignaciones.FirstOrDefault(p => p.Item1 == gridView3.GetRowCellValue(rowHandle, "Item1").ToString()
                                    && p.Item2 == gridView3.GetRowCellValue(rowHandle, "Item2").ToString()
                                    && p.Item3 == gridView3.GetRowCellValue(rowHandle, "Item3").ToString()));

                        dispositivo.Value.Item5.Remove(parent.EmpleadoRUTIndex[RUT]);
                        parent.EmpleadoTable[parent.EmpleadoRUTIndex[RUT]].Item6.Item2.Remove(dispositivo.Key);
                        //parent.EmpleadoTable[]
                    }
                }
            }
        }

        private void gridView3_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            btnEliminarAsignacion.Enabled = false;

            if (e.FocusedRowHandle < 0)
            {
                return;
            }

            int rowHandle = gridView3.GetSelectedRows().First<int>();

            var nombreDispositivo = gridView3.GetRowCellValue(rowHandle, "Item3".ToString());

            var dispositivo = parent.DispositivoTable.FirstOrDefault(p => p.Value.Item1 == nombreDispositivo);

            if (
                    (acciones.FirstOrDefault(p => (p is AccionCrearAsignacion) && (((AccionCrearAsignacion)p)).Oid.Equals(dispositivo.Key)) != null)
                ||
                    (bnlAsignacionesTemporales.FirstOrDefault(p => p == dispositivo.Key) != new Guid("00000000-0000-0000-0000-000000000000"))
                )
            {
                btnEliminarAsignacion.Enabled = true;
            }
        }
    }
}
